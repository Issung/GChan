using System;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;

namespace Binding
{
	/// <summary>
	/// This class inherits from the ListView control
	/// and allows binding the control to a datasource
	/// </summary>
	public class BoundListView : ListView
	{
		#region --SYSTEM CODE--

		private System.ComponentModel.Container components = null;

		#region Component Designer generated code

		private void InitializeComponent()
		{
			// 
			// CompassListView
			// 
			this.Name = "CompassListView";
		}
		#endregion
		#endregion

		#region --VARIABLES--
		CurrencyManager cm = null;
		DataView		dv = null;
		#endregion

		#region --CONSTRUCTOR & DESTRUCTOR--
		public BoundListView()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			base.SelectedIndexChanged +=new EventHandler(CompassListView_SelectedIndexChanged);
			base.ColumnClick +=new ColumnClickEventHandler(CompassListView_ColumnClick);
		}

		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#endregion

		#region --PROPERTIES--
		#region --DATASOURCE--
		private Object source;

		[Bindable(true)]
		[TypeConverter("System.Windows.Forms.Design.DataSourceConverter, System.Design")]
		[Category("Data")]
		[DefaultValue(null)]
		public Object DataSource
		{
			get
			{
				return source;
			}
			set
			{
				source = value;
			}
		}
		#endregion

		#region --DATAMEMBER--

		private String data;

		[Category("Data"), Bindable(false)]
		[Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design", "System.Drawing.Design.UITypeEditor, System.Drawing")]
		[RefreshProperties(RefreshProperties.All)]
		[DefaultValue("")]
		public String DataMember
		{
			get
			{
				return data;
			}
			set
			{
				data = value;
				bind();
			}
		}

		#endregion

		#region --SORTING--

		[Browsable(false)]
		public new SortOrder Sorting
		{
			get
			{
				return base.Sorting;
			}
			set
			{
				base.Sorting = value;
			}
		}
		#endregion

		[Browsable(false)]
		protected new bool MultiSelect
		{
			get
			{
				return base.MultiSelect;
			}
			set
			{
				base.MultiSelect = false;
			}
		}
		#endregion

		#region --METHODS--
		/// <summary>
		/// This method is called everytime the DataMember property is set
		/// </summary>
		private void bind()
		{
			Items.Clear(); //clear the existing list

			if(source is DataSet)
			{
				DataSet ds = source as DataSet;				
				DataTable dt = ds.Tables[0];
				
				if(dt!=null)
				{
					cm = (CurrencyManager)BindingContext[ds, ds.Tables[0].TableName];
					cm.CurrentChanged +=new EventHandler(cm_CurrentChanged);
					dv = (DataView)cm.List;

					Columns.Add(DataMember, ClientRectangle.Width - 17, HorizontalAlignment.Left);

					foreach(DataRow dr in dt.Rows)
					{
						ListViewItem lvi = new ListViewItem(dr[DataMember].ToString());
						lvi.Tag = dr;
						Items.Add(lvi);
					}
					Sorting = SortOrder.Ascending;
					dv.Sort = this.Columns[0].Text + " ASC";
				}
			}
			else
				cm = null;
		}

		#endregion

		#region --EVENTS--

		private void CompassListView_SelectedIndexChanged(object sender, EventArgs e)
		{
			if(this.SelectedIndices.Count>0)
			{				
				if(cm!=null)
				{
					cm.Position = base.SelectedIndices[0];
				}
			}
		}

		private void CompassListView_ColumnClick(object sender, ColumnClickEventArgs e)
		{
			if(Sorting==SortOrder.None || Sorting == SortOrder.Descending)
			{
				Sorting = SortOrder.Ascending;
				dv.Sort = this.Columns[0].Text + " ASC";
			}
			else if(Sorting==SortOrder.Ascending)
			{
				Sorting = SortOrder.Descending;
				dv.Sort = this.Columns[0].Text + " DESC";
			}
		}

		private void cm_CurrentChanged(object sender, EventArgs e)
		{
			this.Items[cm.Position].Selected = true;
		}

		#endregion
	}
}