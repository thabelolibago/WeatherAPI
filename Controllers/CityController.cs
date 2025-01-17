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
		public async Task<IActionResult> GetCityByName(string name)
		{
			var city = await _cityRepository.GetCityByNameAsync(name);

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
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var city = new City
			{
				Name = cityDto.Name
			};

			
			if (imageFile != null)
			{
				
				if (string.IsNullOrEmpty(_env.WebRootPath))
				{
					return StatusCode(500, "Web root path is not configured.");
				}

				
				if (string.IsNullOrEmpty(imageFile.FileName))
				{
					return BadRequest("Image file name is not provided.");
				}

				
				var cityFolder = Path.Combine(_env.WebRootPath, "Images", "Cities", "Limpopo");

				
				if (!Directory.Exists(cityFolder))
				{
					Directory.CreateDirectory(cityFolder);
				}

				
				var imagePath = Path.Combine(cityFolder, cityDto.Name + Path.GetExtension(imageFile.FileName));

				
				using (var stream = new FileStream(imagePath, FileMode.Create))
				{
					await imageFile.CopyToAsync(stream);
				}

				
				city.ImageUrl = $"/Images/Cities/Limpopo/{cityDto.Name}{Path.GetExtension(imageFile.FileName)}";
			}

			
			await _cityRepository.AddCityAsync(city);

			
			return CreatedAtAction(nameof(GetCityByName), new { name = city.Name }, city);
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
