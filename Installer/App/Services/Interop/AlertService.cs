using Microsoft.JSInterop;

namespace Installer.App.Services.Interop;

public class AlertService
{
    private readonly IJSRuntime JsRuntime;

    public AlertService(IJSRuntime jsRuntime)
    {
        JsRuntime = jsRuntime;
    }

    public async Task Info(string title, string message)
    {
        await JsRuntime.InvokeVoidAsync("mwi.alerts.info", title, message);
    }
    
    public async Task Success(string title, string message)
    {
        await JsRuntime.InvokeVoidAsync("mwi.alerts.success", title, message);
    }
    
    public async Task Warning(string title, string message)
    {
        await JsRuntime.InvokeVoidAsync("mwi.alerts.warning", title, message);
    }
    
    public async Task Error(string title, string message)
    {
        await JsRuntime.InvokeVoidAsync("mwi.alerts.error", title, message);
    }
    
    public async Task<string> Text(string title, string message)
    {
        return await JsRuntime.InvokeAsync<string>("mwi.alerts.text", title, message);
    }
    
    public async Task<bool> YesNo(string title, string yes, string no)
    {
        try
        {
            return await JsRuntime.InvokeAsync<bool>("mwi.alerts.yesno", title, yes, no);
        }
        catch (Exception)
        {
            return false;
        }
    }
}