namespace BetaLixt.StreamNotification;
using BetaLixt.StreamNotification.Models;
using System.Diagnostics;
using Microsoft.Extensions.Options;

public class NotificationDispatch : IObservable<TracedEvent>
{
    private readonly IList<IObserver<TracedEvent>> _observers = new List<IObserver<TracedEvent>>();
    private readonly NotificationDispatchOptions _options;
    public NotificationDispatch(IOptions<NotificationDispatchOptions> options)
    {
        this._options = options.Value;
    }

    public IDisposable Subscribe(IObserver<TracedEvent> observer)
    {
        if (!this._observers.Contains(observer))
         this._observers.Add(observer);
        return new Unsubscriber(this._observers, observer);
    }

    public void DispatchEventNotification(
        string eventId,
        string stream,
        string streamId,
        int streamVersion,
        string evnt,
        object? data,
        DateTimeOffset createdDateTime
    )
    {
        var tracedEvent = new TracedEvent {
            ServiceName = this._options.ServiceName,
            TraceId = Activity.Current?.Id,
            TracePartition = Activity.Current?.GetBaggageItem("tracePartition"),
            Id = eventId,
            Stream = stream,
            StreamId = streamId,
            StreamVersion =  streamVersion,
            Event = evnt,
            Data = data,
            CreatedDateTime = createdDateTime,
        };
        foreach(var obs in this._observers)
        {
            obs.OnNext(tracedEvent);
        }
    }

    private class Unsubscriber : IDisposable
    {
        private IList<IObserver<TracedEvent>>_observers;
        private IObserver<TracedEvent> _observer;

        public Unsubscriber(IList<IObserver<TracedEvent>> observers, IObserver<TracedEvent> observer)
        {
            this._observers = observers;
            this._observer = observer;
        }

        public void Dispose()
        {
            if (_observer != null && _observers.Contains(_observer))
                _observers.Remove(_observer);
        }
    }
}