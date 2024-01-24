﻿using System.Text;
using Installer.Installers;
using Spectre.Console;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

AnsiConsole.MarkupLine("[aqua]Moonlight Panel Installer[/]");
AnsiConsole.MarkupLine(
    "[white]Welcome to the moonlight panel installer. This program will guide you through the installation of the moonlight panel, the daemon and wings[/]");

var installer = AnsiConsole.Prompt(
    new SelectionPrompt<string>()
        .Title("[white]Please select the software you want to install on this machine[/]")
        .AddChoices(
            "Moonlight Panel",
            "Moonlight Daemon",
            "Wings"
        )
);

AnsiConsole.MarkupLine($"[white]Starting installer for: {installer}[/]");

switch (installer) {
    case "Moonlight Panel":
        await PanelInstaller.Install();
        break;
    case "Moonlight Daemon":
        await DaemonInstaller.Install();
        break;
    case "Wings":
        await WingsInstaller.Install();
        break;
}

AnsiConsole.MarkupLine($"[white]{installer} has been installed![/]");