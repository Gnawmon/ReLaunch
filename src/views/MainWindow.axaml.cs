using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using static ReLaunch.Launcher;
using static ReLaunch.Downloader;
using static ReLaunch.Dialog;
using static ReLaunch.SettingsUtility;
namespace ReLaunch;

public partial class MainWindow : Window
{
    Settings settings;

    public MainWindow()
    {
        SettingsUtility su = new SettingsUtility();

        InitializeComponent();

        settings = LoadSettings("settings.json");
        usernameText.Text = "Logged in as " + settings.username;
        if ((bool)settings.UseDefaultNativesLibraries)
        {
            if (!AreNativesInstalled("natives/"))
            {
                DownloadNatives("natives/");

            }
            if (!AreLibrariesInstalled("libraries/"))
            {
                DownloadLibraries("libraries/");
            }
        }
    }


    public void LaunchButtonClick(object source, RoutedEventArgs args)
    {
        if (!File.Exists($"jars/{settings.MinecraftJar}.jar"))
        {
            if (IsVanilla(settings.MinecraftJar))
            {
                DownloadVanilla(settings.MinecraftJar, "jars/");
            }
            else
            {
                Console.WriteLine("Jar not found :C");
                return;
            }
        }
        LaunchMinecraft(settings, "natives/", "jars/", "libraries/", ".minecraft/", "net.minecraft.client.Minecraft");
    }
    public async void SettingsButtonClick(object source, RoutedEventArgs args)
    {
        SettingsManager sm = new SettingsManager();
        await sm.ShowDialog(this);
        settings = LoadSettings("settings.json"); //reload the settings
    }

}

