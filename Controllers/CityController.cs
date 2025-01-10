using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using WeatherV2API.Repositories;
using WeatherV2API.Models.Domain;
using WeatherV2API.Models.DTOs;

namespace WeatherV2API.Controllers
{
	[ApiController]
	[Route("api/cities")]
	public class CityController : ControllerBase
	{
		private readonly ICityRepository _cityRepository;
		private readonly IMapper _mapper;

		public CityController(ICityRepository cityRepository, IMapper mapper)
		{
			_cityRepository = cityRepository;
			_mapper = mapper;
		}

		[HttpGet]
		public async Task<IActionResult> GetCities()
		{
			var cities = await _cityRepository.GetAllCitiesAsync();
			var cityDtos = _mapper.Map<IEnumerable<CityDto>>(cities);
			return Ok(cityDtos);
		}

		[HttpGet("{id:int}")]
		public async Task<IActionResult> GetCity(int id)
		{
			var city = await _cityRepository.GetCityByIdAsync(id);
			if (city == null)
			{
				return NotFound(new { message = $"City with ID {id} not found." });
			}

			var cityDto = _mapper.Map<CityDto>(city);
			return Ok(cityDto);
		}


		[HttpPost]
		public async Task<IActionResult> CreateCity(CityDto cityDto)
		{
			if (ModelState.IsValid)
			{
				var city = _mapper.Map<City>(cityDto);
				await _cityRepository.AddCityAsync(city);

				return CreatedAtAction(nameof(GetCity), new { id = city.Id }, cityDto);

			}
			else
			{
				return BadRequest();
			}

			
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateCity(int id, CityDto cityDto)
		{
			var city = await _cityRepository.GetCityByNameAsync(id.ToString());
			if (city == null) return NotFound();

			_mapper.Map(cityDto, city);
			await _cityRepository.UpdateCityAsync(city);

			return NoContent();
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteCity(int id)
		{
			var city = await _cityRepository.GetCityByNameAsync(id.ToString());
			if (city == null) return NotFound();

			await _cityRepository.DeleteCityAsync(id);
			return NoContent();
		}
	}
}
