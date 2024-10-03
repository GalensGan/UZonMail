using System;
using UZonMail.DB.SQL.EmailSending;
using UZonMail.DB.SQL.Settings;

namespace Uamazing.Utils.Email
{
    public class EmailBodyDecoratorParams(IServiceProvider serviceProvider, SettingsReader userSettings, SendingItem sendingItem, string outboxEmail)
    {
        public IServiceProvider ServiceProvider { get; } = serviceProvider;

        public SettingsReader UserSettings { get; } = userSettings;

        public SendingItem SendingItem { get; } = sendingItem;

        public string OutboxEmail { get; set; } = outboxEmail;
    }
}
