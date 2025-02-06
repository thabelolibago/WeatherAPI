namespace WeatherV2API.Models.Domain
{
	public class WeatherResponse
	{
		public Coord Coord { get; set; }
		public List<WeatherCondition> Weather { get; set; }
		public Weathermain Main { get; set; }
		public WeatherWind Wind { get; set; }

		public class WeatherCondition
		{
			public string Description { get; set; }
			public string Main { get; set; }
			public string Icon { get; set; }
		}

		public class Weathermain
		{
			public float Temp { get; set; }
			public int Humidity { get; set; }
			public int Pressure { get; set; }
		}

		public class WeatherWind
		{
			public float Speed { get; set; }
		}

		public class WeatherForecastResponse
		{
			public List<WeatherForecastItem> List { get; set; }
		}

		public class WeatherForecastItem
		{
			public long Dt { get; set; }
			public WeatherResponse.WeatherCondition[] Weather { get; set; }
			public WeatherResponse.Weathermain Main { get; set; }
			public WeatherResponse.WeatherWind Wind { get; set; }
		}
	}

	
	public class Coord
	{
		public double Lat { get; set; }
		public double Lon { get; set; }
	}
}
