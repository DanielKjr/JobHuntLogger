using AngleSharp.Dom;
using Blazored.Toast.Services;
using JobHuntLoggerTests.Setups;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using JobHuntLogger.Components.Pages.UserHandling;

namespace JobHuntLoggerTests
{
	[TestFixture]
	[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
	 class LogInOrOutTests : BunitContext
	{

		[SetUp]
		public void Setup()
		{
			AddAuthorization();
		}

		[Test]
		public void UnAuthorizedRendersLogIn()
		{
			// Arrange

			ServiceSetups.RegisterJobHuntApi(Services);
			ServiceSetups.RegisterUnauthorizedSetup(Services);
			Services.AddSingleton<IToastService, ToastService>();
			JSInterop.SetupModule("./js/clipboardModule.js");


			// Act
			var cut = Render<CascadingAuthenticationState>(parameters => parameters
				.AddChildContent<LoginOrOut>()
			);

			// Assert

			cut.Find("div.name").MarkupMatches(@"<div class=""name"">Guest</div>");
			var loginButton = cut.Find("button.b1");
			Assert.AreEqual("Login", loginButton.TextContent);
		}

		[Test]
		public void AuthorizedRendersLogOut()
		{

			ServiceSetups.RegisterJobHuntApi(Services);
			ServiceSetups.RegisterAuthorizedSetup(Services);
			JSInterop.SetupModule("./js/clipboardModule.js");
			AddAuthorization().SetAuthorized("NameOfUser");
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
		public void LoginButtonNavigatesToMicrosoftSite()
		{
			ServiceSetups.RegisterJobHuntApi(Services);
			ServiceSetups.RegisterUnauthorizedSetup(Services);
			
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
		public void UserIsClearedOnLogout()
		{
			ServiceSetups.RegisterJobHuntApi(Services);
			ServiceSetups.RegisterAuthorizedSetup(Services);
			JSInterop.SetupModule("./js/clipboardModule.js");
			AddAuthorization().SetAuthorized("NameOfUser");
			Services.AddSingleton<IToastService, ToastService>();
			var cut = Render<CascadingAuthenticationState>(parameters => parameters
				.AddChildContent<LoginOrOut>()
			);

			NavigationManager? navigationManager = Services.GetRequiredService<NavigationManager>() as NavigationManager;
			IElement logoutButton = cut.Find("button.b1");
			Assert.AreEqual("Logout", logoutButton.TextContent);
			var username = cut.Find("div.name");
			Assert.AreEqual("NameOfUser", username.TextContent);
			logoutButton.Click();
			string url = navigationManager!.Uri;

			Assert.AreEqual("http://localhost/Account/LogOut", url);
			var s = Render<CascadingAuthenticationState>();

			// var authService = Services.GetRequiredService<CascadingAuthenticationState>() as CascadingAuthenticationState;

		}
	}
}
