using Microsoft.AspNetCore.Mvc;
using BlazorShared;
using Microsoft.AspNetCore.Authorization;

namespace BlazorBff.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    /*
    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }
    */

    [HttpGet]
    public IEnumerable<WeatherForecast> GetPublic()
    {
        return CreateData();
    }

    [HttpGet("protected")]
    [Authorize]
    public IEnumerable<WeatherForecast> GetProtected()
    {
        return CreateData();
    }

    private IEnumerable<WeatherForecast> CreateData()
    {
        var rng = new Random();
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = rng.Next(-20, 55),
            Summary = Summaries[rng.Next(Summaries.Length)]
        })
        .ToArray();
    }

}
