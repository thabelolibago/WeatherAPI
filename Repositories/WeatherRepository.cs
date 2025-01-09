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
}
