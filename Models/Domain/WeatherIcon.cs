namespace WeatherV2API.Models.Domain
{
	public class WeatherIcon
	{
		public int Id { get; set; }
		public string PrecipitationType { get; set; }
		public string IconDayUrl { get; set; }
		public string IconNightUrl { get; set; }

		public int CityId { get; set; }

		public City City { get; set; }
		public int WeatherIconId { get; set; }
	}
}

