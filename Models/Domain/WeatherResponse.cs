namespace WeatherV2API.Models.Domain
{
	public class WeatherResponse
	{

		public List<WeatherCondition> Weather { get; set; }
		public Weathermain Main { get; set; }
		public WeatherWind Wind { get; set; }
		public object Daily { get; internal set; }

		public class WeatherCondition
		{
			public string Main { get; set; }
			public string Description { get; set; }
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
	}
}
