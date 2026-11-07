using GChan.Controls;
using GChan.Forms;
using GChan.Models.Trackers;
using GChan.Services;
using NLog;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GChan.ViewModels
{
    class MainFormModel : INotifyPropertyChanged
    {
        private readonly ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly MainForm form;
        private readonly ProcessQueue processQueue;

        public MainFormModel(
            MainForm form,
            ProcessQueue processQueue
        )
        {
            this.form = form;
            this.processQueue = processQueue;

            Threads.ListChanged += Threads_ListChanged;
            Boards.ListChanged += Boards_ListChanged;

            processQueue.PausedPropertyChanged += QueuePausedPropertyChanged;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
#if DEBUG
            logger.Trace($"NotifyPropertyChanged! MainFormModel.{propertyName}.");
#endif

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public SortableBindingList<Thread> Threads { get; set; } = [];

        public SortableBindingList<Board> Boards { get; set; } = [];

        public string ThreadsTabText => $"Threads ({Threads.Count})";

        public string BoardsTabText => $"Boards ({Boards.Count})";

        public string NotificationTrayTooltip
        {
            get
            {
                return $"{(QueueIsProcessing ? "Scraping " : string.Empty)}{Threads.Count} thread{(Threads.Count != 1 ? "s" : string.Empty)} and {Boards.Count} board{(Boards.Count != 1 ? "s" : string.Empty)}{(QueueIsPaused ? " currently paused" : string.Empty)}." +
                    "\nClick to show/hide.";
            }
        }

        public bool QueueIsPaused => processQueue.Pause;
        public bool QueueIsProcessing => !processQueue.Pause;

        private void Threads_ListChanged(object sender, ListChangedEventArgs e)
        {
            // Binding the TagPage's text to this text is easily doable, but it causes weird refresh behaviour which makes scrolling on the 
            // grid view a terrible experience, causing it to constantly scroll back to where you were. So setting the text manually resolves this.
            form.threadsTabPage.Text = ThreadsTabText;
        }

        private void Boards_ListChanged(object sender, ListChangedEventArgs e)
        {
            NotifyPropertyChanged(nameof(Boards));
            // We set the text manually instead of using data binding to avoid weird refresh scroll issues, read comment above for threads tabpage.
            form.boardsTabPage.Text = BoardsTabText;
        }

        private void QueuePausedPropertyChanged()
        {
            NotifyPropertyChanged(nameof(QueueIsPaused));
            NotifyPropertyChanged(nameof(QueueIsProcessing));
        }
    }
}