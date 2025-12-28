// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Set_Odometers__NGC_.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Extensions.FakeInstruments;
using DetroitDiesel.UnitConversion;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Set_Odometers__NGC_.panel;

public class UserPanel : CustomPanel
{
  private const string Icuc01tName = "ICUC01T";
  private const string Icc501tName = "ICC501T";
  private const string CPC302TName = "CPC302T";
  private const string CPC501TName = "CPC501T";
  private const string J1939CpcName = "J1939-0";
  private const string VersionQualifier = "CO_SoftwareVersion";
  private const string ICUCOdometerQualifier = "paramStoredOdometer";
  private const string Icc501tOdometerQualifier = "ET_ODO_Odometer";
  private const string SetCpcOdometerRoutine = "DL_ID_Odometer";
  private const string J1939CpcOdometerQualifier = "DT_917";
  private Channel instrumentCluster;
  private Parameter instrumentClusterOdometerParameter;
  private Conversion instrumentClusterValueToDisplay = (Conversion) null;
  private Conversion instrumentClusterDisplayToValue = (Conversion) null;
  private double instrumentClusterToSupportedConversionFactor = 1.0;
  private double instrumentClusterFromSupportedConversionFactor = 1.0;
  private string instrumentClusterSupportedConversionUnit = "km";
  private Channel cpc;
  private Conversion cpcValueToDisplay = (Conversion) null;
  private Conversion cpcDisplayToValue = (Conversion) null;
  private Service cpcOdometerService = (Service) null;
  private Channel j1939Cpc;
  private Instrument j1939CpcOdometerInstrument;
  private Conversion j1939CpcValueToDisplay = (Conversion) null;
  private Conversion j1939CpcDisplayToValue = (Conversion) null;
  private Conversion kmToDisplay;
  private Tuple<string, bool>[] ProhibitedVersions = new Tuple<string, bool>[2]
  {
    Tuple.Create<string, bool>("27.03.01", true),
    Tuple.Create<string, bool>("26.83.54", true)
  };
  private RuntimeFakeInstrument fakeIcOdometer;
  private string proposedNewValue = "";
  private UserPanel.OdometerSetState odometerSetState = UserPanel.OdometerSetState.NotSet;
  private UserPanel.Step currentStep = UserPanel.Step.Ready;
  private UserPanel.Operation currentOperation = UserPanel.Operation.SetBoth;
  private string errorString;
  private double previousOdometerValue = -1.0;
  private TableLayoutPanel tableLayoutPanelMain;
  private Checkmark checkmark1;
  private Button buttonSet;
  private Button buttonClose;
  private TextBox textBoxCustomValue;
  private TableLayoutPanel tableLayoutPanelStatus;
  private TableLayoutPanel tableLayoutPanelOdometerDisplay;
  private TableLayoutPanel tableLayoutPanel4;
  private DigitalReadoutInstrument digitalReadoutInstrumentCpcOdometer;
  private SeekTimeListView seekTimeListView;
  private Button buttonSyncToCpc;
  private TableLayoutPanel tableLayoutPanel5;
  private Button buttonSyncToInstrumentCluster;
  private System.Windows.Forms.Label labelCustomValue;
  private DigitalReadoutInstrument digitalReadoutInstrumentIcucOdometer;
  private System.Windows.Forms.Label labelCondition;

  private void AppendDisplayMessage(string txt)
  {
    this.LabelLog(this.seekTimeListView.RequiredUserLabelPrefix, txt);
    this.AddStatusMessage(txt);
  }

  public UserPanel()
  {
    this.CreateFakeIcOdometer(Converter.GlobalInstance.SelectedUnitSystem == 1 ? "miles" : "km");
    this.kmToDisplay = Converter.GlobalInstance.GetConversion("km", Converter.GlobalInstance.SelectedUnitSystem == 1 ? "miles" : "km");
    this.InitializeComponent();
  }

  protected virtual void OnLoad(EventArgs e)
  {
    ((ContainerControl) this).ParentForm.FormClosing += new FormClosingEventHandler(this.OnFormClosing);
    this.textBoxCustomValue.TextChanged += new EventHandler(this.textBoxCustomValueTextChanged);
    this.textBoxCustomValue.KeyPress += new KeyPressEventHandler(this.textBoxCustomValueKeyPress);
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
    this.UpdateChannels();
    if (this.CheckProhibited())
      ((ContainerControl) this).ParentForm.Close();
    this.UpdateUserInterface();
  }

  private void OnFormClosing(object sender, FormClosingEventArgs e)
  {
    if (!this.CanClose)
      e.Cancel = true;
    if (e.Cancel)
      return;
    this.DisposeFakeIcOdometer();
    if (this.instrumentCluster != null)
    {
      this.instrumentCluster.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
      if (this.instrumentClusterOdometerParameter != null)
      {
        this.instrumentClusterOdometerParameter.ParameterUpdateEvent -= new ParameterUpdateEventHandler(this.OdometerParameterUpdateEvent);
        this.instrumentClusterOdometerParameter = (Parameter) null;
      }
    }
    if (this.cpc != null)
      this.cpc.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
    if (((ContainerControl) this).ParentForm != null)
      ((ContainerControl) this).ParentForm.FormClosing -= new FormClosingEventHandler(this.OnFormClosing);
  }

  private bool CheckProhibited()
  {
    if (this.cpc != null && this.cpc.EcuInfos["CO_SoftwareVersion"] != null && this.cpc.Ecu.Name.Equals("CPC302T", StringComparison.OrdinalIgnoreCase))
    {
      string strA = this.cpc.EcuInfos["CO_SoftwareVersion"].Value;
      foreach (Tuple<string, bool> prohibitedVersion in this.ProhibitedVersions)
      {
        if (string.Compare(strA, prohibitedVersion.Item1, StringComparison.OrdinalIgnoreCase) == 0)
        {
          if (prohibitedVersion.Item2)
          {
            int num1 = (int) MessageBox.Show(string.Format(Resources.MessageFormat_UnsupportedSoftwareVersion, (object) this.cpc.Ecu.Name), Resources.Message_UnsupportedSoftwareVersionTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
          }
          else
          {
            int num2 = (int) MessageBox.Show(string.Format(Resources.MessageFormat_UnsupportedSoftwareVersionNoRepair, (object) this.cpc.Ecu.Name), Resources.Message_UnsupportedSoftwareVersionTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
          }
          return true;
        }
      }
    }
    return false;
  }

  private void GoMachine()
  {
    switch (this.currentStep)
    {
      case UserPanel.Step.Warning:
        this.currentStep = this.cpc == null || !(this.cpcOdometerService != (Service) null) || this.currentOperation == UserPanel.Operation.SyncToCpc ? UserPanel.Step.SetInstrumentCluster : (DialogResult.OK != MessageBox.Show(Resources.Message_DDECReportsLifeToDateDataWillBeResetClickOkToContinueOrClickCancelToAbortChangesAndExit, Application.ProductName, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) ? UserPanel.Step.Done : UserPanel.Step.SetCpc);
        this.GoMachine();
        break;
      case UserPanel.Step.SetCpc:
        ++this.currentStep;
        if (this.currentOperation != UserPanel.Operation.SyncToCpc)
        {
          this.AppendDisplayMessage(Resources.Message_SettingCPC);
          this.SetCpcOdometerEcuInfoValue();
          break;
        }
        this.GoMachine();
        break;
      case UserPanel.Step.SetInstrumentCluster:
        ++this.currentStep;
        if (this.instrumentCluster != null && this.currentOperation != UserPanel.Operation.SyncToInstrumentCluster)
        {
          this.AppendDisplayMessage(Resources.Message_SettingInstrumentCluster);
          this.SetInstrumentClusterOdometerParameter();
          break;
        }
        this.GoMachine();
        break;
      case UserPanel.Step.Done:
        this.AppendDisplayMessage(Resources.Message_Done);
        this.odometerSetState = UserPanel.OdometerSetState.Set;
        break;
      case UserPanel.Step.Error:
        this.odometerSetState = UserPanel.OdometerSetState.Error;
        this.AppendDisplayMessage(Resources.Message_Error);
        this.AppendDisplayMessage(this.errorString);
        break;
    }
    this.UpdateUserInterface();
  }

  private void SetCpcOdometerEcuInfoValue()
  {
    if (this.cpcOdometerService != (Service) null)
    {
      this.odometerSetState = UserPanel.OdometerSetState.Working;
      if (this.cpcDisplayToValue != null)
        this.cpcOdometerService.InputValues[0].Value = (object) this.cpcDisplayToValue.Convert((object) this.proposedNewValue);
      else
        this.cpcOdometerService.InputValues[0].Value = (object) this.proposedNewValue;
      this.cpcOdometerService.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.SetCpcOdometerServiceCompleteEvent);
      this.cpcOdometerService.Execute(false);
    }
    else
    {
      this.currentStep = UserPanel.Step.Error;
      this.errorString = Resources.Message_CPCServiceNA;
      this.GoMachine();
    }
  }

  private void SetCpcOdometerServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    (sender as Service).ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.SetCpcOdometerServiceCompleteEvent);
    if (!e.Succeeded)
    {
      this.currentStep = UserPanel.Step.Error;
      this.errorString = e.Exception.Message;
    }
    this.GoMachine();
  }

  private void SetInstrumentClusterOdometerParameter()
  {
    if (this.instrumentClusterOdometerParameter == null)
      return;
    this.odometerSetState = UserPanel.OdometerSetState.Working;
    this.instrumentClusterOdometerParameter.Value = (object) (this.instrumentClusterDisplayToValue.Convert((object) this.proposedNewValue) * this.instrumentClusterFromSupportedConversionFactor);
    this.instrumentCluster.Parameters.ParametersWriteCompleteEvent += new ParametersWriteCompleteEventHandler(this.InstrumentClusterOdometerWriteCompleteEvent);
    this.UpdateUserInterface();
    this.instrumentCluster.Parameters.Write(false);
  }

  private void InstrumentClusterOdometerWriteCompleteEvent(object sender, ResultEventArgs e)
  {
    if (this.instrumentCluster != null)
      this.instrumentCluster.Parameters.ParametersWriteCompleteEvent -= new ParametersWriteCompleteEventHandler(this.InstrumentClusterOdometerWriteCompleteEvent);
    if (!e.Succeeded)
    {
      this.odometerSetState = UserPanel.OdometerSetState.Error;
      this.errorString = e.Exception.Message;
    }
    this.GoMachine();
  }

  public virtual void OnChannelsChanged() => this.UpdateChannels();

  private string DetermineInputUnits()
  {
    return this.j1939CpcValueToDisplay != null ? this.j1939CpcValueToDisplay.OutputUnit : (this.instrumentClusterValueToDisplay != null ? this.instrumentClusterValueToDisplay.OutputUnit : "km");
  }

  private void UpdateChannels()
  {
    string inputUnits = this.DetermineInputUnits();
    Channel channel1 = this.GetChannel("CPC302T", (CustomPanel.ChannelLookupOptions) 5) ?? this.GetChannel("CPC501T", (CustomPanel.ChannelLookupOptions) 5);
    if (this.cpc != channel1)
    {
      if (this.cpc != null)
      {
        this.cpc.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
        this.cpcDisplayToValue = (Conversion) null;
      }
      this.cpc = channel1;
      if (this.cpc != null)
      {
        this.cpc.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
        this.cpcOdometerService = channel1.Services["DL_ID_Odometer"];
        if (this.cpcOdometerService != (Service) null && this.cpcOdometerService.InputValues[0] != null)
        {
          this.cpcValueToDisplay = Converter.GlobalInstance.GetConversion(this.cpcOdometerService.InputValues[0].Units.ToString(), Converter.GlobalInstance.SelectedUnitSystem);
          this.cpcDisplayToValue = this.cpcValueToDisplay != null ? Converter.GlobalInstance.GetConversion(this.cpcValueToDisplay.OutputUnit, this.cpcOdometerService.InputValues[0].Units.ToString()) : (Conversion) null;
          this.PopulatetextBoxCustomValueText(this.LargestOdometer());
        }
      }
    }
    Channel channel2 = this.GetChannel("J1939-0");
    if (this.j1939Cpc != channel2)
    {
      if (this.j1939Cpc != null)
        this.j1939Cpc.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
      this.j1939Cpc = channel2;
      if (this.j1939Cpc != null)
      {
        this.j1939CpcOdometerInstrument = channel2.Instruments["DT_917"];
        if (this.j1939CpcOdometerInstrument != (Instrument) null)
        {
          this.j1939CpcValueToDisplay = Converter.GlobalInstance.GetConversion(this.j1939CpcOdometerInstrument.Units.ToString(), Converter.GlobalInstance.SelectedUnitSystem);
          this.j1939CpcDisplayToValue = Converter.GlobalInstance.GetConversion(this.j1939CpcValueToDisplay.OutputUnit, this.j1939CpcOdometerInstrument.Units.ToString());
        }
        inputUnits = this.DetermineInputUnits();
        this.PopulatetextBoxCustomValueText(this.LargestOdometer());
      }
    }
    Channel channel3 = this.GetChannel("ICUC01T", (CustomPanel.ChannelLookupOptions) 5);
    if (this.instrumentCluster != channel3)
    {
      if (this.instrumentCluster != null)
      {
        this.instrumentCluster.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
        if (this.instrumentClusterOdometerParameter != null)
        {
          this.instrumentClusterOdometerParameter.ParameterUpdateEvent -= new ParameterUpdateEventHandler(this.OdometerParameterUpdateEvent);
          this.instrumentClusterOdometerParameter = (Parameter) null;
        }
      }
      this.instrumentCluster = channel3;
      if (channel3 != null && channel3.CommunicationsState == CommunicationsState.Online && (channel3.Ecu.Name.Equals("ICUC01T", StringComparison.OrdinalIgnoreCase) || channel3.Ecu.Name.Equals("ICC501T", StringComparison.OrdinalIgnoreCase)))
      {
        this.instrumentCluster.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
        this.instrumentClusterOdometerParameter = this.instrumentCluster.Parameters[channel3.Ecu.Name.Equals("ICC501T", StringComparison.OrdinalIgnoreCase) ? "ET_ODO_Odometer" : "paramStoredOdometer"];
        if (this.instrumentClusterOdometerParameter != null)
        {
          if (this.instrumentClusterOdometerParameter.Units == "m")
          {
            this.instrumentClusterToSupportedConversionFactor = 0.001;
            this.instrumentClusterFromSupportedConversionFactor = 1000.0;
            this.instrumentClusterSupportedConversionUnit = "km";
          }
          else
          {
            this.instrumentClusterToSupportedConversionFactor = 1.0;
            this.instrumentClusterFromSupportedConversionFactor = 1.0;
            this.instrumentClusterSupportedConversionUnit = this.instrumentClusterOdometerParameter.Units;
          }
          this.instrumentCluster.Parameters.ReadGroup(this.instrumentClusterOdometerParameter.GroupQualifier, false, true);
          this.instrumentClusterValueToDisplay = Converter.GlobalInstance.GetConversion(this.instrumentClusterSupportedConversionUnit, Converter.GlobalInstance.SelectedUnitSystem);
          this.instrumentClusterDisplayToValue = Converter.GlobalInstance.GetConversion(this.instrumentClusterValueToDisplay.OutputUnit, this.instrumentClusterSupportedConversionUnit);
          this.instrumentClusterOdometerParameter.ParameterUpdateEvent += new ParameterUpdateEventHandler(this.OdometerParameterUpdateEvent);
          this.SetFakeIcOdometer(this.InstrumentClusterOdometerValue);
          inputUnits = this.DetermineInputUnits();
          this.PopulatetextBoxCustomValueText(this.LargestOdometer());
        }
      }
      this.UpdateUserInterface();
    }
    this.labelCustomValue.Text = $"Custom Value({inputUnits}):";
    if (this.cpc != null || this.instrumentCluster != null)
      return;
    ((ContainerControl) this).ParentForm.Close();
  }

  private void CreateFakeIcOdometer(string units)
  {
    if (this.fakeIcOdometer != null)
      return;
    this.fakeIcOdometer = RuntimeFakeInstrument.Create("fakeIcOdometer", Resources.Message_InstrumentClusterOdometer, units);
  }

  private void SetFakeIcOdometer(double? v)
  {
    if (this.fakeIcOdometer == null)
      return;
    this.fakeIcOdometer.SetValue(v.HasValue ? (object) string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0:0.00}", (object) this.InstrumentClusterOdometerValue) : (object) (string) null);
  }

  private void DisposeFakeIcOdometer()
  {
    if (this.fakeIcOdometer == null)
      return;
    ((FakeInstrument) this.fakeIcOdometer).Dispose();
  }

  private double? J1939CpcOdometerValue
  {
    get
    {
      if (!(this.j1939CpcOdometerInstrument != (Instrument) null) || this.j1939CpcOdometerInstrument.InstrumentValues.Current == null)
        return new double?();
      double? nullable = new double?(double.Parse(this.j1939CpcOdometerInstrument.InstrumentValues.Current.Value.ToString(), (IFormatProvider) CultureInfo.CurrentCulture));
      return !nullable.HasValue ? new double?() : new double?(this.j1939CpcValueToDisplay.Convert((object) nullable));
    }
  }

  private double? CpcOdometerValue => this.CpcOnline ? this.J1939CpcOdometerValue : new double?();

  private double? InstrumentClusterOdometerValue
  {
    get
    {
      if (!this.InstrumentClusterOnline || this.instrumentClusterOdometerParameter == null || this.instrumentClusterOdometerParameter.Value == null)
        return new double?();
      double? nullable1 = new double?(double.Parse(this.instrumentClusterOdometerParameter.Value.ToString(), (IFormatProvider) CultureInfo.CurrentCulture));
      double? clusterOdometerValue;
      if (nullable1.HasValue)
      {
        Conversion clusterValueToDisplay = this.instrumentClusterValueToDisplay;
        double? nullable2 = nullable1;
        double conversionFactor = this.instrumentClusterToSupportedConversionFactor;
        __Boxed<double?> local = (ValueType) (nullable2.HasValue ? new double?(nullable2.GetValueOrDefault() * conversionFactor) : new double?());
        clusterOdometerValue = new double?(clusterValueToDisplay.Convert((object) local));
      }
      else
        clusterOdometerValue = new double?();
      return clusterOdometerValue;
    }
  }

  private double LargestOdometer()
  {
    double? nullable;
    double num1;
    if (this.CpcOdometerValue.HasValue)
    {
      nullable = this.CpcOdometerValue;
      num1 = nullable.Value;
    }
    else
      num1 = 0.0;
    double num2 = num1;
    nullable = this.InstrumentClusterOdometerValue;
    double num3;
    if (nullable.HasValue)
    {
      nullable = this.InstrumentClusterOdometerValue;
      num3 = nullable.Value;
    }
    else
      num3 = 0.0;
    double num4 = num3;
    return num4 > num2 ? num4 : num2;
  }

  private void OdometerParameterUpdateEvent(object sender, ResultEventArgs e)
  {
    this.SetFakeIcOdometer(this.InstrumentClusterOdometerValue);
    if (!this.InstrumentClusterOdometerValue.HasValue || !this.OdometerValuesAreEqual(new double?(this.previousOdometerValue), new double?(this.InstrumentClusterOdometerValue.Value)) && this.odometerSetState != UserPanel.OdometerSetState.Working)
    {
      double? clusterOdometerValue = this.InstrumentClusterOdometerValue;
      double num;
      if (!clusterOdometerValue.HasValue)
      {
        num = -1.0;
      }
      else
      {
        clusterOdometerValue = this.InstrumentClusterOdometerValue;
        num = clusterOdometerValue.Value;
      }
      this.previousOdometerValue = num;
      this.odometerSetState = UserPanel.OdometerSetState.NotSet;
    }
    this.UpdateUserInterface();
  }

  private void buttonSet_Click(object sender, EventArgs e)
  {
    this.proposedNewValue = this.textBoxCustomValue.Text;
    this.currentOperation = UserPanel.Operation.SetBoth;
    this.currentStep = UserPanel.Step.Warning;
    this.GoMachine();
  }

  private void OnCommunicationsStateUpdate(object sender, CommunicationsStateEventArgs e)
  {
    this.UpdateUserInterface();
  }

  private bool CpcOnline
  {
    get => this.cpc != null && this.cpc.Online && this.j1939Cpc != null && this.j1939Cpc.Online;
  }

  private bool InstrumentClusterOnline
  {
    get => this.instrumentCluster != null && this.instrumentCluster.Online;
  }

  private bool CpcEcuIsBusy
  {
    get
    {
      return this.CpcOnline && this.cpc.CommunicationsState != CommunicationsState.Online && this.j1939Cpc.CommunicationsState != CommunicationsState.Online;
    }
  }

  private bool IcucEcuIsBusy
  {
    get
    {
      return this.InstrumentClusterOnline && this.instrumentCluster.CommunicationsState != CommunicationsState.Online;
    }
  }

  private bool CanClose
  {
    get
    {
      return this.odometerSetState != UserPanel.OdometerSetState.Working || this.cpc == null && this.instrumentCluster == null;
    }
  }

  private bool OdometerValuesAreEqual(double? v1, double? v2)
  {
    if (v1.HasValue && v2.HasValue)
      return Math.Abs(v1.Value - v2.Value) < 0.25;
    double? nullable1 = v1;
    double? nullable2 = v2;
    return nullable1.GetValueOrDefault() == nullable2.GetValueOrDefault() && nullable1.HasValue == nullable2.HasValue;
  }

  private void UpdateUserInterface()
  {
    ((Control) this.tableLayoutPanelOdometerDisplay).SuspendLayout();
    ((TableLayoutPanel) this.tableLayoutPanelOdometerDisplay).ColumnStyles[2].Width = this.instrumentCluster == null ? 0.0f : 50f;
    ((TableLayoutPanel) this.tableLayoutPanelOdometerDisplay).ColumnStyles[0].Width = this.cpc != null || this.instrumentCluster == null ? 50f : 0.0f;
    ((TableLayoutPanel) this.tableLayoutPanelOdometerDisplay).ColumnStyles[1].Width = this.cpc == null || this.instrumentCluster == null ? 0.0f : 4f;
    ((Control) this.tableLayoutPanelOdometerDisplay).ResumeLayout();
    double? v;
    int num1;
    if (this.odometerSetState == UserPanel.OdometerSetState.Set || this.odometerSetState == UserPanel.OdometerSetState.Error || this.odometerSetState == UserPanel.OdometerSetState.NotSet)
    {
      v = this.InstrumentClusterOdometerValue;
      if (v.HasValue)
      {
        v = this.J1939CpcOdometerValue;
        if (v.HasValue && this.cpc != null)
        {
          num1 = this.OdometerValuesAreEqual(this.CpcOdometerValue, this.InstrumentClusterOdometerValue) ? 1 : 0;
          goto label_5;
        }
      }
    }
    num1 = 1;
label_5:
    if (num1 == 0)
    {
      v = this.InstrumentClusterOdometerValue;
      double? cpcOdometerValue = this.CpcOdometerValue;
      if ((v.GetValueOrDefault() <= cpcOdometerValue.GetValueOrDefault() ? 0 : (v.HasValue & cpcOdometerValue.HasValue ? 1 : 0)) != 0)
      {
        this.buttonSyncToInstrumentCluster.Visible = true;
        this.buttonSyncToCpc.Visible = false;
      }
      else
      {
        this.buttonSyncToInstrumentCluster.Visible = false;
        this.buttonSyncToCpc.Visible = true;
      }
    }
    else
    {
      this.buttonSyncToInstrumentCluster.Visible = false;
      this.buttonSyncToCpc.Visible = false;
    }
    switch (this.odometerSetState)
    {
      case UserPanel.OdometerSetState.NotSet:
        double? nullable;
        if (!string.IsNullOrEmpty(this.textBoxCustomValue.Text))
        {
          nullable = new double?(double.Parse(this.textBoxCustomValue.Text, (IFormatProvider) CultureInfo.CurrentCulture));
        }
        else
        {
          v = new double?();
          nullable = v;
        }
        double? v1 = nullable;
        v = this.InstrumentClusterOdometerValue;
        int num2;
        if (!v.HasValue)
        {
          v = this.J1939CpcOdometerValue;
          num2 = v.HasValue ? 1 : 0;
        }
        else
          num2 = 1;
        if (num2 == 0)
        {
          v = new double?();
          this.SetFakeIcOdometer(v);
          this.buttonSet.Enabled = false;
          this.checkmark1.Checked = false;
          this.labelCondition.Text = Resources.Message_TheOdometerValueIsUnknownAndCannotBeSet;
          break;
        }
        if (this.OdometerValuesAreEqual(v1, new double?(this.LargestOdometer())))
        {
          this.buttonSet.Enabled = false;
          this.checkmark1.Checked = false;
          this.labelCondition.Text = Resources.Message_TheOdometerCannotBeSetBecauseTheProposedValueIsEffectivelyEqualToCurrentOdometer;
          this.labelCondition.BackColor = SystemColors.Control;
          break;
        }
        v = v1;
        double num3 = this.LargestOdometer();
        if ((v.GetValueOrDefault() <= num3 ? 0 : (v.HasValue ? 1 : 0)) != 0)
        {
          this.buttonSet.Enabled = true;
          this.checkmark1.Checked = true;
          this.labelCondition.Text = Resources.Message_TheOdometerIsReadyToBeSetBecauseTheProposedValueIsGreaterThanTheCurrentOdometer;
          this.labelCondition.BackColor = SystemColors.Control;
          break;
        }
        this.buttonSet.Enabled = false;
        this.checkmark1.Checked = false;
        this.labelCondition.Text = Resources.Message_ErrorTheOdometerCannotBeSetBecauseTheProposedValueIsLessThanTheCurrentOdometer;
        this.labelCondition.BackColor = Color.Red;
        break;
      case UserPanel.OdometerSetState.Set:
        this.buttonSet.Enabled = false;
        this.checkmark1.Checked = !this.CpcEcuIsBusy || !this.IcucEcuIsBusy;
        this.labelCondition.Text = this.CpcEcuIsBusy || this.IcucEcuIsBusy ? Resources.Message_WritingOdometerValue : Resources.Message_TheOdometerHasBeenSetTheValueShownMayBeSlightlyDifferentThanTheOneEnteredDueToRoundingIssues;
        this.labelCondition.BackColor = SystemColors.Control;
        break;
      case UserPanel.OdometerSetState.Working:
        this.checkmark1.Checked = this.buttonSet.Enabled = this.buttonSyncToCpc.Enabled = this.buttonSyncToInstrumentCluster.Enabled = false;
        this.labelCondition.Text = Resources.Message_WritingOdometerValue;
        this.labelCondition.BackColor = SystemColors.Control;
        break;
      case UserPanel.OdometerSetState.Error:
        this.buttonSet.Enabled = false;
        this.checkmark1.Checked = false;
        this.labelCondition.Text = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_AnErrorOccurredSettingTheOdometer0, (object) this.errorString);
        break;
      default:
        throw new InvalidOperationException();
    }
  }

  private void PopulatetextBoxCustomValueText(double value)
  {
    this.textBoxCustomValue.Text = string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0:0.00}", (object) value);
  }

  private void textBoxCustomValueTextChanged(object sender, EventArgs e)
  {
    this.odometerSetState = UserPanel.OdometerSetState.NotSet;
    this.UpdateUserInterface();
  }

  private void textBoxCustomValueKeyPress(object sender, KeyPressEventArgs e)
  {
    e.Handled = !char.IsDigit(e.KeyChar) && (this.textBoxCustomValue.Text.Length <= 0 || e.KeyChar != '.') && !char.IsControl(e.KeyChar);
  }

  private void buttonSyncToCpc_Click(object sender, EventArgs e)
  {
    this.proposedNewValue = this.J1939CpcOdometerValue.ToString();
    this.currentOperation = UserPanel.Operation.SyncToCpc;
    this.currentStep = UserPanel.Step.SetInstrumentCluster;
    this.GoMachine();
  }

  private void buttonSyncToInstrumentCluster_Click(object sender, EventArgs e)
  {
    this.proposedNewValue = this.InstrumentClusterOdometerValue.ToString();
    this.currentOperation = UserPanel.Operation.SyncToInstrumentCluster;
    this.currentStep = UserPanel.Step.Warning;
    this.GoMachine();
  }

  private void digitalReadoutInstrumentCpcOdometer_DataChanged(object sender, EventArgs e)
  {
    this.UpdateUserInterface();
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanelMain = new TableLayoutPanel();
    this.tableLayoutPanelStatus = new TableLayoutPanel();
    this.labelCondition = new System.Windows.Forms.Label();
    this.checkmark1 = new Checkmark();
    this.textBoxCustomValue = new TextBox();
    this.buttonSet = new Button();
    this.buttonClose = new Button();
    this.labelCustomValue = new System.Windows.Forms.Label();
    this.tableLayoutPanelOdometerDisplay = new TableLayoutPanel();
    this.tableLayoutPanel5 = new TableLayoutPanel();
    this.buttonSyncToInstrumentCluster = new Button();
    this.digitalReadoutInstrumentIcucOdometer = new DigitalReadoutInstrument();
    this.tableLayoutPanel4 = new TableLayoutPanel();
    this.digitalReadoutInstrumentCpcOdometer = new DigitalReadoutInstrument();
    this.buttonSyncToCpc = new Button();
    this.seekTimeListView = new SeekTimeListView();
    ((Control) this.tableLayoutPanelMain).SuspendLayout();
    ((Control) this.tableLayoutPanelStatus).SuspendLayout();
    ((Control) this.tableLayoutPanelOdometerDisplay).SuspendLayout();
    ((Control) this.tableLayoutPanel5).SuspendLayout();
    ((Control) this.tableLayoutPanel4).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelMain, "tableLayoutPanelMain");
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.tableLayoutPanelStatus, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.tableLayoutPanelOdometerDisplay, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelMain).Controls.Add((Control) this.seekTimeListView, 0, 1);
    ((Control) this.tableLayoutPanelMain).Name = "tableLayoutPanelMain";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelStatus, "tableLayoutPanelStatus");
    ((TableLayoutPanel) this.tableLayoutPanelStatus).Controls.Add((Control) this.labelCondition, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanelStatus).Controls.Add((Control) this.checkmark1, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelStatus).Controls.Add((Control) this.textBoxCustomValue, 2, 1);
    ((TableLayoutPanel) this.tableLayoutPanelStatus).Controls.Add((Control) this.buttonSet, 3, 1);
    ((TableLayoutPanel) this.tableLayoutPanelStatus).Controls.Add((Control) this.buttonClose, 5, 1);
    ((TableLayoutPanel) this.tableLayoutPanelStatus).Controls.Add((Control) this.labelCustomValue, 0, 1);
    ((Control) this.tableLayoutPanelStatus).Name = "tableLayoutPanelStatus";
    ((TableLayoutPanel) this.tableLayoutPanelStatus).SetColumnSpan((Control) this.labelCondition, 5);
    componentResourceManager.ApplyResources((object) this.labelCondition, "labelCondition");
    this.labelCondition.Name = "labelCondition";
    this.labelCondition.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.checkmark1, "checkmark1");
    ((Control) this.checkmark1).Name = "checkmark1";
    componentResourceManager.ApplyResources((object) this.textBoxCustomValue, "textBoxCustomValue");
    this.textBoxCustomValue.Name = "textBoxCustomValue";
    componentResourceManager.ApplyResources((object) this.buttonSet, "buttonSet");
    this.buttonSet.Name = "buttonSet";
    this.buttonSet.UseCompatibleTextRendering = true;
    this.buttonSet.UseVisualStyleBackColor = true;
    this.buttonSet.Click += new EventHandler(this.buttonSet_Click);
    componentResourceManager.ApplyResources((object) this.buttonClose, "buttonClose");
    this.buttonClose.DialogResult = DialogResult.OK;
    this.buttonClose.Name = "buttonClose";
    this.buttonClose.UseCompatibleTextRendering = true;
    this.buttonClose.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.labelCustomValue, "labelCustomValue");
    ((TableLayoutPanel) this.tableLayoutPanelStatus).SetColumnSpan((Control) this.labelCustomValue, 2);
    this.labelCustomValue.Name = "labelCustomValue";
    this.labelCustomValue.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelOdometerDisplay, "tableLayoutPanelOdometerDisplay");
    ((TableLayoutPanel) this.tableLayoutPanelOdometerDisplay).Controls.Add((Control) this.tableLayoutPanel5, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanelOdometerDisplay).Controls.Add((Control) this.tableLayoutPanel4, 0, 0);
    ((Control) this.tableLayoutPanelOdometerDisplay).Name = "tableLayoutPanelOdometerDisplay";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel5, "tableLayoutPanel5");
    ((TableLayoutPanel) this.tableLayoutPanel5).Controls.Add((Control) this.buttonSyncToInstrumentCluster, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel5).Controls.Add((Control) this.digitalReadoutInstrumentIcucOdometer, 0, 0);
    ((Control) this.tableLayoutPanel5).Name = "tableLayoutPanel5";
    componentResourceManager.ApplyResources((object) this.buttonSyncToInstrumentCluster, "buttonSyncToInstrumentCluster");
    this.buttonSyncToInstrumentCluster.Name = "buttonSyncToInstrumentCluster";
    this.buttonSyncToInstrumentCluster.UseCompatibleTextRendering = true;
    this.buttonSyncToInstrumentCluster.UseVisualStyleBackColor = true;
    this.buttonSyncToInstrumentCluster.Click += new EventHandler(this.buttonSyncToInstrumentCluster_Click);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentIcucOdometer, "digitalReadoutInstrumentIcucOdometer");
    this.digitalReadoutInstrumentIcucOdometer.FontGroup = "SetOdometers";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentIcucOdometer).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentIcucOdometer).Instrument = new Qualifier((QualifierTypes) 16 /*0x10*/, "fake", "fakeIcOdometer");
    ((Control) this.digitalReadoutInstrumentIcucOdometer).Name = "digitalReadoutInstrumentIcucOdometer";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentIcucOdometer).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel4, "tableLayoutPanel4");
    ((TableLayoutPanel) this.tableLayoutPanel4).Controls.Add((Control) this.digitalReadoutInstrumentCpcOdometer, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel4).Controls.Add((Control) this.buttonSyncToCpc, 0, 1);
    ((Control) this.tableLayoutPanel4).Name = "tableLayoutPanel4";
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentCpcOdometer, "digitalReadoutInstrumentCpcOdometer");
    this.digitalReadoutInstrumentCpcOdometer.FontGroup = "SetOdometers";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentCpcOdometer).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentCpcOdometer).Instrument = new Qualifier((QualifierTypes) 1, "J1939-0", "DT_917");
    ((Control) this.digitalReadoutInstrumentCpcOdometer).Name = "digitalReadoutInstrumentCpcOdometer";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentCpcOdometer).UnitAlignment = StringAlignment.Near;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentCpcOdometer).DataChanged += new EventHandler(this.digitalReadoutInstrumentCpcOdometer_DataChanged);
    componentResourceManager.ApplyResources((object) this.buttonSyncToCpc, "buttonSyncToCpc");
    this.buttonSyncToCpc.Name = "buttonSyncToCpc";
    this.buttonSyncToCpc.UseCompatibleTextRendering = true;
    this.buttonSyncToCpc.UseVisualStyleBackColor = true;
    this.buttonSyncToCpc.Click += new EventHandler(this.buttonSyncToCpc_Click);
    componentResourceManager.ApplyResources((object) this.seekTimeListView, "seekTimeListView");
    this.seekTimeListView.FilterUserLabels = true;
    ((Control) this.seekTimeListView).Name = "seekTimeListView";
    this.seekTimeListView.RequiredUserLabelPrefix = "SetOdometer";
    this.seekTimeListView.SelectedTime = new DateTime?();
    this.seekTimeListView.ShowChannelLabels = false;
    this.seekTimeListView.ShowCommunicationsState = false;
    this.seekTimeListView.ShowControlPanel = false;
    this.seekTimeListView.ShowDeviceColumn = false;
    componentResourceManager.ApplyResources((object) this, "$this");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanelMain);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanelMain).ResumeLayout(false);
    ((Control) this.tableLayoutPanelMain).PerformLayout();
    ((Control) this.tableLayoutPanelStatus).ResumeLayout(false);
    ((Control) this.tableLayoutPanelStatus).PerformLayout();
    ((Control) this.tableLayoutPanelOdometerDisplay).ResumeLayout(false);
    ((Control) this.tableLayoutPanel5).ResumeLayout(false);
    ((Control) this.tableLayoutPanel5).PerformLayout();
    ((Control) this.tableLayoutPanel4).ResumeLayout(false);
    ((Control) this.tableLayoutPanel4).PerformLayout();
    ((Control) this).ResumeLayout(false);
  }

  private enum OdometerSetState
  {
    NotSet,
    Set,
    Working,
    Error,
  }

  private enum Step
  {
    Ready = 0,
    Warning = 1,
    SetCpc = 2,
    SetInstrumentCluster = 3,
    Done = 4,
    Error = 255, // 0x000000FF
  }

  private enum Operation
  {
    SetBoth,
    SyncToCpc,
    SyncToInstrumentCluster,
  }
}
