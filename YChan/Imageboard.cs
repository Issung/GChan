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

namespace GChan
{
    public abstract class Imageboard
    {
        protected string URL;                            // Thread/Board URL
        protected string SaveTo;                         // Path to save to
        protected string siteName;                       // Name of the site
        protected string threadName;                     // Name of the thread.
        protected bool board;                            // Flag to distinguish Boards and Threads of an IB
        protected bool Gone = false;                     // Flag for 404

        // Constructor, setting URL and Type (Board/Thread)
        public Imageboard(string url, bool isBoard)
        {
            this.URL = url;
            this.board = isBoard;
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

        public string getPath()
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

        public override string ToString()
        {
            return this.URL;
        }
    }
}