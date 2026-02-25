using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using Blazored.Toast.Services;
using JobHuntApiService;
using JobHuntLogger.Components.Pages.UserHandling;
using JobHuntLogger.Components.Pages.Utilities;
using JobHuntLogger.Components.Shared;
using JobHuntLogger.Services.Authentication;
using JobHuntLoggerTests.Setups;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web;
using Moq;

namespace JobHuntLoggerTests.Shared
{
	[TestFixture]
	[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
	internal class JSModuleLoaderTests : BunitContext
	{

		[Test]
		public void ClipboardModule_Is_Loaded()
		{
			this.SetAuthorized();
			Services.AddSingleton<IToastService, ToastService>();
			Services.RegisterJobHuntApi();
			JSInterop.SetupModule("./js/clipboardModule.js");
			var cut = Render<CascadingAuthenticationState>(parameters => parameters
			.AddChildContent<LoginOrOut>()
		);
			

			var loader = cut.FindComponent<JsModuleLoader>();

			Assert.AreEqual(ModuleType.ClipBoardModule, loader.Instance.LoadedModules.Single());
		}

		[Test]
		public void ModuleLoaderLoadsMultipleModules()
		{
			Services.RegisterJobHuntApi();
			var apiClientMock = new Mock<JobHuntApiClient>("http://dummy", new HttpClient());

			JSInterop.SetupModule("./js/browserInterop.js");
			JSInterop.SetupModule("./js/pdfModule.js");

			Services.AddSingleton<JobHuntApiClient>(apiClientMock.Object);
			var cut = Render<PdfPreviewComponent>();

			var loadedModules = cut.Instance.LoadedModules;
			Assert.IsNotNull(loadedModules);
			Assert.AreEqual(2, loadedModules.Length);
			Assert.Contains(ModuleType.PdfModule, loadedModules);
			Assert.Contains(ModuleType.BrowserInterop, loadedModules);
		}



	}
}
