using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace JobHuntLogger.Components.Shared
{
	public abstract class JsBaseComponent : ComponentBase
	{
		[Inject] protected IJSRuntime JS { get; set; } = default!;
		[Parameter] public string JsModuleName { get; set; } = string.Empty;
		protected IJSObjectReference? JsModule;

		protected override async Task OnAfterRenderAsync(bool firstRender)
		{
			if (!firstRender) return;

			//var modules = Directory.GetFiles("wwwroot/js");
			//if (!JsModuleName.EndsWith(".js"))
			//	JsModuleName += ".js";
			//if (modules.Any(m => m.EndsWith(JsModuleName)))
			JsModule = await JS.InvokeAsync<IJSObjectReference>("import", $"./js/{JsModuleName}.js");
		}
	}
}
