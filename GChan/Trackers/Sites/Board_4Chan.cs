using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace GChan.Trackers
{
    public class Board_4Chan : Board
    {
        public const string boardRegex = "boards.(4chan|4channel).org/[a-zA-Z0-9]*?/*$";

        public Board_4Chan(string URL) : base(URL)
        {

        }

        public static bool UrlIsBoard(string url)
        {
            Regex urlMatcher = new Regex(boardRegex);
            return (urlMatcher.IsMatch(url));
        }

        override public string[] GetThreadLinks()
        {
            string URL = "http://a.4cdn.org/" + base.URL.Split('/')[3] + "/catalog.json"; //example: http://a.4cdn.org/b/catalog.json
            List<string> threadLinks = new List<string>();
            string str = "";
            XmlNodeList threadIDs;

            try
            {
                string json = new WebClient().DownloadString(URL);
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
                threadIDs = doc.DocumentElement.SelectNodes("/root/item/threads/item/no");

                for (int i = 0; i < threadIDs.Count; i++)
                {
                    threadLinks.Add("http://boards.4chan.org/" + base.URL.Split('/')[3] + "/thread/" + threadIDs[i].InnerText);
                }
            }
            catch (WebException webEx)
            {
                Program.Log(webEx);
#if DEBUG
                MessageBox.Show("Connection Error: " + webEx.Message);
#endif
            }
            return threadLinks.ToArray();
        }
    }
}
