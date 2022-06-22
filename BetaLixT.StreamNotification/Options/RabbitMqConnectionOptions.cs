namespace BetaLixt.StreamNotification.Options;

public class RabbitMqConnectionOptions
{
    public const string OptionsKey = "RabbitMqConnectionOptions";
    public string HostName { get; set; } = "localhost";
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public int Port { get; set; } = 5672;
}