// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Detroit_Assurance_Camera_Height.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Detroit_Assurance_Camera_Height.panel;

public class UserPanel : CustomPanel
{
  private const string VRDUName = "VRDU01T";
  private const string HeightFaultCode = "00FBED";
  private const string HeightParameterGroupQualifier = "VCD_Camera_Parameter";
  private const string DefaultCameraConfigPartNumber = "A0004472751001";
  private Channel vrdu = (Channel) null;
  private bool waitingForParameterWrite;
  private bool waitingForFaultStatusChange;
  private bool waitingForFaultClear;
  private DigitalReadoutInstrument digitalReadoutInstrument1;
  private TableLayoutPanel tableLayoutPanel1;
  private Button buttonSetHeight;
  private System.Windows.Forms.Label labelStatus;
  private DigitalReadoutInstrument digitalReadoutInstrumentCameraHeight;

  public UserPanel()
  {
    this.InitializeComponent();
    this.UpdateUserInterface();
  }

  protected virtual void OnLoad(EventArgs e)
  {
    ((ContainerControl) this).ParentForm.FormClosing += new FormClosingEventHandler(this.OnFormClosing);
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
    this.UpdateChannel();
  }

  private void OnFormClosing(object sender, FormClosingEventArgs e)
  {
    if (this.Busy)
      e.Cancel = true;
    if (e.Cancel)
      return;
    ((ContainerControl) this).ParentForm.FormClosing -= new FormClosingEventHandler(this.OnFormClosing);
    if (this.vrdu != null)
    {
      this.vrdu.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
      this.vrdu.FaultCodes.FaultCodesUpdateEvent -= new FaultCodesUpdateEventHandler(this.FaultCodes_FaultCodesUpdateEvent);
    }
  }

  public virtual void OnChannelsChanged() => this.UpdateChannel();

  private void UpdateChannel()
  {
    Channel channel = SapiManager.GlobalInstance.ActiveChannels.FirstOrDefault<Channel>((Func<Channel, bool>) (c => c.Ecu.Name == "VRDU01T"));
    if (this.vrdu == channel)
      return;
    if (this.vrdu != null)
    {
      this.vrdu.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
      this.vrdu.FaultCodes.FaultCodesUpdateEvent -= new FaultCodesUpdateEventHandler(this.FaultCodes_FaultCodesUpdateEvent);
    }
    this.vrdu = channel;
    if (this.vrdu != null)
    {
      this.vrdu.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnCommunicationsStateUpdate);
      this.vrdu.FaultCodes.FaultCodesUpdateEvent += new FaultCodesUpdateEventHandler(this.FaultCodes_FaultCodesUpdateEvent);
      this.UpdateParameterState();
      this.UpdateStatus(Resources.Message_Ready);
    }
  }

  private void OnCommunicationsStateUpdate(object sender, CommunicationsStateEventArgs e)
  {
    this.UpdateUserInterface();
  }

  private void UpdateStatus(string message) => this.labelStatus.Text = message;

  private void UpdateUserInterface()
  {
    if (this.Online && !this.Busy)
    {
      this.buttonSetHeight.Enabled = true;
      Cursor.Current = Cursors.Default;
    }
    else
    {
      this.buttonSetHeight.Enabled = false;
      Cursor.Current = Cursors.WaitCursor;
    }
  }

  private void UpdateParameterState()
  {
    if (!this.Online)
      return;
    this.vrdu.Parameters.ReadGroup("VCD_Camera_Parameter", false, false);
  }

  private void ResetCameraHeightFault()
  {
    if (this.Online)
    {
      FaultCode faultCode = this.vrdu.FaultCodes["00FBED"];
      if (faultCode != null)
      {
        this.vrdu.FaultCodes.FaultCodesUpdateEvent += new FaultCodesUpdateEventHandler(this.FaultCodes_FaultCodesUpdateEvent);
        this.UpdateStatus(Resources.Message_ClearingFault);
        this.waitingForFaultClear = true;
        faultCode.Reset(false);
      }
    }
    this.UpdateUserInterface();
  }

  private bool Online
  {
    get => this.vrdu != null && this.vrdu.CommunicationsState == CommunicationsState.Online;
  }

  private bool Busy
  {
    get
    {
      return this.waitingForParameterWrite || this.waitingForFaultClear || this.waitingForFaultStatusChange;
    }
  }

  private void buttonSetHeight_Click(object sender, EventArgs e)
  {
    if (this.Online)
    {
      CodingChoice codingForPart = this.vrdu.CodingParameterGroups.GetCodingForPart("A0004472751001");
      if (codingForPart != null)
      {
        this.UpdateStatus(Resources.Message_WritingParameter);
        this.waitingForParameterWrite = true;
        try
        {
          codingForPart.SetAsValue();
          this.vrdu.Parameters.ParametersWriteCompleteEvent += new ParametersWriteCompleteEventHandler(this.Parameters_ParametersWriteCompleteEvent);
          this.vrdu.Parameters.Write(false);
        }
        catch (CaesarException ex)
        {
          this.waitingForParameterWrite = false;
          if (ex.ErrorNumber == 5098L)
            this.UpdateStatus(Resources.Message_DefaultSettingNotFound);
          else
            this.UpdateStatus(Resources.Message_FailedToWriteParameters);
        }
      }
      else
        this.UpdateStatus(Resources.Message_DefaultSettingNotFound);
    }
    this.UpdateUserInterface();
  }

  private void Parameters_ParametersWriteCompleteEvent(object sender, ResultEventArgs e)
  {
    this.vrdu.Parameters.ParametersWriteCompleteEvent -= new ParametersWriteCompleteEventHandler(this.Parameters_ParametersWriteCompleteEvent);
    if (e.Succeeded)
    {
      FaultCode faultCode = this.vrdu.FaultCodes.Current.FirstOrDefault<FaultCode>((Func<FaultCode, bool>) (f => f.Code == "00FBED"));
      if (faultCode != null)
      {
        if (faultCode.FaultCodeIncidents.Current.Active == ActiveStatus.Active)
        {
          this.UpdateStatus(Resources.Message_WaitingForFaultToGoInactive);
          this.waitingForFaultStatusChange = true;
        }
        else
          this.ResetCameraHeightFault();
      }
      else
        this.UpdateStatus(Resources.Message_Ready1);
    }
    else
      this.UpdateStatus(Resources.Message_FailedToWriteParameters);
    this.waitingForParameterWrite = false;
    this.UpdateUserInterface();
  }

  private void FaultCodes_FaultCodesUpdateEvent(object sender, ResultEventArgs e)
  {
    if (this.waitingForFaultStatusChange)
    {
      this.waitingForFaultStatusChange = false;
      this.ResetCameraHeightFault();
    }
    else if (this.waitingForFaultClear)
    {
      this.UpdateStatus(Resources.Message_Ready2a);
      this.waitingForFaultClear = false;
    }
    this.UpdateUserInterface();
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.digitalReadoutInstrument1 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentCameraHeight = new DigitalReadoutInstrument();
    this.buttonSetHeight = new Button();
    this.labelStatus = new System.Windows.Forms.Label();
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument1, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrumentCameraHeight, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonSetHeight, 2, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.labelStatus, 0, 2);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument1, "digitalReadoutInstrument1");
    this.digitalReadoutInstrument1.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes) 32 /*0x20*/, "VRDU01T", "00FBED");
    ((Control) this.digitalReadoutInstrument1).Name = "digitalReadoutInstrument1";
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.digitalReadoutInstrumentCameraHeight, 2);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentCameraHeight, "digitalReadoutInstrumentCameraHeight");
    this.digitalReadoutInstrumentCameraHeight.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentCameraHeight).FreezeValue = false;
    this.digitalReadoutInstrumentCameraHeight.Gradient.Initialize((ValueState) 3, 2);
    this.digitalReadoutInstrumentCameraHeight.Gradient.Modify(1, 2.549, (ValueState) 1);
    this.digitalReadoutInstrumentCameraHeight.Gradient.Modify(2, 2.56, (ValueState) 2);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentCameraHeight).Instrument = new Qualifier((QualifierTypes) 4, "VRDU01T", "camera_height");
    ((Control) this.digitalReadoutInstrumentCameraHeight).Name = "digitalReadoutInstrumentCameraHeight";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentCameraHeight).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.buttonSetHeight, "buttonSetHeight");
    this.buttonSetHeight.Name = "buttonSetHeight";
    this.buttonSetHeight.UseCompatibleTextRendering = true;
    this.buttonSetHeight.UseVisualStyleBackColor = true;
    this.buttonSetHeight.Click += new EventHandler(this.buttonSetHeight_Click);
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.labelStatus, 2);
    componentResourceManager.ApplyResources((object) this.labelStatus, "labelStatus");
    this.labelStatus.Name = "labelStatus";
    this.labelStatus.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this, "$this");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel1);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }
}
