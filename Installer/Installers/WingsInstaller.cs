using Installer.Helpers;
using Spectre.Console;

namespace Installer.Installers;

public class WingsInstaller
{
    public static async Task Install()
    {
        bool dockerInstalled = false;

        await DisplayHelper.RunAsStatus("[white]Checking if docker is installed on your system[/]",
            async () => { dockerInstalled = await BashHelper.ExecuteCommandForExitCode("docker") == 0; });

        if (!dockerInstalled)
        {
            AnsiConsole.MarkupLine("[white]Docker seems [red]not[/] to be installed[/]");

            if (AnsiConsole.Confirm("[white]Do you want to install docker?[/]"))
            {
                try
                {
                    await DisplayHelper.RunAsStatus("[white]Installing docker[/]",
                        async () =>
                        {
                            await BashHelper.ExecuteCommand("curl -sSL https://get.docker.com/ | CHANNEL=stable bash");
                        });

                    AnsiConsole.MarkupLine("[white]Docker has been [green]successfully[/] installed on your system[/]");
                }
                catch (Exception e)
                {
                    AnsiConsole.MarkupLine("[white]An [red]error[/] occured while installing docker[/]");
                    AnsiConsole.WriteLine(e.Message);
                    return;
                }
            }
        }
        else
        {
            AnsiConsole.MarkupLine("[white]Docker is [aqua]installed[/][/]");
        }

        AnsiConsole.MarkupLine("[white]Creating environment[/]");
        Directory.CreateDirectory("/etc/pterodactyl/");

        var architectureType = await BashHelper.ExecuteCommand("uname -m");
        architectureType = architectureType.Trim();

        if (architectureType == "x86_64")
            architectureType = "amd64";
        else
            architectureType = "arm64";

        AnsiConsole.MarkupLine($"[white]Selecting architecture {architectureType}[/]");

        await DisplayHelper.RunAsStatus("[white]Downloading wings binary[/]", async () =>
        {
            using var httpClient = new HttpClient();
            var fs = File.Create("/usr/local/bin/wings");

            var response = await httpClient
                .GetAsync(
                    $"https://github.com/pterodactyl/wings/releases/latest/download/wings_linux__{architectureType}");

            await response.Content.CopyToAsync(fs);

            await fs.FlushAsync();
            fs.Close();
        });

        await DisplayHelper.RunAsStatus("[white]Changing file permissions[/]",
            async () => { await BashHelper.ExecuteCommand("chmod u+x /usr/local/bin/wings"); });
        
        await DisplayHelper.RunAsStatus("[white]Downloading systemd service[/]", async () =>
        {
            using var httpClient = new HttpClient();
            var fs = File.Create("/etc/systemd/system/wings.service");

            var response = await httpClient
                .GetAsync(
                    "https://install.moonlightpanel.xyz/daemonFiles/wings.service");

            await response.Content.CopyToAsync(fs);

            await fs.FlushAsync();
            fs.Close();
        });
        
        await DisplayHelper.RunAsStatus("[white]Reloading systemd daemon[/]", async () =>
        {
            await BashHelper.ExecuteCommand("systemctl daemon-reload");
        });
        
        await DisplayHelper.RunAsStatus("[white]Enabling wings[/]", async () =>
        {
            await BashHelper.ExecuteCommand("systemctl enable --now wings");
        });
        
        AnsiConsole.MarkupLine("[white]Wings has been successfully installed. If you want to enable swap, look at the grub section of the following page: https://docs.moonlightpanel.xyz/install-the-daemon[/]");
        AnsiConsole.MarkupLine("[white]To complete the setup, add a node in the moonlight panel and press setup[/]");
        AnsiConsole.MarkupLine("[white]After that, run 'systemctl restart wings' and 'systemctl restart moonlightdaemon' to apply changes[/]");
    }
}