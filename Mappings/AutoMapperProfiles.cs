using AutoMapper;
using WeatherV2API.Models.Domain;
using WeatherV2API.Models.DTOs;

namespace WeatherV2API.Mappings
{
	public class AutoMapperProfiles: Profile
	{
		public AutoMapperProfiles() 
		{
			CreateMap<City, CityDto>().ReverseMap();
			CreateMap<WeatherIcon, WeatherIconDto>().ReverseMap();
			CreateMap<WeatherResponse, WeatherResponseDto>().ReverseMap();
		}
		
	}
}
