﻿using Innofactor.EfCoreJsonValueConverter;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;
using UZonMailService.Models.SqlLite.Base;
using UZonMailService.Models.SqlLite.Emails;
using UZonMailService.Models.SqlLite.Files;
using UZonMailService.Models.SqlLite.NoEntity;
using UZonMailService.Models.SqlLite.Templates;
using UZonMailService.Models.SqlLite.UserInfos;
using UZonMailService.Services.EmailSending.Sender;

namespace UZonMailService.Models.SqlLite.EmailSending
{
    /// <summary>
    /// 邮件项
    /// 实体配置参考：https://learn.microsoft.com/zh-cn/ef/core/modeling/#grouping-configuration
    /// </summary>
    //[EntityTypeConfiguration(typeof(SendingItem))]
    public class SendingItem : SqlId, IEntityTypeConfiguration<SendingItem>
    {
        /// <summary>
        /// 所属发送任务
        /// </summary>
        public int SendingGroupId { get; set; }
        public SendingGroup SendingGroup { get; set; }

        /// <summary>
        /// 所属用户
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 发件人
        /// 由于是多线程发件，这个值只有发送后才能确定
        /// 若一开始由数据指定，则其值 > 0
        /// </summary>
        public int OutBoxId { get; set; }

        /// <summary>
        /// 实际发件人
        /// 由于是多线程发件，这个值只有发送后才能确定
        /// </summary>
        public string? FromEmail { get; set; }

        /// <summary>
        /// 收件人
        /// 可能有多个收件人
        /// </summary>
        [JsonField]
        public List<EmailAddress> Inboxes { get; set; }

        /// <summary>
        /// 抄送人
        /// </summary>
        [JsonField]
        public List<EmailAddress>? CC { get; set; }

        /// <summary>
        /// 密送人
        /// </summary>
        [JsonField]
        public List<EmailAddress>? BCC { get; set; }

        /// <summary>
        /// 邮件模板 Id
        /// 可以为 0，表示不使用模板
        /// 模板是发送时，动态指定的
        /// 若一开始由数据指定，则其值 > 0
        /// </summary>
        public int EmailTemplateId { get; set; }

        /// <summary>
        /// 发送主题
        /// 由于可能是随机主题，因此要记录
        /// </summary>
        public string? Subject { get; set; }

        /// <summary>
        /// 实际发送内容
        /// 将模板与数据进行合并后的内容
        /// 若初始时不为空，则说明是被重写了
        /// </summary>
        public string? Content { get; set; }

        /// <summary>
        /// 附件的 FileUsageId
        /// </summary>
        public List<FileUsage>? Attachments { get; set; } = [];

        /// <summary>
        /// 批量发送
        /// </summary>
        public bool IsSendingBatch { get; set; }

        /// <summary>
        /// 发送代理
        /// </summary>
        public int ProxyId { get; set; }
        #region 发送结果
        /// <summary>
        /// 发送日期
        /// </summary>
        public DateTime SendDate { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public SendingItemStatus Status { get; set; }

        /// <summary>
        /// 发送结果
        /// </summary>
        public string? SendResult { get; set; }

        /// <summary>
        /// 重试次数
        /// </summary>
        public int TriedCount { get; set; }

        /// <summary>
        /// Smpt 服务器返回的 Id
        /// 通过这个 id 去获取阅读状态
        /// </summary>
        public string? ReceiptId { get; set; }
        #endregion

        /// <summary>
        /// 转换成 SendItem
        /// </summary>
        /// <returns></returns>
        public SendItem ToSendItem()
        {
            var sendItem = new SendItem(this);
            return sendItem;
        }

        /// <summary>
        /// 配置
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(EntityTypeBuilder<SendingItem> builder)
        {
            builder.HasMany(x => x.Attachments).WithMany();

            //builder.Property(x => x.Attachments)
            //        .HasConversion(
            //            v => JsonConvert.SerializeObject(v),
            //            v => JsonConvert.DeserializeObject<List<FileUsage>>(v));

            //builder.OwnsMany(x => x.Attachments, cb =>
            //{
            //    cb.ToJson();
            //});
        }
    }
}
