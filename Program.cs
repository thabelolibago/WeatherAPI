using WeatherV2API.Data;
using Microsoft.EntityFrameworkCore;
using WeatherApi.Data;
using WeatherV2API.Domain.Repositories;
using WeatherV2API.Repositories;

namespace WeatherV2API
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			builder.Services.AddScoped<ICityRepository, CityRepository>();
			builder.Services.AddScoped<IWeatherIconRepository, WeatherIconRepository>();
			builder.Services.AddScoped<IWeatherRepository, WeatherRepository>();

			// Add services to the container.
			builder.Services.AddControllers();
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			builder.Services.AddDbContext<WeatherDbContext>(options =>
			options.UseSqlServer(builder.Configuration.GetConnectionString("WeatherDb")));

			builder.Services.AddHttpClient();

			builder.Services.AddAutoMapper(typeof(Program));


			var app = builder.Build(); 

			
			using (IServiceScope scope = app.Services.CreateScope())
			{
				var dbContext = scope.ServiceProvider.GetRequiredService<WeatherDbContext>();
				DbInitializer.Seed(dbContext);
			}

			
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();

			app.UseAuthorization();

			app.MapControllers();

			app.Run();
		}
	}
}
