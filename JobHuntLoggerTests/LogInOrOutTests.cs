using System.Security.Claims;
using AngleSharp.Dom;
using Blazored.Toast.Services;
using JobHuntLogger.Components.Pages.UserHandling;
using JobHuntLoggerTests.Setups;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace JobHuntLoggerTests
{
	[TestFixture]
	[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
	class LogInOrOutTests : BunitContext
	{


		[Test]
		public void UserActionUnAuthorizedRendersLogIn()
		{

			ServiceSetups.RegisterJobHuntApi(Services);
			var auth = AddAuthorization();
		
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
		public void UserActionAuthorizedRendersLogOut()
		{

			AddAuthorization().SetAuthorized("NameOfUser").SetClaims(new Claim("name", "NameOfUser"), new Claim(ClaimTypes.Name, "Guest"));
			ServiceSetups.RegisterJobHuntApi(Services);
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
			ServiceSetups.RegisterJobHuntApi(Services);
			var auth = AddAuthorization();
			
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
			ServiceSetups.RegisterJobHuntApi(Services);
			JSInterop.SetupModule("./js/clipboardModule.js");
			AddAuthorization().SetAuthorizing();
			Services.AddSingleton<IToastService, ToastService>();
			var cut = Render<CascadingAuthenticationState>(parameters => parameters
				.AddChildContent<LoginOrOut>()
			);
			var userAction = cut.Find("div.name");

			Assert.AreEqual("Authorizing...", userAction.TextContent);

		}


	}
}
