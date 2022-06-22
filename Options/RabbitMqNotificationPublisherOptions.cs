namespace BetaLixt.StreamNotification.Options;

public class RabbitMqNotificationPublisherOptions
{
    public string NotificationExchangeName { get; set; } = "notifications";
    public string NotificationExchangeType { get; set; } = "topic";
}