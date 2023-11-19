using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using static ReLaunch.Launcher;

namespace ReLaunch;

public partial class MainWindow : Window
{
    Settings settings;

    public MainWindow()
    {
        SettingsUtility su = new SettingsUtility();
        Downloader downloader = new Downloader();
        InitializeComponent();
        string[] libraries = { "jinput-2.0.5.jar", "lwjgl_util-2.9.0.jar", "lwjgl-2.9.0.jar" };
        foreach (string l in libraries)
        {
            if (!Directory.GetFiles("libraries/").Contains(l) || !Directory.Exists("libraries/")) //probably there is a better way to check this
            {
                downloader.DownloadLibraries("libraries/");
                break;
            }
        }

        try
        {
            settings = su.LoadSettings("settings.json");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
    public void LaunchButtonClick(object source, RoutedEventArgs args)
    {
        RunCommand(LaunchCommandConstructor(GetDefaultJava(), settings.arguments, "natives/", $"jars/{settings.minecraftJar}.jar", GetLibraries("libraries/"), "net.minecraft.client.Minecraft", settings.username));

    }
    public void SettingsButtonClick(object source, RoutedEventArgs args)
    {
        SettingsManager sm = new SettingsManager();
        sm.Show();
    }
    static void RunCommand(string command)
    {
        var processInfo = new ProcessStartInfo
        {
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = Directory.GetCurrentDirectory()
        };

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            processInfo.FileName = "cmd.exe";
            processInfo.Arguments = $"/c {command}";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            processInfo.FileName = "/bin/bash";
            processInfo.Arguments = $"-c \"{command}\"";
        }
        else
        {
            Console.WriteLine("Unsupported operating system");
            return;
        }

        using (var process = new Process { StartInfo = processInfo })
        {
            process.Start();

            using (var writer = process.StandardInput)
            using (var reader = process.StandardOutput)
            using (var errorReader = process.StandardError)
            {
                if (writer.BaseStream.CanWrite)
                {
                    writer.WriteLine(command);
                    writer.Close();
                }

                var output = reader.ReadToEnd();
                var error = errorReader.ReadToEnd();

                Console.WriteLine("Output:");
                Console.WriteLine(output);

                if (!string.IsNullOrEmpty(error))
                {
                    Console.WriteLine("Error:");
                    Console.WriteLine(error);
                }
            }

            process.WaitForExit();
            Environment.ExitCode = process.ExitCode;
        }
    }

}