﻿using UZonMail.Core.Services.SendCore.Contexts;
using UZonMail.Core.Services.SendCore.WaitList;

namespace UZonMail.Core.Services.SendCore.ResponsibilityChains
{
    /// <summary>
    /// 发送项获取器
    /// </summary>
    public class EmailItemGetter(GroupTasksList groupTasksList) : AbstractSendingHandler
    {
        protected override async Task HandleCore(SendingContext context)
        {
            // 如果前面失败了，这一步就不执行
            if (context.Status.HasFlag(ContextStatus.Fail))
                return;

            // 从等待列表中获取一个发送项
            var emailItem = await groupTasksList.GetEmailItem(context);

            // 修改状态
            emailItem?.SetStatus(SendItemMetaStatus.Working);
            // 保存到 context 中
            context.EmailItem = emailItem;

            // 标记失败
            if(emailItem == null)
            {
                context.Status |= ContextStatus.Fail;
            }            
        }
    }
}
