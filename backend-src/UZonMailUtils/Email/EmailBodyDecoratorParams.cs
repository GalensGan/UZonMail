using System;
using UZonMail.DB.SQL.EmailSending;
using UZonMail.DB.SQL.Settings;

namespace Uamazing.Utils.Email
{
    public class EmailBodyDecoratorParams(IServiceProvider serviceProvider, OrganizationSettingReader settingReader, SendingItem sendingItem, string outboxEmail)
    {
        public IServiceProvider ServiceProvider { get; } = serviceProvider;

        public OrganizationSettingReader SettingsReader { get; } = settingReader;

        public SendingItem SendingItem { get; } = sendingItem;

        public string OutboxEmail { get; set; } = outboxEmail;
    }
}
