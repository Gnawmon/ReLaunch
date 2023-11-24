using System.Linq;
using System.Runtime.InteropServices;
using Newtonsoft.Json.Linq;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.IO;

namespace ReLaunch;
class Downloader
{
    public void DownloadVanilla(string version, string location)
    {
        string[] acceptedValues = { "ext1605_20_client", "lilypad_qa_r1", "v1605_preview", "v1605_unrpreview" };
        if (!acceptedValues.Contains(version)) return;
        string link = $"https://github.com/Gnawmon/alphaverJars/raw/main/{version}.jar";

        var t = Task.Run(() => DownloadFileAsync(link, $"{location}{version}.jar"));
        t.Wait();
    }
    public void DownloadNatives(string location)
    {
        string link = "";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            link = "https://libraries.minecraft.net/org/lwjgl/lwjgl/lwjgl-platform/2.9.0/lwjgl-platform-2.9.0-natives-windows.jar";
        }
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            link = "https://libraries.minecraft.net/org/lwjgl/lwjgl/lwjgl-platform/2.9.0/lwjgl-platform-2.9.0-natives-linux.jar";
        }
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            link = "https://libraries.minecraft.net/org/lwjgl/lwjgl/lwjgl-platform/2.9.0/lwjgl-platform-2.9.0-natives-osx.jar";
        }

        if (!File.Exists("natives.zip"))
        {
            var t = Task.Run(() => DownloadFileAsync(link, "natives.zip").Wait());
            t.Wait();
        }
        ZipFile.ExtractToDirectory("natives.zip", location, true);
        File.Delete("natives.zip");
    }
    public void DownloadLibraries(string location)
    {
        if (!Directory.Exists("labraries/"))
        {
            Directory.CreateDirectory("libraries/");
        }
        var t = Task.Run(() => DownloadFileAsync("https://libraries.minecraft.net/org/lwjgl/lwjgl/lwjgl/2.9.0/lwjgl-2.9.0.jar", location + "lwjgl-2.9.0.jar"));
        var t2 = Task.Run(() => DownloadFileAsync("https://libraries.minecraft.net/org/lwjgl/lwjgl/lwjgl_util/2.9.0/lwjgl_util-2.9.0.jar", location + "lwjgl_util-2.9.0.jar"));
        var t3 = Task.Run(() => DownloadFileAsync("https://libraries.minecraft.net/net/java/jinput/jinput/2.0.5/jinput-2.0.5.jar", location + "jinput-2.0.5.jar"));
        t.Wait();
        t2.Wait();
        t3.Wait();

    }
    private static string GetLatestGithubBuild(string apiReleasesLink)
    {
        using (HttpClient clienwt = new HttpClient())
        {
            //set the user agent so github doesnt return 403
            clienwt.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/110.0.5393.187 Safari/537.36");
            var json = clienwt.GetStringAsync(apiReleasesLink);
            JArray o = JArray.Parse(json.ToString());
            //get latest build
            string link = (string)o.SelectToken("[0].assets[0].browser_download_url").ToString();
            return link;
        }
    }
    static async Task DownloadFileAsync(string url, string localPath)
    {
        if (File.Exists(localPath)) return;
        using (HttpClient client = new HttpClient())
        {
            try
            {
                using (HttpResponseMessage response = await client.GetAsync(url))
                {
                    response.EnsureSuccessStatusCode(); // Throws an exception if the status code is not a success code (2xx)

                    using (var fileStream = System.IO.File.Create(localPath))
                    {
                        await response.Content.CopyToAsync(fileStream);
                        fileStream.Close();
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error downloading file: {ex.Message}");
            }
        }
    }
    public static bool AreLibrariesInstalled(string javaLibraryFolder)
    {
        if (!Directory.Exists(javaLibraryFolder))
        {
            return false;
        }
        string[] libraries = { "jinput-2.0.5.jar", "lwjgl_util-2.9.0.jar", "lwjgl-2.9.0.jar" };
        foreach (string l in libraries)
        {
            if (!Directory.GetFiles(javaLibraryFolder).Contains(l)) //probably there is a better way to check this
            {
                return false;
            }
        }
        return true;
    }

    public static bool AreNativesInstalled(string nativesFolder)
    {
        if (!Directory.Exists(nativesFolder)) //return before anything if the natives folder doesnt exist
        {
            return false;
        }

        string[] natives = { };
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            string[] windowsNatives = { "OPENAL32.dll", "OPENAL64.dll", "lwjgl.dll", "lwjdl64.dll" };
            natives = windowsNatives;
        }
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            string[] linuxNatives = { "liblwjgl.so", "liblwjgl64.so", "libopenal.so", "libopenal64.so" };
            natives = linuxNatives;
        }
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            string[] osxNatives = { "liblwjgl.jnilib", "openal.dylib" };
            natives = osxNatives;
        }

        foreach (string l in natives)
        {

            if (!Directory.GetFiles(nativesFolder).Contains(l)) //probably there is a better way to check this
            {
                return false;
            }
        }
        return true;
    }
}