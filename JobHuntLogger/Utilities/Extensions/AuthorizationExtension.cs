﻿using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;

namespace JobHuntLogger.Utilities.Extensions
{
	public static class AuthorizationExtension
	{
		public static IServiceCollection RegisterAuthorization(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
				.AddMicrosoftIdentityWebApp(configuration.GetSection("Entra:Blazor"))
				.EnableTokenAcquisitionToCallDownstreamApi(new[] { "User.Read" })
				.AddMicrosoftGraph().
				AddDistributedTokenCaches();
			// Ensure cookie lifetime is independent from token lifetime and lasts 14 days with sliding expiration.
			services.Configure<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme, static options =>
			{
				options.ExpireTimeSpan = TimeSpan.FromDays(14);
				options.SlidingExpiration = true;
				options.Cookie.IsEssential = true;
				options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
				options.Cookie.HttpOnly = true;

				//Attempt to persist tokens so that you don't have to login daily
				options.Events = new CookieAuthenticationEvents
				{
					OnSigningIn = async ctx =>
					{
						ctx.Properties.IsPersistent = true;
						ctx.Properties.ExpiresUtc = DateTimeOffset.UtcNow.AddDays(14);
						await Task.CompletedTask;

					}
				};


			});

			// Prevent the OIDC middleware from overriding token lifetime
			services.Configure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme, options =>
			{
				options.UseTokenLifetime = false;

			});
			services.AddAuthorization();
			services.AddCascadingAuthenticationState();
			return services;
		}
	}
}
