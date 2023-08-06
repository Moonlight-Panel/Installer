using System.Text;
using Installer.Helpers;
using Installer.Models;
using Newtonsoft.Json;
using Spectre.Console;

namespace Installer.Installers;

public static class PanelInstaller
{
    public static async Task Install()
    {
        var basicConfig = new BasicPanelConfigModel();
        
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

        var moonlightHasBeenInstalled = false;

        await DisplayHelper.RunAsStatus(
            "[white]Checking if moonlight has been already installed at some point of time[/]",
            async () =>
            {
                moonlightHasBeenInstalled =
                    await BashHelper.ExecuteCommandForExitCode(
                        "docker images --format \"{{.Repository}}\" | grep -w \"^moonlightpanel/moonlight$\"") == 0;
            }
        );

        if (!Directory.Exists("/var/lib/docker/volumes/moonlight"))
        {
            AnsiConsole.MarkupLine("[white]The moonlight volume is [red]missing[/]. Creating the required volume[/]");
            await BashHelper.ExecuteCommand("docker volume create moonlight");
        }
        else
        {
            AnsiConsole.MarkupLine("[white]The moonlight volume is already [green]created[/][/]");
        }

        if (!moonlightHasBeenInstalled)
        {
            AnsiConsole.MarkupLine("[white]It seems that you have never installed moonlight before[/]");
            AnsiConsole.MarkupLine("[white]Starting initial setup[/]");

            if (AnsiConsole.Confirm(
                    "[white]Do you want to use a local mysql instance running in a docker container? If you already configured the moonlight database, deny this option[/]"))
            {
                var password = GenerateString(32);
                var command = $"docker run -d --restart=always --add-host=host.docker.internal:host-gateway --publish 0.0.0.0:3307:3306 --name mlmysql -v mlmysql:/var/lib/mysql -e MYSQL_ROOT_PASSWORD={password} -e MYSQL_DATABASE=moonlight -e MYSQL_USER=moonlight -e MYSQL_PASSWORD={password} mysql:latest";

                await DisplayHelper.RunAsStatus("[white]Creating mysql container[/]", async () =>
                {
                    await BashHelper.ExecuteCommand(command, showOutput: true);
                });

                basicConfig.Moonlight.Database.Host = "host.docker.internal";
                basicConfig.Moonlight.Database.Port = 3307;
                basicConfig.Moonlight.Database.Username = "moonlight";
                basicConfig.Moonlight.Database.Password = password;
                basicConfig.Moonlight.Database.Database = "moonlight";
            }
            else if(AnsiConsole.Confirm("[white]Do you want to configure an external database? If you already configured the moonlight database, deny this option[/]"))
            {
                basicConfig.Moonlight.Database.Host = AnsiConsole.Ask<string>("[white]Enter the database host (not localhost or 127.0.0.1)[/]");
                basicConfig.Moonlight.Database.Port = AnsiConsole.Ask<int>("[white]Enter the database port[/]");
                basicConfig.Moonlight.Database.Username = AnsiConsole.Ask<string>("[white]Enter the database username[/]");
                basicConfig.Moonlight.Database.Password = AnsiConsole.Ask<string>("[white]Enter the database password[/]");
                basicConfig.Moonlight.Database.Database = AnsiConsole.Ask<string>("[white]Enter the database name[/]");
            }

            string defaultIp = "your-moonlight-domain.de";

            try
            {
                using var httpClient = new HttpClient();
                defaultIp = await httpClient.GetStringAsync("https://api.ipify.org");
            }
            catch (Exception) {}

            AnsiConsole.WriteLine();
            basicConfig.Moonlight.AppUrl = AnsiConsole.Ask<string>("[white]Enter the app url for moonlight[/]", $"http://{defaultIp}");

            AnsiConsole.MarkupLine("[white]Saving config file...[/]");
            
            Directory.CreateDirectory("/var/lib/docker/volumes/moonlight/_data/configs/");
            await File.WriteAllTextAsync("/var/lib/docker/volumes/moonlight/_data/configs/config.json", JsonConvert.SerializeObject(basicConfig));
        }
        else
            AnsiConsole.MarkupLine("[white]It seems you had already moonlight installed, so we are skipping the configuration steps for you[/]");

        var moonlightContainerExisting = false;

        await DisplayHelper.RunAsStatus("[white]Checking for existing moonlight container[/]", async () =>
        {
            moonlightContainerExisting =
                !string.IsNullOrEmpty(await BashHelper.ExecuteCommand("docker ps -q -f name=moonlight"));
        });

        if (moonlightContainerExisting)
        {
            var moonlightContainerExited = false;
            
            await DisplayHelper.RunAsStatus("[white]Checking for the status of the existing moonlight container[/]", async () =>
            {
                moonlightContainerExited =
                    !string.IsNullOrEmpty(await BashHelper.ExecuteCommand("docker ps -aq -f status=exited -f name=moonlight"));
            });

            if (!moonlightContainerExited)
            {
                await DisplayHelper.RunAsStatus("[white]Stopping moonlight container[/]", async () =>
                {
                    await BashHelper.ExecuteCommand("docker kill moonlight");
                });
            }
            
            await DisplayHelper.RunAsStatus("[white]Removing moonlight container[/]", async () =>
            {
                await BashHelper.ExecuteCommand("docker rm moonlight");
            });
        }
        
        await DisplayHelper.RunAsStatus("[white]Removing old moonlight images if existing[/]", async () =>
        {
            await BashHelper.ExecuteCommand("docker image rm moonlightpanel/moonlight:beta", true);
        });
        
        await DisplayHelper.RunAsStatus("[white]Pulling moonlight docker image[/]", async () =>
        {
            await BashHelper.ExecuteCommand("docker pull moonlightpanel/moonlight:beta");
        });
        
        await DisplayHelper.RunAsStatus("[white]Creating moonlight container[/]", async () =>
        {
            await BashHelper.ExecuteCommand("docker run -d -p 80:80 -p 443:443 --add-host=host.docker.internal:host-gateway -v moonlight:/app/storage --name moonlight --restart=always moonlightpanel/moonlight:beta");
        });
    }

    private static string GenerateString(int length)
    {
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var stringBuilder = new StringBuilder();
        var random = new Random();

        for (int i = 0; i < length; i++)
        {
            stringBuilder.Append(chars[random.Next(chars.Length)]);
        }

        return stringBuilder.ToString();
    }
}