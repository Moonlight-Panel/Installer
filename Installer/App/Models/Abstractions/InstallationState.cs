using MoonCore.Helpers;

namespace Installer.App.Models.Abstractions;

public class InstallationState
{
    public SmartEventHandler OnUpdated { get; set; } = new();
    
    public string Text { get; set; } = "";
    public int CurrentStep { get; set; } = 0;
    public int MaxSteps { get; set; } = 0;

    public async Task Update() => await OnUpdated.Invoke();

    public async Task Update(int currentStep, string text)
    {
        CurrentStep = currentStep;
        Text = text;

        await Update();
    }
}