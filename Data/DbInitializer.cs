using WeatherV2API.Data;
using WeatherV2API.Models.Domain;

namespace WeatherApi.Data
{
	public static class DbInitializer
	{
		public static void Seed(WeatherDbContext context)
		{
			if (!context.Cities.Any())
			{
				var city = new City
				{
					Name = "Thohoyandou",
					ImageUrl = "/Images/Cities/thohoyandou.jpg"
				};

				context.Cities.Add(city);
				context.SaveChanges();

				context.WeatherIcons.Add(new WeatherIcon
				{
					PrecipitationType = "Rain",
					// Store file paths here, not IFormFile
					FilePathDayIcon = "/Images/WeatherIcons/DayIcons/rain.png",
					FilePathNightIcon = "/Images/WeatherIcons/NightIcons/rain.png",
				});

				context.SaveChanges();
			}
		}
	}
}
