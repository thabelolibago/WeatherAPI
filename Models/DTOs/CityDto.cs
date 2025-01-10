using System.ComponentModel.DataAnnotations;

namespace WeatherV2API.Models.DTOs
{
	public class CityDto
	{
	
		public int Id { get; set; }
		[Required]
		[MaxLength(25, ErrorMessage = "City's name must be a maximum of 25 characters.")]
		public string Name { get; set; }
		public string ImageUrl { get; set; }
	}
}
