using Installer.App.Models.Abstractions;
using MoonCore.Attributes;
using MoonCore.Helpers;

namespace Installer.App.Services;

[Singleton]
public class InstallationService
{
    public readonly List<Software> Softwares = new();
    public readonly List<SoftwareDependency> SoftwareDependencies = new();

    public InstallationState? InstallationState { get; private set; }
    public bool IsInstalling { get; set; } = false;

    public Task<SoftwareDependency[]> ResolveDependencies(Software software)
    {
        var result = SoftwareDependencies
            .Where(x => software.Dependencies.Any(y => y == x.GetType().FullName!))
            .ToArray();

        return Task.FromResult(result);
    }

    public Task AddSoftware<T>() where T : Software
    {
        var software = Activator.CreateInstance<T>() as Software;
        Softwares.Add(software);
        
        Logger.Info($"Loaded software '{software.Name}'");
        
        return Task.CompletedTask;
    }
    
    public Task AddSoftwareDependency<T>() where T : SoftwareDependency
    {
        var softwareDependency = Activator.CreateInstance<T>() as SoftwareDependency;
        SoftwareDependencies.Add(softwareDependency);
        
        Logger.Info($"Loaded software dependency '{softwareDependency.Name}'");
        
        return Task.CompletedTask;
    }
}