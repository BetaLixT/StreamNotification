namespace BetaLixt.StreamNotification;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using BetaLixt.StreamNotification.Options;
using BetaLixt.StreamNotification.Publishers;
using BetaLixt.StreamNotification.Observers;
using BetaLixt.StreamNotification.Interfaces;

public static class ServiceExtentions
{
    public static IServiceCollection RegisterRabbitMqNotificationStreamFull(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var rabbitMqOptions = new RabbitMqConnectionOptions();
        configuration.Bind(RabbitMqConnectionOptions.OptionsKey, rabbitMqOptions);
        services.AddSingleton<ConnectionFactory>(new ConnectionFactory {
            HostName = rabbitMqOptions.HostName,
            Port = rabbitMqOptions.Port,
            UserName = rabbitMqOptions.Username,
            Password = rabbitMqOptions.Password,
        });
        
        services.Configure<NotificationDispatchOptions>(configuration.GetSection(NotificationDispatchOptions.OptionsKey));
        services.Configure<RabbitMqNotificationPublisherOptions>(configuration.GetSection(RabbitMqNotificationPublisherOptions.OptionsKey));
        services.Configure<PublishObserverOptions>(configuration.GetSection(PublishObserverOptions.OptionsKey));

        services.AddSingleton<NotificationDispatch>();
        services.AddSingleton<IBatchPublisher, RabbitMqPublisher>();
        services.AddSingleton<PublishObserver>();
        return services;
    }
}