using GChan.Properties;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GChan.Trackers
{
    public class Board_8Kun : Board
    {
        public const string SITE_NAME_8KUN = "8kun";
        public const string boardRegex = @"8kun.top/[a-zA-Z0-9]*?/";
        public const string boardCodeRegex = @"(?<=(8kun.top/))[a-zA-Z0-9]*(?=(/index.html))";

        public Board_8Kun(string url) : base(url)
        {
            SiteName = SITE_NAME_8KUN;

            Match boardCodeMatch = Regex.Match(url, boardCodeRegex);
            BoardCode = boardCodeMatch.Groups[0].Value;

            SaveTo = Path.Combine(Settings.Default.SavePath, SiteName, BoardCode);
        }

        public static bool UrlIsBoard(string url)
        {
            return Regex.IsMatch(url, boardRegex);
        }

        override public string[] GetThreadLinks()
        {
            string url = "http://8kun.top/" + BoardCode + "/catalog.json";
            List<string> threadUrls = new List<string>();
            string str = "";

            try
            {
                JArray jArray;
                using (var web = new WebClient())
                {
                    string json = web.DownloadString(url);
                    byte[] bytes = Encoding.ASCII.GetBytes(json);

                    using (var stream = new MemoryStream(bytes))
                    {
                        using (var streamReader = new StreamReader(stream))
                        {
                            var jsonReader = new JsonTextReader(streamReader);
                            jArray = JArray.Load(jsonReader);
                        }
                    }
                }

                threadUrls = jArray
                    .SelectTokens("..no")
                    .Select(x => "http://8kun.top/" + BoardCode + "/res/" + x + ".html")
                    .ToList();
            }
            catch (WebException webEx)
            {
                logger.Error(webEx, $"Connection error trying to find threads in a board {this}.");
            }

            threadCount = threadUrls.Count;
            return threadUrls.ToArray();
        }
    }
}
