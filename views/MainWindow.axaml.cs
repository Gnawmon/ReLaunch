using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using static ReLaunch.Launcher;
using static ReLaunch.SettingsUtility;

namespace ReLaunch;

public partial class MainWindow : Window
{
    Settings settings;

    public MainWindow()
    {
        SettingsUtility su = new SettingsUtility();
        InitializeComponent();
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
        // System.Console.WriteLine();
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