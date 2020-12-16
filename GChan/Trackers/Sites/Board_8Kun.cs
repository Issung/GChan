using System;
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
    public class Board_8Kun : Board
    {
        public const string boardRegex = "8kun.top/[a-zA-Z0-9]*?/";

        public Board_8Kun(string url) : base(url)
        {
            SiteName = "8kun";

            URL = url;
            SaveTo = Properties.Settings.Default.SavePath + "\\" + SiteName + "\\" + URL.Split('/')[3]; // Set SaveTo path
        }

        public static bool UrlIsBoard(string url)
        {
            return Regex.IsMatch(url, boardRegex);
        }

        override public string[] GetThreadLinks()
        {
            string url = "http://8kun.top/" + URL.Split('/')[3] + "/catalog.json";
            List<string> Res = new List<string>();
            string str = "";
            XmlNodeList tNo;
            try
            {
                string json = new WebClient().DownloadString(url);
                byte[] bytes = Encoding.ASCII.GetBytes(json);
                using (var stream = new MemoryStream(bytes))
                {
                    var quotas = new XmlDictionaryReaderQuotas();
                    var jsonReader = JsonReaderWriterFactory.CreateJsonReader(stream, quotas);
                    var xml = XDocument.Load(jsonReader);
                    str = xml.ToString();                                                 // JSON to XML again
                }

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(str);
                tNo = doc.DocumentElement.SelectNodes("/root/item/threads/item/no");
                for (int i = 0; i < tNo.Count; i++)
                {
                    Res.Add("http://8kun.top/" + url.Split('/')[3] + "/res/" + tNo[i].InnerText + ".html");
                }
            }
            catch (WebException webEx)
            {
                Program.Log(webEx);
#if DEBUG
                MessageBox.Show("Connection Error: " + webEx.Message);
#endif
            }

            return Res.ToArray();
        }
    }
}
