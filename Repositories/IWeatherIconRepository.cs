using WeatherV2API.Models.Domain;
using WeatherV2API.Models.DTO;

namespace WeatherV2API.Repositories
{
	public interface IWeatherIconRepository
	{
		Task<List<WeatherIcon>> GetAllWeatherIconsAsync();
		Task<WeatherIcon> GetWeatherIconByNameAsync( string precipitationType);
		Task<WeatherIcon> CreateWeatherIconAsync(WeatherIconDto weatherIconDto);
		Task<WeatherIcon> GetWeatherIconByIdAsync(int id);
		Task<WeatherIcon> UpdateWeatherIconAsync(int id, WeatherIconDto weatherIconDto);
		Task DeleteWeatherIconAsync(int id);


	}
}
