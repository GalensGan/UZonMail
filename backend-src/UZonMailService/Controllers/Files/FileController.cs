using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Uamazing.Utils.Web.Extensions;
using Uamazing.Utils.Web.ResponseModel;
using UZonMailService.Models.SqlLite.Files;
using UZonMailService.Services.Files;
using UZonMailService.Services.Settings;

namespace UZonMailService.Controllers.Files
{
    /// <summary>
    /// 文件控制器
    /// </summary>
    public class FileController(FileStoreService fileStoreService, TokenService tokenService, IWebHostEnvironment env) : ControllerBaseV1
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
    }
}
