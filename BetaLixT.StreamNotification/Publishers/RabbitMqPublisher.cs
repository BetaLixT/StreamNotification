namespace BetaLixt.StreamNotification.Publishers;
using BetaLixt.StreamNotification.Interfaces;
using BetaLixt.StreamNotification.Models;
using BetaLixt.StreamNotification.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;

public class RabbitMqPublisher : IBatchPublisher
{
    private readonly ConnectionFactory _rabbitConnectionFactory;
    private readonly IConnection _rabbitConnection;
    private readonly IModel _rabbitChannel;
    private readonly RabbitMqNotificationPublisherOptions _options;
    
    
    public RabbitMqPublisher(
        ConnectionFactory rabbitConnectionFactory,
        IOptions<RabbitMqNotificationPublisherOptions> options)
    {
        this._rabbitConnectionFactory = rabbitConnectionFactory;
        this._rabbitConnection = this._rabbitConnectionFactory.CreateConnection();
        this._rabbitChannel = this._rabbitConnection.CreateModel();
        this._options = options.Value;
        this._rabbitChannel.ExchangeDeclare(
            this._options.NotificationExchangeName,
            this._options.NotificationExchangeType,
            durable: true,
            autoDelete: false
        );
    }

    public async Task PublishBatchAsync(IEnumerable<TracedEvent> events)
    {
        var batch = this._rabbitChannel.CreateBasicPublishBatch();
        foreach(var evnt in events)
        {
            var json = JsonConvert.SerializeObject(evnt, new JsonSerializerSettings {ReferenceLoopHandling = ReferenceLoopHandling.Ignore});
            batch.Add(
                this._options.NotificationExchangeName,
                $"{evnt.ServiceName}.{evnt.Stream}.{evnt.Event}",
                true,
                null,
                new ReadOnlyMemory<byte>(Encoding.UTF8.GetBytes(json))
            );
        }
        batch.Publish();
        // TODO: wait for acks/nacks and handle
    }

    public void Dispose()
    {
        try
        {
            this._rabbitConnection.Dispose();
            this._rabbitChannel.Dispose();
        }
        catch {}
    }
}