// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Procedures.Detroit_Maintenance_System.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Help;
using DetroitDiesel.UnitConversion;
using DetroitDiesel.Windows.Forms;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Procedures.Detroit_Maintenance_System.panel;

public class UserPanel : CustomPanel
{
  private bool customEngineMessage = false;
  private List<Checkmark> enginePreconditions = new List<Checkmark>();
  private bool customTransmissionMessage = false;
  private List<Checkmark> transmissionPreconditions = new List<Checkmark>();
  private bool customAxle1Message = false;
  private List<Checkmark> axle1Preconditions = new List<Checkmark>();
  private bool customAxle2Message = false;
  private List<Checkmark> axle2Preconditions = new List<Checkmark>();
  private List<Tuple<string, string>> oilSystems = new List<Tuple<string, string>>((IEnumerable<Tuple<string, string>>) new Tuple<string, string>[4]
  {
    new Tuple<string, string>("Engine", "MOT"),
    new Tuple<string, string>("Transmission", "GET"),
    new Tuple<string, string>("Rear_axle_1", "HA1"),
    new Tuple<string, string>("Rear_axle_2", "HA2")
  });
  private List<Tuple<string, string, string, string, int, bool>> oilSystemValues = new List<Tuple<string, string, string, string, int, bool>>();
  private bool resetInProgress = false;
  private HtmlElement systemInputPane;
  private Channel ms01t = (Channel) null;
  private Channel cgw05t = (Channel) null;
  private Channel activeChannel = (Channel) null;
  private static string resetServiceQualifier = "DL_Reset_service_information_selected_channel";
  private static string minimumOperatingTimeQualifier = "Minimum_operating_time_reset_PAR_BzMinRs_FZG";
  private static string minimumDrivenDistanceQualifier = "Minimum_driven_distance_reset_PAR_FsMinRs_FZG";
  private TableLayoutPanel tableLayoutPanelMain;
  private TableLayoutPanel tableLayoutPanelEngine;
  private TableLayoutPanel tableLayoutPanelEngineStatus;
  private TableLayoutPanel tableLayoutPanel6;
  private Checkmark checkmarkVehicleSpeedEngine;
  private System.Windows.Forms.Label label2;
  private TableLayoutPanel tableLayoutPanel2;
  private Checkmark checkmarkIgnitionSwitchEngine;
  private System.Windows.Forms.Label labelIgnitionSwitchEngine;
  private TableLayoutPanel tableLayoutPanel4;
  private System.Windows.Forms.Label labelOperatingTimeEngine;
  private Checkmark checkmarkOperatingTimeEngine;
  private TableLayoutPanel tableLayoutPanel1;
  private Checkmark checkmarkSystemActiveEngine;
  private System.Windows.Forms.Label labelSystemActiveEngine;
  private TableLayoutPanel tableLayoutPanel3;
  private System.Windows.Forms.Label labelDrivenDistanceEngine;
  private Checkmark checkmarkDrivenDistanceEngine;
  private WebBrowser webBrowsersystemInputs;
  private TableLayoutPanel tableLayoutPanelEngineStatusMessage;
  private System.Windows.Forms.Label labelEngineStatus;
  private RunServiceButton runServiceButtonNewOilFilterEngine;
  private Checkmark checkmarkEngineStatus;
  private TableLayoutPanel tableLayoutPanelHeader;
  private DigitalReadoutInstrument digitalReadoutInstrumentVehicleSpeed;
  private DigitalReadoutInstrument digitalReadoutInstrumentIgnitionSwitch;
  private DigitalReadoutInstrument digitalReadoutInstrumentMinimumDrivenDistance;
  private Panel panelEngine;
  private Panel panelTransmission;
  private TableLayoutPanel tableLayoutPanelTransmission;
  private TableLayoutPanel tableLayoutPanelTransmissionConditions;
  private TableLayoutPanel tableLayoutPanel10;
  private System.Windows.Forms.Label label4;
  private Checkmark checkmarkOperatingTimeTransmission;
  private TableLayoutPanel tableLayoutPanel11;
  private Checkmark checkmarkSystemActiveTransmission;
  private System.Windows.Forms.Label label5;
  private TableLayoutPanel tableLayoutPanel12;
  private System.Windows.Forms.Label label6;
  private Checkmark checkmarkIgnitionSwitchTransmission;
  private TableLayoutPanel tableLayoutPanel13;
  private Checkmark checkmarkVehicleSpeedTransmission;
  private System.Windows.Forms.Label label7;
  private TableLayoutPanel tableLayoutPanel14;
  private System.Windows.Forms.Label label8;
  private Checkmark checkmarkDrivenDistanceTransmission;
  private TableLayoutPanel tableLayoutPanelTransmissionStatus;
  private System.Windows.Forms.Label labelTransmissionStatus;
  private Checkmark checkmarkTransmissionStatus;
  private RunServiceButton runServiceButtonNewOilFilterTransmission;
  private System.Windows.Forms.Label label10;
  private Panel panelAxle1;
  private TableLayoutPanel tableLayoutPanelAxle1;
  private TableLayoutPanel tableLayoutPanelAxle1Conditions;
  private TableLayoutPanel tableLayoutPanel19;
  private System.Windows.Forms.Label label1;
  private Checkmark checkmarkOperatingTimeAxle1;
  private TableLayoutPanel tableLayoutPanel20;
  private Checkmark checkmarkSystemActiveAxle1;
  private System.Windows.Forms.Label label9;
  private TableLayoutPanel tableLayoutPanel21;
  private System.Windows.Forms.Label label11;
  private Checkmark checkmarkIgnitionSwitchAxle1;
  private TableLayoutPanel tableLayoutPanel22;
  private Checkmark checkmarkVehicleSpeedAxle1;
  private System.Windows.Forms.Label label12;
  private TableLayoutPanel tableLayoutPanel23;
  private System.Windows.Forms.Label label13;
  private Checkmark checkmarkDrivenDistanceAxle1;
  private TableLayoutPanel tableLayoutPanelAxle1Status;
  private System.Windows.Forms.Label labelAxle1Status;
  private Checkmark checkmarkAxle1Status;
  private RunServiceButton runServiceButtonNewOilFilterAxle1;
  private System.Windows.Forms.Label label15;
  private Panel panelAxle2;
  private TableLayoutPanel tableLayoutPanelAxle2;
  private TableLayoutPanel Axle2Conditions;
  private TableLayoutPanel tableLayoutPanel28;
  private System.Windows.Forms.Label label14;
  private Checkmark checkmarkOperatingTimeAxle2;
  private TableLayoutPanel tableLayoutPanel29;
  private Checkmark checkmarkSystemActiveAxle2;
  private System.Windows.Forms.Label label16;
  private TableLayoutPanel tableLayoutPanel30;
  private System.Windows.Forms.Label label17;
  private Checkmark checkmarkIgnitionSwitchAxle2;
  private TableLayoutPanel tableLayoutPanel31;
  private Checkmark checkmarkVehicleSpeedAxle2;
  private System.Windows.Forms.Label label18;
  private TableLayoutPanel tableLayoutPanel32;
  private System.Windows.Forms.Label label19;
  private Checkmark checkmarkDrivenDistanceAxle2;
  private TableLayoutPanel tableLayoutPanelAxle2Status;
  private System.Windows.Forms.Label labelAxle2Status;
  private Checkmark checkmarkAxle2Status;
  private RunServiceButton runServiceButtonNewOilFilterAxle2;
  private System.Windows.Forms.Label label21;
  private TableLayoutPanel tableLayoutPanel5;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label label20;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label label22;
  private ScalingLabel scalingLabelMinimumTime;
  private System.Windows.Forms.Label label3;

  public UserPanel()
  {
    this.InitializeComponent();
    this.oilSystemValues.Add(new Tuple<string, string, string, string, int, bool>("All", "DT_STO_{0}_Service_Life_Dates_EEPROM_Driven_distance_LLD_Fs_{1}", Resources.Message_DrivenDistance, "km", 1000, true));
    this.oilSystemValues.Add(new Tuple<string, string, string, string, int, bool>("All", "DT_STO_{0}_Service_Life_Dates_EEPROM_Operating_time_LLD_Bz_{1}", Resources.Message_OperatingTime, "days", 86400, true));
    this.oilSystemValues.Add(new Tuple<string, string, string, string, int, bool>("All", "Channel_activation_PAR_KanAktiv_{1}", Resources.Message_SystemActive, "", 1, true));
    this.oilSystemValues.Add(new Tuple<string, string, string, string, int, bool>("All", "DT_STO_{0}_Prediction_data_EEPROM_Remaining_driving_distance_PRD_RFS_{1}", Resources.Message_RemainingDrivingDistance, "km", 1000, true));
    this.oilSystemValues.Add(new Tuple<string, string, string, string, int, bool>("All", "DT_STO_{0}_Prediction_data_EEPROM_Remaining_operating_time_PRD_RBZ_{1}", Resources.Message_RemainingOperatingTime, "days", 86400, true));
    this.oilSystemValues.Add(new Tuple<string, string, string, string, int, bool>("All", "DT_STO_{0}_Service_Life_Dates_EEPROM_Life_cycle_consumption_relative_LLD_LDVRel_{1}", Resources.Message_LifeCycleConsumption, "%", 1, false));
    this.oilSystemValues.Add(new Tuple<string, string, string, string, int, bool>("Engine", "DT_STO_{0}_Service_Life_Dates_EEPROM_Life_cycle_consumption_relative_due_to_load_accumulative_LLD_LDVRelBelAkk_{1}", Resources.Message_LoadLifeCycleConsumption, "%", 1, false));
    this.oilSystemValues.Add(new Tuple<string, string, string, string, int, bool>("Date", "DT_STO_{0}_Prediction_data_EEPROM_Maintenance_date_year_PRD_WartJahr_{1}", Resources.Message_MaintenanceDateYear, "", 1, true));
    this.oilSystemValues.Add(new Tuple<string, string, string, string, int, bool>("Date", "DT_STO_{0}_Prediction_data_EEPROM_Maintenance_date_month_PRD_WartMonat_{1}", Resources.Message_MaintenanceDateMonth, "", 1, true));
    this.oilSystemValues.Add(new Tuple<string, string, string, string, int, bool>("Date", "DT_STO_{0}_Prediction_data_EEPROM_Maintenance_date_day_PRD_WartTag_{1}", Resources.Message_MaintenanceDateDay, "", 1, true));
    this.enginePreconditions.AddRange((IEnumerable<Checkmark>) new Checkmark[5]
    {
      this.checkmarkIgnitionSwitchEngine,
      this.checkmarkVehicleSpeedEngine,
      this.checkmarkSystemActiveEngine,
      this.checkmarkDrivenDistanceEngine,
      this.checkmarkOperatingTimeEngine
    });
    this.transmissionPreconditions.AddRange((IEnumerable<Checkmark>) new Checkmark[5]
    {
      this.checkmarkIgnitionSwitchTransmission,
      this.checkmarkVehicleSpeedTransmission,
      this.checkmarkSystemActiveTransmission,
      this.checkmarkDrivenDistanceTransmission,
      this.checkmarkOperatingTimeTransmission
    });
    this.axle1Preconditions.AddRange((IEnumerable<Checkmark>) new Checkmark[5]
    {
      this.checkmarkIgnitionSwitchAxle1,
      this.checkmarkVehicleSpeedAxle1,
      this.checkmarkSystemActiveAxle1,
      this.checkmarkDrivenDistanceAxle1,
      this.checkmarkOperatingTimeAxle1
    });
    this.axle2Preconditions.AddRange((IEnumerable<Checkmark>) new Checkmark[5]
    {
      this.checkmarkIgnitionSwitchAxle2,
      this.checkmarkVehicleSpeedAxle2,
      this.checkmarkSystemActiveAxle2,
      this.checkmarkDrivenDistanceAxle2,
      this.checkmarkOperatingTimeAxle2
    });
    this.webBrowsersystemInputs.DocumentText = $"<HTML><HEAD><meta http-equiv='X-UA-Compatible' content='IE=edge'>{base.StyleSheet}\n</HEAD><BODY><DIV id=\"systemInputPane\" name=\"systemInputPane\" class=\"standard\"><br></DIV></BODY></HTML>";
    this.ReadParameters();
    base.OnChannelsChanged();
    this.UpdateWebBrowser();
    this.UpdateUI();
  }

  private void UpdateUI()
  {
    Parameter parameter = (Parameter) null;
    if (this.activeChannel != null && (parameter = this.activeChannel.Parameters[UserPanel.minimumOperatingTimeQualifier]) != null && parameter.Value != null && !string.IsNullOrEmpty(parameter.Value.ToString()))
      ((Control) this.scalingLabelMinimumTime).Text = UserPanel.ConvertAndScaleValue(parameter.Value.ToString(), "", 1440);
    else
      ((Control) this.scalingLabelMinimumTime).Text = Resources.Message_Unavaiable;
    this.UpdateEngineConditions();
    this.UpdateTransmissionConditions();
    this.UpdateAxle1Conditions();
    this.UpdateAxle2Conditions();
  }

  private void UpdateEngineConditions()
  {
    this.checkmarkIgnitionSwitchEngine.CheckState = this.digitalReadoutInstrumentIgnitionSwitch.RepresentedState == 1 ? CheckState.Checked : CheckState.Unchecked;
    this.checkmarkVehicleSpeedEngine.CheckState = this.digitalReadoutInstrumentVehicleSpeed.RepresentedState == 1 ? CheckState.Checked : CheckState.Unchecked;
    ((Control) this.runServiceButtonNewOilFilterEngine).Enabled = false;
    if (this.activeChannel != null)
    {
      if (this.activeChannel.CommunicationsState == CommunicationsState.ReadEcuInfo)
      {
        this.checkmarkEngineStatus.CheckState = this.checkmarkDrivenDistanceEngine.CheckState = this.checkmarkSystemActiveEngine.CheckState = this.checkmarkOperatingTimeEngine.CheckState = CheckState.Unchecked;
        ((Control) this.runServiceButtonNewOilFilterEngine).Enabled = false;
        this.labelEngineStatus.Text = Resources.Message_ReadingECUInformation;
        this.customEngineMessage = false;
      }
      else
      {
        Checkmark systemActiveEngine = this.checkmarkSystemActiveEngine;
        uint? nullable1 = this.RawValue("Channel_activation_PAR_KanAktiv_MOT");
        int num1;
        if ((nullable1.GetValueOrDefault() != 1U ? 0 : (nullable1.HasValue ? 1 : 0)) != 0)
        {
          nullable1 = this.RawValue("System_activation_PAR_SysAktiv_FZG");
          if ((nullable1.GetValueOrDefault() != 1U ? 0 : (nullable1.HasValue ? 1 : 0)) != 0)
          {
            num1 = 1;
            goto label_7;
          }
        }
        num1 = 0;
label_7:
        systemActiveEngine.CheckState = (CheckState) num1;
        Checkmark drivenDistanceEngine = this.checkmarkDrivenDistanceEngine;
        nullable1 = this.RawValue(this.MassageQualifier("DT_STO_Engine_Service_Life_Dates_EEPROM_Driven_distance_LLD_Fs_MOT"));
        uint? nullable2 = this.RawValue(UserPanel.minimumDrivenDistanceQualifier);
        nullable2 = nullable2.HasValue ? new uint?(nullable2.GetValueOrDefault() * 1000U) : new uint?();
        int num2 = (nullable1.GetValueOrDefault() < nullable2.GetValueOrDefault() ? 0 : (nullable1.HasValue & nullable2.HasValue ? 1 : 0)) != 0 ? 1 : 0;
        drivenDistanceEngine.CheckState = (CheckState) num2;
        Checkmark operatingTimeEngine = this.checkmarkOperatingTimeEngine;
        nullable1 = this.RawValue(this.MassageQualifier("DT_STO_Engine_Service_Life_Dates_EEPROM_Operating_time_LLD_Bz_MOT"));
        nullable2 = this.RawValue(UserPanel.minimumOperatingTimeQualifier);
        nullable2 = nullable2.HasValue ? new uint?(nullable2.GetValueOrDefault() * 60U) : new uint?();
        int num3 = (nullable1.GetValueOrDefault() < nullable2.GetValueOrDefault() ? 0 : (nullable1.HasValue & nullable2.HasValue ? 1 : 0)) != 0 ? 1 : 0;
        operatingTimeEngine.CheckState = (CheckState) num3;
        string str = Resources.Message_ReadyToReset;
        CheckState checkState = CheckState.Checked;
        foreach (Checkmark enginePrecondition in this.enginePreconditions)
        {
          if ((this.checkmarkEngineStatus.CheckState = enginePrecondition.CheckState) == CheckState.Unchecked)
          {
            str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_CannotResetEngineSystem01, (object) Environment.NewLine, (object) (string) ((Control) enginePrecondition).Tag);
            checkState = CheckState.Unchecked;
            this.customEngineMessage = false;
            break;
          }
        }
        this.checkmarkEngineStatus.CheckState = checkState;
        this.labelEngineStatus.Text = !this.customEngineMessage ? str : this.labelEngineStatus.Text;
        ((Control) this.runServiceButtonNewOilFilterEngine).Enabled = this.checkmarkEngineStatus.CheckState == CheckState.Checked && this.activeChannel.Online && !this.resetInProgress;
      }
    }
    else
    {
      this.checkmarkEngineStatus.CheckState = this.checkmarkDrivenDistanceEngine.CheckState = this.checkmarkSystemActiveEngine.CheckState = this.checkmarkOperatingTimeEngine.CheckState = CheckState.Unchecked;
      ((Control) this.runServiceButtonNewOilFilterEngine).Enabled = false;
      this.labelEngineStatus.Text = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_CannotResetEngineSystem0MS01TOffline, (object) Environment.NewLine);
      this.customEngineMessage = false;
    }
  }

  private void UpdateTransmissionConditions()
  {
    this.checkmarkIgnitionSwitchTransmission.CheckState = this.digitalReadoutInstrumentIgnitionSwitch.RepresentedState == 1 ? CheckState.Checked : CheckState.Unchecked;
    this.checkmarkVehicleSpeedTransmission.CheckState = this.digitalReadoutInstrumentVehicleSpeed.RepresentedState == 1 ? CheckState.Checked : CheckState.Unchecked;
    ((Control) this.runServiceButtonNewOilFilterTransmission).Enabled = false;
    if (this.activeChannel != null)
    {
      if (this.activeChannel.CommunicationsState == CommunicationsState.ReadEcuInfo)
      {
        this.checkmarkTransmissionStatus.CheckState = this.checkmarkDrivenDistanceTransmission.CheckState = this.checkmarkSystemActiveTransmission.CheckState = this.checkmarkOperatingTimeTransmission.CheckState = CheckState.Unchecked;
        ((Control) this.runServiceButtonNewOilFilterTransmission).Enabled = false;
        this.labelTransmissionStatus.Text = Resources.Message_ReadingECUInformation;
        this.customTransmissionMessage = false;
      }
      else
      {
        Checkmark activeTransmission = this.checkmarkSystemActiveTransmission;
        uint? nullable1 = this.RawValue("Channel_activation_PAR_KanAktiv_GET");
        int num1;
        if ((nullable1.GetValueOrDefault() != 1U ? 0 : (nullable1.HasValue ? 1 : 0)) != 0)
        {
          nullable1 = this.RawValue("System_activation_PAR_SysAktiv_FZG");
          if ((nullable1.GetValueOrDefault() != 1U ? 0 : (nullable1.HasValue ? 1 : 0)) != 0)
          {
            num1 = 1;
            goto label_7;
          }
        }
        num1 = 0;
label_7:
        activeTransmission.CheckState = (CheckState) num1;
        Checkmark distanceTransmission = this.checkmarkDrivenDistanceTransmission;
        nullable1 = this.RawValue(this.MassageQualifier("DT_STO_Transmission_Service_Life_Dates_EEPROM_Driven_distance_LLD_Fs_GET"));
        uint? nullable2 = this.RawValue(UserPanel.minimumDrivenDistanceQualifier);
        nullable2 = nullable2.HasValue ? new uint?(nullable2.GetValueOrDefault() * 1000U) : new uint?();
        int num2 = (nullable1.GetValueOrDefault() < nullable2.GetValueOrDefault() ? 0 : (nullable1.HasValue & nullable2.HasValue ? 1 : 0)) != 0 ? 1 : 0;
        distanceTransmission.CheckState = (CheckState) num2;
        Checkmark timeTransmission = this.checkmarkOperatingTimeTransmission;
        nullable1 = this.RawValue(this.MassageQualifier("DT_STO_Transmission_Service_Life_Dates_EEPROM_Operating_time_LLD_Bz_GET"));
        nullable2 = this.RawValue(UserPanel.minimumOperatingTimeQualifier);
        nullable2 = nullable2.HasValue ? new uint?(nullable2.GetValueOrDefault() * 60U) : new uint?();
        int num3 = (nullable1.GetValueOrDefault() < nullable2.GetValueOrDefault() ? 0 : (nullable1.HasValue & nullable2.HasValue ? 1 : 0)) != 0 ? 1 : 0;
        timeTransmission.CheckState = (CheckState) num3;
        string str = Resources.Message_ReadyToReset;
        CheckState checkState = CheckState.Checked;
        foreach (Checkmark transmissionPrecondition in this.transmissionPreconditions)
        {
          if ((this.checkmarkTransmissionStatus.CheckState = transmissionPrecondition.CheckState) == CheckState.Unchecked)
          {
            str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_CannotResetTransmissionSystem01, (object) Environment.NewLine, (object) (string) ((Control) transmissionPrecondition).Tag);
            checkState = CheckState.Unchecked;
            this.customTransmissionMessage = false;
            break;
          }
        }
        this.checkmarkTransmissionStatus.CheckState = checkState;
        this.labelTransmissionStatus.Text = !this.customTransmissionMessage ? str : this.labelTransmissionStatus.Text;
        ((Control) this.runServiceButtonNewOilFilterTransmission).Enabled = this.checkmarkTransmissionStatus.CheckState == CheckState.Checked && this.activeChannel.Online && !this.resetInProgress;
      }
    }
    else
    {
      this.checkmarkTransmissionStatus.CheckState = this.checkmarkDrivenDistanceTransmission.CheckState = this.checkmarkSystemActiveTransmission.CheckState = this.checkmarkOperatingTimeTransmission.CheckState = CheckState.Unchecked;
      ((Control) this.runServiceButtonNewOilFilterTransmission).Enabled = false;
      this.labelTransmissionStatus.Text = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_CannotResetTransmissionSystem0MS01TOffline, (object) Environment.NewLine);
      this.customTransmissionMessage = false;
    }
  }

  private void UpdateAxle1Conditions()
  {
    this.checkmarkIgnitionSwitchAxle1.CheckState = this.digitalReadoutInstrumentIgnitionSwitch.RepresentedState == 1 ? CheckState.Checked : CheckState.Unchecked;
    this.checkmarkVehicleSpeedAxle1.CheckState = this.digitalReadoutInstrumentVehicleSpeed.RepresentedState == 1 ? CheckState.Checked : CheckState.Unchecked;
    ((Control) this.runServiceButtonNewOilFilterAxle1).Enabled = false;
    if (this.activeChannel != null)
    {
      if (this.activeChannel.CommunicationsState == CommunicationsState.ReadEcuInfo)
      {
        this.checkmarkAxle1Status.CheckState = this.checkmarkDrivenDistanceAxle1.CheckState = this.checkmarkSystemActiveAxle1.CheckState = this.checkmarkOperatingTimeAxle1.CheckState = CheckState.Unchecked;
        ((Control) this.runServiceButtonNewOilFilterAxle1).Enabled = false;
        this.labelAxle1Status.Text = Resources.Message_ReadingECUInformation;
        this.customAxle1Message = false;
      }
      else
      {
        Checkmark systemActiveAxle1 = this.checkmarkSystemActiveAxle1;
        uint? nullable1 = this.RawValue("Channel_activation_PAR_KanAktiv_HA1");
        int num1;
        if ((nullable1.GetValueOrDefault() != 1U ? 0 : (nullable1.HasValue ? 1 : 0)) != 0)
        {
          nullable1 = this.RawValue("System_activation_PAR_SysAktiv_FZG");
          if ((nullable1.GetValueOrDefault() != 1U ? 0 : (nullable1.HasValue ? 1 : 0)) != 0)
          {
            num1 = 1;
            goto label_7;
          }
        }
        num1 = 0;
label_7:
        systemActiveAxle1.CheckState = (CheckState) num1;
        Checkmark drivenDistanceAxle1 = this.checkmarkDrivenDistanceAxle1;
        nullable1 = this.RawValue(this.MassageQualifier("DT_STO_Rear_axle_1_Service_Life_Dates_EEPROM_Driven_distance_LLD_Fs_HA1"));
        uint? nullable2 = this.RawValue(UserPanel.minimumDrivenDistanceQualifier);
        nullable2 = nullable2.HasValue ? new uint?(nullable2.GetValueOrDefault() * 1000U) : new uint?();
        int num2 = (nullable1.GetValueOrDefault() < nullable2.GetValueOrDefault() ? 0 : (nullable1.HasValue & nullable2.HasValue ? 1 : 0)) != 0 ? 1 : 0;
        drivenDistanceAxle1.CheckState = (CheckState) num2;
        Checkmark operatingTimeAxle1 = this.checkmarkOperatingTimeAxle1;
        nullable1 = this.RawValue(this.MassageQualifier("DT_STO_Rear_axle_1_Service_Life_Dates_EEPROM_Operating_time_LLD_Bz_HA1"));
        nullable2 = this.RawValue(UserPanel.minimumOperatingTimeQualifier);
        nullable2 = nullable2.HasValue ? new uint?(nullable2.GetValueOrDefault() * 60U) : new uint?();
        int num3 = (nullable1.GetValueOrDefault() < nullable2.GetValueOrDefault() ? 0 : (nullable1.HasValue & nullable2.HasValue ? 1 : 0)) != 0 ? 1 : 0;
        operatingTimeAxle1.CheckState = (CheckState) num3;
        string str = Resources.Message_ReadyToReset;
        CheckState checkState = CheckState.Checked;
        foreach (Checkmark axle1Precondition in this.axle1Preconditions)
        {
          if ((this.checkmarkAxle1Status.CheckState = axle1Precondition.CheckState) == CheckState.Unchecked)
          {
            str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_CannotResetAxle1System01, (object) Environment.NewLine, (object) (string) ((Control) axle1Precondition).Tag);
            checkState = CheckState.Unchecked;
            this.customAxle1Message = false;
            break;
          }
        }
        this.checkmarkAxle1Status.CheckState = checkState;
        this.labelAxle1Status.Text = !this.customAxle1Message ? str : this.labelAxle1Status.Text;
        ((Control) this.runServiceButtonNewOilFilterAxle1).Enabled = this.checkmarkAxle1Status.CheckState == CheckState.Checked && this.activeChannel.Online && !this.resetInProgress;
      }
    }
    else
    {
      this.checkmarkAxle1Status.CheckState = this.checkmarkDrivenDistanceAxle1.CheckState = this.checkmarkSystemActiveAxle1.CheckState = this.checkmarkOperatingTimeAxle1.CheckState = CheckState.Unchecked;
      ((Control) this.runServiceButtonNewOilFilterAxle1).Enabled = false;
      this.labelAxle1Status.Text = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_CannotResetAxle1System0MS01TOffline, (object) Environment.NewLine);
      this.customAxle1Message = false;
    }
  }

  private void UpdateAxle2Conditions()
  {
    this.checkmarkIgnitionSwitchAxle2.CheckState = this.digitalReadoutInstrumentIgnitionSwitch.RepresentedState == 1 ? CheckState.Checked : CheckState.Unchecked;
    this.checkmarkVehicleSpeedAxle2.CheckState = this.digitalReadoutInstrumentVehicleSpeed.RepresentedState == 1 ? CheckState.Checked : CheckState.Unchecked;
    ((Control) this.runServiceButtonNewOilFilterAxle2).Enabled = false;
    if (this.activeChannel != null)
    {
      if (this.activeChannel.CommunicationsState == CommunicationsState.ReadEcuInfo)
      {
        this.checkmarkAxle2Status.CheckState = this.checkmarkDrivenDistanceAxle2.CheckState = this.checkmarkSystemActiveAxle2.CheckState = this.checkmarkOperatingTimeAxle2.CheckState = CheckState.Unchecked;
        ((Control) this.runServiceButtonNewOilFilterAxle2).Enabled = false;
        this.labelAxle2Status.Text = Resources.Message_ReadingECUInformation;
        this.customAxle2Message = false;
      }
      else
      {
        Checkmark systemActiveAxle2 = this.checkmarkSystemActiveAxle2;
        uint? nullable1 = this.RawValue("Channel_activation_PAR_KanAktiv_HA2");
        int num1;
        if ((nullable1.GetValueOrDefault() != 1U ? 0 : (nullable1.HasValue ? 1 : 0)) != 0)
        {
          nullable1 = this.RawValue("System_activation_PAR_SysAktiv_FZG");
          if ((nullable1.GetValueOrDefault() != 1U ? 0 : (nullable1.HasValue ? 1 : 0)) != 0)
          {
            num1 = 1;
            goto label_7;
          }
        }
        num1 = 0;
label_7:
        systemActiveAxle2.CheckState = (CheckState) num1;
        Checkmark drivenDistanceAxle2 = this.checkmarkDrivenDistanceAxle2;
        nullable1 = this.RawValue(this.MassageQualifier("DT_STO_Rear_axle_2_Service_Life_Dates_EEPROM_Driven_distance_LLD_Fs_HA2"));
        uint? nullable2 = this.RawValue(UserPanel.minimumDrivenDistanceQualifier);
        nullable2 = nullable2.HasValue ? new uint?(nullable2.GetValueOrDefault() * 1000U) : new uint?();
        int num2 = (nullable1.GetValueOrDefault() < nullable2.GetValueOrDefault() ? 0 : (nullable1.HasValue & nullable2.HasValue ? 1 : 0)) != 0 ? 1 : 0;
        drivenDistanceAxle2.CheckState = (CheckState) num2;
        Checkmark operatingTimeAxle2 = this.checkmarkOperatingTimeAxle2;
        nullable1 = this.RawValue(this.MassageQualifier("DT_STO_Rear_axle_2_Service_Life_Dates_EEPROM_Operating_time_LLD_Bz_HA2"));
        nullable2 = this.RawValue(UserPanel.minimumOperatingTimeQualifier);
        nullable2 = nullable2.HasValue ? new uint?(nullable2.GetValueOrDefault() * 60U) : new uint?();
        int num3 = (nullable1.GetValueOrDefault() < nullable2.GetValueOrDefault() ? 0 : (nullable1.HasValue & nullable2.HasValue ? 1 : 0)) != 0 ? 1 : 0;
        operatingTimeAxle2.CheckState = (CheckState) num3;
        string str = Resources.Message_ReadyToReset;
        CheckState checkState = CheckState.Checked;
        foreach (Checkmark axle2Precondition in this.axle2Preconditions)
        {
          if ((this.checkmarkAxle2Status.CheckState = axle2Precondition.CheckState) == CheckState.Unchecked)
          {
            str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_CannotResetAxle2System01, (object) Environment.NewLine, (object) (string) ((Control) axle2Precondition).Tag);
            checkState = CheckState.Unchecked;
            this.customAxle2Message = false;
            break;
          }
        }
        this.checkmarkAxle2Status.CheckState = checkState;
        this.labelAxle2Status.Text = !this.customAxle2Message ? str : this.labelAxle2Status.Text;
        ((Control) this.runServiceButtonNewOilFilterAxle2).Enabled = this.checkmarkAxle2Status.CheckState == CheckState.Checked && this.activeChannel.Online && !this.resetInProgress;
      }
    }
    else
    {
      this.checkmarkAxle2Status.CheckState = this.checkmarkDrivenDistanceAxle2.CheckState = this.checkmarkSystemActiveAxle2.CheckState = this.checkmarkOperatingTimeAxle2.CheckState = CheckState.Unchecked;
      ((Control) this.runServiceButtonNewOilFilterAxle2).Enabled = false;
      this.labelAxle2Status.Text = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_CannotResetAxle2System0MS01TOffline, (object) Environment.NewLine);
      this.customAxle2Message = false;
    }
  }

  private string PrintValue(
    string estimatedReplacementDate,
    string qualifier,
    string units,
    int scalingFactor)
  {
    if (this.activeChannel != null)
    {
      if (estimatedReplacementDate == Resources.Message_NotEnoughOperatingTime && qualifier.Contains("Prediction"))
        return Resources.Message_NotEnoughOperatingTime;
      EcuInfo ecuInfo;
      if ((ecuInfo = this.activeChannel.EcuInfos[qualifier]) != null && !string.IsNullOrEmpty(ecuInfo.Value))
        return string.IsNullOrEmpty(units) ? ecuInfo.Value.ToString() : UserPanel.ConvertAndScaleValue(ecuInfo.Value.ToString(), units, scalingFactor);
      Parameter parameter;
      if ((parameter = this.activeChannel.Parameters[qualifier]) != null && parameter.Value != null && !string.IsNullOrEmpty(parameter.Value.ToString()))
        return string.IsNullOrEmpty(units) ? parameter.Value.ToString() : UserPanel.ConvertAndScaleValue(parameter.Value.ToString(), units, scalingFactor);
    }
    return Resources.Message_Unavaiable;
  }

  private static string ConvertAndScaleValue(string value, string units, int scalingFactor)
  {
    double result = 0.0;
    if (!double.TryParse(value, out result))
      return Resources.Message_Unavaiable;
    string str = units;
    double num = result / (double) scalingFactor;
    if (units == "km")
    {
      Conversion conversion = Converter.GlobalInstance.GetConversion("km");
      str = conversion.OutputUnit;
      num = conversion.Convert(num);
    }
    return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0:#.00} {1} ", (object) num, (object) str);
  }

  private uint? RawValue(string qualifier)
  {
    if (this.activeChannel != null)
    {
      EcuInfo ecuInfo;
      if ((ecuInfo = this.activeChannel.EcuInfos[qualifier]) != null && !string.IsNullOrEmpty(ecuInfo.Value))
      {
        uint result;
        return uint.TryParse(ecuInfo.Value.ToString(), out result) ? new uint?(result) : new uint?();
      }
      Parameter parameter;
      if ((parameter = this.activeChannel.Parameters[qualifier]) != null && parameter.Value != null && !string.IsNullOrEmpty(parameter.Value.ToString()))
      {
        Choice choice = parameter.Value as Choice;
        uint result1;
        uint result2;
        return choice != (object) null ? (uint.TryParse(choice.RawValue.ToString(), out result1) ? new uint?(result1) : new uint?()) : (uint.TryParse(parameter.Value.ToString(), out result2) ? new uint?(result2) : new uint?());
      }
    }
    return new uint?();
  }

  public virtual void OnChannelsChanged()
  {
    this.SetChannelMS01T(this.GetChannel("MS01T", (CustomPanel.ChannelLookupOptions) 3));
    this.SetChannelCGW05T(this.GetChannel("CGW05T", (CustomPanel.ChannelLookupOptions) 3));
    this.UpdateUI();
  }

  private void SetChannelMS01T(Channel channel)
  {
    if ((channel == null || this.ms01t != null) && (channel != null || this.ms01t == null))
      return;
    if (this.ms01t != null)
    {
      this.ms01t.Parameters.ParametersWriteCompleteEvent -= new ParametersWriteCompleteEventHandler(this.ParametersWriteCompleteEvent);
      this.ms01t.Parameters.ParametersReadCompleteEvent -= new ParametersReadCompleteEventHandler(this.ParametersReadCompleteEvent);
      this.ms01t.EcuInfos.EcuInfosReadCompleteEvent -= new EcuInfosReadCompleteEventHandler(this.EcuInfosReadComplete);
    }
    this.ms01t = channel;
    this.activeChannel = channel;
    if (this.ms01t != null)
    {
      ((SingleInstrumentBase) this.digitalReadoutInstrumentMinimumDrivenDistance).Instrument = new Qualifier((QualifierTypes) 4, "MS01T", UserPanel.minimumDrivenDistanceQualifier);
      this.runServiceButtonNewOilFilterEngine.ServiceCall = this.CreateServiceCall("MSF01T", 1U);
      this.runServiceButtonNewOilFilterTransmission.ServiceCall = this.CreateServiceCall("MSF01T", 2U);
      this.runServiceButtonNewOilFilterAxle1.ServiceCall = this.CreateServiceCall("MSF01T", 6U);
      this.runServiceButtonNewOilFilterAxle2.ServiceCall = this.CreateServiceCall("MSF01T", 20U);
      this.ms01t.Parameters.ParametersWriteCompleteEvent += new ParametersWriteCompleteEventHandler(this.ParametersWriteCompleteEvent);
      this.ms01t.Parameters.ParametersReadCompleteEvent += new ParametersReadCompleteEventHandler(this.ParametersReadCompleteEvent);
      this.ms01t.EcuInfos.EcuInfosReadCompleteEvent += new EcuInfosReadCompleteEventHandler(this.EcuInfosReadComplete);
      this.ReadParameters();
    }
    this.resetInProgress = this.customEngineMessage = this.customTransmissionMessage = this.customAxle1Message = this.customAxle2Message = false;
    this.UpdateWebBrowser();
  }

  private void SetChannelCGW05T(Channel channel)
  {
    if ((channel == null || this.cgw05t != null) && (channel != null || this.cgw05t == null))
      return;
    if (this.cgw05t != null)
    {
      this.cgw05t.Parameters.ParametersWriteCompleteEvent -= new ParametersWriteCompleteEventHandler(this.ParametersWriteCompleteEvent);
      this.cgw05t.Parameters.ParametersReadCompleteEvent -= new ParametersReadCompleteEventHandler(this.ParametersReadCompleteEvent);
      this.cgw05t.EcuInfos.EcuInfosReadCompleteEvent -= new EcuInfosReadCompleteEventHandler(this.EcuInfosReadComplete);
    }
    this.cgw05t = channel;
    this.activeChannel = channel;
    if (this.cgw05t != null)
    {
      ((SingleInstrumentBase) this.digitalReadoutInstrumentMinimumDrivenDistance).Instrument = new Qualifier((QualifierTypes) 4, "CGW05T", UserPanel.minimumDrivenDistanceQualifier);
      this.runServiceButtonNewOilFilterEngine.ServiceCall = this.CreateServiceCall("CGW05T", 1U);
      this.runServiceButtonNewOilFilterTransmission.ServiceCall = this.CreateServiceCall("CGW05T", 2U);
      this.runServiceButtonNewOilFilterAxle1.ServiceCall = this.CreateServiceCall("CGW05T", 6U);
      this.runServiceButtonNewOilFilterAxle2.ServiceCall = this.CreateServiceCall("CGW05T", 20U);
      this.cgw05t.Parameters.ParametersWriteCompleteEvent += new ParametersWriteCompleteEventHandler(this.ParametersWriteCompleteEvent);
      this.cgw05t.Parameters.ParametersReadCompleteEvent += new ParametersReadCompleteEventHandler(this.ParametersReadCompleteEvent);
      this.cgw05t.EcuInfos.EcuInfosReadCompleteEvent += new EcuInfosReadCompleteEventHandler(this.EcuInfosReadComplete);
      this.ReadParameters();
    }
    this.resetInProgress = this.customEngineMessage = this.customTransmissionMessage = this.customAxle1Message = this.customAxle2Message = false;
    this.UpdateWebBrowser();
  }

  private ServiceCall CreateServiceCall(string ecuName, uint channelNumber)
  {
    return new ServiceCall(ecuName, this.MassageQualifier(UserPanel.resetServiceQualifier), (IEnumerable<string>) new string[2]
    {
      $"Channel_number={channelNumber}",
      "Filter_condition_Diesel_particle_filter_only=0"
    });
  }

  private string MassageQualifier(string qualifier)
  {
    return this.activeChannel == this.cgw05t && !qualifier.Contains("_MS_") ? qualifier.Replace("DT_STO_", "DT_STO_MS_").Replace("DL_", "DL_MS_") : qualifier;
  }

  private void RefreshEcuInfo(string systemName, string systemAcronym)
  {
    if (this.activeChannel == null || this.activeChannel.CommunicationsState != CommunicationsState.Online)
      return;
    foreach (Tuple<string, string, string, string, int, bool> tuple in this.oilSystemValues.Where<Tuple<string, string, string, string, int, bool>>((Func<Tuple<string, string, string, string, int, bool>, bool>) (v => (v.Item2 == systemName || v.Item2 == "All" || v.Item2 == "Date") && v.Item3.Contains("EEPROM"))))
      this.activeChannel.EcuInfos[this.MassageQualifier(string.Format((IFormatProvider) CultureInfo.InvariantCulture, tuple.Item2, (object) systemName, (object) systemAcronym))].Read(false);
  }

  private void ReadParameters()
  {
    if (this.activeChannel == null || this.activeChannel.CommunicationsState != CommunicationsState.Online)
      return;
    this.ReadParameter(UserPanel.minimumDrivenDistanceQualifier);
    this.ReadParameter(UserPanel.minimumOperatingTimeQualifier);
    foreach (Tuple<string, string> oilSystem in this.oilSystems)
    {
      foreach (Tuple<string, string, string, string, int, bool> tuple in this.oilSystemValues.Where<Tuple<string, string, string, string, int, bool>>((Func<Tuple<string, string, string, string, int, bool>, bool>) (ev => ev.Item2.Contains("_PAR_"))))
        this.ReadParameter(this.MassageQualifier(string.Format((IFormatProvider) CultureInfo.InvariantCulture, tuple.Item2, (object) oilSystem.Item1, (object) oilSystem.Item2)));
    }
  }

  private void ReadParameter(string qualifier)
  {
    if (this.activeChannel == null || this.activeChannel.CommunicationsState != CommunicationsState.Online || this.activeChannel.Parameters[qualifier] == null || this.activeChannel.Parameters[qualifier].HasBeenReadFromEcu)
      return;
    this.activeChannel.Parameters.ReadGroup(this.activeChannel.Parameters[qualifier].GroupQualifier, true, false);
  }

  private void EcuInfosReadComplete(object sender, ResultEventArgs result)
  {
    this.UpdateWebBrowser();
    this.UpdateUI();
    this.ReadParameters();
    this.resetInProgress = this.customEngineMessage = this.customTransmissionMessage = this.customAxle1Message = this.customAxle2Message = false;
  }

  private void ParametersReadCompleteEvent(object sender, ResultEventArgs result)
  {
    this.UpdateWebBrowser();
    this.UpdateUI();
    this.ReadParameters();
    this.resetInProgress = this.customEngineMessage = this.customTransmissionMessage = this.customAxle1Message = this.customAxle2Message = false;
  }

  private void ParametersWriteCompleteEvent(object sender, ResultEventArgs result)
  {
    this.UpdateWebBrowser();
    this.UpdateUI();
    this.resetInProgress = this.customEngineMessage = this.customTransmissionMessage = this.customAxle1Message = this.customAxle2Message = false;
  }

  private void webBrowserEngineValues_DocumentCompleted(
    object sender,
    WebBrowserDocumentCompletedEventArgs e)
  {
    if (this.webBrowsersystemInputs.Document.All.Count > 0)
    {
      HtmlElementCollection elementsByName = this.webBrowsersystemInputs.Document.All.GetElementsByName("systemInputPane");
      if (elementsByName != null && elementsByName.Count > 0)
      {
        this.systemInputPane = elementsByName[0];
        this.UpdateWebBrowser();
      }
    }
    base.OnChannelsChanged();
  }

  private void UpdateWebBrowser()
  {
    if (!(this.systemInputPane != (HtmlElement) null))
      return;
    StringBuilder stringBuilder = new StringBuilder();
    using (XmlWriter writer = PrintHelper.CreateWriter(stringBuilder))
      this.UpdateContent(writer);
    this.systemInputPane.InnerHtml = stringBuilder.ToString();
    this.webBrowsersystemInputs.Size = this.webBrowsersystemInputs.Document.Body.ScrollRectangle.Size;
  }

  private void UpdateContent(XmlWriter writer)
  {
    if (this.activeChannel != null)
    {
      writer.WriteStartElement("table");
      foreach (Tuple<string, string> oilSystem1 in this.oilSystems)
      {
        Tuple<string, string> oilSystem = oilSystem1;
        writer.WriteStartElement("tr");
        writer.WriteStartElement("th");
        writer.WriteStartAttribute("colspan");
        writer.WriteString("2");
        writer.WriteEndAttribute();
        writer.WriteString(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_0OilSystemValues, (object) oilSystem.Item1.Replace("_", " ")));
        writer.WriteEndElement();
        writer.WriteEndElement();
        UserPanel.WriteRow(writer, Resources.Message_Description, Resources.Message_Value);
        string estimatedReplacementDate = this.EstimatedReplacementDate(oilSystem.Item1, oilSystem.Item2);
        UserPanel.WriteRow(writer, Resources.Message_EstimatedReplacementDate, estimatedReplacementDate);
        foreach (Tuple<string, string, string, string, int, bool> tuple in this.oilSystemValues.Where<Tuple<string, string, string, string, int, bool>>((Func<Tuple<string, string, string, string, int, bool>, bool>) (ev => ev.Item1 == oilSystem.Item1 || ev.Item1 == "All")))
        {
          if (tuple.Item6 || ApplicationInformation.Branding.IsProductName("Engineering"))
            UserPanel.WriteRow(writer, tuple.Item3, this.PrintValue(estimatedReplacementDate, this.MassageQualifier(string.Format((IFormatProvider) CultureInfo.InvariantCulture, tuple.Item2, (object) oilSystem.Item1, (object) oilSystem.Item2)), tuple.Item4, tuple.Item5));
        }
      }
      writer.WriteEndElement();
    }
    else
      writer.WriteString(Resources.Message_MS01TOffline);
  }

  private static void WriteRow(XmlWriter writer, params string[] columns)
  {
    writer.WriteStartElement("tr");
    foreach (string column in columns)
    {
      writer.WriteStartElement("td");
      writer.WriteString(column);
      writer.WriteEndElement();
    }
    writer.WriteEndElement();
  }

  private string EstimatedReplacementDate(string system, string systemAcronym)
  {
    string empty = string.Empty;
    int num = 0;
    foreach (Tuple<string, string, string, string, int, bool> tuple in this.oilSystemValues.Where<Tuple<string, string, string, string, int, bool>>((Func<Tuple<string, string, string, string, int, bool>, bool>) (ev => ev.Item1 == "Date")))
    {
      string qualifier = this.MassageQualifier(string.Format((IFormatProvider) CultureInfo.InvariantCulture, tuple.Item2, (object) system, (object) systemAcronym));
      EcuInfo ecuInfo = (EcuInfo) null;
      if (this.activeChannel != null && (ecuInfo = this.activeChannel.EcuInfos[qualifier]) != null && ecuInfo.Value != null)
      {
        empty += string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}-", (object) ecuInfo.Value);
        ++num;
      }
    }
    string str1;
    if (num != 3)
      str1 = Resources.Message_Unavaiable;
    else
      str1 = empty.TrimEnd('-');
    string str2 = str1;
    return str2 == "2255-255-255" ? Resources.Message_NotEnoughOperatingTime : str2;
  }

  private void digitalReadoutInstrument_RepresentedStateChanged(object sender, EventArgs e)
  {
    this.UpdateUI();
  }

  public virtual string StyleSheet
  {
    get
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("<style type=\"text/css\">td {padding-right: 10px}");
      stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "h1 {{ font-family:{0}; font-size:{1}pt }}", (object) ((Control) this).Font.FontFamily.Name, (object) (((Control) this).Font.SizeInPoints * 2f).ToString((IFormatProvider) CultureInfo.InvariantCulture));
      stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "p {{ font-family:{0}; font-size:{1}pt }}", (object) ((Control) this).Font.FontFamily.Name, (object) ((Control) this).Font.SizeInPoints.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "table {{ font-family:{0}; font-size:{1}pt; border: 1px solid black; border-collapse: collapse; width:100%}}", (object) ((Control) this).Font.FontFamily.Name, (object) ((Control) this).Font.SizeInPoints.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      stringBuilder.AppendLine("th { border-width: 1px; padding: 4px; border: 1px solid black; background: #f4f4f4 }");
      stringBuilder.AppendLine("td { border-width: 1px; padding: 4px; border: 1px solid black; }");
      stringBuilder.AppendLine("tr:nth-child(odd) { background: #f1fdf1 }");
      stringBuilder.AppendLine("tr:nth-child(even) { background: #f1f1fd }");
      stringBuilder.AppendLine("tr.disabled { color: #aaaaaa }");
      stringBuilder.Append("</style>");
      return stringBuilder.ToString();
    }
  }

  private void runServiceButtonReplaceOilFilterEngine_Started(
    object sender,
    PassFailResultEventArgs e)
  {
    if (e.Result == 1)
    {
      this.labelEngineStatus.Text = Resources.Message_ResetComplete;
      this.RefreshEcuInfo(this.oilSystems[0].Item1, this.oilSystems[0].Item2);
    }
    else
      this.labelEngineStatus.Text = ((ResultEventArgs) e).Exception.Message;
    this.customEngineMessage = true;
    this.resetInProgress = false;
    this.UpdateWebBrowser();
    this.UpdateUI();
  }

  private void runServiceButtonReplaceOilFilterEngine_Starting(object sender, CancelEventArgs e)
  {
    this.resetInProgress = true;
    this.customEngineMessage = true;
    this.labelEngineStatus.Text = Resources.Message_Starting;
    this.UpdateUI();
  }

  private void runServiceButtonReplaceOilFilterTransmission_Started(
    object sender,
    PassFailResultEventArgs e)
  {
    if (e.Result == 1)
    {
      this.labelTransmissionStatus.Text = Resources.Message_ResetComplete;
      this.RefreshEcuInfo(this.oilSystems[1].Item1, this.oilSystems[1].Item2);
    }
    else
      this.labelTransmissionStatus.Text = ((ResultEventArgs) e).Exception.Message;
    this.customTransmissionMessage = true;
    this.resetInProgress = false;
    this.UpdateWebBrowser();
    this.UpdateUI();
  }

  private void runServiceButtonReplaceOilFilterTransmission_Starting(
    object sender,
    CancelEventArgs e)
  {
    this.resetInProgress = true;
    this.customTransmissionMessage = true;
    this.labelTransmissionStatus.Text = Resources.Message_Starting;
    this.UpdateUI();
  }

  private void runServiceButtonReplaceOilFilterAxle1_Started(
    object sender,
    PassFailResultEventArgs e)
  {
    if (e.Result == 1)
    {
      this.labelAxle1Status.Text = Resources.Message_ResetComplete;
      this.RefreshEcuInfo(this.oilSystems[2].Item1, this.oilSystems[2].Item2);
    }
    else
      this.labelAxle1Status.Text = ((ResultEventArgs) e).Exception.Message;
    this.customAxle1Message = true;
    this.resetInProgress = false;
    this.UpdateWebBrowser();
    this.UpdateUI();
  }

  private void runServiceButtonReplaceOilFilterAxle1_Starting(object sender, CancelEventArgs e)
  {
    this.resetInProgress = true;
    this.customAxle1Message = true;
    this.labelAxle1Status.Text = Resources.Message_Starting;
    this.UpdateUI();
  }

  private void runServiceButtonReplaceOilFilterAxle2_Started(
    object sender,
    PassFailResultEventArgs e)
  {
    if (e.Result == 1)
    {
      this.labelAxle2Status.Text = Resources.Message_ResetComplete;
      this.RefreshEcuInfo(this.oilSystems[3].Item1, this.oilSystems[3].Item2);
    }
    else
      this.labelAxle2Status.Text = ((ResultEventArgs) e).Exception.Message;
    this.customAxle2Message = true;
    this.resetInProgress = false;
    this.UpdateWebBrowser();
    this.UpdateUI();
  }

  private void runServiceButtonReplaceOilFilterAxle2_Starting(object sender, CancelEventArgs e)
  {
    this.resetInProgress = true;
    this.customAxle2Message = true;
    this.labelAxle2Status.Text = Resources.Message_Starting;
    this.UpdateUI();
  }

  private void UserPanel_SizeChanged(object sender, EventArgs e)
  {
    if (this.webBrowsersystemInputs == null || !(this.webBrowsersystemInputs.Document != (HtmlDocument) null) || !(this.webBrowsersystemInputs.Document.Body != (HtmlElement) null))
      return;
    this.webBrowsersystemInputs.Size = this.webBrowsersystemInputs.Document.Body.ScrollRectangle.Size;
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanelMain = new TableLayoutPanel();
    this.panelTransmission = new Panel();
    this.tableLayoutPanelTransmission = new TableLayoutPanel();
    this.tableLayoutPanelTransmissionConditions = new TableLayoutPanel();
    this.tableLayoutPanel10 = new TableLayoutPanel();
    this.label4 = new System.Windows.Forms.Label();
    this.checkmarkOperatingTimeTransmission = new Checkmark();
    this.tableLayoutPanel11 = new TableLayoutPanel();
    this.checkmarkSystemActiveTransmission = new Checkmark();
    this.label5 = new System.Windows.Forms.Label();
    this.tableLayoutPanel12 = new TableLayoutPanel();
    this.label6 = new System.Windows.Forms.Label();
    this.checkmarkIgnitionSwitchTransmission = new Checkmark();
    this.tableLayoutPanel13 = new TableLayoutPanel();
    this.checkmarkVehicleSpeedTransmission = new Checkmark();
    this.label7 = new System.Windows.Forms.Label();
    this.tableLayoutPanel14 = new TableLayoutPanel();
    this.label8 = new System.Windows.Forms.Label();
    this.checkmarkDrivenDistanceTransmission = new Checkmark();
    this.tableLayoutPanelTransmissionStatus = new TableLayoutPanel();
    this.labelTransmissionStatus = new System.Windows.Forms.Label();
    this.checkmarkTransmissionStatus = new Checkmark();
    this.runServiceButtonNewOilFilterTransmission = new RunServiceButton();
    this.label10 = new System.Windows.Forms.Label();
    this.webBrowsersystemInputs = new WebBrowser();
    this.tableLayoutPanelHeader = new TableLayoutPanel();
    this.digitalReadoutInstrumentVehicleSpeed = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentIgnitionSwitch = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentMinimumDrivenDistance = new DigitalReadoutInstrument();
    this.tableLayoutPanel5 = new TableLayoutPanel();
    this.label20 = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.label22 = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.scalingLabelMinimumTime = new ScalingLabel();
    this.panelEngine = new Panel();
    this.tableLayoutPanelEngine = new TableLayoutPanel();
    this.tableLayoutPanelEngineStatus = new TableLayoutPanel();
    this.tableLayoutPanel4 = new TableLayoutPanel();
    this.labelOperatingTimeEngine = new System.Windows.Forms.Label();
    this.checkmarkOperatingTimeEngine = new Checkmark();
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.checkmarkSystemActiveEngine = new Checkmark();
    this.labelSystemActiveEngine = new System.Windows.Forms.Label();
    this.tableLayoutPanel2 = new TableLayoutPanel();
    this.labelIgnitionSwitchEngine = new System.Windows.Forms.Label();
    this.checkmarkIgnitionSwitchEngine = new Checkmark();
    this.tableLayoutPanel6 = new TableLayoutPanel();
    this.checkmarkVehicleSpeedEngine = new Checkmark();
    this.label2 = new System.Windows.Forms.Label();
    this.tableLayoutPanel3 = new TableLayoutPanel();
    this.labelDrivenDistanceEngine = new System.Windows.Forms.Label();
    this.checkmarkDrivenDistanceEngine = new Checkmark();
    this.tableLayoutPanelEngineStatusMessage = new TableLayoutPanel();
    this.labelEngineStatus = new System.Windows.Forms.Label();
    this.checkmarkEngineStatus = new Checkmark();
    this.runServiceButtonNewOilFilterEngine = new RunServiceButton();
    this.label3 = new System.Windows.Forms.Label();
    this.panelAxle1 = new Panel();
    this.tableLayoutPanelAxle1 = new TableLayoutPanel();
    this.tableLayoutPanelAxle1Conditions = new TableLayoutPanel();
    this.tableLayoutPanel19 = new TableLayoutPanel();
    this.label1 = new System.Windows.Forms.Label();
    this.checkmarkOperatingTimeAxle1 = new Checkmark();
    this.tableLayoutPanel20 = new TableLayoutPanel();
    this.checkmarkSystemActiveAxle1 = new Checkmark();
    this.label9 = new System.Windows.Forms.Label();
    this.tableLayoutPanel21 = new TableLayoutPanel();
    this.label11 = new System.Windows.Forms.Label();
    this.checkmarkIgnitionSwitchAxle1 = new Checkmark();
    this.tableLayoutPanel22 = new TableLayoutPanel();
    this.checkmarkVehicleSpeedAxle1 = new Checkmark();
    this.label12 = new System.Windows.Forms.Label();
    this.tableLayoutPanel23 = new TableLayoutPanel();
    this.label13 = new System.Windows.Forms.Label();
    this.checkmarkDrivenDistanceAxle1 = new Checkmark();
    this.tableLayoutPanelAxle1Status = new TableLayoutPanel();
    this.labelAxle1Status = new System.Windows.Forms.Label();
    this.checkmarkAxle1Status = new Checkmark();
    this.runServiceButtonNewOilFilterAxle1 = new RunServiceButton();
    this.label15 = new System.Windows.Forms.Label();
    this.panelAxle2 = new Panel();
    this.tableLayoutPanelAxle2 = new TableLayoutPanel();
    this.Axle2Conditions = new TableLayoutPanel();
    this.tableLayoutPanel28 = new TableLayoutPanel();
    this.label14 = new System.Windows.Forms.Label();
    this.checkmarkOperatingTimeAxle2 = new Checkmark();
    this.tableLayoutPanel29 = new TableLayoutPanel();
    this.checkmarkSystemActiveAxle2 = new Checkmark();
    this.label16 = new System.Windows.Forms.Label();
    this.tableLayoutPanel30 = new TableLayoutPanel();
    this.label17 = new System.Windows.Forms.Label();
    this.checkmarkIgnitionSwitchAxle2 = new Checkmark();
    this.tableLayoutPanel31 = new TableLayoutPanel();
    this.checkmarkVehicleSpeedAxle2 = new Checkmark();
    this.label18 = new System.Windows.Forms.Label();
    this.tableLayoutPanel32 = new TableLayoutPanel();
    this.label19 = new System.Windows.Forms.Label();
    this.checkmarkDrivenDistanceAxle2 = new Checkmark();
    this.tableLayoutPanelAxle2Status = new TableLayoutPanel();
    this.labelAxle2Status = new System.Windows.Forms.Label();
    this.checkmarkAxle2Status = new Checkmark();
    this.runServiceButtonNewOilFilterAxle2 = new RunServiceButton();
    this.label21 = new System.Windows.Forms.Label();
    ((Control) this.tableLayoutPanelMain).SuspendLayout();
    this.panelTransmission.SuspendLayout();
    ((Control) this.tableLayoutPanelTransmission).SuspendLayout();
    ((Control) this.tableLayoutPanelTransmissionConditions).SuspendLayout();
    ((Control) this.tableLayoutPanel10).SuspendLayout();
    ((Control) this.tableLayoutPanel11).SuspendLayout();
    ((Control) this.tableLayoutPanel12).SuspendLayout();
    ((Control) this.tableLayoutPanel13).SuspendLayout();
    ((Control) this.tableLayoutPanel14).SuspendLayout();
    ((Control) this.tableLayoutPanelTransmissionStatus).SuspendLayout();
    ((Control) this.tableLayoutPanelHeader).SuspendLayout();
    ((Control) this.tableLayoutPanel5).SuspendLayout();
    this.panelEngine.SuspendLayout();
    ((Control) this.tableLayoutPanelEngine).SuspendLayout();
    ((Control) this.tableLayoutPanelEngineStatus).SuspendLayout();
    ((Control) this.tableLayoutPanel4).SuspendLayout();
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this.tableLayoutPanel2).SuspendLayout();
    ((Control) this.tableLayoutPanel6).SuspendLayout();
    ((Control) this.tableLayoutPanel3).SuspendLayout();
    ((Control) this.tableLayoutPanelEngineStatusMessage).SuspendLayout();
    this.panelAxle1.SuspendLayout();
    ((Control) this.tableLayoutPanelAxle1).SuspendLayout();
    ((Control) this.tableLayoutPanelAxle1Conditions).SuspendLayout();
    ((Control) this.tableLayoutPanel19).SuspendLayout();
    ((Control) this.tableLayoutPanel20).SuspendLayout();
    ((Control) this.tableLayoutPanel21).SuspendLayout();
    ((Control) this.tableLayoutPanel22).SuspendLayout();
    ((Control) this.tableLayoutPanel23).SuspendLayout();
    ((Control) this.tableLayoutPanelAxle1Status).SuspendLayout();
    this.panelAxle2.SuspendLayout();
    ((Control) this.tableLayoutPanelAxle2).SuspendLayout();
    ((Control) this.Axle2Conditions).SuspendLayout();
    ((Control) this.tableLayoutPanel28).SuspendLayout();
    ((Control) this.tableLayoutPanel29).SuspendLayout();
    ((Control) this.tableLayoutPanel30).SuspendLayout();
    ((Control) this.tableLayoutPanel31).SuspendLayout();
    ((Control) this.tableLayoutPanel32).SuspendLayout();
    ((Control) this.tableLayoutPanelAxle2Status).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelMain, "tableLayoutPanelMain");
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.panelTransmission, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.webBrowsersystemInputs, 1, 1);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.tableLayoutPanelHeader, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.panelEngine, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.panelAxle1, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.panelAxle2, 0, 4);
    ((Control) this.tableLayoutPanelMain).Name = "tableLayoutPanelMain";
    componentResourceManager.ApplyResources((object) this.panelTransmission, "panelTransmission");
    this.panelTransmission.BorderStyle = BorderStyle.FixedSingle;
    this.panelTransmission.Controls.Add((Control) this.tableLayoutPanelTransmission);
    this.panelTransmission.Name = "panelTransmission";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelTransmission, "tableLayoutPanelTransmission");
    ((TableLayoutPanel) this.tableLayoutPanelTransmission).Controls.Add((Control) this.tableLayoutPanelTransmissionConditions, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanelTransmission).Controls.Add((Control) this.tableLayoutPanelTransmissionStatus, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanelTransmission).Controls.Add((Control) this.label10, 0, 0);
    ((Control) this.tableLayoutPanelTransmission).Name = "tableLayoutPanelTransmission";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelTransmissionConditions, "tableLayoutPanelTransmissionConditions");
    ((TableLayoutPanel) this.tableLayoutPanelTransmissionConditions).Controls.Add((Control) this.tableLayoutPanel10, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanelTransmissionConditions).Controls.Add((Control) this.tableLayoutPanel11, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanelTransmissionConditions).Controls.Add((Control) this.tableLayoutPanel12, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanelTransmissionConditions).Controls.Add((Control) this.tableLayoutPanel13, 2, 1);
    ((TableLayoutPanel) this.tableLayoutPanelTransmissionConditions).Controls.Add((Control) this.tableLayoutPanel14, 0, 0);
    ((Control) this.tableLayoutPanelTransmissionConditions).Name = "tableLayoutPanelTransmissionConditions";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel10, "tableLayoutPanel10");
    ((TableLayoutPanel) this.tableLayoutPanel10).Controls.Add((Control) this.label4, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel10).Controls.Add((Control) this.checkmarkOperatingTimeTransmission, 0, 0);
    ((Control) this.tableLayoutPanel10).Name = "tableLayoutPanel10";
    componentResourceManager.ApplyResources((object) this.label4, "label4");
    this.label4.Name = "label4";
    this.label4.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.checkmarkOperatingTimeTransmission, "checkmarkOperatingTimeTransmission");
    ((Control) this.checkmarkOperatingTimeTransmission).Name = "checkmarkOperatingTimeTransmission";
    ((Control) this.checkmarkOperatingTimeTransmission).Tag = (object) "Operating Time too low.";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel11, "tableLayoutPanel11");
    ((TableLayoutPanel) this.tableLayoutPanel11).Controls.Add((Control) this.checkmarkSystemActiveTransmission, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel11).Controls.Add((Control) this.label5, 1, 0);
    ((Control) this.tableLayoutPanel11).Name = "tableLayoutPanel11";
    componentResourceManager.ApplyResources((object) this.checkmarkSystemActiveTransmission, "checkmarkSystemActiveTransmission");
    ((Control) this.checkmarkSystemActiveTransmission).Name = "checkmarkSystemActiveTransmission";
    ((Control) this.checkmarkSystemActiveTransmission).Tag = (object) "Oil System Inactive.";
    componentResourceManager.ApplyResources((object) this.label5, "label5");
    this.label5.Name = "label5";
    this.label5.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel12, "tableLayoutPanel12");
    ((TableLayoutPanel) this.tableLayoutPanel12).Controls.Add((Control) this.label6, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel12).Controls.Add((Control) this.checkmarkIgnitionSwitchTransmission, 0, 0);
    ((Control) this.tableLayoutPanel12).Name = "tableLayoutPanel12";
    componentResourceManager.ApplyResources((object) this.label6, "label6");
    this.label6.Name = "label6";
    this.label6.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.checkmarkIgnitionSwitchTransmission, "checkmarkIgnitionSwitchTransmission");
    ((Control) this.checkmarkIgnitionSwitchTransmission).Name = "checkmarkIgnitionSwitchTransmission";
    ((Control) this.checkmarkIgnitionSwitchTransmission).Tag = (object) "Ignition Switch Off.";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel13, "tableLayoutPanel13");
    ((TableLayoutPanel) this.tableLayoutPanel13).Controls.Add((Control) this.checkmarkVehicleSpeedTransmission, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel13).Controls.Add((Control) this.label7, 1, 0);
    ((Control) this.tableLayoutPanel13).Name = "tableLayoutPanel13";
    componentResourceManager.ApplyResources((object) this.checkmarkVehicleSpeedTransmission, "checkmarkVehicleSpeedTransmission");
    ((Control) this.checkmarkVehicleSpeedTransmission).Name = "checkmarkVehicleSpeedTransmission";
    ((Control) this.checkmarkVehicleSpeedTransmission).Tag = (object) "Vehicle Speed Invalid.";
    componentResourceManager.ApplyResources((object) this.label7, "label7");
    this.label7.Name = "label7";
    this.label7.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel14, "tableLayoutPanel14");
    ((TableLayoutPanel) this.tableLayoutPanel14).Controls.Add((Control) this.label8, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel14).Controls.Add((Control) this.checkmarkDrivenDistanceTransmission, 0, 0);
    ((Control) this.tableLayoutPanel14).Name = "tableLayoutPanel14";
    componentResourceManager.ApplyResources((object) this.label8, "label8");
    this.label8.Name = "label8";
    this.label8.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.checkmarkDrivenDistanceTransmission, "checkmarkDrivenDistanceTransmission");
    ((Control) this.checkmarkDrivenDistanceTransmission).Name = "checkmarkDrivenDistanceTransmission";
    ((Control) this.checkmarkDrivenDistanceTransmission).Tag = (object) "Driven Distance too low.";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelTransmissionStatus, "tableLayoutPanelTransmissionStatus");
    ((TableLayoutPanel) this.tableLayoutPanelTransmissionStatus).Controls.Add((Control) this.labelTransmissionStatus, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanelTransmissionStatus).Controls.Add((Control) this.checkmarkTransmissionStatus, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelTransmissionStatus).Controls.Add((Control) this.runServiceButtonNewOilFilterTransmission, 2, 0);
    ((Control) this.tableLayoutPanelTransmissionStatus).Name = "tableLayoutPanelTransmissionStatus";
    componentResourceManager.ApplyResources((object) this.labelTransmissionStatus, "labelTransmissionStatus");
    this.labelTransmissionStatus.Name = "labelTransmissionStatus";
    ((TableLayoutPanel) this.tableLayoutPanelTransmissionStatus).SetRowSpan((Control) this.labelTransmissionStatus, 2);
    this.labelTransmissionStatus.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.checkmarkTransmissionStatus, "checkmarkTransmissionStatus");
    ((Control) this.checkmarkTransmissionStatus).Name = "checkmarkTransmissionStatus";
    ((TableLayoutPanel) this.tableLayoutPanelTransmissionStatus).SetRowSpan((Control) this.checkmarkTransmissionStatus, 2);
    componentResourceManager.ApplyResources((object) this.runServiceButtonNewOilFilterTransmission, "runServiceButtonNewOilFilterTransmission");
    ((Control) this.runServiceButtonNewOilFilterTransmission).Name = "runServiceButtonNewOilFilterTransmission";
    ((TableLayoutPanel) this.tableLayoutPanelTransmissionStatus).SetRowSpan((Control) this.runServiceButtonNewOilFilterTransmission, 2);
    this.runServiceButtonNewOilFilterTransmission.ServiceCall = new ServiceCall("MS01T", "DL_Reset_service_information_selected_channel", (IEnumerable<string>) new string[2]
    {
      "Channel_number=2",
      "Filter_condition_Diesel_particle_filter_only=0"
    });
    ((Control) this.runServiceButtonNewOilFilterTransmission).Tag = (object) "Engine";
    ((RunSharedProcedureButtonBase) this.runServiceButtonNewOilFilterTransmission).Starting += new EventHandler<CancelEventArgs>(this.runServiceButtonReplaceOilFilterTransmission_Starting);
    ((RunSharedProcedureButtonBase) this.runServiceButtonNewOilFilterTransmission).Started += new EventHandler<PassFailResultEventArgs>(this.runServiceButtonReplaceOilFilterTransmission_Started);
    componentResourceManager.ApplyResources((object) this.label10, "label10");
    this.label10.Name = "label10";
    this.label10.UseCompatibleTextRendering = true;
    this.webBrowsersystemInputs.AllowWebBrowserDrop = false;
    componentResourceManager.ApplyResources((object) this.webBrowsersystemInputs, "webBrowsersystemInputs");
    this.webBrowsersystemInputs.IsWebBrowserContextMenuEnabled = false;
    this.webBrowsersystemInputs.Name = "webBrowsersystemInputs";
    ((TableLayoutPanel) this.tableLayoutPanelMain).SetRowSpan((Control) this.webBrowsersystemInputs, 5);
    this.webBrowsersystemInputs.Url = new Uri("about: blank", UriKind.Absolute);
    this.webBrowsersystemInputs.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(this.webBrowserEngineValues_DocumentCompleted);
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelHeader, "tableLayoutPanelHeader");
    ((TableLayoutPanel) this.tableLayoutPanelMain).SetColumnSpan((Control) this.tableLayoutPanelHeader, 2);
    ((TableLayoutPanel) this.tableLayoutPanelHeader).Controls.Add((Control) this.digitalReadoutInstrumentVehicleSpeed, 3, 0);
    ((TableLayoutPanel) this.tableLayoutPanelHeader).Controls.Add((Control) this.digitalReadoutInstrumentIgnitionSwitch, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanelHeader).Controls.Add((Control) this.digitalReadoutInstrumentMinimumDrivenDistance, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelHeader).Controls.Add((Control) this.tableLayoutPanel5, 1, 0);
    ((Control) this.tableLayoutPanelHeader).Name = "tableLayoutPanelHeader";
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentVehicleSpeed, "digitalReadoutInstrumentVehicleSpeed");
    this.digitalReadoutInstrumentVehicleSpeed.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentVehicleSpeed).FreezeValue = false;
    this.digitalReadoutInstrumentVehicleSpeed.Gradient.Initialize((ValueState) 3, 2);
    this.digitalReadoutInstrumentVehicleSpeed.Gradient.Modify(1, 0.0, (ValueState) 1);
    this.digitalReadoutInstrumentVehicleSpeed.Gradient.Modify(2, 1.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentVehicleSpeed).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "vehicleSpeed");
    ((Control) this.digitalReadoutInstrumentVehicleSpeed).Name = "digitalReadoutInstrumentVehicleSpeed";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentVehicleSpeed).UnitAlignment = StringAlignment.Near;
    this.digitalReadoutInstrumentVehicleSpeed.RepresentedStateChanged += new EventHandler(this.digitalReadoutInstrument_RepresentedStateChanged);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentIgnitionSwitch, "digitalReadoutInstrumentIgnitionSwitch");
    this.digitalReadoutInstrumentIgnitionSwitch.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentIgnitionSwitch).FreezeValue = false;
    this.digitalReadoutInstrumentIgnitionSwitch.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText"));
    this.digitalReadoutInstrumentIgnitionSwitch.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText1"));
    this.digitalReadoutInstrumentIgnitionSwitch.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText2"));
    this.digitalReadoutInstrumentIgnitionSwitch.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText3"));
    this.digitalReadoutInstrumentIgnitionSwitch.Gradient.Initialize((ValueState) 3, 3);
    this.digitalReadoutInstrumentIgnitionSwitch.Gradient.Modify(1, 0.0, (ValueState) 3);
    this.digitalReadoutInstrumentIgnitionSwitch.Gradient.Modify(2, 1.0, (ValueState) 1);
    this.digitalReadoutInstrumentIgnitionSwitch.Gradient.Modify(3, 2.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentIgnitionSwitch).Instrument = new Qualifier((QualifierTypes) 1, "virtual", "ignitionStatus");
    ((Control) this.digitalReadoutInstrumentIgnitionSwitch).Name = "digitalReadoutInstrumentIgnitionSwitch";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentIgnitionSwitch).ShowValueReadout = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentIgnitionSwitch).UnitAlignment = StringAlignment.Near;
    this.digitalReadoutInstrumentIgnitionSwitch.RepresentedStateChanged += new EventHandler(this.digitalReadoutInstrument_RepresentedStateChanged);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentMinimumDrivenDistance, "digitalReadoutInstrumentMinimumDrivenDistance");
    this.digitalReadoutInstrumentMinimumDrivenDistance.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentMinimumDrivenDistance).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentMinimumDrivenDistance).Instrument = new Qualifier((QualifierTypes) 4, "MS01T", "Minimum_driven_distance_reset_PAR_FsMinRs_FZG");
    ((Control) this.digitalReadoutInstrumentMinimumDrivenDistance).Name = "digitalReadoutInstrumentMinimumDrivenDistance";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentMinimumDrivenDistance).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel5, "tableLayoutPanel5");
    ((TableLayoutPanel) this.tableLayoutPanel5).Controls.Add((Control) this.label20, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel5).Controls.Add((Control) this.label22, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel5).Controls.Add((Control) this.scalingLabelMinimumTime, 0, 1);
    ((Control) this.tableLayoutPanel5).Name = "tableLayoutPanel5";
    this.label20.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.label20, "label20");
    ((Control) this.label20).Name = "label20";
    this.label20.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.label22.Alignment = StringAlignment.Far;
    componentResourceManager.ApplyResources((object) this.label22, "label22");
    ((Control) this.label22).Name = "label22";
    this.label22.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.scalingLabelMinimumTime.Alignment = StringAlignment.Far;
    ((TableLayoutPanel) this.tableLayoutPanel5).SetColumnSpan((Control) this.scalingLabelMinimumTime, 2);
    componentResourceManager.ApplyResources((object) this.scalingLabelMinimumTime, "scalingLabelMinimumTime");
    this.scalingLabelMinimumTime.FontGroup = (string) null;
    this.scalingLabelMinimumTime.LineAlignment = StringAlignment.Center;
    ((Control) this.scalingLabelMinimumTime).Name = "scalingLabelMinimumTime";
    componentResourceManager.ApplyResources((object) this.panelEngine, "panelEngine");
    this.panelEngine.BorderStyle = BorderStyle.FixedSingle;
    this.panelEngine.Controls.Add((Control) this.tableLayoutPanelEngine);
    this.panelEngine.Name = "panelEngine";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelEngine, "tableLayoutPanelEngine");
    ((TableLayoutPanel) this.tableLayoutPanelEngine).Controls.Add((Control) this.tableLayoutPanelEngineStatus, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanelEngine).Controls.Add((Control) this.tableLayoutPanelEngineStatusMessage, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanelEngine).Controls.Add((Control) this.label3, 0, 0);
    ((Control) this.tableLayoutPanelEngine).Name = "tableLayoutPanelEngine";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelEngineStatus, "tableLayoutPanelEngineStatus");
    ((TableLayoutPanel) this.tableLayoutPanelEngineStatus).Controls.Add((Control) this.tableLayoutPanel4, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanelEngineStatus).Controls.Add((Control) this.tableLayoutPanel1, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanelEngineStatus).Controls.Add((Control) this.tableLayoutPanel2, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanelEngineStatus).Controls.Add((Control) this.tableLayoutPanel6, 2, 1);
    ((TableLayoutPanel) this.tableLayoutPanelEngineStatus).Controls.Add((Control) this.tableLayoutPanel3, 0, 0);
    ((Control) this.tableLayoutPanelEngineStatus).Name = "tableLayoutPanelEngineStatus";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel4, "tableLayoutPanel4");
    ((TableLayoutPanel) this.tableLayoutPanel4).Controls.Add((Control) this.labelOperatingTimeEngine, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel4).Controls.Add((Control) this.checkmarkOperatingTimeEngine, 0, 0);
    ((Control) this.tableLayoutPanel4).Name = "tableLayoutPanel4";
    componentResourceManager.ApplyResources((object) this.labelOperatingTimeEngine, "labelOperatingTimeEngine");
    this.labelOperatingTimeEngine.Name = "labelOperatingTimeEngine";
    this.labelOperatingTimeEngine.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.checkmarkOperatingTimeEngine, "checkmarkOperatingTimeEngine");
    ((Control) this.checkmarkOperatingTimeEngine).Name = "checkmarkOperatingTimeEngine";
    ((Control) this.checkmarkOperatingTimeEngine).Tag = (object) "Operating Time too low.";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.checkmarkSystemActiveEngine, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.labelSystemActiveEngine, 1, 0);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    componentResourceManager.ApplyResources((object) this.checkmarkSystemActiveEngine, "checkmarkSystemActiveEngine");
    ((Control) this.checkmarkSystemActiveEngine).Name = "checkmarkSystemActiveEngine";
    ((Control) this.checkmarkSystemActiveEngine).Tag = (object) "Oil System Inactive.";
    componentResourceManager.ApplyResources((object) this.labelSystemActiveEngine, "labelSystemActiveEngine");
    this.labelSystemActiveEngine.Name = "labelSystemActiveEngine";
    this.labelSystemActiveEngine.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel2, "tableLayoutPanel2");
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.labelIgnitionSwitchEngine, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.checkmarkIgnitionSwitchEngine, 0, 0);
    ((Control) this.tableLayoutPanel2).Name = "tableLayoutPanel2";
    componentResourceManager.ApplyResources((object) this.labelIgnitionSwitchEngine, "labelIgnitionSwitchEngine");
    this.labelIgnitionSwitchEngine.Name = "labelIgnitionSwitchEngine";
    this.labelIgnitionSwitchEngine.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.checkmarkIgnitionSwitchEngine, "checkmarkIgnitionSwitchEngine");
    ((Control) this.checkmarkIgnitionSwitchEngine).Name = "checkmarkIgnitionSwitchEngine";
    ((Control) this.checkmarkIgnitionSwitchEngine).Tag = (object) "Ignition Switch Off.";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel6, "tableLayoutPanel6");
    ((TableLayoutPanel) this.tableLayoutPanel6).Controls.Add((Control) this.checkmarkVehicleSpeedEngine, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel6).Controls.Add((Control) this.label2, 1, 0);
    ((Control) this.tableLayoutPanel6).Name = "tableLayoutPanel6";
    componentResourceManager.ApplyResources((object) this.checkmarkVehicleSpeedEngine, "checkmarkVehicleSpeedEngine");
    ((Control) this.checkmarkVehicleSpeedEngine).Name = "checkmarkVehicleSpeedEngine";
    ((Control) this.checkmarkVehicleSpeedEngine).Tag = (object) "Vehicle Speed Invalid.";
    componentResourceManager.ApplyResources((object) this.label2, "label2");
    this.label2.Name = "label2";
    this.label2.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel3, "tableLayoutPanel3");
    ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.labelDrivenDistanceEngine, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.checkmarkDrivenDistanceEngine, 0, 0);
    ((Control) this.tableLayoutPanel3).Name = "tableLayoutPanel3";
    componentResourceManager.ApplyResources((object) this.labelDrivenDistanceEngine, "labelDrivenDistanceEngine");
    this.labelDrivenDistanceEngine.Name = "labelDrivenDistanceEngine";
    this.labelDrivenDistanceEngine.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.checkmarkDrivenDistanceEngine, "checkmarkDrivenDistanceEngine");
    ((Control) this.checkmarkDrivenDistanceEngine).Name = "checkmarkDrivenDistanceEngine";
    ((Control) this.checkmarkDrivenDistanceEngine).Tag = (object) "Driven Distance too low.";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelEngineStatusMessage, "tableLayoutPanelEngineStatusMessage");
    ((TableLayoutPanel) this.tableLayoutPanelEngineStatusMessage).Controls.Add((Control) this.labelEngineStatus, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanelEngineStatusMessage).Controls.Add((Control) this.checkmarkEngineStatus, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelEngineStatusMessage).Controls.Add((Control) this.runServiceButtonNewOilFilterEngine, 2, 0);
    ((Control) this.tableLayoutPanelEngineStatusMessage).Name = "tableLayoutPanelEngineStatusMessage";
    componentResourceManager.ApplyResources((object) this.labelEngineStatus, "labelEngineStatus");
    this.labelEngineStatus.Name = "labelEngineStatus";
    ((TableLayoutPanel) this.tableLayoutPanelEngineStatusMessage).SetRowSpan((Control) this.labelEngineStatus, 2);
    this.labelEngineStatus.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.checkmarkEngineStatus, "checkmarkEngineStatus");
    ((Control) this.checkmarkEngineStatus).Name = "checkmarkEngineStatus";
    ((TableLayoutPanel) this.tableLayoutPanelEngineStatusMessage).SetRowSpan((Control) this.checkmarkEngineStatus, 2);
    componentResourceManager.ApplyResources((object) this.runServiceButtonNewOilFilterEngine, "runServiceButtonNewOilFilterEngine");
    ((Control) this.runServiceButtonNewOilFilterEngine).Name = "runServiceButtonNewOilFilterEngine";
    ((TableLayoutPanel) this.tableLayoutPanelEngineStatusMessage).SetRowSpan((Control) this.runServiceButtonNewOilFilterEngine, 2);
    this.runServiceButtonNewOilFilterEngine.ServiceCall = new ServiceCall("MS01T", "DL_Reset_service_information_selected_channel", (IEnumerable<string>) new string[2]
    {
      "Channel_number=1",
      "Filter_condition_Diesel_particle_filter_only=0"
    });
    ((Control) this.runServiceButtonNewOilFilterEngine).Tag = (object) "Engine";
    ((RunSharedProcedureButtonBase) this.runServiceButtonNewOilFilterEngine).Starting += new EventHandler<CancelEventArgs>(this.runServiceButtonReplaceOilFilterEngine_Starting);
    ((RunSharedProcedureButtonBase) this.runServiceButtonNewOilFilterEngine).Started += new EventHandler<PassFailResultEventArgs>(this.runServiceButtonReplaceOilFilterEngine_Started);
    componentResourceManager.ApplyResources((object) this.label3, "label3");
    this.label3.Name = "label3";
    this.label3.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.panelAxle1, "panelAxle1");
    this.panelAxle1.BorderStyle = BorderStyle.FixedSingle;
    this.panelAxle1.Controls.Add((Control) this.tableLayoutPanelAxle1);
    this.panelAxle1.Name = "panelAxle1";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelAxle1, "tableLayoutPanelAxle1");
    ((TableLayoutPanel) this.tableLayoutPanelAxle1).Controls.Add((Control) this.tableLayoutPanelAxle1Conditions, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanelAxle1).Controls.Add((Control) this.tableLayoutPanelAxle1Status, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanelAxle1).Controls.Add((Control) this.label15, 0, 0);
    ((Control) this.tableLayoutPanelAxle1).Name = "tableLayoutPanelAxle1";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelAxle1Conditions, "tableLayoutPanelAxle1Conditions");
    ((TableLayoutPanel) this.tableLayoutPanelAxle1Conditions).Controls.Add((Control) this.tableLayoutPanel19, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanelAxle1Conditions).Controls.Add((Control) this.tableLayoutPanel20, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanelAxle1Conditions).Controls.Add((Control) this.tableLayoutPanel21, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanelAxle1Conditions).Controls.Add((Control) this.tableLayoutPanel22, 2, 1);
    ((TableLayoutPanel) this.tableLayoutPanelAxle1Conditions).Controls.Add((Control) this.tableLayoutPanel23, 0, 0);
    ((Control) this.tableLayoutPanelAxle1Conditions).Name = "tableLayoutPanelAxle1Conditions";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel19, "tableLayoutPanel19");
    ((TableLayoutPanel) this.tableLayoutPanel19).Controls.Add((Control) this.label1, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel19).Controls.Add((Control) this.checkmarkOperatingTimeAxle1, 0, 0);
    ((Control) this.tableLayoutPanel19).Name = "tableLayoutPanel19";
    componentResourceManager.ApplyResources((object) this.label1, "label1");
    this.label1.Name = "label1";
    this.label1.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.checkmarkOperatingTimeAxle1, "checkmarkOperatingTimeAxle1");
    ((Control) this.checkmarkOperatingTimeAxle1).Name = "checkmarkOperatingTimeAxle1";
    ((Control) this.checkmarkOperatingTimeAxle1).Tag = (object) "Operating Time too low.";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel20, "tableLayoutPanel20");
    ((TableLayoutPanel) this.tableLayoutPanel20).Controls.Add((Control) this.checkmarkSystemActiveAxle1, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel20).Controls.Add((Control) this.label9, 1, 0);
    ((Control) this.tableLayoutPanel20).Name = "tableLayoutPanel20";
    componentResourceManager.ApplyResources((object) this.checkmarkSystemActiveAxle1, "checkmarkSystemActiveAxle1");
    ((Control) this.checkmarkSystemActiveAxle1).Name = "checkmarkSystemActiveAxle1";
    ((Control) this.checkmarkSystemActiveAxle1).Tag = (object) "Oil System Inactive.";
    componentResourceManager.ApplyResources((object) this.label9, "label9");
    this.label9.Name = "label9";
    this.label9.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel21, "tableLayoutPanel21");
    ((TableLayoutPanel) this.tableLayoutPanel21).Controls.Add((Control) this.label11, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel21).Controls.Add((Control) this.checkmarkIgnitionSwitchAxle1, 0, 0);
    ((Control) this.tableLayoutPanel21).Name = "tableLayoutPanel21";
    componentResourceManager.ApplyResources((object) this.label11, "label11");
    this.label11.Name = "label11";
    this.label11.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.checkmarkIgnitionSwitchAxle1, "checkmarkIgnitionSwitchAxle1");
    ((Control) this.checkmarkIgnitionSwitchAxle1).Name = "checkmarkIgnitionSwitchAxle1";
    ((Control) this.checkmarkIgnitionSwitchAxle1).Tag = (object) "Ignition Switch Off.";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel22, "tableLayoutPanel22");
    ((TableLayoutPanel) this.tableLayoutPanel22).Controls.Add((Control) this.checkmarkVehicleSpeedAxle1, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel22).Controls.Add((Control) this.label12, 1, 0);
    ((Control) this.tableLayoutPanel22).Name = "tableLayoutPanel22";
    componentResourceManager.ApplyResources((object) this.checkmarkVehicleSpeedAxle1, "checkmarkVehicleSpeedAxle1");
    ((Control) this.checkmarkVehicleSpeedAxle1).Name = "checkmarkVehicleSpeedAxle1";
    ((Control) this.checkmarkVehicleSpeedAxle1).Tag = (object) "Vehicle Speed Invalid.";
    componentResourceManager.ApplyResources((object) this.label12, "label12");
    this.label12.Name = "label12";
    this.label12.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel23, "tableLayoutPanel23");
    ((TableLayoutPanel) this.tableLayoutPanel23).Controls.Add((Control) this.label13, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel23).Controls.Add((Control) this.checkmarkDrivenDistanceAxle1, 0, 0);
    ((Control) this.tableLayoutPanel23).Name = "tableLayoutPanel23";
    componentResourceManager.ApplyResources((object) this.label13, "label13");
    this.label13.Name = "label13";
    this.label13.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.checkmarkDrivenDistanceAxle1, "checkmarkDrivenDistanceAxle1");
    ((Control) this.checkmarkDrivenDistanceAxle1).Name = "checkmarkDrivenDistanceAxle1";
    ((Control) this.checkmarkDrivenDistanceAxle1).Tag = (object) "Driven Distance too low.";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelAxle1Status, "tableLayoutPanelAxle1Status");
    ((TableLayoutPanel) this.tableLayoutPanelAxle1Status).Controls.Add((Control) this.labelAxle1Status, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanelAxle1Status).Controls.Add((Control) this.checkmarkAxle1Status, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelAxle1Status).Controls.Add((Control) this.runServiceButtonNewOilFilterAxle1, 2, 0);
    ((Control) this.tableLayoutPanelAxle1Status).Name = "tableLayoutPanelAxle1Status";
    componentResourceManager.ApplyResources((object) this.labelAxle1Status, "labelAxle1Status");
    this.labelAxle1Status.Name = "labelAxle1Status";
    ((TableLayoutPanel) this.tableLayoutPanelAxle1Status).SetRowSpan((Control) this.labelAxle1Status, 2);
    this.labelAxle1Status.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.checkmarkAxle1Status, "checkmarkAxle1Status");
    ((Control) this.checkmarkAxle1Status).Name = "checkmarkAxle1Status";
    ((TableLayoutPanel) this.tableLayoutPanelAxle1Status).SetRowSpan((Control) this.checkmarkAxle1Status, 2);
    componentResourceManager.ApplyResources((object) this.runServiceButtonNewOilFilterAxle1, "runServiceButtonNewOilFilterAxle1");
    ((Control) this.runServiceButtonNewOilFilterAxle1).Name = "runServiceButtonNewOilFilterAxle1";
    ((TableLayoutPanel) this.tableLayoutPanelAxle1Status).SetRowSpan((Control) this.runServiceButtonNewOilFilterAxle1, 2);
    this.runServiceButtonNewOilFilterAxle1.ServiceCall = new ServiceCall("MS01T", "DL_Reset_service_information_selected_channel", (IEnumerable<string>) new string[2]
    {
      "Channel_number=6",
      "Filter_condition_Diesel_particle_filter_only=0"
    });
    ((Control) this.runServiceButtonNewOilFilterAxle1).Tag = (object) "Engine";
    ((RunSharedProcedureButtonBase) this.runServiceButtonNewOilFilterAxle1).Starting += new EventHandler<CancelEventArgs>(this.runServiceButtonReplaceOilFilterAxle1_Starting);
    ((RunSharedProcedureButtonBase) this.runServiceButtonNewOilFilterAxle1).Started += new EventHandler<PassFailResultEventArgs>(this.runServiceButtonReplaceOilFilterAxle1_Started);
    componentResourceManager.ApplyResources((object) this.label15, "label15");
    this.label15.Name = "label15";
    this.label15.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.panelAxle2, "panelAxle2");
    this.panelAxle2.BorderStyle = BorderStyle.FixedSingle;
    this.panelAxle2.Controls.Add((Control) this.tableLayoutPanelAxle2);
    this.panelAxle2.Name = "panelAxle2";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelAxle2, "tableLayoutPanelAxle2");
    ((TableLayoutPanel) this.tableLayoutPanelAxle2).Controls.Add((Control) this.Axle2Conditions, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanelAxle2).Controls.Add((Control) this.tableLayoutPanelAxle2Status, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanelAxle2).Controls.Add((Control) this.label21, 0, 0);
    ((Control) this.tableLayoutPanelAxle2).Name = "tableLayoutPanelAxle2";
    componentResourceManager.ApplyResources((object) this.Axle2Conditions, "Axle2Conditions");
    ((TableLayoutPanel) this.Axle2Conditions).Controls.Add((Control) this.tableLayoutPanel28, 1, 0);
    ((TableLayoutPanel) this.Axle2Conditions).Controls.Add((Control) this.tableLayoutPanel29, 0, 1);
    ((TableLayoutPanel) this.Axle2Conditions).Controls.Add((Control) this.tableLayoutPanel30, 2, 0);
    ((TableLayoutPanel) this.Axle2Conditions).Controls.Add((Control) this.tableLayoutPanel31, 2, 1);
    ((TableLayoutPanel) this.Axle2Conditions).Controls.Add((Control) this.tableLayoutPanel32, 0, 0);
    ((Control) this.Axle2Conditions).Name = "Axle2Conditions";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel28, "tableLayoutPanel28");
    ((TableLayoutPanel) this.tableLayoutPanel28).Controls.Add((Control) this.label14, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel28).Controls.Add((Control) this.checkmarkOperatingTimeAxle2, 0, 0);
    ((Control) this.tableLayoutPanel28).Name = "tableLayoutPanel28";
    componentResourceManager.ApplyResources((object) this.label14, "label14");
    this.label14.Name = "label14";
    this.label14.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.checkmarkOperatingTimeAxle2, "checkmarkOperatingTimeAxle2");
    ((Control) this.checkmarkOperatingTimeAxle2).Name = "checkmarkOperatingTimeAxle2";
    ((Control) this.checkmarkOperatingTimeAxle2).Tag = (object) "Operating Time too low.";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel29, "tableLayoutPanel29");
    ((TableLayoutPanel) this.tableLayoutPanel29).Controls.Add((Control) this.checkmarkSystemActiveAxle2, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel29).Controls.Add((Control) this.label16, 1, 0);
    ((Control) this.tableLayoutPanel29).Name = "tableLayoutPanel29";
    componentResourceManager.ApplyResources((object) this.checkmarkSystemActiveAxle2, "checkmarkSystemActiveAxle2");
    ((Control) this.checkmarkSystemActiveAxle2).Name = "checkmarkSystemActiveAxle2";
    ((Control) this.checkmarkSystemActiveAxle2).Tag = (object) "Oil System Inactive.";
    componentResourceManager.ApplyResources((object) this.label16, "label16");
    this.label16.Name = "label16";
    this.label16.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel30, "tableLayoutPanel30");
    ((TableLayoutPanel) this.tableLayoutPanel30).Controls.Add((Control) this.label17, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel30).Controls.Add((Control) this.checkmarkIgnitionSwitchAxle2, 0, 0);
    ((Control) this.tableLayoutPanel30).Name = "tableLayoutPanel30";
    componentResourceManager.ApplyResources((object) this.label17, "label17");
    this.label17.Name = "label17";
    this.label17.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.checkmarkIgnitionSwitchAxle2, "checkmarkIgnitionSwitchAxle2");
    ((Control) this.checkmarkIgnitionSwitchAxle2).Name = "checkmarkIgnitionSwitchAxle2";
    ((Control) this.checkmarkIgnitionSwitchAxle2).Tag = (object) "Ignition Switch Off.";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel31, "tableLayoutPanel31");
    ((TableLayoutPanel) this.tableLayoutPanel31).Controls.Add((Control) this.checkmarkVehicleSpeedAxle2, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel31).Controls.Add((Control) this.label18, 1, 0);
    ((Control) this.tableLayoutPanel31).Name = "tableLayoutPanel31";
    componentResourceManager.ApplyResources((object) this.checkmarkVehicleSpeedAxle2, "checkmarkVehicleSpeedAxle2");
    ((Control) this.checkmarkVehicleSpeedAxle2).Name = "checkmarkVehicleSpeedAxle2";
    ((Control) this.checkmarkVehicleSpeedAxle2).Tag = (object) "Vehicle Speed Invalid.";
    componentResourceManager.ApplyResources((object) this.label18, "label18");
    this.label18.Name = "label18";
    this.label18.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel32, "tableLayoutPanel32");
    ((TableLayoutPanel) this.tableLayoutPanel32).Controls.Add((Control) this.label19, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel32).Controls.Add((Control) this.checkmarkDrivenDistanceAxle2, 0, 0);
    ((Control) this.tableLayoutPanel32).Name = "tableLayoutPanel32";
    componentResourceManager.ApplyResources((object) this.label19, "label19");
    this.label19.Name = "label19";
    this.label19.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.checkmarkDrivenDistanceAxle2, "checkmarkDrivenDistanceAxle2");
    ((Control) this.checkmarkDrivenDistanceAxle2).Name = "checkmarkDrivenDistanceAxle2";
    ((Control) this.checkmarkDrivenDistanceAxle2).Tag = (object) "Driven Distance too low.";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelAxle2Status, "tableLayoutPanelAxle2Status");
    ((TableLayoutPanel) this.tableLayoutPanelAxle2Status).Controls.Add((Control) this.labelAxle2Status, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanelAxle2Status).Controls.Add((Control) this.checkmarkAxle2Status, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelAxle2Status).Controls.Add((Control) this.runServiceButtonNewOilFilterAxle2, 2, 0);
    ((Control) this.tableLayoutPanelAxle2Status).Name = "tableLayoutPanelAxle2Status";
    componentResourceManager.ApplyResources((object) this.labelAxle2Status, "labelAxle2Status");
    this.labelAxle2Status.Name = "labelAxle2Status";
    ((TableLayoutPanel) this.tableLayoutPanelAxle2Status).SetRowSpan((Control) this.labelAxle2Status, 2);
    this.labelAxle2Status.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.checkmarkAxle2Status, "checkmarkAxle2Status");
    ((Control) this.checkmarkAxle2Status).Name = "checkmarkAxle2Status";
    ((TableLayoutPanel) this.tableLayoutPanelAxle2Status).SetRowSpan((Control) this.checkmarkAxle2Status, 2);
    componentResourceManager.ApplyResources((object) this.runServiceButtonNewOilFilterAxle2, "runServiceButtonNewOilFilterAxle2");
    ((Control) this.runServiceButtonNewOilFilterAxle2).Name = "runServiceButtonNewOilFilterAxle2";
    ((TableLayoutPanel) this.tableLayoutPanelAxle2Status).SetRowSpan((Control) this.runServiceButtonNewOilFilterAxle2, 2);
    this.runServiceButtonNewOilFilterAxle2.ServiceCall = new ServiceCall("MS01T", "DL_Reset_service_information_selected_channel", (IEnumerable<string>) new string[2]
    {
      "Channel_number=20",
      "Filter_condition_Diesel_particle_filter_only=0"
    });
    ((Control) this.runServiceButtonNewOilFilterAxle2).Tag = (object) "Engine";
    ((RunSharedProcedureButtonBase) this.runServiceButtonNewOilFilterAxle2).Starting += new EventHandler<CancelEventArgs>(this.runServiceButtonReplaceOilFilterAxle2_Starting);
    ((RunSharedProcedureButtonBase) this.runServiceButtonNewOilFilterAxle2).Started += new EventHandler<PassFailResultEventArgs>(this.runServiceButtonReplaceOilFilterAxle2_Started);
    componentResourceManager.ApplyResources((object) this.label21, "label21");
    this.label21.Name = "label21";
    this.label21.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("_DDDL.chm_Detroit_Maintenance_System");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanelMain);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this).SizeChanged += new EventHandler(this.UserPanel_SizeChanged);
    ((Control) this.tableLayoutPanelMain).ResumeLayout(false);
    ((Control) this.tableLayoutPanelMain).PerformLayout();
    this.panelTransmission.ResumeLayout(false);
    this.panelTransmission.PerformLayout();
    ((Control) this.tableLayoutPanelTransmission).ResumeLayout(false);
    ((Control) this.tableLayoutPanelTransmission).PerformLayout();
    ((Control) this.tableLayoutPanelTransmissionConditions).ResumeLayout(false);
    ((Control) this.tableLayoutPanelTransmissionConditions).PerformLayout();
    ((Control) this.tableLayoutPanel10).ResumeLayout(false);
    ((Control) this.tableLayoutPanel11).ResumeLayout(false);
    ((Control) this.tableLayoutPanel12).ResumeLayout(false);
    ((Control) this.tableLayoutPanel13).ResumeLayout(false);
    ((Control) this.tableLayoutPanel14).ResumeLayout(false);
    ((Control) this.tableLayoutPanelTransmissionStatus).ResumeLayout(false);
    ((Control) this.tableLayoutPanelTransmissionStatus).PerformLayout();
    ((Control) this.tableLayoutPanelHeader).ResumeLayout(false);
    ((Control) this.tableLayoutPanel5).ResumeLayout(false);
    this.panelEngine.ResumeLayout(false);
    this.panelEngine.PerformLayout();
    ((Control) this.tableLayoutPanelEngine).ResumeLayout(false);
    ((Control) this.tableLayoutPanelEngine).PerformLayout();
    ((Control) this.tableLayoutPanelEngineStatus).ResumeLayout(false);
    ((Control) this.tableLayoutPanelEngineStatus).PerformLayout();
    ((Control) this.tableLayoutPanel4).ResumeLayout(false);
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this.tableLayoutPanel2).ResumeLayout(false);
    ((Control) this.tableLayoutPanel6).ResumeLayout(false);
    ((Control) this.tableLayoutPanel3).ResumeLayout(false);
    ((Control) this.tableLayoutPanelEngineStatusMessage).ResumeLayout(false);
    ((Control) this.tableLayoutPanelEngineStatusMessage).PerformLayout();
    this.panelAxle1.ResumeLayout(false);
    this.panelAxle1.PerformLayout();
    ((Control) this.tableLayoutPanelAxle1).ResumeLayout(false);
    ((Control) this.tableLayoutPanelAxle1).PerformLayout();
    ((Control) this.tableLayoutPanelAxle1Conditions).ResumeLayout(false);
    ((Control) this.tableLayoutPanelAxle1Conditions).PerformLayout();
    ((Control) this.tableLayoutPanel19).ResumeLayout(false);
    ((Control) this.tableLayoutPanel20).ResumeLayout(false);
    ((Control) this.tableLayoutPanel21).ResumeLayout(false);
    ((Control) this.tableLayoutPanel22).ResumeLayout(false);
    ((Control) this.tableLayoutPanel23).ResumeLayout(false);
    ((Control) this.tableLayoutPanelAxle1Status).ResumeLayout(false);
    ((Control) this.tableLayoutPanelAxle1Status).PerformLayout();
    this.panelAxle2.ResumeLayout(false);
    this.panelAxle2.PerformLayout();
    ((Control) this.tableLayoutPanelAxle2).ResumeLayout(false);
    ((Control) this.tableLayoutPanelAxle2).PerformLayout();
    ((Control) this.Axle2Conditions).ResumeLayout(false);
    ((Control) this.Axle2Conditions).PerformLayout();
    ((Control) this.tableLayoutPanel28).ResumeLayout(false);
    ((Control) this.tableLayoutPanel29).ResumeLayout(false);
    ((Control) this.tableLayoutPanel30).ResumeLayout(false);
    ((Control) this.tableLayoutPanel31).ResumeLayout(false);
    ((Control) this.tableLayoutPanel32).ResumeLayout(false);
    ((Control) this.tableLayoutPanelAxle2Status).ResumeLayout(false);
    ((Control) this.tableLayoutPanelAxle2Status).PerformLayout();
    ((Control) this).ResumeLayout(false);
    ((Control) this).PerformLayout();
  }
}
