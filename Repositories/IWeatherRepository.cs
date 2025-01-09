using WeatherV2API.Models.Domain;
using System.Threading.Tasks;

namespace WeatherV2API.Domain.Repositories
{
	public interface IWeatherRepository
	{
		Task<WeatherResponse> GetWeatherDataAsync(string cityName);
		Task<City> GetCityByNameAsync(string cityName);
	}
}
