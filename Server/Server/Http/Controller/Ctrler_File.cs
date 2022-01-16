using EmbedIO;
using EmbedIO.Routing;
using EmbedIO.WebApi;
using HttpMultipartParser;
using LiteDB;
using Server.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Http.Controller
{
    /// <summary>
    /// 文件接口
    /// </summary>
    class Ctrler_File : BaseController
    {
        /// <summary>
        /// 新增文件
        /// </summary>
        [Route(HttpVerbs.Post, "/file")]
        public void UploadFile()
        {
            // 找到当前的用户名
            var userId = Token.UserId;

            if (string.IsNullOrEmpty(userId))
            {
                ResponseError("需要登陆才能上传");
                return;
            }

            // 获取文件流
            var parser = MultipartFormDataParser.Parse(Request.InputStream);

            if (parser.Files.Any() == false)
                throw new Exception("Invalid request, you need to post a file");

            // 获取子目录
            var subPaths = parser.Parameters.Where(p => p.Name == Fields.subPath);

            string subPath = "files";
            if (subPaths.Count() > 0) subPath = subPaths.First().Data;

            var fs = LiteDb.Database.FileStorage;

            List<string> fileIds = new List<string>();

            // 可能会同时上传多个文件
            foreach (var file in parser.Files)
            {
                // 获取文件名
                var fileId = $"_{userId}/{subPath}/{file.FileName}";
                // 保存到数据库中
                fs.Upload(fileId, file.FileName, file.Data);
                fileIds.Add(fileId);
            }

            // 返回id
            ResponseSuccess(fileIds);
        }

        [Route(HttpVerbs.Get, "/file")]
        public void DownloadFile([QueryField] string fileId)
        {
            if (string.IsNullOrEmpty(fileId))
            {
                ResponseError("fileId 为空");
                return;
            }

            var fs = LiteDb.Database.FileStorage;
            var file = fs.FindById(fileId);

            using (var stream = HttpContext.OpenResponseStream())
            {
                // https://blog.csdn.net/yudldl/article/details/83095523
                HttpContext.Response.ContentType = "arraybuffer";
                file.CopyTo(stream);
            }
                
        }
    }
}
