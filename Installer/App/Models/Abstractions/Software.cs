namespace Installer.App.Models.Abstractions;

public abstract class Software
{
    public string Name { get; set; }
    public string Icon { get; set; }
    public string Description { get; set; }
    public object Config { get; set; }

    public abstract Task Install(InstallationState installationState);
}