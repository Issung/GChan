using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

/// <summary>
/// Credit goes to Günther M. FOIDL https://github.com/gfoidl
/// https://www.codeproject.com/Articles/37087/DataGridView-that-Saves-Column-Order-Width-and-Vis
/// </summary>
namespace GChan.Controls
{
    [Description("DataGridView but with inbuilt column hide/show context menu and saving/loading preferences of column's visibility, width & ordering.")]
    [ToolboxBitmap(typeof(DataGridView))]
    class PreferencesDataGridView : DataGridView
    {
        ContextMenuStrip contextMenu = new ContextMenuStrip();

        protected override bool ShowFocusCues => false;

        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            if (!DesignMode)
            {
                try
                {
                    LoadPreferences();
                }
                catch (Exception e)
                {
                    Program.Log("Failed to load datagridview preferences.", e);
                }

                foreach (DataGridViewColumn column in Columns)
                {
                    ToolStripMenuItem toolStripItem = new ToolStripMenuItem(column.HeaderText)
                    {
                        CheckOnClick = true,
                        Checked = column.Visible,
                    };
                    toolStripItem.CheckStateChanged += ToolStripItem_CheckStateChanged;
                    contextMenu.Items.Add(toolStripItem);
                    column.HeaderCell.ContextMenuStrip = contextMenu;
                }
            }
        }

        private void ToolStripItem_CheckStateChanged(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            Columns[contextMenu.Items.IndexOf(item)].Visible = item.Checked;
        }

        private void LoadPreferences()
        {
            //Load settings from last version if this is first run.
            if (DataGridViewPreferencesSetting.Default.FirstStart)
            {
                DataGridViewPreferencesSetting.Default.Upgrade();
                DataGridViewPreferencesSetting.Default.Reload();
                DataGridViewPreferencesSetting.Default.FirstStart = false;
                DataGridViewPreferencesSetting.Default.Save();
            }

            if (!DataGridViewPreferencesSetting.Default.ColumnOrder.ContainsKey(this.Name))
                return;

            List<ColumnOrderItem> columnOrder = DataGridViewPreferencesSetting.Default.ColumnOrder[this.Name];

            if (columnOrder != null)
            {
                var sorted = columnOrder.OrderBy(i => i.DisplayIndex);
                foreach (var item in sorted)
                {
                    Columns[item.ColumnIndex].DisplayIndex = item.DisplayIndex;
                    Columns[item.ColumnIndex].Visible = item.Visible;
                    Columns[item.ColumnIndex].FillWeight = item.FillWeight;
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            SavePreferences();
            base.Dispose(disposing);
        }

        private void SavePreferences()
        {
            List<ColumnOrderItem> columnOrder = new List<ColumnOrderItem>();
            DataGridViewColumnCollection columns = Columns;

            for (int i = 0; i < columns.Count; i++)
            {
                columnOrder.Add(new ColumnOrderItem
                {
                    ColumnIndex = i,
                    DisplayIndex = columns[i].DisplayIndex,
                    Visible = columns[i].Visible,
                    FillWeight = columns[i].FillWeight
                });
            }

            DataGridViewPreferencesSetting.Default.ColumnOrder[this.Name] = columnOrder;
            DataGridViewPreferencesSetting.Default.FirstStart = false;
            DataGridViewPreferencesSetting.Default.Save();
        }
    }
}