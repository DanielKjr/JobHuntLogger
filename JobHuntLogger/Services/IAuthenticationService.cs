using System.Security.Claims;

namespace JobHuntLogger.Services
{
	public interface IAuthenticationService
	{
		Task<ClaimsPrincipal> GetUserAsync();
		Task<Guid?> GetUserIdAsync();
		Task<string> GetUserNameAsync();
		Task<bool> IsUserAuthenticatedAsync();
	}
}