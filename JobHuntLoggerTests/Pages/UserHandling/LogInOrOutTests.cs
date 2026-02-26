using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using AngleSharp.Dom;
using Blazored.Toast.Services;
using JobHuntApiService;
using JobHuntLogger.Components.Pages.UserHandling;
using JobHuntLogger.Components.Shared;
using JobHuntLogger.Services.Authentication;
using JobHuntLoggerTests.Setups;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web;
using Moq;

namespace JobHuntLoggerTests.Pages.UserHandling
{
	[TestFixture]
	[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
	class LogInOrOutTests : BunitContext
	{


		[Test]
		public void UserActionUnAuthorizedRendersLogIn()
		{

			Services.RegisterJobHuntApi();
			var auth = AddAuthorization();
			Services.AddTransient<IJSModuleLoader, JSModuleLoader>();
			Services.AddSingleton<IToastService, ToastService>();
			JSInterop.SetupModule("./js/clipboardModule.js");


			var cut = Render<CascadingAuthenticationState>(parameters => parameters
				.AddChildContent<LoginOrOut>()
			);

			cut.Find("div.name").MarkupMatches(@"<div class=""name"">Guest</div>");
			var loginButton = cut.Find("button.b1");
			Assert.AreEqual("Login", loginButton.TextContent);
		}

		[Test]
		public void UserActionLogoutButtonNavigatesToMicrosoft()
		{
			this.SetAuthorized();
			Services.RegisterJobHuntApi();
			Services.AddTransient<IJSModuleLoader, JSModuleLoader>();
			JSInterop.SetupModule("./js/clipboardModule.js");
			Services.AddSingleton<IToastService, ToastService>();
			var cut = Render<CascadingAuthenticationState>(parameters => parameters
				.AddChildContent<LoginOrOut>()
			);

			NavigationManager? navigationManager = Services.GetRequiredService<NavigationManager>() as NavigationManager;
			IElement logoutButton = cut.Find("button.b1");
			Assert.AreEqual("Logout", logoutButton.TextContent);
			logoutButton.Click();
			string url = navigationManager!.Uri;
			Assert.AreEqual("http://localhost/Account/LogOut", url);

		}

		[Test]
		public void UserActionAuthorizedRendersLogOut()
		{
			this.SetAuthorized();
			Services.RegisterJobHuntApi();
			Services.AddTransient<IJSModuleLoader, JSModuleLoader>();
			JSInterop.SetupModule("./js/clipboardModule.js");
			Services.AddSingleton<IToastService, ToastService>();

			var cut = Render<CascadingAuthenticationState>(parameters => parameters
				.AddChildContent<LoginOrOut>()
			);


			var username = cut.Find("div.name");
			var logoutButton = cut.Find("button.b1");
			Assert.AreEqual("Logout", logoutButton.TextContent);
			Assert.AreEqual("NameOfUser", username.TextContent);
		}


		[Test]
		public void UserActionLoginButtonNavigatesToMicrosoft()
		{
			Services.RegisterJobHuntApi();
			var auth = AddAuthorization();
			Services.AddTransient<IJSModuleLoader, JSModuleLoader>();
			JSInterop.SetupModule("./js/clipboardModule.js");
			Services.AddSingleton<IToastService, ToastService>();
			IRenderedComponent<CascadingAuthenticationState>? cut = Render<CascadingAuthenticationState>(parameters => parameters
				.AddChildContent<LoginOrOut>()
			);

			NavigationManager? navigationManager = Services.GetRequiredService<NavigationManager>() as NavigationManager;
			IElement loginButton = cut.Find("button.b1");
			Assert.AreEqual("Login", loginButton.TextContent);
			loginButton.Click();
			string url = navigationManager!.Uri;
			Assert.AreEqual("http://localhost/Account/Login", url);


		}

		[Test]
		public void UserActionsDisplayAuthorizing()
		{
			Services.RegisterJobHuntApi();
			Services.AddTransient<IJSModuleLoader, JSModuleLoader>();
			JSInterop.SetupModule("./js/clipboardModule.js");
			AddAuthorization().SetAuthorizing();
			Services.AddSingleton<IToastService, ToastService>();
			var cut = Render<CascadingAuthenticationState>(parameters => parameters
				.AddChildContent<LoginOrOut>()
			);
			var userAction = cut.Find("div.name");

			Assert.AreEqual("Authorizing...", userAction.TextContent);

		}

		[Test]
		public void ClickOnUserNameCopiesID()
		{
			//TODO need to simplify this. This is a niche case where i need to test
			//the fetched token and have to verify it, but its not pretty
			this.SetAuthorized();
			var expectedGuid = Guid.NewGuid().ToString();
			var tokenAcquisitionMock = new Mock<ITokenAcquisition>();
			tokenAcquisitionMock.Setup(t => t.GetAccessTokenForUserAsync(
				It.IsAny<IEnumerable<string>>(), It.IsAny<string>(),
				It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(),
				It.IsAny<TokenAcquisitionOptions>())).ReturnsAsync(expectedGuid);

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
			Services.AddSingleton<IToastService, ToastService>();
			Services.AddTransient<IJSModuleLoader, JSModuleLoader>();
			var clipboard = JSInterop.SetupModule("./js/clipboardModule.js");
			clipboard.Setup<bool>("copyTextToClipboard", args => true)
				.SetResult(true);

			var cut = Render<CascadingAuthenticationState>(parameters => parameters
				.AddChildContent<LoginOrOut>()
			);

			cut.Find("div.name").Click();

			cut.WaitForAssertion(() =>
			{
				var invocation = clipboard.Invocations
					.Single(i => i.Identifier == "copyTextToClipboard");

				Assert.AreEqual(expectedGuid, invocation.Arguments[0]);
			});

		}

	}
}
