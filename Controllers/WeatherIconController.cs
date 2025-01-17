using Microsoft.AspNetCore.Mvc;
using WeatherV2API.Repositories;
using WeatherV2API.Models.DTO;

namespace WeatherV2API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class WeatherIconController : ControllerBase
	{
		private readonly IWeatherIconRepository _weatherIconRepository;

		public WeatherIconController(IWeatherIconRepository weatherIconRepository)
		{
			_weatherIconRepository = weatherIconRepository;
		}

		[HttpPost("CreateWeatherIcon")]
		public async Task<IActionResult> CreateWeatherIcon([FromForm] WeatherIconDto weatherIconDto)
		{
			if (weatherIconDto.DayIcon == null || weatherIconDto.NightIcon == null)
			{
				return BadRequest("Both Day and Night icons must be provided.");
			}

			try
			{
				var result = await _weatherIconRepository.CreateWeatherIconAsync(weatherIconDto);
				return Ok(result);
			}
			catch (ArgumentException ex)
			{
				return BadRequest(ex.Message);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		[HttpGet]
		public async Task<IActionResult> GetAllWeatherIcons()
		{
			var weatherIcons = await _weatherIconRepository.GetAllWeatherIconsAsync();

			return Ok(weatherIcons);
		}

		[HttpGet("ById/{id}")]
		public async Task<IActionResult> GetWeatherIconById(int id)
		{
			try
			{
				var weatherIcon = await _weatherIconRepository.GetWeatherIconByIdAsync(id);

				if (weatherIcon == null)
				{
					return NotFound($"Weather icon with ID {id} not found.");
				}

				return Ok(weatherIcon);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		[HttpGet("{WeatherIconName}")]
		public async Task<IActionResult> GetWeatherIconByName(string WeatherIconName)
		{
			if (string.IsNullOrWhiteSpace(WeatherIconName))
			{
				return BadRequest("Weather icon name cannot be null or empty.");
			}

			try
			{
				
				var weatherIcon = await _weatherIconRepository.GetWeatherIconByNameAsync(WeatherIconName);

				if (weatherIcon == null)
				{
					return NotFound($"No weather icon found for name: {WeatherIconName}");
				}

				return Ok(weatherIcon);
			}
			catch (Exception ex)
			{
				
				Console.WriteLine(ex.Message);

				
				return StatusCode(500, "An error occurred while processing your request.");
			}
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateWeatherIcon(int id, [FromForm] WeatherIconDto weatherIconDto)
		{
			if (weatherIconDto.DayIcon == null || weatherIconDto.NightIcon == null)
			{
				return BadRequest("Both Day and Night icons must be provided.");
			}

			try
			{
				var existingIcon = await _weatherIconRepository.GetWeatherIconByIdAsync(id);
				if (existingIcon == null)
				{
					return NotFound($"No weather icon found with ID: {id}");
				}

				var updatedIcon = await _weatherIconRepository.UpdateWeatherIconAsync(id, weatherIconDto);
				return Ok(updatedIcon);
			}
			catch (ArgumentException ex)
			{
				return BadRequest(ex.Message);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteWeatherIcon(int id)
		{
			try
			{
				var existingIcon = await _weatherIconRepository.GetWeatherIconByIdAsync(id);
				if (existingIcon == null)
				{
					return NotFound($"No weather icon found with ID: {id}");
				}

				await _weatherIconRepository.DeleteWeatherIconAsync(id);
				return NoContent();
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}



	}
}
