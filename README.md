# GChan      <img align="right" width="90" height="90" src="http://puu.sh/Grv6g.jpg">
[![Release](https://img.shields.io/github/release/Issung/GChan.svg)](https://github.com/Issung/GChan/releases)
[![Downloads](https://img.shields.io/github/downloads/Issung/GChan/total.svg)](https://github.com/Issung/GChan/releases)

GChan is a fork of YChan which aims to improve on it and add some extra features which some may find useful. YChan is an automatic imageboard scraper, it supports multithreading and auto-rechecking of threads with a custom set timer.

Supported Websites: [4chan.org](http://4chan.org/) and will attempt to support 8Chan (now called 8kun) once I get a chance.

Requires .NET Framework 4.8 or higher, GUI is made with Winforms.

![GChan Window](http://puu.sh/ERKQ8.png)

## New Features
* Improvements to the user interface and quality of life.
    * New thread layout using grid.
        * Columns that display thread Subject, Board, ID and File Count.
        * Subject renaming, useful for threads with no subject.
        * Sorting via clicking column headers, supports ascending and descending.
    * Display of tracking threads and board count on tabs.
* Add multiple threads at once seperated by a comma (,).
* Copy URLs of all followed threads to clipboard seperated by a comma (,).
* Right click a thread and copy URL to clipboard.
* Set a custom subject to be displayed for a thread, useful if the thread has no subject or a non-descriptive subject.
* New settings layout.
* Option to start GChan with Windows.
* Option to automatically add a thread's subject to thread's directory when thread automatically or manually removed.
* Choices for saved files filename format:
  * ID Only (eg. '1570301.jpg')
  * Original Filename Only (eg. 'LittleSaintJames.jpg')
  * ID - OriginalFilename (eg. '1570301 - LittleSaintJames.jpg')
* Add URLs by just clicking the Add Button, no need to paste it in the box (Lazy Entry)
