using System;
using System.Net;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System.IO;
using System.Diagnostics;
using VrtNuDownloader.Models;
using System.Text.RegularExpressions;
using System.Linq;

namespace VrtNuDownloader
{
    class Program
    {
        const string DOWNLOAD_DIR = "Downloads";
        const string CONFIG_DIR = "Files";
        const string WATCH_FILE = "watchlist.txt";
        const string CHECK_FILE = "done.txt";
        static readonly string WATCH_FILE_PATH = Path.Combine(CONFIG_DIR, WATCH_FILE);
        static readonly string CHECK_FILE_PATH = Path.Combine(CONFIG_DIR, CHECK_FILE);

        static void Main(string[] args)
        {
            EnsureFolderExists(DOWNLOAD_DIR);
            EnsureFolderExists(CONFIG_DIR);
            if (!File.Exists(WATCH_FILE_PATH)) File.Create(WATCH_FILE_PATH).Dispose();
            if (!File.Exists(CHECK_FILE_PATH)) File.Create(CHECK_FILE_PATH).Dispose();

            var showURL = default(string);
            var fileReader = new StreamReader(WATCH_FILE_PATH);
            while ((showURL = fileReader.ReadLine()) != null)
            {
                Console.WriteLine("Current show: " + showURL);
                VRTNU(showURL);
            }
        }

        private static void VRTNU(string showURL)
        {
            // Get page content
            var r = (HttpWebRequest)WebRequest.Create(showURL);
            r.Method = "GET";
            HttpWebResponse response;
            try { response = (HttpWebResponse)r.GetResponse(); }
            catch { return; }

            var responseStr = new StreamReader(response.GetResponseStream()).ReadToEnd();

            var doc = new HtmlDocument();
            doc.LoadHtml(responseStr);

            if (responseStr.Contains("Selecteer een Seizoen"))
            {
                // Multiple seasons available
                Console.WriteLine("Multiple seasons available");
                var seasonSelect = doc.GetElementbyId("dropdown");
                var seasons = seasonSelect.SelectNodes("//option");
                foreach (var season in seasons)
                {
                    string seasonSource = new WebClient().DownloadString("https://vrt.be" + season.Attributes["data-href"].Value);
                    var htdoc = new HtmlDocument();
                    htdoc.LoadHtml(seasonSource);
                    DownloadSeason(htdoc);
                }
            }
            else
            {
                // Only one season
                Console.WriteLine("Only one season available");
                DownloadSeason(doc);
            }
        }

        private static void DownloadSeason(HtmlDocument doc)
        {
            // Get episodes
            var epList = doc.DocumentNode.SelectSingleNode("//ul[@aria-labelledby='episodelist-title']");
            if (epList == null)
            {
                Console.WriteLine("No episodes available");
                return;
            }
            doc.LoadHtml(epList.InnerHtml);
            var episodes = doc.DocumentNode.SelectNodes("//li//a");
            foreach (var episode in episodes)
            {
                DownloadEpisode(episode);
            }
        }

        private static void DownloadEpisode(HtmlNode episode)
        {
            // Handle each episode URL
            var episodeURL = "https://www.vrt.be" + episode.Attributes["href"].Value;

            // Create content.json URL
            var contentJsonURL = episodeURL.Remove(episodeURL.Length - 1) + ".content.json";
            var contentJson = new WebClient().DownloadString(contentJsonURL);
            var VC = JsonConvert.DeserializeObject<VRTContentJson>(contentJson);

            var filename = VC.programTitle;
            if (Int32.TryParse(VC.seasonTitle, out int seasonNr))
            {
                var episodeNumber = VC.episodeNumber < 100 ? VC.episodeNumber.ToString("00") : VC.episodeNumber.ToString("000");
                filename += ($" - S{seasonNr:00}E{episodeNumber} - {VC.title}.mp4");
            }
            else
            {
                filename += ($" - S{VC.seasonTitle:00}E{VC.episodeNumber} - {VC.title}.mp4");
            }

            var folderName = Path.Combine(DOWNLOAD_DIR, MakeValidFolderName(VC.programTitle));
            EnsureFolderExists(folderName);
            filename = Path.Combine(folderName, MakeValidFileName(filename));

            // Create pbs-pub URL
            string pbsPubURL = $"https://mediazone.vrt.be/api/v1/vrtvideo/assets/{VC.publicationId}${VC.videoId}";
            string pbsPubJson = new WebClient().DownloadString(pbsPubURL);

            // Handle JSON
            VRTPbsPub VP = JsonConvert.DeserializeObject<VRTPbsPub>(pbsPubJson);
            string episodeDownloadURL = VP.targetUrls.FirstOrDefault(x => x.type == "HLS")?.url;
            if (episodeDownloadURL == null)
            {
                Console.WriteLine("Couldn't find a valid M3U8");
                return;
            }

            // Episode download URL found
            if (AlreadyDownloaded(VC.name))
                Console.WriteLine("Already downloaded, skipping...");
            else
                DownloadEpisode(episodeDownloadURL, filename, VC.name);

        }

        private static bool AlreadyDownloaded(string slug)
        {
            using (StreamReader f = new StreamReader(CHECK_FILE_PATH))
            {
                string line;
                while ((line = f.ReadLine()) != null)
                {
                    if (line == slug) return true;
                }
            }
            return false;
        }

        private static void DownloadEpisode(string downloadURL, string filename, string slug)
        {
            Process p = new Process();
            p.StartInfo.FileName = "ffmpeg";
            p.StartInfo.Arguments = $"-i \"{downloadURL}\" -c copy \"{filename}\"";
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.UseShellExecute = false;
            if (!p.Start()) Console.WriteLine("Error downloading episode");
            StreamReader r = p.StandardError;
            string line;
            bool firstDuration = false;
            while ((line = r.ReadLine()) != null)
            {
                var time = string.Empty;
                if (line.Contains("Duration: ") && !firstDuration)
                {
                    firstDuration = true;
                    time = line.Replace(", start: 0.000000, bitrate: N/A", "").Replace("  ", "").Replace(", start: 0.000000, bitrate: 0 kb/s", "").Replace("Duration: ", "");
                    Console.WriteLine("Lengte aflevering: " + time);
                }
                //if (line.Contains("time="))
                //{
                //    Console.Clear();
                //    line = line.Substring(line.IndexOf("time=") + 5);
                //    string[] outputLines = line.Split(' ');
                //    line = outputLines[0];
                //    Console.WriteLine("Downloading: " + filename);
                //    Console.WriteLine("Progress: " + line + "/" + time);
                //    Console.WriteLine("Saving as: Downloads/" + filename);
                //}
            }
            p.Close();
            Console.WriteLine("Done downloading " + filename);
            File.AppendAllText(CHECK_FILE_PATH, slug + Environment.NewLine);
        }

        private static void EnsureFolderExists(string folderName)
        {
            if (!Directory.Exists(folderName)) Directory.CreateDirectory(folderName);
        }

        private static string MakeValidFileName(string name)
        {
            string invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);
            return Regex.Replace(name, invalidRegStr, "_");
        }

        private static string MakeValidFolderName(string name)
        {
            string invalidChars = Regex.Escape(new string(Path.GetInvalidPathChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);
            return Regex.Replace(name, invalidRegStr, "_");
        }
    }
}
