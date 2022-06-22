namespace BetaLixt.StreamNotification.Models;

public class TracedEvent : EventEntity
{
    public string? TraceId { get; set; }
    public string? TracePartition { get; set; }
}