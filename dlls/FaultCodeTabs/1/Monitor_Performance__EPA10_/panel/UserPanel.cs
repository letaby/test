// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.FaultCodeTabs.General.Monitor_Performance__EPA10_.panel.UserPanel
// Assembly: FaultCodeTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: 35DAF471-66CA-4F8E-B39E-2FF7E69A8BE3
// Assembly location: C:\Users\petra\Downloads\Архив (2)\FaultCodeTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Interfaces;
using DetroitDiesel.Windows.Forms;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using DetroitDiesel.Windows.Forms.Themed;
using SapiLayer1;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

#nullable disable
namespace DetroitDiesel.FaultCodeTabs.General.Monitor_Performance__EPA10_.panel;

public class UserPanel : CustomPanel, IRefreshable, ISearchable, ISupportEdit
{
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
  private Dictionary<Channel, Dictionary<object, UserPanel.UpdateItem>> updateMap = new Dictionary<Channel, Dictionary<object, UserPanel.UpdateItem>>();
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

  protected static string EcuInfoValue(Channel channel, string qualifier)
  {
    EcuInfo ecuInfo = channel.EcuInfos[qualifier];
    return ecuInfo != null && ecuInfo.Value != null ? ecuInfo.Value : string.Empty;
  }

  protected static string EcuInfoRawValue(Channel channel, string qualifier)
  {
    EcuInfo ecuInfo = channel.EcuInfos[qualifier];
    if (ecuInfo == null || ecuInfo.EcuInfoValues.Current == null || ecuInfo.EcuInfoValues.Current.Value == null)
      return string.Empty;
    Choice choice = ecuInfo.EcuInfoValues.Current.Value as Choice;
    return choice != (object) null ? choice.RawValue.ToString() : ecuInfo.EcuInfoValues.Current.Value.ToString();
  }

  private static string GetDescriptionForGroup(Channel channel, string groupid)
  {
    EcuInfo ecuInfo = channel.EcuInfos["DT_STO_Read_aggregated_RBM_group_rates_Assigned_RBM_group_number_1"];
    if (ecuInfo != null)
    {
      ChoiceCollection choiceCollection = SapiExtensions.Choices(ecuInfo);
      if (choiceCollection != null)
      {
        string descriptionForGroup = SapiExtensions.NameFromRawValue(choiceCollection, (object) groupid);
        if (descriptionForGroup != null)
          return descriptionForGroup;
      }
    }
    return string.Empty;
  }

  private static string Ratio(string numerator, string denominator)
  {
    string empty = string.Empty;
    if (!string.IsNullOrEmpty(numerator) && !string.IsNullOrEmpty(denominator))
    {
      double result1 = 0.0;
      double result2 = 0.0;
      if (double.TryParse(numerator, out result1) && double.TryParse(denominator, out result2) && result2 > 0.0)
        empty = (result1 / result2).ToString("N2", (IFormatProvider) CultureInfo.InvariantCulture);
    }
    return empty;
  }

  public UserPanel()
  {
    this.InitializeComponent();
    this.imageList = new ImageList();
    this.imageList.ColorDepth = ColorDepth.Depth32Bit;
    this.imageList.TransparentColor = Color.Transparent;
    this.imageList.Images.Add("group", FaultListIcons.Group);
    this.listView.ShowStateImages = (ImageBehavior) 2;
    ((ListView) this.listView).StateImageList = this.imageList;
    ((ListView) this.listView).UseCompatibleStateImageBehavior = false;
    this.boldFont = new Font(((Control) this).Font.FontFamily, ((Control) this).Font.Size, FontStyle.Bold);
    SapiManager.GlobalInstance.ActiveChannelsChanged += new EventHandler(this.OnActiveChannelsChanged);
    this.UpdateActiveChannels();
  }

  protected virtual void OnLoad(EventArgs e)
  {
    int num = ((Control) this.listView).ClientSize.Width - SystemInformation.VerticalScrollBarWidth * 2;
    this.columnHeaderDescription.Width = num / 2;
    this.columnHeaderNumber.Width = num / 10;
    this.columnHeaderMode.Width = num / 10;
    this.columnHeaderNumerator.Width = num / 10;
    this.columnHeaderDenominator.Width = num / 10;
    this.columnHeaderRatio.Width = num / 10;
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
  }

  private void OnActiveChannelsChanged(object sender, EventArgs e) => this.UpdateActiveChannels();

  private void UpdateActiveChannels()
  {
    if (this.activeChannels != null)
    {
      this.activeChannels.ConnectCompleteEvent -= new ConnectCompleteEventHandler(this.OnConnectCompleteEvent);
      this.activeChannels.DisconnectCompleteEvent -= new DisconnectCompleteEventHandler(this.OnDisconnectCompleteEvent);
      foreach (Channel activeChannel in this.activeChannels)
        this.RemoveChannel(activeChannel);
    }
    this.activeChannels = SapiManager.GlobalInstance.ActiveChannels;
    if (this.activeChannels == null)
      return;
    this.activeChannels.ConnectCompleteEvent += new ConnectCompleteEventHandler(this.OnConnectCompleteEvent);
    this.activeChannels.DisconnectCompleteEvent += new DisconnectCompleteEventHandler(this.OnDisconnectCompleteEvent);
    foreach (Channel activeChannel in this.activeChannels)
      this.AddChannel(activeChannel);
  }

  private void OnConnectCompleteEvent(object sender, ResultEventArgs e)
  {
    if (!e.Succeeded)
      return;
    this.AddChannel((Channel) sender);
  }

  private void OnDisconnectCompleteEvent(object sender, EventArgs e)
  {
    this.RemoveChannel(sender as Channel);
  }

  private void AddChannel(Channel channel)
  {
    if (!channel.FaultCodes.SupportsSnapshot)
      return;
    channel.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdateEvent);
    channel.EcuInfos.EcuInfoUpdateEvent += new EcuInfoUpdateEventHandler(this.ecuInfos_EcuInfoUpdateEvent);
    channel.FaultCodes.FaultCodesUpdateEvent += new FaultCodesUpdateEventHandler(this.OnFaultCodeCollectionUpdateEvent);
    if (channel.CommunicationsState == CommunicationsState.Online || channel.LogFile != null)
      this.AddChannelItems(channel);
    else
      this.waitingToReadComplete.Add(channel);
  }

  private Dictionary<string, List<FaultCode>> GetGroupItemMap(Channel channel)
  {
    this.updateMap[channel] = new Dictionary<object, UserPanel.UpdateItem>();
    Dictionary<string, List<FaultCode>> groupItemMap = new Dictionary<string, List<FaultCode>>();
    this.updateMap[channel].Add((object) channel, new UserPanel.UpdateItem(channel.EcuInfos["DT_STO_Read_aggregated_RBM_group_rates_Ignition_Cycle_counter"], channel.EcuInfos["DT_STO_Read_aggregated_RBM_group_rates_General_Denominator"]));
    int result1 = 0;
    int.TryParse(UserPanel.EcuInfoValue(channel, "DT_STO_Read_aggregated_RBM_group_rates_Number_of_following_MU_rates"), out result1);
    for (int index = 1; index <= result1; ++index)
    {
      string key = UserPanel.EcuInfoRawValue(channel, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "DT_STO_Read_aggregated_RBM_group_rates_Assigned_RBM_group_number_{0}", (object) index));
      if (!groupItemMap.ContainsKey(key))
      {
        groupItemMap.Add(key, new List<FaultCode>());
        if (!this.updateMap[channel].ContainsKey((object) ("group" + key)))
          this.updateMap[channel].Add((object) ("group" + key), new UserPanel.UpdateItem(channel.EcuInfos[string.Format((IFormatProvider) CultureInfo.InvariantCulture, "DT_STO_Read_aggregated_RBM_group_rates_Numerator_value_{0}", (object) index)], channel.EcuInfos[string.Format((IFormatProvider) CultureInfo.InvariantCulture, "DT_STO_Read_aggregated_RBM_group_rates_Denominator_value_{0}", (object) index)]));
      }
    }
    int num = 1;
    while (true)
    {
      string s = UserPanel.EcuInfoValue(channel, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "DT_STO_Read_MU_individual_rates_DataFrame{0}_Number_of_following_MU_rates", (object) num));
      if (!string.IsNullOrEmpty(s))
      {
        int result2 = 0;
        int.TryParse(s, out result2);
        if (result2 > 0)
        {
          for (int index = 1; index <= result2; ++index)
          {
            string key = UserPanel.EcuInfoRawValue(channel, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "DT_STO_Read_MU_individual_rates_DataFrame{0}_Assigned_RBM_group_number_{1}", (object) num, (object) index));
            if (groupItemMap.ContainsKey(key))
            {
              string code = UserPanel.EcuInfoValue(channel, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "DT_STO_Read_MU_individual_rates_DataFrame{0}_SPN_Low_Byte_{1}", (object) num, (object) index)) + UserPanel.EcuInfoValue(channel, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "DT_STO_Read_MU_individual_rates_DataFrame{0}_SPN_Mid_Byte_{1}", (object) num, (object) index)) + UserPanel.EcuInfoValue(channel, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "DT_STO_Read_MU_individual_rates_DataFrame{0}_SPN_High_Byte_FMI_{1}", (object) num, (object) index));
              FaultCode faultCode = channel.FaultCodes[code];
              if (faultCode != null)
              {
                groupItemMap[key].Add(faultCode);
                if (!this.updateMap[channel].ContainsKey((object) faultCode))
                  this.updateMap[channel].Add((object) faultCode, new UserPanel.UpdateItem(channel.EcuInfos[string.Format((IFormatProvider) CultureInfo.InvariantCulture, "DT_STO_Read_MU_individual_rates_DataFrame{0}_Numerator_value_{1}", (object) num, (object) index)], channel.EcuInfos[string.Format((IFormatProvider) CultureInfo.InvariantCulture, "DT_STO_Read_MU_individual_rates_DataFrame{0}_Denominator_value_{1}", (object) num, (object) index)]));
              }
            }
          }
        }
        ++num;
      }
      else
        break;
    }
    return groupItemMap;
  }

  private ListViewExGroupItem AddItem(
    string name,
    object tag,
    string number,
    string mode,
    int stateImage,
    bool error)
  {
    ListViewExGroupItem listViewExGroupItem = new ListViewExGroupItem(name);
    ((ListViewItem) listViewExGroupItem).SubItems.Add(number);
    ((ListViewItem) listViewExGroupItem).SubItems.Add(mode);
    ((ListViewItem) listViewExGroupItem).SubItems.Add(string.Empty);
    ((ListViewItem) listViewExGroupItem).SubItems.Add(string.Empty);
    ((ListViewItem) listViewExGroupItem).SubItems.Add(string.Empty);
    ((ListViewItem) listViewExGroupItem).UseItemStyleForSubItems = false;
    ((ListViewItem) listViewExGroupItem).Tag = tag;
    if (stateImage != -1)
      ((ListViewItem) listViewExGroupItem).StateImageIndex = stateImage;
    if (error)
      ((ListViewItem) listViewExGroupItem).ForeColor = Color.Red;
    else
      ((ListViewItem) listViewExGroupItem).ForeColor = SystemColors.ControlText;
    return listViewExGroupItem;
  }

  private bool IsCodeActionable(FaultCode faultCode)
  {
    FaultCodeIncident currentByFunction = faultCode.FaultCodeIncidents.GetCurrentByFunction(ReadFunctions.NonPermanent);
    return currentByFunction != null && SapiManager.IsFaultActionable(currentByFunction);
  }

  private ListViewExGroupItem AddPrioritizedChannelItem(ListViewExGroupItem channelItem)
  {
    int? nullable1 = new int?((((ListViewItem) channelItem).Tag as Channel).Ecu.Priority);
    for (int index = 0; index < ((ListView) this.listView).Items.Count; ++index)
    {
      if (((ListView) this.listView).Items[index].Tag is Channel tag)
      {
        int? nullable2 = new int?(tag.Ecu.Priority);
        if (nullable2.HasValue && nullable2.Value > nullable1.Value)
          return (ListViewExGroupItem) ((ListView) this.listView).Items.Insert(index, (ListViewItem) channelItem);
      }
    }
    return (ListViewExGroupItem) ((ListView) this.listView).Items.Add((ListViewItem) channelItem);
  }

  private void AddChannelItems(Channel channel)
  {
    Dictionary<string, List<FaultCode>> groupItemMap = this.GetGroupItemMap(channel);
    if (groupItemMap.Keys.Count <= 0)
      return;
    this.listView.BeginUpdate();
    this.listView.LockSorting();
    ListViewExGroupItem channelItem = this.AddItem(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "[{0}] {1} - {2}", (object) channel.Ecu.Identifier, (object) channel.Ecu.Name, (object) channel.Ecu.ShortDescription), (object) channel, string.Empty, string.Empty, 0, false);
    ((ListViewItem) channelItem).Font = this.boldFont;
    UserPanel.UpdateItem ratioQualifiers1 = this.GetRatioQualifiers(channel, ((ListViewItem) channelItem).Tag);
    ratioQualifiers1.item = channelItem;
    this.UpdateRatio(ratioQualifiers1);
    this.AddPrioritizedChannelItem(channelItem);
    foreach (string key in groupItemMap.Keys)
    {
      if (groupItemMap[key].Count > 0)
      {
        ListViewExGroupItem listViewExGroupItem1 = this.AddItem(UserPanel.GetDescriptionForGroup(channel, key), (object) ("group" + key), string.Empty, string.Empty, 0, false);
        UserPanel.UpdateItem ratioQualifiers2 = this.GetRatioQualifiers(channel, ((ListViewItem) listViewExGroupItem1).Tag);
        ratioQualifiers2.item = listViewExGroupItem1;
        this.UpdateRatio(ratioQualifiers2);
        channelItem.Add(listViewExGroupItem1);
        foreach (FaultCode faultCode in groupItemMap[key])
        {
          ListViewExGroupItem listViewExGroupItem2 = this.AddItem(faultCode.Text, (object) faultCode, faultCode.Number, faultCode.Mode, -1, this.IsCodeActionable(faultCode));
          UserPanel.UpdateItem ratioQualifiers3 = this.GetRatioQualifiers(channel, ((ListViewItem) listViewExGroupItem2).Tag);
          ratioQualifiers3.item = listViewExGroupItem2;
          this.UpdateRatio(ratioQualifiers3);
          listViewExGroupItem1.Add(listViewExGroupItem2);
        }
        listViewExGroupItem1.Collapse();
      }
    }
    this.listView.UnlockSorting();
    this.listView.EndUpdate();
  }

  private ListViewExGroupItem FindItemByTag(object itemSought)
  {
    ListViewExGroupItem itemByTag = (ListViewExGroupItem) null;
    foreach (ListViewExGroupItem listViewExGroupItem in ((ListView) this.listView).Items)
    {
      if (((ListViewItem) listViewExGroupItem).Tag == itemSought)
      {
        itemByTag = listViewExGroupItem;
        break;
      }
    }
    return itemByTag;
  }

  private void RemoveTimer(UserPanel.UpdateItem updateItem)
  {
    if (updateItem.timer == null)
      return;
    updateItem.timer.Stop();
    updateItem.timer.Tick -= new EventHandler(this.updateTimer_Tick);
    updateItem.timer.Dispose();
    updateItem.timer = (Timer) null;
  }

  private void RemoveChannel(Channel channel)
  {
    if (!channel.FaultCodes.SupportsSnapshot)
      return;
    channel.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdateEvent);
    channel.EcuInfos.EcuInfoUpdateEvent -= new EcuInfoUpdateEventHandler(this.ecuInfos_EcuInfoUpdateEvent);
    channel.FaultCodes.FaultCodesUpdateEvent -= new FaultCodesUpdateEventHandler(this.OnFaultCodeCollectionUpdateEvent);
    ListViewExGroupItem itemByTag = this.FindItemByTag((object) channel);
    if (itemByTag != null)
    {
      itemByTag.RemoveAll();
      ((ListViewItem) itemByTag).Remove();
    }
    if (this.updateMap.ContainsKey(channel))
    {
      Dictionary<object, UserPanel.UpdateItem> update = this.updateMap[channel];
      foreach (object key in update.Keys)
      {
        UserPanel.UpdateItem updateItem = update[key];
        if (updateItem.timer != null)
          this.RemoveTimer(updateItem);
      }
      this.updateMap.Remove(channel);
    }
  }

  private void OnCommunicationsStateUpdateEvent(object sender, CommunicationsStateEventArgs e)
  {
    Channel channel = sender as Channel;
    if (channel.CommunicationsState != CommunicationsState.Online || !this.waitingToReadComplete.Contains(channel))
      return;
    this.AddChannelItems(channel);
    this.waitingToReadComplete.Remove(channel);
  }

  private UserPanel.UpdateItem GetRatioQualifiers(Channel channel, object tag)
  {
    if (this.updateMap.ContainsKey(channel))
    {
      Dictionary<object, UserPanel.UpdateItem> update = this.updateMap[channel];
      if (update.ContainsKey(tag))
        return update[tag];
    }
    return (UserPanel.UpdateItem) null;
  }

  private void UpdateListSubItem(ListViewExGroupItem item, int index, string text)
  {
    if (item == null || ((ListViewItem) item).SubItems.Count <= index)
      return;
    string str = text != null ? text : string.Empty;
    if (!str.Equals(((ListViewItem) item).SubItems[index].Text))
    {
      if (((ListViewItem) item).SubItems[index].Text != string.Empty)
      {
        ((ListViewItem) item).SubItems[index].BackColor = ThemeProvider.GlobalInstance.ActiveTheme.GetStateBackColor((ValueState) 1);
        ((ListViewItem) item).SubItems[index].ForeColor = ThemeProvider.GlobalInstance.ActiveTheme.GetStateForeColor((ValueState) 1);
      }
      ((ListViewItem) item).SubItems[index].Text = str;
    }
  }

  private void UpdateRatio(UserPanel.UpdateItem qualifiers)
  {
    if (qualifiers == null || qualifiers.numerator == null || qualifiers.denominator == null || qualifiers.item == null)
      return;
    this.UpdateListSubItem(qualifiers.item, 3, qualifiers.numerator.Value);
    this.UpdateListSubItem(qualifiers.item, 4, qualifiers.denominator.Value);
    this.UpdateListSubItem(qualifiers.item, 5, UserPanel.Ratio(qualifiers.numerator.Value, qualifiers.denominator.Value));
  }

  private void ecuInfos_EcuInfoUpdateEvent(object sender, ResultEventArgs e)
  {
    EcuInfo ecuInfo = sender as EcuInfo;
    if (!this.updateMap.ContainsKey(ecuInfo.Channel))
      return;
    Dictionary<object, UserPanel.UpdateItem> update = this.updateMap[ecuInfo.Channel];
    foreach (object key in update.Keys)
    {
      UserPanel.UpdateItem qualifiers = update[key];
      if (sender == qualifiers.numerator || sender == qualifiers.denominator)
      {
        this.listView.BeginUpdate();
        this.UpdateRatio(qualifiers);
        this.listView.EndUpdate();
        if (qualifiers.timer == null)
        {
          qualifiers.timer = new Timer();
          qualifiers.timer.Tick += new EventHandler(this.updateTimer_Tick);
        }
        else
          qualifiers.timer.Stop();
        qualifiers.timer.Interval = 30000;
        qualifiers.timer.Start();
        break;
      }
    }
  }

  private void updateTimer_Tick(object sender, EventArgs e)
  {
    Timer timer = sender as Timer;
    foreach (Channel key1 in this.updateMap.Keys)
    {
      Dictionary<object, UserPanel.UpdateItem> update = this.updateMap[key1];
      foreach (object key2 in update.Keys)
      {
        UserPanel.UpdateItem updateItem = update[key2];
        if (updateItem.timer == timer)
        {
          if (updateItem.item != null && ((ListViewItem) updateItem.item).SubItems.Count > 5)
          {
            this.listView.BeginUpdate();
            ((ListViewItem) updateItem.item).SubItems[3].BackColor = ((ListViewItem) updateItem.item).SubItems[4].BackColor = ((ListViewItem) updateItem.item).SubItems[5].BackColor = SystemColors.Window;
            ((ListViewItem) updateItem.item).SubItems[3].ForeColor = ((ListViewItem) updateItem.item).SubItems[4].ForeColor = ((ListViewItem) updateItem.item).SubItems[5].ForeColor = SystemColors.WindowText;
            this.listView.EndUpdate();
          }
          this.RemoveTimer(updateItem);
        }
      }
    }
  }

  private void UpdateColor(ListViewExGroupItem item, bool actionable)
  {
    Color color = actionable ? Color.Red : SystemColors.WindowText;
    if (!(((ListViewItem) item).ForeColor != color))
      return;
    ((ListViewItem) item).ForeColor = color;
  }

  private void OnFaultCodeCollectionUpdateEvent(object sender, ResultEventArgs e)
  {
    ListViewExGroupItem itemByTag = this.FindItemByTag((object) (sender as FaultCodeCollection).Channel);
    if (itemByTag == null)
      return;
    this.listView.BeginUpdate();
    foreach (ListViewExGroupItem child1 in (IEnumerable) itemByTag.Children)
    {
      foreach (ListViewExGroupItem child2 in (IEnumerable) child1.Children)
      {
        if (((ListViewItem) child2).Tag is FaultCode tag)
          this.UpdateColor(child2, this.IsCodeActionable(tag));
      }
    }
    this.listView.EndUpdate();
  }

  public bool CanRefreshView
  {
    get
    {
      if (!SapiManager.GlobalInstance.Online)
        return false;
      bool canRefreshView = true;
      foreach (Channel activeChannel in this.activeChannels)
      {
        if (activeChannel.Online && activeChannel.CommunicationsState != CommunicationsState.Online)
          canRefreshView = false;
      }
      return canRefreshView;
    }
  }

  public void RefreshView()
  {
    Sapi.GetSapi();
    if (!SapiManager.GlobalInstance.Online)
      return;
    foreach (Channel activeChannel in this.activeChannels)
    {
      if (activeChannel.Online && activeChannel.CommunicationsState == CommunicationsState.Online)
      {
        foreach (EcuInfo ecuInfo in (ReadOnlyCollection<EcuInfo>) activeChannel.EcuInfos)
          ecuInfo.Marked = false;
        if (this.updateMap.ContainsKey(activeChannel))
        {
          Dictionary<object, UserPanel.UpdateItem> update = this.updateMap[activeChannel];
          foreach (object key in update.Keys)
          {
            UserPanel.UpdateItem updateItem = update[key];
            if (updateItem.numerator != null)
              updateItem.numerator.Marked = true;
            if (updateItem.denominator != null)
              updateItem.denominator.Marked = true;
          }
        }
        activeChannel.EcuInfos.EcuInfosReadCompleteEvent += new EcuInfosReadCompleteEventHandler(this.ecuInfos_ReadCompleteEvent);
        activeChannel.EcuInfos.Read(false);
      }
    }
  }

  private void ecuInfos_ReadCompleteEvent(object sender, ResultEventArgs e)
  {
    EcuInfoCollection ecuInfoCollection = sender as EcuInfoCollection;
    foreach (EcuInfo ecuInfo in (ReadOnlyCollection<EcuInfo>) ecuInfoCollection)
      ecuInfo.Marked = true;
    ecuInfoCollection.EcuInfosReadCompleteEvent -= new EcuInfosReadCompleteEventHandler(this.ecuInfos_ReadCompleteEvent);
  }

  public virtual bool CanUndo => this.listView.CanUndo;

  public virtual void Undo() => this.listView.Undo();

  public virtual bool CanCopy => this.listView.CanCopy;

  public virtual void Copy() => this.listView.Copy();

  public virtual bool CanDelete => this.listView.CanDelete;

  public virtual void Delete() => this.listView.Delete();

  public virtual bool CanPaste => this.listView.CanPaste;

  public virtual void Paste() => this.listView.Paste();

  public virtual bool CanCut => this.listView.CanCut;

  public virtual void Cut() => this.listView.Cut();

  public virtual bool CanSelectAll => this.listView.CanSelectAll;

  public virtual void SelectAll() => this.listView.SelectAll();

  public virtual bool CanProvideHtml => ((ListView) this.listView).Items.Count > 0;

  public virtual string ToHtml()
  {
    StringBuilder stringBuilder = new StringBuilder();
    if (base.CanProvideHtml)
    {
      using (XmlWriter writer = PrintHelper.CreateWriter(stringBuilder))
      {
        this.AddReadiness(writer);
        writer.WriteStartElement("div");
        writer.WriteStartElement("table");
        writer.WriteStartElement("thead");
        writer.WriteElementString("h1", Resources.Message_MonitorPerformanceData);
        writer.WriteEndElement();
        writer.WriteStartElement("tbody");
        foreach (ListViewExGroupItem listViewExGroupItem in ((ListView) this.listView).Items)
        {
          if (listViewExGroupItem.Level == 0)
          {
            writer.WriteStartElement("div");
            writer.WriteStartAttribute("id");
            writer.WriteString(((ListViewItem) listViewExGroupItem).SubItems[0].Text);
            writer.WriteEndAttribute();
            writer.WriteStartAttribute("name");
            writer.WriteString(((ListViewItem) listViewExGroupItem).SubItems[0].Text);
            writer.WriteEndAttribute();
            writer.WriteStartElement("table");
            writer.WriteStartElement("tr");
            writer.WriteStartElement("td");
            writer.WriteStartAttribute("class");
            writer.WriteString("ecu");
            writer.WriteEndAttribute();
            writer.WriteString(((ListViewItem) listViewExGroupItem).SubItems[0].Text);
            writer.WriteFullEndElement();
            writer.WriteStartElement("td");
            writer.WriteStartAttribute("valign");
            writer.WriteString("bottom");
            writer.WriteEndAttribute();
            writer.WriteStartAttribute("class");
            writer.WriteString("standard");
            writer.WriteEndAttribute();
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteStartElement("div");
            writer.WriteStartAttribute("class");
            writer.WriteString("gradientline0");
            writer.WriteEndAttribute();
            writer.WriteFullEndElement();
            writer.WriteStartElement("table");
            writer.WriteStartElement("tr");
            for (int index = 0; index <= ((ListView) this.listView).Columns.Count - 1; ++index)
            {
              writer.WriteStartElement("th");
              writer.WriteStartAttribute("align");
              writer.WriteString("left");
              writer.WriteEndAttribute();
              writer.WriteStartElement("font");
              writer.WriteStartAttribute("face");
              writer.WriteString("Arial");
              writer.WriteEndAttribute();
              writer.WriteString(((ListView) this.listView).Columns[index].Text);
              writer.WriteFullEndElement();
            }
            writer.WriteFullEndElement();
            this.AddChildrenToHtml(listViewExGroupItem, writer);
            writer.WriteFullEndElement();
            writer.WriteFullEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();
          }
        }
      }
    }
    return stringBuilder.ToString();
  }

  private void AddReadiness(XmlWriter xmlWriter)
  {
    ThemeDefinition activeTheme = ThemeProvider.GlobalInstance.ActiveTheme;
    xmlWriter.WriteStartElement("div");
    xmlWriter.WriteStartElement("table");
    xmlWriter.WriteStartElement("thead");
    xmlWriter.WriteElementString("h1", Resources.Message_ReadinessState);
    xmlWriter.WriteEndElement();
    xmlWriter.WriteStartElement("tbody");
    foreach (IGrouping<Channel, DataItem> grouping in this.readinessControl1.DisplayedReadinessDataItems.GroupBy<DataItem, Channel>((Func<DataItem, Channel>) (x => x.Channel)))
    {
      xmlWriter.WriteStartElement("tr");
      foreach (DataItem dataItem in (IEnumerable<DataItem>) grouping)
      {
        xmlWriter.WriteStartElement("td");
        xmlWriter.WriteAttributeString("class", "standard");
        xmlWriter.WriteAttributeString("style", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "background-color: {0}; color: {1};", (object) ColorTranslator.ToHtml(activeTheme.GetStateBackColor(dataItem.ValueState)), (object) ColorTranslator.ToHtml(activeTheme.GetStateForeColor(dataItem.ValueState))));
        xmlWriter.WriteString(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0} ({1}) {2}", (object) ReadinessControl.GetReadinessItemShortName(dataItem), (object) grouping.Key.Ecu.Name, (object) dataItem.Value.ToString()));
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
    foreach (ListViewExGroupItem child in (IEnumerable) item.Children)
    {
      if (child.HasChildren)
      {
        xmlWriter.WriteStartElement("tr");
        xmlWriter.WriteStartElement("td");
        xmlWriter.WriteStartAttribute("class");
        xmlWriter.WriteString("group");
        xmlWriter.WriteEndAttribute();
        xmlWriter.WriteString(((ListViewItem) child).Text);
        xmlWriter.WriteFullEndElement();
        xmlWriter.WriteStartElement("td");
        xmlWriter.WriteStartAttribute("class");
        xmlWriter.WriteString("group");
        xmlWriter.WriteEndAttribute();
        xmlWriter.WriteFullEndElement();
        xmlWriter.WriteFullEndElement();
        this.AddChildrenToHtml(child, xmlWriter);
      }
      else
      {
        xmlWriter.WriteStartElement("tr");
        xmlWriter.WriteAttributeString("class", Resources.Message_Standard);
        for (int index = 0; index <= ((ListView) this.listView).Columns.Count - 1; ++index)
        {
          xmlWriter.WriteStartElement("td");
          xmlWriter.WriteAttributeString("bgcolor", ColorTranslator.ToHtml(((ListViewItem) child).SubItems[index].BackColor));
          xmlWriter.WriteString(((ListViewItem) child).SubItems[index].Text);
          xmlWriter.WriteFullEndElement();
        }
        xmlWriter.WriteFullEndElement();
      }
    }
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.listView = new ListViewEx();
    this.columnHeaderDescription = new ColumnHeader();
    this.columnHeaderNumber = new ColumnHeader();
    this.columnHeaderMode = new ColumnHeader();
    this.columnHeaderNumerator = new ColumnHeader();
    this.columnHeaderDenominator = new ColumnHeader();
    this.columnHeaderRatio = new ColumnHeader();
    this.readinessControl1 = new ReadinessControl();
    ((ISupportInitialize) this.listView).BeginInit();
    ((Control) this).SuspendLayout();
    this.listView.CanDelete = false;
    ((ListView) this.listView).Columns.AddRange(new ColumnHeader[6]
    {
      this.columnHeaderDescription,
      this.columnHeaderNumber,
      this.columnHeaderMode,
      this.columnHeaderNumerator,
      this.columnHeaderDenominator,
      this.columnHeaderRatio
    });
    componentResourceManager.ApplyResources((object) this.listView, "listView");
    this.listView.EditableColumn = -1;
    this.listView.GridLines = true;
    ((Control) this.listView).Name = "listView";
    ((ListView) this.listView).UseCompatibleStateImageBehavior = false;
    componentResourceManager.ApplyResources((object) this.columnHeaderDescription, "columnHeaderDescription");
    componentResourceManager.ApplyResources((object) this.columnHeaderNumber, "columnHeaderNumber");
    componentResourceManager.ApplyResources((object) this.columnHeaderMode, "columnHeaderMode");
    componentResourceManager.ApplyResources((object) this.columnHeaderNumerator, "columnHeaderNumerator");
    componentResourceManager.ApplyResources((object) this.columnHeaderDenominator, "columnHeaderDenominator");
    componentResourceManager.ApplyResources((object) this.columnHeaderRatio, "columnHeaderRatio");
    componentResourceManager.ApplyResources((object) this.readinessControl1, "readinessControl1");
    ((Control) this.readinessControl1).Name = "readinessControl1";
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("Panel_MonitorPerformance");
    ((Control) this).Controls.Add((Control) this.listView);
    ((Control) this).Controls.Add((Control) this.readinessControl1);
    ((Control) this).Name = nameof (UserPanel);
    ((ISupportInitialize) this.listView).EndInit();
    ((Control) this).ResumeLayout(false);
    ((Control) this).PerformLayout();
  }

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
}
