namespace WeatherV2API.Models.DTOs
{
	public class WeatherFullResponseDto
	{
		public string City { get; set; }
		public string ImageUrl { get; set; }
		public double Latitude { get; set; } 
		public double Longitude { get; set; }
		public WeatherResponseDto CurrentWeather { get; set; }
		public WeatherResponseDto TomorrowWeather { get; set; }
		public List<WeatherResponseDto> SixDayForecast { get; set; }
	}
}
