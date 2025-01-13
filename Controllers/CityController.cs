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

		[HttpGet("{name}")]
		public async Task<IActionResult> GetCityByName(string name)
		{
			var city = await _cityRepository.GetCityByNameAsync(name);

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
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var city = new City
			{
				Name = cityDto.Name
			};

			// Handle image upload
			if (imageFile != null)
			{
				// Ensure the web root path is not null
				if (string.IsNullOrEmpty(_env.WebRootPath))
				{
					return StatusCode(500, "Web root path is not configured.");
				}

				// Ensure the image file name is not null or empty
				if (string.IsNullOrEmpty(imageFile.FileName))
				{
					return BadRequest("Image file name is not provided.");
				}

				// Define the directory and image file name (e.g., Musina.png)
				var cityFolder = Path.Combine(_env.WebRootPath, "Images", "Cities", "Limpopo");

				// Create the directory if it doesn't exist
				if (!Directory.Exists(cityFolder))
				{
					Directory.CreateDirectory(cityFolder);
				}

				// Combine the directory path with the city name (to avoid GUID-based names)
				var imagePath = Path.Combine(cityFolder, cityDto.Name + Path.GetExtension(imageFile.FileName));

				// Save the image to the file system
				using (var stream = new FileStream(imagePath, FileMode.Create))
				{
					await imageFile.CopyToAsync(stream);
				}

				// Save only the relative path in the database (starting from wwwroot)
				city.ImageUrl = $"/Images/Cities/Limpopo/{cityDto.Name}{Path.GetExtension(imageFile.FileName)}";
			}

			// Add the city to the database
			await _cityRepository.AddCityAsync(city);

			// Return the created city details, including the generated ImageUrl
			return CreatedAtAction(nameof(GetCityByName), new { name = city.Name }, city);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateCity(int id, [FromForm] CityDto cityDto, IFormFile imageFile)
		{
			// Check if the provided ID matches the city DTO ID
			if (id != cityDto.Id)
				return BadRequest("City ID mismatch.");

			// Fetch the city to update from the repository
			var city = await _cityRepository.GetCityByIdAsync(id);
			if (city == null)
				return NotFound();

			// Update the city's name
			city.Name = cityDto.Name;

			// If a new image is uploaded, handle it
			if (imageFile != null)
			{
				// Delete the old image if it exists
				if (!string.IsNullOrEmpty(city.ImageUrl))
				{
					var oldFilePath = Path.Combine(_env.WebRootPath, city.ImageUrl.TrimStart('/'));
					if (System.IO.File.Exists(oldFilePath))
					{
						System.IO.File.Delete(oldFilePath); // Delete the old image file
					}
				}

				// Save the new image file to the server's directory
				var newImagePath = Path.Combine(_env.WebRootPath, "Images", "Cities", imageFile.FileName);
				using (var stream = new FileStream(newImagePath, FileMode.Create))
				{
					await imageFile.CopyToAsync(stream); // Save the uploaded image to the server
				}

				// Update the ImageUrl in the database with the new image path
				city.ImageUrl = "/Images/Cities/" + imageFile.FileName;
			}
			else
			{
				// Remove the ImageUrl if no image is uploaded
				city.ImageUrl = null;
			}

			// Save the updated city details to the repository
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
