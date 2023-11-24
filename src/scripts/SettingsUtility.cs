using System;
using System.IO;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
namespace ReLaunch;
public class SettingsUtility
{

    public void SaveSettings(Settings settings, string filename)
    {
        string json = JsonConvert.SerializeObject(settings);
        File.WriteAllText(filename, json);
    }
    /// <summary>
    /// Loads a settings file. If it doesn't exists it returns the default values.
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    public Settings LoadSettings(string file)
    {
        if (File.Exists(file))
        {
            string json = File.ReadAllText(file);
            return JsonConvert.DeserializeObject<Settings>(json);
        }
        else
        {
            Settings defaultSettings = new Settings();
            defaultSettings.username = "Player";
            defaultSettings.MinecraftJar = "ext1605_20_client";
            defaultSettings.JavaPath = Launcher.GetDefaultJava();
            defaultSettings.Arguments = "";
            return defaultSettings;
        }
    }

}
public class Settings
{

    public string? username { get; set; }
    public string? MinecraftJar { get; set; }
    public string? JavaPath { get; set; }
    public string? Arguments { get; set; }
    public bool? UseDefaultNativesLibraries { get; set; }
    public bool firstLaunch = true;
}