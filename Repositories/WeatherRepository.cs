using Microsoft.EntityFrameworkCore;
using WeatherV2API.Data;
using WeatherV2API.Domain.Repositories;
using WeatherV2API.Models.Domain;

public class WeatherRepository : IWeatherRepository
{
	private readonly WeatherDbContext _context;
	private readonly HttpClient _httpClient;
	private readonly string _openWeatherApiKey;

	public WeatherRepository(WeatherDbContext context, HttpClient httpClient, IConfiguration configuration)
	{
		_context = context;
		_httpClient = httpClient;
		_openWeatherApiKey = configuration["OpenWeatherApi:ApiKey"];
	}

	public async Task<WeatherResponse> GetWeatherDataAsync(string cityName)
	{
		var apiUrl = $"https://api.openweathermap.org/data/2.5/weather?q={cityName}&appid={_openWeatherApiKey}&units=metric";

		try
		{
			var weatherData = await _httpClient.GetFromJsonAsync<WeatherResponse>(apiUrl);
			return weatherData;
		}
		catch (HttpRequestException ex)
		{
			
			throw new Exception($"Failed to retrieve weather data for city: {cityName}", ex);
		}
	}

	public async Task<City> GetCityByNameAsync(string cityName)
	{
		return await _context.Cities.FirstOrDefaultAsync(c => c.Name.ToLower() == cityName.ToLower());
	}

	public async Task<WeatherResponse> GetTomorrowWeatherDataAsync(string cityName)
	{
		var apiUrl = $"https://api.openweathermap.org/data/2.5/forecast?q={cityName}&appid={_openWeatherApiKey}&units=metric";

		try
		{
			var weatherData = await _httpClient.GetFromJsonAsync<WeatherForecastResponse>(apiUrl);

			if (weatherData == null || weatherData.List == null) return null;

			var tomorrow = DateTime.UtcNow.AddDays(1).Date;
			var tomorrowWeather = weatherData.List.FirstOrDefault(forecast =>
				DateTimeOffset.FromUnixTimeSeconds(forecast.Dt).UtcDateTime.Date == tomorrow);

			if (tomorrowWeather == null) return null;

			return new WeatherResponse
			{
				Weather = tomorrowWeather.Weather.ToList(),
				Main = tomorrowWeather.Main,
				Wind = tomorrowWeather.Wind
			};
		}
		catch (HttpRequestException ex)
		{
			throw new Exception($"Failed to retrieve tomorrow's weather data for city: {cityName}", ex);
		}
	}

	public async Task<WeatherForecastResponse> GetSevenDayForecastAsync(string cityName)
	{
		var apiUrl = $"https://api.openweathermap.org/data/2.5/forecast?q={cityName}&appid={_openWeatherApiKey}&units=metric";

		try
		{
			var weatherData = await _httpClient.GetFromJsonAsync<WeatherForecastResponse>(apiUrl);
			return weatherData;
		}
		catch (HttpRequestException ex)
		{
			throw new Exception($"Failed to retrieve 7-day forecast data for city: {cityName}", ex);
		}
	}

	public class WeatherForecastResponse
	{
		public List<WeatherForecastItem> List { get; set; }
	}

	public class WeatherForecastItem
	{
		public long Dt { get; set; }
		public WeatherResponse.WeatherCondition[] Weather { get; set; }
		public WeatherResponse.Weathermain Main { get; set; }
		public WeatherResponse.WeatherWind Wind { get; set; }
	}

}
