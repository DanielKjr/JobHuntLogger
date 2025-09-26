using Microsoft.Identity.Web;
using System.Net.Http.Headers;

namespace JobHuntLogger.Components.Model.Authentication
{
	public class JobHuntApiService(ITokenAcquisition tokentAcquisition, IHttpClientFactory httpClientFactory, IConfiguration configuration)
	{


		public async Task<HttpResponseMessage> CallApi()
		{
			var scopes = configuration["Entra:Blazor:Scopes"]?.Split(' ') ?? Array.Empty<string>();
			string accessToken = await tokentAcquisition.GetAccessTokenForUserAsync(scopes);
			var httpClient = httpClientFactory.CreateClient();
			httpClient.BaseAddress = new Uri("http://jobhuntapi:8080/");
			httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
			return await httpClient.GetAsync("authenticate/temp");
		}
	}
}
