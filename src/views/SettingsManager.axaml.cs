using Avalonia;
using Avalonia.Controls;
using Newtonsoft.Json;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;
using System;

namespace ReLaunch;

public partial class SettingsManager : Window
{
    SettingsUtility su = new SettingsUtility();
    Settings settings = new Settings();
    public SettingsManager()
    {
        InitializeComponent();
        try
        {
            settings = su.LoadSettings("settings.json");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        username.Text = settings.username;
        javaPath.Text = settings.JavaPath;
        arguments.Text = settings.Arguments;
        defaultNatives.IsChecked = settings.UseDefaultNativesLibraries;
    }
    public void SaveButtonClick(object source, RoutedEventArgs args)
    {

        settings.UseDefaultNativesLibraries = defaultNatives.IsChecked;
        settings.username = username.Text;
        settings.MinecraftJar = "ext1605_20_client"; //idk how to select from a combobox so its gonna only launch r4 for now lolololololol
        settings.JavaPath = javaPath.Text;
        settings.Arguments = arguments.Text;
        su.SaveSettings(settings, "settings.json");
    }
}