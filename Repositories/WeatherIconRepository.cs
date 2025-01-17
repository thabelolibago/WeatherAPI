using Microsoft.EntityFrameworkCore;
using WeatherV2API.Data;
using WeatherV2API.Models.Domain;
using WeatherV2API.Models.DTO;

namespace WeatherV2API.Repositories
{
	public class WeatherIconRepository : IWeatherIconRepository
	{
		private readonly IWebHostEnvironment _webHostEnvironment;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly WeatherDbContext _weatherDbContext;

		public WeatherIconRepository(IWebHostEnvironment webHostEnvironment,
									  IHttpContextAccessor httpContextAccessor,
									  WeatherDbContext weatherDbContext)
		{
			_webHostEnvironment = webHostEnvironment;
			_httpContextAccessor = httpContextAccessor;
			_weatherDbContext = weatherDbContext;
		}

		public async Task<List<WeatherIcon>> GetAllWeatherIconsAsync()
		{
			return await _weatherDbContext.WeatherIcons.ToListAsync(); 
		}

		public async Task<WeatherIcon> GetWeatherIconByNameAsync(string weatherIconName)
		{
			if (string.IsNullOrWhiteSpace(weatherIconName))
			{
				throw new ArgumentException("Weather icon name cannot be null or empty.", nameof(weatherIconName));
			}

			try
			{
				
				var weatherIcon = await _weatherDbContext.WeatherIcons
					.FirstOrDefaultAsync(wi => wi.PrecipitationType.ToLower() == weatherIconName.ToLower());

				if (weatherIcon == null)
				{
					throw new KeyNotFoundException($"No weather icon found for precipitation type: {weatherIconName}");
				}

				return weatherIcon;
			}
			catch (Exception ex)
			{
				
				Console.WriteLine($"Error fetching weather icon: {ex.Message}");
				throw;
			}
		}

		public async Task<WeatherIcon> CreateWeatherIconAsync(WeatherIconDto weatherIconDto)
		{
			if (weatherIconDto.DayIcon == null || weatherIconDto.NightIcon == null)
			{
				throw new ArgumentException("Both Day and Night icons are required.");
			}

			var dayIconFolderPath = Path.Combine(_webHostEnvironment.WebRootPath, "Images", "WeatherIcons", "DayIcons");
			var nightIconFolderPath = Path.Combine(_webHostEnvironment.WebRootPath, "Images", "WeatherIcons", "NightIcons");

			if (!Directory.Exists(dayIconFolderPath)) Directory.CreateDirectory(dayIconFolderPath);
			if (!Directory.Exists(nightIconFolderPath)) Directory.CreateDirectory(nightIconFolderPath);

			var dayIconFilePath = Path.Combine(dayIconFolderPath, weatherIconDto.DayIcon.FileName);
			var nightIconFilePath = Path.Combine(nightIconFolderPath, weatherIconDto.NightIcon.FileName);

			using (var dayIconStream = new FileStream(dayIconFilePath, FileMode.Create))
			{
				await weatherIconDto.DayIcon.CopyToAsync(dayIconStream);
			}

			using (var nightStream = new FileStream(nightIconFilePath, FileMode.Create))
			{
				await weatherIconDto.NightIcon.CopyToAsync(nightStream);
			}

			var dayIconUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/Images/WeatherIcons/DayIcons/{weatherIconDto.DayIcon.FileName}";
			var nightIconUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/Images/WeatherIcons/NightIcons/{weatherIconDto.NightIcon.FileName}";

			var weatherIcon = new WeatherIcon
			{
				PrecipitationType = weatherIconDto.PrecipitationType,
				FilePathDayIcon = dayIconUrl,
				FilePathNightIcon = nightIconUrl
			};

			await _weatherDbContext.WeatherIcons.AddAsync(weatherIcon);
			await _weatherDbContext.SaveChangesAsync();

			return weatherIcon;
		}

		public async Task<WeatherIcon> GetWeatherIconByIdAsync(int id)
		{
			return await _weatherDbContext.WeatherIcons.FirstOrDefaultAsync(wi => wi.Id == id);
		}

		public async Task<WeatherIcon> UpdateWeatherIconAsync(int id, WeatherIconDto weatherIconDto)
		{
			var weatherIcon = await GetWeatherIconByIdAsync(id);
			if (weatherIcon == null)
			{
				throw new KeyNotFoundException($"No weather icon found with ID: {id}");
			}

			var dayIconFolderPath = Path.Combine(_webHostEnvironment.WebRootPath, "Images", "WeatherIcons", "DayIcons");
			var nightIconFolderPath = Path.Combine(_webHostEnvironment.WebRootPath, "Images", "WeatherIcons", "NightIcons");

			if (!Directory.Exists(dayIconFolderPath)) Directory.CreateDirectory(dayIconFolderPath);
			if (!Directory.Exists(nightIconFolderPath)) Directory.CreateDirectory(nightIconFolderPath);

			var dayIconFilePath = Path.Combine(dayIconFolderPath, weatherIconDto.DayIcon.FileName);
			var nightIconFilePath = Path.Combine(nightIconFolderPath, weatherIconDto.NightIcon.FileName);

			using (var dayIconStream = new FileStream(dayIconFilePath, FileMode.Create))
			{
				await weatherIconDto.DayIcon.CopyToAsync(dayIconStream);
			}

			using (var nightStream = new FileStream(nightIconFilePath, FileMode.Create))
			{
				await weatherIconDto.NightIcon.CopyToAsync(nightStream);
			}

			weatherIcon.FilePathDayIcon = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/Images/WeatherIcons/DayIcons/{weatherIconDto.DayIcon.FileName}";
			weatherIcon.FilePathNightIcon = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/Images/WeatherIcons/NightIcons/{weatherIconDto.NightIcon.FileName}";
			weatherIcon.PrecipitationType = weatherIconDto.PrecipitationType;

			_weatherDbContext.WeatherIcons.Update(weatherIcon);
			await _weatherDbContext.SaveChangesAsync();

			return weatherIcon;
		}

		public async Task DeleteWeatherIconAsync(int id)
		{
			var weatherIcon = await GetWeatherIconByIdAsync(id);
			if (weatherIcon == null)
			{
				throw new KeyNotFoundException($"No weather icon found with ID: {id}");
			}

			_weatherDbContext.WeatherIcons.Remove(weatherIcon);
			await _weatherDbContext.SaveChangesAsync();
		}

	}
}
