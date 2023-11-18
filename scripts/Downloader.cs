using System.Linq;
using System.Net;
using Newtonsoft.Json.Linq;
class Downloader
{
    public void DownloadVanilla(string version, string location)
    {
        string[] acceptedValues = { "ext1605_20_client", "lilypad_qa_r1", "v1605_preview", "v1605_unrpreview" };
        if (!acceptedValues.Contains(version)) return;
        string link = $"https://github.com/Gnawmon/alphaverJars/raw/main/{version}.jar";


        using (var client = new WebClient())
        {
            client.DownloadFile(link, $"{location}{version}.jar");
        }


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
}