using Microsoft.AspNetCore.Mvc;
using WeatherV2API.Models.Domain;
using WeatherV2API.Domain.Repositories;
using WeatherV2API.Models.DTOs;
using System.Threading.Tasks;
using WeatherV2API.Repositories;

namespace WeatherV2API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class WeatherController : ControllerBase
	{
		private readonly IWeatherRepository _weatherRepository;
		private readonly ICityRepository _cityRepository;

		public WeatherController(IWeatherRepository weatherRepository, ICityRepository cityRepository)
		{
			_weatherRepository = weatherRepository;
			_cityRepository = cityRepository;
		}

		[HttpGet("{cityName}")]
		public async Task<IActionResult> GetCityWeather(string cityName)
		{
			var city = await _cityRepository.GetCityByNameAsync(cityName);
			if (city == null)
				return NotFound("City not found.");

			var weatherData = await _weatherRepository.GetWeatherDataAsync(cityName);

			var weatherResponseDto = new WeatherResponseDto
			{
				City = city.Name,
				ImageUrl = city.ImageUrl,
				Temperature = weatherData.Main.Temp,
				WindSpeed = weatherData.Wind.Speed,
				Humidity = weatherData.Main.Humidity,
				Pressure = weatherData.Main.Pressure
			};

			return Ok(weatherResponseDto);
		}

		
	}
}
