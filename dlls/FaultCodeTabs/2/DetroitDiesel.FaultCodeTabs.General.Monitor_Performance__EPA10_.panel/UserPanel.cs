using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
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

namespace DetroitDiesel.FaultCodeTabs.General.Monitor_Performance__EPA10_.panel;

public class UserPanel : CustomPanel, IRefreshable, ISearchable, ISupportEdit
{
	private class UpdateItem
	{
		public EcuInfo numerator;

		public EcuInfo denominator;

		public ListViewExGroupItem item;

		public Timer timer;

		public UpdateItem(EcuInfo numerator, EcuInfo denominator)
		{
			this.numerator = numerator;
			this.denominator = denominator;
		}
	}

	private const string AggregateIgnitionCycleCounterQualifier = "DT_STO_Read_aggregated_RBM_group_rates_Ignition_Cycle_counter";

	private const string AggregateGeneralDenominatorQualifier = "DT_STO_Read_aggregated_RBM_group_rates_General_Denominator";

	private const string AggregateNumberGroupRatesQualifier = "DT_STO_Read_aggregated_RBM_group_rates_Number_of_following_MU_rates";

	private const string AggregateGroupNumberQualifierFormat = "DT_STO_Read_aggregated_RBM_group_rates_Assigned_RBM_group_number_{0}";

	private const string AggregateGroupDenominatorQualifierFormat = "DT_STO_Read_aggregated_RBM_group_rates_Denominator_value_{0}";

	private const string AggregateGroupNumeratorQualifierFormat = "DT_STO_Read_aggregated_RBM_group_rates_Numerator_value_{0}";

	private const string IndividualNumberRatesQualifierFormat = "DT_STO_Read_MU_individual_rates_DataFrame{0}_Number_of_following_MU_rates";

	private const string IndividualAssignedGroupQualifierFormat = "DT_STO_Read_MU_individual_rates_DataFrame{0}_Assigned_RBM_group_number_{1}";

	private const string IndividualDenominatorQualifierFormat = "DT_STO_Read_MU_individual_rates_DataFrame{0}_Denominator_value_{1}";

	private const string IndividualNumeratorQualifierFormat = "DT_STO_Read_MU_individual_rates_DataFrame{0}_Numerator_value_{1}";

	private const string IndividualSPNLowQualifierFormat = "DT_STO_Read_MU_individual_rates_DataFrame{0}_SPN_Low_Byte_{1}";

	private const string IndividualSPNMidQualifierFormat = "DT_STO_Read_MU_individual_rates_DataFrame{0}_SPN_Mid_Byte_{1}";

	private const string IndividualSPNHiFMIQualifierFormat = "DT_STO_Read_MU_individual_rates_DataFrame{0}_SPN_High_Byte_FMI_{1}";

	private const int highlightInterval = 30000;

	private List<Channel> waitingToReadComplete = new List<Channel>();

	private Dictionary<Channel, Dictionary<object, UpdateItem>> updateMap = new Dictionary<Channel, Dictionary<object, UpdateItem>>();

	private ImageList imageList;

	private Font boldFont;

	private ChannelBaseCollection activeChannels;

	private ListViewEx listView;

	private ColumnHeader columnHeaderDescription;

	private ColumnHeader columnHeaderNumber;

	private ColumnHeader columnHeaderMode;

	private ColumnHeader columnHeaderNumerator;

	private ColumnHeader columnHeaderRatio;

	private ReadinessControl readinessControl1;

	private ColumnHeader columnHeaderDenominator;

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

	protected static string EcuInfoRawValue(Channel channel, string qualifier)
	{
		EcuInfo ecuInfo = channel.EcuInfos[qualifier];
		if (ecuInfo != null && ecuInfo.EcuInfoValues.Current != null && ecuInfo.EcuInfoValues.Current.Value != null)
		{
			Choice choice = ecuInfo.EcuInfoValues.Current.Value as Choice;
			if (choice != null)
			{
				return choice.RawValue.ToString();
			}
			return ecuInfo.EcuInfoValues.Current.Value.ToString();
		}
		return string.Empty;
	}

	private static string GetDescriptionForGroup(Channel channel, string groupid)
	{
		EcuInfo ecuInfo = channel.EcuInfos["DT_STO_Read_aggregated_RBM_group_rates_Assigned_RBM_group_number_1"];
		if (ecuInfo != null)
		{
			ChoiceCollection choiceCollection = SapiExtensions.Choices(ecuInfo);
			if (choiceCollection != null)
			{
				string text = SapiExtensions.NameFromRawValue(choiceCollection, (object)groupid);
				if (text != null)
				{
					return text;
				}
			}
		}
		return string.Empty;
	}

	private static string Ratio(string numerator, string denominator)
	{
		string result = string.Empty;
		if (!string.IsNullOrEmpty(numerator) && !string.IsNullOrEmpty(denominator))
		{
			double result2 = 0.0;
			double result3 = 0.0;
			if (double.TryParse(numerator, out result2) && double.TryParse(denominator, out result3) && result3 > 0.0)
			{
				result = (result2 / result3).ToString("N2", CultureInfo.InvariantCulture);
			}
		}
		return result;
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

	protected override void OnLoad(EventArgs e)
	{
		int num = ((Control)(object)listView).ClientSize.Width - SystemInformation.VerticalScrollBarWidth * 2;
		columnHeaderDescription.Width = num / 2;
		columnHeaderNumber.Width = num / 10;
		columnHeaderMode.Width = num / 10;
		columnHeaderNumerator.Width = num / 10;
		columnHeaderDenominator.Width = num / 10;
		columnHeaderRatio.Width = num / 10;
		((UserControl)this).OnLoad(e);
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
			channel.EcuInfos.EcuInfoUpdateEvent += ecuInfos_EcuInfoUpdateEvent;
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

	private Dictionary<string, List<FaultCode>> GetGroupItemMap(Channel channel)
	{
		updateMap[channel] = new Dictionary<object, UpdateItem>();
		Dictionary<string, List<FaultCode>> dictionary = new Dictionary<string, List<FaultCode>>();
		updateMap[channel].Add(channel, new UpdateItem(channel.EcuInfos["DT_STO_Read_aggregated_RBM_group_rates_Ignition_Cycle_counter"], channel.EcuInfos["DT_STO_Read_aggregated_RBM_group_rates_General_Denominator"]));
		int result = 0;
		int.TryParse(EcuInfoValue(channel, "DT_STO_Read_aggregated_RBM_group_rates_Number_of_following_MU_rates"), out result);
		for (int i = 1; i <= result; i++)
		{
			string text = EcuInfoRawValue(channel, string.Format(CultureInfo.InvariantCulture, "DT_STO_Read_aggregated_RBM_group_rates_Assigned_RBM_group_number_{0}", i));
			if (!dictionary.ContainsKey(text))
			{
				dictionary.Add(text, new List<FaultCode>());
				if (!updateMap[channel].ContainsKey("group" + text))
				{
					updateMap[channel].Add("group" + text, new UpdateItem(channel.EcuInfos[string.Format(CultureInfo.InvariantCulture, "DT_STO_Read_aggregated_RBM_group_rates_Numerator_value_{0}", i)], channel.EcuInfos[string.Format(CultureInfo.InvariantCulture, "DT_STO_Read_aggregated_RBM_group_rates_Denominator_value_{0}", i)]));
				}
			}
		}
		int num = 1;
		while (true)
		{
			bool flag = true;
			string text2 = EcuInfoValue(channel, string.Format(CultureInfo.InvariantCulture, "DT_STO_Read_MU_individual_rates_DataFrame{0}_Number_of_following_MU_rates", num));
			if (string.IsNullOrEmpty(text2))
			{
				break;
			}
			int result2 = 0;
			int.TryParse(text2, out result2);
			if (result2 > 0)
			{
				for (int i = 1; i <= result2; i++)
				{
					string key = EcuInfoRawValue(channel, string.Format(CultureInfo.InvariantCulture, "DT_STO_Read_MU_individual_rates_DataFrame{0}_Assigned_RBM_group_number_{1}", num, i));
					if (!dictionary.ContainsKey(key))
					{
						continue;
					}
					string code = EcuInfoValue(channel, string.Format(CultureInfo.InvariantCulture, "DT_STO_Read_MU_individual_rates_DataFrame{0}_SPN_Low_Byte_{1}", num, i)) + EcuInfoValue(channel, string.Format(CultureInfo.InvariantCulture, "DT_STO_Read_MU_individual_rates_DataFrame{0}_SPN_Mid_Byte_{1}", num, i)) + EcuInfoValue(channel, string.Format(CultureInfo.InvariantCulture, "DT_STO_Read_MU_individual_rates_DataFrame{0}_SPN_High_Byte_FMI_{1}", num, i));
					FaultCode faultCode = channel.FaultCodes[code];
					if (faultCode != null)
					{
						dictionary[key].Add(faultCode);
						if (!updateMap[channel].ContainsKey(faultCode))
						{
							updateMap[channel].Add(faultCode, new UpdateItem(channel.EcuInfos[string.Format(CultureInfo.InvariantCulture, "DT_STO_Read_MU_individual_rates_DataFrame{0}_Numerator_value_{1}", num, i)], channel.EcuInfos[string.Format(CultureInfo.InvariantCulture, "DT_STO_Read_MU_individual_rates_DataFrame{0}_Denominator_value_{1}", num, i)]));
						}
					}
				}
			}
			num++;
		}
		return dictionary;
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

	private ListViewExGroupItem AddPrioritizedChannelItem(ListViewExGroupItem channelItem)
	{
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Expected O, but got Unknown
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Expected O, but got Unknown
		int? num = (((ListViewItem)(object)channelItem).Tag as Channel).Ecu.Priority;
		for (int i = 0; i < ((ListView)(object)listView).Items.Count; i++)
		{
			if (((ListView)(object)listView).Items[i].Tag is Channel channel)
			{
				int? num2 = channel.Ecu.Priority;
				if (num2.HasValue && num2.Value > num.Value)
				{
					return (ListViewExGroupItem)((ListView)(object)listView).Items.Insert(i, (ListViewItem)(object)channelItem);
				}
			}
		}
		return (ListViewExGroupItem)((ListView)(object)listView).Items.Add((ListViewItem)(object)channelItem);
	}

	private void AddChannelItems(Channel channel)
	{
		Dictionary<string, List<FaultCode>> groupItemMap = GetGroupItemMap(channel);
		if (groupItemMap.Keys.Count <= 0)
		{
			return;
		}
		listView.BeginUpdate();
		listView.LockSorting();
		ListViewExGroupItem val = AddItem(string.Format(CultureInfo.CurrentCulture, "[{0}] {1} - {2}", channel.Ecu.Identifier, channel.Ecu.Name, channel.Ecu.ShortDescription), channel, string.Empty, string.Empty, 0, error: false);
		((ListViewItem)(object)val).Font = boldFont;
		UpdateItem ratioQualifiers = GetRatioQualifiers(channel, ((ListViewItem)(object)val).Tag);
		ratioQualifiers.item = val;
		UpdateRatio(ratioQualifiers);
		AddPrioritizedChannelItem(val);
		foreach (string key in groupItemMap.Keys)
		{
			if (groupItemMap[key].Count <= 0)
			{
				continue;
			}
			ListViewExGroupItem val2 = AddItem(GetDescriptionForGroup(channel, key), "group" + key, string.Empty, string.Empty, 0, error: false);
			UpdateItem ratioQualifiers2 = GetRatioQualifiers(channel, ((ListViewItem)(object)val2).Tag);
			ratioQualifiers2.item = val2;
			UpdateRatio(ratioQualifiers2);
			val.Add(val2);
			foreach (FaultCode item in groupItemMap[key])
			{
				ListViewExGroupItem val3 = AddItem(item.Text, item, item.Number, item.Mode, -1, IsCodeActionable(item));
				UpdateItem ratioQualifiers3 = GetRatioQualifiers(channel, ((ListViewItem)(object)val3).Tag);
				ratioQualifiers3.item = val3;
				UpdateRatio(ratioQualifiers3);
				val2.Add(val3);
			}
			val2.Collapse();
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

	private void RemoveTimer(UpdateItem updateItem)
	{
		if (updateItem.timer != null)
		{
			updateItem.timer.Stop();
			updateItem.timer.Tick -= updateTimer_Tick;
			updateItem.timer.Dispose();
			updateItem.timer = null;
		}
	}

	private void RemoveChannel(Channel channel)
	{
		if (!channel.FaultCodes.SupportsSnapshot)
		{
			return;
		}
		channel.CommunicationsStateUpdateEvent -= OnCommunicationsStateUpdateEvent;
		channel.EcuInfos.EcuInfoUpdateEvent -= ecuInfos_EcuInfoUpdateEvent;
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
			if (updateItem.timer != null)
			{
				RemoveTimer(updateItem);
			}
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

	private UpdateItem GetRatioQualifiers(Channel channel, object tag)
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

	private void UpdateListSubItem(ListViewExGroupItem item, int index, string text)
	{
		if (item == null || ((ListViewItem)(object)item).SubItems.Count <= index)
		{
			return;
		}
		string text2 = ((text != null) ? text : string.Empty);
		if (!text2.Equals(((ListViewItem)(object)item).SubItems[index].Text))
		{
			if (((ListViewItem)(object)item).SubItems[index].Text != string.Empty)
			{
				((ListViewItem)(object)item).SubItems[index].BackColor = ThemeProvider.GlobalInstance.ActiveTheme.GetStateBackColor((ValueState)1);
				((ListViewItem)(object)item).SubItems[index].ForeColor = ThemeProvider.GlobalInstance.ActiveTheme.GetStateForeColor((ValueState)1);
			}
			((ListViewItem)(object)item).SubItems[index].Text = text2;
		}
	}

	private void UpdateRatio(UpdateItem qualifiers)
	{
		if (qualifiers != null && qualifiers.numerator != null && qualifiers.denominator != null && qualifiers.item != null)
		{
			UpdateListSubItem(qualifiers.item, 3, qualifiers.numerator.Value);
			UpdateListSubItem(qualifiers.item, 4, qualifiers.denominator.Value);
			UpdateListSubItem(qualifiers.item, 5, Ratio(qualifiers.numerator.Value, qualifiers.denominator.Value));
		}
	}

	private void ecuInfos_EcuInfoUpdateEvent(object sender, ResultEventArgs e)
	{
		EcuInfo ecuInfo = sender as EcuInfo;
		if (!updateMap.ContainsKey(ecuInfo.Channel))
		{
			return;
		}
		Dictionary<object, UpdateItem> dictionary = updateMap[ecuInfo.Channel];
		foreach (object key in dictionary.Keys)
		{
			UpdateItem updateItem = dictionary[key];
			if (sender == updateItem.numerator || sender == updateItem.denominator)
			{
				listView.BeginUpdate();
				UpdateRatio(updateItem);
				listView.EndUpdate();
				if (updateItem.timer == null)
				{
					updateItem.timer = new Timer();
					updateItem.timer.Tick += updateTimer_Tick;
				}
				else
				{
					updateItem.timer.Stop();
				}
				updateItem.timer.Interval = 30000;
				updateItem.timer.Start();
				break;
			}
		}
	}

	private void updateTimer_Tick(object sender, EventArgs e)
	{
		Timer timer = sender as Timer;
		foreach (Channel key in updateMap.Keys)
		{
			Dictionary<object, UpdateItem> dictionary = updateMap[key];
			foreach (object key2 in dictionary.Keys)
			{
				UpdateItem updateItem = dictionary[key2];
				if (updateItem.timer == timer)
				{
					if (updateItem.item != null && ((ListViewItem)(object)updateItem.item).SubItems.Count > 5)
					{
						listView.BeginUpdate();
						ListViewItem.ListViewSubItem listViewSubItem = ((ListViewItem)(object)updateItem.item).SubItems[3];
						ListViewItem.ListViewSubItem listViewSubItem2 = ((ListViewItem)(object)updateItem.item).SubItems[4];
						Color color = (((ListViewItem)(object)updateItem.item).SubItems[5].BackColor = SystemColors.Window);
						color = (listViewSubItem2.BackColor = color);
						listViewSubItem.BackColor = color;
						ListViewItem.ListViewSubItem listViewSubItem3 = ((ListViewItem)(object)updateItem.item).SubItems[3];
						ListViewItem.ListViewSubItem listViewSubItem4 = ((ListViewItem)(object)updateItem.item).SubItems[4];
						color = (((ListViewItem)(object)updateItem.item).SubItems[5].ForeColor = SystemColors.WindowText);
						color = (listViewSubItem4.ForeColor = color);
						listViewSubItem3.ForeColor = color;
						listView.EndUpdate();
					}
					RemoveTimer(updateItem);
				}
			}
		}
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
					if (updateItem.numerator != null)
					{
						updateItem.numerator.Marked = true;
					}
					if (updateItem.denominator != null)
					{
						updateItem.denominator.Marked = true;
					}
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
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Expected O, but got Unknown
		StringBuilder stringBuilder = new StringBuilder();
		if (((CustomPanel)this).CanProvideHtml)
		{
			using XmlWriter xmlWriter = PrintHelper.CreateWriter(stringBuilder);
			AddReadiness(xmlWriter);
			xmlWriter.WriteStartElement("div");
			xmlWriter.WriteStartElement("table");
			xmlWriter.WriteStartElement("thead");
			xmlWriter.WriteElementString("h1", Resources.Message_MonitorPerformanceData);
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
					xmlWriter.WriteEndElement();
					xmlWriter.WriteEndElement();
					xmlWriter.WriteEndElement();
				}
			}
		}
		return stringBuilder.ToString();
	}

	private void AddReadiness(XmlWriter xmlWriter)
	{
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		ThemeDefinition activeTheme = ThemeProvider.GlobalInstance.ActiveTheme;
		xmlWriter.WriteStartElement("div");
		xmlWriter.WriteStartElement("table");
		xmlWriter.WriteStartElement("thead");
		xmlWriter.WriteElementString("h1", Resources.Message_ReadinessState);
		xmlWriter.WriteEndElement();
		xmlWriter.WriteStartElement("tbody");
		IEnumerable<IGrouping<Channel, DataItem>> enumerable = from x in readinessControl1.DisplayedReadinessDataItems
			group x by x.Channel;
		foreach (IGrouping<Channel, DataItem> item in enumerable)
		{
			xmlWriter.WriteStartElement("tr");
			foreach (DataItem item2 in item)
			{
				xmlWriter.WriteStartElement("td");
				xmlWriter.WriteAttributeString("class", "standard");
				xmlWriter.WriteAttributeString("style", string.Format(CultureInfo.InvariantCulture, "background-color: {0}; color: {1};", ColorTranslator.ToHtml(activeTheme.GetStateBackColor(item2.ValueState)), ColorTranslator.ToHtml(activeTheme.GetStateForeColor(item2.ValueState))));
				xmlWriter.WriteString(string.Format(CultureInfo.CurrentCulture, "{0} ({1}) {2}", ReadinessControl.GetReadinessItemShortName(item2), item.Key.Ecu.Name, item2.Value.ToString()));
				xmlWriter.WriteEndElement();
			}
			xmlWriter.WriteEndElement();
		}
		xmlWriter.WriteEndElement();
		xmlWriter.WriteEndElement();
		xmlWriter.WriteEndElement();
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

	private void InitializeComponent()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		listView = new ListViewEx();
		columnHeaderDescription = new ColumnHeader();
		columnHeaderNumber = new ColumnHeader();
		columnHeaderMode = new ColumnHeader();
		columnHeaderNumerator = new ColumnHeader();
		columnHeaderDenominator = new ColumnHeader();
		columnHeaderRatio = new ColumnHeader();
		readinessControl1 = new ReadinessControl();
		((ISupportInitialize)listView).BeginInit();
		((Control)this).SuspendLayout();
		listView.CanDelete = false;
		((ListView)(object)listView).Columns.AddRange(new ColumnHeader[6] { columnHeaderDescription, columnHeaderNumber, columnHeaderMode, columnHeaderNumerator, columnHeaderDenominator, columnHeaderRatio });
		componentResourceManager.ApplyResources(listView, "listView");
		listView.EditableColumn = -1;
		listView.GridLines = true;
		((Control)(object)listView).Name = "listView";
		((ListView)(object)listView).UseCompatibleStateImageBehavior = false;
		componentResourceManager.ApplyResources(columnHeaderDescription, "columnHeaderDescription");
		componentResourceManager.ApplyResources(columnHeaderNumber, "columnHeaderNumber");
		componentResourceManager.ApplyResources(columnHeaderMode, "columnHeaderMode");
		componentResourceManager.ApplyResources(columnHeaderNumerator, "columnHeaderNumerator");
		componentResourceManager.ApplyResources(columnHeaderDenominator, "columnHeaderDenominator");
		componentResourceManager.ApplyResources(columnHeaderRatio, "columnHeaderRatio");
		componentResourceManager.ApplyResources(readinessControl1, "readinessControl1");
		((Control)(object)readinessControl1).Name = "readinessControl1";
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("Panel_MonitorPerformance");
		((Control)this).Controls.Add((Control)(object)listView);
		((Control)this).Controls.Add((Control)(object)readinessControl1);
		((Control)this).Name = "UserPanel";
		((ISupportInitialize)listView).EndInit();
		((Control)this).ResumeLayout(performLayout: false);
		((Control)this).PerformLayout();
	}
}
