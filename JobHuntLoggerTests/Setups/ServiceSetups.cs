using System.Net.Http;
using System.Security.Claims;
using JobHuntApiService;
using JobHuntLogger.Services.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web;
using Moq;

namespace JobHuntLoggerTests.Setups
{

	public static class ServiceSetups
	{
		public static BunitAuthorizationContext SetAuthorized(this BunitContext context)
		{
			return context.AddAuthorization().SetAuthorized("NameOfUser").SetClaims(new Claim("name", "NameOfUser"), new Claim(ClaimTypes.Name, "Guest"));

		}
		public static IServiceCollection RegisterJobHuntApi(this IServiceCollection services)
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

			services.AddSingleton<TokenFetcherService>(jobHuntApiService);

			services.AddSingleton<IAuthenticationService, AuthenticationService>();
			return services;
		}


	}
}
