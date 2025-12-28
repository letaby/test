// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Predictive_Cruise_Control_diagnostic__MY13_.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Parameters;
using SapiLayer1;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Predictive_Cruise_Control_diagnostic__MY13_.panel;

public class UserPanel : CustomPanel
{
  private const string PccEcuName = "J1939-17";
  private Channel cpc;
  private Parameter pccParameter;
  private TableLayoutPanel tableLayoutPanel1;
  private Checkmark checkmark1;
  private System.Windows.Forms.Label labelStatus;
  private TableLayoutPanel tableLayoutPanel2;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label labelPccTitle;
  private TableLayoutPanel tableLayoutPanel3;
  private ParameterEditor parameterEditor;
  private Button button1;
  private ScalingLabel scalingLabelPcc;

  public UserPanel() => this.InitializeComponent();

  public virtual void OnChannelsChanged()
  {
    Qualifier instrument = ((SingleInstrumentBase) this.parameterEditor).Instrument;
    this.SetCPC(this.GetChannel(((Qualifier) ref instrument).Ecu));
    this.UpdateUserInterface();
  }

  private void UserPanel_ParentFormClosing(object sender, FormClosingEventArgs e)
  {
    if (e.Cancel)
      return;
    this.SetCPC((Channel) null);
  }

  private void button1_Click(object sender, EventArgs e)
  {
    if (this.cpc == null)
      return;
    this.cpc.Parameters.Write(false);
  }

  private void cpc_CommunicationsStateUpdateEvent(object sender, CommunicationsStateEventArgs e)
  {
    if (this.pccParameter != null && !this.pccParameter.HasBeenReadFromEcu && e.CommunicationsState == CommunicationsState.Online)
      this.cpc.Parameters.ReadGroup(this.pccParameter.GroupQualifier, true, false);
    this.UpdateUserInterface();
  }

  private void pccParameter_ParameterUpdateEvent(object sender, ResultEventArgs e)
  {
    this.UpdateSendButton();
  }

  private void Parameters_ParametersReadCompleteEvent(object sender, ResultEventArgs e)
  {
    this.UpdateUserInterface();
  }

  private PccPresenceStatus PccPresence
  {
    get
    {
      if (!SapiManager.GlobalInstance.RollCallEnabled)
        return PccPresenceStatus.NoRollCall;
      float? load = ChannelCollection.GetManager(Protocol.J1939).Load;
      return ((double) load.GetValueOrDefault() <= 0.0 ? 0 : (load.HasValue ? 1 : 0)) != 0 ? (SapiManager.GlobalInstance.Sapi.Channels.Any<Channel>((Func<Channel, bool>) (c => c.Online && c.Ecu.Name == "J1939-17")) ? PccPresenceStatus.Detected : PccPresenceStatus.NotDetected) : PccPresenceStatus.NoDataLink;
    }
  }

  private CpcStatus CpcState
  {
    get
    {
      if (this.cpc == null || !this.cpc.Online || this.pccParameter == null)
        return CpcStatus.OfflineOrNoParameter;
      return this.cpc.CommunicationsState != CommunicationsState.WriteParameters ? (this.pccParameter.HasBeenReadFromEcu ? CpcStatus.Ready : CpcStatus.ParameterNotRead) : CpcStatus.WritingParameters;
    }
  }

  private bool PccConfigured
  {
    get
    {
      if (this.pccParameter != null && this.pccParameter.HasBeenReadFromEcu)
      {
        Choice originalValue = this.pccParameter.OriginalValue as Choice;
        if (originalValue != (object) null)
          return Convert.ToByte(originalValue.RawValue) == (byte) 1;
      }
      return false;
    }
  }

  private void UpdateUserInterface()
  {
    PccPresenceStatus pccPresence = this.PccPresence;
    CpcStatus cpcState = this.CpcState;
    switch (pccPresence)
    {
      case PccPresenceStatus.NoRollCall:
        ((Control) this.scalingLabelPcc).Text = Resources.Message_UnableToDetect;
        this.labelStatus.Text = Resources.Message_ThePredictiveCruiseControlConfigurationCanOnlyBeValidatedWhenAutomaticConnectionToStandardSAEJ1939DevicesIsEnabledInToolsOptionsConnections;
        this.checkmark1.CheckState = CheckState.Indeterminate;
        break;
      case PccPresenceStatus.NoDataLink:
        ((Control) this.scalingLabelPcc).Text = Resources.Message_NoDataLink;
        this.labelStatus.Text = Resources.Message_ThePredictiveCruiseControlConfigurationCanOnlyBeValidatedWhenTheDiagnosticToolIsConnectedToAJ1939Vehicle;
        this.checkmark1.CheckState = CheckState.Indeterminate;
        break;
      default:
        ((Control) this.scalingLabelPcc).Text = pccPresence == PccPresenceStatus.Detected ? Resources.Message_Detected0 : Resources.Message_NotDetected;
        switch (cpcState)
        {
          case CpcStatus.OfflineOrNoParameter:
            this.labelStatus.Text = Resources.Message_ThePredictiveCruiseControlConfigurationCanOnlyBeValidatedWhenTheCPCIsOnlineAndInAnApplicationMode;
            this.checkmark1.CheckState = CheckState.Indeterminate;
            break;
          case CpcStatus.WritingParameters:
            this.labelStatus.Text = Resources.Message_ThePredictiveCruiseControlConfigurationIsBeingWrittenToTheCPCPleaseWait;
            this.checkmark1.CheckState = CheckState.Indeterminate;
            break;
          case CpcStatus.ParameterNotRead:
            this.labelStatus.Text = Resources.Message_ThePredictiveCruiseControlConfigurationCanOnlyBeValidatedOnceParametersHaveBeenReadFromTheCPCPleaseWait;
            this.checkmark1.CheckState = CheckState.Indeterminate;
            break;
          case CpcStatus.Ready:
            if (pccPresence == PccPresenceStatus.Detected)
            {
              if (this.PccConfigured)
              {
                this.checkmark1.CheckState = CheckState.Checked;
                this.labelStatus.Text = Resources.Message_ThePredictiveCruiseControlDeviceIsDetectedOnTheDataLinkAndTheCPCAppearsToBeConfiguredToUseIt;
                break;
              }
              this.checkmark1.CheckState = CheckState.Unchecked;
              this.labelStatus.Text = Resources.Message_ThePredictiveCruiseControlDeviceIsDetectedOnTheDataLinkButTheCPCIsNotConfiguredToUseItUseTheDropDownToChangeTheCPCConfigurationThenPressTheSendButtonToCommit;
              break;
            }
            if (this.PccConfigured)
            {
              this.checkmark1.CheckState = CheckState.Unchecked;
              this.labelStatus.Text = Resources.Message_TheCPCIsConfiguredForPredictiveCruiseControlOperationButTheDeviceIsNotDetectedOnTheDataLinkCheckTheVehicleWiring;
            }
            else
            {
              this.checkmark1.CheckState = CheckState.Indeterminate;
              this.labelStatus.Text = Resources.Message_TheCPCIsNotConfiguredForPredictiveCruiseControlOperationAndTheDeviceIsNotDetectedOnTheDataLink;
            }
            break;
        }
        break;
    }
    ((Control) this.parameterEditor).Enabled = cpcState == CpcStatus.Ready;
    this.UpdateSendButton();
  }

  private void UpdateSendButton()
  {
    this.button1.Enabled = this.CpcState == CpcStatus.Ready && this.pccParameter.OriginalValue != this.pccParameter.Value;
  }

  private void SetCPC(Channel channel)
  {
    if (this.cpc == channel)
      return;
    if (this.cpc != null)
    {
      this.cpc.Parameters.ParametersReadCompleteEvent -= new ParametersReadCompleteEventHandler(this.Parameters_ParametersReadCompleteEvent);
      this.cpc.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.cpc_CommunicationsStateUpdateEvent);
      if (this.pccParameter != null)
      {
        this.pccParameter.ParameterUpdateEvent -= new ParameterUpdateEventHandler(this.pccParameter_ParameterUpdateEvent);
        this.pccParameter = (Parameter) null;
      }
    }
    this.cpc = channel;
    if (this.cpc != null)
    {
      this.cpc.Parameters.ParametersReadCompleteEvent += new ParametersReadCompleteEventHandler(this.Parameters_ParametersReadCompleteEvent);
      this.cpc.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.cpc_CommunicationsStateUpdateEvent);
      ParameterCollection parameters = this.cpc.Parameters;
      Qualifier instrument = ((SingleInstrumentBase) this.parameterEditor).Instrument;
      string name = ((Qualifier) ref instrument).Name;
      this.pccParameter = parameters[name];
      if (this.pccParameter != null)
      {
        this.pccParameter.ParameterUpdateEvent += new ParameterUpdateEventHandler(this.pccParameter_ParameterUpdateEvent);
        if (!this.pccParameter.HasBeenReadFromEcu && this.cpc.CommunicationsState == CommunicationsState.Online)
          this.cpc.Parameters.ReadGroup(this.pccParameter.GroupQualifier, true, false);
      }
    }
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.tableLayoutPanel2 = new TableLayoutPanel();
    this.tableLayoutPanel3 = new TableLayoutPanel();
    this.parameterEditor = new ParameterEditor();
    this.button1 = new Button();
    this.labelPccTitle = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.scalingLabelPcc = new ScalingLabel();
    this.checkmark1 = new Checkmark();
    this.labelStatus = new System.Windows.Forms.Label();
    ((Control) this.tableLayoutPanel3).SuspendLayout();
    ((Control) this.tableLayoutPanel2).SuspendLayout();
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel3, "tableLayoutPanel3");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.tableLayoutPanel3, 2);
    ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.parameterEditor, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel3).Controls.Add((Control) this.button1, 1, 0);
    ((Control) this.tableLayoutPanel3).Name = "tableLayoutPanel3";
    componentResourceManager.ApplyResources((object) this.parameterEditor, "parameterEditor");
    ((SingleInstrumentBase) this.parameterEditor).FreezeValue = false;
    ((SingleInstrumentBase) this.parameterEditor).Instrument = new Qualifier((QualifierTypes) 4, "CPC04T", "PCC_Enable");
    ((Control) this.parameterEditor).Name = "parameterEditor";
    ((SingleInstrumentBase) this.parameterEditor).ShowBorder = false;
    ((SingleInstrumentBase) this.parameterEditor).ShowUnits = false;
    ((SingleInstrumentBase) this.parameterEditor).TitleLengthPercentOfControl = 50;
    this.parameterEditor.TitleWordWrap = true;
    this.parameterEditor.UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.button1, "button1");
    this.button1.Name = "button1";
    this.button1.UseCompatibleTextRendering = true;
    this.button1.UseVisualStyleBackColor = true;
    this.button1.Click += new EventHandler(this.button1_Click);
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel2, "tableLayoutPanel2");
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.tableLayoutPanel2, 2);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.labelPccTitle, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.scalingLabelPcc, 0, 1);
    ((Control) this.tableLayoutPanel2).Name = "tableLayoutPanel2";
    this.labelPccTitle.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.labelPccTitle, "labelPccTitle");
    ((Control) this.labelPccTitle).Name = "labelPccTitle";
    this.labelPccTitle.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.scalingLabelPcc.Alignment = StringAlignment.Far;
    componentResourceManager.ApplyResources((object) this.scalingLabelPcc, "scalingLabelPcc");
    this.scalingLabelPcc.FontGroup = "";
    this.scalingLabelPcc.LineAlignment = StringAlignment.Center;
    ((Control) this.scalingLabelPcc).Name = "scalingLabelPcc";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.checkmark1, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.labelStatus, 1, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.tableLayoutPanel2, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.tableLayoutPanel3, 0, 1);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    componentResourceManager.ApplyResources((object) this.checkmark1, "checkmark1");
    ((Control) this.checkmark1).Name = "checkmark1";
    componentResourceManager.ApplyResources((object) this.labelStatus, "labelStatus");
    this.labelStatus.Name = "labelStatus";
    this.labelStatus.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("Panel_PredictiveCruiseControlDiagnostic");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel1);
    ((Control) this).Name = nameof (UserPanel);
    this.ParentFormClosing += new EventHandler<FormClosingEventArgs>(this.UserPanel_ParentFormClosing);
    ((Control) this.tableLayoutPanel3).ResumeLayout(false);
    ((Control) this.tableLayoutPanel2).ResumeLayout(false);
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }
}
