using GChan.Properties;
using NLog;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace GChan.Trackers
{
    public class Board_8Kun : Board
    {
        public const string SITE_NAME = "8kun";
        public const string boardRegex = @"8kun.top/[a-zA-Z0-9]*?/";
        public const string boardCodeRegex = @"(?<=(8kun.top/))[a-zA-Z0-9]*(?=(/index.html))";

        public Board_8Kun(string url) : base(url)
        {
            SiteName = SITE_NAME;

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
            XmlNodeList threadNos;

            try
            {
                string json = new WebClient().DownloadString(url);
                byte[] bytes = Encoding.ASCII.GetBytes(json);
                using (var stream = new MemoryStream(bytes))
                {
                    var quotas = new XmlDictionaryReaderQuotas();
                    var jsonReader = JsonReaderWriterFactory.CreateJsonReader(stream, quotas);
                    var xml = XDocument.Load(jsonReader);
                    str = xml.ToString();
                }

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(str);
                threadNos = doc.DocumentElement.SelectNodes("//no");
                for (int i = 0; i < threadNos.Count; i++)
                {
                    threadUrls.Add("http://8kun.top/" + BoardCode + "/res/" + threadNos[i].InnerText + ".html");
                }
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
