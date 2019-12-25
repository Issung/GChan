using System.Windows.Forms;

namespace GChan.Controls
{
    class CustomDataGridView : DataGridView
    {
        protected override bool ShowFocusCues { get { return false; } }
    }
}
