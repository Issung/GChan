using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GChan.Trackers
{
    public class Board_4Chan : Board
    {
        public const string SITE_NAME_4CHAN = "4chan";
        public const string IS_BOARD_REGEX = "boards.(4chan|4channel).org/[a-zA-Z0-9]*?/*$";
        public const string BOARD_CODE_REGEX = "(?<=((chan|channel).org/))[a-zA-Z0-9]+(?=(/))?";

        public Board_4Chan(string URL) : base(URL)
        {
            SiteName = SITE_NAME_4CHAN;

            Match boardCodeMatch = Regex.Match(URL, BOARD_CODE_REGEX);
            BoardCode = boardCodeMatch.Groups[0].Value;
        }

        public static bool UrlIsBoard(string url)
        {
            return Regex.IsMatch(url, IS_BOARD_REGEX);
        }

        override public string[] GetThreadLinks()
        {
            //string URL = "http://a.4cdn.org/" + base.URL.Split('/')[3] + "/catalog.json"; //example: http://a.4cdn.org/b/catalog.json
            string URL = "http://a.4cdn.org/" + BoardCode + "/catalog.json";
            List<string> threadLinks = new List<string>();
            string str = "";

            try
            {
                JArray jArray;
                using (var web = new WebClient())
                {
                    string json = web.DownloadString(URL);
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

                threadLinks = jArray
                    .SelectTokens("[*].threads[*]")
                    .Select(x => "http://boards.4chan.org/" + Url.Split('/')[3] + "/thread/" + x["no"])
                    .ToList();
            }
            catch (WebException webEx)
            {
                logger.Error(webEx, "Error occured attempting to get thread links.");
                
#if DEBUG
                MessageBox.Show("Connection Error: " + webEx.Message);
#endif
            }

            threadCount = threadLinks.Count;
            return threadLinks.ToArray();
        }
    }
}
