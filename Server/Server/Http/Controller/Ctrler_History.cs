using EmbedIO;
using EmbedIO.Routing;
using Newtonsoft.Json.Linq;
using Server.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Http.Controller
{
    /// <summary>
    /// 发件历史
    /// </summary>
    public class Ctrler_History : BaseController
    {
        // 获取发件历史
        [Route(HttpVerbs.Get, "/histories")]
        public void GetHistories()
        {
            var histories = LiteDb.Fetch<HistoryGroup>(h => h.userId == Token.UserId).OrderByDescending(item => item._id);

            foreach (HistoryGroup history in histories)
            {
                // 获取成功的数量
                history.successCount = LiteDb.Fetch<SendItem>(s => s.historyId == history._id && s.isSent).Count;
            }

            // 获取状态
            ResponseSuccess(histories);
        }

        // 获取发件历史
        [Route(HttpVerbs.Get, "/histories/{historyId}")]
        public void GetHistory(string historyId)
        {
            var history = LiteDb.SingleById<HistoryGroup>(historyId);

            // 获取成功的数量
            history.successCount = LiteDb.Fetch<SendItem>(s => s.historyId == history._id && s.isSent).Count;


            // 获取状态
            ResponseSuccess(history);
        }

        // 获取发件历史
        [Route(HttpVerbs.Get, "/histories/{historyId}/items")]
        public void GetSendItems(string historyId)
        {
            var results = LiteDb.Fetch<SendItem>(s => s.historyId == historyId);
            // 获取状态
            ResponseSuccess(results);
        }

        // 删除发件历史 
        [Route(HttpVerbs.Delete, "/histories/{historyId}")]
        public void DeleteHistoryGroup(string historyId)
        {
            // 删除发送记录
            LiteDb.DeleteMany<SendItem>(item => item.historyId == historyId);

            // 删除组
            LiteDb.Delete<HistoryGroup>(historyId);

            // 获取状态
            ResponseSuccess(true);
        }
    }
}
