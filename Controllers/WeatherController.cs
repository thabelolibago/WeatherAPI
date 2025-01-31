using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
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

			if (currentWeather == null)
			{
				return NotFound("Current weather data not available.");
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
				} : null
			};

			return Ok(responseDto);
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

		
	}
}
