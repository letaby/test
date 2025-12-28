// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.Windows.Forms.Diagnostics.Container.BusMonitorStatisticsForm
// Assembly: Drumroll, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: C4A91DC6-1B26-469B-9D8E-0DD5580BB754
// Assembly location: C:\Users\petra\Downloads\Telegram Desktop\Drumroll.exe

using DetroitDiesel.Common;
using DetroitDiesel.Settings;
using DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties;
using SapiLayer1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.Windows.Forms.Diagnostics.Container;

public class BusMonitorStatisticsForm : Form
{
  private BusMonitorControl busMonitorControl;
  private ListViewExGroupItem busloadItem;
  private ListViewExGroupItem burstsTotalItem;
  private ListViewExGroupItem burstTimeItem;
  private ListViewExGroupItem framesPerBurst;
  private ListViewExGroupItem dataFramesPerSecItem;
  private ListViewExGroupItem dataFramesTotalItem;
  private ListViewExGroupItem filteredFramesPerSecItem;
  private ListViewExGroupItem filteredFramesTotalItem;
  private ListViewExGroupItem errorFramesPerSecItem;
  private ListViewExGroupItem errorFramesTotalItem;
  private ListViewExGroupItem chipStateItem;
  private ListViewExGroupItem txErrorCountItem;
  private ListViewExGroupItem rxErrorCountItem;
  private ListViewExGroupItem identifierFramesTotalItem;
  private ListViewExGroupItem identifierFramesPerSecItem;
  private Dictionary<string, ListViewExGroupItem> identifierFramesTotalItems;
  private Dictionary<string, ListViewExGroupItem> identifierFramesPerSecItems;
  private IContainer components;
  private ListViewEx listView;
  private ColumnHeader columnHeaderStatistic;

  public BusMonitorStatisticsForm(BusMonitorControl busMonitorControl)
  {
    this.Font = SystemFonts.MessageBoxFont;
    this.InitializeComponent();
    this.busMonitorControl = busMonitorControl;
  }

  protected override void OnLoad(EventArgs e)
  {
    base.OnLoad(e);
    this.LoadSettings();
    this.busloadItem = this.AddItem(Resources.BusMonitorStatisticsForm_Busload);
    this.burstsTotalItem = this.AddItem(Resources.BusMonitorStatisticsForm_BurstsTotal);
    this.burstTimeItem = this.AddItem(Resources.BusMonitorStatisticsForm_BurstTime);
    this.framesPerBurst = this.AddItem(Resources.BusMonitorStatisticsForm_FramesPerBurst);
    this.dataFramesPerSecItem = this.AddItem(Resources.BusMonitorStatisticsForm_DataFramesPerSecond);
    this.dataFramesTotalItem = this.AddItem(Resources.BusMonitorStatisticsForm_DataFramesTotal);
    this.filteredFramesPerSecItem = this.AddItem(Resources.BusMonitorStatisticsForm_FilteredDataFramesPerSecond);
    this.filteredFramesTotalItem = this.AddItem(Resources.BusMonitorStatisticsForm_FilteredDataFramesTotal);
    this.errorFramesPerSecItem = this.AddItem(Resources.BusMonitorStatisticsForm_ErrorFramesPerSecond);
    this.errorFramesTotalItem = this.AddItem(Resources.BusMonitorStatisticsForm_ErrorFramesTotal);
    this.identifierFramesPerSecItem = this.AddItem(Resources.BusMonitorStatisticsForm_IdentifierDataFramesPerSecond);
    this.identifierFramesPerSecItems = new Dictionary<string, ListViewExGroupItem>();
    this.identifierFramesTotalItem = this.AddItem(Resources.BusMonitorStatisticsForm_IdentifierDataFramesTotal);
    this.identifierFramesTotalItems = new Dictionary<string, ListViewExGroupItem>();
    this.chipStateItem = this.AddItem(Resources.BusMonitorStatisticsForm_ChipState);
    this.txErrorCountItem = this.AddItem(Resources.BusMonitorStatisticsForm_TransmitErrorCount, this.chipStateItem);
    this.rxErrorCountItem = this.AddItem(Resources.BusMonitorStatisticsForm_ReceiveErrorCount, this.chipStateItem);
    this.UpdateColumns(this.busMonitorControl.ChannelStatistics.ToList<BusMonitorChannelStatistics>());
    this.UpdateKnownIdentifiers();
    this.UpdateFrameInformation();
    this.UpdateChipState();
    this.busMonitorControl.PropertyChanged += new PropertyChangedEventHandler(this.BusMonitorControl_PropertyChanged);
  }

  protected override void OnFormClosing(FormClosingEventArgs e)
  {
    this.UpdateColumns(new List<BusMonitorChannelStatistics>());
    this.SaveSettings();
    base.OnFormClosing(e);
  }

  private void LoadSettings()
  {
    Point savedLocation = SettingsManager.GlobalInstance.GetValue<Point>("Location", "BusMonitorStatisticsWindow", this.Location);
    if (!((IEnumerable<Screen>) Screen.AllScreens).Any<Screen>((Func<Screen, bool>) (s => s.Bounds.Contains(savedLocation))))
      savedLocation = Screen.PrimaryScreen.WorkingArea.Location;
    this.Bounds = new Rectangle(savedLocation, SettingsManager.GlobalInstance.GetValue<Size>("Size", "BusMonitorStatisticsWindow", this.Size));
  }

  private void SaveSettings()
  {
    SettingsManager.GlobalInstance.SetValue<Point>("Location", "BusMonitorStatisticsWindow", this.Location, false);
    SettingsManager.GlobalInstance.SetValue<Size>("Size", "BusMonitorStatisticsWindow", this.Size, false);
  }

  private void BusMonitorControl_PropertyChanged(object sender, PropertyChangedEventArgs e)
  {
    switch (e.PropertyName)
    {
      case "ChannelStatisticSet":
        this.UpdateColumns(this.busMonitorControl.ChannelStatistics.ToList<BusMonitorChannelStatistics>());
        break;
      case "KnownChannels":
      case "KnownIdentifiers":
        this.UpdateKnownIdentifiers();
        break;
      case "FrameInformation":
        this.UpdateFrameInformation();
        break;
      case "ChipState":
        this.UpdateChipState();
        break;
    }
  }

  private void UpdateKnownIdentifiers()
  {
    this.listView.BeginUpdate();
    bool flag = this.busMonitorControl.ChannelStatistics.Any<BusMonitorChannelStatistics>((Func<BusMonitorChannelStatistics, bool>) (cs => cs.HasMonitor));
    if (flag)
      this.identifierFramesPerSecItem.RemoveAll();
    this.identifierFramesTotalItem.RemoveAll();
    this.identifierFramesPerSecItems.Clear();
    this.identifierFramesTotalItems.Clear();
    foreach (string str in (IEnumerable<string>) this.busMonitorControl.KnownIdentifiers.OrderBy<string, int>((Func<string, int>) (i => BusMonitorForm.GetPriority(i))))
    {
      string identifier = str;
      Channel channel = this.busMonitorControl.KnownChannels.FirstOrDefault<Channel>((Func<Channel, bool>) (c => c.Ecu.Identifier == identifier));
      string name = channel != null ? channel.Ecu.Name : identifier;
      if (flag)
        this.identifierFramesPerSecItems[identifier] = this.AddItem(name, this.identifierFramesPerSecItem);
      this.identifierFramesTotalItems[identifier] = this.AddItem(name, this.identifierFramesTotalItem);
    }
    this.listView.EndUpdate();
  }

  private void UpdateColumns(
    List<BusMonitorChannelStatistics> channelStatistics)
  {
    int num = ((ListView) this.listView).Columns.Count - 1;
    this.listView.BeginUpdate();
    foreach (ColumnHeader column in ((ListView) this.listView).Columns.OfType<ColumnHeader>().Where<ColumnHeader>((Func<ColumnHeader, bool>) (c => c != this.columnHeaderStatistic)).ToList<ColumnHeader>())
    {
      foreach (ListViewItem listViewItem in ((ListView) this.listView).Items)
        listViewItem.SubItems.RemoveAt(column.Index);
      ((ListView) this.listView).Columns.Remove(column);
    }
    this.SwitchOnlineOfflineMode(channelStatistics.Any<BusMonitorChannelStatistics>((Func<BusMonitorChannelStatistics, bool>) (cs => cs.HasMonitor)));
    foreach (BusMonitorChannelStatistics channelStatistic in channelStatistics)
    {
      ((ListView) this.listView).Columns.Add(new ColumnHeader()
      {
        Text = channelStatistic.Title
      });
      foreach (ListViewItem listViewItem in ((ListView) this.listView).Items)
        listViewItem.SubItems.Add(string.Empty);
    }
    this.listView.EndUpdate();
    this.ClientSize = new Size(this.ClientSize.Width / (num + 2) * (channelStatistics.Count + 2), this.ClientSize.Height);
  }

  private void SwitchOnlineOfflineMode(bool hasMonitor)
  {
    if (((ListView) this.listView).Items.Contains((ListViewItem) this.busloadItem))
    {
      if (hasMonitor)
        return;
      foreach (KeyValuePair<string, ListViewExGroupItem> framesPerSecItem in this.identifierFramesPerSecItems)
        this.identifierFramesPerSecItem.Remove(framesPerSecItem.Value);
      this.identifierFramesPerSecItems.Clear();
      this.chipStateItem.Remove(this.txErrorCountItem);
      this.chipStateItem.Remove(this.rxErrorCountItem);
      ListViewExGroupItem[] listViewExGroupItemArray = new ListViewExGroupItem[9]
      {
        this.busloadItem,
        this.burstsTotalItem,
        this.burstTimeItem,
        this.framesPerBurst,
        this.dataFramesPerSecItem,
        this.errorFramesPerSecItem,
        this.filteredFramesPerSecItem,
        this.identifierFramesPerSecItem,
        this.chipStateItem
      };
      foreach (ListViewItem listViewItem in listViewExGroupItemArray)
        listViewItem.Remove();
    }
    else
    {
      if (!hasMonitor)
        return;
      ((ListView) this.listView).Items.Insert(0, (ListViewItem) this.busloadItem);
      ((ListView) this.listView).Items.Insert(((ListViewItem) this.busloadItem).Index + 1, (ListViewItem) this.burstsTotalItem);
      ((ListView) this.listView).Items.Insert(((ListViewItem) this.burstsTotalItem).Index + 1, (ListViewItem) this.burstTimeItem);
      ((ListView) this.listView).Items.Insert(((ListViewItem) this.burstTimeItem).Index + 1, (ListViewItem) this.framesPerBurst);
      ((ListView) this.listView).Items.Insert(((ListViewItem) this.dataFramesTotalItem).Index, (ListViewItem) this.dataFramesPerSecItem);
      ((ListView) this.listView).Items.Insert(((ListViewItem) this.errorFramesTotalItem).Index, (ListViewItem) this.errorFramesPerSecItem);
      ((ListView) this.listView).Items.Insert(((ListViewItem) this.filteredFramesTotalItem).Index, (ListViewItem) this.filteredFramesPerSecItem);
      ((ListView) this.listView).Items.Insert(((ListViewItem) this.identifierFramesTotalItem).Index, (ListViewItem) this.identifierFramesPerSecItem);
      ((ListView) this.listView).Items.Add((ListViewItem) this.chipStateItem);
      this.chipStateItem.Expanded = true;
      this.chipStateItem.Add(this.txErrorCountItem);
      this.chipStateItem.Add(this.rxErrorCountItem);
    }
  }

  private void UpdateFrameInformation()
  {
    bool flag = this.busMonitorControl.ChannelStatistics.Any<BusMonitorChannelStatistics>((Func<BusMonitorChannelStatistics, bool>) (cs => cs.HasMonitor));
    int index = 1;
    foreach (BusMonitorChannelStatistics channelStatistic in this.busMonitorControl.ChannelStatistics)
    {
      BusMonitorChannelStatistics statistics = channelStatistic;
      if (index < ((ListView) this.listView).Columns.Count)
      {
        BusMonitorStatisticsForm.UpdateItem(this.dataFramesTotalItem, index, new long?(statistics.TotalFrames));
        BusMonitorStatisticsForm.UpdateItem(this.filteredFramesTotalItem, index, new long?(statistics.TotalFilteredFrames));
        BusMonitorStatisticsForm.UpdateItem(this.errorFramesTotalItem, index, new long?(statistics.TotalErrorFrames));
        if (flag)
        {
          double? nullable = new double?();
          long? framesPerSecond = statistics.FramesPerSecond;
          if (framesPerSecond.HasValue)
          {
            BusMonitorControl busMonitorControl = this.busMonitorControl;
            framesPerSecond = statistics.FramesPerSecond;
            long num = framesPerSecond.Value;
            nullable = busMonitorControl.CalculateBusload(num);
          }
          ((ListViewItem) this.busloadItem).SubItems[index].Text = nullable.HasValue ? nullable.Value.ToString("F2", (IFormatProvider) CultureInfo.CurrentCulture) : Resources.BusMonitorStatisticsForm_NotAvailable;
          BusMonitorStatisticsForm.UpdateItem(this.burstsTotalItem, index, new long?(statistics.TotalBursts));
          BusMonitorStatisticsForm.UpdateItem(this.burstTimeItem, index, statistics.BurstInterval);
          BusMonitorStatisticsForm.UpdateItem(this.framesPerBurst, index, statistics.FramesPerBurst);
          BusMonitorStatisticsForm.UpdateItem(this.dataFramesPerSecItem, index, statistics.FramesPerSecond);
          BusMonitorStatisticsForm.UpdateItem(this.filteredFramesPerSecItem, index, statistics.FilteredFramesPerSecond);
          BusMonitorStatisticsForm.UpdateItem(this.errorFramesPerSecItem, index, statistics.ErrorFramesPerSecond);
        }
        UpdateIdentifierCount(this.identifierFramesTotalItems, (Func<string, long?>) (id => statistics.GetIdentifierFrameCount(id)), new long?(statistics.TotalFrames));
        if (flag)
          UpdateIdentifierCount(this.identifierFramesPerSecItems, (Func<string, long?>) (id => statistics.GetIdentifierFrameCountPerSecond(id)), statistics.FramesPerSecond);
        index++;
      }
    }
    ((Control) this.listView).Update();

    void UpdateIdentifierCount(
      Dictionary<string, ListViewExGroupItem> targetItems,
      Func<string, long?> getCount,
      long? total)
    {
      foreach (KeyValuePair<string, ListViewExGroupItem> targetItem in targetItems)
      {
        long? nullable = getCount(targetItem.Key);
        if (nullable.HasValue)
          BusMonitorStatisticsForm.UpdateItemPercentage(targetItem.Value, index, nullable, total);
        else
          BusMonitorStatisticsForm.UpdateItem(targetItem.Value, index, new long?());
      }
    }
  }

  private static string GetDisplayChipState(ChipStates? chipState)
  {
    if (chipState.HasValue)
    {
      ChipStates? nullable = chipState;
      if (nullable.HasValue)
      {
        ChipStates valueOrDefault = nullable.GetValueOrDefault();
        switch (valueOrDefault - 1)
        {
          case 0:
            return Resources.BusMonitorStatisticsForm_ChipStateBusoff;
          case 1:
            return Resources.BusMonitorStatisticsForm_ChipStatePassive;
          case 2:
            break;
          case 3:
            return Resources.BusMonitorStatisticsForm_ChipStateWarning;
          default:
            if (valueOrDefault == 8)
              return Resources.BusMonitorStatisticsForm_ChipStateActive;
            break;
        }
      }
    }
    return Resources.BusMonitorStatisticsForm_NotAvailable;
  }

  private void UpdateChipState()
  {
    if (!this.busMonitorControl.ChannelStatistics.Any<BusMonitorChannelStatistics>((Func<BusMonitorChannelStatistics, bool>) (cs => cs.HasMonitor)))
      return;
    int index1 = 1;
    foreach (BusMonitorChannelStatistics channelStatistic in this.busMonitorControl.ChannelStatistics)
    {
      if (index1 < ((ListView) this.listView).Columns.Count)
      {
        ((ListViewItem) this.chipStateItem).SubItems[index1].Text = BusMonitorStatisticsForm.GetDisplayChipState(channelStatistic.ChipState);
        ListViewExGroupItem txErrorCountItem = this.txErrorCountItem;
        int index2 = index1;
        byte? nullable1 = channelStatistic.TXErrorCount;
        long? nullable2 = nullable1.HasValue ? new long?((long) nullable1.GetValueOrDefault()) : new long?();
        BusMonitorStatisticsForm.UpdateItem(txErrorCountItem, index2, nullable2);
        ListViewExGroupItem rxErrorCountItem = this.rxErrorCountItem;
        int index3 = index1;
        nullable1 = channelStatistic.RXErrorCount;
        long? nullable3 = nullable1.HasValue ? new long?((long) nullable1.GetValueOrDefault()) : new long?();
        BusMonitorStatisticsForm.UpdateItem(rxErrorCountItem, index3, nullable3);
        ++index1;
      }
    }
    ((Control) this.listView).Update();
  }

  private static void UpdateItem(ListViewExGroupItem item, int index, long? value)
  {
    ((ListViewItem) item).SubItems[index].Text = value.HasValue ? value.Value.ToString((IFormatProvider) CultureInfo.CurrentCulture) : Resources.BusMonitorStatisticsForm_NotAvailable;
  }

  private static void UpdateItemPercentage(
    ListViewExGroupItem item,
    int index,
    long? value,
    long? total)
  {
    string str1 = Resources.BusMonitorStatisticsForm_NotAvailable;
    if (value.HasValue)
    {
      string str2 = value.Value.ToString((IFormatProvider) CultureInfo.CurrentCulture);
      string str3;
      if (!total.HasValue)
      {
        str3 = string.Empty;
      }
      else
      {
        CultureInfo invariantCulture = CultureInfo.InvariantCulture;
        double num1 = (double) value.Value;
        long? nullable1 = total;
        double? nullable2 = nullable1.HasValue ? new double?((double) nullable1.GetValueOrDefault()) : new double?();
        double? nullable3 = nullable2.HasValue ? new double?(num1 / nullable2.GetValueOrDefault()) : new double?();
        double num2 = 100.0;
        double? nullable4;
        if (!nullable3.HasValue)
        {
          nullable2 = new double?();
          nullable4 = nullable2;
        }
        else
          nullable4 = new double?(nullable3.GetValueOrDefault() * num2);
        // ISSUE: variable of a boxed type
        __Boxed<double?> local = (ValueType) nullable4;
        str3 = string.Format((IFormatProvider) invariantCulture, " [{0:0.0}%]", (object) local);
      }
      str1 = str2 + str3;
    }
    ((ListViewItem) item).SubItems[index].Text = str1;
  }

  private ListViewExGroupItem AddItem(string name, ListViewExGroupItem parent = null)
  {
    ListViewExGroupItem listViewExGroupItem = new ListViewExGroupItem(name);
    if (parent == null)
    {
      ((ListView) this.listView).Items.Add((ListViewItem) listViewExGroupItem);
    }
    else
    {
      parent.Expanded = true;
      parent.Add(listViewExGroupItem);
    }
    for (int index = 0; index < ((ListView) this.listView).Columns.Count - 1; ++index)
      ((ListViewItem) listViewExGroupItem).SubItems.Add(string.Empty);
    return listViewExGroupItem;
  }

  protected override void OnClientSizeChanged(EventArgs e)
  {
    this.OnSizeChanged(e);
    int num1 = ((ListView) this.listView).Columns.Count - 1;
    Size size = ((Control) this.listView).ClientSize;
    int num2 = size.Width - SystemInformation.VerticalScrollBarWidth;
    size = SystemInformation.BorderSize;
    int num3 = size.Width * 4;
    int num4 = num2 - num3;
    this.columnHeaderStatistic.Width = num4 / (num1 + 2) * 2;
    foreach (ColumnHeader columnHeader in ((ListView) this.listView).Columns.OfType<ColumnHeader>().Where<ColumnHeader>((Func<ColumnHeader, bool>) (c => c != this.columnHeaderStatistic)))
      columnHeader.Width = num4 / (num1 + 2);
  }

  protected override void Dispose(bool disposing)
  {
    if (disposing)
    {
      this.busMonitorControl.PropertyChanged -= new PropertyChangedEventHandler(this.BusMonitorControl_PropertyChanged);
      this.busMonitorControl = (BusMonitorControl) null;
      if (this.components != null)
        this.components.Dispose();
    }
    base.Dispose(disposing);
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (BusMonitorStatisticsForm));
    this.listView = new ListViewEx();
    this.columnHeaderStatistic = new ColumnHeader();
    ((ISupportInitialize) this.listView).BeginInit();
    this.SuspendLayout();
    this.listView.CanDelete = false;
    ((ListView) this.listView).Columns.AddRange(new ColumnHeader[1]
    {
      this.columnHeaderStatistic
    });
    componentResourceManager.ApplyResources((object) this.listView, "listView");
    this.listView.EditableColumn = -1;
    this.listView.GridLines = true;
    ((Control) this.listView).Name = "listView";
    this.listView.ShowGlyphs = (GlyphBehavior) 1;
    this.listView.ShowItemImages = (ImageBehavior) 1;
    this.listView.ShowStateImages = (ImageBehavior) 1;
    ((ListView) this.listView).UseCompatibleStateImageBehavior = false;
    componentResourceManager.ApplyResources((object) this.columnHeaderStatistic, "columnHeaderStatistic");
    componentResourceManager.ApplyResources((object) this, "$this");
    this.AutoScaleMode = AutoScaleMode.Font;
    this.Controls.Add((Control) this.listView);
    this.FormBorderStyle = FormBorderStyle.SizableToolWindow;
    this.Name = nameof (BusMonitorStatisticsForm);
    ((ISupportInitialize) this.listView).EndInit();
    this.ResumeLayout(false);
  }
}
