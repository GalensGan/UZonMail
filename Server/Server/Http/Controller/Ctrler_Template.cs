using EmbedIO;
using EmbedIO.Routing;
using Server.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Http.Controller
{
    /// <summary>
    /// 模板api
    /// </summary>
    public class Ctrler_Template : BaseController
    {
        // 新建模板
        [Route(HttpVerbs.Post, "/template")]
        public void UploadTemplate()
        {
            // 获取用户名
            var template = Body.ToObject<Template>();
            template.userId = Token.UserId;
            template.createDate = DateTime.Now;

            LiteDb.Insert(template);
            ResponseSuccess(template);
        }

        // 获取所有模板
        [Route(HttpVerbs.Get, "/templates")]
        public void GetTemplates()
        {
            // 获取用户名
            List<Template> results = LiteDb.Fetch<Template>(t => t.userId == Token.UserId);
            ResponseSuccess(results);
        }

        // 删除模板
        [Route(HttpVerbs.Delete, "/template/{id}")]
        public void DeleteTemplates(int id)
        {
            var deleteResult = LiteDb.Delete<Template>(id);
            if (deleteResult) ResponseSuccess(deleteResult);
            else ResponseError("删除失败");
        }
    }
}
