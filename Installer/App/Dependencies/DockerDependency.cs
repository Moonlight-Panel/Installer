using Installer.App.Models.Abstractions;
using Installer.App.Services;
using MoonCore.Exceptions;

namespace Installer.App.Dependencies;

public class DockerDependency : SoftwareDependency
{
    public override string Name => "Docker Engine";
    public override string Icon => "bx bx-sm bxl-docker";
    public override string Description => "Docker is an open platform for developing, shipping, and running applications. Docker enables you to separate your applications from your infrastructure so you can deliver software quickly. With Docker, you can manage your infrastructure in the same ways you manage your applications. By taking advantage of Docker's methodologies for shipping, testing, and deploying code, you can significantly reduce the delay between writing code and running it in production.";
    public override object Config { get; set; } = new();

    public override async Task Install(IServiceProvider provider, InstallationState state)
    {
        // Setup
        state.MaxSteps = 3;
        await state.Update();
        
        // Load dependencies
        var bashService = provider.GetRequiredService<BashService>();
        
        /*
         *
         * // Step 1, ensuring curl and bash have been installed
           await state.Update(1, "Checking if curl and bash are installed");
           await bashService.ExecuteCommand("apt install curl bash -y");
         */
        
       /*
        * // Step 2, running the install script for docker
                 await state.Update(2, "Installing docker using the install script");
                 await bashService.ExecuteCommand("curl -sSL https://get.docker.com/ | CHANNEL=stable bash");
        * 
        */

       await state.Update(2, "Installing docker using the install script");
       await Task.Delay(5000);
        
       /*
        *
        * // Step 3, checking if installation was successful
          if (!await IsFulfilled(provider))
              throw new DisplayException(
                  "The install script ran but docker is still missing. Please check if your operating system is supported");
        */

       await state.Update(3, "Verifying installation");
    }

    public override async Task<bool> IsFulfilled(IServiceProvider provider)
    {
        return false;
        
        var bashService = provider.GetRequiredService<BashService>();

        var whichDockerResult = await bashService.ExecuteCommand("which docker");

        if (string.IsNullOrEmpty(whichDockerResult))
            return false;

        return true;
    }
}