namespace WeatherV2API.Models.DTOs
{
	public class WeatherResponseDto
	{
		public float Temperature { get; set; }
		public float WindSpeed { get; set; }
		public int Humidity { get; set; }
		public int Pressure { get; set; }
		public string Description { get; set; }  
		public string IconUrl { get; set; } 

	}

}
