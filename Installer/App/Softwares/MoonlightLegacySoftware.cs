using Installer.App.Dependencies;
using Installer.App.Models.Abstractions;
using Installer.App.Models.Configs;

namespace Installer.App.Softwares;

public class MoonlightLegacySoftware : Software
{
    public override string Name => "Moonlight Legacy (v1b)";
    public override string Icon => "bx bx-sm bxs-moon";

    public override string Description =>
        "This is the latest somewhat stable version of moonlight but currently rewritten in moonlight v2. So do not expect any updates for this version";

    public override object Config { get; set; } = new MoonlightLegacyConfig();

    public MoonlightLegacySoftware()
    {
        AddDependency<DockerDependency>();
    }

    public override Task Install(IServiceProvider provider, InstallationState state)
    {
        throw new NotImplementedException();
    }
}