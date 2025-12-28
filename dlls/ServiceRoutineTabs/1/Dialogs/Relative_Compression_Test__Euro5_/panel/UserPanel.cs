// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Relative_Compression_Test__Euro5_.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Interfaces;
using DetroitDiesel.Windows.Forms;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Relative_Compression_Test__Euro5_.panel;

public class UserPanel : CustomPanel, IProvideHtml
{
  private const string ServiceRoutineStart = "RT_SR0207_Automatic_Compression_Detection_Start";
  private const string ServiceRoutineCompressionValue = "RT_SR0207_Automatic_Compression_Detection_Request_Results_Compression_Value_Segment_{0}";
  private const string ServiceRoutineStatus = "RT_SR0207_Automatic_Compression_Detection_Request_Results_State_Byte";
  private ScalingLabel[] labels;
  private int CylinderCount = 6;
  private WarningManager warningManager;
  private string resultString = Resources.Message_The_Test_Has_Not_Started;
  private bool TestIsRunning;
  private Channel channel;
  private bool testSucceed = false;
  private ScalingLabel CylinderOutput3;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label CylinderLabel3;
  private ScalingLabel CylinderOutput2;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label CylinderLabel2;
  private ScalingLabel CylinderOutput1;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label CylinderLabel1;
  private ScalingLabel CylinderOutput6;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label CylinderLabel6;
  private ScalingLabel CylinderOutput4;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label CylinderLabel4;
  private ScalingLabel CylinderOutput5;
  private TableLayoutPanel tableLayoutPanel1;
  private SeekTimeListView seekTimeListView;
  private TableLayoutPanel tableLayoutPanel3;
  private Button buttonRun;
  private Button buttonPrint;
  private DigitalReadoutInstrument engineSpeedDigitalReadoutInstrument;
  private System.Windows.Forms.Label label2;
  private TableLayoutPanel tableLayoutPanel6;
  private TableLayoutPanel tableLayoutPanel2;
  private Checkmark checkmark1;
  private System.Windows.Forms.Label labelInstructions;
  private DigitalReadoutInstrument digitalReadoutInstrumentParkingBrake;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label CylinderLabel5;

  public UserPanel()
  {
    this.InitializeComponent();
    this.labels = new ScalingLabel[6]
    {
      this.CylinderOutput1,
      this.CylinderOutput2,
      this.CylinderOutput3,
      this.CylinderOutput4,
      this.CylinderOutput5,
      this.CylinderOutput6
    };
    this.warningManager = new WarningManager(string.Empty, Resources.Message_CompressionTest, this.seekTimeListView.RequiredUserLabelPrefix);
    this.labelInstructions.Text = Resources.Message_StartTheTestWithTheEngineOffAndTheIgnitionOn;
    this.buttonPrint.Click += new EventHandler(this.ButtonPrintClick);
    this.buttonRun.Click += new EventHandler(this.OnExecuteButtonClick);
    SapiManager.GlobalInstance.EquipmentTypeChanged += new EventHandler<EquipmentTypeChangedEventArgs>(this.GlobalInstance_EquipmentTypeChanged);
  }

  private bool ExecuteServiceRoutine(string serviceName, ref Service service)
  {
    bool flag = false;
    if (this.channel == null)
    {
      this.DisplayStatusString($"{Resources.Message_TheMR2IsOfflineCannotExecuteService}{serviceName}\".");
    }
    else
    {
      service = this.channel.Services[serviceName];
      if (service == (Service) null)
      {
        this.DisplayStatusString($"{Resources.Message_TheMR2DoesNotSupportTheServiceRoutine}{serviceName}\".");
      }
      else
      {
        try
        {
          service.Execute(true);
          flag = true;
        }
        catch (CaesarException ex)
        {
          this.DisplayStatusString(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
          this.DisplayStatusString(ex.Message);
        }
      }
    }
    return flag;
  }

  private void GetandDisplayCylinderValue(int cylinder)
  {
    Service service = (Service) null;
    string empty = string.Empty;
    bool flag = false;
    ScalingLabel label = this.labels[cylinder - 1];
    string str;
    if (this.ExecuteServiceRoutine($"RT_SR0207_Automatic_Compression_Detection_Request_Results_Compression_Value_Segment_{cylinder - 1}", ref service) && service != (Service) null && service.OutputValues != null && service.OutputValues.Count > 0)
    {
      float num = (float) service.OutputValues[0].Value;
      flag = (double) num >= 85.0;
      str = $"{num,5}%";
    }
    else
      str = Resources.Message_Error;
    ((Control) label).Text = str;
    if (cylinder == 1)
      this.testSucceed = flag;
    else
      this.testSucceed &= flag;
    this.SetResultString(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Message_Result, (object) cylinder, (object) str, flag ? (object) Resources.Message_ResultPassed : (object) Resources.Message_ResultFailed));
  }

  private void SetResultString(string resultString)
  {
    UserPanel userPanel = this;
    userPanel.resultString = $"{userPanel.resultString}<br />{resultString}";
  }

  private void ClearOutputs()
  {
    this.resultString = string.Empty;
    for (int index = 0; index < this.CylinderCount; ++index)
    {
      ((Control) this.labels[index]).Text = string.Empty;
      this.labels[index].RepresentedState = (ValueState) 0;
    }
  }

  private void DisplayStatusString(string statusString)
  {
    this.LabelLog(this.seekTimeListView.RequiredUserLabelPrefix, statusString);
  }

  private void UpdateButtonStatus()
  {
    this.buttonRun.Enabled = false;
    this.checkmark1.Checked = false;
    if (this.TestIsRunning)
      this.labelInstructions.Text = Resources.Message_TestIsRunning;
    else if (this.channel != null)
    {
      if (!this.EngineIsRunning)
      {
        if (this.digitalReadoutInstrumentParkingBrake.RepresentedState == 1)
        {
          this.buttonRun.Enabled = true;
          this.checkmark1.Checked = true;
          this.labelInstructions.Text = Resources.Message_TheTestIsReadyToBeStarted;
        }
        else
          this.labelInstructions.Text = Resources.Message_ParkingBrakeOn;
      }
      else
        this.labelInstructions.Text = Resources.Message_CannotRunTestWithEngineRunning;
    }
    else
      this.labelInstructions.Text = Resources.Message_TheTestCannotBeRunUntilAnMR2IsConnected;
  }

  protected virtual void OnLoad(EventArgs e)
  {
    ((ContainerControl) this).ParentForm.FormClosing += new FormClosingEventHandler(this.OnFormClosing);
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
  }

  private void OnFormClosing(object sender, FormClosingEventArgs e)
  {
    ((Control) this).Tag = (object) new object[2]
    {
      (object) this.testSucceed,
      (object) this.resultString
    };
    ((ContainerControl) this).ParentForm.FormClosing -= new FormClosingEventHandler(this.OnFormClosing);
  }

  public virtual void OnChannelsChanged()
  {
    this.warningManager.Reset();
    Channel channel = this.GetChannel("MR201T");
    if (channel != this.channel)
      this.channel = channel;
    this.UpdateConnectedEquipmentType();
    this.UpdateButtonStatus();
  }

  public virtual bool CanProvideHtml => !this.TestIsRunning;

  public virtual string ToHtml()
  {
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.Append(Resources.Print_Header);
    stringBuilder.AppendFormat(Resources.Print_Results_String, (object) this.resultString);
    stringBuilder.Append(Resources.Print_Test_History);
    stringBuilder.Append(this.seekTimeListView.Text.Replace(Environment.NewLine, "</ul><ul>"));
    stringBuilder.Append("</p></div>");
    return stringBuilder.ToString();
  }

  private void ButtonPrintClick(object sender, EventArgs e)
  {
    PrintHelper.ShowPrintDialog(Resources.Report_Title, (IProvideHtml) this, (IncludeInfo) 3);
  }

  private void OnExecuteButtonClick(object sender, EventArgs e)
  {
    this.TestIsRunning = true;
    this.UpdateButtonStatus();
    this.RunTest();
    this.TestIsRunning = false;
    this.UpdateButtonStatus();
  }

  private void RunTest()
  {
    if (this.warningManager.RequestContinue())
    {
      Service service = (Service) null;
      this.ClearOutputs();
      if (this.channel == null)
        this.labelInstructions.Text = Resources.Message_TheTestCannotBeRunUntilAnMR2IsConnected;
      else if (!this.ExecuteServiceRoutine("RT_SR0207_Automatic_Compression_Detection_Start", ref service))
        this.DisplayStatusString(Resources.Message_TheTestFailedToRun);
      else if (MessageBox.Show(Resources.Message_PleaseTurnTheIgnitionKey, Resources.Message_AutomaticCompressionTest, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) != DialogResult.OK)
      {
        this.DisplayStatusString(Resources.Message_TheTestWasCancelledByTheUser);
      }
      else
      {
        bool flag = this.ExecuteServiceRoutine("RT_SR0207_Automatic_Compression_Detection_Request_Results_State_Byte", ref service);
        ServiceOutputValue outputValue = service.OutputValues[0];
        if (!flag || outputValue == null)
        {
          this.DisplayStatusString(Resources.Message_TheTestFailedToRun);
        }
        else
        {
          Choice choice = outputValue.Value as Choice;
          if (choice == (object) null || (int) choice.RawValue != 1)
          {
            this.DisplayStatusString(string.Format(Resources.MessageFormat_ErrorOperatorError, (object) outputValue.Value.ToString()));
          }
          else
          {
            for (int cylinder = 1; cylinder <= this.CylinderCount; ++cylinder)
              this.GetandDisplayCylinderValue(cylinder);
            if (this.testSucceed)
            {
              this.DisplayStatusString(Resources.Message_TheTestCompletedSuccessfully);
              this.testSucceed = true;
            }
            else
              this.DisplayStatusString(Resources.Message_TheTestFailedToRun);
          }
        }
      }
    }
    else
      this.DisplayStatusString(Resources.Message_TheTestWasCancelledByTheUser);
  }

  private void GlobalInstance_EquipmentTypeChanged(object sender, EquipmentTypeChangedEventArgs e)
  {
    if (!(e.Category == "Engine"))
      return;
    this.UpdateConnectedEquipmentType();
  }

  private int GetCylinderCount(string equipment)
  {
    return (this.channel == null || this.channel.EcuInfos["CO_CertificationNumber"] == null ? string.Empty : this.channel.EcuInfos["CO_CertificationNumber"].Value).StartsWith("OM924") ? 4 : 6;
  }

  private void UpdateConnectedEquipmentType()
  {
    EquipmentType equipmentType = SapiManager.GlobalInstance.ConnectedEquipment.FirstOrDefault<EquipmentType>((Func<EquipmentType, bool>) (et =>
    {
      ElectronicsFamily family = ((EquipmentType) ref et).Family;
      return ((ElectronicsFamily) ref family).Category == "Engine";
    }));
    if (!EquipmentType.op_Inequality(equipmentType, EquipmentType.Empty))
      return;
    int cylinderCount = this.GetCylinderCount(((EquipmentType) ref equipmentType).Name);
    if (cylinderCount != this.CylinderCount)
    {
      this.CylinderCount = cylinderCount;
      ((Control) this.tableLayoutPanel3).SuspendLayout();
      if (this.CylinderCount == 6)
      {
        ((Control) this.CylinderLabel5).Visible = ((Control) this.CylinderOutput5).Visible = true;
        ((Control) this.CylinderLabel6).Visible = ((Control) this.CylinderOutput6).Visible = true;
        ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.CylinderLabel3, 2, 0);
        ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.CylinderOutput3, 2, 1);
        ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.CylinderLabel4, 0, 2);
        ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.CylinderOutput4, 0, 3);
      }
      else
      {
        ((Control) this.CylinderLabel6).Visible = ((Control) this.CylinderOutput6).Visible = false;
        ((Control) this.CylinderLabel5).Visible = ((Control) this.CylinderOutput5).Visible = false;
        ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.CylinderLabel3, 0, 2);
        ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.CylinderOutput3, 0, 3);
        ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.CylinderLabel4, 1, 2);
        ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.CylinderOutput4, 1, 3);
      }
      ((Control) this.tableLayoutPanel3).ResumeLayout();
    }
  }

  private bool EngineIsRunning => this.engineSpeedDigitalReadoutInstrument.RepresentedState != 1;

  private void StartCondition_RepresentedStateChanged(object sender, EventArgs e)
  {
    this.UpdateButtonStatus();
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanel3 = new TableLayoutPanel();
    this.CylinderLabel1 = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.CylinderLabel2 = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.CylinderLabel3 = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.CylinderOutput3 = new ScalingLabel();
    this.CylinderOutput6 = new ScalingLabel();
    this.CylinderLabel6 = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.CylinderOutput5 = new ScalingLabel();
    this.CylinderLabel5 = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.CylinderOutput4 = new ScalingLabel();
    this.CylinderLabel4 = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.CylinderOutput1 = new ScalingLabel();
    this.CylinderOutput2 = new ScalingLabel();
    this.seekTimeListView = new SeekTimeListView();
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.tableLayoutPanel6 = new TableLayoutPanel();
    this.engineSpeedDigitalReadoutInstrument = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentParkingBrake = new DigitalReadoutInstrument();
    this.label2 = new System.Windows.Forms.Label();
    this.tableLayoutPanel2 = new TableLayoutPanel();
    this.buttonPrint = new Button();
    this.buttonRun = new Button();
    this.checkmark1 = new Checkmark();
    this.labelInstructions = new System.Windows.Forms.Label();
    ((Control) this.tableLayoutPanel3).SuspendLayout();
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this.tableLayoutPanel6).SuspendLayout();
    ((Control) this.tableLayoutPanel2).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel3, "tableLayoutPanel3");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.tableLayoutPanel3, 2);
    ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.CylinderLabel1, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.CylinderLabel2, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.CylinderLabel3, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.CylinderOutput3, 2, 1);
    ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.CylinderOutput6, 2, 3);
    ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.CylinderLabel6, 2, 2);
    ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.CylinderOutput5, 1, 3);
    ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.CylinderLabel5, 1, 2);
    ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.CylinderOutput4, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.CylinderLabel4, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.CylinderOutput1, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.CylinderOutput2, 1, 1);
    ((TableLayoutPanel) this.tableLayoutPanel3).GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
    ((Control) this.tableLayoutPanel3).Name = "tableLayoutPanel3";
    this.CylinderLabel1.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.CylinderLabel1, "CylinderLabel1");
    ((Control) this.CylinderLabel1).Name = "CylinderLabel1";
    this.CylinderLabel1.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.CylinderLabel2.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.CylinderLabel2, "CylinderLabel2");
    ((Control) this.CylinderLabel2).Name = "CylinderLabel2";
    this.CylinderLabel2.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.CylinderLabel3.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.CylinderLabel3, "CylinderLabel3");
    ((Control) this.CylinderLabel3).Name = "CylinderLabel3";
    this.CylinderLabel3.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.CylinderOutput3.Alignment = StringAlignment.Far;
    componentResourceManager.ApplyResources((object) this.CylinderOutput3, "CylinderOutput3");
    this.CylinderOutput3.FontGroup = (string) null;
    this.CylinderOutput3.LineAlignment = StringAlignment.Center;
    ((Control) this.CylinderOutput3).Name = "CylinderOutput3";
    this.CylinderOutput6.Alignment = StringAlignment.Far;
    componentResourceManager.ApplyResources((object) this.CylinderOutput6, "CylinderOutput6");
    this.CylinderOutput6.FontGroup = (string) null;
    this.CylinderOutput6.LineAlignment = StringAlignment.Center;
    ((Control) this.CylinderOutput6).Name = "CylinderOutput6";
    this.CylinderLabel6.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.CylinderLabel6, "CylinderLabel6");
    ((Control) this.CylinderLabel6).Name = "CylinderLabel6";
    this.CylinderLabel6.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.CylinderOutput5.Alignment = StringAlignment.Far;
    componentResourceManager.ApplyResources((object) this.CylinderOutput5, "CylinderOutput5");
    this.CylinderOutput5.FontGroup = (string) null;
    this.CylinderOutput5.LineAlignment = StringAlignment.Center;
    ((Control) this.CylinderOutput5).Name = "CylinderOutput5";
    this.CylinderLabel5.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.CylinderLabel5, "CylinderLabel5");
    ((Control) this.CylinderLabel5).Name = "CylinderLabel5";
    this.CylinderLabel5.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.CylinderOutput4.Alignment = StringAlignment.Far;
    componentResourceManager.ApplyResources((object) this.CylinderOutput4, "CylinderOutput4");
    this.CylinderOutput4.FontGroup = (string) null;
    this.CylinderOutput4.LineAlignment = StringAlignment.Center;
    ((Control) this.CylinderOutput4).Name = "CylinderOutput4";
    this.CylinderLabel4.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.CylinderLabel4, "CylinderLabel4");
    ((Control) this.CylinderLabel4).Name = "CylinderLabel4";
    this.CylinderLabel4.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.CylinderOutput1.Alignment = StringAlignment.Far;
    componentResourceManager.ApplyResources((object) this.CylinderOutput1, "CylinderOutput1");
    this.CylinderOutput1.FontGroup = (string) null;
    this.CylinderOutput1.LineAlignment = StringAlignment.Center;
    ((Control) this.CylinderOutput1).Name = "CylinderOutput1";
    this.CylinderOutput2.Alignment = StringAlignment.Far;
    componentResourceManager.ApplyResources((object) this.CylinderOutput2, "CylinderOutput2");
    this.CylinderOutput2.FontGroup = (string) null;
    this.CylinderOutput2.LineAlignment = StringAlignment.Center;
    ((Control) this.CylinderOutput2).Name = "CylinderOutput2";
    componentResourceManager.ApplyResources((object) this.seekTimeListView, "seekTimeListView");
    this.seekTimeListView.FilterUserLabels = true;
    ((Control) this.seekTimeListView).Name = "seekTimeListView";
    this.seekTimeListView.RequiredUserLabelPrefix = "Relative Compression Test";
    this.seekTimeListView.SelectedTime = new DateTime?();
    this.seekTimeListView.ShowChannelLabels = false;
    this.seekTimeListView.ShowCommunicationsState = false;
    this.seekTimeListView.ShowControlPanel = false;
    this.seekTimeListView.ShowDeviceColumn = false;
    this.seekTimeListView.TimeFormat = "HH:mm:ss.fff";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.tableLayoutPanel3, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.tableLayoutPanel6, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.label2, 1, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.tableLayoutPanel2, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.seekTimeListView, 1, 1);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel6, "tableLayoutPanel6");
    ((TableLayoutPanel) this.tableLayoutPanel6).Controls.Add((Control) this.engineSpeedDigitalReadoutInstrument, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel6).Controls.Add((Control) this.digitalReadoutInstrumentParkingBrake, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel6).GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
    ((Control) this.tableLayoutPanel6).Name = "tableLayoutPanel6";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetRowSpan((Control) this.tableLayoutPanel6, 2);
    componentResourceManager.ApplyResources((object) this.engineSpeedDigitalReadoutInstrument, "engineSpeedDigitalReadoutInstrument");
    this.engineSpeedDigitalReadoutInstrument.FontGroup = (string) null;
    ((SingleInstrumentBase) this.engineSpeedDigitalReadoutInstrument).FreezeValue = false;
    this.engineSpeedDigitalReadoutInstrument.Gradient.Initialize((ValueState) 1, 1);
    this.engineSpeedDigitalReadoutInstrument.Gradient.Modify(1, 1.0, (ValueState) 0);
    ((SingleInstrumentBase) this.engineSpeedDigitalReadoutInstrument).Instrument = new Qualifier((QualifierTypes) 1, "MR201T", "DT_AAS_Engine_Speed");
    ((Control) this.engineSpeedDigitalReadoutInstrument).Name = "engineSpeedDigitalReadoutInstrument";
    ((SingleInstrumentBase) this.engineSpeedDigitalReadoutInstrument).UnitAlignment = StringAlignment.Near;
    this.engineSpeedDigitalReadoutInstrument.RepresentedStateChanged += new EventHandler(this.StartCondition_RepresentedStateChanged);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentParkingBrake, "digitalReadoutInstrumentParkingBrake");
    this.digitalReadoutInstrumentParkingBrake.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentParkingBrake).FreezeValue = false;
    this.digitalReadoutInstrumentParkingBrake.Gradient.Initialize((ValueState) 0, 4);
    this.digitalReadoutInstrumentParkingBrake.Gradient.Modify(1, 0.0, (ValueState) 3);
    this.digitalReadoutInstrumentParkingBrake.Gradient.Modify(2, 1.0, (ValueState) 1);
    this.digitalReadoutInstrumentParkingBrake.Gradient.Modify(3, 2.0, (ValueState) 2);
    this.digitalReadoutInstrumentParkingBrake.Gradient.Modify(4, 3.0, (ValueState) 2);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentParkingBrake).Instrument = new Qualifier((QualifierTypes) 1, "CPC04T", "DT_DSL_Parking_Brake");
    ((Control) this.digitalReadoutInstrumentParkingBrake).Name = "digitalReadoutInstrumentParkingBrake";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentParkingBrake).UnitAlignment = StringAlignment.Near;
    this.digitalReadoutInstrumentParkingBrake.RepresentedStateChanged += new EventHandler(this.StartCondition_RepresentedStateChanged);
    componentResourceManager.ApplyResources((object) this.label2, "label2");
    this.label2.Name = "label2";
    this.label2.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel2, "tableLayoutPanel2");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.tableLayoutPanel2, 2);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.buttonPrint, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.buttonRun, 3, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.checkmark1, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.labelInstructions, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
    ((Control) this.tableLayoutPanel2).Name = "tableLayoutPanel2";
    componentResourceManager.ApplyResources((object) this.buttonPrint, "buttonPrint");
    this.buttonPrint.Name = "buttonPrint";
    this.buttonPrint.UseCompatibleTextRendering = true;
    this.buttonPrint.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.buttonRun, "buttonRun");
    this.buttonRun.Name = "buttonRun";
    this.buttonRun.UseCompatibleTextRendering = true;
    this.buttonRun.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.checkmark1, "checkmark1");
    ((Control) this.checkmark1).Name = "checkmark1";
    componentResourceManager.ApplyResources((object) this.labelInstructions, "labelInstructions");
    this.labelInstructions.Name = "labelInstructions";
    this.labelInstructions.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("Panel_CompressionTest");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel1);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanel3).ResumeLayout(false);
    ((Control) this.tableLayoutPanel3).PerformLayout();
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this.tableLayoutPanel6).ResumeLayout(false);
    ((Control) this.tableLayoutPanel2).ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }
}
