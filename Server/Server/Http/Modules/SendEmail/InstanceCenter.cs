using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Http.Modules.SendEmail
{
    /// <summary>
    /// 单例管理中心
    /// </summary>
    class InstanceCenter
    {
        private static UserInstance<SendTask> _sendTasks;
        /// <summary>
        /// 发件任务
        /// </summary>
        public static UserInstance<SendTask> SendTasks
        {
            get
            {
                if (_sendTasks == null) _sendTasks = new UserInstance<SendTask>();

                return _sendTasks;
            }
        }

        private static UserInstance<EmailPreview> _emailPreview;
        /// <summary>
        /// 预览任务
        /// </summary>
        public static UserInstance<EmailPreview> EmailPreview
        {
            get
            {
                if (_emailPreview == null) _emailPreview = new UserInstance<EmailPreview>();

                return _emailPreview;
            }
        }

        private static UserInstance<EmailReady> _emailReady;
        /// <summary>
        /// 发件前准备任务
        /// </summary>
        public static UserInstance<EmailReady> EmailReady
        {
            get
            {
                if (_emailReady == null) _emailReady = new UserInstance<EmailReady>();

                return _emailReady;
            }
        }
    }
}
