namespace WeatherV2API.Models.Domain
{
	public class WeatherIcon
	{
		public int Id { get; set; }
		public string PrecipitationType { get; set; }
		public string FilePathDayIcon { get; set; }
		public string FilePathNightIcon { get; set; }
	}
}

