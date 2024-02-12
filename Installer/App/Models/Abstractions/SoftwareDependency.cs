namespace Installer.App.Models.Abstractions;

public abstract class SoftwareDependency
{
    public abstract string Name { get; }
    public abstract string Icon { get; }
    public abstract string Description { get; }
    public abstract object Config { get; set; }

    public abstract Task Install(IServiceProvider provider, InstallationState state);

    public abstract Task<bool> IsFulfilled(IServiceProvider provider);
}