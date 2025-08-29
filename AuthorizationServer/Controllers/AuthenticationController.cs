using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace AuthorizationServer.Controllers
{

	public class AuthenticationController : Controller
	{
		private readonly IOpenIddictApplicationManager _applicationManager;

		public AuthenticationController(IOpenIddictApplicationManager applicationManager)
			=> _applicationManager = applicationManager;

		[HttpPost("~/connect/token"), Produces("application/json")]
		public async Task<IActionResult> Exchange()
		{
			var request = HttpContext.GetOpenIddictServerRequest();
			if (request.IsClientCredentialsGrantType())
			{

				var application = await _applicationManager.FindByClientIdAsync(request.ClientId) ??
					throw new InvalidOperationException("The application cannot be found.");

				var identity = new ClaimsIdentity(TokenValidationParameters.DefaultAuthenticationType, Claims.Name, Claims.Role);

				identity.SetClaim(Claims.Subject, await _applicationManager.GetClientIdAsync(application));
				identity.SetClaim(Claims.Name, await _applicationManager.GetDisplayNameAsync(application));

				identity.SetDestinations(static claim => claim.Type switch
				{
					// Allow the "name" claim to be stored in both the access and identity tokens
					// when the "profile" scope was granted (by calling principal.SetScopes(...)).
					Claims.Name when claim.Subject.HasScope(Scopes.Profile)
						=> [Destinations.AccessToken, Destinations.IdentityToken],

					// Otherwise, only store the claim in the access tokens.
					_ => [Destinations.AccessToken]
				});

				return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
			}

			throw new NotImplementedException("The specified grant is not implemented.");
		}
	}
}
