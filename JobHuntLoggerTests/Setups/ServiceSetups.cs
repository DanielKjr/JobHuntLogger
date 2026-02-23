using System.Net.Http;
using JobHuntApiService;
using JobHuntLogger.Services.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web;
using Moq;

namespace JobHuntLoggerTests.Setups
{

	public static class ServiceSetups
	{

		public static void RegisterJobHuntApi(IServiceCollection Services)
		{
			var tokenAcquisitionMock = new Mock<ITokenAcquisition>();
			var httpClientFactoryMock = new Mock<IHttpClientFactory>();
			var configurationMock = new Mock<IConfiguration>();
			var apiClientMock = new Mock<JobHuntApiClient>("http://dummy", new HttpClient());

			var jobHuntApiService = new TokenFetcherService(
				tokenAcquisitionMock.Object,
				httpClientFactoryMock.Object,
				configurationMock.Object,
				apiClientMock.Object
			);
			Services.AddSingleton<TokenFetcherService>(jobHuntApiService);

			Services.AddSingleton<IAuthenticationService, AuthenticationService>();
		}


	}
}
