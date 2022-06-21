namespace BetaLixt.StreamNotification.Interfaces;
using BetaLixt.StreamNotification.Models;

public interface IBatchPublisher
{
    Task PublishBatchAsync(IEnumerable<EventEntity> events);
}