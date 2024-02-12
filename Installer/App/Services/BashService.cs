using System.Diagnostics;
using MoonCore.Attributes;

namespace Installer.App.Services;

[Singleton]
public class BashService
{
    public async Task<string> ExecuteCommand(string command, bool ignoreErrors = false, bool showOutput = false)
    {
        var process = await ExecuteCommandRaw(command);

        string output = showOutput ? "" : await process.StandardOutput.ReadToEndAsync();
        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
        {
            if(!ignoreErrors)
                throw new Exception(await process.StandardError.ReadToEndAsync());
        }

        return output;
    }
    
    public async Task<int> ExecuteCommandForExitCode(string command)
    {
        var process = await ExecuteCommandRaw(command);
        
        await process.WaitForExitAsync();

        return process.ExitCode;
    }
    
    public Task<Process> ExecuteCommandRaw(string command)
    {
        Process process = new Process();
        
        process.StartInfo.FileName = "/bin/bash";
        process.StartInfo.Arguments = $"-c \"{command.Replace("\"", "\\\"")}\"";
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;

        process.Start();

        return Task.FromResult(process);
    }
}