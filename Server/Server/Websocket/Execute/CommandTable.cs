using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Execute
{
    public enum CommandTable
    {
        None,

        /// <summary>
        /// 获取文件信息
        /// </summary>
        getFileInfos,

        /// <summary>
        /// 获取历史工作目录
        /// </summary>
        getHistoryWorkPath,

        /// <summary>
        /// 设置历史工作目录
        /// </summary>
        setHistoryWorkPath,

        /// <summary>
        /// 下载文件
        /// </summary>
        downloadFile,

        /// <summary>
        /// 上传文件
        /// </summary>
        uploadFile,

        /// <summary>
        /// 验证工作目录的正确性
        /// </summary>
        validateWorkPath,

        /// <summary>
        /// 选择工作目录
        /// </summary>
        selectWorkPath,

        /// <summary>
        /// 删除本地文件
        /// </summary>
        deleteLocalFiles,

        /// <summary>
        /// 在文件管理器中打开
        /// </summary>
        revealInFileExplore,

        /// <summary>
        /// 用默认程序打开文件
        /// </summary>
        openFilesByDefault,

        /// <summary>
        /// 获取文件的图标
        /// </summary>
        getFilesIcons,

        /// <summary>
        /// 获取参考
        /// </summary>
        getReferences,

        /// <summary>
        /// 上传glb文件
        /// </summary>
        uploadGLB,

        /// <summary>
        /// 获取当前版本
        /// </summary>
        getCurrentVersion,

        /// <summary>
        /// 更新至最新版本
        /// </summary>
        pullLatestVersion,

        /// <summary>
        /// 置顶
        /// </summary>
        bringToTop,

        /// <summary>
        /// 取消置顶
        /// </summary>
        cancleTop,

        /// <summary>
        /// 是否有快速上传下载
        /// 兼容原来的上传下载方式
        /// </summary>
        hasFastUpdownload,

        /// <summary>
        /// 快速上传
        /// </summary>
        fastUpload,

        /// <summary>
        /// 快速下载
        /// </summary>
        fastDownload,

        /// <summary>
        /// 判断是否有某个command存在
        /// </summary>
        checkCommand,
    }
}
