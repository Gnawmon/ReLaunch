using System.IO;
using Newtonsoft.Json;

public class SettingsUtility
{

    public void SaveSettings(Settings settings, string filename)
    {
        string json = JsonConvert.SerializeObject(settings);
        File.WriteAllText(filename, json);
    }
    public Settings LoadSettings(string file){
        string json = File.ReadAllText(file);
        return JsonConvert.DeserializeObject<Settings>(json);
    }
    public class Settings
    {

        public string username { get; set; }
        public string minecraftVersion { get; set; }
        public string javaPath { get; set; }
         int appVersion = 0;
    }
}