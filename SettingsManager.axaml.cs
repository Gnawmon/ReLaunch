using Avalonia;
using Avalonia.Controls;
using Newtonsoft.Json;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;

namespace ReLaunch;

public partial class SettingsManager : Window
{
    public SettingsManager()
    {
        InitializeComponent();
    }
    public void SaveButtonClick(object source, RoutedEventArgs args)
    {
       SettingsUtility su = new SettingsUtility();
       SettingsUtility.Settings settings = new SettingsUtility.Settings();
       settings.username = "Gnawmon";
       settings.minecraftVersion = "lilypad_qa_r1";
       settings.javaPath = "/usr/lib/jvm/java-8-openjdk/";
       su.SaveSettings(settings,"settings.json");
    }
}