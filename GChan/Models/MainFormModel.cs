using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using GChan.Controls;
using GChan.Trackers;
using GChan.Forms;
using System.Windows.Forms;

namespace GChan.Models
{
    class MainFormModel : INotifyPropertyChanged
    {
        MainForm form;

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
#if DEBUG
            Console.WriteLine($"NotifyPropertyChanged! propertyName: {propertyName}");
#endif

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public SortableBindingList<Thread> Threads { get; set; } = new SortableBindingList<Thread>();

        public BindingList<Board> Boards { get; set; } = new BindingList<Board>();

        public string ThreadsTabText => $"Threads ({Threads.Count})";

        public string BoardsTabText => $"Boards ({Boards.Count})";

        public string NotificationTrayTooltip {
            get {
                return $"Scraping {Threads.Count} thread{(Threads.Count != 1 ? "s" : "")} and {Boards.Count} board{(Boards.Count != 1 ? "s" : "")} every {Properties.Settings.Default.ScanTimer / 60 / 1000} minute{((Properties.Settings.Default.ScanTimer / 60 / 1000) != 1 ? "s" : "")}." +
                    "\nClick to show/hide.";
            }
        }

        public MainFormModel(MainForm form)
        {
            this.form = form;

            Threads.ListChanged += Threads_ListChanged;
            Boards.ListChanged += Boards_ListChanged;
        }

        private void Threads_ListChanged(object sender, ListChangedEventArgs e)
        {
            int topRowIndex = form.threadGridView.FirstDisplayedScrollingRowIndex; //Save top row index to use after updating grid view to keep scroll position.

            NotifyPropertyChanged(nameof(Threads));

            if (form.threadGridView.Rows.Count > 0) //If there are no rows let the grid view handle itself.
                form.threadGridView.FirstDisplayedScrollingRowIndex = Math.Max(0, topRowIndex);    //Reset it to saved index after list updated. Credit: https://stackoverflow.com/a/5159926/8306962

            form.threadsTabPage.Text = ThreadsTabText;
        }

        private void Boards_ListChanged(object sender, ListChangedEventArgs e)
        {
            NotifyPropertyChanged(nameof(Boards));
            form.boardsTabPage.Text = BoardsTabText;
        }
    }
}