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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Text.RegularExpressions;

/***********************************************************************************************************
 * Imageboard.cs v0.4                                                                                      *
 * abstract class to use as skeleton for the imageboard classes. all IB classes inherit from this class    *
 * isBoard/isThread contain a Regex check whether the IB class matches or not, basic shit                  *
 * getLinks    -> get Links from Thread, seperated by \n                                                   *
 * getPath     -> get DL Path                                                                              *
 * getThreads  -> same with Thread-Links from Board                                                        *
 * download    -> Download Images from Thread                                                              *
 ***********************************************************************************************************/


namespace YChan {
    public class Imageboard {
        protected string URL;                            // Thread/Board URL
        protected string SaveTo;                         // Path to save to
        protected string imName;                         // Name of the IB
        protected bool Board;                            // Flag to distinguish Boards and Threads of an IB
        protected bool Gone = false;                     // Flag for 404 


        public Imageboard(string url, bool isBoard) {    // Constructor, setting URL and Type (Board/Thread)
            this.URL       = url;
            this.Board     = isBoard;
        }

        public bool isGone() {
            return this.Gone;
        }

        public string getURL() {
            return this.URL;        
        }

        public string getImName() {
            return this.imName;
        }
        public string getPath() {
            return this.SaveTo;
        }

        static public bool isThread(string url) {
            return false;
        }

        static public bool isBoard(string url)  {
            return false;
        }
        virtual public void         download()          {}
        virtual protected string    getLinks()          { return ""; }
        virtual public string       getThreads()        { return ""; }
    }
}
