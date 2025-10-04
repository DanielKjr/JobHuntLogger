using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace JobHuntLogger.Services
{
	public class AuthenticationService(AuthenticationStateProvider _authenticationStateProvider)
	{
		public async Task<bool> IsUserAuthenticatedAsync()
		{
			var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
			return authState.User.Identity?.IsAuthenticated ?? false;
		}

		public async Task<string> GetUserNameAsync()
		{
			var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
			return authState.User.Identity?.IsAuthenticated == true
				? authState.User.Claims.FirstOrDefault(c => c.Type == "name")?.Value ?? "Unknown"
				: "Guest";
		}

		public async Task<ClaimsPrincipal> GetUserAsync()
		{
			var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
			return authState.User;
		}
	}
}
