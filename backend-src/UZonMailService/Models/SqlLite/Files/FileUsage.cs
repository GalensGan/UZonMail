using System.ComponentModel.DataAnnotations.Schema;
using UZonMailService.Models.SqlLite.Base;
using UZonMailService.Models.SqlLite.UserInfos;

namespace UZonMailService.Models.SqlLite.Files
{
    /// <summary>
    /// 文件使用情况
    /// </summary>
    public class FileUsage : SqlId
    {
        /// <summary>
        /// 为了提取前端上传的附件
        /// </summary>
        [NotMapped]
        public int __fileUsageId { get; set; }

        /// <summary>
        /// 拥有者的用户ID
        /// </summary>
        public int OwnerUserId { get; set; }
        public User OwnerUser { get; set; }

        /// <summary>
        /// 唯一名称
        /// 若有唯一名称，则会替换原来的文件名，减少文件存储
        /// </summary>
        public string? UniqueName { get; set; }

        /// <summary>
        /// 文件名（包含后缀）
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 文件 id
        /// </summary>
        public int FileObjectId { get; set; }
        /// <summary>
        /// 文件对象
        /// </summary>
        public FileObject FileObject { get; set; }

        /// <summary>
        /// 是否是公共文件
        /// 公共文件不需要权限即可访问
        /// </summary>
        public bool IsPublic { get; set; }
    }
}
