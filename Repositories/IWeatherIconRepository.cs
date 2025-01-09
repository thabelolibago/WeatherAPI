using WeatherV2API.Models.Domain;

namespace WeatherV2API.Repositories
{
	public interface IWeatherIconRepository
	{
		Task<List<WeatherIcon>> GetAllWeatherIconsAsync();
		Task<WeatherIcon> AddWeatherIconAsync(WeatherIcon weatherIcon);
		Task<WeatherIcon> UpdateWeatherIconAsync(WeatherIcon weatherIcon);
		Task<bool> DeleteWeatherIconAsync(int weatherIconId);
	}
}
