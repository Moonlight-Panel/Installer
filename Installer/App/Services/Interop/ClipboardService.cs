using Microsoft.JSInterop;

namespace Installer.App.Services.Interop;

public class ClipboardService
{
    private readonly IJSRuntime JsRuntime;

    public ClipboardService(IJSRuntime jsRuntime)
    {
        JsRuntime = jsRuntime;
    }

    public async Task Copy(string content)
    {
        await JsRuntime.InvokeVoidAsync("mwi.clipboard.copy", content);
    }
}