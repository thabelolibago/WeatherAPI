﻿using WeatherV2API.Models.Domain;
using static WeatherRepository;

namespace WeatherV2API.Domain.Repositories
{
	public interface IWeatherRepository
	{
		Task<WeatherResponse> GetWeatherDataAsync(string cityName);
		Task<City> GetCityByNameAsync(string cityName);
		Task<WeatherResponse> GetTomorrowWeatherDataAsync(string cityName);
		Task<WeatherForecastResponse> GetSevenDayForecastAsync(string cityName);


	}
}
