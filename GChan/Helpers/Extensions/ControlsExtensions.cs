using System.Windows.Forms;

namespace GChan.Helpers.Extensions
{
    public static class ControlsExtensions
    {
        /// <returns>-1 if no selected row.</returns>
        public static int SelectedRowIndex(this DataGridView dataGridView)
        {
            return dataGridView?.CurrentCell?.RowIndex ?? -1;
        }
    }
}
