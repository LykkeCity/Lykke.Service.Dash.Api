using Lykke.Service.Dash.Api.Core.Settings;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.Dash.Api.Settings
{
    public class AppSettings
    {
        public DashApiSettings DashApiService { get; set; }

        public SlackNotificationsSettings SlackNotifications { get; set; }
    }
}
