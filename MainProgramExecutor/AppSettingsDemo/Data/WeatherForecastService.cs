using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace AppSettingsDemo.Data
{
    public class WeatherForecastService
    {
        public WeatherForecastService(IConfiguration config) //asking for config using dependency injection. Establishing all dependencies in the constructor.
        {
            this._config = config;
        }
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
        private readonly IConfiguration _config;

        public Task<WeatherForecast[]> GetForecastAsync(DateTime startDate)
        {
            var rng = new Random();
            return Task.FromResult(Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = startDate.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            }).ToArray());
        }
    }
}
