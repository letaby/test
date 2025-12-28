using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Settings;
using DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties;
using SapiLayer1;

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
		Font = SystemFonts.MessageBoxFont;
		InitializeComponent();
		this.busMonitorControl = busMonitorControl;
	}

	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		LoadSettings();
		busloadItem = AddItem(Resources.BusMonitorStatisticsForm_Busload);
		burstsTotalItem = AddItem(Resources.BusMonitorStatisticsForm_BurstsTotal);
		burstTimeItem = AddItem(Resources.BusMonitorStatisticsForm_BurstTime);
		framesPerBurst = AddItem(Resources.BusMonitorStatisticsForm_FramesPerBurst);
		dataFramesPerSecItem = AddItem(Resources.BusMonitorStatisticsForm_DataFramesPerSecond);
		dataFramesTotalItem = AddItem(Resources.BusMonitorStatisticsForm_DataFramesTotal);
		filteredFramesPerSecItem = AddItem(Resources.BusMonitorStatisticsForm_FilteredDataFramesPerSecond);
		filteredFramesTotalItem = AddItem(Resources.BusMonitorStatisticsForm_FilteredDataFramesTotal);
		errorFramesPerSecItem = AddItem(Resources.BusMonitorStatisticsForm_ErrorFramesPerSecond);
		errorFramesTotalItem = AddItem(Resources.BusMonitorStatisticsForm_ErrorFramesTotal);
		identifierFramesPerSecItem = AddItem(Resources.BusMonitorStatisticsForm_IdentifierDataFramesPerSecond);
		identifierFramesPerSecItems = new Dictionary<string, ListViewExGroupItem>();
		identifierFramesTotalItem = AddItem(Resources.BusMonitorStatisticsForm_IdentifierDataFramesTotal);
		identifierFramesTotalItems = new Dictionary<string, ListViewExGroupItem>();
		chipStateItem = AddItem(Resources.BusMonitorStatisticsForm_ChipState);
		txErrorCountItem = AddItem(Resources.BusMonitorStatisticsForm_TransmitErrorCount, chipStateItem);
		rxErrorCountItem = AddItem(Resources.BusMonitorStatisticsForm_ReceiveErrorCount, chipStateItem);
		UpdateColumns(busMonitorControl.ChannelStatistics.ToList());
		UpdateKnownIdentifiers();
		UpdateFrameInformation();
		UpdateChipState();
		busMonitorControl.PropertyChanged += BusMonitorControl_PropertyChanged;
	}

	protected override void OnFormClosing(FormClosingEventArgs e)
	{
		UpdateColumns(new List<BusMonitorChannelStatistics>());
		SaveSettings();
		base.OnFormClosing(e);
	}

	private void LoadSettings()
	{
		Point savedLocation = SettingsManager.GlobalInstance.GetValue<Point>("Location", "BusMonitorStatisticsWindow", base.Location);
		if (!Screen.AllScreens.Any((Screen s) => s.Bounds.Contains(savedLocation)))
		{
			savedLocation = Screen.PrimaryScreen.WorkingArea.Location;
		}
		base.Bounds = new Rectangle(savedLocation, SettingsManager.GlobalInstance.GetValue<Size>("Size", "BusMonitorStatisticsWindow", base.Size));
	}

	private void SaveSettings()
	{
		SettingsManager.GlobalInstance.SetValue<Point>("Location", "BusMonitorStatisticsWindow", base.Location, false);
		SettingsManager.GlobalInstance.SetValue<Size>("Size", "BusMonitorStatisticsWindow", base.Size, false);
	}

	private void BusMonitorControl_PropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		switch (e.PropertyName)
		{
		case "ChannelStatisticSet":
			UpdateColumns(busMonitorControl.ChannelStatistics.ToList());
			break;
		case "KnownChannels":
		case "KnownIdentifiers":
			UpdateKnownIdentifiers();
			break;
		case "FrameInformation":
			UpdateFrameInformation();
			break;
		case "ChipState":
			UpdateChipState();
			break;
		}
	}

	private void UpdateKnownIdentifiers()
	{
		listView.BeginUpdate();
		bool flag = busMonitorControl.ChannelStatistics.Any((BusMonitorChannelStatistics cs) => cs.HasMonitor);
		if (flag)
		{
			identifierFramesPerSecItem.RemoveAll();
		}
		identifierFramesTotalItem.RemoveAll();
		identifierFramesPerSecItems.Clear();
		identifierFramesTotalItems.Clear();
		foreach (string identifier in busMonitorControl.KnownIdentifiers.OrderBy((string i) => BusMonitorForm.GetPriority(i)))
		{
			Channel val = busMonitorControl.KnownChannels.FirstOrDefault((Channel c) => c.Ecu.Identifier == identifier);
			string name = ((val != null) ? val.Ecu.Name : identifier);
			if (flag)
			{
				identifierFramesPerSecItems[identifier] = AddItem(name, identifierFramesPerSecItem);
			}
			identifierFramesTotalItems[identifier] = AddItem(name, identifierFramesTotalItem);
		}
		listView.EndUpdate();
	}

	private void UpdateColumns(List<BusMonitorChannelStatistics> channelStatistics)
	{
		int num = ((ListView)(object)listView).Columns.Count - 1;
		listView.BeginUpdate();
		foreach (ColumnHeader item in (from c in ((ListView)(object)listView).Columns.OfType<ColumnHeader>()
			where c != columnHeaderStatistic
			select c).ToList())
		{
			foreach (ListViewItem item2 in ((ListView)(object)listView).Items)
			{
				item2.SubItems.RemoveAt(item.Index);
			}
			((ListView)(object)listView).Columns.Remove(item);
		}
		SwitchOnlineOfflineMode(channelStatistics.Any((BusMonitorChannelStatistics cs) => cs.HasMonitor));
		foreach (BusMonitorChannelStatistics channelStatistic in channelStatistics)
		{
			ColumnHeader columnHeader = new ColumnHeader();
			columnHeader.Text = channelStatistic.Title;
			((ListView)(object)listView).Columns.Add(columnHeader);
			foreach (ListViewItem item3 in ((ListView)(object)listView).Items)
			{
				item3.SubItems.Add(string.Empty);
			}
		}
		listView.EndUpdate();
		base.ClientSize = new Size(base.ClientSize.Width / (num + 2) * (channelStatistics.Count + 2), base.ClientSize.Height);
	}

	private void SwitchOnlineOfflineMode(bool hasMonitor)
	{
		if (((ListView)(object)listView).Items.Contains((ListViewItem)(object)busloadItem))
		{
			if (hasMonitor)
			{
				return;
			}
			foreach (KeyValuePair<string, ListViewExGroupItem> identifierFramesPerSecItem in identifierFramesPerSecItems)
			{
				this.identifierFramesPerSecItem.Remove(identifierFramesPerSecItem.Value);
			}
			identifierFramesPerSecItems.Clear();
			chipStateItem.Remove(txErrorCountItem);
			chipStateItem.Remove(rxErrorCountItem);
			ListViewExGroupItem[] array = (ListViewExGroupItem[])(object)new ListViewExGroupItem[9] { busloadItem, burstsTotalItem, burstTimeItem, framesPerBurst, dataFramesPerSecItem, errorFramesPerSecItem, filteredFramesPerSecItem, this.identifierFramesPerSecItem, chipStateItem };
			foreach (ListViewExGroupItem val in array)
			{
				((ListViewItem)(object)val).Remove();
			}
		}
		else if (hasMonitor)
		{
			((ListView)(object)listView).Items.Insert(0, (ListViewItem)(object)busloadItem);
			((ListView)(object)listView).Items.Insert(((ListViewItem)(object)busloadItem).Index + 1, (ListViewItem)(object)burstsTotalItem);
			((ListView)(object)listView).Items.Insert(((ListViewItem)(object)burstsTotalItem).Index + 1, (ListViewItem)(object)burstTimeItem);
			((ListView)(object)listView).Items.Insert(((ListViewItem)(object)burstTimeItem).Index + 1, (ListViewItem)(object)framesPerBurst);
			((ListView)(object)listView).Items.Insert(((ListViewItem)(object)dataFramesTotalItem).Index, (ListViewItem)(object)dataFramesPerSecItem);
			((ListView)(object)listView).Items.Insert(((ListViewItem)(object)errorFramesTotalItem).Index, (ListViewItem)(object)errorFramesPerSecItem);
			((ListView)(object)listView).Items.Insert(((ListViewItem)(object)filteredFramesTotalItem).Index, (ListViewItem)(object)filteredFramesPerSecItem);
			((ListView)(object)listView).Items.Insert(((ListViewItem)(object)identifierFramesTotalItem).Index, (ListViewItem)(object)this.identifierFramesPerSecItem);
			((ListView)(object)listView).Items.Add((ListViewItem)(object)chipStateItem);
			chipStateItem.Expanded = true;
			chipStateItem.Add(txErrorCountItem);
			chipStateItem.Add(rxErrorCountItem);
		}
	}

	private void UpdateFrameInformation()
	{
		bool flag = busMonitorControl.ChannelStatistics.Any((BusMonitorChannelStatistics cs) => cs.HasMonitor);
		int index = 1;
		foreach (BusMonitorChannelStatistics statistics in busMonitorControl.ChannelStatistics)
		{
			if (index >= ((ListView)(object)listView).Columns.Count)
			{
				continue;
			}
			UpdateItem(dataFramesTotalItem, index, statistics.TotalFrames);
			UpdateItem(filteredFramesTotalItem, index, statistics.TotalFilteredFrames);
			UpdateItem(errorFramesTotalItem, index, statistics.TotalErrorFrames);
			if (flag)
			{
				double? num = null;
				if (statistics.FramesPerSecond.HasValue)
				{
					num = busMonitorControl.CalculateBusload(statistics.FramesPerSecond.Value);
				}
				((ListViewItem)(object)busloadItem).SubItems[index].Text = (num.HasValue ? num.Value.ToString("F2", CultureInfo.CurrentCulture) : Resources.BusMonitorStatisticsForm_NotAvailable);
				UpdateItem(burstsTotalItem, index, statistics.TotalBursts);
				UpdateItem(burstTimeItem, index, statistics.BurstInterval);
				UpdateItem(framesPerBurst, index, statistics.FramesPerBurst);
				UpdateItem(dataFramesPerSecItem, index, statistics.FramesPerSecond);
				UpdateItem(filteredFramesPerSecItem, index, statistics.FilteredFramesPerSecond);
				UpdateItem(errorFramesPerSecItem, index, statistics.ErrorFramesPerSecond);
			}
			UpdateIdentifierCount(identifierFramesTotalItems, (string id) => statistics.GetIdentifierFrameCount(id), statistics.TotalFrames);
			if (flag)
			{
				UpdateIdentifierCount(identifierFramesPerSecItems, (string id) => statistics.GetIdentifierFrameCountPerSecond(id), statistics.FramesPerSecond);
			}
			index++;
		}
		((Control)(object)listView).Update();
		void UpdateIdentifierCount(Dictionary<string, ListViewExGroupItem> targetItems, Func<string, long?> getCount, long? total)
		{
			foreach (KeyValuePair<string, ListViewExGroupItem> targetItem in targetItems)
			{
				long? value = getCount(targetItem.Key);
				if (value.HasValue)
				{
					UpdateItemPercentage(targetItem.Value, index, value, total);
				}
				else
				{
					UpdateItem(targetItem.Value, index, null);
				}
			}
		}
	}

	private static string GetDisplayChipState(ChipStates? chipState)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Expected I4, but got Unknown
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Invalid comparison between Unknown and I4
		if (chipState.HasValue)
		{
			ChipStates? val = chipState;
			if (val.HasValue)
			{
				ChipStates valueOrDefault = val.GetValueOrDefault();
				switch (valueOrDefault - 1)
				{
				default:
					if ((int)valueOrDefault == 8)
					{
						return Resources.BusMonitorStatisticsForm_ChipStateActive;
					}
					break;
				case 1:
					return Resources.BusMonitorStatisticsForm_ChipStatePassive;
				case 3:
					return Resources.BusMonitorStatisticsForm_ChipStateWarning;
				case 0:
					return Resources.BusMonitorStatisticsForm_ChipStateBusoff;
				case 2:
					break;
				}
			}
		}
		return Resources.BusMonitorStatisticsForm_NotAvailable;
	}

	private void UpdateChipState()
	{
		if (!busMonitorControl.ChannelStatistics.Any((BusMonitorChannelStatistics cs) => cs.HasMonitor))
		{
			return;
		}
		int num = 1;
		foreach (BusMonitorChannelStatistics channelStatistic in busMonitorControl.ChannelStatistics)
		{
			if (num < ((ListView)(object)listView).Columns.Count)
			{
				((ListViewItem)(object)chipStateItem).SubItems[num].Text = GetDisplayChipState(channelStatistic.ChipState);
				UpdateItem(txErrorCountItem, num, channelStatistic.TXErrorCount);
				UpdateItem(rxErrorCountItem, num, channelStatistic.RXErrorCount);
				num++;
			}
		}
		((Control)(object)listView).Update();
	}

	private static void UpdateItem(ListViewExGroupItem item, int index, long? value)
	{
		((ListViewItem)(object)item).SubItems[index].Text = (value.HasValue ? value.Value.ToString(CultureInfo.CurrentCulture) : Resources.BusMonitorStatisticsForm_NotAvailable);
	}

	private static void UpdateItemPercentage(ListViewExGroupItem item, int index, long? value, long? total)
	{
		string text = Resources.BusMonitorStatisticsForm_NotAvailable;
		if (value.HasValue)
		{
			text = value.Value.ToString(CultureInfo.CurrentCulture) + (total.HasValue ? string.Format(CultureInfo.InvariantCulture, " [{0:0.0}%]", (double)value.Value / (double?)total * 100.0) : string.Empty);
		}
		((ListViewItem)(object)item).SubItems[index].Text = text;
	}

	private ListViewExGroupItem AddItem(string name, ListViewExGroupItem parent = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		ListViewExGroupItem val = new ListViewExGroupItem(name);
		if (parent == null)
		{
			((ListView)(object)listView).Items.Add((ListViewItem)(object)val);
		}
		else
		{
			parent.Expanded = true;
			parent.Add(val);
		}
		for (int i = 0; i < ((ListView)(object)listView).Columns.Count - 1; i++)
		{
			((ListViewItem)(object)val).SubItems.Add(string.Empty);
		}
		return val;
	}

	protected override void OnClientSizeChanged(EventArgs e)
	{
		base.OnSizeChanged(e);
		int num = ((ListView)(object)listView).Columns.Count - 1;
		int num2 = ((Control)(object)listView).ClientSize.Width - SystemInformation.VerticalScrollBarWidth - SystemInformation.BorderSize.Width * 4;
		columnHeaderStatistic.Width = num2 / (num + 2) * 2;
		foreach (ColumnHeader item in from c in ((ListView)(object)listView).Columns.OfType<ColumnHeader>()
			where c != columnHeaderStatistic
			select c)
		{
			item.Width = num2 / (num + 2);
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			busMonitorControl.PropertyChanged -= BusMonitorControl_PropertyChanged;
			busMonitorControl = null;
			if (components != null)
			{
				components.Dispose();
			}
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected O, but got Unknown
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DetroitDiesel.Windows.Forms.Diagnostics.Container.BusMonitorStatisticsForm));
		this.listView = new ListViewEx();
		this.columnHeaderStatistic = new System.Windows.Forms.ColumnHeader();
		((System.ComponentModel.ISupportInitialize)this.listView).BeginInit();
		base.SuspendLayout();
		this.listView.CanDelete = false;
		((System.Windows.Forms.ListView)(object)this.listView).Columns.AddRange(new System.Windows.Forms.ColumnHeader[1] { this.columnHeaderStatistic });
		resources.ApplyResources(this.listView, "listView");
		this.listView.EditableColumn = -1;
		this.listView.GridLines = true;
		((System.Windows.Forms.Control)(object)this.listView).Name = "listView";
		this.listView.ShowGlyphs = (GlyphBehavior)1;
		this.listView.ShowItemImages = (ImageBehavior)1;
		this.listView.ShowStateImages = (ImageBehavior)1;
		((System.Windows.Forms.ListView)(object)this.listView).UseCompatibleStateImageBehavior = false;
		resources.ApplyResources(this.columnHeaderStatistic, "columnHeaderStatistic");
		resources.ApplyResources(this, "$this");
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add((System.Windows.Forms.Control)(object)this.listView);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
		base.Name = "BusMonitorStatisticsForm";
		((System.ComponentModel.ISupportInitialize)this.listView).EndInit();
		base.ResumeLayout(false);
	}
}
