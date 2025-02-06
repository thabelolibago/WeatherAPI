using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using WeatherV2API.Domain.Repositories;
using WeatherV2API.Models.Domain;
using WeatherV2API.Models.DTOs;
using WeatherV2API.Repositories;

namespace WeatherV2API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class WeatherController : ControllerBase
	{
		private readonly IWeatherRepository _weatherRepository;
		private readonly ICityRepository _cityRepository;
		private readonly IWeatherIconRepository _weatherIconRepository;
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly IConfiguration _configuration;

		public WeatherController(
			IWeatherRepository weatherRepository,
			ICityRepository cityRepository,
			IWeatherIconRepository weatherIconRepository,
			IHttpClientFactory httpClientFactory,
			IConfiguration configuration)
		{
			_weatherRepository = weatherRepository;
			_cityRepository = cityRepository;
			_weatherIconRepository = weatherIconRepository;
			_httpClientFactory = httpClientFactory;
			_configuration = configuration;
		}

		[HttpGet("{cityName}")]
		public async Task<IActionResult> GetCityWeather(string cityName)
		{
			var city = await _cityRepository.GetCityByNameAsync(cityName);
			string cityImageUrl = city?.ImageUrl ?? await GetCityImageFromUnsplash(cityName);

			if (cityImageUrl == null)
			{
				return NotFound("City not found in the database, and no image is available from Unsplash.");
			}

			var currentWeather = await _weatherRepository.GetWeatherDataAsync(cityName);
			var tomorrowWeather = await _weatherRepository.GetTomorrowWeatherDataAsync(cityName);
			var location = await GetCityLocationAsync(cityName);
			var sixDayForecast = await GetSixDayForecastAsync(cityName);

			if (currentWeather == null || sixDayForecast == null)
			{
				return NotFound("Weather data not available.");
			}

			var currentCondition = currentWeather.Weather.FirstOrDefault();
			var tomorrowCondition = tomorrowWeather?.Weather.FirstOrDefault();

			var currentIcon = await _weatherIconRepository.GetWeatherIconByNameAsync(currentCondition?.Main ?? "Clear");
			var tomorrowIcon = await _weatherIconRepository.GetWeatherIconByNameAsync(tomorrowCondition?.Main ?? "Clear");

			if (currentIcon == null || tomorrowIcon == null)
			{
				return NotFound("Weather icons not available.");
			}

			var currentIconUrl = GetIconUrl(currentIcon);
			var tomorrowIconUrl = GetIconUrl(tomorrowIcon);

			var responseDto = new WeatherFullResponseDto
			{
				City = cityName,
				ImageUrl = cityImageUrl,
				Latitude = location?.Latitude ?? 0,
				Longitude = location?.Longitude ?? 0,
				CurrentWeather = new WeatherResponseDto
				{
					Temperature = currentWeather.Main.Temp,
					WindSpeed = currentWeather.Wind.Speed,
					Humidity = currentWeather.Main.Humidity,
					Pressure = currentWeather.Main.Pressure,
					Description = currentCondition?.Description,
					IconUrl = currentIconUrl
				},
				TomorrowWeather = tomorrowWeather != null ? new WeatherResponseDto
				{
					Temperature = tomorrowWeather.Main.Temp,
					WindSpeed = tomorrowWeather.Wind.Speed,
					Humidity = tomorrowWeather.Main.Humidity,
					Pressure = tomorrowWeather.Main.Pressure,
					Description = tomorrowCondition?.Description,
					IconUrl = tomorrowIconUrl
				} : null,
				SixDayForecast = sixDayForecast
			};

			return Ok(responseDto);
		}

		private async Task<(double Latitude, double Longitude)?> GetCityLocationAsync(string cityName)
		{
			var apiKey = _configuration["OpenWeatherApi:ApiKey"];
			var client = _httpClientFactory.CreateClient();
			var apiUrl = $"https://api.openweathermap.org/data/2.5/weather?q={cityName}&appid={apiKey}";

			try
			{
				var response = await client.GetFromJsonAsync<WeatherResponse>(apiUrl);

				if (response?.Coord != null)
				{
					return (response.Coord.Lat, response.Coord.Lon);
				}

				return null;
			}
			catch
			{
				return null;
			}
		}

		private string GetIconUrl(WeatherIcon icon)
		{
			var currentTime = DateTime.Now;
			return (currentTime.Hour >= 6 && currentTime.Hour < 18) ? icon.FilePathDayIcon : icon.FilePathNightIcon;
		}

		private async Task<string> GetCityImageFromUnsplash(string cityName)
		{
			var apiKey = _configuration["UnsplashApi:ApiKey"];
			var client = _httpClientFactory.CreateClient();
			var url = $"https://api.unsplash.com/photos/random?query={cityName}&client_id={apiKey}";

			try
			{
				var response = await client.GetAsync(url);
				if (!response.IsSuccessStatusCode)
				{
					return null;
				}

				var content = await response.Content.ReadAsStringAsync();
				var json = JsonDocument.Parse(content);
				var imageUrl = json.RootElement.GetProperty("urls").GetProperty("regular").GetString();
				return imageUrl;
			}
			catch
			{
				return null;
			}
		}

		private async Task<List<WeatherResponseDto>> GetSixDayForecastAsync(string cityName)
		{
			var forecastData = await _weatherRepository.GetSevenDayForecastAsync(cityName);

			if (forecastData == null || forecastData.List == null)
			{
				return null;
			}

			var forecastResponse = new List<WeatherResponseDto>();

			var today = DateTime.UtcNow.Date;

			foreach (var forecast in forecastData.List
				.GroupBy(f => DateTimeOffset.FromUnixTimeSeconds(f.Dt).UtcDateTime.Date)
				.Where(g => g.Key >= today)
				.Take(6))
			{
				var firstForecast = forecast.First();
				var weatherCondition = firstForecast.Weather.FirstOrDefault()?.Main ?? "Clear";

				var icon = await _weatherIconRepository.GetWeatherIconByNameAsync(weatherCondition);

				if (icon == null)
				{
					return null;
				}

				var forecastTime = DateTimeOffset.FromUnixTimeSeconds(firstForecast.Dt).UtcDateTime;
				var isDayTime = forecastTime.Hour >= 6 && forecastTime.Hour < 18;
				var iconUrl = isDayTime ? icon.FilePathDayIcon : icon.FilePathNightIcon;

				forecastResponse.Add(new WeatherResponseDto
				{
					Temperature = firstForecast.Main.Temp,
					WindSpeed = firstForecast.Wind.Speed,
					Humidity = firstForecast.Main.Humidity,
					Pressure = firstForecast.Main.Pressure,
					Description = firstForecast.Weather.FirstOrDefault()?.Description,
					IconUrl = iconUrl
				});
			}

			return forecastResponse;
		}
	}
}
