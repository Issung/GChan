using GChan.Controls;
using GChan.Forms;
using GChan.Properties;
using GChan.Trackers;
using NLog;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GChan.Models
{
    class MainFormModel : INotifyPropertyChanged
    {
        MainForm form;

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
#if DEBUG
            logger.Trace($"NotifyPropertyChanged! propertyName: {propertyName}.");
#endif

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public SortableBindingList<Thread> Threads { get; set; } = new();

        public SortableBindingList<Board> Boards { get; set; } = new();

        public string ThreadsTabText => $"Threads ({Threads.Count})";

        public string BoardsTabText => $"Boards ({Boards.Count})";

        public string NotificationTrayTooltip {
            get {
                return $"Scraping {Threads.Count} thread{(Threads.Count != 1 ? "s" : "")} and {Boards.Count} board{(Boards.Count != 1 ? "s" : "")} every {Settings.Default.ScanTimer / 60 / 1000} minute{((Settings.Default.ScanTimer / 60 / 1000) != 1 ? "s" : "")}." +
                    "\nClick to show/hide.";
            }
        }

        private readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public MainFormModel(MainForm form)
        {
            this.form = form;

            Threads.ListChanged += Threads_ListChanged;
            Boards.ListChanged += Boards_ListChanged;
        }

        private void Threads_ListChanged(object sender, ListChangedEventArgs e)
        {
            form.threadsTabPage.Text = ThreadsTabText;
        }

        private void Boards_ListChanged(object sender, ListChangedEventArgs e)
        {
            NotifyPropertyChanged(nameof(Boards));
            form.boardsTabPage.Text = BoardsTabText;
        }
    }
}