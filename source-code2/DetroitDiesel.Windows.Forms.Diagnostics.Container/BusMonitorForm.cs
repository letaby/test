using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.CrashHandling;
using DetroitDiesel.Settings;
using DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties;
using SapiLayer1;

namespace DetroitDiesel.Windows.Forms.Diagnostics.Container;

public class BusMonitorForm : Form
{
	private static class NativeMethods
	{
		public const int GWL_HWNDPARENT = -8;

		public static IntPtr SetWindowLongPtr(HandleRef hWnd, int nIndex, IntPtr dwNewLong)
		{
			if (IntPtr.Size == 8)
			{
				return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
			}
			return new IntPtr(SetWindowLong32(hWnd, nIndex, dwNewLong.ToInt32()));
		}

		[DllImport("user32.dll", EntryPoint = "SetWindowLong")]
		private static extern int SetWindowLong32(HandleRef hWnd, int nIndex, int dwNewLong);

		[DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
		private static extern IntPtr SetWindowLongPtr64(HandleRef hWnd, int nIndex, IntPtr dwNewLong);
	}

	private IntPtr handle;

	private Form mainForm;

	private ManualResetEvent formStartedEvent;

	private static BusMonitorForm globalInstance;

	private BusMonitorStatisticsForm statisticsForm;

	private System.Timers.Timer searchTimer;

	private const int TimeForEvent = 50;

	private const int FramesForEvent = 500;

	private IContainer components;

	private ToolStrip toolStrip;

	private ToolStripButton toolStripButtonStart;

	private ToolStripButton toolStripButtonStop;

	private ToolStripComboBox toolStripComboBoxResources;

	private ToolStripSeparator toolStripSeparator1;

	private ToolStripButton toolStripButtonShowEcuNames;

	private ToolStripButton toolStripButtonShowCompletePacket;

	private ToolStripComboBox toolStripComboBoxEcuFilter;

	private ToolStripButton toolStripButtonPause;

	private BusMonitorControl busMonitorControl;

	private ToolStripSeparator toolStripSeparator2;

	private ToolStripButton toolStripButtonLoad;

	private ToolStripButton toolStripButtonSave;

	private ToolStripButton toolStripButtonTimestampDisplay;

	private TableLayoutPanel tableLayoutPanelProperties;

	private Label labelTitleLabel;

	private Label labelStartTimeLabel;

	private Label labelConnectionResourceLabel;

	private Label labelTitle;

	private Label labelStartTime;

	private Label labelConnectionResource;

	private ToolStripButton toolStripButtonSettings;

	private ToolStripSeparator toolStripSeparator3;

	private ToolStripButton toolStripButtonSymbolic;

	private ToolStripButton toolStripButtonClearContents;

	private ToolStripLabel toolStripLabelFilter;

	private ToolStripTextBox toolStripTextBoxFilter;

	private ToolStripButton toolStripButtonStatistics;

	private ToolStripSeparator toolStripSeparator4;

	private BusMonitorForm(ManualResetEvent formStartedEvent)
	{
		this.formStartedEvent = formStartedEvent;
		Font = SystemFonts.MessageBoxFont;
		InitializeComponent();
		toolStrip.Items.Remove(toolStripButtonSymbolic);
		toolStrip.Items.Remove(toolStripButtonShowEcuNames);
		toolStrip.Items.Remove(toolStripSeparator4);
		toolStrip.Items.Remove(toolStripButtonStatistics);
		using (Graphics graphics = CreateGraphics())
		{
			foreach (ToolStrip item in base.Controls.OfType<ToolStrip>())
			{
				item.AutoSize = false;
				item.ImageScalingSize = new Size((int)(20f * graphics.DpiX / 96f), (int)(20f * graphics.DpiY / 96f));
				item.AutoSize = true;
			}
		}
		mainForm = Application.OpenForms[0];
		mainForm.FormClosing += mainForm_FormClosing;
		SapiManager.GlobalInstance.SapiResetCompleted += GlobalInstance_SapiResetCompleted;
		busMonitorControl.PropertyChanged += busMonitorControl_PropertyChanged;
		busMonitorControl.MonitorException += busMonitorControl_MonitorException;
	}

	private void mainForm_FormClosing(object sender, FormClosingEventArgs e)
	{
		BeginInvoke(new Action(base.Close));
	}

	public new static void Show()
	{
		EnsureCreated();
	}

	private static void EnsureCreated()
	{
		if (globalInstance == null)
		{
			ManualResetEvent manualResetEvent = new ManualResetEvent(initialState: false);
			Thread thread = new Thread(BusMonitorFormThreadFunc);
			thread.SetApartmentState(ApartmentState.STA);
			thread.Start(manualResetEvent);
			manualResetEvent.WaitOne();
			globalInstance.SetOwner();
		}
		else
		{
			globalInstance.Activate();
		}
	}

	private static void BusMonitorFormThreadFunc(object formStartedEvent)
	{
		CrashHandler.Initialize();
		globalInstance = new BusMonitorForm(formStartedEvent as ManualResetEvent);
		Application.Run(globalInstance);
		globalInstance = null;
	}

	private void SetOwner()
	{
		NativeMethods.SetWindowLongPtr(new HandleRef(this, handle), -8, mainForm.Handle);
	}

	protected override void OnLoad(EventArgs e)
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Expected O, but got Unknown
		base.OnLoad(e);
		handle = base.Handle;
		formStartedEvent.Set();
		if (!base.DesignMode)
		{
			Sapi sapi = SapiManager.GlobalInstance.Sapi;
			((ChannelBaseCollection)sapi.Channels).ConnectCompleteEvent += new ConnectCompleteEventHandler(Channels_ConnectCompleteEvent);
			ResetIdentifierMap();
			base.KeyPreview = true;
			toolStripComboBoxResources.Items.AddRange(new string[3] { "CAN1", "CAN2", "ETHERNET" });
			LoadSettings();
			UpdateUserInterface();
			UpdateMonitorProperties();
			searchTimer = new System.Timers.Timer(1000.0);
			searchTimer.SynchronizingObject = this;
			searchTimer.AutoReset = false;
			searchTimer.Elapsed += SearchTimer_Elapsed;
		}
	}

	protected override void OnKeyDown(KeyEventArgs e)
	{
		switch (e.KeyCode)
		{
		case Keys.MediaPlayPause:
			if (toolStripButtonStart.Enabled)
			{
				toolStripButtonStart_Click(this, new EventArgs());
			}
			else if (toolStripButtonPause.Enabled)
			{
				toolStripButtonPause_Click(this, new EventArgs());
			}
			break;
		case Keys.MediaStop:
			if (toolStripButtonStop.Enabled)
			{
				toolStripButtonStop_Click(this, new EventArgs());
			}
			break;
		}
		base.OnKeyDown(e);
	}

	private void Channels_ConnectCompleteEvent(object sender, ResultEventArgs e)
	{
		if (base.InvokeRequired)
		{
			BeginInvoke(new EventHandler<ResultEventArgs>(Channels_ConnectCompleteEvent), sender, e);
			return;
		}
		Channel val = (Channel)((sender is Channel) ? sender : null);
		if (e.Succeeded && val != null && !busMonitorControl.ContentIsFromFile && IsSupported(val.Ecu))
		{
			busMonitorControl.AddKnownChannels(Enumerable.Repeat<Channel>(val, 1));
		}
	}

	protected override void OnFormClosing(FormClosingEventArgs e)
	{
		if (busMonitorControl.HasMonitor)
		{
			busMonitorControl.StopMonitor();
		}
		mainForm.FormClosing -= mainForm_FormClosing;
		SaveSettings();
		base.OnFormClosing(e);
	}

	public new void Activate()
	{
		if (base.InvokeRequired)
		{
			BeginInvoke(new Action(Activate));
		}
		else
		{
			base.Activate();
		}
	}

	private void GlobalInstance_SapiResetCompleted(object sender, EventArgs e)
	{
		if (base.InvokeRequired)
		{
			BeginInvoke(new EventHandler(GlobalInstance_SapiResetCompleted), sender, e);
		}
		else
		{
			ResetIdentifierMap();
		}
	}

	private void ResetIdentifierMap()
	{
		busMonitorControl.ClearKnownChannels();
		busMonitorControl.AddKnownChannels(((IEnumerable<Channel>)SapiManager.GlobalInstance.Sapi.Channels).Where((Channel c) => c.Online && IsSupported(c.Ecu)));
	}

	private void SelectEcu(Ecu ecu)
	{
		for (int i = 0; i < toolStripComboBoxEcuFilter.Items.Count; i++)
		{
			object obj = toolStripComboBoxEcuFilter.Items[i];
			Ecu val = (Ecu)((obj is Ecu) ? obj : null);
			if (val != null && val.Identifier == ecu.Identifier)
			{
				toolStripComboBoxEcuFilter.SelectedItem = val;
				break;
			}
		}
	}

	internal static int GetPriority(string identifier)
	{
		string[] array = identifier.Split("-".ToCharArray());
		if (array.Length > 1 && int.TryParse(array[1], out var result))
		{
			if (!(array[0] == "UDS"))
			{
				return result + 255;
			}
			return result;
		}
		return int.MaxValue;
	}

	private void UpdateEcuCombo()
	{
		string s = toolStripComboBoxEcuFilter.Text;
		object selectedItem = toolStripComboBoxEcuFilter.SelectedItem;
		Ecu val = (Ecu)((selectedItem is Ecu) ? selectedItem : null);
		toolStripComboBoxEcuFilter.Items.Clear();
		toolStripComboBoxEcuFilter.Items.Add(Resources.BusMonitorForm_MonitorAll);
		toolStripComboBoxEcuFilter.Items.Add(Resources.BusMonitorForm_MonitorAllKnown);
		foreach (string identifier in busMonitorControl.KnownIdentifiers.OrderBy((string i) => GetPriority(i)))
		{
			Channel val2 = busMonitorControl.KnownChannels.FirstOrDefault((Channel c) => c.Ecu.Identifier == identifier);
			toolStripComboBoxEcuFilter.Items.Add((val2 != null) ? ((string)(object)val2.Ecu) : identifier);
		}
		if (val != null)
		{
			SelectEcu(val);
			if (toolStripComboBoxEcuFilter.SelectedItem == null)
			{
				toolStripComboBoxEcuFilter.SelectedIndex = 1;
			}
		}
		else
		{
			int num = toolStripComboBoxEcuFilter.FindStringExact(s);
			toolStripComboBoxEcuFilter.SelectedIndex = ((num != -1) ? num : 0);
		}
	}

	private void UpdateUserInterface()
	{
		Sapi sapi = Sapi.GetSapi();
		toolStripButtonStart.Enabled = (!busMonitorControl.HasMonitor && !((IEnumerable<BusMonitor>)sapi.BusMonitors).Any()) || busMonitorControl.Paused;
		toolStripButtonStop.Enabled = busMonitorControl.HasMonitor;
		toolStripButtonPause.Enabled = busMonitorControl.HasMonitor && !busMonitorControl.Paused;
		toolStripComboBoxResources.Enabled = !busMonitorControl.HasMonitor;
		toolStripButtonLoad.Enabled = !busMonitorControl.HasMonitor;
		toolStripButtonSave.Enabled = busMonitorControl.HasContent && !busMonitorControl.ContentIsFromFile && !busMonitorControl.HasMonitor;
		toolStripButtonClearContents.Enabled = busMonitorControl.HasContent && !busMonitorControl.HasMonitor;
		toolStripButtonSettings.Enabled = !busMonitorControl.HasMonitor;
		UpdateTitle();
	}

	private void UpdateTitle()
	{
		Text = string.Format(CultureInfo.CurrentCulture, Resources.BusMonitorForm_Title, busMonitorControl.ContentIsFromFile ? Path.GetFileName(busMonitorControl.ContentFromFile) : ((!busMonitorControl.HasMonitor) ? Resources.BusMonitorForm_Stopped : (busMonitorControl.Paused ? Resources.BusMonitorForm_Paused : Resources.BusMonitorForm_RunningNoFrames)));
	}

	private static bool IsSupported(Ecu ecu)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Invalid comparison between Unknown and I4
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Invalid comparison between Unknown and I4
		if ((int)ecu.DiagnosisSource != 0 && (int)ecu.DiagnosisSource != 2)
		{
			return (int)ecu.DiagnosisSource == 5;
		}
		return true;
	}

	private void toolStripButtonStart_Click(object sender, EventArgs e)
	{
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Expected O, but got Unknown
		if (busMonitorControl.Paused)
		{
			busMonitorControl.Paused = false;
			return;
		}
		string text = toolStripComboBoxResources.Text;
		if (busMonitorControl.ContentIsFromFile || busMonitorControl.IsEthernet || (busMonitorControl.ConnectionProperties.ContainsKey("PhysicalInterfaceLink") && busMonitorControl.ConnectionProperties["PhysicalInterfaceLink"] != text))
		{
			if (!busMonitorControl.ContentIsFromFile && PromptContentClear())
			{
				return;
			}
			busMonitorControl.Clear();
			ResetIdentifierMap();
		}
		try
		{
			if (busMonitorControl.StartMonitor(text, 50, 500))
			{
				labelStartTime.Text = busMonitorControl.StartTime.ToString(CultureInfo.CurrentCulture);
				return;
			}
			string value = SettingsManager.GlobalInstance.GetValue<StringSetting>(text, "BusMonitorWindow", new StringSetting()).Value;
			if (!string.IsNullOrEmpty(value))
			{
				ControlHelpers.ShowMessageBox((Control)this, string.Format(CultureInfo.CurrentCulture, Resources.BusMonitorForm_CannotLocateInterfaceWithConfiguredSetting, toolStripComboBoxResources.Text, value), MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
			}
			else
			{
				ControlHelpers.ShowMessageBox((Control)this, string.Format(CultureInfo.CurrentCulture, Resources.BusMonitorForm_CannotLocateInterfaceNoConfiguredSetting, toolStripComboBoxResources.Text), MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
			}
		}
		catch (Exception ex) when (ex is CaesarException || ex is DllNotFoundException || ex is BadImageFormatException || ex is InvalidOperationException)
		{
			ControlHelpers.ShowMessageBox((Control)this, ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
		}
	}

	private void toolStripButtonPause_Click(object sender, EventArgs e)
	{
		busMonitorControl.Paused = true;
	}

	private void toolStripButtonStop_Click(object sender, EventArgs e)
	{
		busMonitorControl.StopMonitor();
	}

	private void busMonitorControl_PropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		switch (e.PropertyName)
		{
		case "HasMonitor":
		case "Paused":
		case "HasContent":
			UpdateUserInterface();
			break;
		case "KnownChannels":
		case "KnownIdentifiers":
			UpdateEcuCombo();
			break;
		case "Title":
		case "ConnectionProperties":
			UpdateMonitorProperties();
			break;
		case "IsEthernet":
			toolStripButtonShowCompletePacket.Enabled = busMonitorControl.IsEthernet;
			break;
		}
	}

	private void busMonitorControl_MonitorException(object sender, ResultEventArgs e)
	{
		ControlHelpers.ShowMessageBox((Control)this, string.Format(CultureInfo.CurrentCulture, Resources.BusMonitorForm_MonitorException, e.Exception.Message), MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
	}

	private void UpdateMonitorProperties()
	{
		if (busMonitorControl.StartTime != DateTime.MinValue)
		{
			labelStartTime.Text = busMonitorControl.StartTime.ToString(CultureInfo.CurrentCulture);
			Label label = labelStartTime;
			bool visible = (labelStartTimeLabel.Visible = true);
			label.Visible = visible;
		}
		else
		{
			Label label2 = labelStartTime;
			bool visible = (labelStartTimeLabel.Visible = false);
			label2.Visible = visible;
		}
		if (busMonitorControl.ContentIsFromFile && !string.IsNullOrEmpty(busMonitorControl.Title))
		{
			labelTitle.Text = busMonitorControl.Title;
			Label label3 = labelTitle;
			bool visible = (labelTitleLabel.Visible = true);
			label3.Visible = visible;
		}
		else
		{
			Label label4 = labelTitle;
			bool visible = (labelTitleLabel.Visible = false);
			label4.Visible = visible;
		}
		if (busMonitorControl.ConnectionProperties.Count > 0)
		{
			if (busMonitorControl.ConnectionProperties.ContainsKey("BaudRate"))
			{
				labelConnectionResource.Text = string.Format(CultureInfo.CurrentCulture, Resources.BusMonitorForm_ConnectionResourceWithBaud, busMonitorControl.ConnectionProperties["PhysicalInterfaceLink"], busMonitorControl.ConnectionProperties["InterfaceBoard"], busMonitorControl.ConnectionProperties["BaudRate"]);
			}
			else
			{
				labelConnectionResource.Text = string.Format(CultureInfo.CurrentCulture, Resources.BusMonitorForm_ConnectionResource, busMonitorControl.ConnectionProperties["PhysicalInterfaceLink"], busMonitorControl.ConnectionProperties["InterfaceBoard"]);
			}
			Label label5 = labelConnectionResource;
			bool visible = (labelConnectionResourceLabel.Visible = true);
			label5.Visible = visible;
		}
		else
		{
			Label label6 = labelConnectionResource;
			bool visible = (labelConnectionResourceLabel.Visible = false);
			label6.Visible = visible;
		}
	}

	private void toolStripButtonShowEcuNames_CheckStateChanged(object sender, EventArgs e)
	{
		busMonitorControl.ShowEcuNames = toolStripButtonShowEcuNames.Checked;
	}

	private void toolStripButtonShowCompletePacket_CheckStateChanged(object sender, EventArgs e)
	{
		busMonitorControl.ShowCompletePacket = toolStripButtonShowCompletePacket.Checked;
	}

	private void toolStripButtonSymbolic_CheckStateChanged(object sender, EventArgs e)
	{
		busMonitorControl.ShowSymbolic = toolStripButtonSymbolic.Checked;
		int num = (toolStripButtonSymbolic.Checked ? (base.ClientSize.Width * 3 / 2) : (base.ClientSize.Width / 3 * 2));
		base.ClientSize = new Size(num, base.ClientSize.Height);
	}

	private void toolStripButtonTimestampDisplay_CheckStateChanged(object sender, EventArgs e)
	{
		busMonitorControl.DisplayAbsoluteTimestamp = toolStripButtonTimestampDisplay.Checked;
	}

	private void toolStripComboBoxEcuFilter_SelectedIndexChanged(object sender, EventArgs e)
	{
		switch (toolStripComboBoxEcuFilter.SelectedIndex)
		{
		case 0:
			busMonitorControl.Filter = (BusMonitorControlFilterType)0;
			break;
		case 1:
			busMonitorControl.Filter = (BusMonitorControlFilterType)1;
			break;
		default:
			busMonitorControl.FilterEcu = toolStripComboBoxEcuFilter.Text;
			break;
		}
	}

	private void LoadSettings()
	{
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Expected O, but got Unknown
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Expected O, but got Unknown
		Point savedLocation = SettingsManager.GlobalInstance.GetValue<Point>("Location", "BusMonitorWindow", base.Location);
		if (!Screen.AllScreens.Any((Screen s) => s.Bounds.Contains(savedLocation)))
		{
			savedLocation = Screen.PrimaryScreen.WorkingArea.Location;
		}
		Rectangle rectangle = new Rectangle(savedLocation, SettingsManager.GlobalInstance.GetValue<Size>("Size", "BusMonitorWindow", base.Size));
		rectangle.Intersect(Screen.GetWorkingArea(rectangle));
		base.Bounds = rectangle;
		toolStripButtonTimestampDisplay.Checked = SettingsManager.GlobalInstance.GetValue<bool>("ShowTimeAsAbsolute", "BusMonitorWindow", false);
		StringSetting filterSetting = SettingsManager.GlobalInstance.GetValue<StringSetting>("Filter", "BusMonitorWindow", new StringSetting(Resources.BusMonitorForm_MonitorAll));
		Ecu val = ((IEnumerable<Ecu>)SapiManager.GlobalInstance.Sapi.Ecus).FirstOrDefault((Ecu e) => e.Name == filterSetting.Value);
		if (val != null)
		{
			SelectEcu(val);
		}
		else
		{
			toolStripComboBoxEcuFilter.SelectedIndex = toolStripComboBoxEcuFilter.FindStringExact(filterSetting.Value);
		}
		if (toolStripComboBoxEcuFilter.SelectedIndex == -1)
		{
			toolStripComboBoxEcuFilter.SelectedIndex = 0;
		}
		StringSetting value = SettingsManager.GlobalInstance.GetValue<StringSetting>("Resource", "BusMonitorWindow", new StringSetting());
		if (value.Value != null)
		{
			toolStripComboBoxResources.SelectedIndex = toolStripComboBoxResources.FindString(value.Value);
		}
		if (toolStripComboBoxResources.SelectedIndex == -1 && toolStripComboBoxResources.Items.Count > 0)
		{
			toolStripComboBoxResources.SelectedIndex = 0;
		}
	}

	private void SaveSettings()
	{
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Expected O, but got Unknown
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Expected O, but got Unknown
		SettingsManager.GlobalInstance.SetValue<Point>("Location", "BusMonitorWindow", base.Location, false);
		SettingsManager.GlobalInstance.SetValue<Size>("Size", "BusMonitorWindow", base.Size, false);
		SettingsManager.GlobalInstance.SetValue<bool>("ShowTimeAsAbsolute", "BusMonitorWindow", toolStripButtonTimestampDisplay.Checked, false);
		SettingsManager.GlobalInstance.SetValue<StringSetting>("Filter", "BusMonitorWindow", new StringSetting(toolStripComboBoxEcuFilter.Text), false);
		SettingsManager.GlobalInstance.SetValue<StringSetting>("Resource", "BusMonitorWindow", new StringSetting(toolStripComboBoxResources.Text), false);
	}

	private void toolStripComboBoxResources_DropDown(object sender, EventArgs e)
	{
		AdjustComboDropDownWidth(sender as ToolStripComboBox);
	}

	private void toolStripComboBoxEcuFilter_DropDown(object sender, EventArgs e)
	{
		AdjustComboDropDownWidth(sender as ToolStripComboBox);
	}

	private static void AdjustComboDropDownWidth(ToolStripComboBox senderComboBox)
	{
		int num = senderComboBox.DropDownWidth;
		int num2 = ((senderComboBox.Items.Count > senderComboBox.MaxDropDownItems) ? SystemInformation.VerticalScrollBarWidth : 0);
		foreach (string item in from object item in senderComboBox.Items
			select item.ToString())
		{
			int num3 = TextRenderer.MeasureText(item, senderComboBox.Font).Width + num2;
			if (num < num3)
			{
				num = num3;
			}
		}
		senderComboBox.DropDownWidth = num;
	}

	private bool PromptContentClear()
	{
		if (busMonitorControl.HasContent && !busMonitorControl.ContentIsFromFile && ControlHelpers.ShowMessageBox((Control)this, Resources.BusMonitorForm_ContentClearPrompt, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.No)
		{
			return true;
		}
		return false;
	}

	private void toolStripButtonLoad_Click(object sender, EventArgs e)
	{
		if (!PromptContentClear())
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.DefaultExt = "trc";
			openFileDialog.Filter = Resources.BusMonitor_OpenFileDialogFilter;
			openFileDialog.Title = Resources.BusMonitor_OpenFileDialogTitle;
			DialogResult dialogResult = openFileDialog.ShowDialog(this);
			if (dialogResult == DialogResult.OK)
			{
				Application.UseWaitCursor = true;
				busMonitorControl.Open(openFileDialog.FileName);
				Application.UseWaitCursor = false;
			}
		}
	}

	private void toolStripButtonSave_Click(object sender, EventArgs e)
	{
		SaveFileDialog saveFileDialog = new SaveFileDialog();
		if (busMonitorControl.IsEthernet)
		{
			saveFileDialog.DefaultExt = "pcapng";
			saveFileDialog.Filter = Resources.BusMonitor_SaveFileDialogFilterEthernet;
		}
		else
		{
			saveFileDialog.DefaultExt = "trc";
			saveFileDialog.Filter = Resources.BusMonitor_SaveFileDialogFilterCAN;
		}
		saveFileDialog.Title = Resources.BusMonitor_SaveFileDialogTitle;
		DialogResult dialogResult = saveFileDialog.ShowDialog(this);
		if (dialogResult == DialogResult.OK)
		{
			Application.UseWaitCursor = true;
			busMonitorControl.Save(saveFileDialog.FileName);
			Application.UseWaitCursor = false;
		}
	}

	private void toolStripButtonSettings_Click(object sender, EventArgs e)
	{
		BusMonitorSettingsForm busMonitorSettingsForm = new BusMonitorSettingsForm();
		busMonitorSettingsForm.ShowDialog();
	}

	private void toolStripButtonClearContents_Click(object sender, EventArgs e)
	{
		if (!PromptContentClear())
		{
			busMonitorControl.Clear();
			ResetIdentifierMap();
		}
	}

	private void toolStripTextBoxFilter_TextChanged(object sender, EventArgs e)
	{
		if (searchTimer.Enabled)
		{
			searchTimer.Stop();
		}
		searchTimer.Start();
	}

	private void SearchTimer_Elapsed(object sender, ElapsedEventArgs e)
	{
		searchTimer.Stop();
		busMonitorControl.FilterMessageDescription = toolStripTextBoxFilter.Text;
	}

	private void toolStripButtonStatistics_Click(object sender, EventArgs e)
	{
	}

	protected override void Dispose(bool disposing)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Expected O, but got Unknown
		if (disposing)
		{
			Sapi sapi = SapiManager.GlobalInstance.Sapi;
			if (sapi != null)
			{
				((ChannelBaseCollection)sapi.Channels).ConnectCompleteEvent -= new ConnectCompleteEventHandler(Channels_ConnectCompleteEvent);
			}
			SapiManager.GlobalInstance.SapiResetCompleted -= GlobalInstance_SapiResetCompleted;
			if (statisticsForm != null)
			{
				statisticsForm.Close();
				statisticsForm = null;
			}
			if (busMonitorControl != null)
			{
				busMonitorControl.PropertyChanged -= busMonitorControl_PropertyChanged;
				busMonitorControl.MonitorException -= busMonitorControl_MonitorException;
				busMonitorControl.StopMonitor();
			}
			if (searchTimer != null)
			{
				searchTimer.Elapsed -= SearchTimer_Elapsed;
				searchTimer.Dispose();
				searchTimer = null;
			}
			if (components != null)
			{
				components.Dispose();
			}
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Expected O, but got Unknown
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DetroitDiesel.Windows.Forms.Diagnostics.Container.BusMonitorForm));
		this.toolStrip = new System.Windows.Forms.ToolStrip();
		this.toolStripButtonClearContents = new System.Windows.Forms.ToolStripButton();
		this.toolStripButtonStart = new System.Windows.Forms.ToolStripButton();
		this.toolStripButtonPause = new System.Windows.Forms.ToolStripButton();
		this.toolStripButtonStop = new System.Windows.Forms.ToolStripButton();
		this.toolStripComboBoxResources = new System.Windows.Forms.ToolStripComboBox();
		this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
		this.toolStripButtonLoad = new System.Windows.Forms.ToolStripButton();
		this.toolStripButtonSave = new System.Windows.Forms.ToolStripButton();
		this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
		this.toolStripButtonTimestampDisplay = new System.Windows.Forms.ToolStripButton();
		this.toolStripButtonShowEcuNames = new System.Windows.Forms.ToolStripButton();
		this.toolStripButtonShowCompletePacket = new System.Windows.Forms.ToolStripButton();
		this.toolStripComboBoxEcuFilter = new System.Windows.Forms.ToolStripComboBox();
		this.toolStripLabelFilter = new System.Windows.Forms.ToolStripLabel();
		this.toolStripTextBoxFilter = new System.Windows.Forms.ToolStripTextBox();
		this.toolStripButtonSymbolic = new System.Windows.Forms.ToolStripButton();
		this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
		this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
		this.toolStripButtonSettings = new System.Windows.Forms.ToolStripButton();
		this.tableLayoutPanelProperties = new System.Windows.Forms.TableLayoutPanel();
		this.toolStripButtonStatistics = new System.Windows.Forms.ToolStripButton();
		this.labelTitleLabel = new System.Windows.Forms.Label();
		this.labelStartTimeLabel = new System.Windows.Forms.Label();
		this.labelConnectionResourceLabel = new System.Windows.Forms.Label();
		this.labelTitle = new System.Windows.Forms.Label();
		this.labelStartTime = new System.Windows.Forms.Label();
		this.labelConnectionResource = new System.Windows.Forms.Label();
		this.busMonitorControl = new BusMonitorControl();
		this.toolStrip.SuspendLayout();
		this.tableLayoutPanelProperties.SuspendLayout();
		base.SuspendLayout();
		this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[20]
		{
			this.toolStripButtonClearContents, this.toolStripButtonStart, this.toolStripButtonPause, this.toolStripButtonStop, this.toolStripComboBoxResources, this.toolStripSeparator1, this.toolStripButtonLoad, this.toolStripButtonSave, this.toolStripSeparator2, this.toolStripButtonTimestampDisplay,
			this.toolStripButtonShowEcuNames, this.toolStripComboBoxEcuFilter, this.toolStripLabelFilter, this.toolStripTextBoxFilter, this.toolStripButtonShowCompletePacket, this.toolStripButtonSymbolic, this.toolStripSeparator3, this.toolStripButtonSettings, this.toolStripSeparator4, this.toolStripButtonStatistics
		});
		resources.ApplyResources(this.toolStrip, "toolStrip");
		this.toolStrip.Name = "toolStrip";
		this.toolStripButtonClearContents.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.toolStripButtonClearContents.Image = DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties.Resources.document_plain;
		resources.ApplyResources(this.toolStripButtonClearContents, "toolStripButtonClearContents");
		this.toolStripButtonClearContents.Name = "toolStripButtonClearContents";
		this.toolStripButtonClearContents.Click += new System.EventHandler(toolStripButtonClearContents_Click);
		this.toolStripButtonStart.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.toolStripButtonStart.Image = DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties.Resources.log_play;
		resources.ApplyResources(this.toolStripButtonStart, "toolStripButtonStart");
		this.toolStripButtonStart.Name = "toolStripButtonStart";
		this.toolStripButtonStart.Click += new System.EventHandler(toolStripButtonStart_Click);
		this.toolStripButtonPause.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.toolStripButtonPause.Image = DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties.Resources.log_pause;
		resources.ApplyResources(this.toolStripButtonPause, "toolStripButtonPause");
		this.toolStripButtonPause.Name = "toolStripButtonPause";
		this.toolStripButtonPause.Click += new System.EventHandler(toolStripButtonPause_Click);
		this.toolStripButtonStop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.toolStripButtonStop.Image = DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties.Resources.log_stop;
		resources.ApplyResources(this.toolStripButtonStop, "toolStripButtonStop");
		this.toolStripButtonStop.Name = "toolStripButtonStop";
		this.toolStripButtonStop.Click += new System.EventHandler(toolStripButtonStop_Click);
		this.toolStripComboBoxResources.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.toolStripComboBoxResources.Name = "toolStripComboBoxResources";
		resources.ApplyResources(this.toolStripComboBoxResources, "toolStripComboBoxResources");
		this.toolStripComboBoxResources.DropDown += new System.EventHandler(toolStripComboBoxResources_DropDown);
		this.toolStripSeparator1.Name = "toolStripSeparator1";
		resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
		this.toolStripButtonLoad.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.toolStripButtonLoad.Image = DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties.Resources.folder_out;
		resources.ApplyResources(this.toolStripButtonLoad, "toolStripButtonLoad");
		this.toolStripButtonLoad.Name = "toolStripButtonLoad";
		this.toolStripButtonLoad.Click += new System.EventHandler(toolStripButtonLoad_Click);
		this.toolStripButtonSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.toolStripButtonSave.Image = DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties.Resources.disk_blue;
		resources.ApplyResources(this.toolStripButtonSave, "toolStripButtonSave");
		this.toolStripButtonSave.Name = "toolStripButtonSave";
		this.toolStripButtonSave.Click += new System.EventHandler(toolStripButtonSave_Click);
		this.toolStripSeparator2.Name = "toolStripSeparator2";
		resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
		this.toolStripButtonTimestampDisplay.CheckOnClick = true;
		this.toolStripButtonTimestampDisplay.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.toolStripButtonTimestampDisplay.Image = DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties.Resources.clock_refresh;
		resources.ApplyResources(this.toolStripButtonTimestampDisplay, "toolStripButtonTimestampDisplay");
		this.toolStripButtonTimestampDisplay.Name = "toolStripButtonTimestampDisplay";
		this.toolStripButtonTimestampDisplay.CheckStateChanged += new System.EventHandler(toolStripButtonTimestampDisplay_CheckStateChanged);
		this.toolStripButtonShowEcuNames.Checked = true;
		this.toolStripButtonShowEcuNames.CheckOnClick = true;
		this.toolStripButtonShowEcuNames.CheckState = System.Windows.Forms.CheckState.Checked;
		this.toolStripButtonShowEcuNames.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.toolStripButtonShowEcuNames.Image = DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties.Resources.ViewAll;
		resources.ApplyResources(this.toolStripButtonShowEcuNames, "toolStripButtonShowEcuNames");
		this.toolStripButtonShowEcuNames.Name = "toolStripButtonShowEcuNames";
		this.toolStripButtonShowEcuNames.CheckStateChanged += new System.EventHandler(toolStripButtonShowEcuNames_CheckStateChanged);
		this.toolStripButtonShowCompletePacket.Checked = false;
		this.toolStripButtonShowCompletePacket.CheckOnClick = true;
		this.toolStripButtonShowCompletePacket.CheckState = System.Windows.Forms.CheckState.Unchecked;
		this.toolStripButtonShowCompletePacket.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.toolStripButtonShowCompletePacket.Image = DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties.Resources.row;
		resources.ApplyResources(this.toolStripButtonShowCompletePacket, "toolStripButtonShowCompletePacket");
		this.toolStripButtonShowCompletePacket.Name = "toolStripButtonShowCompletePacket";
		this.toolStripButtonShowCompletePacket.Enabled = false;
		this.toolStripButtonShowCompletePacket.CheckStateChanged += new System.EventHandler(toolStripButtonShowCompletePacket_CheckStateChanged);
		this.toolStripComboBoxEcuFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.toolStripComboBoxEcuFilter.Name = "toolStripComboBoxEcuFilter";
		resources.ApplyResources(this.toolStripComboBoxEcuFilter, "toolStripComboBoxEcuFilter");
		this.toolStripComboBoxEcuFilter.DropDown += new System.EventHandler(toolStripComboBoxEcuFilter_DropDown);
		this.toolStripComboBoxEcuFilter.SelectedIndexChanged += new System.EventHandler(toolStripComboBoxEcuFilter_SelectedIndexChanged);
		this.toolStripLabelFilter.Name = "toolStripLabelFilter";
		resources.ApplyResources(this.toolStripLabelFilter, "toolStripLabelFilter");
		this.toolStripTextBoxFilter.Name = "toolStripTextBoxFilter";
		resources.ApplyResources(this.toolStripTextBoxFilter, "toolStripTextBoxFilter");
		this.toolStripTextBoxFilter.TextChanged += new System.EventHandler(toolStripTextBoxFilter_TextChanged);
		this.toolStripButtonSymbolic.CheckOnClick = true;
		this.toolStripButtonSymbolic.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.toolStripButtonSymbolic.Image = DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties.Resources.about;
		resources.ApplyResources(this.toolStripButtonSymbolic, "toolStripButtonSymbolic");
		this.toolStripButtonSymbolic.Name = "toolStripButtonSymbolic";
		this.toolStripButtonSymbolic.CheckStateChanged += new System.EventHandler(toolStripButtonSymbolic_CheckStateChanged);
		this.toolStripSeparator3.Name = "toolStripSeparator3";
		resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
		this.toolStripSeparator4.Name = "toolStripSeparator4";
		resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
		this.toolStripButtonSettings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.toolStripButtonSettings.Image = DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties.Resources.gear;
		resources.ApplyResources(this.toolStripButtonSettings, "toolStripButtonSettings");
		this.toolStripButtonSettings.Name = "toolStripButtonSettings";
		this.toolStripButtonSettings.Click += new System.EventHandler(toolStripButtonSettings_Click);
		resources.ApplyResources(this.tableLayoutPanelProperties, "tableLayoutPanelProperties");
		this.tableLayoutPanelProperties.Controls.Add(this.labelTitleLabel, 0, 0);
		this.tableLayoutPanelProperties.Controls.Add(this.labelStartTimeLabel, 0, 1);
		this.tableLayoutPanelProperties.Controls.Add(this.labelConnectionResourceLabel, 0, 2);
		this.tableLayoutPanelProperties.Controls.Add(this.labelTitle, 1, 0);
		this.tableLayoutPanelProperties.Controls.Add(this.labelStartTime, 1, 1);
		this.tableLayoutPanelProperties.Controls.Add(this.labelConnectionResource, 1, 2);
		this.tableLayoutPanelProperties.Name = "tableLayoutPanelProperties";
		this.toolStripButtonStatistics.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		resources.ApplyResources(this.toolStripButtonStatistics, "toolStripButtonStatistics");
		this.toolStripButtonStatistics.Name = "toolStripButtonStatistics";
		this.toolStripButtonStatistics.Click += new System.EventHandler(toolStripButtonStatistics_Click);
		resources.ApplyResources(this.labelTitleLabel, "labelTitleLabel");
		this.labelTitleLabel.Name = "labelTitleLabel";
		resources.ApplyResources(this.labelStartTimeLabel, "labelStartTimeLabel");
		this.labelStartTimeLabel.Name = "labelStartTimeLabel";
		resources.ApplyResources(this.labelConnectionResourceLabel, "labelConnectionResourceLabel");
		this.labelConnectionResourceLabel.Name = "labelConnectionResourceLabel";
		resources.ApplyResources(this.labelTitle, "labelTitle");
		this.labelTitle.Name = "labelTitle";
		resources.ApplyResources(this.labelStartTime, "labelStartTime");
		this.labelStartTime.Name = "labelStartTime";
		resources.ApplyResources(this.labelConnectionResource, "labelConnectionResource");
		this.labelConnectionResource.Name = "labelConnectionResource";
		this.busMonitorControl.DisplayAbsoluteTimestamp = false;
		resources.ApplyResources(this.busMonitorControl, "busMonitorControl");
		this.busMonitorControl.Filter = (BusMonitorControlFilterType)0;
		this.busMonitorControl.FilterEcu = null;
		this.busMonitorControl.FilterMessageDescription = null;
		((System.Windows.Forms.Control)(object)this.busMonitorControl).Name = "busMonitorControl";
		this.busMonitorControl.ShowEcuNames = true;
		this.busMonitorControl.ShowSymbolic = false;
		resources.ApplyResources(this, "$this");
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add((System.Windows.Forms.Control)(object)this.busMonitorControl);
		base.Controls.Add(this.tableLayoutPanelProperties);
		base.Controls.Add(this.toolStrip);
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "BusMonitorForm";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		this.toolStrip.ResumeLayout(false);
		this.toolStrip.PerformLayout();
		this.tableLayoutPanelProperties.ResumeLayout(false);
		this.tableLayoutPanelProperties.PerformLayout();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
