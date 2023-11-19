using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using Newtonsoft.Json.Linq;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.IO;

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


        var t = Task.Run(() => DownloadFileAsync(link, "natives.zip").Wait());
        t.Wait();
        ZipFile.ExtractToDirectory("natives.zip", location);

    }
    public void DownloadLibraries(string location)
    {
        var t = Task.Run(() => DownloadFileAsync("https://libraries.minecraft.net/org/lwjgl/lwjgl/lwjgl/2.9.0/lwjgl-2.9.0.jar", location + "lwjgl-2.9.0.jar"));
        var t2 = Task.Run(() => DownloadFileAsync("https://libraries.minecraft.net/org/lwjgl/lwjgl/lwjgl_util/2.9.0/lwjgl_util-2.9.0.jar", location + "lwjgl_util-2.9.0.jar"));
        var t3 = Task.Run(() => DownloadFileAsync("https://libraries.minecraft.net/net/java/jinput/jinput/2.0.5/jinput-2.0.5.jar", location + "jinput-2.0.5.jar"));
        t.Wait();
        t2.Wait();
        t3.Wait();

    }
    private static string GetLatestGithubBuild(string apiReleasesLink)
    {
        WebClient client = new WebClient();
        //set the user agent so github doesnt return 403
        client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/110.0.5393.187 Safari/537.36");
        var json = client.DownloadString(apiReleasesLink);
        JArray o = JArray.Parse(json);
        //get latest build
        string link = (string)o.SelectToken("[0].assets[0].browser_download_url").ToString();
        return link;
    }
    static async Task DownloadFileAsync(string url, string localPath)
    {
        if(File.Exists(localPath)) return;
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
}