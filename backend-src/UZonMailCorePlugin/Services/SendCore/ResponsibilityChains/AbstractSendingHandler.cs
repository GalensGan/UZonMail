﻿using Org.BouncyCastle.Asn1.Ocsp;
using System.Reflection.Metadata;
using UZonMail.Core.Services.SendCore.Contexts;

namespace UZonMail.Core.Services.SendCore.ResponsibilityChains
{
    /// <summary>
    /// 子类通过修改 status 来控制是否继续向下执行
    /// </summary>
    public abstract class AbstractSendingHandler : ISendingHandler
    {
        private ISendingHandler? _nextHandler;

        public async Task Handle(SendingContext context)
        {
            // 触发当前处理者的处理方法
            if(!context.Status.HasFlag(ContextStatus.SkipNext))
            {
                await HandleCore(context);
            }

            // 调用下一个处理者
            await this.Next(context);
        }

        public ISendingHandler SetNext(ISendingHandler handler)
        {
            this._nextHandler = handler;
            return handler;
        }

        protected async Task Next(SendingContext context)
        {
            if (this._nextHandler != null)
            {
                await this._nextHandler.Handle(context);
            }
        }

        protected abstract Task HandleCore(SendingContext context);
    }
}
