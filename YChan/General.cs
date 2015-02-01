using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Net;
using System.IO;

namespace YChan {
    class General {
        public static string path  = "";                                                    // dl Path
        public static int    timer = 0;                                                     // Timer for frmMain.scnTimer
        public static bool   loadHTML = false;                                              // HTML download activated?
        public static bool   firstStart = false;                                            // First start?
        public static bool   saveOnClose = false;                                           // saveonclose activated?
        public static bool   minimizeToTray = true;                                         // Minimize to tray activated?

        public static string loadURLs(bool board) {                                         // read saved URLS
            if(board && File.Exists(Application.CommonAppDataPath+"\\boards.dat"))
                return File.ReadAllText(Application.CommonAppDataPath+"\\boards.dat");
            else if(!board && File.Exists(Application.CommonAppDataPath+"\\threads.dat"))
                return File.ReadAllText(Application.CommonAppDataPath+"\\threads.dat");
            else
                return "";
        }

        public static void writeURLs(List<Imageboard> Boards, List<Imageboard> Threads) {   // save URLS
            string Buffer = "";
            for(int i = 0; i < Boards.Count; i++)
                Buffer = Buffer + Boards[i].getURL() + "\n";
            File.WriteAllText(Application.CommonAppDataPath+"\\boards.dat", Buffer);      
            
            Buffer = "";

            for(int i = 0; i < Threads.Count; i++)
                Buffer = Buffer + Threads[i].getURL() + "\n";
            File.WriteAllText(Application.CommonAppDataPath+"\\threads.dat", Buffer);
        }

        public static bool IsDigitsOnly(string str) {                                      
            foreach(char c in str) {
                if(c < '0' || c > '9')
                    return false;
            }

            return true;
        }

        public static void setSettings(string path, int time, bool HTML, bool Save, bool tray) { // set settings
            General.path = path;
            General.timer = time;
            General.loadHTML = HTML;
            General.saveOnClose = Save;
            General.minimizeToTray = tray;
            General.writeSettings();
        }

        public static void writeSettings() {                                                    // save settings in ugly XML
            string lines = "<?xml version=\"1.0\"?>\r\n<data>\r\n<timer>" + General.timer + "</timer>\r\n<path>" + General.path + "</path>\r\n<loadhtml>";
            if(loadHTML)
                lines = lines + "1";
            else
                lines = lines + "0";
            lines = lines + "</loadhtml>\r\n<saveonclose>";
            if(saveOnClose)
                lines = lines + "1";
            else
                lines = lines + "0";
            lines = lines + "</saveonclose><minimizetotray>";
            if(minimizeToTray)
                lines = lines + "1";
            else
                lines = lines + "0";
            lines = lines + "</minimizetotray>\r\n</data>\r\n";
            
            StreamWriter file = new StreamWriter(Application.CommonAppDataPath+"\\YCHAN2.xml");
            file.WriteLine(lines);
            file.Close();
        }

        public static void loadSettings() {                                                 // read settings from XML
            if(File.Exists(Application.CommonAppDataPath+"\\YCHAN2.xml")) {
                XmlDocument doc = new XmlDocument();
                doc.Load(Application.CommonAppDataPath+"\\YCHAN2.xml");

                XmlNode NodTimer    = doc.DocumentElement.SelectSingleNode("/data/timer");
                string Time         = NodTimer.InnerText;

                XmlNode NodPath     = doc.DocumentElement.SelectSingleNode("/data/path");
                string Path         = NodPath.InnerText;

                XmlNode NodLoadhtml     = doc.DocumentElement.SelectSingleNode("/data/loadhtml");
                bool html = (NodLoadhtml.InnerText == "1");

                XmlNode NodSaveOnClose = doc.DocumentElement.SelectSingleNode("/data/saveonclose");
                bool sav;
                if(NodSaveOnClose == null)
                    sav = false;
                else
                    sav = (NodSaveOnClose.InnerText == "1");

                XmlNode NodMinimizeToTray = doc.DocumentElement.SelectSingleNode("/data/minimizetotray");
                bool tray;
                if(NodMinimizeToTray == null)
                    tray = true;
                else
                    tray = (NodMinimizeToTray.InnerText == "1");

                int iTime = 10000;
                if(IsDigitsOnly(Time))
                    iTime = int.Parse(Time);

                General.setSettings(Path, iTime, html, sav, tray);                      
            } else {
                firstStart = true;                                                                      // No settings file,
                General.setSettings("C:\\", 10000, false, false, true);                                 // set default values
            }
            
            if(!File.Exists(Application.CommonAppDataPath+"\\2.2")){                                    // Old settings file, new
                firstStart = true;                                                                      // version, first start,
                File.Create(Application.CommonAppDataPath+"\\2.2").Dispose();                           // create file to save version
            }
        }
        public static Imageboard createNewIMB(string url, bool board) {                                 // Create a new Imageboard
            if(!board) {
                if(Fchan.isThread(url))                                                                 // if FChan, create FChan
                    return new Fchan(url, board);
                else if(Infinitechan.isThread(url))
                    return new Infinitechan(url, board);
            } else {
                if(Fchan.isBoard(url))
                    return new Fchan(url, board);                                                       // if 8chan, create 8chan
                else if(Infinitechan.isBoard(url))
                    return new Infinitechan(url, board);
            }
            return null;
        }
        private static string GetFileName(string hrefLink) {
            string[] parts = hrefLink.Split('/');
            string fileName = "";

            if(parts.Length > 0)
                fileName = parts[parts.Length - 1];
            else
                fileName = hrefLink;

            return fileName;
        }

        public static bool dlTo(string url, string dir) {                                               // download to dir

            if(!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            string FN = GetFileName(url);
            dir = dir + "\\"+ FN;
            try {
                if(!File.Exists(dir)) {
                    WebClient webClient = new WebClient();
 //                   MessageBox.Show(url);
//                    MessageBox.Show(dir);
                    webClient.DownloadFile(url, dir);
                }
                return true;
            } catch(WebException WebE) {
                return false;
                throw WebE;
            }
        }
    }
}
