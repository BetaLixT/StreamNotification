namespace BetaLixt.StreamNotification.Observer;
using BetaLixt.StreamNotification.Interfaces;
using BetaLixt.StreamNotification.Models;
using BetaLixt.StreamNotification.Options;
using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.Logging;

public class PublishObserver : IObserver<EventEntity>
{
    
    private readonly BufferBlock<EventEntity> _messageBuffer = new BufferBlock<EventEntity>();
    private readonly IBatchPublisher _publisher;
    private readonly ILogger _logger;
    private readonly PublishObserverOptions _options;
    private readonly Task _publishTask;

    public PublishObserver(
        IBatchPublisher publisher,
        ILogger<PublishObserver> logger,
        PublishObserverOptions options
    )
    {
        this._publisher = publisher;
        this._options = options;
        this._publishTask = this.ProcessEventQueueAsync();
        this._logger = logger;
    }

    // public void Subscribe(IObservable<EventEntity> provider)
    // {

    // } 

    public void OnNext(EventEntity evnt)
    {
        var attemptCount = 0;
        while (!this._messageBuffer.Post(evnt) && attemptCount < this._options.MaxPublishRetries)
        {
            attemptCount++;
            System.Threading.Thread.Sleep(100);
        }
    }

    public void OnCompleted()
    {
        this._messageBuffer.Complete();
        this._publishTask.Wait();
    }

    public void OnError(Exception e)
    {
        // Shouldn't really happen
    }

    private async Task ProcessEventQueueAsync()
    {
        try
        {
            while(await this._messageBuffer.OutputAvailableAsync())
            {
                if(this._messageBuffer.TryReceiveAll(out var messages))
                {
                    try
                    {
                        await this._publisher.PublishBatchAsync(messages);
                    }
                    catch(Exception e)
                    {
                        this._logger.LogError(e, "Exception publishing message");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Exception waiting for messages");
            try
            {
                this._messageBuffer.Complete();
            }
            catch (Exception e)
            {
                this._logger.LogError(e, "Exception completing message buffer");
            }
        }
    }
        
}