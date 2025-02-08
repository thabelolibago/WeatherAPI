using Microsoft.AspNetCore.Mvc;
using WeatherV2API.Models.Domain;
using WeatherV2API.Models.DTOs;
using WeatherV2API.Repositories;

namespace WeatherV2API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CityController : ControllerBase
	{
		private readonly ICityRepository _cityRepository;
		private readonly IWebHostEnvironment _env;

		public CityController(ICityRepository cityRepository, IWebHostEnvironment env)
		{
			_cityRepository = cityRepository;
			_env = env;
		}

		[HttpGet("{cityName}")]
		public async Task<IActionResult> GetCityByName(string cityName)
		{
			var city = await _cityRepository.GetCityByNameAsync(cityName);
			if (city == null)
				return NotFound();
			return Ok(city);
		}

		[HttpGet("ById/{id}")]
		public async Task<IActionResult> GetCityById(int id)
		{
			var city = await _cityRepository.GetCityByIdAsync(id);

			if (city == null)
				return NotFound();

			return Ok(city);
		}


		[HttpGet]
		public async Task<IActionResult> GetAllCities()
		{
			var cities = await _cityRepository.GetAllCitiesAsync();
			return Ok(cities);
		}

		[HttpPost]
		public async Task<IActionResult> CreateCity([FromForm] CityDto cityDto, IFormFile imageFile)
		{
			// Check if the city already exists
			var existingCity = await _cityRepository.GetCityByNameAsync(cityDto.Name);
			if (existingCity != null)
			{
				// If the city exists, return a Conflict (409) response
				return Conflict(new { message = "City already exists." });
			}

			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			// Create a new city
			var city = new City
			{
				Name = cityDto.Name
			};

			if (imageFile != null)
			{
				if (string.IsNullOrEmpty(imageFile.FileName))
				{
					return BadRequest("Image file name is not provided.");
				}

				// Use the provided province or set a default one
				var province = string.IsNullOrEmpty(cityDto.Province) ? "DefaultProvince" : cityDto.Province;

				// Create the folder path based on the province
				var cityFolder = Path.Combine(_env.WebRootPath, "Images", "Cities", province);

				if (!Directory.Exists(cityFolder))
				{
					Directory.CreateDirectory(cityFolder);
				}

				// Save the image file with a unique name (e.g., using the city name)
				var imagePath = Path.Combine(cityFolder, cityDto.Name + Path.GetExtension(imageFile.FileName));

				using (var stream = new FileStream(imagePath, FileMode.Create))
				{
					await imageFile.CopyToAsync(stream);
				}

				city.ImageUrl = $"/Images/Cities/{province}/{cityDto.Name}{Path.GetExtension(imageFile.FileName)}";
			}

			// Add the city to the database
			await _cityRepository.AddCityAsync(city);

			// Return Created response with the city's details
			return CreatedAtAction(nameof(GetCityByName), new { cityName = city.Name }, city);
		}



		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateCity(int id, [FromForm] CityDto cityDto, IFormFile? imageFile)
		{
			
			var city = await _cityRepository.GetCityByIdAsync(id);
			if (city == null)
				return NotFound();

			
			if (!string.IsNullOrWhiteSpace(cityDto.Name))
			{
				city.Name = cityDto.Name;
			}

			
			if (imageFile != null)
			{
				
				if (!string.IsNullOrEmpty(city.ImageUrl))
				{
					var oldFilePath = Path.Combine(_env.WebRootPath, city.ImageUrl.TrimStart('/'));
					if (System.IO.File.Exists(oldFilePath))
					{
						System.IO.File.Delete(oldFilePath); 
					}
				}

				
				var newImagePath = Path.Combine(_env.WebRootPath, "Images", "Cities", imageFile.FileName);
				using (var stream = new FileStream(newImagePath, FileMode.Create))
				{
					await imageFile.CopyToAsync(stream); 
				}

				
				city.ImageUrl = "/Images/Cities/" + imageFile.FileName;
			}

			
			await _cityRepository.UpdateCityAsync(city);

			return NoContent();
		}


		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteCity(int id)
		{
			var city = await _cityRepository.GetCityByIdAsync(id);

			if (city == null)
				return NotFound();

			await _cityRepository.DeleteCityAsync(city.Id);

			return NoContent();
		}
	}
}
