using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using DetroitDiesel.Collections;
using DetroitDiesel.Common;
using DetroitDiesel.Interfaces;
using DetroitDiesel.Net;
using DetroitDiesel.UnitConversion;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Parameters.Properties;
using SapiLayer1;

namespace DetroitDiesel.Windows.Forms.Diagnostics.Panels.Parameters;

public sealed class ListParameterCompare : UserControl, ISearchable, ISupportEdit, IProvideHtml
{
	internal enum ImageIndex
	{
		NoImage = -1,
		Edited,
		Group,
		GroupEdited,
		Normal
	}

	private class GroupItem
	{
		private Parameter parameter;

		private DictionaryEntry entry;

		private CodingChoice codingChoice;

		private bool alwaysShow;

		private bool modifyHeader;

		private CodingParameterGroup codingParameterGroup;

		private bool isEngineeringCorrectionFactor;

		public string Qualifier
		{
			get
			{
				if (parameter == null)
				{
					if (codingChoice == null)
					{
						if (codingParameterGroup == null)
						{
							return entry.Key.ToString();
						}
						return codingParameterGroup.Qualifier;
					}
					return codingChoice.ParameterGroup.Qualifier;
				}
				return parameter.Qualifier;
			}
		}

		public string Name
		{
			get
			{
				if (parameter == null)
				{
					if (codingChoice == null)
					{
						return entry.Key.ToString();
					}
					return codingChoice.ParameterGroup.Name;
				}
				return parameter.Name;
			}
		}

		public object Value
		{
			get
			{
				if (parameter == null)
				{
					if (codingChoice == null)
					{
						if (codingParameterGroup == null)
						{
							return entry.Value;
						}
						return string.Empty;
					}
					return codingChoice.Meaning;
				}
				return parameter.Value;
			}
		}

		public object Precision
		{
			get
			{
				if (parameter == null)
				{
					return -1;
				}
				return parameter.Precision;
			}
		}

		public string Units
		{
			get
			{
				if (parameter == null)
				{
					return string.Empty;
				}
				return parameter.Units;
			}
		}

		public bool AlwaysShow => alwaysShow;

		public bool ModifyHeader => modifyHeader;

		public bool ReadOnly
		{
			get
			{
				if (parameter == null)
				{
					return true;
				}
				return parameter.ReadOnly;
			}
		}

		public string PartNumber
		{
			get
			{
				if (codingChoice == null)
				{
					if (parameter == null && codingParameterGroup == null)
					{
						return string.Empty;
					}
					return "n/a";
				}
				return codingChoice.Part.ToString();
			}
		}

		public bool IsDefaultString => codingParameterGroup != null;

		public bool IsPartNumberInherited
		{
			get
			{
				if (codingChoice != null && !IsDefaultString)
				{
					return codingChoice.Parameter == null;
				}
				return false;
			}
		}

		public bool IsEngineeringCorrectionFactor => isEngineeringCorrectionFactor;

		public GroupItem(CodingParameterGroup codingParameterGroup, CodingChoice defaultStringChoice)
		{
			this.codingParameterGroup = codingParameterGroup;
			codingChoice = defaultStringChoice;
			alwaysShow = true;
		}

		public GroupItem(Parameter parameter, CodingChoice fragmentChoice, bool isEngineeringCorrectionFactor)
		{
			this.parameter = parameter;
			codingChoice = fragmentChoice;
			this.isEngineeringCorrectionFactor = isEngineeringCorrectionFactor;
		}

		public GroupItem(DictionaryEntry entry, bool isIdentification)
		{
			this.entry = entry;
			alwaysShow = (modifyHeader = isIdentification);
		}
	}

	private enum ChangedStatus
	{
		NotChanged,
		Changed
	}

	private static int columnAutoWidth = -1;

	[Category("Behavior")]
	[Description("Show differences only")]
	[DefaultValue(true)]
	private bool showDifferencesOnly = true;

	[Category("Behavior")]
	[Description("Filters out items that only exist in a connected data source.")]
	[DefaultValue(false)]
	private bool showOfflineItemsOnly;

	private CompareSource[] sources;

	private Channel[] relatedOnlineChannels;

	private Dictionary<string, bool> expandedStates = new Dictionary<string, bool>();

	private Color[] colors = new Color[10]
	{
		Color.FromArgb(255, 255, 192),
		Color.FromArgb(255, 192, 255),
		Color.FromArgb(255, 192, 192),
		Color.FromArgb(192, 255, 255),
		Color.FromArgb(192, 255, 192),
		Color.FromArgb(192, 192, 255),
		Color.FromArgb(192, 192, 192),
		Color.FromArgb(192, 192, 160),
		Color.FromArgb(192, 160, 192),
		Color.FromArgb(192, 160, 160)
	};

	private bool updateColumnHeaders;

	private bool dirty;

	private EditSupportHelper editSupport = new EditSupportHelper();

	private IContainer components;

	private ListViewEx listView;

	private ColumnHeader columnParameter;

	private ColumnHeader columnUnits;

	private ImageList stateImages;

	public bool ShowDifferencesOnly
	{
		get
		{
			return showDifferencesOnly;
		}
		set
		{
			if (value != showDifferencesOnly)
			{
				showDifferencesOnly = value;
				BuildList();
			}
		}
	}

	public bool ShowOfflineItemsOnly
	{
		get
		{
			return showOfflineItemsOnly;
		}
		set
		{
			if (value != showOfflineItemsOnly)
			{
				showOfflineItemsOnly = value;
				BuildList();
			}
		}
	}

	public int SourceCount
	{
		get
		{
			if (sources == null)
			{
				return 0;
			}
			return sources.Length;
		}
		set
		{
			if (value > colors.Length)
			{
				throw new ArgumentOutOfRangeException("value", value, "Exceeded maximum sources allowed");
			}
			if (sources != null && value == sources.Length)
			{
				return;
			}
			((ListView)(object)listView).Items.Clear();
			((ListView)(object)listView).Columns.Clear();
			if (sources != null)
			{
				CompareSource[] array = sources;
				foreach (CompareSource compareSource in array)
				{
					compareSource.SourceChanged -= CompareSource_SourceChanged;
					compareSource.SourceContentChanged -= CompareSource_SourceContentChanged;
					compareSource.Clear();
				}
			}
			sources = new CompareSource[value];
			relatedOnlineChannels = new Channel[value];
			((ListView)(object)listView).Columns.Add(columnParameter);
			for (int j = 0; j < value; j++)
			{
				((ListView)(object)listView).Columns.Add(Resources.ListParameterComparePartHeader, 0);
				sources[j] = new CompareSource();
				sources[j].SourceChanged += CompareSource_SourceChanged;
				sources[j].SourceContentChanged += CompareSource_SourceContentChanged;
				sources[j].Color = colors[j];
				((ListView)(object)listView).Columns.Add(string.Format(CultureInfo.CurrentCulture, Resources.ColumnHeaderFormatSourceN, j + 1), 100);
			}
			((ListView)(object)listView).Columns.Add(columnUnits);
		}
	}

	public string SelectedParameter
	{
		get
		{
			List<string> list = new List<string>();
			if (((ListView)(object)listView).SelectedItems.Count > 0)
			{
				ListViewItem listViewItem = ((ListView)(object)listView).SelectedItems[0];
				ListViewExGroupItem val = (ListViewExGroupItem)(object)((listViewItem is ListViewExGroupItem) ? listViewItem : null);
				if (val.Parent != null)
				{
					list.Add(((ListViewItem)(object)val.Parent).Name);
				}
				list.Add(((ListViewItem)(object)val).Name);
			}
			return string.Join("/", list);
		}
	}

	public bool CanSearch => ((ISearchable)listView).CanSearch;

	public bool CanUndo => false;

	public bool CanCopy
	{
		get
		{
			editSupport.SetTarget((object)base.ActiveControl);
			return editSupport.CanCopy;
		}
	}

	public bool CanDelete => false;

	public bool CanPaste => false;

	public bool CanCut => false;

	public bool CanSelectAll
	{
		get
		{
			editSupport.SetTarget((object)base.ActiveControl);
			return editSupport.CanSelectAll;
		}
	}

	public bool CanProvideHtml
	{
		get
		{
			if (SourceCount > 0)
			{
				return sources[0].Loaded;
			}
			return false;
		}
	}

	public string StyleSheet => string.Empty;

	public event EventHandler SelectedParameterChanged;

	public CompareSource Source(int index)
	{
		if (index < SourceCount)
		{
			return sources[index];
		}
		return null;
	}

	private void CompareSource_SourceContentChanged(object sender, EventArgs e)
	{
		BuildList();
	}

	private void CompareSource_SourceChanged(object sender, EventArgs e)
	{
		for (int i = 0; i < sources.Length; i++)
		{
			if (sources[i] == sender)
			{
				UpdateRelatedOnlineChannel(sources[i], ref relatedOnlineChannels[i]);
				break;
			}
		}
		updateColumnHeaders = true;
		BuildList();
	}

	private void Parameters_ParameterUpdateEvent(object sender, ResultEventArgs e)
	{
		UpdateParameterEditState(sender as Parameter, reset: false);
	}

	private void ListView_SelectedIndexChanged(object sender, EventArgs e)
	{
		OnSelectedParameterChanged();
	}

	private void GroupItem_ExpandCollapseItem(object sender, ListViewExGroupItemExpandEventArgs e)
	{
		ListViewExGroupItem val = (ListViewExGroupItem)((sender is ListViewExGroupItem) ? sender : null);
		expandedStates[((ListViewItem)(object)val).Name] = !val.Expanded;
	}

	private void SapiManager_ActiveChannelsListChanged(object sender, EventArgs e)
	{
		for (int i = 0; i < sources.Length; i++)
		{
			UpdateRelatedOnlineChannel(sources[i], ref relatedOnlineChannels[i]);
		}
	}

	public ListParameterCompare()
	{
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Expected O, but got Unknown
		InitializeComponent();
		stateImages.Images.Add(ImageIndex.Edited.ToString(), Resources.edited);
		stateImages.Images.Add(ImageIndex.Group.ToString(), Resources.group_open);
		stateImages.Images.Add(ImageIndex.GroupEdited.ToString(), Resources.group_edit);
		SourceCount = 2;
		Converter.GlobalInstance.UnitsSelectionChanged += GlobalInstance_UnitsSelectionChanged;
		SapiManager.GlobalInstance.ActiveChannelsListChanged += SapiManager_ActiveChannelsListChanged;
		SapiManager.GlobalInstance.AccessLevelFilterChanged += SapiManager_AccessLevelFilterChanged;
		((ListView)(object)listView).SelectedIndexChanged += ListView_SelectedIndexChanged;
		expandedStates.Add(Resources.GroupNameIdentification, value: true);
	}

	private void SapiManager_AccessLevelFilterChanged(object sender, EventArgs e)
	{
		BuildList();
	}

	private bool UpdateRelatedOnlineChannel(CompareSource source, ref Channel targetOnlineChannel)
	{
		Channel channel = (source.Loaded ? SapiManager.GlobalInstance.ActiveChannels.FirstOrDefault((Channel ac) => ac.Ecu == source.Parameters.Channel.Ecu) : null);
		if (channel != targetOnlineChannel)
		{
			if (targetOnlineChannel != null)
			{
				targetOnlineChannel.Parameters.ParameterUpdateEvent -= Parameters_ParameterUpdateEvent;
				foreach (Parameter parameter in targetOnlineChannel.Parameters)
				{
					UpdateParameterEditState(parameter, reset: true);
				}
			}
			targetOnlineChannel = channel;
			if (targetOnlineChannel != null)
			{
				targetOnlineChannel.Parameters.ParameterUpdateEvent += Parameters_ParameterUpdateEvent;
				foreach (Parameter parameter2 in targetOnlineChannel.Parameters)
				{
					UpdateParameterEditState(parameter2, reset: false);
				}
			}
			return true;
		}
		return false;
	}

	private void UpdateParameterEditState(Parameter parameter, bool reset)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
		if (parameter == null || !parameter.Channel.Parameters.HaveBeenReadFromEcu)
		{
			return;
		}
		ListViewExGroupItem val = (ListViewExGroupItem)((ListView)(object)listView).Items[parameter.GroupName];
		if (val != null)
		{
			ListViewExGroupItem val2 = val.Children.OfType<ListViewExGroupItem>().FirstOrDefault((ListViewExGroupItem i) => ((ListViewItem)(object)i).Name == parameter.Name);
			if (val2 != null)
			{
				((ListViewItem)(object)val2).StateImageIndex = ((reset || object.Equals(parameter.Value, parameter.OriginalValue)) ? (-1) : 0);
				((ListViewItem)(object)val).StateImageIndex = ((reset || !(parameter.GroupCodingString != parameter.OriginalGroupCodingString)) ? 1 : 2);
			}
		}
	}

	private void OnSelectedParameterChanged()
	{
		if (this.SelectedParameterChanged != null)
		{
			this.SelectedParameterChanged(this, new EventArgs());
		}
	}

	private void GlobalInstance_UnitsSelectionChanged(object sender, EventArgs e)
	{
		BuildList();
	}

	private void BuildList()
	{
		dirty = true;
		Invalidate();
	}

	protected override void OnPaint(PaintEventArgs e)
	{
		if (dirty)
		{
			dirty = false;
			Cursor.Current = Cursors.WaitCursor;
			InternalBuildList();
			Cursor.Current = Cursors.Default;
			updateColumnHeaders = false;
		}
		base.OnPaint(e);
	}

	private static bool ShowParameter(Parameter parameter)
	{
		if (parameter.Visible)
		{
			if (SapiManager.GlobalInstance.AccessLevelFilter.HasValue)
			{
				return parameter.ReadAccess <= SapiManager.GlobalInstance.AccessLevelFilter;
			}
			return true;
		}
		return false;
	}

	private void InternalBuildList()
	{
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Expected O, but got Unknown
		//IL_0514: Unknown result type (might be due to invalid IL or missing references)
		//IL_051b: Expected O, but got Unknown
		//IL_052c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0533: Expected O, but got Unknown
		//IL_05aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b1: Expected O, but got Unknown
		bool flag = false;
		int num = ((((ListView)(object)listView).TopItem != null) ? ((ListView)(object)listView).TopItem.Index : (-1));
		string selectedParameter = SelectedParameter;
		ListViewExGroupItem val = null;
		listView.LockSorting();
		listView.BeginUpdate();
		((ListView)(object)listView).Items.Clear();
		IEnumerable<EdexConfigurationInformation> enumerable = from s in sources
			where s.EdexConfigurationInformation != null
			select s.EdexConfigurationInformation;
		GroupCollection[] array = (GroupCollection[])(object)new GroupCollection[SourceCount];
		for (int num2 = 0; num2 < SourceCount; num2++)
		{
			array[num2] = new GroupCollection();
			if (sources[num2].Loaded)
			{
				if (updateColumnHeaders)
				{
					((ListView)(object)listView).Columns[num2 * 2 + 2].Text = string.Empty;
				}
				flag = true;
				foreach (DictionaryEntry identification in sources[num2].IdentificationList)
				{
					if (identification.Value != null)
					{
						array[num2].Add(Resources.GroupNameIdentification, (object)new GroupItem(identification, isIdentification: true));
					}
				}
				if (sources[num2].Parameters.Channel.ConnectionResource == null || sources[num2].Parameters.HaveBeenReadFromEcu)
				{
					EdexConfigurationInformation edexConfigurationInformation = sources[num2].EdexConfigurationInformation;
					object obj;
					if (edexConfigurationInformation == null)
					{
						obj = null;
					}
					else
					{
						DeviceInformation deviceInformation = edexConfigurationInformation.DeviceInformation;
						obj = ((deviceInformation != null) ? deviceInformation.EdexFactoryConfiguration : null);
					}
					EdexConfigurationInformation val2 = (EdexConfigurationInformation)obj;
					Channel channel = sources[num2].Parameters.Channel;
					foreach (IGrouping<string, Parameter> item in from p in sources[num2].Parameters
						where p.Value != null && ShowParameter(p)
						group p by p.GroupQualifier)
					{
						CodingInfoForCoding val3 = null;
						CodingChoice codingChoice = null;
						string text = channel.Parameters.GroupCodingStrings[item.Key];
						CodingParameterGroup codingParameterGroup = channel.CodingParameterGroups[item.Key];
						if (codingParameterGroup != null)
						{
							IEnumerable<EdexConfigurationInformation> obj2 = ((sources[num2].EdexConfigurationInformation != null) ? Enumerable.Repeat<EdexConfigurationInformation>(sources[num2].EdexConfigurationInformation, 1) : enumerable);
							object obj3;
							if (val2 == null)
							{
								obj3 = null;
							}
							else
							{
								EdexSettingItem aggregateSetting = val2.GetAggregateSetting(codingParameterGroup);
								obj3 = ((aggregateSetting != null) ? aggregateSetting.PartNumber : null);
							}
							val3 = ServerDataManager.GetCodingChoicesForParameterGroup(codingParameterGroup, text, obj2, (Part)obj3, (val2 != null) ? val2.GetEngineeringCorrectionFactors(channel.ParameterGroups[item.Key]) : null);
							codingChoice = ((val3 != null) ? val3.CodingChoices.FirstOrDefault((CodingChoice cc) => cc.Parameter == null) : null);
							array[num2].Add(item.First().GroupName, (object)new GroupItem(codingParameterGroup, codingChoice));
						}
						foreach (Parameter parameter in item)
						{
							CodingChoice codingChoice2 = null;
							bool isEngineeringCorrectionFactor = false;
							if (val3 != null)
							{
								codingChoice2 = val3.CodingChoices.FirstOrDefault((CodingChoice cc) => cc.Parameter != null && cc.Parameter.RelatedParameter == parameter);
								if (codingChoice2 == null && codingChoice != null)
								{
									if (parameter.IsValueEqualInCodingStrings(new Dump(text), val3.Coding))
									{
										codingChoice2 = codingChoice;
									}
									else if (val3.EngineeringCorrectionFactors.Contains(parameter.Qualifier, StringComparer.OrdinalIgnoreCase))
									{
										codingChoice2 = codingChoice;
										isEngineeringCorrectionFactor = true;
									}
								}
							}
							array[num2].Add(parameter.GroupName, (object)new GroupItem(parameter, codingChoice2, isEngineeringCorrectionFactor));
						}
					}
				}
				foreach (DictionaryEntry unknown in sources[num2].UnknownList)
				{
					if (unknown.Value != null)
					{
						array[num2].Add(Resources.GroupNameInvalidData, (object)new GroupItem(unknown, isIdentification: false));
					}
				}
			}
			else if (updateColumnHeaders)
			{
				((ListView)(object)listView).Columns[num2 * 2 + 2].Text = Resources.ValueNoSource;
			}
		}
		if (flag)
		{
			for (int num3 = 0; num3 < array.Length; num3++)
			{
				GroupCollection val4 = array[num3];
				foreach (Group item2 in val4)
				{
					bool flag2 = false;
					ListViewExGroupItem val5 = (ListViewExGroupItem)((ListView)(object)listView).Items[item2.Name];
					if (val5 == null)
					{
						flag2 = true;
						val5 = new ListViewExGroupItem(item2.Name);
						((ListViewItem)(object)val5).Name = item2.Name;
						((ListViewItem)(object)val5).StateImageIndex = 1;
						((ListViewItem)(object)val5).UseItemStyleForSubItems = false;
						if (((ListViewItem)(object)val5).Name == selectedParameter)
						{
							val = val5;
						}
					}
					while (item2.Items.Count > 0)
					{
						GroupItem groupItem = item2.Items[0] as GroupItem;
						if (groupItem.IsDefaultString)
						{
							PopulateItem(array, num3, item2.Name, val5, groupItem);
							continue;
						}
						ListViewExGroupItem val6 = new ListViewExGroupItem(groupItem.Name);
						((ListViewItem)(object)val6).Name = groupItem.Name;
						((ListViewItem)(object)val6).UseItemStyleForSubItems = false;
						Parameter parameter2 = ((relatedOnlineChannels[num3] != null) ? relatedOnlineChannels[num3].Parameters[groupItem.Qualifier] : null);
						if (parameter2 != null && !object.Equals(parameter2.Value, parameter2.OriginalValue))
						{
							((ListViewItem)(object)val6).StateImageIndex = 0;
							((ListViewItem)(object)val5).StateImageIndex = 2;
						}
						if (PopulateItem(array, num3, item2.Name, val6, groupItem))
						{
							val5.Add(val6);
							if (string.Join("/", item2.Name, groupItem.Name) == selectedParameter)
							{
								val = val6;
							}
						}
					}
					if (flag2 && val5.Children.Count > 0)
					{
						((ListView)(object)listView).Items.Add((ListViewItem)(object)val5);
						if (expandedStates.ContainsKey(((ListViewItem)(object)val5).Name) && expandedStates[((ListViewItem)(object)val5).Name])
						{
							val5.Expand();
						}
						val5.ExpandCollapseItem += GroupItem_ExpandCollapseItem;
					}
				}
			}
			if (updateColumnHeaders)
			{
				columnParameter.Width = columnAutoWidth;
				int num4 = ((Control)(object)listView).ClientSize.Width - columnParameter.Width - columnUnits.Width - SystemInformation.VerticalScrollBarWidth;
				int num5 = SourceCount + sources.Count((CompareSource s) => s.Parameters != null && SapiExtensions.IsDataSourceEdex(s.Parameters.Channel.Ecu));
				int num6 = num4 / num5;
				for (int num7 = 0; num7 < SourceCount; num7++)
				{
					((ListView)(object)listView).Columns[num7 * 2 + 1].Width = ((sources[num7].Parameters != null && SapiExtensions.IsDataSourceEdex(sources[num7].Parameters.Channel.Ecu)) ? num6 : 0);
					((ListView)(object)listView).Columns[num7 * 2 + 2].Width = num6;
				}
			}
		}
		try
		{
			if (val != null)
			{
				((ListViewItem)(object)val).Selected = true;
			}
			if (num != -1 && ((ListView)(object)listView).Items.Count > num)
			{
				((ListView)(object)listView).TopItem = ((ListView)(object)listView).Items[num];
			}
		}
		catch (NullReferenceException)
		{
		}
		listView.EndUpdate();
		listView.UnlockSorting();
		OnSelectedParameterChanged();
	}

	private static GroupItem FindItem(Group group, string qualifier)
	{
		foreach (GroupItem item in group.Items)
		{
			if (string.Equals(item.Qualifier, qualifier, StringComparison.OrdinalIgnoreCase))
			{
				return item;
			}
		}
		return null;
	}

	private bool PopulateItem(GroupCollection[] groupCollections, int index, string groupName, ListViewExGroupItem item, GroupItem sourceParameter)
	{
		bool flag = index != 0 && sources[0].Loaded;
		bool flag2 = true;
		if (sourceParameter.ReadOnly)
		{
			((ListViewItem)(object)item).ForeColor = SystemColors.GrayText;
		}
		Conversion conversion = Converter.GlobalInstance.GetConversion(sourceParameter.Units);
		GroupItem[] array = new GroupItem[SourceCount];
		for (int i = 0; i < SourceCount; i++)
		{
			GroupCollection val = groupCollections[i];
			Group val2 = ((IEnumerable<Group>)val).FirstOrDefault((Group x) => string.Equals(x.Name, groupName, StringComparison.OrdinalIgnoreCase)) ?? val[groupName];
			GroupItem groupItem = FindItem(val2, sourceParameter.Qualifier);
			if (groupItem != null)
			{
				array[i] = groupItem;
				val2.Items.Remove(groupItem);
				ListViewItem.ListViewSubItem listViewSubItem = ((ListViewItem)(object)item).SubItems.Add((!groupItem.IsPartNumberInherited) ? groupItem.PartNumber : (groupItem.IsEngineeringCorrectionFactor ? Resources.Message_ParameterValueFromParentEngineeringCorrectionFactor : Resources.Message_ParameterValueFromParent));
				ListViewItem.ListViewSubItem listViewSubItem2 = ((ListViewItem)(object)item).SubItems.Add((conversion != null) ? Converter.ConvertToString((IFormatProvider)CultureInfo.CurrentCulture, groupItem.Value, conversion, groupItem.Precision) : groupItem.Value.ToString());
				if (groupItem.ReadOnly)
				{
					Color foreColor = (listViewSubItem2.ForeColor = SystemColors.GrayText);
					listViewSubItem.ForeColor = foreColor;
				}
				if (i != index)
				{
					flag2 = false;
				}
			}
			else
			{
				((ListViewItem)(object)item).SubItems.Add(string.Empty);
				((ListViewItem)(object)item).SubItems.Add(string.Empty);
			}
		}
		ListViewItem.ListViewSubItem listViewSubItem3 = ((ListViewItem)(object)item).SubItems.Add(Converter.Translate((conversion != null) ? conversion.OutputUnit : sourceParameter.Units));
		if (sourceParameter.ReadOnly)
		{
			listViewSubItem3.ForeColor = SystemColors.GrayText;
		}
		for (int num = index; num < SourceCount; num++)
		{
			if (flag)
			{
				break;
			}
			GroupItem groupItem2 = array[num];
			if (groupItem2 != null)
			{
				for (int num2 = num + 1; num2 < SourceCount; num2++)
				{
					if (flag)
					{
						break;
					}
					GroupItem groupItem3 = array[num2];
					if (groupItem3 != null && !object.Equals(groupItem2.Value, groupItem3.Value))
					{
						flag = true;
					}
				}
			}
			else if (sources[num].Loaded)
			{
				flag = true;
			}
		}
		if (flag || flag2)
		{
			for (int num3 = index; num3 < SourceCount; num3++)
			{
				GroupItem groupItem4 = array[num3];
				if (groupItem4 == null)
				{
					continue;
				}
				ListViewItem.ListViewSubItem listViewSubItem4 = ((ListViewItem)(object)item).SubItems[2 * num3 + 2];
				listViewSubItem4.BackColor = sources[num3].Color;
				listViewSubItem4.Tag = ChangedStatus.Changed;
				((ListViewItem)(object)item).SubItems[2 * num3 + 1].BackColor = sources[num3].Color;
				if (groupItem4.ModifyHeader && updateColumnHeaders)
				{
					ColumnHeader columnHeader = ((ListView)(object)listView).Columns[num3 * 2 + 2];
					columnHeader.Text = string.Concat(columnHeader.Text, groupItem4.Value, " ");
				}
				for (int num4 = num3 + 1; num4 < SourceCount; num4++)
				{
					GroupItem groupItem5 = array[num4];
					if (groupItem5 != null && object.Equals(groupItem4.Value, groupItem5.Value))
					{
						ListViewItem.ListViewSubItem listViewSubItem5 = ((ListViewItem)(object)item).SubItems[num4 * 2 + 2];
						listViewSubItem5.BackColor = sources[num3].Color;
						listViewSubItem5.Tag = ChangedStatus.Changed;
						((ListViewItem)(object)item).SubItems[2 * num4 + 1].BackColor = sources[num3].Color;
						if (groupItem4.ModifyHeader && updateColumnHeaders)
						{
							ColumnHeader columnHeader2 = ((ListView)(object)listView).Columns[num4 * 2 + 2];
							columnHeader2.Text = string.Concat(columnHeader2.Text, groupItem4.Value, " ");
						}
						array[num4] = null;
					}
				}
			}
		}
		if (flag || flag2 || !showDifferencesOnly || sourceParameter.AlwaysShow)
		{
			if (!flag2)
			{
				return true;
			}
			ListViewItem.ListViewSubItem listViewSubItem6 = ((ListViewItem)(object)item).SubItems[0];
			listViewSubItem6.BackColor = sources[index].Color;
			listViewSubItem6.Tag = ChangedStatus.Changed;
			if (sources[index].Parameters.Channel.ConnectionResource == null || !showOfflineItemsOnly)
			{
				return true;
			}
		}
		return false;
	}

	public bool Search(string searchText, bool caseSensitive, FindMode direction)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		return ((ISearchable)listView).Search(searchText, caseSensitive, direction);
	}

	public void Undo()
	{
	}

	public void Copy()
	{
		editSupport.SetTarget((object)base.ActiveControl);
		editSupport.Copy();
	}

	public void Delete()
	{
	}

	public void Paste()
	{
	}

	public void Cut()
	{
	}

	public void SelectAll()
	{
		editSupport.SetTarget((object)base.ActiveControl);
		editSupport.SelectAll();
	}

	public string ToHtml()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Expected O, but got Unknown
		StringBuilder stringBuilder = new StringBuilder();
		if (CanProvideHtml)
		{
			XmlWriter xmlWriter = PrintHelper.CreateWriter(stringBuilder);
			xmlWriter.WriteStartElement("table");
			foreach (ListViewExGroupItem item in ((ListView)(object)listView).Items)
			{
				ListViewExGroupItem val = item;
				xmlWriter.WriteStartElement("tr");
				xmlWriter.WriteAttributeString("class", val.HasChildren ? "group" : "standard");
				for (int i = 0; i < ((ListViewItem)(object)val).SubItems.Count; i++)
				{
					xmlWriter.WriteStartElement("td");
					xmlWriter.WriteAttributeString("bgcolor", ColorTranslator.ToHtml(((ListViewItem)(object)val).SubItems[i].BackColor));
					if (!showDifferencesOnly && ((ListViewItem)(object)val).SubItems[i].Tag != null && (ChangedStatus)((ListViewItem)(object)val).SubItems[i].Tag == ChangedStatus.Changed)
					{
						xmlWriter.WriteString("[*]");
					}
					xmlWriter.WriteString(((ListViewItem)(object)val).SubItems[i].Text);
					xmlWriter.WriteFullEndElement();
					if (i == 0)
					{
						xmlWriter.WriteStartElement("td");
						xmlWriter.WriteRaw("&nbsp;");
						xmlWriter.WriteFullEndElement();
					}
				}
				xmlWriter.WriteFullEndElement();
			}
			xmlWriter.WriteFullEndElement();
			xmlWriter.Close();
		}
		return stringBuilder.ToString();
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		this.components = new System.ComponentModel.Container();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DetroitDiesel.Windows.Forms.Diagnostics.Panels.Parameters.ListParameterCompare));
		this.listView = new ListViewEx();
		this.columnParameter = new System.Windows.Forms.ColumnHeader();
		this.columnUnits = new System.Windows.Forms.ColumnHeader();
		this.stateImages = new System.Windows.Forms.ImageList(this.components);
		base.SuspendLayout();
		((System.Windows.Forms.ListView)(object)this.listView).Columns.AddRange(new System.Windows.Forms.ColumnHeader[2] { this.columnParameter, this.columnUnits });
		resources.ApplyResources(this.listView, "listView");
		this.listView.EditableColumn = -1;
		this.listView.GridLines = true;
		((System.Windows.Forms.Control)(object)this.listView).Name = "listView";
		this.listView.ShowGlyphs = (GlyphBehavior)2;
		this.listView.ShowStateImages = (ImageBehavior)2;
		((System.Windows.Forms.ListView)(object)this.listView).StateImageList = this.stateImages;
		((System.Windows.Forms.ListView)(object)this.listView).UseCompatibleStateImageBehavior = false;
		resources.ApplyResources(this.columnParameter, "columnParameter");
		resources.ApplyResources(this.columnUnits, "columnUnits");
		this.stateImages.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
		resources.ApplyResources(this.stateImages, "stateImages");
		this.stateImages.TransparentColor = System.Drawing.Color.Transparent;
		resources.ApplyResources(this, "$this");
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add((System.Windows.Forms.Control)(object)this.listView);
		base.Name = "ListParameterCompare";
		base.ResumeLayout(false);
	}
}
