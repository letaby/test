using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties;
using SapiLayer1;

namespace DetroitDiesel.Windows.Forms.Diagnostics.Container;

internal class ConnectionOptionsPanel : OptionsPanel
{
	private SapiManager sapi;

	private IContainer components;

	private ComboBox comboBoxHardware;

	private ToolTip toolTip;

	private Button openSidConfigure;

	private GroupBox groupBox1;

	private PictureBox pictureBoxRollcallSupportWarning;

	private Label labelRollcallSupport;

	private CheckBox checkBoxRollCall;

	private Label labelDevice;

	private TextBox textBoxSelectedDevice;

	private TableLayoutPanel tableLayoutPanelVCI;

	private CheckBox checkBoxPowertrainOnly;

	private GroupBox groupSelectionPanel;

	private TableLayoutPanel tableLayoutPanelProprietaryDevices;

	private Button selectNoneButton;

	private Button selectAllButton;

	private Button resetToDefaultsButton;

	private Panel panelTableLayoutAutoConnect;

	private TableLayoutPanel tableLayoutPanelAutoConnect;

	private CheckBox checkBoxUseMcd;

	private CheckBox checkBoxDoIPAutoConnect;

	private TableLayoutPanel tableLayoutPanelRollCall;

	private CheckBox checkBoxAllowAutoBaud;

	public ConnectionOptionsPanel()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Expected O, but got Unknown
		base.MinAccessLevel = 1;
		InitializeComponent();
		sapi = SapiManager.GlobalInstance;
		sapi.Sapi.Ecus.EcusUpdateEvent += new EcusUpdateEventHandler(OnEcusUpdate);
		base.HeaderImage = new Bitmap(typeof(ConnectionOptionsPanel), "option_connection.png");
		tableLayoutPanelAutoConnect.Padding = new Padding(0, 0, SystemInformation.VerticalScrollBarWidth, 0);
	}

	private void OnEcusUpdate(object sender, EventArgs e)
	{
		BuildConnectionList();
	}

	protected override void OnLoad(EventArgs e)
	{
		BuildHardwareList();
		BuildConnectionList();
		UpdateSelectedTranslator();
		checkBoxUseMcd.Checked = SapiManager.GlobalInstance.UseMcd;
		checkBoxUseMcd.Enabled = SapiManager.GlobalInstance.McdAvailable;
		checkBoxDoIPAutoConnect.Visible = ApplicationInformation.CanRollCallDoIP;
		checkBoxDoIPAutoConnect.Enabled = checkBoxUseMcd.Checked;
		if (!checkBoxUseMcd.Checked)
		{
			checkBoxDoIPAutoConnect.Checked = false;
		}
		else
		{
			checkBoxDoIPAutoConnect.Checked = SapiManager.GlobalInstance.DoIPRollCallEnabled;
		}
		checkBoxAllowAutoBaud.Visible = ApplicationInformation.CanDisableAutoBaud;
		checkBoxAllowAutoBaud.Enabled = SapiManager.GlobalInstance.IsAutoBaudCapable;
		checkBoxAllowAutoBaud.Checked = checkBoxAllowAutoBaud.Enabled && SapiManager.GlobalInstance.AllowAutoBaud;
		tableLayoutPanelVCI.SetRowSpan(checkBoxUseMcd, checkBoxAllowAutoBaud.Visible ? 1 : 2);
		base.OnLoad(e);
	}

	private void selectNoneButton_Click(object sender, EventArgs e)
	{
		CheckBox[] array = (from x in tableLayoutPanelAutoConnect.Controls.OfType<CheckBox>()
			where x.Visible && x.Checked
			select x).ToArray();
		CheckBox[] array2 = array;
		foreach (CheckBox box in array2)
		{
			SetCheckState(box, CheckState.Unchecked);
		}
		if (array.Any())
		{
			MarkDirty();
		}
	}

	private void selectAllButton_Click(object sender, EventArgs e)
	{
		CheckBox[] array = (from x in tableLayoutPanelAutoConnect.Controls.OfType<CheckBox>()
			where x.Visible && !x.Checked
			select x).ToArray();
		CheckBox[] array2 = array;
		foreach (CheckBox box in array2)
		{
			SetCheckState(box, CheckState.Checked);
		}
		if (array.Any())
		{
			MarkDirty();
		}
	}

	private void resetToDefaultsButton_Click(object sender, EventArgs e)
	{
		CheckBox[] array = (from x in tableLayoutPanelAutoConnect.Controls.OfType<CheckBox>()
			where x.Visible
			select x).ToArray();
		bool flag = false;
		CheckBox[] array2 = array;
		foreach (CheckBox cb in array2)
		{
			CheckState checkState = (((IEnumerable<Ecu>)Sapi.GetSapi().Ecus).Where((Ecu ecu) => ecu.Identifier == cb.Name).Any((Ecu ecu) => SapiManager.IsDefaultAutoConnect(ecu)) ? CheckState.Checked : CheckState.Unchecked);
			if (cb.CheckState != checkState)
			{
				SetCheckState(cb, checkState);
				flag = true;
			}
		}
		if (flag)
		{
			MarkDirty();
		}
	}

	private void BuildHardwareList()
	{
		comboBoxHardware.Items.Clear();
		foreach (Choice item in (ReadOnlyCollection<Choice>)(object)sapi.HardwareTypes)
		{
			comboBoxHardware.Items.Add(item);
			if (item.Name == sapi.HardwareType)
			{
				comboBoxHardware.SelectedItem = item;
			}
		}
		UpdateSidConfigureButton();
	}

	private bool ConfirmRestartConnection(bool allowCancel)
	{
		if (((sapi.ActiveChannels == null || ((ReadOnlyCollection<Channel>)(object)sapi.ActiveChannels).Count == 0) && (sapi.LogFileAllChannels == null || ((ReadOnlyCollection<Channel>)(object)sapi.LogFileAllChannels).Count == 0)) || MessageBox.Show(Resources.MessageConnectionsAndLogFilesClosedBeforeChangesApplied, ApplicationInformation.ProductName, allowCancel ? MessageBoxButtons.OKCancel : MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2, ControlHelpers.IsRightToLeft((Control)this) ? (MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading) : ((MessageBoxOptions)0)) == DialogResult.OK)
		{
			return true;
		}
		return false;
	}

	public override bool ApplySettings()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected O, but got Unknown
		bool flag = true;
		if (base.IsDirty)
		{
			Cursor.Current = Cursors.WaitCursor;
			bool flag2 = false;
			Choice val = (Choice)comboBoxHardware.SelectedItem;
			if (val.Name != sapi.HardwareType || checkBoxUseMcd.Checked != SapiManager.GlobalInstance.UseMcd)
			{
				if (ConfirmRestartConnection(allowCancel: true))
				{
					flag2 = true;
					if (val.Name != sapi.HardwareType)
					{
						sapi.SetHardwareType(val.Name, false);
					}
					if (checkBoxUseMcd.Checked != SapiManager.GlobalInstance.UseMcd)
					{
						SapiManager.GlobalInstance.UseMcd = checkBoxUseMcd.Checked;
					}
				}
				else
				{
					flag = false;
				}
			}
			if (flag)
			{
				bool flag3 = false;
				foreach (CheckBox item in tableLayoutPanelAutoConnect.Controls.OfType<CheckBox>())
				{
					switch (item.CheckState)
					{
					case CheckState.Checked:
						sapi.SetAutoConnect(item.Name, true, false);
						flag3 = true;
						break;
					default:
						sapi.SetAutoConnect(item.Name, false, false);
						flag3 = true;
						break;
					case CheckState.Indeterminate:
						break;
					}
				}
				SapiManager globalInstance = SapiManager.GlobalInstance;
				bool rollCallEnabled = (SapiManager.GlobalInstance.MonitoringEnabled = checkBoxRollCall.Checked);
				globalInstance.RollCallEnabled = rollCallEnabled;
				if (checkBoxRollCall.Checked)
				{
					flag3 = true;
				}
				SapiManager.GlobalInstance.RollCallPowertrainOnly = checkBoxPowertrainOnly.Checked;
				SapiManager globalInstance2 = SapiManager.GlobalInstance;
				rollCallEnabled = (SapiManager.GlobalInstance.DoIPMonitoringEnabled = checkBoxDoIPAutoConnect.Checked);
				globalInstance2.DoIPRollCallEnabled = rollCallEnabled;
				if (checkBoxDoIPAutoConnect.Checked)
				{
					flag3 = true;
				}
				if (checkBoxAllowAutoBaud.Enabled)
				{
					SapiManager.GlobalInstance.AllowAutoBaud = checkBoxAllowAutoBaud.Checked;
				}
				if (flag2)
				{
					Application.UseWaitCursor = true;
					sapi.ResetSapi();
					Application.UseWaitCursor = false;
				}
				else if (flag3 && !sapi.LogFileIsOpen)
				{
					sapi.SuspendAutoConnect();
					sapi.ResumeAutoConnect();
				}
				flag = base.ApplySettings();
				Cursor.Current = Cursors.Default;
			}
		}
		return flag;
	}

	private bool IsIdentifierVisible(string identifier)
	{
		if (checkBoxPowertrainOnly.Checked)
		{
			return ((IEnumerable<Ecu>)sapi.Sapi.Ecus).Where((Ecu ecu) => ecu.Identifier == identifier).Any((Ecu ecu) => ecu.IsPowertrainDevice);
		}
		return true;
	}

	private void BuildConnectionList()
	{
		if (base.IsDisposed)
		{
			return;
		}
		tableLayoutPanelAutoConnect.SuspendLayout();
		tableLayoutPanelAutoConnect.Controls.Clear();
		Dictionary<string, List<Ecu>> dictionary = new Dictionary<string, List<Ecu>>();
		foreach (Ecu item in from e in (IEnumerable<Ecu>)sapi.Sapi.Ecus
			where !e.IsRollCall && !e.IsVirtual && !e.OfflineSupportOnly && !SapiExtensions.ProhibitAutoConnection(e)
			orderby e.Priority
			select e)
		{
			if (!dictionary.ContainsKey(item.Identifier))
			{
				dictionary[item.Identifier] = new List<Ecu>((IEnumerable<Ecu>)(object)new Ecu[1] { item });
			}
			else
			{
				dictionary[item.Identifier].Add(item);
			}
		}
		groupSelectionPanel.Visible = dictionary.Count > 1;
		foreach (string key in dictionary.Keys)
		{
			CheckBox box = new CheckBox();
			box.ImageAlign = ContentAlignment.MiddleLeft;
			box.TextAlign = ContentAlignment.MiddleLeft;
			box.TextImageRelation = TextImageRelation.ImageBeforeText;
			box.Text = key;
			box.Name = key;
			box.Margin = new Padding(0);
			box.AutoCheck = false;
			box.AutoSize = true;
			box.Padding = new Padding(3, 0, 0, 0);
			box.Visible = IsIdentifierVisible(key);
			if (box.Visible && sapi.IsSetForAutoConnect(key))
			{
				if (sapi.Sapi.Ecus.GetMarkedForAutoConnect(key))
				{
					SetCheckState(box, CheckState.Checked);
				}
				else
				{
					SetCheckState(box, CheckState.Indeterminate);
				}
			}
			else
			{
				SetCheckState(box, CheckState.Unchecked);
			}
			box.Enabled = true;
			box.Click += OnBoxClick;
			tableLayoutPanelAutoConnect.RowCount++;
			tableLayoutPanelAutoConnect.Controls.Add(box);
			Label label = CreateLabel(GetDescription(dictionary[key]), box.Visible);
			Label label2 = CreateLabel(string.Join("/", dictionary[key]), box.Visible);
			tableLayoutPanelAutoConnect.Controls.Add(label);
			tableLayoutPanelAutoConnect.Controls.Add(label2);
			label.Click += delegate
			{
				OnBoxClick(box, new EventArgs());
			};
			label2.Click += delegate
			{
				OnBoxClick(box, new EventArgs());
			};
			box.Tag = new Tuple<Control, Control>(label, label2);
		}
		tableLayoutPanelAutoConnect.ResumeLayout(performLayout: true);
		CheckRollCallSupport();
	}

	private static Label CreateLabel(string text, bool visible)
	{
		return new Label
		{
			AutoSize = true,
			TextAlign = ContentAlignment.MiddleLeft,
			Dock = DockStyle.Left,
			Padding = new Padding(0, 0, 0, 1),
			Text = text,
			Visible = visible
		};
	}

	private static string GetDescription(IEnumerable<Ecu> ecus)
	{
		string text = Resources.ResourceManager.GetString("ConnectionOptionsPanel_Identifier" + ecus.First().Identifier.Replace('-', '_'));
		if (text == null)
		{
			text = ecus.First().ShortDescription;
		}
		return text;
	}

	private void OnSelectedHardwareChanged(object sender, EventArgs e)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Expected O, but got Unknown
		if (!base.IsDirty)
		{
			Choice val = (Choice)comboBoxHardware.SelectedItem;
			if (val.Name != sapi.HardwareType)
			{
				MarkDirty();
			}
		}
		UpdateSidConfigureButton();
	}

	private void UpdateSidConfigureButton()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		Choice val = (Choice)comboBoxHardware.SelectedItem;
		string text = ((val != (object)null) ? (val.RawValue as string) : null);
		openSidConfigure.Enabled = text != null && text.IndexOf("J", StringComparison.OrdinalIgnoreCase) != -1;
	}

	private void OnBoxClick(object sender, EventArgs e)
	{
		CheckBox checkBox = sender as CheckBox;
		switch (checkBox.CheckState)
		{
		case CheckState.Checked:
			SetCheckState(checkBox, CheckState.Unchecked);
			break;
		case CheckState.Unchecked:
		case CheckState.Indeterminate:
			SetCheckState(checkBox, CheckState.Checked);
			break;
		}
		MarkDirty();
	}

	private void SetCheckState(CheckBox box, CheckState state)
	{
		if (box.Enabled)
		{
			switch (state)
			{
			case CheckState.Checked:
				box.Image = Resources.autoconnect_on;
				if (SapiManager.GlobalInstance.IsSetForAutoConnect(box.Name))
				{
					if (sapi.Sapi.Ecus.GetMarkedForAutoConnect(box.Name))
					{
						toolTip.SetToolTip(box, Resources.TooltipAutoConnectIsEnabledForDevice);
					}
					else
					{
						toolTip.SetToolTip(box, Resources.TooltipAutoConnectWillBeResumedForDevice);
					}
				}
				else
				{
					toolTip.SetToolTip(box, Resources.TooltipAutoConnectWillBeEnabledForDevice);
				}
				break;
			case CheckState.Indeterminate:
				box.Image = Resources.autoconnect_pause;
				toolTip.SetToolTip(box, Resources.TooltipAutoConnectSuspendedForDevice);
				break;
			case CheckState.Unchecked:
				box.Image = Resources.autoconnect_off;
				if (!SapiManager.GlobalInstance.IsSetForAutoConnect(box.Name))
				{
					toolTip.SetToolTip(box, Resources.TooltipAutoConnectIsDisabledForDevice);
				}
				else
				{
					toolTip.SetToolTip(box, Resources.TooltipAutoConnectWillBeDisabledForDevice);
				}
				break;
			}
		}
		box.CheckState = state;
	}

	private void ExecuteSidConfigure(object sender, EventArgs e)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Expected O, but got Unknown
		if (!ConfirmRestartConnection(allowCancel: false) || !Program.RunSidConfigure(checkIniFile: false))
		{
			return;
		}
		bool complete = false;
		LongOperationForm val = new LongOperationForm(ApplicationInformation.ProductName, (Action<BackgroundWorker, DoWorkEventArgs>)delegate(BackgroundWorker worker, DoWorkEventArgs eventArgs)
		{
			worker.ReportProgress(50, Resources.StatusPreparingCommunications);
			Thread.Sleep(500);
			BeginInvoke((Action)delegate
			{
				SapiManager.GlobalInstance.ResetSapi();
				complete = true;
			});
			while (!complete)
			{
				Thread.Sleep(100);
			}
		}, (object)null, false);
		val.ShowDialog();
		UpdateSelectedTranslator();
		if (checkBoxAllowAutoBaud.Enabled != SapiManager.GlobalInstance.IsAutoBaudCapable)
		{
			CheckBox checkBox = checkBoxAllowAutoBaud;
			bool flag = (checkBoxAllowAutoBaud.Enabled = SapiManager.GlobalInstance.IsAutoBaudCapable);
			checkBox.Checked = flag;
		}
	}

	private void checkBoxUseMcd_CheckedChanged(object sender, EventArgs e)
	{
		if (!base.IsDirty && checkBoxUseMcd.Checked != SapiManager.GlobalInstance.UseMcd)
		{
			MarkDirty();
		}
		checkBoxDoIPAutoConnect.Enabled = checkBoxUseMcd.Checked;
		if (!checkBoxUseMcd.Checked)
		{
			checkBoxDoIPAutoConnect.Checked = false;
		}
	}

	private void checkBoxAllowAutoBaud_CheckedChanged(object sender, EventArgs e)
	{
		if (!base.IsDirty && checkBoxAllowAutoBaud.Checked != SapiManager.GlobalInstance.AllowAutoBaud)
		{
			MarkDirty();
		}
	}

	private void checkBoxRollCall_CheckedChanged(object sender, EventArgs e)
	{
		if (!base.IsDirty && checkBoxRollCall.Checked != SapiManager.GlobalInstance.RollCallEnabled)
		{
			MarkDirty();
		}
		if (!checkBoxRollCall.Checked)
		{
			checkBoxPowertrainOnly.Checked = false;
		}
		checkBoxPowertrainOnly.Enabled = checkBoxRollCall.Checked;
	}

	private void checkBoxPowertrainOnly_CheckedChanged(object sender, EventArgs e)
	{
		if (!base.IsDirty && checkBoxPowertrainOnly.Checked != SapiManager.GlobalInstance.RollCallPowertrainOnly)
		{
			MarkDirty();
		}
		tableLayoutPanelAutoConnect.SuspendLayout();
		foreach (CheckBox item2 in tableLayoutPanelAutoConnect.Controls.OfType<CheckBox>())
		{
			Tuple<Control, Control> tuple = (Tuple<Control, Control>)item2.Tag;
			Control item = tuple.Item1;
			bool flag = (tuple.Item2.Visible = IsIdentifierVisible(item2.Name));
			bool visible = (item.Visible = flag);
			item2.Visible = visible;
			if (!item2.Visible)
			{
				SetCheckState(item2, CheckState.Unchecked);
			}
		}
		tableLayoutPanelAutoConnect.ResumeLayout();
	}

	private void checkBoxDoIPAutoConnect_CheckedChanged(object sender, EventArgs e)
	{
		if (!base.IsDirty && checkBoxDoIPAutoConnect.Checked != SapiManager.GlobalInstance.DoIPRollCallEnabled)
		{
			MarkDirty();
		}
	}

	private void UpdateSelectedTranslator()
	{
		if (AdapterInformation.GlobalInstance != null)
		{
			textBoxSelectedDevice.Text = AdapterInformation.GlobalInstance.ConnectedAdapterNames.FirstOrDefault();
		}
	}

	private void CheckRollCallSupport()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Expected I4, but got Unknown
		if (AdapterInformation.GlobalInstance != null && AdapterInformation.GlobalInstance.ConnectedAdapterNames.Any())
		{
			FeatureSupport supportsConcurrentProtocols = AdapterInformation.SupportsConcurrentProtocols;
			switch ((int)supportsConcurrentProtocols)
			{
			case 0:
				labelRollcallSupport.Visible = false;
				pictureBoxRollcallSupportWarning.Visible = false;
				checkBoxRollCall.Enabled = true;
				break;
			case 1:
				labelRollcallSupport.Text = Resources.ConnectionOptionsPanel_CheckRollCallSupport_Unsupported_Feature;
				labelRollcallSupport.Visible = true;
				pictureBoxRollcallSupportWarning.Image = Resources.stop;
				pictureBoxRollcallSupportWarning.Visible = true;
				checkBoxRollCall.Enabled = false;
				if (SapiManager.GlobalInstance.RollCallEnabled)
				{
					SapiManager globalInstance = SapiManager.GlobalInstance;
					bool rollCallEnabled = (SapiManager.GlobalInstance.MonitoringEnabled = false);
					globalInstance.RollCallEnabled = rollCallEnabled;
					if (!sapi.LogFileIsOpen)
					{
						sapi.SuspendAutoConnect();
						sapi.ResumeAutoConnect();
					}
				}
				break;
			case 2:
				labelRollcallSupport.Visible = true;
				labelRollcallSupport.Text = Resources.ConnectionOptionsPanel_CheckRollCallSupport_Untested_Support_Feature;
				pictureBoxRollcallSupportWarning.Image = Resources.warning;
				pictureBoxRollcallSupportWarning.Visible = true;
				checkBoxRollCall.Enabled = true;
				break;
			}
		}
		else
		{
			labelRollcallSupport.Text = Resources.ConnectionOptionsPanel_RollCallSupport_NoSelectedTranslator;
			pictureBoxRollcallSupportWarning.Image = Resources.stop;
			Label label = labelRollcallSupport;
			bool rollCallEnabled = (pictureBoxRollcallSupportWarning.Visible = true);
			label.Visible = rollCallEnabled;
			checkBoxRollCall.Enabled = false;
		}
		checkBoxRollCall.Checked = SapiManager.GlobalInstance.RollCallEnabled;
		checkBoxPowertrainOnly.Enabled = checkBoxRollCall.Checked;
		checkBoxPowertrainOnly.Checked = checkBoxRollCall.Checked && SapiManager.GlobalInstance.RollCallPowertrainOnly;
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
		this.components = new System.ComponentModel.Container();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DetroitDiesel.Windows.Forms.Diagnostics.Container.ConnectionOptionsPanel));
		this.comboBoxHardware = new System.Windows.Forms.ComboBox();
		this.toolTip = new System.Windows.Forms.ToolTip(this.components);
		this.openSidConfigure = new System.Windows.Forms.Button();
		this.groupBox1 = new System.Windows.Forms.GroupBox();
		this.tableLayoutPanelRollCall = new System.Windows.Forms.TableLayoutPanel();
		this.checkBoxRollCall = new System.Windows.Forms.CheckBox();
		this.labelRollcallSupport = new System.Windows.Forms.Label();
		this.pictureBoxRollcallSupportWarning = new System.Windows.Forms.PictureBox();
		this.checkBoxDoIPAutoConnect = new System.Windows.Forms.CheckBox();
		this.checkBoxPowertrainOnly = new System.Windows.Forms.CheckBox();
		this.labelDevice = new System.Windows.Forms.Label();
		this.textBoxSelectedDevice = new System.Windows.Forms.TextBox();
		this.tableLayoutPanelVCI = new System.Windows.Forms.TableLayoutPanel();
		this.checkBoxAllowAutoBaud = new System.Windows.Forms.CheckBox();
		this.checkBoxUseMcd = new System.Windows.Forms.CheckBox();
		this.groupSelectionPanel = new System.Windows.Forms.GroupBox();
		this.tableLayoutPanelProprietaryDevices = new System.Windows.Forms.TableLayoutPanel();
		this.selectNoneButton = new System.Windows.Forms.Button();
		this.selectAllButton = new System.Windows.Forms.Button();
		this.resetToDefaultsButton = new System.Windows.Forms.Button();
		this.panelTableLayoutAutoConnect = new System.Windows.Forms.Panel();
		this.tableLayoutPanelAutoConnect = new System.Windows.Forms.TableLayoutPanel();
		System.Windows.Forms.Label label = new System.Windows.Forms.Label();
		this.groupBox1.SuspendLayout();
		this.tableLayoutPanelRollCall.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.pictureBoxRollcallSupportWarning).BeginInit();
		this.tableLayoutPanelVCI.SuspendLayout();
		this.groupSelectionPanel.SuspendLayout();
		this.tableLayoutPanelProprietaryDevices.SuspendLayout();
		this.panelTableLayoutAutoConnect.SuspendLayout();
		base.SuspendLayout();
		resources.ApplyResources(label, "labelHardware");
		label.Name = "labelHardware";
		this.tableLayoutPanelVCI.SetRowSpan(label, 2);
		resources.ApplyResources(this.comboBoxHardware, "comboBoxHardware");
		this.comboBoxHardware.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.comboBoxHardware.FormattingEnabled = true;
		this.comboBoxHardware.Name = "comboBoxHardware";
		this.tableLayoutPanelVCI.SetRowSpan(this.comboBoxHardware, 2);
		this.comboBoxHardware.SelectedIndexChanged += new System.EventHandler(OnSelectedHardwareChanged);
		this.toolTip.AutomaticDelay = 0;
		resources.ApplyResources(this.openSidConfigure, "openSidConfigure");
		this.openSidConfigure.Name = "openSidConfigure";
		this.tableLayoutPanelVCI.SetRowSpan(this.openSidConfigure, 2);
		this.openSidConfigure.UseVisualStyleBackColor = true;
		this.openSidConfigure.Click += new System.EventHandler(ExecuteSidConfigure);
		resources.ApplyResources(this.groupBox1, "groupBox1");
		this.groupBox1.Controls.Add(this.tableLayoutPanelRollCall);
		this.groupBox1.Name = "groupBox1";
		this.groupBox1.TabStop = false;
		resources.ApplyResources(this.tableLayoutPanelRollCall, "tableLayoutPanelRollCall");
		this.tableLayoutPanelRollCall.Controls.Add(this.checkBoxRollCall, 0, 0);
		this.tableLayoutPanelRollCall.Controls.Add(this.labelRollcallSupport, 2, 0);
		this.tableLayoutPanelRollCall.Controls.Add(this.pictureBoxRollcallSupportWarning, 1, 0);
		this.tableLayoutPanelRollCall.Controls.Add(this.checkBoxDoIPAutoConnect, 0, 2);
		this.tableLayoutPanelRollCall.Controls.Add(this.checkBoxPowertrainOnly, 0, 1);
		this.tableLayoutPanelRollCall.Name = "tableLayoutPanelRollCall";
		resources.ApplyResources(this.checkBoxRollCall, "checkBoxRollCall");
		this.checkBoxRollCall.Name = "checkBoxRollCall";
		this.checkBoxRollCall.UseVisualStyleBackColor = true;
		this.checkBoxRollCall.CheckedChanged += new System.EventHandler(checkBoxRollCall_CheckedChanged);
		resources.ApplyResources(this.labelRollcallSupport, "labelRollcallSupport");
		this.labelRollcallSupport.Name = "labelRollcallSupport";
		this.tableLayoutPanelRollCall.SetRowSpan(this.labelRollcallSupport, 3);
		resources.ApplyResources(this.pictureBoxRollcallSupportWarning, "pictureBoxRollcallSupportWarning");
		this.pictureBoxRollcallSupportWarning.Image = DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties.Resources.stop;
		this.pictureBoxRollcallSupportWarning.Name = "pictureBoxRollcallSupportWarning";
		this.tableLayoutPanelRollCall.SetRowSpan(this.pictureBoxRollcallSupportWarning, 3);
		this.pictureBoxRollcallSupportWarning.TabStop = false;
		resources.ApplyResources(this.checkBoxDoIPAutoConnect, "checkBoxDoIPAutoConnect");
		this.checkBoxDoIPAutoConnect.Name = "checkBoxDoIPAutoConnect";
		this.checkBoxDoIPAutoConnect.UseVisualStyleBackColor = true;
		this.checkBoxDoIPAutoConnect.CheckedChanged += new System.EventHandler(checkBoxDoIPAutoConnect_CheckedChanged);
		resources.ApplyResources(this.checkBoxPowertrainOnly, "checkBoxPowertrainOnly");
		this.checkBoxPowertrainOnly.Name = "checkBoxPowertrainOnly";
		this.checkBoxPowertrainOnly.UseVisualStyleBackColor = true;
		this.checkBoxPowertrainOnly.CheckedChanged += new System.EventHandler(checkBoxPowertrainOnly_CheckedChanged);
		resources.ApplyResources(this.labelDevice, "labelDevice");
		this.labelDevice.Name = "labelDevice";
		this.tableLayoutPanelVCI.SetRowSpan(this.labelDevice, 2);
		resources.ApplyResources(this.textBoxSelectedDevice, "textBoxSelectedDevice");
		this.textBoxSelectedDevice.Name = "textBoxSelectedDevice";
		this.tableLayoutPanelVCI.SetRowSpan(this.textBoxSelectedDevice, 2);
		resources.ApplyResources(this.tableLayoutPanelVCI, "tableLayoutPanelVCI");
		this.tableLayoutPanelVCI.Controls.Add(this.checkBoxAllowAutoBaud, 5, 1);
		this.tableLayoutPanelVCI.Controls.Add(label, 0, 0);
		this.tableLayoutPanelVCI.Controls.Add(this.comboBoxHardware, 1, 0);
		this.tableLayoutPanelVCI.Controls.Add(this.textBoxSelectedDevice, 3, 0);
		this.tableLayoutPanelVCI.Controls.Add(this.openSidConfigure, 4, 0);
		this.tableLayoutPanelVCI.Controls.Add(this.labelDevice, 2, 0);
		this.tableLayoutPanelVCI.Controls.Add(this.checkBoxUseMcd, 5, 0);
		this.tableLayoutPanelVCI.Name = "tableLayoutPanelVCI";
		resources.ApplyResources(this.checkBoxAllowAutoBaud, "checkBoxAllowAutoBaud");
		this.checkBoxAllowAutoBaud.Name = "checkBoxAllowAutoBaud";
		this.checkBoxAllowAutoBaud.UseVisualStyleBackColor = true;
		this.checkBoxAllowAutoBaud.CheckedChanged += new System.EventHandler(checkBoxAllowAutoBaud_CheckedChanged);
		resources.ApplyResources(this.checkBoxUseMcd, "checkBoxUseMcd");
		this.checkBoxUseMcd.Name = "checkBoxUseMcd";
		this.checkBoxUseMcd.UseVisualStyleBackColor = true;
		this.checkBoxUseMcd.CheckedChanged += new System.EventHandler(checkBoxUseMcd_CheckedChanged);
		this.groupSelectionPanel.Controls.Add(this.tableLayoutPanelProprietaryDevices);
		resources.ApplyResources(this.groupSelectionPanel, "groupSelectionPanel");
		this.groupSelectionPanel.Name = "groupSelectionPanel";
		this.groupSelectionPanel.TabStop = false;
		resources.ApplyResources(this.tableLayoutPanelProprietaryDevices, "tableLayoutPanelProprietaryDevices");
		this.tableLayoutPanelProprietaryDevices.Controls.Add(this.selectNoneButton, 1, 2);
		this.tableLayoutPanelProprietaryDevices.Controls.Add(this.selectAllButton, 1, 1);
		this.tableLayoutPanelProprietaryDevices.Controls.Add(this.resetToDefaultsButton, 1, 0);
		this.tableLayoutPanelProprietaryDevices.Controls.Add(this.panelTableLayoutAutoConnect, 0, 0);
		this.tableLayoutPanelProprietaryDevices.Name = "tableLayoutPanelProprietaryDevices";
		resources.ApplyResources(this.selectNoneButton, "selectNoneButton");
		this.selectNoneButton.Name = "selectNoneButton";
		this.selectNoneButton.UseVisualStyleBackColor = true;
		this.selectNoneButton.Click += new System.EventHandler(selectNoneButton_Click);
		resources.ApplyResources(this.selectAllButton, "selectAllButton");
		this.selectAllButton.Name = "selectAllButton";
		this.selectAllButton.UseVisualStyleBackColor = true;
		this.selectAllButton.Click += new System.EventHandler(selectAllButton_Click);
		resources.ApplyResources(this.resetToDefaultsButton, "resetToDefaultsButton");
		this.resetToDefaultsButton.Name = "resetToDefaultsButton";
		this.resetToDefaultsButton.UseVisualStyleBackColor = true;
		this.resetToDefaultsButton.Click += new System.EventHandler(resetToDefaultsButton_Click);
		this.panelTableLayoutAutoConnect.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		this.panelTableLayoutAutoConnect.Controls.Add(this.tableLayoutPanelAutoConnect);
		resources.ApplyResources(this.panelTableLayoutAutoConnect, "panelTableLayoutAutoConnect");
		this.panelTableLayoutAutoConnect.Name = "panelTableLayoutAutoConnect";
		this.tableLayoutPanelProprietaryDevices.SetRowSpan(this.panelTableLayoutAutoConnect, 4);
		resources.ApplyResources(this.tableLayoutPanelAutoConnect, "tableLayoutPanelAutoConnect");
		this.tableLayoutPanelAutoConnect.BackColor = System.Drawing.SystemColors.Window;
		this.tableLayoutPanelAutoConnect.Name = "tableLayoutPanelAutoConnect";
		resources.ApplyResources(this, "$this");
		base.Controls.Add(this.groupSelectionPanel);
		base.Controls.Add(this.tableLayoutPanelVCI);
		base.Controls.Add(this.groupBox1);
		base.Name = "ConnectionOptionsPanel";
		base.Controls.SetChildIndex(this.groupBox1, 0);
		base.Controls.SetChildIndex(this.tableLayoutPanelVCI, 0);
		base.Controls.SetChildIndex(this.groupSelectionPanel, 0);
		this.groupBox1.ResumeLayout(false);
		this.tableLayoutPanelRollCall.ResumeLayout(false);
		this.tableLayoutPanelRollCall.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.pictureBoxRollcallSupportWarning).EndInit();
		this.tableLayoutPanelVCI.ResumeLayout(false);
		this.tableLayoutPanelVCI.PerformLayout();
		this.groupSelectionPanel.ResumeLayout(false);
		this.tableLayoutPanelProprietaryDevices.ResumeLayout(false);
		this.tableLayoutPanelProprietaryDevices.PerformLayout();
		this.panelTableLayoutAutoConnect.ResumeLayout(false);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
