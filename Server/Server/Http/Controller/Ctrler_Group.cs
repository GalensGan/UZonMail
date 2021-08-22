using EmbedIO;
using EmbedIO.Routing;
using EmbedIO.WebApi;
using LiteDB;
using Server.Database.Extensions;
using Server.Database.Models;
using Server.SDK.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Http.Controller
{
    public class Ctrler_Group : BaseController
    {

        [Route(HttpVerbs.Get, "/group")]
        public void GetGroups([QueryField] string groupType)
        {
            if (string.IsNullOrEmpty(groupType))
            {
                ResponseError("请传递组的类型:[send,receive]");
            };

            var results = LiteDb.Fetch<Group>(g => g.groupType == groupType).ToList();
            ResponseSuccess(results);
        }

        [Route(HttpVerbs.Post, "/group")]
        public void NewGroup()
        {
            string parentId = Body.Value<string>("parentId");
            string name = Body.Value<string>("name");
            string groupType = Body.Value<string>("groupType");
            string description = Body.Value<string>("description");

            var newGroup = new Group()
            {                
                userId = Token.UserId,
                parentId = parentId,
                name = name,
                groupType = groupType,
                description = description,
            };

            LiteDb.Insert(newGroup);

            ResponseSuccess(newGroup);
        }

        [Route(HttpVerbs.Delete, "/group")]
        public void DeleteGroup()
        {
            List<string> ids = Body["groupIds"].ToObject<List<string>>();
            LiteDb.DeleteMany<Group>(g=>ids.Contains(g.id));
            ResponseSuccess(ids);
        }

        [Route(HttpVerbs.Put, "/groups/{id}")]
        public void UpdateGroup(string id)
        {
            // 获取所有待更新的key
            List<string> keys = Body.Properties().ToList().ConvertAll(p => p.Name);
            Group group =  Body.ToObject<Group>();
            var res = LiteDb.Upsert2(g => g.id == id, group, new Database.Definitions.UpdateOptions(keys));
            ResponseSuccess(res);
        }
    }
}
