using Installer.App.Models.Abstractions;

namespace Installer.App.Actions.Panel;

public class PanelSoftware : Software
{
    public PanelSoftware()
    {
        Name = "Moonlight Panel v1b";
        Description = "The ";
    }
    
    public override Task Install(InstallationState installationState)
    {
        throw new NotImplementedException();
    }
}