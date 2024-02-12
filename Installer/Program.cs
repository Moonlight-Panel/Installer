using Installer.App.Dependencies;
using Installer.App.Services;
using Installer.App.Softwares;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MoonCore.Extensions;
using MoonCore.Helpers;
using MoonCoreUI.Services;

var builder = WebApplication.CreateBuilder(args);

Logger.Setup(isDebug: args.Contains("--debug"));
builder.Logging.MigrateToMoonCore();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Services
builder.Services.AddScoped<CookieService>();
builder.Services.AddScoped<FileDownloadService>();
builder.Services.AddScoped<AlertService>();
builder.Services.AddScoped<ToastService>();
builder.Services.AddScoped<ClipboardService>();
builder.Services.AddScoped<ModalService>();

builder.Services.ConstructMoonCoreDi<Program>();

// Configure interop
ToastService.Prefix = "moonlight.toasts";
ModalService.Prefix = "moonlight.modals";
AlertService.Prefix = "moonlight.alerts";
ClipboardService.Prefix = "moonlight.clipboard";
FileDownloadService.Prefix = "moonlight.utils";

var config =
    new ConfigurationBuilder().AddJsonString(
        "{\"LogLevel\":{\"Default\":\"Information\",\"Microsoft.AspNetCore\":\"Warning\"}}");
builder.Logging.AddConfiguration(config.Build());

var app = builder.Build();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

var installationService = app.Services.GetRequiredService<InstallationService>();

// Software
await installationService.AddSoftware<MoonlightLegacySoftware>();

// Software dependencies
await installationService.AddSoftwareDependency<DockerDependency>();

app.Run();