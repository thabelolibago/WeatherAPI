using WeatherV2API.Data;
using Microsoft.EntityFrameworkCore;
using WeatherV2API.Domain.Repositories;
using WeatherV2API.Repositories;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using WeatherApi.Data;

namespace WeatherV2API
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Register repositories
			builder.Services.AddScoped<ICityRepository, CityRepository>();
			builder.Services.AddScoped<IWeatherIconRepository, WeatherIconRepository>();
			builder.Services.AddScoped<IWeatherRepository, WeatherRepository>();

			// Add services to the container.
			builder.Services.AddControllers();

			// Enable Swagger/OpenAPI
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			// Configure the DbContext with SQL Server
			builder.Services.AddDbContext<WeatherDbContext>(options =>
				options.UseSqlServer(builder.Configuration.GetConnectionString("WeatherDb")));

			// Add HttpClient service
			builder.Services.AddHttpClient();
			builder.Services.AddHttpContextAccessor();

			// Add AutoMapper for DTO mapping
			builder.Services.AddAutoMapper(typeof(Program));

			// Create the app instance
			var app = builder.Build();

			// Ensure that the database is seeded
			using (IServiceScope scope = app.Services.CreateScope())
			{
				var dbContext = scope.ServiceProvider.GetRequiredService<WeatherDbContext>();
				DbInitializer.Seed(dbContext);
			}

			// Add static files middleware to serve files from wwwroot
			app.UseStaticFiles(); // This serves files from wwwroot (like images)

			// Enable Swagger UI for development environment
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			// Enable HTTPS redirection and Authorization
			app.UseHttpsRedirection();
			app.UseAuthorization();

			// Map the controllers (API routes)
			app.MapControllers();

			// Run the application
			app.Run();
		}
	}
}
