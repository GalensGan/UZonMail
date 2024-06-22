using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Uamazing.Utils.Web.Extensions;
using Uamazing.Utils.Web.ResponseModel;
using UZonMailService.Models.SQL;
using UZonMailService.Models.SQL.Emails;
using UZonMailService.Models.SQL.Files;
using UZonMailService.Services.Files;
using UZonMailService.Services.Settings;
using UZonMailService.Utils.ASPNETCore.PagingQuery;

namespace UZonMailService.Controllers.Files
{
    /// <summary>
    /// 文件控制器
    /// </summary>
    public class FileController(SqlContext db, FileStoreService fileStoreService, TokenService tokenService, IWebHostEnvironment env) : ControllerBaseV1
    {
        /// <summary>
        /// 获取文件ID
        /// </summary>
        /// <returns></returns>
        [HttpGet("file-id")]
        public async Task<ResponseResult<int>> GetFileId(string sha256, string fileName)
        {
            // 判断是否存在 fileObject
            FileObject? fileObject = await fileStoreService.GetExistFileObject(sha256);
            if (fileObject == null) return (-1).ToSuccessResponse();

            int userId = tokenService.GetIntUserId();

            // 获取 fileUsageId
            FileUsage fileUsage = await fileStoreService.GetOrCreateFileUsage(userId, fileName, sha256);
            return fileUsage.Id.ToSuccessResponse();
        }

        /// <summary>
        /// 上传文件对象
        /// </summary>
        /// <param name="fileParams"></param>
        /// <returns></returns>
        [HttpPost("upload-file-object")]
        public async Task<ResponseResult<int>> UploadFileObject(ObjectFileUploaderBody fileParams)
        {
            int userId = tokenService.GetIntUserId();
            fileParams.File ??= Request.Form.Files.FirstOrDefault();
            FileUsage fileUsage = await fileStoreService.UploadFileObject(userId, fileParams);
            return fileUsage.Id.ToSuccessResponse();
        }

        /// <summary>
        /// 获取公共的文件流
        /// 该接口不需要权限即可访问
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("public-file-stream/{fileUsageId}")]
        public async Task<IActionResult> GetPublicFileStream(int fileUsageId)
        {
            // 获取文件流
            string fullPath = await fileStoreService.GetFileFullPath(fileUsageId, true);
            Stream stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
            var result = new FileStreamResult(stream, "application/octet-stream")
            {
                FileDownloadName = Path.GetFileName(fullPath)
            };
            return result;
        }

        /// <summary>
        /// 获取文件流
        /// </summary>
        /// <param name="fileUsageId"></param>
        /// <returns></returns>
        [HttpGet("file-stream/{fileUsageId}")]
        public async Task<IActionResult> GetFileStream(int fileUsageId)
        {
            // 获取文件流
            string fullPath = await fileStoreService.GetFileFullPath(fileUsageId, false);
            Stream stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
            var result = new FileStreamResult(stream, "application/octet-stream")
            {
                FileDownloadName = Path.GetFileName(fullPath)
            };
            return result;
        }

        /// <summary>
        /// 上传到静态目录位置
        /// </summary>
        /// <param name="fileParams"></param>
        /// <returns></returns>
        [HttpPost("upload-static-file")]
        public ResponseResult<string> UploadToStaticFile(StaticFileUploaderBody fileParams)
        {
            int userId = tokenService.GetIntUserId();
            var (fullPath, relativePath) = fileStoreService.GenerateStaticFilePath(userId.ToString(), fileParams.SubPath, fileParams.File.FileName);

            using var stream = new FileStream(fullPath, FileMode.Create);
            fileParams.File.CopyTo(stream);
            return relativePath.ToSuccessResponse();
        }

        /// <summary>
        /// 获取文件数量
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet("file-usages/filtered-count")]
        public async Task<ResponseResult<int>> GetFileUsagesCount(string? filter)
        {
            int userId = tokenService.GetIntUserId();
            // 收件箱
            var dbSet = db.FileUsages.Where(x => x.OwnerUserId == userId);
            if (!string.IsNullOrEmpty(filter))
            {
                dbSet = dbSet.Where(x => x.DisplayName.Contains(filter) || x.FileName.Contains(filter));
            }
            int count = await dbSet.CountAsync();
            return count.ToSuccessResponse();
        }

        /// <summary>
        /// 获取文件数据
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="pagination"></param>
        /// <returns></returns>
        [HttpPost("file-usages/filtered-data")]
        public async Task<ResponseResult<List<FileUsage>>> GetFileUsagesData(string? filter, [FromBody] Pagination pagination)
        {
            int userId = tokenService.GetIntUserId();
            // 收件箱
            var dbSet = db.FileUsages.Where(x => x.OwnerUserId == userId);
            if (!string.IsNullOrEmpty(filter))
            {
                dbSet = dbSet.Where(x => x.DisplayName.Contains(filter) || x.FileName.Contains(filter));
            }
            var results = await dbSet.Include(x => x.FileObject).Page(pagination).ToListAsync();
            return results.ToSuccessResponse();
        }

        /// <summary>
        /// 删除文件记录
        /// </summary>
        /// <param name="fileUsageId"></param>
        /// <returns></returns>
        [HttpDelete("file-usages/{fileUsageId:int}")]
        public async Task<ResponseResult<bool>> DeleteFileUsage(int fileUsageId)
        {
            var fileUsage = await db.FileUsages.FirstOrDefaultAsync(x => x.Id == fileUsageId);
            if (fileUsage == null) return true.ToSuccessResponse();
            db.FileUsages.Remove(fileUsage);

            // 若文件不存在 fileUsage引用，则删除原始文件
            var otherFileUsagesCount = await db.FileUsages.Where(x => x.FileObjectId == fileUsage.FileObjectId && x.Id != fileUsageId).CountAsync();
            if (otherFileUsagesCount == 0)
            {
                await fileStoreService.DeleteFileObject(fileUsage.FileObject.Sha256);
            }

            await db.SaveChangesAsync();
            return true.ToSuccessResponse();
        }

        /// <summary>
        /// 更新文件显示名称
        /// </summary>
        /// <param name="fileUsageId"></param>
        /// <param name="displayName"></param>
        /// <returns></returns>
        [HttpPut("file-usages/{fileUsageId:int}/display-name")]
        public async Task<ResponseResult<bool>> UpdateDisplayName(int fileUsageId, [FromQuery]string displayName)
        {
            var fileUsage = await db.FileUsages.FirstOrDefaultAsync(x => x.Id == fileUsageId);
            if (fileUsage == null) return false.ToErrorResponse("文件不存在");

            if (string.IsNullOrWhiteSpace(displayName)) fileUsage.DisplayName = fileUsage.FileName;
            else fileUsage.DisplayName = displayName;
            await db.SaveChangesAsync();

            return true.ToSuccessResponse();
        }
    }
}
