using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeatherV2API.Data;
using WeatherV2API.Models.Domain;

namespace WeatherApi.Controllers
{
	[ApiController]
	[Route("api/weather")]
	public class WeatherController : ControllerBase
	{
		private readonly WeatherDbContext _context;
		private readonly HttpClient _httpClient;
		private readonly string _openWeatherApiKey;
		private readonly ILogger<WeatherController> _logger;

		public WeatherController(WeatherDbContext context, HttpClient httpClient,IConfiguration configuration, ILogger<WeatherController> logger)
		{
			_context = context;
			_httpClient = httpClient;
			_openWeatherApiKey = configuration["OpenWeatherApi:ApiKey"];
			_logger = logger;
		}

		[HttpGet("{cityName}")]
		public async Task<IActionResult> GetWeather(string cityName)
		{
			if (string.IsNullOrWhiteSpace(cityName))
				return BadRequest("City name cannot be empty.");

			var apiUrl = $"https://api.openweathermap.org/data/2.5/weather?q={cityName}&appid={_openWeatherApiKey}&units=metric";

			try
			{
				var weatherData = await _httpClient.GetFromJsonAsync<WeatherResponse>(apiUrl);
				if (weatherData == null || weatherData.Main == null || weatherData.Wind == null)
					return NotFound("City not found or weather data incomplete in OpenWeather API.");

				var city = await _context.Cities.FirstOrDefaultAsync(c => c.Name.ToLower() == cityName.ToLower());

				return Ok(new
				{
					City = city?.Name ?? cityName,
					ImageUrl = city?.ImageUrl,
					Temperature = weatherData.Main.Temp,
					WindSpeed = weatherData.Wind.Speed,
					Humidity = weatherData.Main.Humidity,
					Pressure = weatherData.Main.Pressure
				});
			}
			catch (HttpRequestException ex)
			{
				_logger.LogError(ex, "Error fetching weather data for {CityName}", cityName);
				return StatusCode(500, "Failed to fetch weather data. Please try again later.");
			}
		}
	}
}

