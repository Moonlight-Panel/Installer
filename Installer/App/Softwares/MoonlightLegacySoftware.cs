using Installer.App.Dependencies;
using Installer.App.Models.Abstractions;
using Installer.App.Models.Configs;
using MoonCore.Helpers;

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

    public override async Task Install(IServiceProvider provider, InstallationState state)
    {
        state.MaxSteps = 100;
        await state.Update();

        for (int i = 0; i < 101; i++)
        {
            await Task.Delay(500);
            await state.Update(i, Formatter.GenerateString(16));
        }
    }
}