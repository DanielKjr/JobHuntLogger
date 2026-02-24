using System.Diagnostics;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace JobHuntLogger.Components.Shared
{
	/// <summary>
	/// Not as elegant as I'd prefer but it gets the job done. 
	/// Uses ModuleType enum to match up which modules to load, allowing the child classes to access JsModules with enum index
	/// </summary>
	public abstract partial class JsModuleLoader : ComponentBase
	{
		protected string[] Modules = ["browserInterop", "clipboardModule", "fileInputInteropModule", "pdfModule"];
		protected Dictionary<int, IJSObjectReference>? JsModules = new Dictionary<int, IJSObjectReference>();
		private ModuleType[] loadedModules = [];
		public ModuleType[] LoadedModules => loadedModules;
		protected void SetModules(params ModuleType[] _loadedModules) => this.loadedModules = _loadedModules;

		[Inject] IJSRuntime JS { get; set; }

		protected override async Task OnAfterRenderAsync(bool firstRender)
		{

			if (!firstRender)
				return;

			foreach (var module in loadedModules)
			{
				var ijsObj = await JS.InvokeAsync<IJSObjectReference>("import", $"./js/{Modules[(int)module]}.js");
				JsModules!.Add((int)module, ijsObj);

			}
		}
	}
}
