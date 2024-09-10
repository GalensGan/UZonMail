using System;
using UZonMail.DB.SQL.EmailSending;
using UZonMail.DB.SQL.Settings;

namespace Uamazing.Utils.Email
{
    public class EmailBodyDecoratorParams(IServiceProvider serviceProvider, UserSettingsReader userSettings, SendingItem sendingItem, string outboxEmail)
    {
        public IServiceProvider ServiceProvider { get; } = serviceProvider;

        public UserSettingsReader UserSettings { get; } = userSettings;

        public SendingItem SendingItem { get; } = sendingItem;

        public string OutboxEmail { get; set; } = outboxEmail;
    }
}
