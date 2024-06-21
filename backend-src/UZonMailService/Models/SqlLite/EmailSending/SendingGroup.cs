using Innofactor.EfCoreJsonValueConverter;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations.Schema;
using Uamazing.ConfValidatation.Core.Entrance;
using Uamazing.ConfValidatation.Core.Results;
using UZonMailService.Models.SqlLite.Base;
using UZonMailService.Models.SqlLite.Emails;
using UZonMailService.Models.SqlLite.Files;
using UZonMailService.Models.SqlLite.Settings;
using UZonMailService.Models.SqlLite.Templates;

namespace UZonMailService.Models.SqlLite.EmailSending
{
    /// <summary>
    /// 发件组
    /// 此处只记录统计数据
    /// 具体的数据由 EmailItem 记录
    /// </summary>
    public class SendingGroup : SqlId, IEntityTypeConfiguration<SendingGroup>
    {
        #region EF 定义
        /// <summary>
        /// 用户名
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 主题
        /// 多个主题使用分号或者换行分隔
        /// </summary>
        public string Subjects { get; set; }

        /// <summary>
        /// 模板
        /// 不包含用户中的模板
        /// </summary>
        public List<EmailTemplate>? Templates { get; set; }

        /// <summary>
        /// 正文内容
        /// </summary>
        public string? Body { get; set; }

        /// <summary>
        /// 发件箱
        /// 不包含数据中的发件箱
        /// </summary>
        public List<Outbox> Outboxes { get; set; }

        /// <summary>
        /// 收件箱
        /// </summary>
        [JsonField]
        public List<EmailAddress> Inboxes { get; set; }

        /// <summary>
        /// 抄送箱
        /// </summary>
        [JsonField]
        public List<EmailAddress>? CcBoxes { get; set; }

        /// <summary>
        /// 密送
        /// </summary>
        [JsonField]
        public List<EmailAddress>? BccBoxes { get; set; }

        /// <summary>
        /// 附件
        /// </summary>
        public List<FileUsage>? Attachments { get; set; }

        /// <summary>
        /// 用户通过 excel 上传的数据
        /// </summary>
        [JsonField]
        public JArray? Data { get; set; }

        /// <summary>
        /// 是否分布式发件
        /// </summary>
        public bool IsDistributed { get; set; }

        /// <summary>
        /// 总发件数量
        /// Inboxes 的数量
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 成功的数量
        /// </summary>
        public int SuccessCount { get; set; }

        /// <summary>
        /// 已经发送的数量
        /// </summary>
        public int SentCount { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public SendingGroupStatus Status { get; set; }

        /// <summary>
        /// 发送开始时间
        /// </summary>
        public DateTime SendStartDate { get; set; }

        /// <summary>
        /// 发送结束时间
        /// </summary>
        public DateTime SendEndDate { get; set; }

        /// <summary>
        /// 最后一条邮件的消息
        /// </summary>
        public string? LastMessage { get; set; }

        #region 定时发件相关
        /// <summary>
        /// 发件类型
        /// </summary>
        public SendingGroupType SendingType { get; set; }

        /// <summary>
        /// 定时发件时间
        /// </summary>
        public DateTime ScheduleDate { get; set; }
        #endregion

        #endregion

        #region 临时数据，不保存到数据库
        /// <summary>
        /// smtp 密码解密 key
        /// </summary>
        [NotMapped]
        public List<string> SmtpPasswordSecretKeys { get; set; }

        /// <summary>
        /// 批量改善
        /// </summary>
        [NotMapped]
        public bool SendBatch { get; set; }
        #endregion

        #region 外部工具方法       
        private List<string>? _subjects;
        private static readonly string[] separators = ["\r\n", "\n", ";", "；"];
        private List<string> SplitSubjects()
        {
            if (_subjects == null)
            {
                // 说明没有初始化
                if (string.IsNullOrEmpty(Subjects))
                {
                    _subjects = [string.Empty];
                }

                // 分割主题
                _subjects = [.. Subjects.Split(separators, StringSplitOptions.RemoveEmptyEntries)];
            }
            return _subjects;
        }
        /// <summary>
        /// 若有多个主题，则获取随机主题
        /// </summary>
        /// <returns></returns>
        public string GetRandSubject()
        {
            SplitSubjects();

            // 返回随机主题
            return _subjects[new Random().Next(_subjects.Count)];
        }

        /// <summary>
        /// 获取第一个主题
        /// </summary>
        /// <returns></returns>
        public string GetFirstSubject()
        {
            SplitSubjects();
            return _subjects.FirstOrDefault();
        }

        #endregion

        #region 数据验证
        /// <summary>
        /// 验证数据
        /// </summary>
        /// <param name="validateOption"></param>
        /// <returns></returns>
        public ValidateResult Validate(ValidateOption validateOption = ValidateOption.None)
        {
            // 1. 主题必须
            if (string.IsNullOrEmpty(Subjects))
            {
                return new ValidateResult(false, "主题不能为空");
            }

            // 没有 excel 数据的情况
            if (Data == null || Data.Count == 0)
            {
                // 2. 没有数据
                var globalVdResult = ValidateGlobalData();
                if (!globalVdResult) return globalVdResult;
            }
            else
            {
                // 有 excel 数据的情况
                ExcelDataInfo excelDataInfo = new(Data);
                // 若用户选择了收件箱，要判断发件箱是否在数据表格中出现
                if (Inboxes.Count > 0)
                {
                    int dataCount = excelDataInfo.InboxSet.Count;
                    Inboxes.ForEach(x => excelDataInfo.InboxSet.Add(x.Email));
                    int allCount = excelDataInfo.InboxSet.Count;
                    if (dataCount < allCount)
                    {
                        // 验证通用数据
                        var globalVdResult = ValidateGlobalData();
                        if (!globalVdResult) return globalVdResult;
                    }
                }

                // 验证其它数据
                if (Outboxes.Count == 0 && excelDataInfo.OutboxStatus != ExcelDataStatus.All)
                {
                    return new ValidateResult(false, "缺失发件箱，请在数据中指定发件箱或选择发件箱");
                }
                if(excelDataInfo.InboxStatus!=ExcelDataStatus.All)
                {
                    return new ValidateResult(false, "请保证每条数据都有 inbox (收件人邮箱)");
                }
                if(!ExistGlobalBody() && excelDataInfo.BodyStatus != ExcelDataStatus.All)
                {
                    return new ValidateResult(false, "缺失邮件正文，请在数据中指定邮件正文 或 选择模板 或 填写正文");
                }
            }

            return new ValidateResult(true, string.Empty);
        }

        private ValidateResult ValidateGlobalData()
        {
            if (!ExistGlobalBody())
            {
                return new ValidateResult(false, "邮件模板和正文必须有一个不为空");
            }

            if (Outboxes.Count == 0)
            {
                return new ValidateResult(false, "请选择发件人");
            }

            if (Inboxes.Count == 0)
            {
                return new ValidateResult(false, "请选择收件人");
            }

            return new ValidateResult(true, string.Empty);
        }

        // 是否存在全局正文
        private bool ExistGlobalBody()
        {
            return Templates?.Count > 0 || !string.IsNullOrEmpty(Body);
        }
        #endregion

        public void Configure(EntityTypeBuilder<SendingGroup> builder)
        {
            builder.HasMany(x => x.Templates).WithMany();
            builder.HasMany(x => x.Outboxes).WithMany();
            builder.HasMany(x => x.Attachments).WithMany();
        }
    }
}
