using System.Text;
using Spectre.Console;

namespace Installer.Helpers;

public class DisplayHelper
{
    public static async Task RunAsStatus(string statusText, Func<Task> work)
    {
        await AnsiConsole.Status()
            .Spinner(Spinner.Known.Dots)
            .StartAsync(statusText, async _ =>
            {
                await work.Invoke();
            });
    }
}