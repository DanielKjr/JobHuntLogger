
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace AuthorizationServer.Controllers;

[ApiController]
public class AuthorizationController(LoginClient authenticationClient) : ControllerBase
{
	/// <summary>
	/// Endpoint exists just to remind to user other endpoint when mixing up http request
	/// </summary>
	/// <returns></returns>
	/// <exception cref="InvalidOperationException"></exception>
	[HttpGet("~/connect/authorize")]
	public IActionResult Authorize()
	{
		var request = HttpContext.GetOpenIddictServerRequest()
			?? throw new InvalidOperationException("OpenID Connect request not available.");

		return Ok(new { message = "POST credentials to this endpoint with the same query string." });
	}


	[HttpPost("~/connect/authorize")]
	[Consumes("application/x-www-form-urlencoded")]
	public async Task<IActionResult> AuthorizePost([FromForm] string userName, [FromForm] string password)
	{
		var request = HttpContext.GetOpenIddictServerRequest()
			?? throw new InvalidOperationException("OpenID Connect request not available.");

		//request to jobhuntapi api
		var valid = await authenticationClient.LoginAsync(new UserDto
		{
			UserName = userName,
			Password = password
		});

		if (!valid)
			return Forbid(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

		var identity = new ClaimsIdentity(
			TokenValidationParameters.DefaultAuthenticationType,
			Claims.Name, Claims.Role);

		identity.SetClaim(Claims.Subject, userName);
		identity.SetClaim(Claims.Name, userName);

		identity.SetScopes(request.GetScopes());
		identity.SetDestinations(_ => [Destinations.AccessToken, Destinations.IdentityToken]);

		return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
	}

	[HttpPost("~/connect/newUser")]
	[Consumes("application/x-www-form-urlencoded")]
	public async Task<IActionResult> RegisterAndAuthorize([FromForm] string userName, [FromForm] string password)
	{
		try
		{
			var created = await authenticationClient.CreateAsync(new UserDto
			{
				UserName = userName,
				Password = password
			});

			if (!created)
				return Forbid(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);


		}
		catch (ApiException e)
		{
			string errorMessage = e.Response;
			try
			{
				var errorObj = JsonConvert.DeserializeObject<Dictionary<string, string>>(errorMessage);
				if (errorObj != null && errorObj.TryGetValue("error", out var msg))
					return BadRequest(msg);
			}
			catch
			{
				
				return BadRequest(errorMessage);
			}

			return BadRequest("Failed to create user.");
		}

		return Ok("User has been created.");
	}

}

