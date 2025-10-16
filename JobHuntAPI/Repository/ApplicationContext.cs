using JobHuntAPI.Model;
using Microsoft.EntityFrameworkCore;

namespace JobHuntAPI.Repository
{
	public class ApplicationContext(IConfiguration configuration) : DbContext
	{
		public DbSet<JobApplication> JobApplications { get; set; } = null!;
		public DbSet<PdfFile> PdfFiles { get; set; } = null!;
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseNpgsql(configuration.GetConnectionString("Postgres"));
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<JobApplication>()
				.HasOne(a => a.EncryptedApplicationPdf)
				.WithOne()
				.HasForeignKey<PdfFile>(p => p.JobApplicationId)
				.OnDelete(DeleteBehavior.Cascade);
			modelBuilder.Entity<JobApplication>()
				.HasOne(a => a.EncryptedResumePdf)
				.WithOne()
				.HasForeignKey<PdfFile>(p => p.JobApplicationId)
				.OnDelete(DeleteBehavior.Cascade);
		}

	}
}
