namespace Installer.App.Models.Abstractions;

public abstract class SoftwareDependency
{
    public string Name { get; set; }
    public string Icon { get; set; }
    public string Description { get; set; }
    public bool IsAutomatic { get; set; }
    public object Config { get; set; }

    public abstract Task Install(InstallationState installationState);
    public abstract Task<bool> CheckFulfilled();
}