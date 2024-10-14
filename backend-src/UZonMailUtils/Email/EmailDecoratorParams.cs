using System;
using UZonMail.DB.SQL.EmailSending;
using UZonMail.DB.SQL.Settings;

namespace UZonMail.Utils.Email
{
    public class EmailDecoratorParams(IServiceProvider serviceProvider, OrganizationSettingReader settingReader, SendingItem sendingItem, string outboxEmail)
    {
        public IServiceProvider ServiceProvider { get; } = serviceProvider;

        public OrganizationSettingReader SettingsReader { get; } = settingReader;

        public SendingItem SendingItem { get; } = sendingItem;

        /// <summary>
        /// 发件箱
        /// </summary>
        public string OutboxEmail { get; set; } = outboxEmail;
    }
}
