namespace BetaLixt.StreamNotification.Publishers;
using BetaLixt.StreamNotification.Interfaces;
using BetaLixt.StreamNotification.Models;

public class RabbitMqPublisher : IBatchPublisher
{
    public async Task PublishBatchAsync(IEnumerable<EventEntity> events)
    {

    }
}