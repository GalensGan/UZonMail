﻿using System.Collections.Concurrent;
using UZonMail.DB.Managers.Cache;
using UZonMail.DB.SQL;
using UZonMail.DB.SQL.Templates;

namespace UZonMail.Core.Services.SendCore.EmailWaitList
{
    /// <summary>
    /// 可用的模板列表
    /// </summary>
    public class UsableTemplateList(long userId)
    {
        private ConcurrentDictionary<long, long> _sendingItemTemplateIds = [];
        private readonly List<long> _sendingGroupTemplateIds = [];

        /// <summary>
        /// 为 SendingItem 添加指定模板
        /// </summary>
        /// <param name="sendingItemId"></param>
        /// <param name="templateId"></param>
        public void AddSendingItemTemplate(long sendingItemId, long templateId)
        {
            _sendingItemTemplateIds.TryAdd(sendingItemId, templateId);
        }

        /// <summary>
        /// 添加发件组通用的模板
        /// </summary>
        /// <param name="templateIds"></param>
        public void AddSendingGroupTemplates(List<long> templateIds)
        {
            _sendingGroupTemplateIds.AddRange(templateIds);
        }

        /// <summary>
        /// 获取 SendingItem 对应的模板
        /// 若没有对应，则返回随机模板
        /// </summary>
        /// <param name="sendingItemId"></param>
        /// <returns></returns>
        public async Task<EmailTemplate?> GetTemplate(SqlContext sqlContext, long sendingItemId)
        {
            // 获取所有的模板
            var allTemplates = await DBCacher.GetCache<UserTemplatesCache>(sqlContext, userId);

            if (_sendingItemTemplateIds.TryGetValue(sendingItemId, out var templateId))
            {
                return allTemplates.Where(x => x.Id == templateId).FirstOrDefault();
            }

            // 随机获取一个模板
            var random = new Random();
            var index = random.Next(0, _sendingGroupTemplateIds.Count);
            var template = allTemplates.Where(x => x.Id == _sendingGroupTemplateIds[index]).FirstOrDefault();
            return template;
        }

        public async Task<EmailTemplate?> GetTemplateById(SqlContext sqlContext, long templateId)
        {
            // 从自己可使用的所有模板中获取
            var allTemplates = await DBCacher.GetCache<UserTemplatesCache>(sqlContext, userId);
            return allTemplates.Where(x => x.Id == templateId).FirstOrDefault();
        }

        public async Task<EmailTemplate?> GetTemplateByName(SqlContext sqlContext, string templateName)
        {
            // 从自己可使用的所有模板中获取
            var allTemplates = await DBCacher.GetCache<UserTemplatesCache>(sqlContext, userId);
            return allTemplates.Where(x => x.Name == templateName).FirstOrDefault();
        }
    }
}
