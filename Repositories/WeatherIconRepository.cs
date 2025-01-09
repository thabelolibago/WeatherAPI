using Microsoft.EntityFrameworkCore;
using WeatherV2API.Data;
using WeatherV2API.Models.Domain;
using WeatherV2API.Repositories;

namespace WeatherV2API.Domain.Repositories
{
	public class WeatherIconRepository : IWeatherIconRepository
	{
		private readonly WeatherDbContext _context;

		public WeatherIconRepository(WeatherDbContext context)
		{
			_context = context;
		}

		public async Task<List<WeatherIcon>> GetAllWeatherIconsAsync()
		{
			return await _context.WeatherIcons.ToListAsync();
		}

		public async Task<WeatherIcon> AddWeatherIconAsync(WeatherIcon weatherIcon)
		{
			_context.WeatherIcons.Add(weatherIcon);
			await _context.SaveChangesAsync();
			return weatherIcon;
		}

		public async Task<WeatherIcon> UpdateWeatherIconAsync(WeatherIcon weatherIcon)
		{
			_context.WeatherIcons.Update(weatherIcon);
			await _context.SaveChangesAsync();
			return weatherIcon;
		}

		public async Task<bool> DeleteWeatherIconAsync(int weatherIconId)
		{
			var weatherIcon = await _context.WeatherIcons.FindAsync(weatherIconId);
			if (weatherIcon == null) return false;

			_context.WeatherIcons.Remove(weatherIcon);
			await _context.SaveChangesAsync();
			return true;
		}
	}
}
