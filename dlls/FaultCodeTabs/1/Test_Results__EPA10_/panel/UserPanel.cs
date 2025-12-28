// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.FaultCodeTabs.General.Test_Results__EPA10_.panel.UserPanel
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
using System.Text;
using System.Windows.Forms;
using System.Xml;

#nullable disable
namespace DetroitDiesel.FaultCodeTabs.General.Test_Results__EPA10_.panel;

public class UserPanel : CustomPanel, IRefreshable, ISearchable, ISupportEdit
{
  private const string TestValueQualifierFormat = "DT_STO_ID_Scaled_Test_Results_Value_{0}_Test_Value";
  private const string TestMinQualifierFormat = "DT_STO_ID_Scaled_Test_Results_Value_{0}_Min_Test_Limit";
  private const string TestMaxQualifierFormat = "DT_STO_ID_Scaled_Test_Results_Value_{0}_Max_Test_Limit";
  private const string UnitAndScalingIDQualifierFormat = "DT_STO_ID_Scaled_Test_Results_Value_{0}_Unit_And_Scaling_ID";
  private const string SPNHighByteFMIQualifierFormat = "DT_STO_ID_Scaled_Test_Results_Fault_Value_{0}_SPN_High_Byte_FMI";
  private const string SPNMidByteQualifierFormat = "DT_STO_ID_Scaled_Test_Results_Fault_Value_{0}_SPN_Mid_Byte";
  private const string SPNLowByteQualifierFormat = "DT_STO_ID_Scaled_Test_Results_Fault_Value_{0}_SPN_Low_Byte";
  private const int highlightInterval = 30000;
  private List<Channel> waitingToReadComplete = new List<Channel>();
  private Dictionary<Channel, Dictionary<object, UserPanel.UpdateItem>> updateMap = new Dictionary<Channel, Dictionary<object, UserPanel.UpdateItem>>();
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

  protected static string EcuInfoValue(Channel channel, string qualifier)
  {
    EcuInfo ecuInfo = channel.EcuInfos[qualifier];
    return ecuInfo != null && ecuInfo.Value != null ? ecuInfo.Value : string.Empty;
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
    channel.FaultCodes.FaultCodesUpdateEvent += new FaultCodesUpdateEventHandler(this.OnFaultCodeCollectionUpdateEvent);
    if (channel.CommunicationsState == CommunicationsState.Online || channel.LogFile != null)
      this.AddChannelItems(channel);
    else
      this.waitingToReadComplete.Add(channel);
  }

  private bool TryParseTestIndex(string qualifier, out int index)
  {
    index = -1;
    string str1 = "DT_STO_ID_Scaled_Test_Results_Value_{0}_Test_Value".Substring(0, "DT_STO_ID_Scaled_Test_Results_Value_{0}_Test_Value".IndexOf("{"));
    string str2 = "DT_STO_ID_Scaled_Test_Results_Value_{0}_Test_Value".Substring("DT_STO_ID_Scaled_Test_Results_Value_{0}_Test_Value".IndexOf("}") + 1);
    if (!qualifier.StartsWith(str1) || !qualifier.EndsWith(str2))
      return false;
    string str3 = qualifier.Substring(str1.Length, qualifier.Length - str1.Length - str2.Length);
    index = Convert.ToInt32(str3);
    return true;
  }

  private IEnumerable<int> GetValidTestIndexes(Channel channel)
  {
    foreach (EcuInfo ecuInfo in (ReadOnlyCollection<EcuInfo>) channel.EcuInfos)
    {
      int result;
      if (this.TryParseTestIndex(ecuInfo.Qualifier, out result))
        yield return result;
    }
  }

  private List<FaultCode> GetItemList(Channel channel)
  {
    this.updateMap[channel] = new Dictionary<object, UserPanel.UpdateItem>();
    List<FaultCode> itemList = new List<FaultCode>();
    foreach (int validTestIndex in this.GetValidTestIndexes(channel))
    {
      string code = UserPanel.EcuInfoValue(channel, $"DT_STO_ID_Scaled_Test_Results_Fault_Value_{validTestIndex}_SPN_Low_Byte") + UserPanel.EcuInfoValue(channel, $"DT_STO_ID_Scaled_Test_Results_Fault_Value_{validTestIndex}_SPN_Mid_Byte") + UserPanel.EcuInfoValue(channel, $"DT_STO_ID_Scaled_Test_Results_Fault_Value_{validTestIndex}_SPN_High_Byte_FMI");
      FaultCode faultCode = channel.FaultCodes[code];
      if (faultCode != null)
      {
        itemList.Add(faultCode);
        this.updateMap[channel].Add((object) faultCode, new UserPanel.UpdateItem(DataItem.Create(new Qualifier((QualifierTypes) 8, channel.Ecu.Name, $"DT_STO_ID_Scaled_Test_Results_Value_{validTestIndex}_Test_Value"), (IEnumerable<Channel>) this.activeChannels), DataItem.Create(new Qualifier((QualifierTypes) 8, channel.Ecu.Name, $"DT_STO_ID_Scaled_Test_Results_Value_{validTestIndex}_Min_Test_Limit"), (IEnumerable<Channel>) this.activeChannels), DataItem.Create(new Qualifier((QualifierTypes) 8, channel.Ecu.Name, $"DT_STO_ID_Scaled_Test_Results_Value_{validTestIndex}_Max_Test_Limit"), (IEnumerable<Channel>) this.activeChannels)));
      }
    }
    return itemList;
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

  private void AddChannelItems(Channel channel)
  {
    List<FaultCode> itemList = this.GetItemList(channel);
    if (itemList.Count <= 0)
      return;
    this.listView.BeginUpdate();
    this.listView.LockSorting();
    ListViewExGroupItem listViewExGroupItem1 = this.AddItem(channel.Ecu.Name, (object) channel, string.Empty, string.Empty, 0, false);
    ((ListViewItem) listViewExGroupItem1).Font = this.boldFont;
    ((ListView) this.listView).Items.Add((ListViewItem) listViewExGroupItem1);
    foreach (FaultCode faultCode in itemList)
    {
      ListViewExGroupItem listViewExGroupItem2 = this.AddItem(faultCode.Text, (object) faultCode, faultCode.Number, faultCode.Mode, -1, this.IsCodeActionable(faultCode));
      UserPanel.UpdateItem updateItem = this.GetUpdateItem(channel, faultCode);
      updateItem.ListItem = listViewExGroupItem2;
      updateItem.UpdateValueMinMax();
      listViewExGroupItem1.Add(listViewExGroupItem2);
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

  private void RemoveChannel(Channel channel)
  {
    if (!channel.FaultCodes.SupportsSnapshot)
      return;
    channel.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdateEvent);
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
        update[key].RemoveTimer();
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

  private UserPanel.UpdateItem GetUpdateItem(Channel channel, FaultCode tag)
  {
    if (this.updateMap.ContainsKey(channel))
    {
      Dictionary<object, UserPanel.UpdateItem> update = this.updateMap[channel];
      if (update.ContainsKey((object) tag))
        return update[(object) tag];
    }
    return (UserPanel.UpdateItem) null;
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
            update[key].MarkForRefresh();
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

  public bool Search(string searchText, bool caseSensitive, FindMode direction)
  {
    return this.listView.Search(searchText, caseSensitive, direction);
  }

  public bool CanSearch => this.listView.CanSearch;

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
      XmlWriter writer = PrintHelper.CreateWriter(stringBuilder);
      writer.WriteStartElement("div");
      writer.WriteStartElement("table");
      writer.WriteStartElement("thead");
      writer.WriteElementString("h1", Resources.Message_TestResultsData);
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
        }
      }
      writer.Close();
    }
    return stringBuilder.ToString();
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.listView = new ListViewEx();
    this.columnHeader1 = new ColumnHeader();
    this.columnHeader2 = new ColumnHeader();
    this.columnHeader3 = new ColumnHeader();
    this.columnHeader4 = new ColumnHeader();
    this.columnHeader5 = new ColumnHeader();
    this.columnHeader6 = new ColumnHeader();
    this.columnHeader7 = new ColumnHeader();
    ((ISupportInitialize) this.listView).BeginInit();
    ((Control) this).SuspendLayout();
    this.listView.CanDelete = false;
    ((ListView) this.listView).Columns.AddRange(new ColumnHeader[7]
    {
      this.columnHeader1,
      this.columnHeader2,
      this.columnHeader3,
      this.columnHeader4,
      this.columnHeader5,
      this.columnHeader6,
      this.columnHeader7
    });
    componentResourceManager.ApplyResources((object) this.listView, "listView");
    this.listView.EditableColumn = -1;
    this.listView.GridLines = true;
    ((Control) this.listView).Name = "listView";
    ((ListView) this.listView).UseCompatibleStateImageBehavior = false;
    componentResourceManager.ApplyResources((object) this.columnHeader1, "columnHeader1");
    componentResourceManager.ApplyResources((object) this.columnHeader2, "columnHeader2");
    componentResourceManager.ApplyResources((object) this.columnHeader3, "columnHeader3");
    componentResourceManager.ApplyResources((object) this.columnHeader4, "columnHeader4");
    componentResourceManager.ApplyResources((object) this.columnHeader5, "columnHeader5");
    componentResourceManager.ApplyResources((object) this.columnHeader6, "columnHeader6");
    componentResourceManager.ApplyResources((object) this.columnHeader7, "columnHeader7");
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("Panel_TestResults");
    ((Control) this).Controls.Add((Control) this.listView);
    ((Control) this).Name = nameof (UserPanel);
    ((ISupportInitialize) this.listView).EndInit();
    ((Control) this).ResumeLayout(false);
  }

  private class UpdateItem
  {
    private DataItem value;
    private DataItem min;
    private DataItem max;
    private Timer timer;
    private ListViewExGroupItem item;

    public UpdateItem(DataItem value, DataItem min, DataItem max)
    {
      this.value = value;
      this.min = min;
      this.max = max;
      this.value.UpdateEvent += new EventHandler<ResultEventArgs>(this.dataItem_UpdateEvent);
      this.min.UpdateEvent += new EventHandler<ResultEventArgs>(this.dataItem_UpdateEvent);
      this.max.UpdateEvent += new EventHandler<ResultEventArgs>(this.dataItem_UpdateEvent);
    }

    public ListViewExGroupItem ListItem
    {
      set => this.item = value;
    }

    private void UpdateListSubItem(int index, string text, bool color)
    {
      if (this.item == null || ((ListViewItem) this.item).SubItems.Count <= index)
        return;
      string str = text != null ? text : string.Empty;
      if (!str.Equals(((ListViewItem) this.item).SubItems[index].Text))
      {
        if (color && ((ListViewItem) this.item).SubItems[index].Text != string.Empty)
        {
          ((ListViewItem) this.item).SubItems[index].BackColor = ThemeProvider.GlobalInstance.ActiveTheme.GetStateBackColor((ValueState) 1);
          ((ListViewItem) this.item).SubItems[index].ForeColor = ThemeProvider.GlobalInstance.ActiveTheme.GetStateForeColor((ValueState) 1);
        }
        ((ListViewItem) this.item).SubItems[index].Text = str;
      }
    }

    public void UpdateValueMinMax()
    {
      if (this.value == null || this.min == null || this.max == null || this.item == null)
        return;
      this.UpdateListSubItem(3, this.value.ValueAsString(this.value.Value), true);
      this.UpdateListSubItem(4, this.min.ValueAsString(this.min.Value), true);
      this.UpdateListSubItem(5, this.max.ValueAsString(this.max.Value), true);
      this.UpdateListSubItem(6, this.value.Units, false);
    }

    private void dataItem_UpdateEvent(object sender, ResultEventArgs e)
    {
      this.UpdateValueMinMax();
      if (this.timer == null)
      {
        this.timer = new Timer();
        this.timer.Tick += new EventHandler(this.updateTimer_Tick);
      }
      else
        this.timer.Stop();
      this.timer.Interval = 30000;
      this.timer.Start();
    }

    private void updateTimer_Tick(object sender, EventArgs e)
    {
      if (this.item != null && ((ListViewItem) this.item).SubItems.Count > 5)
      {
        ((ListViewItem) this.item).SubItems[3].BackColor = ((ListViewItem) this.item).SubItems[4].BackColor = ((ListViewItem) this.item).SubItems[5].BackColor = SystemColors.Window;
        ((ListViewItem) this.item).SubItems[3].ForeColor = ((ListViewItem) this.item).SubItems[4].ForeColor = ((ListViewItem) this.item).SubItems[5].ForeColor = SystemColors.WindowText;
      }
      this.RemoveTimer();
    }

    public void RemoveTimer()
    {
      if (this.timer == null)
        return;
      this.timer.Stop();
      this.timer.Tick -= new EventHandler(this.updateTimer_Tick);
      this.timer.Dispose();
      this.timer = (Timer) null;
    }

    public void MarkForRefresh()
    {
      if (this.value == null || this.min == null || this.max == null)
        return;
      EcuInfoCollection ecuInfos1 = this.value.Channel.EcuInfos;
      Qualifier qualifier1 = this.value.Qualifier;
      string name1 = ((Qualifier) ref qualifier1).Name;
      ecuInfos1[name1].Marked = true;
      EcuInfoCollection ecuInfos2 = this.min.Channel.EcuInfos;
      Qualifier qualifier2 = this.min.Qualifier;
      string name2 = ((Qualifier) ref qualifier2).Name;
      ecuInfos2[name2].Marked = true;
      EcuInfoCollection ecuInfos3 = this.max.Channel.EcuInfos;
      Qualifier qualifier3 = this.max.Qualifier;
      string name3 = ((Qualifier) ref qualifier3).Name;
      ecuInfos3[name3].Marked = true;
    }
  }
}
