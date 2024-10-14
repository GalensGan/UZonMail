using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UZonMail.DB.SQL;
using UZonMail.DB.SQL.EmailSending;
using UZonMail.DB.SQL.Settings;

namespace UZonMail.Utils.Email.BodyDecorator
{
    public class EmailBodyDecorators
    {
        private static List<IEmailBodyDecroator> Decorators { get; } = [];

        /// <summary>
        /// 添加邮件正文装饰器
        /// </summary>
        /// <param name="decorator"></param>
        public static void AddDecorator(IEmailBodyDecroator decorator)
        {
            Decorators.Add(decorator);
        }

        /// <summary>
        /// 开始对邮件内容进行装饰
        /// </summary>
        /// <param name="decoratorParams"></param>
        /// <param name="originBody"></param>
        /// <returns></returns>
        public static async Task<string> StartDecorating(EmailDecoratorParams decoratorParams, string originBody)
        {
            var bodyTemp = originBody;
            foreach (var decorator in Decorators)
            {
                bodyTemp = await decorator.StartDecorating(decoratorParams, bodyTemp);
            }

            return bodyTemp;
        }
    }
}
