using JobHuntApiService;
using JobHuntLogger.Services.Authentication;
using JobHuntLogger.Services.Authorization;

namespace JobHuntLogger.Utilities.ServiceExtentions
{
	public static class ApiConfigExtention
	{
		public static IServiceCollection AddApiConfiguration(this IServiceCollection services, IConfiguration configuration)
		{
			
			string baseUrl = configuration["JobHuntApi:BaseUrl"]!;
			services.AddTransient<TokenFetcherService>();
			services.AddScoped<ITokenProvider, TokenProvider>();
			services.AddTransient<BearerTokenHandler>();

			services.AddHttpClient<JobHuntApiClient>((sp, client) =>
			{
				client.BaseAddress = new Uri(baseUrl);
			})
			.AddHttpMessageHandler<BearerTokenHandler>();

			services.AddScoped(sp =>
			{
				
				var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
				var httpClient = httpClientFactory.CreateClient(nameof(JobHuntApiClient));
				return new JobHuntApiClient(baseUrl, httpClient);
			});
			return services;
		}
		
	}
}
