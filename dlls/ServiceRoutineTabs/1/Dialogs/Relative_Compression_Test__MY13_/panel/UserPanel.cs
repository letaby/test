// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Relative_Compression_Test__MY13_.panel.UserPanel
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
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Relative_Compression_Test__MY13_.panel;

public class UserPanel : CustomPanel, IProvideHtml
{
  private const string ServiceRoutineStart = "RT_SR006_Automatic_Compression_Test_Start_Status";
  private const string ServiceRoutineStop = "RT_SR006_Automatic_Compression_Test_Stop_acd_activate_status_bit_0";
  private const string ServiceRoutineStatus = "RT_SR006_Automatic_Compression_Test_Request_Results_acd_activate_status_bit_0";
  private ScalingLabel[] labels;
  private int CylinderCount = 6;
  private bool HasEngineBrakes = true;
  private WarningManager warningManager;
  private string resultString = Resources.Message_The_Test_Has_Not_Started;
  private bool resetJakeBrakes;
  private bool TestIsRunning;
  private Channel channel;
  private bool testSucceed = false;
  private DigitalReadoutInstrument DigitalReadoutInstrumentVehicleCheckStatus;
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
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label labelInstructions;
  private TableLayoutPanel tableLayoutPanel2;
  private Button btnClose;
  private Button buttonRun;
  private Button buttonPrint;
  private ScalingLabel scalingLabelHasEngineBrakes;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label labelHasEngineBrakesTitle;
  private DigitalReadoutInstrument engineSpeedDigitalReadoutInstrument;
  private System.Windows.Forms.Label label2;
  private TableLayoutPanel tableLayoutPanel4;
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
    ((Control) this.labelInstructions).Text = Resources.Message_StartTheTestWithTheEngineOffAndTheIgnitionOn;
    this.buttonRun.Click += new EventHandler(this.OnExecuteButtonClick);
    this.buttonPrint.Click += new EventHandler(this.ButtonPrintClick);
    SapiManager.GlobalInstance.EquipmentTypeChanged += new EventHandler<EquipmentTypeChangedEventArgs>(this.GlobalInstance_EquipmentTypeChanged);
  }

  private bool ExecuteServiceRoutine(string serviceName, ref Service service)
  {
    bool flag = false;
    if (this.channel == null)
    {
      this.DisplayStatusString($"{Resources.Message_TheMCMIsOfflineCannotExecuteService}{serviceName}\".");
    }
    else
    {
      service = this.channel.Services[serviceName];
      if (service == (Service) null)
      {
        this.DisplayStatusString($"{Resources.Message_TheMCMDoesNotSupportTheServiceRoutine}{serviceName}\".");
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
    if (this.ExecuteServiceRoutine($"RT_SR006_Automatic_Compression_Test_Request_Results_acd_cyl_{cylinder}_compression_value", ref service) && service != (Service) null && service.OutputValues != null && service.OutputValues.Count > 0)
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
    if (this.TestIsRunning)
      ((Control) this.labelInstructions).Text = Resources.Message_TestIsRunning;
    else if (this.channel != null)
    {
      if (this.channel.Parameters.HaveBeenReadFromEcu)
      {
        if (!this.EngineIsRunning)
        {
          if (this.DigitalReadoutInstrumentVehicleCheckStatus.RepresentedState == 1)
          {
            this.buttonRun.Enabled = true;
            ((Control) this.labelInstructions).Text = Resources.Message_TheTestIsReadyToBeStarted;
          }
          else
            ((Control) this.labelInstructions).Text = Resources.Message_ParkingBrakeOnTransInNeutral;
        }
        else
          ((Control) this.labelInstructions).Text = Resources.Message_CannotRunTestWithEngineRunning;
      }
      else
        ((Control) this.labelInstructions).Text = Resources.Message_ReadingEngineBrakeAvailabilityStatus;
    }
    else
      ((Control) this.labelInstructions).Text = Resources.Message_TheTestCannotBeRunUntilAnMCMIsConnected;
  }

  protected virtual void OnLoad(EventArgs e)
  {
    ((ContainerControl) this).ParentForm.FormClosing += new FormClosingEventHandler(this.OnFormClosing);
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
  }

  private void OnFormClosing(object sender, FormClosingEventArgs e)
  {
    if (this.HasEngineBrakes)
      this.RestoreJakeBrake();
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
    Channel channel = this.GetChannel("MCM21T");
    if (channel != this.channel)
    {
      if (this.channel != null)
        this.channel.Parameters.ParametersReadCompleteEvent -= new ParametersReadCompleteEventHandler(this.Parameters_ParametersReadCompleteEvent);
      this.channel = channel;
      if (this.channel != null)
      {
        this.channel.Parameters.ParametersReadCompleteEvent += new ParametersReadCompleteEventHandler(this.Parameters_ParametersReadCompleteEvent);
        if (!this.channel.Parameters.HaveBeenReadFromEcu)
          this.channel.Parameters.Read(false);
        else
          this.UpdateEngineBrakeAvailibility();
      }
    }
    this.UpdateConnectedEquipmentType();
    this.UpdateButtonStatus();
  }

  private bool DisableJakeBrake()
  {
    if (this.channel == null)
      return false;
    try
    {
      if (this.HasEngineBrakes)
      {
        Service service = this.channel.Services["RT_SR003_PWM_Routine_by_Function_Start_Function_Name"];
        if (service != (Service) null)
        {
          service.InputValues[0].Value = (object) service.Choices.GetItemFromRawValue((object) 6);
          service.InputValues[1].Value = (object) 0;
          service.InputValues[2].Value = (object) int.MaxValue;
          service.Execute(true);
          this.resetJakeBrakes = true;
          service.InputValues[0].Value = (object) service.Choices.GetItemFromRawValue((object) 7);
          service.InputValues[1].Value = (object) 0;
          service.InputValues[2].Value = (object) int.MaxValue;
          service.Execute(true);
        }
      }
    }
    catch (CaesarException ex)
    {
      this.LabelLog(this.seekTimeListView.RequiredUserLabelPrefix, ex.Message);
      return false;
    }
    return true;
  }

  private void RestoreJakeBrake()
  {
    try
    {
      if (this.channel == null || !this.resetJakeBrakes || !this.HasEngineBrakes)
        return;
      Service service = this.channel.Services["RT_SR003_PWM_Routine_by_Function_Stop_Function_Name"];
      if (service != (Service) null)
      {
        service.InputValues[0].Value = (object) service.Choices.GetItemFromRawValue((object) 6);
        service.Execute(true);
        service.InputValues[0].Value = (object) service.Choices.GetItemFromRawValue((object) 7);
        service.Execute(true);
        this.resetJakeBrakes = false;
      }
    }
    catch (CaesarException ex)
    {
    }
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
        ((Control) this.labelInstructions).Text = Resources.Message_TheTestCannotBeRunUntilAnMCMIsConnected;
      else if (!this.DisableJakeBrake())
      {
        this.DisplayStatusString(Resources.Message_TestWasUnableToStartBecauseTheEngineBrakeIsEnabled);
      }
      else
      {
        bool flag1 = this.ExecuteServiceRoutine("RT_SR006_Automatic_Compression_Test_Start_Status", ref service);
        ServiceOutputValue outputValue1 = service.OutputValues[0];
        if (!flag1 || outputValue1 == null)
        {
          this.DisplayStatusString(Resources.Message_TheTestFailedToRun);
        }
        else
        {
          Choice choice1 = outputValue1.Value as Choice;
          bool flag2;
          if (choice1 == (object) null || (int) choice1.RawValue != 1)
          {
            this.DisplayStatusString($"{Resources.Message_TheTestFailedToRun} {outputValue1.Value.ToString()}");
            flag2 = this.ExecuteServiceRoutine("RT_SR006_Automatic_Compression_Test_Stop_acd_activate_status_bit_0", ref service);
          }
          else if (MessageBox.Show(Resources.Message_PleaseTurnTheIgnitionKey, Resources.Message_AutomaticCompressionTest, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) != DialogResult.OK)
          {
            this.DisplayStatusString(Resources.Message_TheTestWasCancelledByTheUser);
            flag2 = this.ExecuteServiceRoutine("RT_SR006_Automatic_Compression_Test_Stop_acd_activate_status_bit_0", ref service);
          }
          else
          {
            bool flag3 = this.ExecuteServiceRoutine("RT_SR006_Automatic_Compression_Test_Request_Results_acd_activate_status_bit_0", ref service);
            ServiceOutputValue outputValue2 = service.OutputValues[0];
            if (!flag3 || outputValue2 == null)
            {
              this.DisplayStatusString(Resources.Message_TheTestFailedToRun);
            }
            else
            {
              Choice choice2 = outputValue2.Value as Choice;
              if (choice2 == (object) null || (int) choice2.RawValue != 1)
              {
                this.DisplayStatusString(string.Format(Resources.MessageFormat_ErrorOperatorError, (object) outputValue2.Value.ToString()));
                flag2 = this.ExecuteServiceRoutine("RT_SR006_Automatic_Compression_Test_Stop_acd_activate_status_bit_0", ref service);
              }
              else
              {
                for (int cylinder = 1; cylinder <= this.CylinderCount; ++cylinder)
                  this.GetandDisplayCylinderValue(cylinder);
                if (this.ExecuteServiceRoutine("RT_SR006_Automatic_Compression_Test_Stop_acd_activate_status_bit_0", ref service))
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
    switch (equipment)
    {
      case "DD5":
      case "MDEG 4-Cylinder Tier4":
        return 4;
      default:
        return 6;
    }
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
      ((Control) this.CylinderLabel5).Visible = ((Control) this.CylinderOutput5).Visible = this.CylinderCount == 6;
      ((Control) this.CylinderLabel6).Visible = ((Control) this.CylinderOutput6).Visible = this.CylinderCount == 6;
    }
  }

  private void Parameters_ParametersReadCompleteEvent(object sender, ResultEventArgs e)
  {
    this.UpdateEngineBrakeAvailibility();
    this.UpdateButtonStatus();
  }

  private void UpdateEngineBrakeAvailibility()
  {
    if (this.channel != null && this.channel.Parameters.HaveBeenReadFromEcu)
    {
      this.HasEngineBrakes = this.channel.Parameters.Where<Parameter>((Func<Parameter, bool>) (p => p.GroupQualifier.StartsWith("VCD_PGR001_PropValve", StringComparison.Ordinal))).Any<Parameter>((Func<Parameter, bool>) (p => p.Value != null && (object) (p.Value as Choice) != null && Convert.ToInt32(((Choice) p.Value).RawValue) == 6));
      ((Control) this.scalingLabelHasEngineBrakes).Text = this.HasEngineBrakes ? Resources.Message_HasEngineBrake : Resources.Message_NoEngineBrake;
    }
    else
      ((Control) this.scalingLabelHasEngineBrakes).Text = Resources.Message_Unknown;
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
    this.seekTimeListView = new SeekTimeListView();
    this.labelInstructions = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.CylinderLabel4 = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.CylinderLabel5 = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.CylinderLabel6 = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.CylinderOutput6 = new ScalingLabel();
    this.CylinderOutput5 = new ScalingLabel();
    this.CylinderOutput4 = new ScalingLabel();
    this.CylinderLabel3 = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.CylinderOutput3 = new ScalingLabel();
    this.CylinderLabel2 = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.CylinderOutput2 = new ScalingLabel();
    this.CylinderLabel1 = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.CylinderOutput1 = new ScalingLabel();
    this.tableLayoutPanel2 = new TableLayoutPanel();
    this.btnClose = new Button();
    this.buttonRun = new Button();
    this.buttonPrint = new Button();
    this.engineSpeedDigitalReadoutInstrument = new DigitalReadoutInstrument();
    this.label2 = new System.Windows.Forms.Label();
    this.DigitalReadoutInstrumentVehicleCheckStatus = new DigitalReadoutInstrument();
    this.tableLayoutPanel4 = new TableLayoutPanel();
    this.labelHasEngineBrakesTitle = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.scalingLabelHasEngineBrakes = new ScalingLabel();
    ((Control) this.tableLayoutPanel3).SuspendLayout();
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this.tableLayoutPanel2).SuspendLayout();
    ((Control) this.tableLayoutPanel4).SuspendLayout();
    ((Control) this).SuspendLayout();
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.tableLayoutPanel3, 2);
    ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.seekTimeListView, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.labelInstructions, 0, 0);
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel3, "tableLayoutPanel3");
    ((Control) this.tableLayoutPanel3).Name = "tableLayoutPanel3";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetRowSpan((Control) this.tableLayoutPanel3, 4);
    componentResourceManager.ApplyResources((object) this.seekTimeListView, "seekTimeListView");
    this.seekTimeListView.FilterUserLabels = true;
    ((Control) this.seekTimeListView).Name = "seekTimeListView";
    this.seekTimeListView.RequiredUserLabelPrefix = "Relative Compression Test";
    ((TableLayoutPanel) this.tableLayoutPanel3).SetRowSpan((Control) this.seekTimeListView, 2);
    this.seekTimeListView.SelectedTime = new DateTime?();
    this.seekTimeListView.ShowChannelLabels = false;
    this.seekTimeListView.ShowCommunicationsState = false;
    this.seekTimeListView.ShowControlPanel = false;
    this.seekTimeListView.ShowDeviceColumn = false;
    this.seekTimeListView.TimeFormat = "HH:mm:ss.fff";
    this.labelInstructions.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.labelInstructions, "labelInstructions");
    ((Control) this.labelInstructions).Name = "labelInstructions";
    this.labelInstructions.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.CylinderLabel4, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.CylinderLabel5, 1, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.CylinderLabel6, 2, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.CylinderOutput6, 2, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.CylinderOutput5, 1, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.CylinderOutput4, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.CylinderLabel3, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.CylinderOutput3, 2, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.CylinderLabel2, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.CylinderOutput2, 1, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.CylinderLabel1, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.CylinderOutput1, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.tableLayoutPanel2, 2, 9);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.tableLayoutPanel3, 1, 4);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.engineSpeedDigitalReadoutInstrument, 0, 7);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.label2, 1, 8);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.DigitalReadoutInstrumentVehicleCheckStatus, 0, 5);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.tableLayoutPanel4, 0, 4);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    this.CylinderLabel4.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.CylinderLabel4, "CylinderLabel4");
    ((Control) this.CylinderLabel4).Name = "CylinderLabel4";
    this.CylinderLabel4.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.CylinderLabel5.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.CylinderLabel5, "CylinderLabel5");
    ((Control) this.CylinderLabel5).Name = "CylinderLabel5";
    this.CylinderLabel5.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.CylinderLabel6.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.CylinderLabel6, "CylinderLabel6");
    ((Control) this.CylinderLabel6).Name = "CylinderLabel6";
    this.CylinderLabel6.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.CylinderOutput6.Alignment = StringAlignment.Far;
    componentResourceManager.ApplyResources((object) this.CylinderOutput6, "CylinderOutput6");
    this.CylinderOutput6.FontGroup = (string) null;
    this.CylinderOutput6.LineAlignment = StringAlignment.Center;
    ((Control) this.CylinderOutput6).Name = "CylinderOutput6";
    this.CylinderOutput5.Alignment = StringAlignment.Far;
    componentResourceManager.ApplyResources((object) this.CylinderOutput5, "CylinderOutput5");
    this.CylinderOutput5.FontGroup = (string) null;
    this.CylinderOutput5.LineAlignment = StringAlignment.Center;
    ((Control) this.CylinderOutput5).Name = "CylinderOutput5";
    this.CylinderOutput4.Alignment = StringAlignment.Far;
    componentResourceManager.ApplyResources((object) this.CylinderOutput4, "CylinderOutput4");
    this.CylinderOutput4.FontGroup = (string) null;
    this.CylinderOutput4.LineAlignment = StringAlignment.Center;
    ((Control) this.CylinderOutput4).Name = "CylinderOutput4";
    this.CylinderLabel3.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.CylinderLabel3, "CylinderLabel3");
    ((Control) this.CylinderLabel3).Name = "CylinderLabel3";
    this.CylinderLabel3.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.CylinderOutput3.Alignment = StringAlignment.Far;
    componentResourceManager.ApplyResources((object) this.CylinderOutput3, "CylinderOutput3");
    this.CylinderOutput3.FontGroup = (string) null;
    this.CylinderOutput3.LineAlignment = StringAlignment.Center;
    ((Control) this.CylinderOutput3).Name = "CylinderOutput3";
    this.CylinderLabel2.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.CylinderLabel2, "CylinderLabel2");
    ((Control) this.CylinderLabel2).Name = "CylinderLabel2";
    this.CylinderLabel2.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.CylinderOutput2.Alignment = StringAlignment.Far;
    componentResourceManager.ApplyResources((object) this.CylinderOutput2, "CylinderOutput2");
    this.CylinderOutput2.FontGroup = (string) null;
    this.CylinderOutput2.LineAlignment = StringAlignment.Center;
    ((Control) this.CylinderOutput2).Name = "CylinderOutput2";
    this.CylinderLabel1.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.CylinderLabel1, "CylinderLabel1");
    ((Control) this.CylinderLabel1).Name = "CylinderLabel1";
    this.CylinderLabel1.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.CylinderOutput1.Alignment = StringAlignment.Far;
    componentResourceManager.ApplyResources((object) this.CylinderOutput1, "CylinderOutput1");
    this.CylinderOutput1.FontGroup = (string) null;
    this.CylinderOutput1.LineAlignment = StringAlignment.Center;
    ((Control) this.CylinderOutput1).Name = "CylinderOutput1";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel2, "tableLayoutPanel2");
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.btnClose, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.buttonRun, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.buttonPrint, 0, 0);
    ((Control) this.tableLayoutPanel2).Name = "tableLayoutPanel2";
    componentResourceManager.ApplyResources((object) this.btnClose, "btnClose");
    this.btnClose.DialogResult = DialogResult.OK;
    this.btnClose.Name = "btnClose";
    this.btnClose.UseCompatibleTextRendering = true;
    this.btnClose.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.buttonRun, "buttonRun");
    this.buttonRun.Name = "buttonRun";
    this.buttonRun.UseCompatibleTextRendering = true;
    this.buttonRun.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.buttonPrint, "buttonPrint");
    this.buttonPrint.Name = "buttonPrint";
    this.buttonPrint.UseCompatibleTextRendering = true;
    this.buttonPrint.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.engineSpeedDigitalReadoutInstrument, "engineSpeedDigitalReadoutInstrument");
    this.engineSpeedDigitalReadoutInstrument.FontGroup = (string) null;
    ((SingleInstrumentBase) this.engineSpeedDigitalReadoutInstrument).FreezeValue = false;
    this.engineSpeedDigitalReadoutInstrument.Gradient.Initialize((ValueState) 1, 1);
    this.engineSpeedDigitalReadoutInstrument.Gradient.Modify(1, 1.0, (ValueState) 0);
    ((SingleInstrumentBase) this.engineSpeedDigitalReadoutInstrument).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS010_Engine_Speed");
    ((Control) this.engineSpeedDigitalReadoutInstrument).Name = "engineSpeedDigitalReadoutInstrument";
    ((SingleInstrumentBase) this.engineSpeedDigitalReadoutInstrument).UnitAlignment = StringAlignment.Near;
    this.engineSpeedDigitalReadoutInstrument.RepresentedStateChanged += new EventHandler(this.StartCondition_RepresentedStateChanged);
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.label2, 2);
    componentResourceManager.ApplyResources((object) this.label2, "label2");
    this.label2.Name = "label2";
    this.label2.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.DigitalReadoutInstrumentVehicleCheckStatus, "DigitalReadoutInstrumentVehicleCheckStatus");
    this.DigitalReadoutInstrumentVehicleCheckStatus.FontGroup = (string) null;
    ((SingleInstrumentBase) this.DigitalReadoutInstrumentVehicleCheckStatus).FreezeValue = false;
    this.DigitalReadoutInstrumentVehicleCheckStatus.Gradient.Initialize((ValueState) 0, 4);
    this.DigitalReadoutInstrumentVehicleCheckStatus.Gradient.Modify(1, 0.0, (ValueState) 3);
    this.DigitalReadoutInstrumentVehicleCheckStatus.Gradient.Modify(2, 1.0, (ValueState) 1);
    this.DigitalReadoutInstrumentVehicleCheckStatus.Gradient.Modify(3, 2.0, (ValueState) 0);
    this.DigitalReadoutInstrumentVehicleCheckStatus.Gradient.Modify(4, 3.0, (ValueState) 0);
    ((SingleInstrumentBase) this.DigitalReadoutInstrumentVehicleCheckStatus).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "DT_DS019_Vehicle_Check_Status");
    ((Control) this.DigitalReadoutInstrumentVehicleCheckStatus).Name = "DigitalReadoutInstrumentVehicleCheckStatus";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetRowSpan((Control) this.DigitalReadoutInstrumentVehicleCheckStatus, 2);
    ((SingleInstrumentBase) this.DigitalReadoutInstrumentVehicleCheckStatus).UnitAlignment = StringAlignment.Near;
    this.DigitalReadoutInstrumentVehicleCheckStatus.RepresentedStateChanged += new EventHandler(this.StartCondition_RepresentedStateChanged);
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel4, "tableLayoutPanel4");
    ((TableLayoutPanel) this.tableLayoutPanel4).Controls.Add((Control) this.labelHasEngineBrakesTitle, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel4).Controls.Add((Control) this.scalingLabelHasEngineBrakes, 0, 1);
    ((Control) this.tableLayoutPanel4).Name = "tableLayoutPanel4";
    this.labelHasEngineBrakesTitle.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.labelHasEngineBrakesTitle, "labelHasEngineBrakesTitle");
    ((Control) this.labelHasEngineBrakesTitle).Name = "labelHasEngineBrakesTitle";
    this.labelHasEngineBrakesTitle.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.scalingLabelHasEngineBrakes.Alignment = StringAlignment.Far;
    componentResourceManager.ApplyResources((object) this.scalingLabelHasEngineBrakes, "scalingLabelHasEngineBrakes");
    this.scalingLabelHasEngineBrakes.FontGroup = (string) null;
    this.scalingLabelHasEngineBrakes.LineAlignment = StringAlignment.Center;
    ((Control) this.scalingLabelHasEngineBrakes).Name = "scalingLabelHasEngineBrakes";
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("Panel_CompressionTest");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel1);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanel3).ResumeLayout(false);
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this.tableLayoutPanel1).PerformLayout();
    ((Control) this.tableLayoutPanel2).ResumeLayout(false);
    ((Control) this.tableLayoutPanel4).ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }
}
