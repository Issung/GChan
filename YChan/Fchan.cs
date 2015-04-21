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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using System.Net;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Linq;
using System.Runtime.Serialization.Json;
using System.IO;
// see Infinitechan.cs for explaneation

namespace YChan {
    class Fchan : Imageboard {
        public static string regThread  = "boards.4chan.org/[a-zA-Z0-9]*?/thread/[0-9]*";
        public static string regBoard   = "boards.4chan.org/[a-zA-Z0-9]*?/$";

        
        public Fchan(string url, bool isBoard) : base(url, isBoard) {
            this.Board     = isBoard;
            this.imName    = "4chan";
            if(!isBoard) {
                Match match = Regex.Match(url, @"boards.4chan.org/[a-zA-Z0-9]*?/thread/\d*");
                this.URL       = "http://" + match.Groups[0].Value;
            } else {
                this.URL = url;
            }
            if(!isBoard)
                this.SaveTo    = General.path + "\\" + this.imName+ "\\" + getURL().Split('/')[3] + "\\" + getURL().Split('/')[5];
            else
                this.SaveTo    = General.path + "\\" + this.imName + "\\" + getURL().Split('/')[3];
        }

        public new static bool isThread(string url) { 
            Regex urlMatcher = new Regex(regThread);
            if(urlMatcher.IsMatch(url))
                return true;
            else
                return false;
        }
        
        public new static bool isBoard(string url) { 
            Regex urlMatcher = new Regex(regBoard);
            if(urlMatcher.IsMatch(url))
                return true;
            else
                return false;
        }

        override protected string getLinks() {
            string exed = "";
            string JSONUrl = "http://a.4cdn.org/" + getURL().Split('/')[3] +"/thread/" + getURL().Split('/')[5] +".json";
            string baseURL = "http://i.4cdn.org/" + getURL().Split('/')[3] + "/";
            string str = "";
            XmlNodeList xmlTim;
            XmlNodeList xmlExt;
            try {
                string Content = new WebClient().DownloadString(JSONUrl);

                byte[] bytes = Encoding.ASCII.GetBytes(Content);
                using(var stream = new MemoryStream(bytes)) {
                    var quotas = new XmlDictionaryReaderQuotas();
                    var jsonReader = JsonReaderWriterFactory.CreateJsonReader(stream, quotas);
                    var xml = XDocument.Load(jsonReader);
                    str = xml.ToString();
                }

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(str);
                if(getURL().Split('/')[3] == "f")
                    xmlTim     = doc.DocumentElement.SelectNodes("/root/posts/item/filename");
                else
                    xmlTim     = doc.DocumentElement.SelectNodes("/root/posts/item/tim");

                xmlExt     = doc.DocumentElement.SelectNodes("/root/posts/item/ext");
                for(int i = 0; i < xmlExt.Count; i++) {
                    exed = exed + baseURL + xmlTim[i].InnerText + xmlExt[i].InnerText + "\n";
                }
            } catch(WebException webEx) {
                if(((int) webEx.Status) == 7)
                    this.Gone = true;

//                MessageBox.Show("Webex: (" + (int)webEx.Status + ") " + webEx.Message); //DEBUG
                throw webEx;

                //                if(((HttpWebResponse) webEx.Response).StatusCode == HttpStatusCode.NotFound)
                //                MessageBox.Show("Removed " + URL);

            }
            return exed;
        }

        override public string getThreads() {
//            MessageBox.Show("getthreads");
            string URL = "http://a.4cdn.org/" + getURL().Split('/')[3] + "/catalog.json";
            string Res = "";
            string str = "";
            XmlNodeList tNa;
            XmlNodeList tNo;
            try {
                string json = new WebClient().DownloadString(URL);
                byte[] bytes = Encoding.ASCII.GetBytes(json);
                using(var stream = new MemoryStream(bytes)) {
                    var quotas = new XmlDictionaryReaderQuotas();
                    var jsonReader = JsonReaderWriterFactory.CreateJsonReader(stream, quotas);
                    var xml = XDocument.Load(jsonReader);
                    str = xml.ToString();
                }

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(str);
                tNo     = doc.DocumentElement.SelectNodes("/root/item/threads/item/no");
                tNa     = doc.DocumentElement.SelectNodes("/root/item/threads/item/semantic_url");
                for(int i = 0; i < tNo.Count; i++) {
                    Res = Res + "http://boards.4chan.org/" + getURL().Split('/')[3] + "/thread/" + tNo[i].InnerText + "/" + tNa[i].InnerText + "\n";
                }
            } catch(WebException webEx) {
//                MessageBox.Show("Connection Error");
            }
//            MessageBox.Show(Res);

            return Res;
        }

        override public void download() {
            string[] URLs;
            string[] thumbs;
            string strThumbs = "";
            string baseURL = "//i.4cdn.org/" + getURL().Split('/')[3] + "/";
            string website  = "";
            if(General.loadHTML) {
                try {
                    website = new WebClient().DownloadString(this.getURL());

                    string JURL =  "http://a.4cdn.org/" + getURL().Split('/')[3] +"/thread/" + getURL().Split('/')[5] +".json";
                    string Res = "";
                    string str = "";

                    string json = new WebClient().DownloadString(JURL);
                    byte[] bytes = Encoding.ASCII.GetBytes(json);
                    using(var stream = new MemoryStream(bytes)) {
                        var quotas = new XmlDictionaryReaderQuotas();
                        var jsonReader = JsonReaderWriterFactory.CreateJsonReader(stream, quotas);
                        var xml = XDocument.Load(jsonReader);
                        str = xml.ToString();
                    }

                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(str);
                    XmlNodeList xmlTim     = doc.DocumentElement.SelectNodes("/root/posts/item/tim");
                    XmlNodeList xmlExt     = doc.DocumentElement.SelectNodes("/root/posts/item/ext");

                    for(int i = 0; i < xmlExt.Count; i++) {
                        string old =baseURL + xmlTim[i].InnerText + xmlExt[i].InnerText;
                        string rep = xmlTim[i].InnerText + xmlExt[i].InnerText;
                        website = website.Replace(old, rep);
                        Regex nmb = new Regex("//[0-9].t.4cdn.org/"+getURL().Split('/')[3] + "/" + xmlTim[i].InnerText +"s.jpg");

                        old = "//t.4cdn.org/" + getURL().Split('/')[3] + "/" + xmlTim[i].InnerText +"s.jpg";
                        rep = "thumb/" + xmlTim[i].InnerText +"s.jpg";
                        strThumbs = strThumbs + "http:" + old +"\n";

                        website = nmb.Replace(website, rep);
//                        website = website.Replace(old, rep);
                    }

                    website = website.Replace("=\"//", "=\"http://");

                    if(!Directory.Exists(this.SaveTo))
                        Directory.CreateDirectory(this.SaveTo);

                    thumbs = strThumbs.Split('\n');

                    for(int i = 0; i < thumbs.Length-1; i++) {
                        General.dlTo(thumbs[i],this.SaveTo + "\\thumb");
                    }

                } catch(WebException webEx) {
                    // should be handled
                }
                if(website != "")
                    File.WriteAllText(this.SaveTo+"\\Thread.html", website);

            }
            

        
            try {
                URLs = Regex.Split(getLinks(), "\n");
            } catch(WebException webEx) {
                if(((int) webEx.Status) == 7)
                    this.Gone = true;
                return;
            }

            for(int y = 0; y < URLs.Length-1; y++) {
                General.dlTo(URLs[y], this.SaveTo);
            }
        }
    }
}
