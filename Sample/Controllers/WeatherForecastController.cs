using Microsoft.AspNetCore.Mvc;
using BetaLixt.StreamNotification;

namespace Sample.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly NotificationDispatch _dispatch;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, NotificationDispatch dispatch)
    {
        _logger = logger;
        _dispatch = dispatch;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        this._dispatch.DispatchEventNotification(
            Guid.NewGuid().ToString(),
            "WeatherForcast",
            Guid.NewGuid().ToString(),
            -1,
            "Testing",
            null,
            DateTimeOffset.UtcNow
        );
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }
}
