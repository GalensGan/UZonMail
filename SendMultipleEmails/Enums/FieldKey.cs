using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMultipleEmails.Enums
{
    public enum FieldKey
    {
        SenderName,
        SenderEmail,
        SenderSMTP,
        ReceiverName,
        ReceiverEmail,        
        SendDate,
        IsSuccess,
        // 当前状态下的消息
        Message,
        // 每次发送的是一个GroupId
        GroupId,
        ResendEnabled,
        // 标题
        EmailSubject,
        // 正文内容
        EmailBody,

        #region  数据库
        /// <summary>
        /// 名称
        /// </summary>
        Name,

        UserId,
        #endregion
    }
}
