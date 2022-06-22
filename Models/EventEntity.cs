namespace BetaLixt.StreamNotification.Models;

public class EventEntity
{
    public string ServiceName { get; set; } = "";
    public string Id { get; set; } = "";
    public string Stream { get; set; } = "";
    public string StreamId { get; set; } = "";
    public long StreamVersion { get; set; } = -1;
    public string Event { get; set; } = "";
    public object? Data { get; set; } = "";
    public DateTimeOffset CreatedDateTime { get; set; }
}