using MimeKit;
using System.Collections.Generic;
using System.Threading.Tasks;
using UZonMail.Utils.Email.BodyDecorator;

namespace UZonMail.Utils.Email.MessageDecorator
{
    public class MimeMessageDecorators
    {
        private static List<IMimeMessageDecroator> Decorators { get; } = [];

        /// <summary>
        /// 添加邮件正文装饰器
        /// </summary>
        /// <param name="decorator"></param>
        public static void AddDecorator(IMimeMessageDecroator decorator)
        {
            Decorators.Add(decorator);
        }

        /// <summary>
        /// 开始对邮件内容进行装饰
        /// </summary>
        /// <param name="decoratorParams"></param>
        /// <param name="originBody"></param>
        /// <returns></returns>
        public static async Task<MimeMessage> StartDecorating(EmailDecoratorParams decoratorParams, MimeMessage mimeMessage)
        {
            foreach (var decorator in Decorators)
            {
                mimeMessage = await decorator.StartDecorating(decoratorParams, mimeMessage);
            }
            return mimeMessage;
        }
    }
}
