namespace WeatherV2API.Models.DTO
{
	public class WeatherIconDto
	{
		public IFormFile DayIcon { get; set; }
		public IFormFile NightIcon { get; set; }
		public string PrecipitationType { get; set; }
	}
}

