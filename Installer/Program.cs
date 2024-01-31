using Installer.App.Extensions;
using Installer.App.Helpers.LogMigrator;
using Installer.App.Services.Interop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Setup logger
var logConfig = new LoggerConfiguration();

logConfig = logConfig.Enrich.FromLogContext()
    .WriteTo.Console(
        outputTemplate:
        "{Timestamp:HH:mm:ss} [{Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}");

if (args.Contains("--debug") || builder.Environment.IsDevelopment())
    logConfig = logConfig.MinimumLevel.Debug();
else
    logConfig = logConfig.MinimumLevel.Information();

Log.Logger = logConfig.CreateLogger();

// Add services

builder.Services.AddScoped<AlertService>();
builder.Services.AddScoped<ClipboardService>();
builder.Services.AddScoped<CookieService>();
builder.Services.AddScoped<FileDownloadService>();
builder.Services.AddScoped<ModalService>();
builder.Services.AddScoped<ToastService>();

//

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Logging.ClearProviders();
builder.Logging.AddProvider(new LogMigrateProvider());

var config =
    new ConfigurationBuilder().AddJsonString(
        "{\"LogLevel\":{\"Default\":\"Information\",\"Microsoft.AspNetCore\":\"Warning\"}}");
builder.Logging.AddConfiguration(config.Build());

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();