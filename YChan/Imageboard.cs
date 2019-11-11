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

/***********************************************************************************************************
 * Imageboard.cs v0.4                                                                                      *
 * abstract class to use as skeleton for the imageboard classes. all IB classes inherit from this class    *
 * isBoard/isThread contain a Regex check whether the IB class matches or not, basic shit                  *
 * getLinks    -> get Links from Thread, seperated by \n                                                   *
 * getPath     -> get DL Path                                                                              *
 * getThreads  -> same with Thread-Links from Board                                                        *
 * download    -> Download Images from Thread                                                              *
 ***********************************************************************************************************/

using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace GChan
{
    public abstract class Imageboard
    {
        public const string NO_SUBJECT = "No Subject";

        protected string URL;                            // Thread/Board URL
        protected string SaveTo;                         // Path to save to
        protected string siteName;                       // Name of the site
        protected bool board;                            // Flag to distinguish Boards and Threads of an IB
        protected bool Gone = false;                     // Flag for 404

        protected string subject;                        // Name of the thread.
        protected string boardName;
        protected string id;

        protected string customSubject = null;                     // A custom subject able to be set by the user.

        /// <summary>
        /// Returns the thread's subject name, if there is a custom subject name then that is set instead.
        /// </summary>
        public string Subject { get { return customSubject == null ? subject : customSubject; } }
        public string BoardName { get { return boardName; } }
        public string ID { get { return id; } }

        // Constructor, setting URL and Type (Board/Thread)
        public Imageboard(string url, bool isBoard)
        {
            this.URL = url;
            this.board = isBoard;
            boardName = GetBoardName();

            if (!board)
            {
                id = GetID();
            }
        }

        public bool isGone()
        {
            return this.Gone;
        }

        public string getURL()
        {
            return this.URL;
        }

        public string getImName()
        {
            return this.siteName;
        }

        public string GetPath()
        {
            return this.SaveTo;
        }

        static public bool urlIsThread(string url)
        {
            return false;
        }

        static public bool urlIsBoard(string url)
        {
            return false;
        }

        public bool isBoard()
        {
            return board;
        }

        public abstract void download();

        public abstract void download(object callback);

        protected abstract string[] getLinks();

        public abstract string[] getThreads();

        public string GetBoardName()
        {
            Regex regex = new Regex(@"(\/(\w|\d)+\/)");

            // Temporarily add a / incase url doesnt end with one.
            var matches = regex.Matches(URL + "/"); 

            if (matches.Count > 0)
            {
                return matches[0].Value;
            }
            else
            {
                return null;
            }
        }

        protected string GetThreadName()
        {
            string subject;

            if (board)
            {
                subject = "No Subject";
            }
            else
            {

                string JSONUrl = "http://a.4cdn.org/" + getURL().Split('/')[3] + "/thread/" + getURL().Split('/')[5] + ".json";
                string Content = new WebClient().DownloadString(JSONUrl);

                dynamic data = JObject.Parse(Content);

                try
                {
                    subject = data.posts[0].sub.ToString();
                }
                catch (RuntimeBinderException rbe)
                {
                    subject = NO_SUBJECT;
                }

            }

            return subject;
        }

        public bool HasCustomSubject()
        {
            return customSubject != null;
        }

        public void SetCustomSubject(string newCustomSubject)
        {
            customSubject = newCustomSubject;
        }

        public string GetID()
        {
            if (!board)
            {
                return URL.Substring(URL.LastIndexOf('/') + 1);
            }

            return null;
        }

        public override string ToString()
        {
            return this.URL;
        }
    }
}