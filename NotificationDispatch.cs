namespace BetaLixt.StreamNotification;
using BetaLixt.StreamNotification.Models;

public class NotificationDispatch : IObservable<EventEntity>
{
    IList<IObserver<EventEntity>> _observers = new List<IObserver<EventEntity>>();

    public IDisposable Subscribe(IObserver<EventEntity> observer)
    {
        if (!this._observers.Contains(observer))
         this._observers.Add(observer);
        return new Unsubscriber(this._observers, observer);
    }

    public void DispatchEventNotification(EventEntity evnt)
    {
        foreach(var obs in this._observers)
        {
            obs.OnNext(evnt);
        }
    }

    private class Unsubscriber : IDisposable
    {
        private IList<IObserver<EventEntity>>_observers;
        private IObserver<EventEntity> _observer;

        public Unsubscriber(IList<IObserver<EventEntity>> observers, IObserver<EventEntity> observer)
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