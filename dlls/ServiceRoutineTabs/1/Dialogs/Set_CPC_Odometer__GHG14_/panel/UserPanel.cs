// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Set_CPC_Odometer__GHG14_.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.UnitConversion;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Set_CPC_Odometer__GHG14_.panel;

public class UserPanel : CustomPanel
{
  private const string CpcName = "CPC04T";
  private const string SetOdometerRoutine = "DL_ID_Odometer";
  private const string KeyOffOnResetFunction = "FN_KeyOffOnReset";
  private const string OdometerQualifier = "CO_Odometer";
  private const string VersionQualifier = "CO_SoftwareVersion";
  private Tuple<string, bool>[] ProhibitedVersions = new Tuple<string, bool>[2]
  {
    Tuple.Create<string, bool>("R34_00_000A", true),
    Tuple.Create<string, bool>("R36_00_000A", false)
  };
  private EcuInfo odometerInfo;
  private Channel cpc;
  private Conversion valueToDisplay;
  private Conversion displayToValue;
  private UserPanel.OdometerSetState odometerSetState = UserPanel.OdometerSetState.NotSet;
  private string errorString;
  private double PreviousOdometerValue = -1.0;
  private TableLayoutPanel tableLayoutPanel1;
  private DigitalReadoutInstrument digitalReadoutInstrument1;
  private Checkmark checkmark1;
  private Button buttonSet;
  private Button buttonClose;
  private TextBox textBox1;
  private System.Windows.Forms.Label labelCondition;

  public UserPanel() => this.InitializeComponent();

  protected virtual void OnLoad(EventArgs e)
  {
    ((ContainerControl) this).ParentForm.FormClosing += new FormClosingEventHandler(this.OnFormClosing);
    this.textBox1.TextChanged += new EventHandler(this.TextBox1TextChanged);
    this.textBox1.KeyPress += new KeyPressEventHandler(this.TextBox1KeyPress);
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
    this.UpdateChannels();
    if (this.CheckProhibited())
      ((ContainerControl) this).ParentForm.Close();
    this.UpdateUserInterface();
  }

  private bool CheckProhibited()
  {
    if (this.cpc != null && this.cpc.EcuInfos["CO_SoftwareVersion"] != null)
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

  private void OnFormClosing(object sender, FormClosingEventArgs e)
  {
    if (e.Cancel || ((ContainerControl) this).ParentForm == null)
      return;
    ((ContainerControl) this).ParentForm.FormClosing -= new FormClosingEventHandler(this.OnFormClosing);
  }

  public virtual void OnChannelsChanged() => this.UpdateChannels();

  private void UpdateChannels()
  {
    Channel channel = this.GetChannel("CPC04T");
    if (this.cpc == channel)
      return;
    if (this.cpc != null)
    {
      this.cpc.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
      if (this.odometerInfo != null)
        this.odometerInfo.EcuInfoUpdateEvent -= new EcuInfoUpdateEventHandler(this.OdometerEcuInfoUpdateEvent);
    }
    this.cpc = channel;
    if (this.cpc != null)
    {
      this.cpc.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
      this.odometerInfo = channel.EcuInfos["CO_Odometer"];
      if (this.odometerInfo != null)
      {
        this.valueToDisplay = Converter.GlobalInstance.GetConversion(this.odometerInfo.Units.ToString(), Converter.GlobalInstance.SelectedUnitSystem);
        this.displayToValue = Converter.GlobalInstance.GetConversion(this.valueToDisplay.OutputUnit, this.odometerInfo.Units.ToString());
        this.odometerInfo.EcuInfoUpdateEvent += new EcuInfoUpdateEventHandler(this.OdometerEcuInfoUpdateEvent);
        if (this.OdometerValue.HasValue)
          this.textBox1.Text = this.valueToDisplay.Convert((object) this.OdometerValue).ToString((IFormatProvider) CultureInfo.CurrentCulture);
        this.UpdateUserInterface();
      }
    }
  }

  private void OdometerEcuInfoUpdateEvent(object sender, ResultEventArgs e)
  {
    double? odometerValue;
    int num1;
    if (this.OdometerValue.HasValue)
    {
      double previousOdometerValue = this.PreviousOdometerValue;
      odometerValue = this.OdometerValue;
      double v2 = odometerValue.Value;
      num1 = this.OdometerValuesAreEqual(previousOdometerValue, v2) ? 1 : 0;
    }
    else
      num1 = 0;
    if (num1 != 0)
      return;
    odometerValue = this.OdometerValue;
    double num2;
    if (!odometerValue.HasValue)
    {
      num2 = -1.0;
    }
    else
    {
      odometerValue = this.OdometerValue;
      num2 = odometerValue.Value;
    }
    this.PreviousOdometerValue = num2;
    this.odometerSetState = UserPanel.OdometerSetState.NotSet;
    this.UpdateUserInterface();
  }

  private void TextBox1ValueChanged(object sender, EventArgs e)
  {
    this.odometerSetState = UserPanel.OdometerSetState.NotSet;
    this.UpdateUserInterface();
  }

  private void buttonSet_Click(object sender, EventArgs e)
  {
    if (DialogResult.OK != MessageBox.Show(Resources.Message_DDECReportsLifeToDateDataWillBeResetClickOkToContinueOrClickCancelToAbortChangesAndExit, Application.ProductName, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2))
      return;
    this.SetOdometerEcuInfoValue();
  }

  private void SetOdometerEcuInfoValue()
  {
    Service service = this.cpc.Services["DL_ID_Odometer"];
    if (!(service != (Service) null))
      return;
    service.InputValues[0].Value = (object) this.displayToValue.Convert((object) this.textBox1.Text);
    service.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.SetOdometerServiceCompleteEvent);
    service.Execute(false);
  }

  private void SetOdometerServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    Service service = sender as Service;
    if (service != (Service) null)
      service.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.SetOdometerServiceCompleteEvent);
    if (e.Succeeded)
    {
      Thread.Sleep(100);
      this.CPCKeyOffOnReset();
    }
    else
    {
      this.odometerSetState = UserPanel.OdometerSetState.Error;
      this.errorString = e.Exception.Message;
    }
    this.UpdateUserInterface();
  }

  private void CPCKeyOffOnReset()
  {
    Service service = this.cpc.Services["FN_KeyOffOnReset"];
    if (!(service != (Service) null))
      return;
    service.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.CPCKeyOffOnResetFunctionCompleteEvent);
    service.Execute(false);
  }

  private void CPCKeyOffOnResetFunctionCompleteEvent(object sender, ResultEventArgs e)
  {
    Service service = sender as Service;
    if (service != (Service) null)
      service.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.CPCKeyOffOnResetFunctionCompleteEvent);
    if (e.Succeeded)
    {
      Thread.Sleep(100);
      this.odometerInfo.Read(false);
      if (this.OdometerValue.HasValue)
        this.textBox1.Text = this.valueToDisplay.Convert((object) this.OdometerValue).ToString((IFormatProvider) CultureInfo.CurrentCulture);
      this.odometerSetState = UserPanel.OdometerSetState.Set;
    }
    else
    {
      this.odometerSetState = UserPanel.OdometerSetState.Error;
      this.errorString = e.Exception.Message;
    }
    this.UpdateUserInterface();
  }

  private void OnCommunicationsStateUpdate(object sender, CommunicationsStateEventArgs e)
  {
    this.UpdateUserInterface();
  }

  private bool Online => this.cpc != null && this.cpc.Online;

  private bool EcuIsBusy
  {
    get => this.Online && this.cpc.CommunicationsState != CommunicationsState.Online;
  }

  private double? OdometerValue
  {
    get
    {
      return this.odometerInfo != null ? new double?(double.Parse(this.odometerInfo.Value, (IFormatProvider) CultureInfo.CurrentCulture)) : new double?();
    }
  }

  private bool OdometerValuesAreEqual(double v1, double v2) => Math.Abs(v1 - v2) < 0.1;

  private void UpdateUserInterface()
  {
    switch (this.odometerSetState)
    {
      case UserPanel.OdometerSetState.NotSet:
        double num1 = this.displayToValue != null ? this.displayToValue.Convert((object) this.textBox1.Text) : double.Parse(this.textBox1.Text, (IFormatProvider) CultureInfo.CurrentCulture);
        double? odometerValue = this.OdometerValue;
        if (!odometerValue.HasValue)
        {
          this.buttonSet.Enabled = false;
          this.checkmark1.Checked = false;
          this.labelCondition.Text = Resources.Message_TheOdometerValueIsUnknownAndCannotBeSet;
          break;
        }
        double v1 = num1;
        odometerValue = this.OdometerValue;
        double v2 = odometerValue.Value;
        if (this.OdometerValuesAreEqual(v1, v2))
        {
          this.buttonSet.Enabled = false;
          this.checkmark1.Checked = false;
          this.labelCondition.Text = Resources.Message_TheOdometerCannotBeSetBecauseTheProposedValueIsEffectivelyEqualToCurrentOdometer;
          this.labelCondition.BackColor = SystemColors.Control;
          break;
        }
        double num2 = num1;
        odometerValue = this.OdometerValue;
        if ((num2 <= odometerValue.GetValueOrDefault() ? 0 : (odometerValue.HasValue ? 1 : 0)) != 0)
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
        this.checkmark1.Checked = !this.EcuIsBusy;
        this.labelCondition.Text = this.EcuIsBusy ? Resources.Message_WritingOdometerValue : Resources.Message_TheOdometerHasBeenSetTheValueShownMayBeSlightlyDifferentThanTheOneEnteredDueToRoundingIssues;
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

  private void TextBox1TextChanged(object sender, EventArgs e)
  {
    this.odometerSetState = UserPanel.OdometerSetState.NotSet;
    this.UpdateUserInterface();
  }

  private void TextBox1KeyPress(object sender, KeyPressEventArgs e)
  {
    e.Handled = !char.IsDigit(e.KeyChar) && e.KeyChar != '.' && !char.IsControl(e.KeyChar);
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.digitalReadoutInstrument1 = new DigitalReadoutInstrument();
    this.buttonSet = new Button();
    this.buttonClose = new Button();
    this.checkmark1 = new Checkmark();
    this.labelCondition = new System.Windows.Forms.Label();
    this.textBox1 = new TextBox();
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument1, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonSet, 2, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonClose, 3, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.checkmark1, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.labelCondition, 1, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.textBox1, 0, 2);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.digitalReadoutInstrument1, 4);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument1, "digitalReadoutInstrument1");
    this.digitalReadoutInstrument1.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes) 8, "CPC04T", "CO_Odometer");
    ((Control) this.digitalReadoutInstrument1).Name = "digitalReadoutInstrument1";
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.buttonSet, "buttonSet");
    this.buttonSet.Name = "buttonSet";
    this.buttonSet.UseCompatibleTextRendering = true;
    this.buttonSet.UseVisualStyleBackColor = true;
    this.buttonSet.Click += new EventHandler(this.buttonSet_Click);
    this.buttonClose.DialogResult = DialogResult.OK;
    componentResourceManager.ApplyResources((object) this.buttonClose, "buttonClose");
    this.buttonClose.Name = "buttonClose";
    this.buttonClose.UseCompatibleTextRendering = true;
    this.buttonClose.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.checkmark1, "checkmark1");
    ((Control) this.checkmark1).Name = "checkmark1";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.labelCondition, 3);
    componentResourceManager.ApplyResources((object) this.labelCondition, "labelCondition");
    this.labelCondition.Name = "labelCondition";
    this.labelCondition.UseCompatibleTextRendering = true;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.textBox1, 2);
    componentResourceManager.ApplyResources((object) this.textBox1, "textBox1");
    this.textBox1.Name = "textBox1";
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("Panel_SetOdometer");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel1);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this.tableLayoutPanel1).PerformLayout();
    ((Control) this).ResumeLayout(false);
  }

  private enum OdometerSetState
  {
    NotSet,
    Set,
    Error,
  }
}
