using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMultipleEmails.Enums
{
    public enum History
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
    }
}
