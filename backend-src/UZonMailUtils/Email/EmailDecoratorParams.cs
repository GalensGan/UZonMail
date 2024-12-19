using System;
using UZonMail.DB.Managers.Cache;
using UZonMail.DB.SQL.EmailSending;

namespace UZonMail.Utils.Email
{
    public class EmailDecoratorParams(IServiceProvider serviceProvider, OrganizationSettingCache settingReader, SendingItem sendingItem, string outboxEmail)
    {
        public IServiceProvider ServiceProvider { get; } = serviceProvider;

        public OrganizationSettingCache SettingsReader { get; } = settingReader;

        public SendingItem SendingItem { get; } = sendingItem;

        /// <summary>
        /// 发件箱
        /// </summary>
        public string OutboxEmail { get; set; } = outboxEmail;
    }
}
