using JobHuntAPI.Model;
using Microsoft.EntityFrameworkCore;

namespace JobHuntAPI.Repository
{
	public class UserContext(IConfiguration configuration) : DbContext
	{

		public DbSet<User> Users { get; set; } = null!;
		public DbSet<UserSalt> UserSalts { get; set; } = null!;

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			
			optionsBuilder.UseNpgsql(configuration["ConnectionStrings:Postgres"]);
		}
	}
}
