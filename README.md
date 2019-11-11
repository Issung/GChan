# GChan
GChan is a fork of YChan which aims to improve on it and add some extra features which some may find useful. YChan is an automatic imageboard scraper, it supports multithreading and auto-rechecking of threads with a custom set timer.

Supported Websites: [4chan.org](http://4chan.org/) and will attempt to support 8Chan (now called 8kun) once I get a chance.

Requires .NET Framework 4.5 or higher, GUI is made with Winforms.

![GChan Window](https://i.imgur.com/K3gBBsM.png)

## New Features
* Improvements to the user interface and quality of life.
    * New thread layout using grid.
        * Columns that seperate thread subject, board and ID.
        * Subject renaming useful for threads with no subject (WIP).
* Add multiple threads at once seperated by a comma (,).
* Copy URLs of all followed threads to clipboard seperated by a comma (,).
* Right click a thread and copy URL to clipboard.
* Set a custom subject to be displayed for a thread, useful if the thread has no subject or a non-descriptive subject.
* New settings layout and new settings.
	* Option to start GChan with Windows.
	* Option to automatically add thread's subject to thread's folder name when automatically or manually removed.
