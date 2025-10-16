using JobHuntApiService;
using JobHuntLogger.Services;
using JobHuntLogger.Services.Authorization;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;

namespace JobHuntLogger.Utilities
{
	public static class ApiConfigExtentions
	{
		public static IServiceCollection AddApiConfiguration(this IServiceCollection services, IConfiguration configuration)
		{
			
			string baseUrl = configuration["JobHuntApi:BaseUrl"]!;
			services.AddTransient<TokenFetcher>();
			services.AddScoped<ITokenProvider, TokenProvider>();
			services.AddTransient<BearerTokenHandler>();
			services.AddHttpClient<JobHuntApiClient>((sp, client) =>
			{
				client.BaseAddress = new Uri(baseUrl);
			})
			.AddHttpMessageHandler<BearerTokenHandler>();

			services.AddScoped<JobHuntApiClient>(sp =>
			{
				// Use the named client so the handler is included
				var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
				var httpClient = httpClientFactory.CreateClient(nameof(JobHuntApiClient));
				return new JobHuntApiClient(baseUrl, httpClient);
			});
			return services;
		}
		public static IServiceCollection ConfigureAuthenticationAndAuthorization(this IServiceCollection services, ConfigurationManager configuration )
		{
		
			//Entra with SQL distributed token cache
			services.AddDistributedSqlServerCache(options =>
			{
				options.ConnectionString = configuration.GetConnectionString("TokenDb");
				options.SchemaName = "Tokens";
				options.TableName = "TokenCache";
			});
			services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
				.AddMicrosoftIdentityWebApp(configuration.GetSection("Entra:Blazor"))
				.EnableTokenAcquisitionToCallDownstreamApi(new[] { "User.Read" })
				.AddMicrosoftGraph().
				AddDistributedTokenCaches();


			services.AddAuthorization();
			services.AddCascadingAuthenticationState();
			return services;
		}
	}
}
