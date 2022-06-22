namespace BetaLixt.StreamNotification.Options;

public class PublishObserverOptions
{
    public const string OptionsKey = "PublishObserverOptions";
    public int MaxPublishRetries { get; set; } = 100;
}