namespace BetaLixt.StreamNotification;
using Microsoft.Extensions.Hosting;
using BetaLixt.StreamNotification.Observers;

public static class WebApplicationExtentions
{
    public static IHost UseNotificationPublisher(this IHost app)
    {
        var obs = app.Services.GetService(typeof(PublishObserver));
        if (obs == null)
        {
            throw new Exception("No publish observer registered");
        }
        return app;
    }
}