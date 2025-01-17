using System.ComponentModel.DataAnnotations;

namespace WeatherV2API.Models.DTOs
{
	public class CityDto
	{
	
		public int Id { get; set; }

		[Required(ErrorMessage = "City name is required.")]
		[MaxLength(25, ErrorMessage = "City name must not exceed 25 characters.")]
		[MinLength(2, ErrorMessage = "City name must be at least 2 characters long.")]
		public string Name { get; set; }

		public string? ImageUrl { get; set; }
	}
}
