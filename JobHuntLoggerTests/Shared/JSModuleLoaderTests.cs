using System.Collections.Generic;
using System.Net.Http;
using Blazored.Toast.Services;
using JobHuntApiService;
using JobHuntLogger.Components.Shared;
using System.Reflection;
using JobHuntLoggerTests.Setups;
using Moq;
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace JobHuntLoggerTests.Shared
{
	[TestFixture]
	[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
	internal class JSModuleLoaderTests : BunitContext
	{

        [Test]
        public async Task ClipboardModule_Is_Loaded()
		{
			this.SetAuthorized();
			Services.AddSingleton<IToastService, ToastService>();
			Services.RegisterJobHuntApi();
			JSInterop.SetupModule("./js/clipboardModule.js");
            // Use the concrete service instance with the test IJSRuntime
            var jsRuntime = Services.GetRequiredService<IJSRuntime>();
            var loader = new JSModuleLoader(jsRuntime);
            await loader.RegisterAsync(ModuleType.ClipBoardModule);

            // Inspect private _modules dictionary to assert the module was loaded
            var modulesField = typeof(JSModuleLoader).GetField("_modules", BindingFlags.NonPublic | BindingFlags.Instance);
            var modules = (Dictionary<int, IJSObjectReference>?)modulesField.GetValue(loader);
            Assert.IsNotNull(modules);
            Assert.AreEqual(1, modules!.Count);
            Assert.IsTrue(modules.ContainsKey((int)ModuleType.ClipBoardModule));
		}

        [Test]
        public async Task ModuleLoaderLoadsMultipleModules()
		{
			Services.RegisterJobHuntApi();
			var apiClientMock = new Mock<JobHuntApiClient>("http://dummy", new HttpClient());

			JSInterop.SetupModule("./js/browserInterop.js");
			JSInterop.SetupModule("./js/pdfModule.js");

			Services.AddSingleton<JobHuntApiClient>(apiClientMock.Object);
            var jsRuntime = Services.GetRequiredService<IJSRuntime>();
            var loader = new JSModuleLoader(jsRuntime);
            await loader.RegisterAsync(ModuleType.PdfModule, ModuleType.BrowserInterop);

            var modulesField = typeof(JSModuleLoader).GetField("_modules", BindingFlags.NonPublic | BindingFlags.Instance);
            var modules = (Dictionary<int, IJSObjectReference>?)modulesField.GetValue(loader);
            Assert.IsNotNull(modules);
            Assert.AreEqual(2, modules!.Count);
            Assert.IsTrue(modules.ContainsKey((int)ModuleType.PdfModule));
            Assert.IsTrue(modules.ContainsKey((int)ModuleType.BrowserInterop));
		}



	}
}
