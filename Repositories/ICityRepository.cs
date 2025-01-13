using WeatherV2API.Models.Domain;

namespace WeatherV2API.Repositories
{
	public interface ICityRepository
	{
		Task<City> GetCityByNameAsync(string cityName);
		Task<List<City>> GetAllCitiesAsync();
		Task<City> GetCityByIdAsync(int cityId);
		Task<City> AddCityAsync(City city);
		Task<City> UpdateCityAsync(City city);
		Task<bool> DeleteCityAsync(int cityId);
	}
}
