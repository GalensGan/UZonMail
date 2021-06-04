using SendMultipleEmails.Enums;
using SendMultipleEmails.Datas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMultipleEmails.Pages
{
    public class SendScreenBase:ScreenChild
    {
        public SendScreenBase(Store store) : base(store) { }

        public event Action<SendStatus> CommandChanged;

        /// <summary>
        /// 激发下一个命令
        /// </summary>
        /// <param name="sendStatus"></param>
        public void NextCommand(SendStatus sendStatus)
        {
            CommandChanged?.Invoke(sendStatus);
        }

        /// <summary>
        /// 加载数据
        /// </summary>
        public virtual void Load()
        {

        }
    }
}
