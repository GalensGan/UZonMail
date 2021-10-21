using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Execute
{
    /// <summary>
    /// 命令类的键值
    /// </summary>
    public enum CommandClassName
    {
        None,

        /// <summary>
        /// 登陆模块
        /// </summary>
        Login,

        /// <summary>
        /// 选择文件
        /// </summary>
        SelectFiles,
    }
}
