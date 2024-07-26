using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UZonMailService.UzonMailDB.SQL;

namespace UZonMail.Utils.UzonMail
{
    /// <summary>
    /// 发送上下文
    /// </summary>
    public interface ISendingContext
    {
        /// <summary>
        /// 数据库
        /// </summary>
        SqlContext SqlContext { get; }

        /// <summary>
        /// 服务提供者
        /// </summary>
        IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// 获取客户端
        /// </summary>
        /// <typeparam name="THub"></typeparam>
        /// <typeparam name="TInterface"></typeparam>
        /// <returns></returns>
        IHubContext<THub, TInterface> GetHubClient<THub, TInterface>() where THub : Hub<TInterface> where TInterface : class;

        /// <summary>
        /// 发送阶段
        /// </summary>
        public List<ISendingStage> SendingStages { get; set; }
        /// <summary>
        /// 添加发送阶段
        /// </summary>
        /// <param name="stage"></param>
        public void AddSendingStage(ISendingStage stage)
        {
            if (stage == null) return;
            if (SendingStages == null)
            {
                SendingStages = [];
            }
            SendingStages.Add(stage);
        }

        /// <summary>
        /// 发送结果
        /// </summary>
        public SentResult? SendResult { get; set; }

        /// <summary>
        /// 设置发送结果
        /// </summary>
        /// <param name="result"></param>
        public void SetSendResult(SentResult result)
        {
            if (SendResult != null) throw new InvalidOperationException("发送结果无法多次设置");
            SendResult = result;
        }
    }
}
