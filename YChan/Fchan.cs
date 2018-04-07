/************************************************************************
 * Copyright (C) 2015 by themirage <mirage@secure-mail.biz>             *
 *                                                                      *
 * This program is free software: you can redistribute it and/or modify *
 * it under the terms of the GNU General Public License as published by *
 * the Free Software Foundation, either version 3 of the License, or    *
 * (at your option) any later version.                                  *
 *                                                                      *
 * This program is distributed in the hope that it will be useful,      *
 * but WITHOUT ANY WARRANTY; without even the implied warranty of       *
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the        *
 * GNU General Public License for more details.                         *
 *                                                                      *
 * You should have received a copy of the GNU General Public License    *
 * along with this program.  If not, see <http://www.gnu.org/licenses/> *
 ************************************************************************/

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

// see Infinitechan.cs for explanation

namespace YChan
{
    internal class Fchan : Imageboard
    {
        public static string regThread = "boards.4chan.org/[a-zA-Z0-9]*?/thread/[0-9]*";
        public static string regBoard = "boards.4chan.org/[a-zA-Z0-9]*?/$";

        public Fchan(string url, bool isBoard) : base(url, isBoard)
        {
            this.Board = isBoard;
            this.imName = "4chan";
            if (!isBoard)
            {
                Match match = Regex.Match(url, @"boards.4chan.org/[a-zA-Z0-9]*?/thread/\d*");
                this.URL = "http://" + match.Groups[0].Value;
            }
            else
            {
                this.URL = url;
            }
            if (!isBoard)
                this.SaveTo = General.path + "\\" + this.imName + "\\" + getURL().Split('/')[3] + "\\" + getURL().Split('/')[5];
            else
                this.SaveTo = General.path + "\\" + this.imName + "\\" + getURL().Split('/')[3];
        }

        public new static bool isThread(string url)
        {
            Regex urlMatcher = new Regex(regThread);
            return (urlMatcher.IsMatch(url));

        }

        public new static bool isBoard(string url)
        {
            Regex urlMatcher = new Regex(regBoard);
            return (urlMatcher.IsMatch(url));
        }

        override protected string[] getLinks()
        {
            List<string> links = new List<string>();
            string JSONUrl = "http://a.4cdn.org/" + getURL().Split('/')[3] + "/thread/" + getURL().Split('/')[5] + ".json";
            string baseURL = "http://i.4cdn.org/" + getURL().Split('/')[3] + "/";
            string str = "";
            XmlNodeList xmlTim;
            XmlNodeList xmlExt;
            try
            {
                string Content = new WebClient().DownloadString(JSONUrl);

                byte[] bytes = Encoding.ASCII.GetBytes(Content);
                using (var stream = new MemoryStream(bytes))
                {
                    var quotas = new XmlDictionaryReaderQuotas();
                    var jsonReader = JsonReaderWriterFactory.CreateJsonReader(stream, quotas);
                    var xml = XDocument.Load(jsonReader);
                    str = xml.ToString();
                }

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(str);
                if (getURL().Split('/')[3] == "f")
                    xmlTim = doc.DocumentElement.SelectNodes("/root/posts/item/filename");
                else
                    xmlTim = doc.DocumentElement.SelectNodes("/root/posts/item/tim");

                xmlExt = doc.DocumentElement.SelectNodes("/root/posts/item/ext");
                for (int i = 0; i < xmlExt.Count; i++)
                {
                    links.Add(baseURL + xmlTim[i].InnerText + xmlExt[i].InnerText);
                }
            }
            catch (WebException webEx)
            {
                throw webEx;
            }
            return links.ToArray();
        }

        override public string getThreads()
        {
            string URL = "http://a.4cdn.org/" + getURL().Split('/')[3] + "/catalog.json";
            string Res = "";
            string str = "";
            XmlNodeList tNa;
            XmlNodeList tNo;
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
                tNo = doc.DocumentElement.SelectNodes("/root/item/threads/item/no");
                tNa = doc.DocumentElement.SelectNodes("/root/item/threads/item/semantic_url");
                for (int i = 0; i < tNo.Count; i++)
                {
                    Res = Res + "http://boards.4chan.org/" + getURL().Split('/')[3] + "/thread/" + tNo[i].InnerText + "/" + tNa[i].InnerText + "\n";
                }
            }
            catch (WebException webEx)
            {
                #if DEBUG
                MessageBox.Show("Connection Error: " + webEx.Message);
                #endif
            }
            return Res;
        }

        override public void download()
        {
            try
            {
                if (!Directory.Exists(this.SaveTo))
                    Directory.CreateDirectory(this.SaveTo);

                if (General.loadHTML)
                    downloadHTMLPage();

                string[] URLs = getLinks();

                for (int y = 0; y < URLs.Length; y++)
                    General.dlTo(URLs[y], this.SaveTo);

            }
            catch (WebException webEx)
            {
                if (((int)webEx.Status) == 7)
                    this.Gone = true;
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message, "No Permission to access folder");
                throw ex;
            }
        }

        private void downloadHTMLPage()
        {
            List<string> thumbs = new List<string>();
            string htmlPage = "";
            string str = "";
            string baseURL = "//i.4cdn.org/" + getURL().Split('/')[3] + "/";
            string JURL = "http://a.4cdn.org/" + getURL().Split('/')[3] + "/thread/" + getURL().Split('/')[5] + ".json";

            try
            {
                //Add a UserAgent to prevent 403
                WebClient web = new WebClient();
                web.Headers["User-Agent"] = "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:47.0) Gecko/20100101 Firefox/47.0";

                htmlPage = web.DownloadString(this.getURL());

                //Prevent the html from being destroyed by the anti adblock script
                htmlPage = htmlPage.Replace("f=\"to\"", "f=\"penis\"");

                string json = web.DownloadString(JURL);
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
                XmlNodeList xmlTim = doc.DocumentElement.SelectNodes("/root/posts/item/tim");
                XmlNodeList xmlExt = doc.DocumentElement.SelectNodes("/root/posts/item/ext");

                for (int i = 0; i < xmlExt.Count; i++)
                {
                    string old = baseURL + xmlTim[i].InnerText + xmlExt[i].InnerText;
                    string rep = xmlTim[i].InnerText + xmlExt[i].InnerText;
                    htmlPage = htmlPage.Replace(old, rep);

                    //Save thumbs for files that need it
                    if (rep.Split('.')[1] == "webm" /*|| rep.Split('.')[1] == ""*/)
                    {
                        old = "//t.4cdn.org/" + getURL().Split('/')[3] + "/" + xmlTim[i].InnerText + "s.jpg";
                        thumbs.Add("http:" + old);

                        htmlPage = htmlPage.Replace("//i.4cdn.org/" + getURL().Split('/')[3] + "/" + xmlTim[i].InnerText, "thumb/" + xmlTim[i].InnerText);
                    }
                    else
                    {
                        string thumbName = rep.Split('.')[0] + "s";
                        htmlPage = htmlPage.Replace(thumbName + ".jpg", rep.Split('.')[0] + "." + rep.Split('.')[1]);
                        htmlPage = htmlPage.Replace("/" + thumbName, thumbName);

                        htmlPage = htmlPage.Replace("//i.4cdn.org/" + getURL().Split('/')[3] + "/" + xmlTim[i].InnerText, xmlTim[i].InnerText);
                    }

                    htmlPage = htmlPage.Replace("/" + rep, rep);
                }

                htmlPage = htmlPage.Replace("=\"//", "=\"http://");

                //Save thumbs for files that need it
                for (int i = 0; i < thumbs.Count; i++)
                    General.dlTo(thumbs[i], this.SaveTo + "\\thumb");



                if (!string.IsNullOrWhiteSpace(htmlPage))
                    File.WriteAllText(this.SaveTo + "\\Thread.html", htmlPage);

            }

            catch (WebException webEx)
            {
                throw webEx;
            }

            catch (UnauthorizedAccessException ex)
            {
                throw ex;
            }
        }
    }
}