using WeatherV2API.Data;
using Microsoft.EntityFrameworkCore;
using WeatherV2API.Domain.Repositories;
using WeatherV2API.Repositories;

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

			// Add CORS policy
			builder.Services.AddCors(options =>
			{
				options.AddPolicy("AllowAllOrigins", builder =>
				{
					builder.AllowAnyOrigin() // Allow requests from any origin
						   .AllowAnyMethod() // Allow all HTTP methods (GET, POST, etc.)
						   .AllowAnyHeader(); // Allow all headers
				});
			});

			// Create the app instance
			var app = builder.Build();


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

			// Enable CORS
			app.UseCors("AllowAllOrigins");

			// Map the controllers (API routes)
			app.MapControllers();

			// Run the application
			app.Run();
		}
	}
}