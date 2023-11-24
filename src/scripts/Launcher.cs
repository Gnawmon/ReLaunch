using System;
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
                JavaPath = "/usr/lib/jvm/java-8-openjdk/bin/java"; //gonna change this later 

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
}