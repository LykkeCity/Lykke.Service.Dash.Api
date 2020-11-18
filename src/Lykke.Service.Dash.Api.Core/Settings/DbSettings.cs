namespace Lykke.Service.Dash.Api.Core.Settings
{
    public class DbSettings
    {
        public string LogsConnString { get; set; }
        public string DataConnString { get; set; }
    }

    public class SlackNotificationsSettings
    {
        public AzureQueuePublicationSettings AzureQueue { get; set; }
    }

    public class AzureQueuePublicationSettings
    {
        public string ConnectionString { get; set; }

        public string QueueName { get; set; }
    }
}
