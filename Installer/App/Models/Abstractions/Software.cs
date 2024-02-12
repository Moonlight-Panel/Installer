namespace Installer.App.Models.Abstractions;

public abstract class Software
{
    public abstract string Name { get; }
    public abstract string Icon { get; }
    public abstract string Description { get; }
    public abstract object Config { get; set; }

    public List<string> Dependencies { get; } = new();

    public abstract Task Install(IServiceProvider provider, InstallationState state);
    
    public void AddDependency<T>() where T : SoftwareDependency
    {
        Dependencies.Add(typeof(T).FullName!);
    }
}