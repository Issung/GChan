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

namespace YChan
{
    internal class Infinitechan : Imageboard
    {
        public static string regThread = "8ch.net/[a-zA-Z0-9]*?/res/[0-9]*.[^0-9]*";  // Regex to check whether is Thread or not
        public static string regBoard = "8ch.net/[a-zA-Z0-9]*?/";                    // Regex to check whether is Board or not

        public Infinitechan(string url, bool isBoard) : base(url, isBoard)
        {
            this.Board = isBoard;
            this.imName = "8ch";
            if (!isBoard)
            {
                Match match = Regex.Match(url, @"8ch.net/[a-zA-Z0-9]*?/res/[0-9]*");
                this.URL = "http://" + match.Groups[0].Value + ".html";      // simplify thread url
                this.SaveTo = (General.path + "\\" + this.imName + "\\" + getURL().Split('/')[3] + "\\" + getURL().Split('/')[5]).Replace(".html", ""); // set saveto path
            }
            else
            {
                this.URL = url;
                this.SaveTo = General.path + "\\" + this.imName + "\\" + getURL().Split('/')[3]; // set saveto path
            }
        }

        public new static bool isThread(string url)
        {
            Regex urlMatcher = new Regex(regThread);
            if (urlMatcher.IsMatch(url))
                return true;
            else
                return false;
        }

        public new static bool isBoard(string url)
        {
            Regex urlMatcher = new Regex(regBoard);
            if (urlMatcher.IsMatch(url))
                return true;
            else
                return false;
        }

        override protected string[] getLinks()
        {
            List<string> links = new List<string>();
            string JSONUrl = ("http://8ch.net/" + getURL().Split('/')[3] + "/res/" + getURL().Split('/')[5] + ".json").Replace(".html", ""); // thread JSON url
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
                    str = xml.ToString();                                                               // convert JSON to XML (funny, I know)
                }

                // get single images
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(str);
                xmlTim = doc.DocumentElement.SelectNodes("/root/posts/item/tim");
                xmlExt = doc.DocumentElement.SelectNodes("/root/posts/item/ext");

                for (int i = 0; i < xmlExt.Count; i++)
                {
                    //exed = exed + "https://8ch.net/" + getURL().Split('/')[3] + "/src/" + xmlTim[i].InnerText + xmlExt[i].InnerText + "\n";
                    links.Add("https://8ch.net/" + "/file_store/" + xmlTim[i].InnerText + xmlExt[i].InnerText);
                }

                // get images of posts with multiple images
                xmlTim = doc.DocumentElement.SelectNodes("/root/posts/item/extra_files/item/tim");
                xmlExt = doc.DocumentElement.SelectNodes("/root/posts/item/extra_files/item/ext");
                for (int i = 0; i < xmlExt.Count; i++)
                {
                    //exed = exed + "https://8ch.net/" + getURL().Split('/')[3] + "/src/" + xmlTim[i].InnerText + xmlExt[i].InnerText + "\n";
                    links.Add("https://8ch.net/" + "/file_store/" + xmlTim[i].InnerText + xmlExt[i].InnerText);
                }
            }
            catch (WebException webEx)
            {
                if (((int)webEx.Status) == 7)                                               // 404
                    this.Gone = true;
                throw webEx;
            }
            return links.ToArray();
        }

        override public string getThreads()
        {
            string URL = "http://8ch.net/" + getURL().Split('/')[3] + "/catalog.json";
            string Res = "";
            string str = "";
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
                    str = xml.ToString();                                                 // JSON to XML again
                }

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(str);
                tNo = doc.DocumentElement.SelectNodes("/root/item/threads/item/no");
                for (int i = 0; i < tNo.Count; i++)
                {
                    Res = Res + "http://8ch.net/" + getURL().Split('/')[3] + "/res/" + tNo[i].InnerText + ".html\n";
                }
            }
            catch (WebException webEx)
            {                                               // I think I should handle this
                                                            //                MessageBox.Show("Connection Error");
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
            string str;

            try
            {
                htmlPage = new WebClient().DownloadString(this.getURL());

                string JURL = this.getURL().Replace(".html", ".json");

                string Content = new WebClient().DownloadString(JURL);

                byte[] bytes = Encoding.ASCII.GetBytes(Content);
                using (var stream = new MemoryStream(bytes))
                {
                    var quotas = new XmlDictionaryReaderQuotas();
                    var jsonReader = JsonReaderWriterFactory.CreateJsonReader(stream, quotas);
                    var xml = XDocument.Load(jsonReader);
                    str = xml.ToString();
                }

                // get single images
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(str);
                XmlNodeList xmlTim = doc.DocumentElement.SelectNodes("/root/posts/item/tim");
                XmlNodeList xmlExt = doc.DocumentElement.SelectNodes("/root/posts/item/ext");
                for (int i = 0; i < xmlExt.Count; i++)
                {
                    string ext = xmlExt[i].InnerText;
                    //                        if(ext == ".webm")
                    //                            ext = ".jpg";
                    thumbs.Add("https://8ch.net/file_store/thumb/" + xmlTim[i].InnerText + ext);

                    htmlPage = htmlPage.Replace("https://8ch.net/file_store/thumb/" + xmlTim[i].InnerText + ext, "thumb/" + xmlTim[i].InnerText + ext);
                    htmlPage = htmlPage.Replace("=\"/file_store/thumb/" + xmlTim[i].InnerText + ext, "=\"thumb/" + xmlTim[i].InnerText + ext);
                    htmlPage = htmlPage.Replace("=\"/file_store/" + xmlTim[i].InnerText + ext, "=\"" + xmlTim[i].InnerText + ext);
                    htmlPage = htmlPage.Replace("https://media.8ch.net/file_store/thumb/" + xmlTim[i].InnerText + ext, "thumb/" + xmlTim[i].InnerText + ext);
                    htmlPage = htmlPage.Replace("https://media.8ch.net/file_store/" + xmlTim[i].InnerText + ext, xmlTim[i].InnerText + ext);
                    htmlPage = htmlPage.Replace("https://8ch.net/file_store/" + xmlTim[i].InnerText + ext, xmlTim[i].InnerText + ext);
                }

                // get images of posts with multiple images
                xmlTim = doc.DocumentElement.SelectNodes("/root/posts/item/extra_files/item/tim");
                xmlExt = doc.DocumentElement.SelectNodes("/root/posts/item/extra_files/item/ext");
                for (int i = 0; i < xmlExt.Count; i++)
                {
                    string ext = xmlExt[i].InnerText;
                    //                        if(ext == ".webm")
                    //                            ext = ".jpg";
                    thumbs.Add("https://8ch.net/file_store/thumb/" + xmlTim[i].InnerText + ext);

                    htmlPage = htmlPage.Replace("https://8ch.net/file_store/thumb/" + xmlTim[i].InnerText + ext, "thumb/" + xmlTim[i].InnerText + ext);
                    htmlPage = htmlPage.Replace("=\"/file_store/thumb/" + xmlTim[i].InnerText + ext, "=\"thumb/" + xmlTim[i].InnerText + ext);
                    htmlPage = htmlPage.Replace("=\"/file_store/" + xmlTim[i].InnerText + ext, "=\"" + xmlTim[i].InnerText + ext);
                    htmlPage = htmlPage.Replace("https://media.8ch.net/file_store/thumb/" + xmlTim[i].InnerText + ext, "thumb/" + xmlTim[i].InnerText + ext);
                    htmlPage = htmlPage.Replace("https://media.8ch.net/file_store/" + xmlTim[i].InnerText + ext, xmlTim[i].InnerText + ext);
                    htmlPage = htmlPage.Replace("https://8ch.net/file_store/" + xmlTim[i].InnerText + ext, xmlTim[i].InnerText + ext);
                }

                htmlPage = htmlPage.Replace("=\"/", "=\"https://8ch.net/");

                for (int i = 0; i < thumbs.Count; i++)
                {
                    General.dlTo(thumbs[i], this.SaveTo + "\\thumb");
                }

                if (!String.IsNullOrWhiteSpace(htmlPage))
                    File.WriteAllText(this.SaveTo + "\\Thread.html", htmlPage); // save thread
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