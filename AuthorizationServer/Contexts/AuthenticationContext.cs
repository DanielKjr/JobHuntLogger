using Microsoft.EntityFrameworkCore;

namespace AuthorizationServer.Contexts
{
	/// <summary>
	/// Very basic authentication context, OpenIddict fills out what it needs 
	/// </summary>
	/// <param name="configuration"></param>
	public class AuthenticationContext(IConfiguration configuration) : DbContext
	{
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			//dbinfo comes from the secret mounted in docker compose
			optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
			optionsBuilder.UseOpenIddict();
			base.OnConfiguring(optionsBuilder);
		}
	}
}
