using LiteDB;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Database.Models
{
    public class SendItem:AutoObjectId
    {
        // 历史id
        public string historyId { get; set; }

        // 发送者信息
        public string senderName { get; set; }
        public string senderEmail { get; set; }

        // 接收者信息
        public string receiverName { get; set; }
        public string receiverEmail { get; set; }

        // 邮件主题
        public string subject { get; set; }

        // 邮件 html 内容
        public string html { get; set; }

        // 进度信息
        public int index { get; set; }
        public int total { get; set; }

        // 生成成果
        public string sendMessage { get; set; }
        public bool isSent { get; set; }

        // 尝试次数
        public int tryCount { get; set; }

        // 发送时间
        public DateTime sendDate { get; set; }

        // 发送格式
        public SendItemType sendItemType { get; set; }

        /// <summary>
        /// 内容 url
        /// </summary>
        public string dataUrl { get; set; }

        // 待发附件
        public List<EmailAttachment> attachments { get; set; }
    }

    public enum SendItemType
    {
        /// <summary>
        /// 无
        /// </summary>
        none,

        /// <summary>
        /// html格式
        /// </summary>
        html,

        /// <summary>
        /// 数据 URL
        /// </summary>
        dataUrl,
    }
}
