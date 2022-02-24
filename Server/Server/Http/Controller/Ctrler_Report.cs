using EmbedIO;
using EmbedIO.Routing;
using LiteDB;
using Newtonsoft.Json.Linq;
using Server.Config;
using Server.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Server.Http.Controller
{
    /// <summary>
    /// 统计报表相关的接口
    /// </summary>
    class Ctrler_Report : BaseControllerAsync
    {
        /// <summary>
        /// 邮件总体到达率
        /// </summary>
        [Route(HttpVerbs.Get, "/report/success-rate")]
        public async Task GetAllSuccessRate()
        {
            // 找到当前的用户名
            var userId = Token.UserId;
            // 获取用户发送的历史组
            var historyGroups = LiteDb.Fetch<HistoryGroup>(g => g.userId == userId).ToList();

            if (historyGroups.Count < 1)
            {
                // 返回1
                await ResponseSuccessAsync(1);
                return;
            };


            // 查找历史组下面的所有的发件
            var sendItems = LiteDb.Fetch<SendItem>(Query.In(Fields.historyId, new BsonArray(historyGroups.ConvertAll(hg => new BsonValue(hg._id)))));
            if (sendItems.Count < 1)
            {
                // 返回1
                await ResponseSuccessAsync(1);
                return;
            }

            // 计算比例
            var successItems = sendItems.FindAll(item => item.isSent);
            await ResponseSuccessAsync(successItems.Count * 1.0 / sendItems.Count);
        }

        /// <summary>
        /// 收件箱种类和数量
        /// </summary>
        [Route(HttpVerbs.Get, "/report/inbox-type-count")]
        public async Task GetINboxTypeAndCount()
        {
            // 找到当前的用户名
            var userId = Token.UserId;
            // 获取用户发送的历史组
            var historyGroups = LiteDb.Fetch<HistoryGroup>(g => g.userId == userId).ToList();
            var defaultResults = new JArray()
                {
                   new JObject(){ { "name","未发件"},{ "value",0} }
                };
            if (historyGroups.Count < 1)
            {
                // 返回默认值
                try
                {
                    await ResponseSuccessAsync(defaultResults);
                }
                catch (Exception e)
                {
                    ;
                }
                return;
            };


            // 查找历史组下面的所有的发件
            var sendItems = LiteDb.Fetch<SendItem>(Query.In(Fields.historyId, new BsonArray(historyGroups.ConvertAll(hg => new BsonValue(hg._id)))));
            if (sendItems.Count < 1)
            {
                // 返回1
                await ResponseSuccessAsync(defaultResults);
                return;
            }

            // 计算每个邮箱对应的值
            Dictionary<string, int> resultDic = new Dictionary<string, int>();
            var regex = new Regex(@"@\w+([-.]\w+)*\.\w+([-.]\w+)*$");
            foreach (var sendItem in sendItems)
            {

                var emailType = regex.Match(sendItem.receiverEmail);
                if (!emailType.Success) continue;

                var typeKey = emailType.Value;
                if (resultDic.ContainsKey(typeKey))
                {
                    resultDic[typeKey] = resultDic[typeKey] + 1;
                }
                else
                {
                    resultDic.Add(typeKey, 1);
                }
            }

            await ResponseSuccessAsync(resultDic.ToList().ConvertAll(item =>
            {
                return new JObject(new JProperty(Fields.name, item.Key), new JProperty(Fields.value, item.Value));
            }));
        }
    }
}
