namespace WeatherV2API.Models.DTOs
{
	public class SevenDayForecastResponseDto
	{
		public string City { get; set; }
		public string ImageUrl { get; set; }
		public List<WeatherResponseDto> Forecast { get; set; }
	}
}
