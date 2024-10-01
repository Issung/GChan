# GChan      <img align="right" width="90" height="90" src="https://i.imgur.com/orC8f3w.png">
[![Release](https://img.shields.io/github/v/release/issung/GChan?style=for-the-badge)](https://github.com/Issung/GChan/releases)
[![Downloads](https://img.shields.io/github/downloads/issung/GCHan/total?style=for-the-badge)](https://github.com/Issung/GChan/releases)
[![Discord](https://img.shields.io/discord/806067523853746196?color=%23738BD8&label=discord&style=for-the-badge)](https://discord.gg/Ayh6UasAVn)

GChan is an automatic imageboard scraper, it supports multithreading and auto-rechecking of threads with a custom set timer.

GChan is a fork of YChan which aims to improve stability and add extra features which some may find useful.

#### Project Status: Active Development + Bug Maintenance.

### Supported Websites: 
* [4chan.org](http://4chan.org/)

Requires .NET Framework 4.8 or higher, made with Windows Forms.

![GChan Window](http://puu.sh/ERKQ8.png)

## Contributing
When contributing please:
1. First make an issue to discuss the change or if it is an existing issue please ask on the issue to be assigned the problem, someone may already be working on it.
2. On your fork's branch please prefix the branch with `feature/` or `bugfix/` and the use the issue number. E.g. `feature/#5-support-2chan`.
3. Show proof of testing in your pull request, or have created tests in the code.
4. Please keep the changes restricted to one area, make the purpose of the change easy to locate and easy to understand.

## Features
* Intuitive thread layout using grid.
   * Adjustable columns display.
   * Subject renaming.
   * Sorting.
* Ease of adding new threads/boards.
   * Add multiple threads at once seperated by a comma (,).
   * Click Add button without pasting it into the text box to add URL from clipboard.
   * Drag and drop URL into window to add it.
* Right click a thread and copy URL to clipboard.
* Copy all thread URLs to clipboard seperated by a comma (,).
* Display of tracked threads and board count on tabs.
* Many settings:
   * Optionally start with Windows, optionally hidden.
   * Options to add a thread's subject to thread's directory when a thread is removed.
      * Name format choice of 'ID - Subject' or 'Subject - ID'.
   * Choices for saved files filename format:
     * ID Only (e.g. '1570301.jpg')
     * Original Filename Only (e.g. 'LittleSaintJames.jpg')
     * ID - OriginalFilename (e.g. '1570301 - LittleSaintJames.jpg')
     * OriginalFilename - ID (e.g. 'LittleSaintJames - 1570301.jpg')
