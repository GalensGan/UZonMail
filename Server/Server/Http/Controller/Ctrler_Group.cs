using EmbedIO;
using EmbedIO.Routing;
using EmbedIO.WebApi;
using LiteDB;
using Newtonsoft.Json.Linq;
using Server.Database.Definitions;
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
            int parentId = Body.Value<int>("parentId");
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

        [Route(HttpVerbs.Delete, "/groups")]
        public void DeleteGroup()
        {
            List<string> ids = Body["groupIds"].ToObject<List<string>>();
            LiteDb.DeleteMany<Group>(g => ids.Contains(g._id));
            ResponseSuccess(ids);
        }

        // 更新group
        [Route(HttpVerbs.Put, "/groups/{id}")]
        public void UpdateGroup(string id)
        {
            // 获取所有待更新的key
            List<string> keys = (Body as JObject).Properties().ToList().ConvertAll(p => p.Name);
            Group group = Body.ToObject<Group>();
            var res = LiteDb.Upsert2(g => g._id == id, group, new Database.Definitions.UpdateOptions(keys));
            ResponseSuccess(res);
        }

        // 新建邮件
        [Route(HttpVerbs.Post, "/groups/{id}/email")]
        public void NewEmail(string id)
        {
            // 根据id获取组
            var group = LiteDb.SingleOrDefault<Group>(g => g._id == id);
            if (group == null)
            {
                ResponseError($"未通过{id}找到组");
                return;
            }
            // 根据key来进行实例化
            EmailInfo res;
            if (group.groupType == "send")
            {
                var emailInfo = Body.ToObject<SendBox>();
                res = LiteDb.Upsert2(g => g.email == emailInfo.email, emailInfo);
            }
            else
            {
                var emailInfo = Body.ToObject<ReceiveBox>();
                res = LiteDb.Upsert2(g => g.email == emailInfo.email, emailInfo);
            }

            ResponseSuccess(res);
        }

        // 新建多个邮件
        [Route(HttpVerbs.Post, "/groups/{id}/emails")]
        public void NewEmails(string id)
        {
            // 获取所有待更新的key
            var group = LiteDb.SingleOrDefault<Group>(g => g._id == id);
            if (group == null)
            {
                ResponseError($"未通过{id}找到组");
                return;
            }
            // 根据key来进行实例化
            if (group.groupType == "send")
            {
                var emailInfos = Body.ToObject<List<SendBox>>();
                emailInfos.ForEach(e => e.groupId = id);
                LiteDb.Database.GetCollection<SendBox>().InsertBulk(emailInfos);
                ResponseSuccess(emailInfos);
            }
            else
            {
                var emailInfos = Body.ToObject<List<ReceiveBox>>();
                emailInfos.ForEach(e => e.groupId = id);
                LiteDb.Database.GetCollection<ReceiveBox>().InsertBulk(emailInfos);
                ResponseSuccess(emailInfos);
            }
        }

        // 获取多个邮件
        [Route(HttpVerbs.Get, "/groups/{id}/emails")]
        public void GetEmails(string id)
        {
            var group = LiteDb.SingleOrDefault<Group>(g => g._id == id);
            if (group == null)
            {
                ResponseError($"未通过{id}找到组");
                return;
            }

            List<EmailInfo> results = new List<EmailInfo>();
            if (group.groupType == "send")
            {
                var emails = LiteDb.Fetch<SendBox>(e => e.groupId == id).ToList();
                results.AddRange(emails);
            }
            else
            {
                var emails = LiteDb.Fetch<ReceiveBox>(e => e.groupId == id).ToList();
                results.AddRange(emails);
            }

            ResponseSuccess(results);
        }

        // 删除单个邮箱
        [Route(HttpVerbs.Delete, "/emails/{id}")]
        public void DeleteEmail(string id)
        {
            // 获取所有待更新的key
            LiteDb.Delete<SendBox>(id);
            LiteDb.Delete<ReceiveBox>(id);

            ResponseSuccess("success");
        }

        // 删除多个邮箱
        [Route(HttpVerbs.Delete, "/groups/{id}/emails")]
        public void DeleteEmails(string id)
        {
            var group = LiteDb.SingleOrDefault<Group>($"_id={id}");
            if (group == null)
            {
                ResponseError($"未通过{id}找到组");
                return;
            }

            // 删除组id对应的所有邮箱
            LiteDb.DeleteMany<SendBox>($"groupId={id}");
            LiteDb.DeleteMany<ReceiveBox>($"groupId={id}");

            ResponseSuccess("success");
        }

        // 修改邮箱
        [Route(HttpVerbs.Put, "/emails/{id}")]
        public void ModifyEmail(string id)
        {
            // 根据id判断属于发件还是收件
            var sendbox = LiteDb.FirstOrDefault<SendBox>(s => s._id == id);
            if (sendbox != null)
            {
                var updateData1 = Body.ToObject<SendBox>();
                var result1 = LiteDb.Upsert2(e => e._id == id, updateData1, new UpdateOptions(true) { "_id", "groupId" });
                ResponseSuccess(result1);
                return;
            }

            // 收件情况
            var receiveBox = LiteDb.FirstOrDefault<ReceiveBox>(r => r._id == id);
            if (receiveBox == null)
            {
                ResponseError($"未找到id:{id}对应的邮箱");
                return;
            }

            var updateData2 = Body.ToObject<ReceiveBox>();
            // 更新
            var result2 = LiteDb.Upsert2(e => e._id == id, updateData2, new UpdateOptions(true) { "_id", "groupId" });
            ResponseSuccess(result2);
        }
    }
}
