using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
namespace ReLaunch;

public class Launcher
{
    public static string GetDefaultJava()
    {
        string JavaPath;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            try
            {
                JavaPath = Environment.GetEnvironmentVariable("JAVA_HOME", EnvironmentVariableTarget.Machine);

            }
            catch (Exception e)
            {
                throw e;
            }
            return (JavaPath);
        }
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            try
            {
                JavaPath = Path.GetFullPath("/usr/lib/jvm/default-runtime/"); //gonna change this later 

            }
            catch (Exception e)
            {
                throw e;
            }
            return (JavaPath);
        }
        return null;


    }
    public static string GetLibraries(string location)
    {
        string returnValue = "";
        foreach (string file in Directory.GetFiles(location))
        {
            returnValue += file + ":";
        }
        return returnValue;
    }
    public static string LaunchCommandConstructor(string javaPath, string arguments, string natives, string jar, string libraries, string entryPoint, string username)
    {
        string javaExecutable = "";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            javaExecutable = "bin/javaw.exe";
        }
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) //idk if its like this on osx too and im too lazy to test it
        {
            javaExecutable = "bin/java";
        }

        return $"{javaPath + javaExecutable} {arguments} -Djava.library.path={natives} -cp \"{jar}:{libraries}\" {entryPoint} {username}";
    }

    public static void LaunchMinecraft(Settings settings, string nativesFolder, string jarsFolder, string librariesFolder, string minecraftFolder, string entryPoint)
    {
        string command = LaunchCommandConstructor(settings.JavaPath, settings.Arguments, nativesFolder, $"{jarsFolder}/{settings.MinecraftJar}.jar", GetLibraries(librariesFolder), entryPoint, settings.username);
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

            process.StartInfo.EnvironmentVariables.Remove("APPDATA");
        process.StartInfo.EnvironmentVariables.Add("APPDATA", Path.GetFullPath(".minecraft/"));
        process.StartInfo.WorkingDirectory = Path.GetFullPath(".minecraft/");
        process.WaitForExit();
        }
    }
}