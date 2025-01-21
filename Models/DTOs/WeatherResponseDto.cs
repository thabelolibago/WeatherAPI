namespace WeatherV2API.Models.DTOs
{
	public class WeatherResponseDto
	{
		public string City { get; set; }
		public string ImageUrl { get; set; }
		public float Temperature { get; set; }
		public float WindSpeed { get; set; }
		public int Humidity { get; set; }
		public int Pressure { get; set; }
		public string Description { get; set; }  // Weather description
		public string IconUrl { get; set; }     // URL for the weather icon
	}
}
