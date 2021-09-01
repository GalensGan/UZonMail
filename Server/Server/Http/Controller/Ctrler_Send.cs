using EmbedIO;
using EmbedIO.Routing;
using Newtonsoft.Json.Linq;
using Server.Database;
using Server.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Server.Http.Definitions;
using Newtonsoft.Json;
using Server.Http.Modules.SendEmail;

namespace Server.Http.Controller
{
    /// <summary>
    /// 本类主要用于发送，一次只能一个发送任务
    /// </summary>
    public class Ctrler_Send : BaseController
    {
        // 获取发送状态，用于是否恢复界面的发送情况
        [Route(HttpVerbs.Get, "/send/status")]
        public void GetSendStatus()
        {
            SendStatus sendStatus = SendStatus.Init;
            if (SendTask.Instance != null)
            {
                sendStatus = SendTask.Instance.SendStatus;
            }

            ResponseSuccess(sendStatus);
        }


        // 新建发送任务
        [Route(HttpVerbs.Post, "/send/task")]
        public void CreateTask()
        {
            // 生成
            string subject = Body.Value<string>("subject");
            JArray receivers = Body.Value<JArray>("receivers");
            JArray data = Body.Value<JArray>("data");
            string templateId = Body.Value<string>("templateId");

            bool createResult = SendTask.CreateSendTask(Token.UserId, subject, receivers, data, templateId, LiteDb, out string message);
            if (createResult) ResponseSuccess(createResult);
            else ResponseError(message);
        }

        // 获取预览内容
        [Route(HttpVerbs.Get, "/send/preview/{key}")]
        public void GetSendPreview(string key)
        {
            SendItem item = SendTask.Instance.GetPreviewHtml(key);
            if (item == null)
            {
                ResponseError("没有可预览项，请检查收件箱是否为空");
                return;
            }

            ResponseSuccess(item);
        }

        // 开始发送邮件
        [Route(HttpVerbs.Post, "/send")]
        public void StartSending(string key)
        {
            var id = SendTask.Instance.StartSending();
            ResponseSuccess(id);
        }

        // 获取发件状态
        [Route(HttpVerbs.Get, "/send/info")]
        public void GetSendingInfo()
        {
            ResponseSuccess(SendTask.Instance.SendingInfo);
        }

        // 获取发件状态
        [Route(HttpVerbs.Get, "/send/history/{id}/result")]
        public void GetHistoryResult(string id)
        {
            HistoryGroup historyGroup = LiteDb.SingleById<HistoryGroup>(id);
            // 获取成功的数量
            int successCount = LiteDb.Fetch<SendItem>(s => s.historyId == id && s.isSent).Count;

            JObject result;
            if (successCount == historyGroup.receiverIds.Count)
            {
                string msg = $"发送成功！共发送：{successCount}/{historyGroup.receiverIds.Count}";
                result = new JObject(new JProperty("message", msg), new JProperty("ok", true));
            }
            else
            {
                string msg = $"未完全发送，共发送：{successCount}/{historyGroup.receiverIds.Count}。请在发件历史中查询重发";
                result = new JObject(new JProperty("message", msg), new JProperty("ok", false));
            }
            ResponseSuccess(result);
        }

        // 重新发件
        [Route(HttpVerbs.Post, "/resend")]
        public void ResendItems()
        {
            if (SendTask.Instance == null)
            {
                // 需要初始化 Instance
                if(!SendTask.CreateSendTask(Token.UserId,LiteDb,out string message))
                {
                    ResponseError(message);
                }
            }
            string historyId = Body.Value<string>("historyId");
            List<string> itemIds = Body.Value<JArray>("sendItemIds").ToList().ConvertAll(jt=>jt.ToString());

            // 开始重新发送
            SendTask.Instance.Resend(historyId, itemIds);

            ResponseSuccess(true);
        }
    }
}
