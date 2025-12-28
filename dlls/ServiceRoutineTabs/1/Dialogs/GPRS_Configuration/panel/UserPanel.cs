// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.GPRS_Configuration.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.GPRS_Configuration.panel;

public class UserPanel : CustomPanel
{
  private const string GprsConfigValue = "1F8B080000000000000073767367656060B8C9758D914B994B3625B33839BF2CB5A852AFB4582F393F29512F2531333727B508C8C915B837935988D9D0DC408AD9D8D0408937B120CF502FA5242F112469A5CC2597985E9E999759A2575094A297925A52949F59929C9F97979A5C02D6FEC7CE499E4B0AA8089782BF761EAC010D4F3922181318B28430151511E3C22A547701000CC0CF68E1000000FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF";
  private const string GprsConfigReadQualifier = "DT_STO_Gprs_Config_1_gprsConfig";
  private const string GprsConfigWriteQualifier = "DL_Gprs_Config_1";
  private const string AntennaDiagnosticGroupQualifier = "VCD_PID_Antenna_diagnostic";
  private const string HardResetQualifier = "FN_HardReset";
  private const string HardwarePartNumberQualifier = "CO_HardwarePartNumber";
  private const string SoftwarePartNumberQualifier = "CO_SoftwarePartNumber";
  private readonly List<string> AffectedHardwarePartNumbers = new List<string>()
  {
    "66-05466-001",
    "66-10777-001",
    "66-13928-001",
    "66-13931-001"
  };
  private readonly List<string> AffectedSoftwarePartNumbers = new List<string>()
  {
    "A0014487260-001",
    "A0014485460-001"
  };
  private Channel channel;
  private TableLayoutPanel tableLayoutPanel1;
  private SeekTimeListView seekTimeListView1;
  private Checkmark checkmarkAffectedPartNumber;
  private Checkmark checkmarkGprsConfigCorrect;
  private Button buttonFixGprs;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label labelAffectedPartNumber;
  private Button buttonClose;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label labelGprsConfigCorrect;

  public UserPanel() => this.InitializeComponent();

  public virtual void OnChannelsChanged()
  {
    this.SetCTP(this.GetChannel("CTP01T", (CustomPanel.ChannelLookupOptions) 3));
  }

  protected virtual void OnLoad(EventArgs e)
  {
    ((ContainerControl) this).ParentForm.FormClosing += new FormClosingEventHandler(this.OnFormClosing);
    this.ResetResults();
    this.SetCTP(this.GetChannel("CTP01T", (CustomPanel.ChannelLookupOptions) 3));
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
  }

  private void OnFormClosing(object sender, FormClosingEventArgs e)
  {
    e.Cancel = this.Busy;
    if (e.Cancel)
      return;
    ((Control) this).Tag = (object) new object[2]
    {
      (object) this.Result,
      (object) this.ResultMessage
    };
    this.buttonClose.DialogResult = this.Result ? DialogResult.Yes : DialogResult.No;
    if (((ContainerControl) this).ParentForm != null)
      ((ContainerControl) this).ParentForm.FormClosing -= new FormClosingEventHandler(this.OnFormClosing);
  }

  private void SetCTP(Channel ctp)
  {
    if (this.channel != ctp)
    {
      this.ResetResults();
      if (this.Busy)
      {
        this.ResultMessage = Resources.Message_CTP01TDeviceChangedDuringProcess;
        this.AddLogLabel(Resources.Message_CTP01TDeviceChangedDuringProcess);
        this.Busy = false;
      }
      if (this.channel != null)
        this.channel.EcuInfos.EcuInfosReadCompleteEvent -= new EcuInfosReadCompleteEventHandler(this.EcuInfos_EcuInfosReadCompleteEvent);
      this.channel = ctp;
      if (this.channel != null)
        this.channel.EcuInfos.EcuInfosReadCompleteEvent += new EcuInfosReadCompleteEventHandler(this.EcuInfos_EcuInfosReadCompleteEvent);
    }
    this.UpdateUserInterface();
  }

  private bool Busy { get; set; }

  public bool Result { get; private set; }

  public string ResultMessage { get; private set; }

  private void ResetResults()
  {
    this.Result = false;
    this.ResultMessage = Resources.Message_None;
  }

  private UserPanel.PartStatus IsAffectedPartNumber
  {
    get
    {
      if (this.channel == null || this.channel.EcuInfos["CO_HardwarePartNumber"] == null || this.channel.EcuInfos["CO_HardwarePartNumber"].Value == null)
        return UserPanel.PartStatus.HardwareUnknown;
      if (this.channel.EcuInfos["CO_SoftwarePartNumber"] == null || this.channel.EcuInfos["CO_SoftwarePartNumber"].Value == null)
        return UserPanel.PartStatus.SoftwareUnknown;
      Part currentHardwarePart = new Part(this.channel.EcuInfos["CO_HardwarePartNumber"].Value);
      if (!this.AffectedHardwarePartNumbers.Where<string>((Func<string, bool>) (x => PartExtensions.IsEqual(currentHardwarePart, x))).Any<string>())
        return UserPanel.PartStatus.HardwareFail;
      Part currentSoftwarePart = new Part(this.channel.EcuInfos["CO_SoftwarePartNumber"].Value);
      return this.AffectedSoftwarePartNumbers.Where<string>((Func<string, bool>) (x => PartExtensions.IsEqual(currentSoftwarePart, x))).Any<string>() ? UserPanel.PartStatus.Pass : UserPanel.PartStatus.SoftwareFail;
    }
  }

  private UserPanel.GprsStatus CurrentGprsConfigCorrect
  {
    get
    {
      if (this.IsAffectedPartNumber != UserPanel.PartStatus.Pass)
        return UserPanel.GprsStatus.IgnoreGprs;
      if (this.channel == null || this.channel.EcuInfos["DT_STO_Gprs_Config_1_gprsConfig"] == null || this.channel.EcuInfos["DT_STO_Gprs_Config_1_gprsConfig"].Value == null)
        return UserPanel.GprsStatus.Unknown;
      return this.channel.EcuInfos["DT_STO_Gprs_Config_1_gprsConfig"].Value.Equals("1F8B080000000000000073767367656060B8C9758D914B994B3625B33839BF2CB5A852AFB4582F393F29512F2531333727B508C8C915B837935988D9D0DC408AD9D8D0408937B120CF502FA5242F112469A5CC2597985E9E999759A2575094A297925A52949F59929C9F97979A5C02D6FEC7CE499E4B0AA8089782BF761EAC010D4F3922181318B28430151511E3C22A547701000CC0CF68E1000000FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF", StringComparison.OrdinalIgnoreCase) ? UserPanel.GprsStatus.Pass : UserPanel.GprsStatus.Fail;
    }
  }

  private void AddLogLabel(string text)
  {
    this.LabelLog(this.seekTimeListView1.RequiredUserLabelPrefix, text);
  }

  private void UpdateUserInterface()
  {
    switch (this.IsAffectedPartNumber)
    {
      case UserPanel.PartStatus.Pass:
        this.checkmarkAffectedPartNumber.CheckState = CheckState.Checked;
        ((Control) this.labelAffectedPartNumber).Text = Resources.Message_ConnectedCTP01TIsAllowed;
        break;
      case UserPanel.PartStatus.HardwareFail:
        this.checkmarkAffectedPartNumber.CheckState = CheckState.Unchecked;
        this.ResultMessage = Resources.Message_ConnectedCTP01THardwareIsNotSupported;
        ((Control) this.labelAffectedPartNumber).Text = Resources.Message_ConnectedCTP01THardwareIsNotSupported;
        break;
      case UserPanel.PartStatus.SoftwareFail:
        this.checkmarkAffectedPartNumber.CheckState = CheckState.Unchecked;
        this.ResultMessage = Resources.Message_ConnectedCTP01TSoftwareIsNotSupported;
        ((Control) this.labelAffectedPartNumber).Text = Resources.Message_ConnectedCTP01TSoftwareIsNotSupported;
        break;
      case UserPanel.PartStatus.HardwareUnknown:
        this.checkmarkAffectedPartNumber.CheckState = CheckState.Indeterminate;
        this.ResultMessage = Resources.Message_CannotConfirmConnectedCTP01THardwareVersion;
        ((Control) this.labelAffectedPartNumber).Text = Resources.Message_CannotConfirmConnectedCTP01THardwareVersion;
        break;
      case UserPanel.PartStatus.SoftwareUnknown:
        this.checkmarkAffectedPartNumber.CheckState = CheckState.Indeterminate;
        this.ResultMessage = Resources.Message_CannotConfirmConnectedCTP01TSoftwareVersion;
        ((Control) this.labelAffectedPartNumber).Text = Resources.Message_CannotConfirmConnectedCTP01TSoftwareVersion;
        break;
    }
    switch (this.CurrentGprsConfigCorrect)
    {
      case UserPanel.GprsStatus.Pass:
        this.checkmarkGprsConfigCorrect.CheckState = CheckState.Checked;
        ((Control) this.labelGprsConfigCorrect).Text = Resources.Message_GPRSConfigurationIsCORRECT;
        break;
      case UserPanel.GprsStatus.Fail:
        this.checkmarkGprsConfigCorrect.CheckState = CheckState.Unchecked;
        ((Control) this.labelGprsConfigCorrect).Text = Resources.Message_GPRSConfigurationIsNOTCORRECT;
        break;
      case UserPanel.GprsStatus.Unknown:
        this.checkmarkGprsConfigCorrect.CheckState = CheckState.Indeterminate;
        ((Control) this.labelGprsConfigCorrect).Text = Resources.Message_GPRSConfigurationCanNotBeRead;
        break;
      case UserPanel.GprsStatus.IgnoreGprs:
        this.checkmarkGprsConfigCorrect.CheckState = CheckState.Indeterminate;
        ((Control) this.labelGprsConfigCorrect).Text = Resources.Message_GPRSNotApplicableToCurrentUnit;
        break;
    }
    this.buttonFixGprs.Enabled = this.channel != null && this.channel.Online && this.checkmarkAffectedPartNumber.CheckState == CheckState.Checked && this.checkmarkGprsConfigCorrect.CheckState == CheckState.Unchecked && !this.Busy;
  }

  private void SendCorrectGprsConfig()
  {
    Service service = this.channel.Services["DL_Gprs_Config_1"];
    if (service != (Service) null)
    {
      this.AddLogLabel(Resources.Message_WritingGPRSConfigurationValue);
      this.Busy = true;
      this.UpdateUserInterface();
      service.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.gprsWriteService_ServiceCompleteEvent);
      service.InputValues[0].Value = (object) "1F8B080000000000000073767367656060B8C9758D914B994B3625B33839BF2CB5A852AFB4582F393F29512F2531333727B508C8C915B837935988D9D0DC408AD9D8D0408937B120CF502FA5242F112469A5CC2597985E9E999759A2575094A297925A52949F59929C9F97979A5C02D6FEC7CE499E4B0AA8089782BF761EAC010D4F3922181318B28430151511E3C22A547701000CC0CF68E1000000FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF";
      service.Execute(false);
    }
    else
      this.AddLogLabel(Resources.Message_CouldNotFindGPRSWriteService);
  }

  private void gprsWriteService_ServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    Service service = sender as Service;
    if (service != (Service) null)
    {
      service.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.gprsWriteService_ServiceCompleteEvent);
      if (e.Succeeded)
      {
        this.AddLogLabel(Resources.Message_GPRSConfigurationWasSuccessfullyUpdated);
        this.EnableAntennaDiagnostics();
      }
      else
      {
        this.Result = false;
        this.ResultMessage = Resources.Message_FailedToUpdateGPRSConfiguration;
        this.AddLogLabel(Resources.Message_FailedToUpdateGPRSConfiguration);
        this.Busy = false;
      }
    }
    else
    {
      this.AddLogLabel(Resources.Message_UnknownErrorWhenExecutingGPRSWrite);
      this.Busy = false;
    }
    this.UpdateUserInterface();
  }

  private void EnableAntennaDiagnostics()
  {
    int num = 0;
    foreach (Parameter parameter in this.channel.Parameters.Where<Parameter>((Func<Parameter, bool>) (p => p.GroupQualifier.Equals("VCD_PID_Antenna_diagnostic", StringComparison.OrdinalIgnoreCase))))
    {
      ++num;
      parameter.Value = (object) parameter.Choices.GetItemFromRawValue((object) 1);
      parameter.Marked = true;
    }
    this.AddLogLabel(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Message_EnablingAntennaDiagnosticsFormat, (object) num));
    this.channel.Parameters.ParametersWriteCompleteEvent += new ParametersWriteCompleteEventHandler(this.Parameters_ParametersWriteCompleteEvent);
    this.channel.Parameters.Write(false);
  }

  private void Parameters_ParametersWriteCompleteEvent(object sender, ResultEventArgs e)
  {
    this.channel.Parameters.ParametersWriteCompleteEvent -= new ParametersWriteCompleteEventHandler(this.Parameters_ParametersWriteCompleteEvent);
    if (e.Succeeded)
    {
      this.Result = true;
      this.ResultMessage = Resources.Message_ProcessCompletedSuccessfully;
      this.AddLogLabel(Resources.Message_AntennaDiagnosticsHaveBeenEnabled);
      this.AddLogLabel(Resources.Message_ProcessCompletedSuccessfully);
    }
    else
    {
      this.Result = false;
      this.ResultMessage = Resources.Message_AntennaDiagnosticsEnableFailedToWrite;
      this.AddLogLabel(Resources.Message_AntennaDiagnosticsEnableFailedToWrite);
    }
    this.channel.EcuInfos.Read(false);
    this.Busy = false;
  }

  private void EcuInfos_EcuInfosReadCompleteEvent(object sender, ResultEventArgs e)
  {
    this.UpdateUserInterface();
  }

  private void buttonFixGprs_Click(object sender, EventArgs e)
  {
    this.ResetResults();
    this.SendCorrectGprsConfig();
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.seekTimeListView1 = new SeekTimeListView();
    this.checkmarkAffectedPartNumber = new Checkmark();
    this.checkmarkGprsConfigCorrect = new Checkmark();
    this.buttonFixGprs = new Button();
    this.labelAffectedPartNumber = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.labelGprsConfigCorrect = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.buttonClose = new Button();
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.seekTimeListView1, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.checkmarkAffectedPartNumber, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.checkmarkGprsConfigCorrect, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonFixGprs, 1, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.labelAffectedPartNumber, 1, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.labelGprsConfigCorrect, 1, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonClose, 2, 3);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.seekTimeListView1, 3);
    componentResourceManager.ApplyResources((object) this.seekTimeListView1, "seekTimeListView1");
    this.seekTimeListView1.FilterUserLabels = true;
    ((Control) this.seekTimeListView1).Name = "seekTimeListView1";
    this.seekTimeListView1.RequiredUserLabelPrefix = "GPRSConfiguration";
    this.seekTimeListView1.SelectedTime = new DateTime?();
    this.seekTimeListView1.ShowCommunicationsState = false;
    this.seekTimeListView1.ShowControlPanel = false;
    this.seekTimeListView1.ShowDeviceColumn = false;
    componentResourceManager.ApplyResources((object) this.checkmarkAffectedPartNumber, "checkmarkAffectedPartNumber");
    ((Control) this.checkmarkAffectedPartNumber).Name = "checkmarkAffectedPartNumber";
    componentResourceManager.ApplyResources((object) this.checkmarkGprsConfigCorrect, "checkmarkGprsConfigCorrect");
    ((Control) this.checkmarkGprsConfigCorrect).Name = "checkmarkGprsConfigCorrect";
    componentResourceManager.ApplyResources((object) this.buttonFixGprs, "buttonFixGprs");
    this.buttonFixGprs.Name = "buttonFixGprs";
    this.buttonFixGprs.UseCompatibleTextRendering = true;
    this.buttonFixGprs.UseVisualStyleBackColor = true;
    this.buttonFixGprs.Click += new EventHandler(this.buttonFixGprs_Click);
    this.labelAffectedPartNumber.Alignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.labelAffectedPartNumber, 2);
    componentResourceManager.ApplyResources((object) this.labelAffectedPartNumber, "labelAffectedPartNumber");
    ((Control) this.labelAffectedPartNumber).Name = "labelAffectedPartNumber";
    this.labelAffectedPartNumber.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.labelGprsConfigCorrect.Alignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.labelGprsConfigCorrect, 2);
    componentResourceManager.ApplyResources((object) this.labelGprsConfigCorrect, "labelGprsConfigCorrect");
    ((Control) this.labelGprsConfigCorrect).Name = "labelGprsConfigCorrect";
    this.labelGprsConfigCorrect.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.buttonClose.DialogResult = DialogResult.OK;
    componentResourceManager.ApplyResources((object) this.buttonClose, "buttonClose");
    this.buttonClose.Name = "buttonClose";
    this.buttonClose.UseCompatibleTextRendering = true;
    this.buttonClose.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this, "$this");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel1);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }

  private enum PartStatus
  {
    Pass,
    HardwareFail,
    SoftwareFail,
    HardwareUnknown,
    SoftwareUnknown,
  }

  private enum GprsStatus
  {
    Pass,
    Fail,
    Unknown,
    IgnoreGprs,
  }
}
