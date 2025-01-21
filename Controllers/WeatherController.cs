using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using WeatherV2API.Domain.Repositories;
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
			// Step 1: Check if the city exists in the database
			var city = await _cityRepository.GetCityByNameAsync(cityName);
			string cityImageUrl = null;

			if (city != null)
			{
				// City found in the database
				cityImageUrl = city.ImageUrl;
			}
			else
			{
				// Step 2: Fetch city image from Unsplash API
				cityImageUrl = await GetCityImageFromUnsplash(cityName);
				if (cityImageUrl == null)
				{
					// If Unsplash also fails, return a NotFound response
					return NotFound("City not found in the database, and no image is available from Unsplash.");
				}
			}

			// Step 3: Fetch weather data for the city
			var weatherData = await _weatherRepository.GetWeatherDataAsync(cityName);
			var weatherCondition = weatherData.Weather.FirstOrDefault();

			// Step 4: Fetch the weather icon based on the weather condition
			var weatherIcon = await _weatherIconRepository.GetWeatherIconByNameAsync(weatherCondition?.Main ?? "Clear");
			if (weatherIcon == null)
			{
				return NotFound($"No icon found for weather condition: {weatherCondition?.Main}");
			}

			// Determine the appropriate icon based on the time of day
			var currentTime = DateTime.Now;
			string iconUrl = (currentTime.Hour >= 6 && currentTime.Hour < 18)
				? weatherIcon.FilePathDayIcon
				: weatherIcon.FilePathNightIcon;

			// Step 5: Prepare the response DTO
			var weatherResponseDto = new WeatherResponseDto
			{
				City = cityName,
				ImageUrl = cityImageUrl,
				Temperature = weatherData.Main.Temp,
				WindSpeed = weatherData.Wind.Speed,
				Humidity = weatherData.Main.Humidity,
				Pressure = weatherData.Main.Pressure,
				Description = weatherCondition?.Description,
				IconUrl = iconUrl
			};

			return Ok(weatherResponseDto);
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
