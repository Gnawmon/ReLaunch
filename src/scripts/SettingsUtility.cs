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
    public Settings LoadSettings(string file)
    {
        
            string json = File.ReadAllText(file);
            return JsonConvert.DeserializeObject<Settings>(json);
       
    }

}
public class Settings
{

    public string username { get; set; }
    public string minecraftJar { get; set; }
    public string javaPath { get; set; }
    public string arguments {get; set;}
    int appVersion = 0;
}