
using Microsoft.Identity.Web;

namespace JobHuntLogger.Services.Authorization
{
	public class TokenProvider(ITokenAcquisition tokentAcquisition, IConfiguration configuration) : ITokenProvider
	{
		public async Task<string> GetTokenAsync()
		{
			var scopes = configuration["Entra:Blazor:Scopes"]?.Split(' ') ?? Array.Empty<string>();
			return await tokentAcquisition.GetAccessTokenForUserAsync(scopes);
		}
	}
}
