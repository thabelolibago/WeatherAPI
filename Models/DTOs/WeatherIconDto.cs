namespace WeatherV2API.Models.DTOs
{
	public class WeatherIconDto
	{
		public int Id { get; set; }
		public string PrecipitationType { get; set; }
		public string IconDayUrl { get; set; }
		public string IconNightUrl { get; set; }
		public int CityId { get; set; }

	}
}
