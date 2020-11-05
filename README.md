# GChan      <img align="right" width="90" height="90" src="http://puu.sh/Grv6g.jpg">
[![Release](https://img.shields.io/github/v/release/issung/GChan?style=for-the-badge)](https://github.com/Issung/GChan/releases)
[![Downloads](https://img.shields.io/github/downloads/issung/GCHan/total?style=for-the-badge)](https://github.com/Issung/GChan/releases)

GChan is an automatic imageboard scraper, it supports multithreading and auto-rechecking of threads with a custom set timer.

GChan is a fork of YChan which aims to improve stability and add extra features which some may find useful.

### Supported Websites: 
* [4chan.org](http://4chan.org/)
* 8Chan (now [8kun.top](https://8kun.top/index.html)) WIP

Requires .NET Framework 4.8 or higher, GUI is made with Winforms.

![GChan Window](http://puu.sh/ERKQ8.png)

## New Features
* Intuitive thread layout using grid.
   * Columns that display thread Subject, Board, ID and File Count.
   * Subject renaming, useful for threads with no subject.
   * Sorting via clicking column headers, supports ascending and descending.
* Ease of adding new threads/boards.
   * Add multiple threads at once seperated by a comma (,).
   * Click Add button without pasting it into the text box to add URL from clipboard.
   * Drag and drop URL to textbox to automatically add it.
* Right click a thread and copy URL to clipboard.
* Copy all thread URLs to clipboard seperated by a comma (,).
* Display of tracked threads and board count on tabs.
* Set a custom subject for a thread, useful if the thread has no subject or a non-descriptive subject.
* New settings and .
   * Option to start GChan with Windows.
      * Option to start hidden within system tray.
   * Option to automatically add a thread's subject to thread's folder name when a thread is automatically or manually removed.
      * Name format choice of 'ID - Subject' or 'Subject - ID'
   * Choices for saved files filename format:
     * ID Only (e.g. '1570301.jpg')
     * Original Filename Only (e.g. 'LittleSaintJames.jpg')
     * ID - OriginalFilename (e.g. '1570301 - LittleSaintJames.jpg')
     * OriginalFilename - ID (e.g. 'LittleSaintJames - 1570301.jpg')
