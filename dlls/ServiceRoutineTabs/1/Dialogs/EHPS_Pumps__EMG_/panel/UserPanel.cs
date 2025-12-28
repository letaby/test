// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.EHPS_Pumps__EMG_.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.EHPS_Pumps__EMG_.panel;

public class UserPanel : CustomPanel
{
  private static string ehps201tName = "EHPS201T";
  private static string ehps401tName = "EHPS401T";
  private static string startTestQualifier = "RT_Pump_Routine_Start(1250,0)";
  private static string stopPumpQualifier = "RT_Pump_Routine_Stop";
  private Channel ehps201tChannel;
  private Channel ehps401tChannel;
  private bool ehps201tPumpTesting = false;
  private bool ehps401tPumpTesting = false;
  private bool ehpsPumpsTesting = false;
  private Panel panelTest;
  private TableLayoutPanel tableLayoutPanelTest;
  private System.Windows.Forms.Label labelEhps401tPumpTest;
  private System.Windows.Forms.Label labelEhps201tPumpTest;
  private System.Windows.Forms.Label labelBothPumpTest;
  private Button buttonBothPumpsTest;
  private Button buttonEHPS401TPumpTest;
  private Button buttonEHPS201TPumpTest;
  private TableLayoutPanel tableLayoutPanelMain;
  private SeekTimeListView seekTimeListView;
  private Button buttonEHPS201TPumpStopTest;
  private Button buttonEHPS401TPumpStopTest;
  private Button buttonClose;
  private TableLayoutPanel tableLayoutPanel4;
  private DigitalReadoutInstrument digitalReadoutInstrumentVehicleSpeed;
  private DigitalReadoutInstrument digitalReadoutInstrumentParkBrake;
  private System.Windows.Forms.Label labelInterlockWarning;
  private System.Windows.Forms.Label label39;
  private Button buttonEHPSPumpsStopTest;

  protected virtual void OnLoad(EventArgs e)
  {
    ((ContainerControl) this).ParentForm.FormClosing += new FormClosingEventHandler(this.OnParentFormClosing);
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
  }

  public UserPanel() => this.InitializeComponent();

  public virtual void OnChannelsChanged()
  {
    this.SetEhps201tChannel(UserPanel.ehps201tName);
    this.SetEhps401tChannel(UserPanel.ehps401tName);
    this.UpdateUI();
  }

  private bool InterlockOk
  {
    get
    {
      return this.digitalReadoutInstrumentVehicleSpeed.RepresentedState == 1 || this.digitalReadoutInstrumentParkBrake.RepresentedState == 1;
    }
  }

  private bool Ehps201TOnline
  {
    get
    {
      return this.ehps201tChannel != null && (this.ehps201tChannel.CommunicationsState == CommunicationsState.Online || this.ehps201tChannel.CommunicationsState == CommunicationsState.ExecuteService);
    }
  }

  private bool Ehps401TOnline
  {
    get
    {
      return this.ehps401tChannel != null && (this.ehps401tChannel.CommunicationsState == CommunicationsState.Online || this.ehps401tChannel.CommunicationsState == CommunicationsState.ExecuteService);
    }
  }

  private void OnParentFormClosing(object sender, FormClosingEventArgs e)
  {
    e.Cancel = this.ehps201tPumpTesting || this.ehps201tPumpTesting || this.ehpsPumpsTesting;
    if (e.Cancel)
      return;
    this.SetEhps201tChannel((string) null);
    this.SetEhps401tChannel((string) null);
    ((ContainerControl) this).ParentForm.FormClosing -= new FormClosingEventHandler(this.OnParentFormClosing);
  }

  private void AddLogLabel(string text)
  {
    if (!(text != string.Empty))
      return;
    this.LabelLog(this.seekTimeListView.RequiredUserLabelPrefix, text);
  }

  private void SetEhps201tChannel(string ecuName)
  {
    Channel channel = this.GetChannel(ecuName, (CustomPanel.ChannelLookupOptions) 3);
    if (this.ehps201tChannel != channel)
    {
      if (this.ehps201tChannel != null)
      {
        this.ehps201tChannel.Services.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.ehps201tChannel_ServiceCompleteEvent);
        this.ehps201tChannel.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.ehps201tChannel_CommunicationsStateUpdateEvent);
      }
      this.ehps201tChannel = channel;
      if (this.ehps201tChannel != null)
      {
        this.ehps201tChannel.Services.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.ehps201tChannel_ServiceCompleteEvent);
        this.ehps201tChannel.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.ehps201tChannel_CommunicationsStateUpdateEvent);
      }
      this.ehps201tPumpTesting = false;
      this.ehpsPumpsTesting = false;
    }
    this.UpdateUI();
  }

  private void ehps201tChannel_CommunicationsStateUpdateEvent(
    object sender,
    CommunicationsStateEventArgs e)
  {
    this.UpdateUI();
  }

  private void ehps201tChannel_ServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    this.UpdateUI();
  }

  private void SetEhps401tChannel(string ecuName)
  {
    Channel channel = this.GetChannel(ecuName, (CustomPanel.ChannelLookupOptions) 3);
    if (this.ehps401tChannel != channel)
    {
      if (this.ehps401tChannel != null)
      {
        this.ehps401tChannel.Services.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.ehps401tChannel_ServiceCompleteEvent);
        this.ehps401tChannel.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.ehps401tChannel_CommunicationsStateUpdateEvent);
      }
      this.ehps401tChannel = channel;
      if (this.ehps401tChannel != null)
      {
        this.ehps401tChannel.Services.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.ehps401tChannel_ServiceCompleteEvent);
        this.ehps401tChannel.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.ehps401tChannel_CommunicationsStateUpdateEvent);
      }
      this.ehps401tPumpTesting = false;
      this.ehpsPumpsTesting = false;
    }
    this.UpdateUI();
  }

  private void ehps401tChannel_CommunicationsStateUpdateEvent(
    object sender,
    CommunicationsStateEventArgs e)
  {
    this.UpdateUI();
  }

  private void ehps401tChannel_ServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    this.UpdateUI();
  }

  private void UpdateUI()
  {
    this.labelInterlockWarning.Visible = !this.InterlockOk;
    this.labelEhps201tPumpTest.Text = this.Ehps201TOnline ? Resources.Message_EHPS201TPumpTest : Resources.Message_EHPS201TOffline;
    this.labelEhps401tPumpTest.Text = this.Ehps401TOnline ? Resources.Message_EHPS401TPumpTest : Resources.Message_EHPS401TOffline;
    this.buttonEHPS201TPumpTest.Enabled = this.InterlockOk && this.Ehps201TOnline && !this.ehps201tPumpTesting && !this.ehpsPumpsTesting;
    this.buttonEHPS401TPumpTest.Enabled = this.InterlockOk && this.Ehps401TOnline && !this.ehps401tPumpTesting && !this.ehpsPumpsTesting;
    this.buttonBothPumpsTest.Enabled = this.InterlockOk && this.Ehps201TOnline && this.Ehps401TOnline && !this.ehpsPumpsTesting;
    this.buttonEHPS201TPumpStopTest.Enabled = this.Ehps201TOnline;
    this.buttonEHPS401TPumpStopTest.Enabled = this.Ehps401TOnline;
    this.buttonEHPSPumpsStopTest.Enabled = this.Ehps201TOnline && this.Ehps401TOnline;
    this.buttonClose.Enabled = !this.ehps201tPumpTesting && !this.ehps401tPumpTesting && !this.ehpsPumpsTesting;
  }

  private bool RunService(
    Channel channel,
    string serviceQualifier,
    ServiceCompleteEventHandler serviceCompleteEventHandler)
  {
    if (channel != null && channel.Online)
    {
      Service service = channel.Services[serviceQualifier];
      if (service != (Service) null)
      {
        if (serviceCompleteEventHandler != null)
          service.ServiceCompleteEvent += serviceCompleteEventHandler;
        channel.Services.Execute(serviceQualifier, false);
        this.AddLogLabel(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_ServiceStarted01, (object) channel.Ecu.Name, (object) serviceQualifier));
        return true;
      }
    }
    this.AddLogLabel(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_ServiceCouldNotBeStarted01, channel == null || channel.Ecu == null ? (object) Resources.Message_Null : (object) channel.Ecu.Name, (object) serviceQualifier));
    return false;
  }

  private void textBox_KeyPress(object sender, KeyPressEventArgs e)
  {
    e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
    this.UpdateUI();
  }

  private void textBox_TextChanged(object sender, EventArgs e) => this.UpdateUI();

  private void buttonEHPS201TPumpTest_Click(object sender, EventArgs e)
  {
    if (this.RunService(this.ehps201tChannel, UserPanel.startTestQualifier, (ServiceCompleteEventHandler) null))
    {
      this.ehps201tPumpTesting = true;
      if (this.ehps401tPumpTesting)
      {
        this.ehps201tPumpTesting = false;
        this.ehps401tPumpTesting = false;
        this.ehpsPumpsTesting = true;
      }
    }
    else
    {
      this.RunService(this.ehps201tChannel, UserPanel.stopPumpQualifier, (ServiceCompleteEventHandler) null);
      this.ehps201tPumpTesting = false;
      this.ehpsPumpsTesting = false;
    }
    this.UpdateUI();
  }

  private void buttonEHPS401TPumpTest_Click(object sender, EventArgs e)
  {
    if (this.RunService(this.ehps401tChannel, UserPanel.startTestQualifier, (ServiceCompleteEventHandler) null))
    {
      this.ehps401tPumpTesting = true;
      if (this.ehps201tPumpTesting)
      {
        this.ehps201tPumpTesting = false;
        this.ehps401tPumpTesting = false;
        this.ehpsPumpsTesting = true;
      }
    }
    else
    {
      this.RunService(this.ehps401tChannel, UserPanel.stopPumpQualifier, (ServiceCompleteEventHandler) null);
      this.ehps401tPumpTesting = false;
      this.ehpsPumpsTesting = false;
    }
    this.UpdateUI();
  }

  private void buttonBothPumpsTest_Click(object sender, EventArgs e)
  {
    this.ehps201tPumpTesting = false;
    this.ehps401tPumpTesting = false;
    if (this.RunService(this.ehps201tChannel, UserPanel.startTestQualifier, (ServiceCompleteEventHandler) null) && this.RunService(this.ehps401tChannel, UserPanel.startTestQualifier, (ServiceCompleteEventHandler) null))
    {
      this.ehpsPumpsTesting = true;
    }
    else
    {
      this.RunService(this.ehps201tChannel, UserPanel.stopPumpQualifier, (ServiceCompleteEventHandler) null);
      this.RunService(this.ehps401tChannel, UserPanel.stopPumpQualifier, (ServiceCompleteEventHandler) null);
      this.ehpsPumpsTesting = false;
    }
    this.UpdateUI();
  }

  private void buttonEHPS201TPumpStopTest_Click(object sender, EventArgs e)
  {
    this.RunService(this.ehps201tChannel, UserPanel.stopPumpQualifier, (ServiceCompleteEventHandler) null);
    this.ehps201tPumpTesting = false;
    this.ehps401tPumpTesting = this.ehps401tPumpTesting || this.ehpsPumpsTesting;
    this.ehpsPumpsTesting = false;
    this.UpdateUI();
  }

  private void buttonEHPS401TPumpStopTest_Click(object sender, EventArgs e)
  {
    this.RunService(this.ehps401tChannel, UserPanel.stopPumpQualifier, (ServiceCompleteEventHandler) null);
    this.ehps201tPumpTesting = this.ehps201tPumpTesting || this.ehpsPumpsTesting;
    this.ehps401tPumpTesting = false;
    this.ehpsPumpsTesting = false;
    this.UpdateUI();
  }

  private void buttonEHPSPumpsStop_Click(object sender, EventArgs e)
  {
    this.RunService(this.ehps201tChannel, UserPanel.stopPumpQualifier, (ServiceCompleteEventHandler) null);
    this.RunService(this.ehps401tChannel, UserPanel.stopPumpQualifier, (ServiceCompleteEventHandler) null);
    this.ehps201tPumpTesting = false;
    this.ehps401tPumpTesting = false;
    this.ehpsPumpsTesting = false;
    this.UpdateUI();
  }

  private void digitalReadoutInstrumentVehicleSpeed_RepresentedStateChanged(
    object sender,
    EventArgs e)
  {
    this.UpdateUI();
  }

  private void digitalReadoutInstrumentParkBrake_RepresentedStateChanged(object sender, EventArgs e)
  {
    this.UpdateUI();
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanelMain = new TableLayoutPanel();
    this.tableLayoutPanel4 = new TableLayoutPanel();
    this.digitalReadoutInstrumentVehicleSpeed = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentParkBrake = new DigitalReadoutInstrument();
    this.labelInterlockWarning = new System.Windows.Forms.Label();
    this.label39 = new System.Windows.Forms.Label();
    this.panelTest = new Panel();
    this.tableLayoutPanelTest = new TableLayoutPanel();
    this.buttonBothPumpsTest = new Button();
    this.buttonEHPS401TPumpTest = new Button();
    this.labelEhps401tPumpTest = new System.Windows.Forms.Label();
    this.labelEhps201tPumpTest = new System.Windows.Forms.Label();
    this.labelBothPumpTest = new System.Windows.Forms.Label();
    this.buttonEHPS201TPumpTest = new Button();
    this.buttonEHPS201TPumpStopTest = new Button();
    this.buttonEHPS401TPumpStopTest = new Button();
    this.buttonEHPSPumpsStopTest = new Button();
    this.seekTimeListView = new SeekTimeListView();
    this.buttonClose = new Button();
    ((Control) this.tableLayoutPanelMain).SuspendLayout();
    ((Control) this.tableLayoutPanel4).SuspendLayout();
    this.panelTest.SuspendLayout();
    ((Control) this.tableLayoutPanelTest).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelMain, "tableLayoutPanelMain");
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.tableLayoutPanel4, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.panelTest, 1, 1);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.seekTimeListView, 1, 2);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.buttonClose, 1, 3);
    ((Control) this.tableLayoutPanelMain).Name = "tableLayoutPanelMain";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel4, "tableLayoutPanel4");
    ((TableLayoutPanel) this.tableLayoutPanel4).Controls.Add((Control) this.digitalReadoutInstrumentVehicleSpeed, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanel4).Controls.Add((Control) this.digitalReadoutInstrumentParkBrake, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel4).Controls.Add((Control) this.labelInterlockWarning, 0, 4);
    ((TableLayoutPanel) this.tableLayoutPanel4).Controls.Add((Control) this.label39, 0, 2);
    ((Control) this.tableLayoutPanel4).Name = "tableLayoutPanel4";
    ((TableLayoutPanel) this.tableLayoutPanelMain).SetRowSpan((Control) this.tableLayoutPanel4, 4);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentVehicleSpeed, "digitalReadoutInstrumentVehicleSpeed");
    this.digitalReadoutInstrumentVehicleSpeed.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentVehicleSpeed).FreezeValue = false;
    this.digitalReadoutInstrumentVehicleSpeed.Gradient.Initialize((ValueState) 3, 4, "mph");
    this.digitalReadoutInstrumentVehicleSpeed.Gradient.Modify(1, 0.0, (ValueState) 1);
    this.digitalReadoutInstrumentVehicleSpeed.Gradient.Modify(2, 5.0, (ValueState) 3);
    this.digitalReadoutInstrumentVehicleSpeed.Gradient.Modify(3, 6.0, (ValueState) 3);
    this.digitalReadoutInstrumentVehicleSpeed.Gradient.Modify(4, (double) int.MaxValue, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentVehicleSpeed).Instrument = new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS012_CurrentVehicleSpeed_CurrentVehicleSpeed");
    ((Control) this.digitalReadoutInstrumentVehicleSpeed).Name = "digitalReadoutInstrumentVehicleSpeed";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentVehicleSpeed).UnitAlignment = StringAlignment.Near;
    this.digitalReadoutInstrumentVehicleSpeed.RepresentedStateChanged += new EventHandler(this.digitalReadoutInstrumentVehicleSpeed_RepresentedStateChanged);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentParkBrake, "digitalReadoutInstrumentParkBrake");
    this.digitalReadoutInstrumentParkBrake.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentParkBrake).FreezeValue = false;
    this.digitalReadoutInstrumentParkBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText"));
    this.digitalReadoutInstrumentParkBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText1"));
    this.digitalReadoutInstrumentParkBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText2"));
    this.digitalReadoutInstrumentParkBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText3"));
    this.digitalReadoutInstrumentParkBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText4"));
    this.digitalReadoutInstrumentParkBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText5"));
    this.digitalReadoutInstrumentParkBrake.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText6"));
    this.digitalReadoutInstrumentParkBrake.Gradient.Initialize((ValueState) 0, 6);
    this.digitalReadoutInstrumentParkBrake.Gradient.Modify(1, 0.0, (ValueState) 0);
    this.digitalReadoutInstrumentParkBrake.Gradient.Modify(2, 1.0, (ValueState) 1);
    this.digitalReadoutInstrumentParkBrake.Gradient.Modify(3, 2.0, (ValueState) 0);
    this.digitalReadoutInstrumentParkBrake.Gradient.Modify(4, 3.0, (ValueState) 0);
    this.digitalReadoutInstrumentParkBrake.Gradient.Modify(5, 4.0, (ValueState) 0);
    this.digitalReadoutInstrumentParkBrake.Gradient.Modify(6, 5.0, (ValueState) 0);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentParkBrake).Instrument = new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_DS002_ParkingBrakeSwitchSumSignal");
    ((Control) this.digitalReadoutInstrumentParkBrake).Name = "digitalReadoutInstrumentParkBrake";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentParkBrake).ShowValueReadout = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentParkBrake).UnitAlignment = StringAlignment.Near;
    this.digitalReadoutInstrumentParkBrake.RepresentedStateChanged += new EventHandler(this.digitalReadoutInstrumentParkBrake_RepresentedStateChanged);
    componentResourceManager.ApplyResources((object) this.labelInterlockWarning, "labelInterlockWarning");
    this.labelInterlockWarning.ForeColor = Color.Red;
    this.labelInterlockWarning.Name = "labelInterlockWarning";
    this.labelInterlockWarning.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.label39, "label39");
    this.label39.Name = "label39";
    this.label39.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.panelTest, "panelTest");
    this.panelTest.BorderStyle = BorderStyle.FixedSingle;
    this.panelTest.Controls.Add((Control) this.tableLayoutPanelTest);
    this.panelTest.Name = "panelTest";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelTest, "tableLayoutPanelTest");
    ((TableLayoutPanel) this.tableLayoutPanelTest).Controls.Add((Control) this.buttonBothPumpsTest, 1, 2);
    ((TableLayoutPanel) this.tableLayoutPanelTest).Controls.Add((Control) this.buttonEHPS401TPumpTest, 1, 1);
    ((TableLayoutPanel) this.tableLayoutPanelTest).Controls.Add((Control) this.labelEhps401tPumpTest, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanelTest).Controls.Add((Control) this.labelEhps201tPumpTest, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelTest).Controls.Add((Control) this.labelBothPumpTest, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanelTest).Controls.Add((Control) this.buttonEHPS201TPumpTest, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanelTest).Controls.Add((Control) this.buttonEHPS201TPumpStopTest, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanelTest).Controls.Add((Control) this.buttonEHPS401TPumpStopTest, 2, 1);
    ((TableLayoutPanel) this.tableLayoutPanelTest).Controls.Add((Control) this.buttonEHPSPumpsStopTest, 2, 2);
    ((Control) this.tableLayoutPanelTest).Name = "tableLayoutPanelTest";
    componentResourceManager.ApplyResources((object) this.buttonBothPumpsTest, "buttonBothPumpsTest");
    this.buttonBothPumpsTest.Name = "buttonBothPumpsTest";
    this.buttonBothPumpsTest.UseVisualStyleBackColor = true;
    this.buttonBothPumpsTest.Click += new EventHandler(this.buttonBothPumpsTest_Click);
    componentResourceManager.ApplyResources((object) this.buttonEHPS401TPumpTest, "buttonEHPS401TPumpTest");
    this.buttonEHPS401TPumpTest.Name = "buttonEHPS401TPumpTest";
    this.buttonEHPS401TPumpTest.UseVisualStyleBackColor = true;
    this.buttonEHPS401TPumpTest.Click += new EventHandler(this.buttonEHPS401TPumpTest_Click);
    componentResourceManager.ApplyResources((object) this.labelEhps401tPumpTest, "labelEhps401tPumpTest");
    this.labelEhps401tPumpTest.Name = "labelEhps401tPumpTest";
    componentResourceManager.ApplyResources((object) this.labelEhps201tPumpTest, "labelEhps201tPumpTest");
    this.labelEhps201tPumpTest.Name = "labelEhps201tPumpTest";
    componentResourceManager.ApplyResources((object) this.labelBothPumpTest, "labelBothPumpTest");
    this.labelBothPumpTest.Name = "labelBothPumpTest";
    componentResourceManager.ApplyResources((object) this.buttonEHPS201TPumpTest, "buttonEHPS201TPumpTest");
    this.buttonEHPS201TPumpTest.Name = "buttonEHPS201TPumpTest";
    this.buttonEHPS201TPumpTest.UseVisualStyleBackColor = true;
    this.buttonEHPS201TPumpTest.Click += new EventHandler(this.buttonEHPS201TPumpTest_Click);
    componentResourceManager.ApplyResources((object) this.buttonEHPS201TPumpStopTest, "buttonEHPS201TPumpStopTest");
    this.buttonEHPS201TPumpStopTest.Name = "buttonEHPS201TPumpStopTest";
    this.buttonEHPS201TPumpStopTest.UseVisualStyleBackColor = true;
    this.buttonEHPS201TPumpStopTest.Click += new EventHandler(this.buttonEHPS201TPumpStopTest_Click);
    componentResourceManager.ApplyResources((object) this.buttonEHPS401TPumpStopTest, "buttonEHPS401TPumpStopTest");
    this.buttonEHPS401TPumpStopTest.Name = "buttonEHPS401TPumpStopTest";
    this.buttonEHPS401TPumpStopTest.UseVisualStyleBackColor = true;
    this.buttonEHPS401TPumpStopTest.Click += new EventHandler(this.buttonEHPS401TPumpStopTest_Click);
    componentResourceManager.ApplyResources((object) this.buttonEHPSPumpsStopTest, "buttonEHPSPumpsStopTest");
    this.buttonEHPSPumpsStopTest.Name = "buttonEHPSPumpsStopTest";
    this.buttonEHPSPumpsStopTest.UseVisualStyleBackColor = true;
    this.buttonEHPSPumpsStopTest.Click += new EventHandler(this.buttonEHPSPumpsStop_Click);
    componentResourceManager.ApplyResources((object) this.seekTimeListView, "seekTimeListView");
    this.seekTimeListView.FilterUserLabels = true;
    ((Control) this.seekTimeListView).Name = "seekTimeListView";
    this.seekTimeListView.RequiredUserLabelPrefix = "ePowerSteering";
    this.seekTimeListView.SelectedTime = new DateTime?();
    this.seekTimeListView.ShowChannelLabels = false;
    this.seekTimeListView.ShowCommunicationsState = false;
    componentResourceManager.ApplyResources((object) this.buttonClose, "buttonClose");
    this.buttonClose.DialogResult = DialogResult.OK;
    this.buttonClose.Name = "buttonClose";
    this.buttonClose.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("_DDDL.chm_ePower_Steering");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanelMain);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanelMain).ResumeLayout(false);
    ((Control) this.tableLayoutPanelMain).PerformLayout();
    ((Control) this.tableLayoutPanel4).ResumeLayout(false);
    ((Control) this.tableLayoutPanel4).PerformLayout();
    this.panelTest.ResumeLayout(false);
    this.panelTest.PerformLayout();
    ((Control) this.tableLayoutPanelTest).ResumeLayout(false);
    ((Control) this.tableLayoutPanelTest).PerformLayout();
    ((Control) this).ResumeLayout(false);
  }
}
