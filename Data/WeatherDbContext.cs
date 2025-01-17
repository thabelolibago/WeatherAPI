using Microsoft.EntityFrameworkCore;
using WeatherV2API.Models.Domain;

namespace WeatherV2API.Data
{
	public class WeatherDbContext : DbContext
	{
		public WeatherDbContext(DbContextOptions<WeatherDbContext> options) : base(options) { }

		public DbSet<City> Cities { get; set; }
		public DbSet<WeatherIcon> WeatherIcons { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			
			modelBuilder.Entity<WeatherIcon>().HasKey(w => w.Id);

			
			base.OnModelCreating(modelBuilder);
		}
	}
}
