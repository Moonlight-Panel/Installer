using System.Diagnostics;

namespace Installer.Helpers;

public static class BashHelper
{
    public static async Task<string> ExecuteCommand(string command, bool ignoreErrors = false, bool showOutput = false)
    {
        Process process = new Process();
        
        process.StartInfo.FileName = "/bin/bash";
        process.StartInfo.Arguments = $"-c \"{command.Replace("\"", "\\\"")}\"";
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = !showOutput;
        process.StartInfo.RedirectStandardError = true;

        process.Start();

        string output = showOutput ? "" : await process.StandardOutput.ReadToEndAsync();
        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
        {
            if(!ignoreErrors)
                throw new Exception(await process.StandardError.ReadToEndAsync());
        }

        return output;
    }
    
    public static async Task<int> ExecuteCommandForExitCode(string command)
    {
        Process process = new Process();
        
        process.StartInfo.FileName = "/bin/bash";
        process.StartInfo.Arguments = $"-c \"{command.Replace("\"", "\\\"")}\"";
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;

        process.Start();

        string output = await process.StandardOutput.ReadToEndAsync();
        await process.WaitForExitAsync();

        return process.ExitCode;
    }
    
    public static Task<Process> ExecuteCommandRaw(string command)
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