using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Interfaces;
using DetroitDiesel.Windows.Forms;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using DetroitDiesel.Windows.Forms.Themed;
using SapiLayer1;

namespace DetroitDiesel.FaultCodeTabs.General.Test_Results__EPA10_.panel;

public class UserPanel : CustomPanel, IRefreshable, ISearchable, ISupportEdit
{
	private class UpdateItem
	{
		private DataItem value;

		private DataItem min;

		private DataItem max;

		private Timer timer;

		private ListViewExGroupItem item;

		public ListViewExGroupItem ListItem
		{
			set
			{
				item = value;
			}
		}

		public UpdateItem(DataItem value, DataItem min, DataItem max)
		{
			this.value = value;
			this.min = min;
			this.max = max;
			this.value.UpdateEvent += dataItem_UpdateEvent;
			this.min.UpdateEvent += dataItem_UpdateEvent;
			this.max.UpdateEvent += dataItem_UpdateEvent;
		}

		private void UpdateListSubItem(int index, string text, bool color)
		{
			if (item == null || ((ListViewItem)(object)item).SubItems.Count <= index)
			{
				return;
			}
			string text2 = ((text != null) ? text : string.Empty);
			if (!text2.Equals(((ListViewItem)(object)item).SubItems[index].Text))
			{
				if (color && ((ListViewItem)(object)item).SubItems[index].Text != string.Empty)
				{
					((ListViewItem)(object)item).SubItems[index].BackColor = ThemeProvider.GlobalInstance.ActiveTheme.GetStateBackColor((ValueState)1);
					((ListViewItem)(object)item).SubItems[index].ForeColor = ThemeProvider.GlobalInstance.ActiveTheme.GetStateForeColor((ValueState)1);
				}
				((ListViewItem)(object)item).SubItems[index].Text = text2;
			}
		}

		public void UpdateValueMinMax()
		{
			if (value != null && min != null && max != null && item != null)
			{
				UpdateListSubItem(3, value.ValueAsString(value.Value), color: true);
				UpdateListSubItem(4, min.ValueAsString(min.Value), color: true);
				UpdateListSubItem(5, max.ValueAsString(max.Value), color: true);
				UpdateListSubItem(6, value.Units, color: false);
			}
		}

		private void dataItem_UpdateEvent(object sender, ResultEventArgs e)
		{
			UpdateValueMinMax();
			if (timer == null)
			{
				timer = new Timer();
				timer.Tick += updateTimer_Tick;
			}
			else
			{
				timer.Stop();
			}
			timer.Interval = 30000;
			timer.Start();
		}

		private void updateTimer_Tick(object sender, EventArgs e)
		{
			if (item != null && ((ListViewItem)(object)item).SubItems.Count > 5)
			{
				ListViewItem.ListViewSubItem listViewSubItem = ((ListViewItem)(object)item).SubItems[3];
				ListViewItem.ListViewSubItem listViewSubItem2 = ((ListViewItem)(object)item).SubItems[4];
				Color color = (((ListViewItem)(object)item).SubItems[5].BackColor = SystemColors.Window);
				color = (listViewSubItem2.BackColor = color);
				listViewSubItem.BackColor = color;
				ListViewItem.ListViewSubItem listViewSubItem3 = ((ListViewItem)(object)item).SubItems[3];
				ListViewItem.ListViewSubItem listViewSubItem4 = ((ListViewItem)(object)item).SubItems[4];
				color = (((ListViewItem)(object)item).SubItems[5].ForeColor = SystemColors.WindowText);
				color = (listViewSubItem4.ForeColor = color);
				listViewSubItem3.ForeColor = color;
			}
			RemoveTimer();
		}

		public void RemoveTimer()
		{
			if (timer != null)
			{
				timer.Stop();
				timer.Tick -= updateTimer_Tick;
				timer.Dispose();
				timer = null;
			}
		}

		public void MarkForRefresh()
		{
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			if (value != null && min != null && max != null)
			{
				EcuInfoCollection ecuInfos = value.Channel.EcuInfos;
				Qualifier qualifier = value.Qualifier;
				ecuInfos[((Qualifier)(ref qualifier)).Name].Marked = true;
				EcuInfoCollection ecuInfos2 = min.Channel.EcuInfos;
				qualifier = min.Qualifier;
				ecuInfos2[((Qualifier)(ref qualifier)).Name].Marked = true;
				EcuInfoCollection ecuInfos3 = max.Channel.EcuInfos;
				qualifier = max.Qualifier;
				ecuInfos3[((Qualifier)(ref qualifier)).Name].Marked = true;
			}
		}
	}

	private const string TestValueQualifierFormat = "DT_STO_ID_Scaled_Test_Results_Value_{0}_Test_Value";

	private const string TestMinQualifierFormat = "DT_STO_ID_Scaled_Test_Results_Value_{0}_Min_Test_Limit";

	private const string TestMaxQualifierFormat = "DT_STO_ID_Scaled_Test_Results_Value_{0}_Max_Test_Limit";

	private const string UnitAndScalingIDQualifierFormat = "DT_STO_ID_Scaled_Test_Results_Value_{0}_Unit_And_Scaling_ID";

	private const string SPNHighByteFMIQualifierFormat = "DT_STO_ID_Scaled_Test_Results_Fault_Value_{0}_SPN_High_Byte_FMI";

	private const string SPNMidByteQualifierFormat = "DT_STO_ID_Scaled_Test_Results_Fault_Value_{0}_SPN_Mid_Byte";

	private const string SPNLowByteQualifierFormat = "DT_STO_ID_Scaled_Test_Results_Fault_Value_{0}_SPN_Low_Byte";

	private const int highlightInterval = 30000;

	private List<Channel> waitingToReadComplete = new List<Channel>();

	private Dictionary<Channel, Dictionary<object, UpdateItem>> updateMap = new Dictionary<Channel, Dictionary<object, UpdateItem>>();

	private ImageList imageList;

	private Font boldFont;

	private ChannelBaseCollection activeChannels;

	private ListViewEx listView;

	private ColumnHeader columnHeader1;

	private ColumnHeader columnHeader2;

	private ColumnHeader columnHeader3;

	private ColumnHeader columnHeader4;

	private ColumnHeader columnHeader6;

	private ColumnHeader columnHeader7;

	private ColumnHeader columnHeader5;

	public bool CanRefreshView
	{
		get
		{
			if (SapiManager.GlobalInstance.Online)
			{
				bool result = true;
				foreach (Channel activeChannel in activeChannels)
				{
					if (activeChannel.Online && activeChannel.CommunicationsState != CommunicationsState.Online)
					{
						result = false;
					}
				}
				return result;
			}
			return false;
		}
	}

	public bool CanSearch => listView.CanSearch;

	public override bool CanUndo => listView.CanUndo;

	public override bool CanCopy => listView.CanCopy;

	public override bool CanDelete => listView.CanDelete;

	public override bool CanPaste => listView.CanPaste;

	public override bool CanCut => listView.CanCut;

	public override bool CanSelectAll => listView.CanSelectAll;

	public override bool CanProvideHtml => ((ListView)(object)listView).Items.Count > 0;

	protected static string EcuInfoValue(Channel channel, string qualifier)
	{
		EcuInfo ecuInfo = channel.EcuInfos[qualifier];
		if (ecuInfo != null && ecuInfo.Value != null)
		{
			return ecuInfo.Value;
		}
		return string.Empty;
	}

	public UserPanel()
	{
		InitializeComponent();
		imageList = new ImageList();
		imageList.ColorDepth = ColorDepth.Depth32Bit;
		imageList.TransparentColor = Color.Transparent;
		imageList.Images.Add("group", FaultListIcons.Group);
		listView.ShowStateImages = (ImageBehavior)2;
		((ListView)(object)listView).StateImageList = imageList;
		((ListView)(object)listView).UseCompatibleStateImageBehavior = false;
		boldFont = new Font(((Control)(object)this).Font.FontFamily, ((Control)(object)this).Font.Size, FontStyle.Bold);
		SapiManager.GlobalInstance.ActiveChannelsChanged += OnActiveChannelsChanged;
		UpdateActiveChannels();
	}

	private void OnActiveChannelsChanged(object sender, EventArgs e)
	{
		UpdateActiveChannels();
	}

	private void UpdateActiveChannels()
	{
		if (activeChannels != null)
		{
			activeChannels.ConnectCompleteEvent -= OnConnectCompleteEvent;
			activeChannels.DisconnectCompleteEvent -= OnDisconnectCompleteEvent;
			foreach (Channel activeChannel in activeChannels)
			{
				RemoveChannel(activeChannel);
			}
		}
		activeChannels = SapiManager.GlobalInstance.ActiveChannels;
		if (activeChannels == null)
		{
			return;
		}
		activeChannels.ConnectCompleteEvent += OnConnectCompleteEvent;
		activeChannels.DisconnectCompleteEvent += OnDisconnectCompleteEvent;
		foreach (Channel activeChannel2 in activeChannels)
		{
			AddChannel(activeChannel2);
		}
	}

	private void OnConnectCompleteEvent(object sender, ResultEventArgs e)
	{
		if (e.Succeeded)
		{
			AddChannel((Channel)sender);
		}
	}

	private void OnDisconnectCompleteEvent(object sender, EventArgs e)
	{
		Channel channel = sender as Channel;
		RemoveChannel(channel);
	}

	private void AddChannel(Channel channel)
	{
		if (channel.FaultCodes.SupportsSnapshot)
		{
			channel.CommunicationsStateUpdateEvent += OnCommunicationsStateUpdateEvent;
			channel.FaultCodes.FaultCodesUpdateEvent += OnFaultCodeCollectionUpdateEvent;
			if (channel.CommunicationsState == CommunicationsState.Online || channel.LogFile != null)
			{
				AddChannelItems(channel);
			}
			else
			{
				waitingToReadComplete.Add(channel);
			}
		}
	}

	private bool TryParseTestIndex(string qualifier, out int index)
	{
		index = -1;
		string text = "DT_STO_ID_Scaled_Test_Results_Value_{0}_Test_Value".Substring(0, "DT_STO_ID_Scaled_Test_Results_Value_{0}_Test_Value".IndexOf("{"));
		string text2 = "DT_STO_ID_Scaled_Test_Results_Value_{0}_Test_Value".Substring("DT_STO_ID_Scaled_Test_Results_Value_{0}_Test_Value".IndexOf("}") + 1);
		if (qualifier.StartsWith(text) && qualifier.EndsWith(text2))
		{
			string value = qualifier.Substring(text.Length, qualifier.Length - text.Length - text2.Length);
			index = Convert.ToInt32(value);
			return true;
		}
		return false;
	}

	private IEnumerable<int> GetValidTestIndexes(Channel channel)
	{
		foreach (EcuInfo ecuInfo in channel.EcuInfos)
		{
			if (TryParseTestIndex(ecuInfo.Qualifier, out var result))
			{
				yield return result;
			}
		}
	}

	private List<FaultCode> GetItemList(Channel channel)
	{
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		updateMap[channel] = new Dictionary<object, UpdateItem>();
		List<FaultCode> list = new List<FaultCode>();
		foreach (int validTestIndex in GetValidTestIndexes(channel))
		{
			string code = EcuInfoValue(channel, $"DT_STO_ID_Scaled_Test_Results_Fault_Value_{validTestIndex}_SPN_Low_Byte") + EcuInfoValue(channel, $"DT_STO_ID_Scaled_Test_Results_Fault_Value_{validTestIndex}_SPN_Mid_Byte") + EcuInfoValue(channel, $"DT_STO_ID_Scaled_Test_Results_Fault_Value_{validTestIndex}_SPN_High_Byte_FMI");
			FaultCode faultCode = channel.FaultCodes[code];
			if (faultCode != null)
			{
				list.Add(faultCode);
				updateMap[channel].Add(faultCode, new UpdateItem(DataItem.Create(new Qualifier((QualifierTypes)8, channel.Ecu.Name, $"DT_STO_ID_Scaled_Test_Results_Value_{validTestIndex}_Test_Value"), (IEnumerable<Channel>)activeChannels), DataItem.Create(new Qualifier((QualifierTypes)8, channel.Ecu.Name, $"DT_STO_ID_Scaled_Test_Results_Value_{validTestIndex}_Min_Test_Limit"), (IEnumerable<Channel>)activeChannels), DataItem.Create(new Qualifier((QualifierTypes)8, channel.Ecu.Name, $"DT_STO_ID_Scaled_Test_Results_Value_{validTestIndex}_Max_Test_Limit"), (IEnumerable<Channel>)activeChannels)));
			}
		}
		return list;
	}

	private ListViewExGroupItem AddItem(string name, object tag, string number, string mode, int stateImage, bool error)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Expected O, but got Unknown
		ListViewExGroupItem val = new ListViewExGroupItem(name);
		((ListViewItem)(object)val).SubItems.Add(number);
		((ListViewItem)(object)val).SubItems.Add(mode);
		((ListViewItem)(object)val).SubItems.Add(string.Empty);
		((ListViewItem)(object)val).SubItems.Add(string.Empty);
		((ListViewItem)(object)val).SubItems.Add(string.Empty);
		((ListViewItem)(object)val).SubItems.Add(string.Empty);
		((ListViewItem)(object)val).UseItemStyleForSubItems = false;
		((ListViewItem)(object)val).Tag = tag;
		if (stateImage != -1)
		{
			((ListViewItem)(object)val).StateImageIndex = stateImage;
		}
		if (error)
		{
			((ListViewItem)(object)val).ForeColor = Color.Red;
		}
		else
		{
			((ListViewItem)(object)val).ForeColor = SystemColors.ControlText;
		}
		return val;
	}

	private bool IsCodeActionable(FaultCode faultCode)
	{
		FaultCodeIncident currentByFunction = faultCode.FaultCodeIncidents.GetCurrentByFunction(ReadFunctions.NonPermanent);
		return currentByFunction != null && SapiManager.IsFaultActionable(currentByFunction);
	}

	private void AddChannelItems(Channel channel)
	{
		List<FaultCode> itemList = GetItemList(channel);
		if (itemList.Count <= 0)
		{
			return;
		}
		listView.BeginUpdate();
		listView.LockSorting();
		ListViewExGroupItem val = AddItem(channel.Ecu.Name, channel, string.Empty, string.Empty, 0, error: false);
		((ListViewItem)(object)val).Font = boldFont;
		((ListView)(object)listView).Items.Add((ListViewItem)(object)val);
		foreach (FaultCode item in itemList)
		{
			ListViewExGroupItem val2 = AddItem(item.Text, item, item.Number, item.Mode, -1, IsCodeActionable(item));
			UpdateItem updateItem = GetUpdateItem(channel, item);
			updateItem.ListItem = val2;
			updateItem.UpdateValueMinMax();
			val.Add(val2);
		}
		listView.UnlockSorting();
		listView.EndUpdate();
	}

	private ListViewExGroupItem FindItemByTag(object itemSought)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected O, but got Unknown
		ListViewExGroupItem result = null;
		foreach (ListViewExGroupItem item in ((ListView)(object)listView).Items)
		{
			ListViewExGroupItem val = item;
			if (((ListViewItem)(object)val).Tag == itemSought)
			{
				result = val;
				break;
			}
		}
		return result;
	}

	private void RemoveChannel(Channel channel)
	{
		if (!channel.FaultCodes.SupportsSnapshot)
		{
			return;
		}
		channel.CommunicationsStateUpdateEvent -= OnCommunicationsStateUpdateEvent;
		channel.FaultCodes.FaultCodesUpdateEvent -= OnFaultCodeCollectionUpdateEvent;
		ListViewExGroupItem val = FindItemByTag(channel);
		if (val != null)
		{
			val.RemoveAll();
			((ListViewItem)(object)val).Remove();
		}
		if (!updateMap.ContainsKey(channel))
		{
			return;
		}
		Dictionary<object, UpdateItem> dictionary = updateMap[channel];
		foreach (object key in dictionary.Keys)
		{
			UpdateItem updateItem = dictionary[key];
			updateItem.RemoveTimer();
		}
		updateMap.Remove(channel);
	}

	private void OnCommunicationsStateUpdateEvent(object sender, CommunicationsStateEventArgs e)
	{
		Channel channel = sender as Channel;
		if (channel.CommunicationsState == CommunicationsState.Online && waitingToReadComplete.Contains(channel))
		{
			AddChannelItems(channel);
			waitingToReadComplete.Remove(channel);
		}
	}

	private UpdateItem GetUpdateItem(Channel channel, FaultCode tag)
	{
		if (updateMap.ContainsKey(channel))
		{
			Dictionary<object, UpdateItem> dictionary = updateMap[channel];
			if (dictionary.ContainsKey(tag))
			{
				return dictionary[tag];
			}
		}
		return null;
	}

	private void UpdateColor(ListViewExGroupItem item, bool actionable)
	{
		Color color = (actionable ? Color.Red : SystemColors.WindowText);
		if (((ListViewItem)(object)item).ForeColor != color)
		{
			((ListViewItem)(object)item).ForeColor = color;
		}
	}

	private void OnFaultCodeCollectionUpdateEvent(object sender, ResultEventArgs e)
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Expected O, but got Unknown
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Expected O, but got Unknown
		FaultCodeCollection faultCodeCollection = sender as FaultCodeCollection;
		ListViewExGroupItem val = FindItemByTag(faultCodeCollection.Channel);
		if (val == null)
		{
			return;
		}
		listView.BeginUpdate();
		foreach (ListViewExGroupItem child in val.Children)
		{
			ListViewExGroupItem val2 = child;
			foreach (ListViewExGroupItem child2 in val2.Children)
			{
				ListViewExGroupItem val3 = child2;
				if (((ListViewItem)(object)val3).Tag is FaultCode faultCode)
				{
					UpdateColor(val3, IsCodeActionable(faultCode));
				}
			}
		}
		listView.EndUpdate();
	}

	public void RefreshView()
	{
		Sapi sapi = Sapi.GetSapi();
		if (!SapiManager.GlobalInstance.Online)
		{
			return;
		}
		foreach (Channel activeChannel in activeChannels)
		{
			if (!activeChannel.Online || activeChannel.CommunicationsState != CommunicationsState.Online)
			{
				continue;
			}
			foreach (EcuInfo ecuInfo in activeChannel.EcuInfos)
			{
				ecuInfo.Marked = false;
			}
			if (updateMap.ContainsKey(activeChannel))
			{
				Dictionary<object, UpdateItem> dictionary = updateMap[activeChannel];
				foreach (object key in dictionary.Keys)
				{
					UpdateItem updateItem = dictionary[key];
					updateItem.MarkForRefresh();
				}
			}
			activeChannel.EcuInfos.EcuInfosReadCompleteEvent += ecuInfos_ReadCompleteEvent;
			activeChannel.EcuInfos.Read(synchronous: false);
		}
	}

	private void ecuInfos_ReadCompleteEvent(object sender, ResultEventArgs e)
	{
		EcuInfoCollection ecuInfoCollection = sender as EcuInfoCollection;
		foreach (EcuInfo item in ecuInfoCollection)
		{
			item.Marked = true;
		}
		ecuInfoCollection.EcuInfosReadCompleteEvent -= ecuInfos_ReadCompleteEvent;
	}

	private void AddChildrenToHtml(ListViewExGroupItem item, XmlWriter xmlWriter)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected O, but got Unknown
		foreach (ListViewExGroupItem child in item.Children)
		{
			ListViewExGroupItem val = child;
			if (val.HasChildren)
			{
				xmlWriter.WriteStartElement("tr");
				xmlWriter.WriteStartElement("td");
				xmlWriter.WriteStartAttribute("class");
				xmlWriter.WriteString("group");
				xmlWriter.WriteEndAttribute();
				xmlWriter.WriteString(((ListViewItem)(object)val).Text);
				xmlWriter.WriteFullEndElement();
				xmlWriter.WriteStartElement("td");
				xmlWriter.WriteStartAttribute("class");
				xmlWriter.WriteString("group");
				xmlWriter.WriteEndAttribute();
				xmlWriter.WriteFullEndElement();
				xmlWriter.WriteFullEndElement();
				AddChildrenToHtml(val, xmlWriter);
			}
			else
			{
				xmlWriter.WriteStartElement("tr");
				xmlWriter.WriteAttributeString("class", Resources.Message_Standard);
				for (int i = 0; i <= ((ListView)(object)listView).Columns.Count - 1; i++)
				{
					xmlWriter.WriteStartElement("td");
					xmlWriter.WriteAttributeString("bgcolor", ColorTranslator.ToHtml(((ListViewItem)(object)val).SubItems[i].BackColor));
					xmlWriter.WriteString(((ListViewItem)(object)val).SubItems[i].Text);
					xmlWriter.WriteFullEndElement();
				}
				xmlWriter.WriteFullEndElement();
			}
		}
	}

	public bool Search(string searchText, bool caseSensitive, FindMode direction)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		return listView.Search(searchText, caseSensitive, direction);
	}

	public override void Undo()
	{
		listView.Undo();
	}

	public override void Copy()
	{
		listView.Copy();
	}

	public override void Delete()
	{
		listView.Delete();
	}

	public override void Paste()
	{
		listView.Paste();
	}

	public override void Cut()
	{
		listView.Cut();
	}

	public override void SelectAll()
	{
		listView.SelectAll();
	}

	public override string ToHtml()
	{
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Expected O, but got Unknown
		StringBuilder stringBuilder = new StringBuilder();
		if (((CustomPanel)this).CanProvideHtml)
		{
			XmlWriter xmlWriter = PrintHelper.CreateWriter(stringBuilder);
			xmlWriter.WriteStartElement("div");
			xmlWriter.WriteStartElement("table");
			xmlWriter.WriteStartElement("thead");
			xmlWriter.WriteElementString("h1", Resources.Message_TestResultsData);
			xmlWriter.WriteEndElement();
			xmlWriter.WriteStartElement("tbody");
			foreach (ListViewExGroupItem item in ((ListView)(object)listView).Items)
			{
				ListViewExGroupItem val = item;
				if (val.Level == 0)
				{
					xmlWriter.WriteStartElement("div");
					xmlWriter.WriteStartAttribute("id");
					xmlWriter.WriteString(((ListViewItem)(object)val).SubItems[0].Text);
					xmlWriter.WriteEndAttribute();
					xmlWriter.WriteStartAttribute("name");
					xmlWriter.WriteString(((ListViewItem)(object)val).SubItems[0].Text);
					xmlWriter.WriteEndAttribute();
					xmlWriter.WriteStartElement("table");
					xmlWriter.WriteStartElement("tr");
					xmlWriter.WriteStartElement("td");
					xmlWriter.WriteStartAttribute("class");
					xmlWriter.WriteString("ecu");
					xmlWriter.WriteEndAttribute();
					xmlWriter.WriteString(((ListViewItem)(object)val).SubItems[0].Text);
					xmlWriter.WriteFullEndElement();
					xmlWriter.WriteStartElement("td");
					xmlWriter.WriteStartAttribute("valign");
					xmlWriter.WriteString("bottom");
					xmlWriter.WriteEndAttribute();
					xmlWriter.WriteStartAttribute("class");
					xmlWriter.WriteString("standard");
					xmlWriter.WriteEndAttribute();
					xmlWriter.WriteFullEndElement();
					xmlWriter.WriteFullEndElement();
					xmlWriter.WriteFullEndElement();
					xmlWriter.WriteStartElement("div");
					xmlWriter.WriteStartAttribute("class");
					xmlWriter.WriteString("gradientline0");
					xmlWriter.WriteEndAttribute();
					xmlWriter.WriteFullEndElement();
					xmlWriter.WriteStartElement("table");
					xmlWriter.WriteStartElement("tr");
					for (int i = 0; i <= ((ListView)(object)listView).Columns.Count - 1; i++)
					{
						xmlWriter.WriteStartElement("th");
						xmlWriter.WriteStartAttribute("align");
						xmlWriter.WriteString("left");
						xmlWriter.WriteEndAttribute();
						xmlWriter.WriteStartElement("font");
						xmlWriter.WriteStartAttribute("face");
						xmlWriter.WriteString("Arial");
						xmlWriter.WriteEndAttribute();
						xmlWriter.WriteString(((ListView)(object)listView).Columns[i].Text);
						xmlWriter.WriteFullEndElement();
					}
					xmlWriter.WriteFullEndElement();
					AddChildrenToHtml(val, xmlWriter);
					xmlWriter.WriteFullEndElement();
					xmlWriter.WriteFullEndElement();
				}
			}
			xmlWriter.Close();
		}
		return stringBuilder.ToString();
	}

	private void InitializeComponent()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		listView = new ListViewEx();
		columnHeader1 = new ColumnHeader();
		columnHeader2 = new ColumnHeader();
		columnHeader3 = new ColumnHeader();
		columnHeader4 = new ColumnHeader();
		columnHeader5 = new ColumnHeader();
		columnHeader6 = new ColumnHeader();
		columnHeader7 = new ColumnHeader();
		((ISupportInitialize)listView).BeginInit();
		((Control)this).SuspendLayout();
		listView.CanDelete = false;
		((ListView)(object)listView).Columns.AddRange(new ColumnHeader[7] { columnHeader1, columnHeader2, columnHeader3, columnHeader4, columnHeader5, columnHeader6, columnHeader7 });
		componentResourceManager.ApplyResources(listView, "listView");
		listView.EditableColumn = -1;
		listView.GridLines = true;
		((Control)(object)listView).Name = "listView";
		((ListView)(object)listView).UseCompatibleStateImageBehavior = false;
		componentResourceManager.ApplyResources(columnHeader1, "columnHeader1");
		componentResourceManager.ApplyResources(columnHeader2, "columnHeader2");
		componentResourceManager.ApplyResources(columnHeader3, "columnHeader3");
		componentResourceManager.ApplyResources(columnHeader4, "columnHeader4");
		componentResourceManager.ApplyResources(columnHeader5, "columnHeader5");
		componentResourceManager.ApplyResources(columnHeader6, "columnHeader6");
		componentResourceManager.ApplyResources(columnHeader7, "columnHeader7");
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("Panel_TestResults");
		((Control)this).Controls.Add((Control)(object)listView);
		((Control)this).Name = "UserPanel";
		((ISupportInitialize)listView).EndInit();
		((Control)this).ResumeLayout(performLayout: false);
	}
}
