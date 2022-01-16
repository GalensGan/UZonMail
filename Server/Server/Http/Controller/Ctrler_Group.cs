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
    public class Ctrler_Group : BaseControllerAsync
    {

        [Route(HttpVerbs.Get, "/group")]
        public async Task GetGroups([QueryField] string groupType)
        {
            if (string.IsNullOrEmpty(groupType))
            {
                await ResponseErrorAsync("请传递组的类型:[send,receive]");
            };

            var results = LiteDb.Fetch<Group>(g => g.groupType == groupType).ToList();
            await ResponseSuccessAsync(results);
        }

        [Route(HttpVerbs.Post, "/group")]
        public async Task NewGroup()
        {
            var parentId = Body.Value<string>("parentId");
            var name = Body.Value<string>("name");
            var groupType = Body.Value<string>("groupType");
            var description = Body.Value<string>("description");

            var newGroup = new Group()
            {
                userId = Token.UserId,
                parentId = parentId,
                name = name,
                groupType = groupType,
                description = description,
            };

            LiteDb.Insert(newGroup);

            await ResponseSuccessAsync(newGroup);
        }

        [Route(HttpVerbs.Delete, "/groups")]
        public async Task DeleteGroup()
        {
            List<string> ids = Body["groupIds"].ToObject<List<string>>();
            LiteDb.DeleteMany<Group>(g => ids.Contains(g._id));
            await ResponseSuccessAsync(ids);
        }

        // 更新group
        [Route(HttpVerbs.Put, "/groups/{id}")]
        public async Task UpdateGroup(string id)
        {
            // 获取所有待更新的key
            List<string> keys = (Body as JObject).Properties().ToList().ConvertAll(p => p.Name);
            Group group = Body.ToObject<Group>();
            var res = LiteDb.Upsert2(g => g._id == id, group, new Database.Definitions.UpdateOptions(keys));
            await ResponseSuccessAsync(res);
        }

        // 新建邮件
        [Route(HttpVerbs.Post, "/groups/{id}/email")]
        public async Task NewEmail(string id)
        {
            // 根据id获取组
            var group = LiteDb.SingleOrDefault<Group>(g => g._id == id);
            if (group == null)
            {
                await ResponseErrorAsync($"未通过{id}找到组");
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

            await ResponseSuccessAsync(res);
        }

        // 新建多个邮件
        [Route(HttpVerbs.Post, "/groups/{id}/emails")]
        public async Task NewEmails(string id)
        {
            // 获取所有待更新的key
            var group = LiteDb.SingleOrDefault<Group>(g => g._id == id);
            if (group == null)
            {
                await ResponseErrorAsync($"未通过{id}找到组");
                return;
            }
            // 根据key来进行实例化
            if (group.groupType == "send")
            {
                var emailInfos = Body.ToObject<List<SendBox>>();
                emailInfos.ForEach(e => e.groupId = id);
                LiteDb.Database.GetCollection<SendBox>().InsertBulk(emailInfos);
                await ResponseSuccessAsync(emailInfos);
            }
            else
            {
                var emailInfos = Body.ToObject<List<ReceiveBox>>();
                emailInfos.ForEach(e => e.groupId = id);
                LiteDb.Database.GetCollection<ReceiveBox>().InsertBulk(emailInfos);
                await ResponseSuccessAsync(emailInfos);
            }
        }

        // 获取多个邮件
        [Route(HttpVerbs.Get, "/groups/{id}/emails")]
        public async Task GetEmails(string id)
        {
            var group = LiteDb.SingleOrDefault<Group>(g => g._id == id);
            if (group == null)
            {
                await ResponseErrorAsync($"未通过{id}找到组");
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

            await ResponseSuccessAsync(results);
        }

        // 删除单个邮箱
        [Route(HttpVerbs.Delete, "/emails/{id}")]
        public async Task DeleteEmail(string id)
        {
            // 获取所有待更新的key
            LiteDb.Delete<SendBox>(id);
            LiteDb.Delete<ReceiveBox>(id);

            await ResponseSuccessAsync("success");
        }

        // 删除多个邮箱
        [Route(HttpVerbs.Delete, "/groups/{id}/emails")]
        public async Task DeleteEmails(string id)
        {
            var group = LiteDb.SingleOrDefault<Group>($"_id='{id}'");
            if (group == null)
            {
                await ResponseErrorAsync($"未通过{id}找到组");
                return;
            }

            // 删除组id对应的所有邮箱
            LiteDb.DeleteMany<SendBox>($"groupId='{id}'");
            LiteDb.DeleteMany<ReceiveBox>($"groupId='{id}'");

            await ResponseSuccessAsync("success");
        }

        // 修改邮箱
        [Route(HttpVerbs.Put, "/emails/{id}")]
        public async Task ModifyEmail(string id)
        {
            // 根据id判断属于发件还是收件
            var sendbox = LiteDb.FirstOrDefault<SendBox>(s => s._id == id);
            if (sendbox != null)
            {
                var updateData1 = Body.ToObject<SendBox>();
                var result1 = LiteDb.Upsert2(e => e._id == id, updateData1, new UpdateOptions(true) { "_id", "groupId" });
                await ResponseSuccessAsync(result1);
                return;
            }

            // 收件情况
            var receiveBox = LiteDb.FirstOrDefault<ReceiveBox>(r => r._id == id);
            if (receiveBox == null)
            {
                await ResponseErrorAsync($"未找到id:{id}对应的邮箱");
                return;
            }

            var updateData2 = Body.ToObject<ReceiveBox>();
            // 更新
            var result2 = LiteDb.Upsert2(e => e._id == id, updateData2, new UpdateOptions(true) { "_id", "groupId" });
            ResponseSuccessAsync(result2);
        }

        // 修改发件箱设置
        [Route(HttpVerbs.Put, "/emails/{id}/settings")]
        public async Task UpdateSendEmailSettings(string id)
        {
            // 根据id判断属于发件还是收件
            var sendbox = LiteDb.FirstOrDefault<SendBox>(s => s._id == id);
            if (sendbox != null)
            {
                if (sendbox.settings == null)
                {
                    sendbox.settings = new SendBoxSetting();
                }

                sendbox.settings.UpdateObject(Body as JObject);
                LiteDb.Update(sendbox);
                await ResponseSuccessAsync(sendbox);
                return;
            }

            await ResponseErrorAsync($"未找到发件箱:{id}");
        }
    }
}
