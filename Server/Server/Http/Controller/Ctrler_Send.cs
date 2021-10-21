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
using Microsoft.Web.WebView2.Wpf;

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
            SendStatus sendStatus = SendStatus.SendFinish;
            if (InstanceCenter.SendTasks[Token.UserId] != null)
            {
                sendStatus = InstanceCenter.SendTasks[Token.UserId].SendStatus;
            }

            ResponseSuccess(sendStatus);
        }

        // 新建预览
        [Route(HttpVerbs.Post, "/send/preview")]
        public void CreatePreview()
        {
            bool createResult = EmailPreview.CreateEmailPreview(Token.UserId,Body, LiteDb, out string message);
            if (createResult)
            {
                InstanceCenter.EmailPreview[Token.UserId].Generate();
                ResponseSuccess(createResult);
            }
            else ResponseError(message);
        }

        // 获取预览内容
        [Route(HttpVerbs.Get, "/send/preview/{key}")]
        public void GetSendPreview(string key)
        {
            SendItem item = InstanceCenter.EmailPreview[Token.UserId].GetPreviewHtml(key);
            if (item == null)
            {
                ResponseError("没有可预览项，请检查收件箱是否为空");
                return;
            }

            ResponseSuccess(item);
        }

        // 新建发送准备
        [Route(HttpVerbs.Post, "/send/task")]
        public void CreateTask()
        {
            bool createResult = EmailReady.CreateEmailReady(Token.UserId,Body, LiteDb, out string message);
            if (!createResult) ResponseError(message);

            var info = InstanceCenter.EmailReady[Token.UserId].Generate();

            if (info.ok) ResponseSuccess(info);
            else ResponseError(info.message);
        }


        // 开始发送邮件
        // 也包括重新发件
        [Route(HttpVerbs.Post, "/send/tasks/{historyGroupId}")]
        public void StartSending(string historyGroupId)
        {
            // 因为创建 history 的时候，检查了发送模块是否进行，所以此处新建发送模块不会造成冲突
            if (!SendTask.CreateSendTask(historyGroupId, Token.UserId, LiteDb, out string message))
            {
                ResponseError(message);
                return;
            }

            InstanceCenter.SendTasks[Token.UserId].StartSending();

            ResponseSuccess(historyGroupId);
        }

        // 获取发件状态
        [Route(HttpVerbs.Get, "/send/info")]
        public void GetSendingInfo()
        {
            ResponseSuccess(InstanceCenter.SendTasks[Token.UserId] == null ? new SendingProgressInfo() : InstanceCenter.SendTasks[Token.UserId].SendingProgressInfo);
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
    }
}
