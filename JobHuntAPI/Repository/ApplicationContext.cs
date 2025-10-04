using JobHuntAPI.Model;
using Microsoft.EntityFrameworkCore;

namespace JobHuntAPI.Repository
{
	public class ApplicationContext(IConfiguration configuration) : DbContext
	{
		public DbSet<JobApplication> JobApplications { get; set; } = null!;
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseNpgsql(configuration.GetConnectionString("Postgres"));
		}
	
	}
}
