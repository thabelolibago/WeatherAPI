using System.Threading.Tasks;
using WeatherV2API.Models.Domain;
using WeatherV2API.Models.DTO;

namespace WeatherV2API.Repositories
{
	public interface IWeatherIconRepository
	{
		Task<List<WeatherIcon>> GetAllWeatherIconsAsync();

		Task<WeatherIcon> GetWeatherIconByNameAsync( string precipitationType);

		Task<WeatherIcon> CreateWeatherIconAsync(WeatherIconDto weatherIconDto);

	}
}
