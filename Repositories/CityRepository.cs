using Microsoft.EntityFrameworkCore;
using WeatherV2API.Data;
using WeatherV2API.Models.Domain;

namespace WeatherV2API.Repositories
{
	public class CityRepository : ICityRepository
	{
		private readonly WeatherDbContext _context;

		public CityRepository(WeatherDbContext context)
		{
			_context = context;
		}

		public async Task<City> GetCityByNameAsync(string cityName)
		{
			return await _context.Cities
				.FirstOrDefaultAsync(c => c.Name.ToLower() == cityName.ToLower());
		}

		public async Task<List<City>> GetAllCitiesAsync()
		{
			return await _context.Cities.ToListAsync();
		}

		public async Task<City> AddCityAsync(City city)
		{
			_context.Cities.Add(city);
			await _context.SaveChangesAsync();
			return city;
		}

		public async Task<City> UpdateCityAsync(City city)
		{
			_context.Cities.Update(city);
			await _context.SaveChangesAsync();
			return city;
		}

		public async Task<bool> DeleteCityAsync(int cityId)
		{
			var city = await _context.Cities.FindAsync(cityId);
			if (city == null) return false;

			_context.Cities.Remove(city);
			await _context.SaveChangesAsync();
			return true;
		}

	}
}
