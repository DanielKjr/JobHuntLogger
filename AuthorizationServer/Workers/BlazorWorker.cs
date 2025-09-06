using AuthorizationServer.Contexts;
using OpenIddict.Abstractions;

namespace AuthorizationServer.Workers
{

	public class BlazorWorker(IServiceProvider serviceProvider) : IHostedService
	{
		public async Task StartAsync(CancellationToken cancellationToken)
		{
			using var scope = serviceProvider.CreateScope();

			var context = scope.ServiceProvider.GetRequiredService<AuthenticationContext>();
			await context.Database.EnsureCreatedAsync(cancellationToken);

			var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

			var existing = await manager.FindByClientIdAsync("blazor-worker", cancellationToken);

			var descriptor = new OpenIddictApplicationDescriptor
			{
				ClientId = "blazor-worker",
				ClientSecret = "388D45FA-B36B-4988-BA59-C187D329C207",
				ClientType = OpenIddictConstants.ClientTypes.Confidential,
				//RedirectUris = { new Uri("https://localhost:44396/signin-oidc") },
				//above is when running IIS express locally, below is in docker env
				RedirectUris =
				{
					new Uri("http://localhost:8000/signin-oidc"),
					new Uri("http://localhost:8180/signin-oidc")
				},

				Permissions =
				{
					OpenIddictConstants.Permissions.Endpoints.Authorization,
					OpenIddictConstants.Permissions.Endpoints.Token,
					OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
					OpenIddictConstants.Permissions.ResponseTypes.Code,
					OpenIddictConstants.Permissions.Prefixes.Scope + "openid",
					OpenIddictConstants.Permissions.Prefixes.Scope + "profile"
					//OpenIddictConstants.Permissions.Scopes.Profile
				},
				Requirements =
				{
					OpenIddictConstants.Requirements.Features.ProofKeyForCodeExchange
				}
			};


			//if we only do update it would fail because the worker instance was never created
			//so check if there's an instance or not
			if (existing is null)
			{
				await manager.CreateAsync(descriptor, cancellationToken);
			}
			else
			{
				await manager.UpdateAsync(existing, descriptor, cancellationToken);
			}
		}

		public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
	}
}

