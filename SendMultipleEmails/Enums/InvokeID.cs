using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMultipleEmails.Enums
{
   public enum InvokeID
    {
        /// <summary>
        /// 个人主页
        /// </summary>
        Dashboard,

        /// <summary>
        /// 设置
        /// </summary>
        Settings,

        /// <summary>
        /// 发件人
        /// </summary>
        Senders,

        /// <summary>
        /// 收件人
        /// </summary>
        Receivers,

        /// <summary>
        /// 导入变量
        /// </summary>
        ImportVariables,

        /// <summary>
        /// 模板
        /// </summary>
        Template,

        /// <summary>
        /// 发送
        /// </summary>
        Send,

        /// <summary>
        /// 日志
        /// </summary>
        Log,

        /// <summary>
        /// 关于
        /// </summary>
        About,

        /// <summary>
        /// 新建发送
        /// </summary>
        Send_New,

        /// <summary>
        /// 预览发送
        /// </summary>
        Send_Preview,

        /// <summary>
        /// 发送中
        /// </summary>
        Send_Sending,

        /// <summary>
        /// 已发送
        /// </summary>
        Send_Sent,
    }
}
