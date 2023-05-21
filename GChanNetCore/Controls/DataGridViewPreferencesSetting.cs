using GChan.Helpers;
using System.Configuration;

/// <summary>
/// Credit goes to Günther M. FOIDL https://github.com/gfoidl
/// https://www.codeproject.com/Articles/37087/DataGridView-that-Saves-Column-Order-Width-and-Vis
/// </summary>
namespace GChan.Controls
{
	internal sealed class DataGridViewPreferencesSetting : ApplicationSettingsBase
	{
		private static DataGridViewPreferencesSetting _defaultInstance = (DataGridViewPreferencesSetting)Synchronized(new DataGridViewPreferencesSetting());
		
		public static DataGridViewPreferencesSetting Default => _defaultInstance;

		// Because there can be more than one DGV in the user-application
		// a dictionary is used to save the settings for this DGV.
		// As name of the control is used as the dictionary key.
		[UserScopedSetting]
		[DefaultSettingValue("")]
		[SettingsSerializeAs(SettingsSerializeAs.Xml)]
		public XmlSerializableDictionary<string, List<ColumnOrderItem>> ColumnOrder
		{
			get
			{
				var temp = this["ColumnOrder"];
				return (temp as XmlSerializableDictionary<string, List<ColumnOrderItem>>) ?? new(); 
			}
			set
			{
				this["ColumnOrder"] = value; 
			}
		}

		
		[UserScopedSetting]
		[SettingsSerializeAs(SettingsSerializeAs.String)]
		[DefaultSettingValue("True")]
		public bool FirstStart
		{
			get
			{
				return (bool)this["FirstStart"];
			}
			set
			{
				this["FirstStart"] = value;

			}
		}
	}

	[Serializable]
	public sealed class ColumnOrderItem
	{
		public int DisplayIndex { get; set; }
		public float FillWeight { get; set; }
		public bool Visible { get; set; }
		public int ColumnIndex { get; set; }
	}
}