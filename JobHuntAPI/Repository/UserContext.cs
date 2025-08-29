using JobHuntAPI.Model;
using Microsoft.EntityFrameworkCore;

namespace JobHuntAPI.Repository
{
	public class UserContext(IConfiguration configuration) : DbContext
	{
		public DbSet<User> Users { get; set; } = null!;

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseNpgsql(configuration.GetConnectionString("Postgres"));
		}
	}
}
