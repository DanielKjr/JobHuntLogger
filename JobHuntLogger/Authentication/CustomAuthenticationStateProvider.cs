using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Security.Claims;

namespace JobHuntLogger.Authentication
{
	public class CustomAuthenticationStateProvider(ProtectedSessionStorage protectedSessionStorage) : AuthenticationStateProvider
	{
		private readonly ProtectedSessionStorage protectedSessionStorage = protectedSessionStorage;

		private ClaimsPrincipal anonymous = new ClaimsPrincipal(new ClaimsIdentity());


		public override async Task<AuthenticationState> GetAuthenticationStateAsync()
		{
			try
			{
				var userSessionStorageResult = await protectedSessionStorage.GetAsync<UserSession>("UserSession");
				var userSession = userSessionStorageResult.Success ? userSessionStorageResult.Value : null;

				if (userSession == null)
					return await Task.FromResult(new AuthenticationState(anonymous));

				var claimPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
				{
					new Claim(ClaimTypes.Name, userSession.UserName),
					new Claim(ClaimTypes.Role, userSession.Role)
				}, "CustomAuth"));

				return await Task.FromResult(new AuthenticationState(claimPrincipal));
			}
			catch
			{

				return await Task.FromResult(new AuthenticationState(anonymous));
			}

		}
		public async Task UpdateAuthenticationState(UserSession userSession)
		{
			ClaimsPrincipal claimsPrincipal;
			if (userSession != null)
			{
				await protectedSessionStorage.SetAsync("UserSession", userSession);
				claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>()
				{
					new Claim(ClaimTypes.Name, userSession.UserName),
					new Claim(ClaimTypes.Role, userSession.Role)
				}));

			}
			else
			{
				await protectedSessionStorage.DeleteAsync("UserSession");
				claimsPrincipal = anonymous;
			}

			NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
		}

	}
}
