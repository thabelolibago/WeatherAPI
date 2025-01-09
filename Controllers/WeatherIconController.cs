using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeatherV2API.Data;
using System.Threading.Tasks;
using WeatherV2API.Models.Domain;

namespace WeatherV2API.Controllers
{
	[Route("api/weathericons")]
	[ApiController]
	public class WeatherIconController : ControllerBase
	{
		private readonly WeatherDbContext _context;

		public WeatherIconController(WeatherDbContext context)
		{
			_context = context;
		}

		
		[HttpGet]
		public async Task<IActionResult> GetWeatherIcons()
		{
			var weatherIcons = await _context.WeatherIcons.ToListAsync();
			return Ok(weatherIcons);
		}

		
		[HttpGet("{id}")]
		public async Task<IActionResult> GetWeatherIcon(int id)
		{
			var weatherIcon = await _context.WeatherIcons.FindAsync(id);
			if (weatherIcon == null)
				return NotFound("Weather icon not found.");

			return Ok(weatherIcon);
		}

		
		[HttpPost]
		public async Task<IActionResult> AddWeatherIcon([FromBody] WeatherIcon weatherIcon)
		{
			_context.WeatherIcons.Add(weatherIcon);
			await _context.SaveChangesAsync();
			return CreatedAtAction(nameof(GetWeatherIcon), new { id = weatherIcon.WeatherIconId }, weatherIcon);
		}

		
		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateWeatherIcon(int id, [FromBody] WeatherIcon weatherIcon)
		{
			if (id != weatherIcon.WeatherIconId)
				return BadRequest("Weather Icon ID mismatch.");

			_context.Entry(weatherIcon).State = EntityState.Modified;
			await _context.SaveChangesAsync();

			return NoContent();
		}

		
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteWeatherIcon(int id)
		{
			var weatherIcon = await _context.WeatherIcons.FindAsync(id);
			if (weatherIcon == null)
				return NotFound("Weather icon not found.");

			_context.WeatherIcons.Remove(weatherIcon);
			await _context.SaveChangesAsync();

			return NoContent();
		}
	}
}
