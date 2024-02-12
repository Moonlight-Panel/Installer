using Installer.App.Actions.Dependencies;
using Installer.App.Actions.Panel;
using Installer.App.Services;
using MoonCore.Extensions;
using MoonCore.Helpers;
using MoonCoreUI.Services;

var builder = WebApplication.CreateBuilder(args);

// Setup logger
Logger.Setup(isDebug: args.Contains("--debug"));
builder.Logging.MigrateToMoonCore();

// Add services
builder.Services.ConstructMoonCoreDi<Program>();

builder.Services.AddScoped<CookieService>();
builder.Services.AddScoped<FileDownloadService>();
builder.Services.AddScoped<AlertService>();
builder.Services.AddScoped<ToastService>();
builder.Services.AddScoped<ClipboardService>();
builder.Services.AddScoped<ModalService>();

// Configure interop
ToastService.Prefix = "mwi.toasts";
ModalService.Prefix = "mwi.modals";
AlertService.Prefix = "mwi.alerts";
ClipboardService.Prefix = "mwi.clipboard";
FileDownloadService.Prefix = "mwi.utils";

//

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

var config =
    new ConfigurationBuilder().AddJsonString(
        "{\"LogLevel\":{\"Default\":\"Information\",\"Microsoft.AspNetCore\":\"Warning\"}}");
builder.Logging.AddConfiguration(config.Build());

var app = builder.Build();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

var installerService = app.Services.GetRequiredService<InstallerService>();

// Software
await installerService.RegisterSoftware<MoonlightLegacySoftware>();

// Software dependencies
await installerService.RegisterSoftwareDependency<DockerDependency>();

app.Run();