using Installer.App.Models.Abstractions;
using MoonCore.Attributes;
using MoonCore.Helpers;

namespace Installer.App.Services;

[Singleton]
public class InstallationService
{
    public readonly List<Software> Softwares = new();
    public readonly List<SoftwareDependency> SoftwareDependencies = new();

    public InstallationState InstallationState { get; private set; } = new();
    public SoftwareDependency? CurrentInstallingDependency { get; private set; }

    private readonly IServiceProvider ServiceProvider;

    public InstallationService(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }

    public Task<SoftwareDependency[]> ResolveDependencies(Software software)
    {
        var result = SoftwareDependencies
            .Where(x => software.Dependencies.Any(y => y == x.GetType().FullName!))
            .ToArray();

        return Task.FromResult(result);
    }

    public async Task Install(Software software)
    {
        using var scope = ServiceProvider.CreateScope();
        var provider = scope.ServiceProvider;
        
        var dependencies = await ResolveDependencies(software);
        
        foreach (var dependency in dependencies)
        {
            if (!await dependency.IsFulfilled(provider))
            {
                CurrentInstallingDependency = dependency;
                
                await dependency.Install(provider, InstallationState);

                CurrentInstallingDependency = null;
            }
        }

        await software.Install(provider, InstallationState);
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