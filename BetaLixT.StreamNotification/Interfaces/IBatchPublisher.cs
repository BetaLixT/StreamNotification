namespace BetaLixt.StreamNotification.Interfaces;
using BetaLixt.StreamNotification.Models;

public interface IBatchPublisher: IDisposable
{
    Task PublishBatchAsync(IEnumerable<TracedEvent> events);
    
}