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
        /// 设计文件上传下载
        /// </summary>
        designFileUpDownload,

        /// <summary>
        /// 模型文件上传
        /// </summary>
        glbModelUpload,

        /// <summary>
        /// glb 本地缓存
        /// </summary>
        glbLocalCache,

        /// <summary>
        /// 文件协同
        /// </summary>
        fileCooperation,

        /// <summary>
        /// 服务版本
        /// </summary>
        serviceVersion,

        /// <summary>
        /// 文件优先级
        /// </summary>
        filePriority,

        /// <summary>
        /// 文件分片多进程快速上传
        /// </summary>
        fastFileUpdownload,

        /// <summary>
        /// 检查api
        /// </summary>
        checkAPI,
    }
}
