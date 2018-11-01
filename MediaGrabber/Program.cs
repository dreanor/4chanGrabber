using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace MediaGrabber
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Restrict or change Filetypes to download: (Press Enter to use: jpg|png|jpeg|gif|webm)");
            string fileTypes = Console.ReadLine();

            if (string.IsNullOrEmpty(fileTypes))
            {
                fileTypes = "jpg|png|jpeg|gif|webm";
            }

            Uri uri = TryGetUri();

            DownloadImages(uri, fileTypes);
        }

        private static void DownloadImages(Uri uri, string fileTypes)
        {
            using (WebClient webClient = new WebClient())
            {
                webClient.Headers.Add("User-Agent: Other");
                var pageSource = webClient.DownloadString(uri);

                string directory = uri.AbsolutePath.Substring(uri.AbsolutePath.LastIndexOf("/") + 1);
                Directory.CreateDirectory(directory);
                var matches = Regex.Matches(pageSource, "<[^>]+href\\s*=\\s*['\"]([^ '\\\"]+.(?:" + fileTypes + "))['\"][^>]*>");

                foreach (object match in matches)
                {
                    string matchValue = match.ToString();

                    string matchlink = matchValue.Substring(matchValue.IndexOf("href=") + 8);
                    string resolvedLink = string.Format("http://{0}", matchlink.Substring(0, matchlink.IndexOf("\"")));
                    webClient.DownloadFile(resolvedLink, string.Format("{0}/{1}", directory, Path.GetFileName(resolvedLink)));
                }
            }
        }

        private static Uri TryGetUri()
        {
            Uri uri = null;

            try
            {
                Console.WriteLine("Enter the url to grab media files from:");
                uri = new Uri(Console.ReadLine());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine();
                uri = TryGetUri();
            }

            return uri;
        }
    }
}
