using Microsoft.EntityFrameworkCore;
using WeatherV2API.Models.Domain;

namespace WeatherV2API.Data
{
	public class WeatherDbContext : DbContext
	{
		public WeatherDbContext(DbContextOptions<WeatherDbContext> options) : base(options) {}
		public DbSet<City> Cities { get; set; }
		public DbSet<WeatherIcon> WeatherIcons { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			
			modelBuilder.Entity<WeatherIcon>()
				.HasOne(w => w.City)
				.WithMany(c => c.WeatherIcons)
				.HasForeignKey(w => w.CityId);

			base.OnModelCreating(modelBuilder);
		}
	}
}
