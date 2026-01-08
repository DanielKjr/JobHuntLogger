using JobHuntApiService;
using JobHuntLogger.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web;
using Moq;
using System.Net.Http;
using System.Security.Claims;

namespace JobHuntLoggerTests.Setups
{
	/// <summary>
	/// Very crude class for now, seems excessive that it needs this much base setup, so I will return to that later.
	/// </summary>
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
		}

		public static void RegisterUnauthorizedSetup(IServiceCollection Services)
		{
			var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[] {new Claim("name", "Guest"),
		new Claim(ClaimTypes.Name, "Guest")}, "TestAuthentication"));

			var authenticationState = new AuthenticationState(claimsPrincipal);

			var authenticationStateProviderMock = new Mock<AuthenticationStateProvider>();
			authenticationStateProviderMock
				.Setup(m => m.GetAuthenticationStateAsync())
				.ReturnsAsync(authenticationState);

			var authenticationService = new AuthenticationService(authenticationStateProviderMock.Object);

			Services.AddSingleton<AuthenticationStateProvider>(authenticationStateProviderMock.Object);
			Services.AddSingleton<IAuthenticationService>(authenticationService);
		}

		public static void RegisterAuthorizedSetup(IServiceCollection Services)
		{

			var claimsIdentity = new ClaimsIdentity(new[]{
			new Claim("name", "NameOfUser"),
			new Claim(ClaimTypes.Name, "Guest")},authenticationType: "TestAuthentication" 
			);

			var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
			
			var authenticationState = new AuthenticationState(claimsPrincipal);

			var authenticationStateProviderMock = new Mock<AuthenticationStateProvider>();
			authenticationStateProviderMock
				.Setup(m => m.GetAuthenticationStateAsync())
				.ReturnsAsync(authenticationState);
			
			var authenticationService = new AuthenticationService(authenticationStateProviderMock.Object);
			Services.AddSingleton(authenticationStateProviderMock.Object);
			Services.AddSingleton<IAuthenticationService>(authenticationService);

		}
	}
}
