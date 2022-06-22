namespace BetaLixt.StreamNotification.Observers;
using BetaLixt.StreamNotification.Interfaces;
using BetaLixt.StreamNotification.Models;
using BetaLixt.StreamNotification.Options;
using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public class PublishObserver : IObserver<TracedEvent>, IDisposable
{
    
    private readonly BufferBlock<TracedEvent> _messageBuffer = new BufferBlock<TracedEvent>();
    private readonly IBatchPublisher _publisher;
    private readonly ILogger _logger;
    private readonly PublishObserverOptions _options;
    private readonly Task _publishTask;
    private readonly NotificationDispatch _dispatch;
    private readonly IDisposable _subscription;

    public PublishObserver(
        IBatchPublisher publisher,
        ILogger<PublishObserver> logger,
        NotificationDispatch dispatch,
        IOptions<PublishObserverOptions> options
    )
    {
        this._publisher = publisher;
        this._options = options.Value;
        this._publishTask = this.ProcessEventQueueAsync();
        this._logger = logger;
        this._dispatch = dispatch;
        this._subscription = this._dispatch.Subscribe(this);
    }

    public void OnNext(TracedEvent evnt)
    {
        var attemptCount = 0;
        while (!this._messageBuffer.Post(evnt) && attemptCount < this._options.MaxPublishRetries)
        {
            attemptCount++;
            System.Threading.Thread.Sleep(100);
        }
    }

    public void Dispose()
    {
        this._subscription.Dispose();
        this.OnCompleted();
    }

    public void OnCompleted()
    {
        try
        {
            this._messageBuffer.Complete();
        }
        catch {}
        this._publishTask.Wait();
        this._publisher.Dispose();
    }

    public void OnError(Exception e) {} // shouldn't really happen 

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