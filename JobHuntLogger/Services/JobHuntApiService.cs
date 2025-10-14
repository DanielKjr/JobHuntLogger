using JobHuntApi;
using Microsoft.Identity.Web;
using System.Net.Http.Headers;

namespace JobHuntLogger.Services
{
	public class JobHuntApiService(ITokenAcquisition tokentAcquisition, IHttpClientFactory httpClientFactory,
		IConfiguration configuration, JobHuntApiClient jobHuntApiClient)
	{


		public async Task<HttpResponseMessage> CallApi()
		{
			var scopes = configuration["Entra:Blazor:Scopes"]?.Split(' ') ?? Array.Empty<string>();
			string accessToken = await tokentAcquisition.GetAccessTokenForUserAsync(scopes);
			var httpClient = httpClientFactory.CreateClient();
			httpClient.BaseAddress = new Uri(configuration["JobHuntApi:BaseUrl"]!);
			httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
			return await httpClient.GetAsync("Application/temp");


			//return await jobHuntApiClient.TempAsync();

		}
		public async Task<string> CallApiClient()
		{
			return await jobHuntApiClient.TempAsync();
		}

		public async Task<string> GetToken()
		{
			var scopes = configuration["Entra:Blazor:Scopes"]?.Split(' ') ?? Array.Empty<string>();
			return await tokentAcquisition.GetAccessTokenForUserAsync(scopes);
		}
	}
}
