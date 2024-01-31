namespace Installer.App.Models.Abstractions;

public class InstallationState
{
    private readonly Func<Task> OnUpdated;

    public InstallationState(Func<Task> onUpdated)
    {
        OnUpdated = onUpdated;
    }

    public int CurrentStep { get; set; }
    public int LastStep { get; set; }
    public string Text { get; set; }

    public async Task Update() => await OnUpdated.Invoke();
}