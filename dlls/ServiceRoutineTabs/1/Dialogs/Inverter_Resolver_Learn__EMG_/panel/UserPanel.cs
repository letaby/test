// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Inverter_Resolver_Learn__EMG_.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Help;
using DetroitDiesel.Utilities;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Inverter_Resolver_Learn__EMG_.panel;

public class UserPanel : CustomPanel
{
  private const string PtfConfPTransGroup = "VCD_PID_222_ptconf_p_Trans";
  private const string PtfConfPTransEmotNum = "ptconf_p_Trans_EmotNum_u8";
  private Channel eCpcChannel;
  private bool isEmot2Num = false;
  private bool isEmot3Num = false;
  private bool isStopped1 = true;
  private bool isStopped2 = true;
  private bool isStopped3 = true;
  private Parameter emotNumParameter = (Parameter) null;
  private TableLayoutPanel tableLayoutPanel1;
  private DigitalReadoutInstrument digitalReadoutInstrument4;
  private DigitalReadoutInstrument digitalReadoutInstrument5;
  private DigitalReadoutInstrument digitalReadoutInstrument6;
  private TableLayoutPanel tableLayoutPanelInverter1LearnRoutine;
  private Button buttonInverter1Start;
  private System.Windows.Forms.Label labelStatusLabelInverter1;
  private Checkmark checkmarkInverter1;
  private DigitalReadoutInstrument digitalReadoutInstrument7;
  private DigitalReadoutInstrument digitalReadoutInstrumentInverterLearnResults1;
  private System.Windows.Forms.Label labelResolver1LearnRoutineHeader;
  private TableLayoutPanel tableLayoutPanelInverter2LearnRoutine;
  private Button buttonInverter2Start;
  private System.Windows.Forms.Label labelStatusLabelInverter2;
  private Checkmark checkmarkInverter2;
  private DigitalReadoutInstrument digitalReadoutInstrumentMotorSpeed2;
  private System.Windows.Forms.Label labelResolver2LearnRoutineHeader;
  private DigitalReadoutInstrument digitalReadoutInstrumentInverterLearnResults2;
  private TableLayoutPanel tableLayoutPanelInverter3LearnRoutine;
  private Button buttonInverter3Start;
  private System.Windows.Forms.Label labelStatusLabelInverter3;
  private Checkmark checkmarkInverter3;
  private DigitalReadoutInstrument digitalReadoutInstrumentMotorSpeed3;
  private DigitalReadoutInstrument digitalReadoutInstrumentInverterLearnResults3;
  private System.Windows.Forms.Label labelResolver3LearnRoutineHeader;
  private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponentInverter1;
  private SharedProcedureCreatorComponent sharedProcedureCreatorComponentInverter1;
  private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponentInverter2;
  private SharedProcedureCreatorComponent sharedProcedureCreatorComponentInverter2;
  private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponentInverter3;
  private SharedProcedureCreatorComponent sharedProcedureCreatorComponentInverter3;
  private SharedProcedureSelection sharedProcedureSelectionInverter3;
  private SharedProcedureSelection sharedProcedureSelectionInverter2;
  private SharedProcedureSelection sharedProcedureSelectionInverter1;
  private TableLayoutPanel tableLayoutPanelTop;
  private WebBrowser webBrowserWarning;
  private PictureBox pictureBox1;
  private TableLayoutPanel tableLayoutPanelBottom;
  private System.Windows.Forms.Label label5;
  private System.Windows.Forms.Label labelStatus2;
  private System.Windows.Forms.Label labelStatus3;
  private SeekTimeListView seekTimeListView1;

  protected virtual void OnLoad(EventArgs e)
  {
    ((ContainerControl) this).ParentForm.FormClosing += new FormClosingEventHandler(this.OnParentFormClosing);
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
    this.isStopped1 = true;
    this.isStopped2 = true;
    this.isStopped3 = true;
    this.digitalReadoutInstrumentInverterLearnResults1.RepresentedStateChanged += new EventHandler(this.digitalReadoutInstrumentInverterLearnResults1_RepresentedStateChanged);
    this.digitalReadoutInstrumentInverterLearnResults2.RepresentedStateChanged += new EventHandler(this.digitalReadoutInstrumentInverterLearnResults2_RepresentedStateChanged);
    this.digitalReadoutInstrumentInverterLearnResults3.RepresentedStateChanged += new EventHandler(this.digitalReadoutInstrumentInverterLearnResults3_RepresentedStateChanged);
    this.UpdateUI();
  }

  public UserPanel() => this.InitializeComponent();

  public virtual void OnChannelsChanged() => this.SetECPC01TChannel("ECPC01T");

  private void OnParentFormClosing(object sender, FormClosingEventArgs e)
  {
    e.Cancel = this.Learning1InProcess || this.Learning2InProcess || this.Learning3InProcess;
    if (e.Cancel)
      return;
    ((ContainerControl) this).ParentForm.FormClosing -= new FormClosingEventHandler(this.OnParentFormClosing);
  }

  private void SetECPC01TChannel(string ecuName)
  {
    if (this.eCpcChannel != this.GetChannel(ecuName))
    {
      this.eCpcChannel = this.GetChannel(ecuName);
      if (this.eCpcChannel != null)
      {
        this.isStopped1 = true;
        this.isStopped2 = true;
        this.isStopped3 = true;
        this.emotNumParameter = this.eCpcChannel.Parameters["ptconf_p_Trans_EmotNum_u8"];
        if (this.emotNumParameter != null && this.eCpcChannel.CommunicationsState == CommunicationsState.Online)
          this.ReadParameterGroup("VCD_PID_222_ptconf_p_Trans");
      }
    }
    this.UpdateUI();
  }

  private bool Learning1InProcess
  {
    get => this.digitalReadoutInstrumentInverterLearnResults1.RepresentedState == 2;
  }

  private bool Learning2InProcess
  {
    get => this.digitalReadoutInstrumentInverterLearnResults2.RepresentedState == 2;
  }

  private bool Learning3InProcess
  {
    get => this.digitalReadoutInstrumentInverterLearnResults3.RepresentedState == 2;
  }

  private void ReadParameterGroup(string group)
  {
    if (this.eCpcChannel == null || this.eCpcChannel.Parameters == null)
      return;
    this.eCpcChannel.Parameters.ParametersReadCompleteEvent += new ParametersReadCompleteEventHandler(this.Parameters_ParametersInitialReadCompleteEvent);
    this.eCpcChannel.Parameters.ReadGroup(group, false, false);
  }

  private void Parameters_ParametersInitialReadCompleteEvent(object sender, ResultEventArgs e)
  {
    this.eCpcChannel.Parameters.ParametersReadCompleteEvent -= new ParametersReadCompleteEventHandler(this.Parameters_ParametersInitialReadCompleteEvent);
    if (e.Succeeded)
    {
      int int32 = Convert.ToInt32(this.emotNumParameter.Value);
      this.isEmot2Num = int32 == 2;
      this.isEmot3Num = int32 == 3;
    }
    this.UpdateUI();
  }

  private void AddLogLabel(string text)
  {
    if (!(text != string.Empty))
      return;
    this.LabelLog(this.seekTimeListView1.RequiredUserLabelPrefix, text);
  }

  private void UpdateWarningMessage(bool displayInverter3Message)
  {
    this.webBrowserWarning.DocumentText = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "<html><style>{0}</style><body><span class='scaled bold red'>{1}</span><span class='scaled bold'>{2}</span><br><span class='scaled'>{3}{4}</span></body></html>", (object) ("html { height:100%; display: table; } " + "body { margin: 0px; padding: 0px; display: table-cell; vertical-align: middle; } " + ".scaled { font-size: calc(0.1vw + 8.75vh); font-family: Segoe UI; padding: 0px; margin: 4px; }  " + ".bold { font-weight: bold; }" + ".red { color: red; }"), (object) Resources.RedWarning, (object) Resources.BlackWarning, (object) Resources.WarningText, displayInverter3Message ? (object) Resources.WarningText_Inverter_3 : (object) string.Empty);
    this.webBrowserWarning.Update();
  }

  private void UpdateUI()
  {
    this.sharedProcedureIntegrationComponentInverter2.ProceduresDropDown = this.isEmot2Num || this.isEmot3Num ? this.sharedProcedureSelectionInverter2 : (SharedProcedureSelection) null;
    ((Control) this.tableLayoutPanelInverter2LearnRoutine).Visible = this.isEmot2Num || this.isEmot3Num;
    ((TableLayoutPanel) this.tableLayoutPanel1).RowStyles[3].Height = this.isEmot2Num || this.isEmot3Num ? 23f : 0.0f;
    this.sharedProcedureIntegrationComponentInverter3.ProceduresDropDown = this.isEmot3Num ? this.sharedProcedureSelectionInverter3 : (SharedProcedureSelection) null;
    ((Control) this.tableLayoutPanelInverter3LearnRoutine).Visible = this.isEmot3Num;
    ((TableLayoutPanel) this.tableLayoutPanel1).RowStyles[5].Height = this.isEmot3Num ? 23f : 0.0f;
    this.UpdateWarningMessage(this.isEmot3Num);
  }

  private void sharedProcedureCreatorComponentInverter1_StartServiceComplete(
    object sender,
    SingleServiceResultEventArgs e)
  {
    this.isStopped1 = false;
    if (((ResultEventArgs) e).Succeeded)
    {
      this.AddLogLabel(Resources.Message_Resolver_1_Learn_Routine_Started);
    }
    else
    {
      this.AddLogLabel(Resources.Message_Resolver_1_Learn_Routine_FailedToStart);
      this.AddLogLabel(((ResultEventArgs) e).Exception.Message);
    }
    this.UpdateUI();
  }

  private void sharedProcedureCreatorComponentInverter1_StopServiceComplete(
    object sender,
    SingleServiceResultEventArgs e)
  {
    this.isStopped1 = true;
    this.AddLogLabel(Resources.Message_Resolver_1_Learn_Routine_Stopped);
    this.UpdateUI();
  }

  private void sharedProcedureCreatorComponentInverter2_StartServiceComplete(
    object sender,
    SingleServiceResultEventArgs e)
  {
    this.isStopped1 = false;
    if (((ResultEventArgs) e).Succeeded)
    {
      this.AddLogLabel(Resources.Message_Resolver_2_Learn_Routine_Started);
    }
    else
    {
      this.AddLogLabel(Resources.Message_Resolver_2_Learn_Routine_FailedToStart);
      this.AddLogLabel(((ResultEventArgs) e).Exception.Message);
    }
    this.UpdateUI();
  }

  private void sharedProcedureCreatorComponentInverter2_StopServiceComplete(
    object sender,
    SingleServiceResultEventArgs e)
  {
    this.isStopped1 = true;
    this.AddLogLabel(Resources.Message_Resolver_2_Learn_Routine_Stopped);
    this.UpdateUI();
  }

  private void sharedProcedureCreatorComponentInverter3_StartServiceComplete(
    object sender,
    SingleServiceResultEventArgs e)
  {
    this.isStopped1 = false;
    if (((ResultEventArgs) e).Succeeded)
    {
      this.AddLogLabel(Resources.Message_Resolver_3_Learn_Routine_Started);
    }
    else
    {
      this.AddLogLabel(Resources.Message_Resolver_3_Learn_Routine_FailedToStart);
      this.AddLogLabel(((ResultEventArgs) e).Exception.Message);
    }
    this.UpdateUI();
  }

  private void sharedProcedureCreatorComponentInverter3_StopServiceComplete(
    object sender,
    SingleServiceResultEventArgs e)
  {
    this.isStopped1 = true;
    this.AddLogLabel(Resources.Message_Resolver_3_Learn_Routine_Stopped);
    this.UpdateUI();
  }

  private void digitalReadoutInstrumentInverterLearnResults1_RepresentedStateChanged(
    object sender,
    EventArgs e)
  {
    if (!this.Learning1InProcess && !this.isStopped1)
      this.buttonInverter1Start.PerformClick();
    this.UpdateUI();
  }

  private void digitalReadoutInstrumentInverterLearnResults2_RepresentedStateChanged(
    object sender,
    EventArgs e)
  {
    if (!this.Learning2InProcess && !this.isStopped2)
      this.buttonInverter2Start.PerformClick();
    this.UpdateUI();
  }

  private void digitalReadoutInstrumentInverterLearnResults3_RepresentedStateChanged(
    object sender,
    EventArgs e)
  {
    if (!this.Learning3InProcess && !this.isStopped3)
      this.buttonInverter3Start.PerformClick();
    this.UpdateUI();
  }

  private void InitializeComponent()
  {
    this.components = (IContainer) new Container();
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    DataItemCondition dataItemCondition1 = new DataItemCondition();
    DataItemCondition dataItemCondition2 = new DataItemCondition();
    DataItemCondition dataItemCondition3 = new DataItemCondition();
    DataItemCondition dataItemCondition4 = new DataItemCondition();
    DataItemCondition dataItemCondition5 = new DataItemCondition();
    DataItemCondition dataItemCondition6 = new DataItemCondition();
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.tableLayoutPanelInverter1LearnRoutine = new TableLayoutPanel();
    this.buttonInverter1Start = new Button();
    this.labelStatusLabelInverter1 = new System.Windows.Forms.Label();
    this.label5 = new System.Windows.Forms.Label();
    this.checkmarkInverter1 = new Checkmark();
    this.digitalReadoutInstrument7 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentInverterLearnResults1 = new DigitalReadoutInstrument();
    this.labelResolver1LearnRoutineHeader = new System.Windows.Forms.Label();
    this.tableLayoutPanelInverter2LearnRoutine = new TableLayoutPanel();
    this.buttonInverter2Start = new Button();
    this.labelStatusLabelInverter2 = new System.Windows.Forms.Label();
    this.labelStatus2 = new System.Windows.Forms.Label();
    this.checkmarkInverter2 = new Checkmark();
    this.digitalReadoutInstrumentMotorSpeed2 = new DigitalReadoutInstrument();
    this.labelResolver2LearnRoutineHeader = new System.Windows.Forms.Label();
    this.digitalReadoutInstrumentInverterLearnResults2 = new DigitalReadoutInstrument();
    this.tableLayoutPanelInverter3LearnRoutine = new TableLayoutPanel();
    this.buttonInverter3Start = new Button();
    this.labelStatusLabelInverter3 = new System.Windows.Forms.Label();
    this.labelStatus3 = new System.Windows.Forms.Label();
    this.checkmarkInverter3 = new Checkmark();
    this.digitalReadoutInstrumentMotorSpeed3 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentInverterLearnResults3 = new DigitalReadoutInstrument();
    this.labelResolver3LearnRoutineHeader = new System.Windows.Forms.Label();
    this.tableLayoutPanelTop = new TableLayoutPanel();
    this.webBrowserWarning = new WebBrowser();
    this.pictureBox1 = new PictureBox();
    this.tableLayoutPanelBottom = new TableLayoutPanel();
    this.seekTimeListView1 = new SeekTimeListView();
    this.sharedProcedureSelectionInverter3 = new SharedProcedureSelection();
    this.digitalReadoutInstrument4 = new DigitalReadoutInstrument();
    this.sharedProcedureSelectionInverter2 = new SharedProcedureSelection();
    this.digitalReadoutInstrument5 = new DigitalReadoutInstrument();
    this.digitalReadoutInstrument6 = new DigitalReadoutInstrument();
    this.sharedProcedureSelectionInverter1 = new SharedProcedureSelection();
    this.sharedProcedureIntegrationComponentInverter1 = new SharedProcedureIntegrationComponent(this.components);
    this.sharedProcedureCreatorComponentInverter1 = new SharedProcedureCreatorComponent(this.components);
    this.sharedProcedureIntegrationComponentInverter2 = new SharedProcedureIntegrationComponent(this.components);
    this.sharedProcedureCreatorComponentInverter2 = new SharedProcedureCreatorComponent(this.components);
    this.sharedProcedureIntegrationComponentInverter3 = new SharedProcedureIntegrationComponent(this.components);
    this.sharedProcedureCreatorComponentInverter3 = new SharedProcedureCreatorComponent(this.components);
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this.tableLayoutPanelInverter1LearnRoutine).SuspendLayout();
    ((Control) this.tableLayoutPanelInverter2LearnRoutine).SuspendLayout();
    ((Control) this.tableLayoutPanelInverter3LearnRoutine).SuspendLayout();
    ((Control) this.tableLayoutPanelTop).SuspendLayout();
    ((ISupportInitialize) this.pictureBox1).BeginInit();
    ((Control) this.tableLayoutPanelBottom).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.tableLayoutPanelInverter1LearnRoutine, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.tableLayoutPanelInverter2LearnRoutine, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.tableLayoutPanelInverter3LearnRoutine, 0, 5);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.tableLayoutPanelTop, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.tableLayoutPanelBottom, 0, 7);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelInverter1LearnRoutine, "tableLayoutPanelInverter1LearnRoutine");
    ((TableLayoutPanel) this.tableLayoutPanelInverter1LearnRoutine).Controls.Add((Control) this.buttonInverter1Start, 4, 2);
    ((TableLayoutPanel) this.tableLayoutPanelInverter1LearnRoutine).Controls.Add((Control) this.labelStatusLabelInverter1, 2, 2);
    ((TableLayoutPanel) this.tableLayoutPanelInverter1LearnRoutine).Controls.Add((Control) this.label5, 1, 2);
    ((TableLayoutPanel) this.tableLayoutPanelInverter1LearnRoutine).Controls.Add((Control) this.checkmarkInverter1, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanelInverter1LearnRoutine).Controls.Add((Control) this.digitalReadoutInstrument7, 2, 1);
    ((TableLayoutPanel) this.tableLayoutPanelInverter1LearnRoutine).Controls.Add((Control) this.digitalReadoutInstrumentInverterLearnResults1, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanelInverter1LearnRoutine).Controls.Add((Control) this.labelResolver1LearnRoutineHeader, 0, 0);
    ((Control) this.tableLayoutPanelInverter1LearnRoutine).Name = "tableLayoutPanelInverter1LearnRoutine";
    componentResourceManager.ApplyResources((object) this.buttonInverter1Start, "buttonInverter1Start");
    this.buttonInverter1Start.Name = "buttonInverter1Start";
    this.buttonInverter1Start.UseCompatibleTextRendering = true;
    this.buttonInverter1Start.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.labelStatusLabelInverter1, "labelStatusLabelInverter1");
    ((TableLayoutPanel) this.tableLayoutPanelInverter1LearnRoutine).SetColumnSpan((Control) this.labelStatusLabelInverter1, 2);
    this.labelStatusLabelInverter1.Name = "labelStatusLabelInverter1";
    this.labelStatusLabelInverter1.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.label5, "label5");
    this.label5.Name = "label5";
    this.label5.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.checkmarkInverter1, "checkmarkInverter1");
    ((Control) this.checkmarkInverter1).Name = "checkmarkInverter1";
    ((TableLayoutPanel) this.tableLayoutPanelInverter1LearnRoutine).SetColumnSpan((Control) this.digitalReadoutInstrument7, 2);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument7, "digitalReadoutInstrument7");
    this.digitalReadoutInstrument7.FontGroup = "AllwaysDisplayedInstruments";
    ((SingleInstrumentBase) this.digitalReadoutInstrument7).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument7).Instrument = new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS124_Actual_E_Motor_Speed_E_Motor_1_Actual_E_Motor_Speed_E_Motor_1");
    ((Control) this.digitalReadoutInstrument7).Name = "digitalReadoutInstrument7";
    ((SingleInstrumentBase) this.digitalReadoutInstrument7).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanelInverter1LearnRoutine).SetColumnSpan((Control) this.digitalReadoutInstrumentInverterLearnResults1, 3);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentInverterLearnResults1, "digitalReadoutInstrumentInverterLearnResults1");
    this.digitalReadoutInstrumentInverterLearnResults1.FontGroup = "AllwaysDisplayedInstruments";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentInverterLearnResults1).FreezeValue = false;
    this.digitalReadoutInstrumentInverterLearnResults1.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText"));
    this.digitalReadoutInstrumentInverterLearnResults1.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText1"));
    this.digitalReadoutInstrumentInverterLearnResults1.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText2"));
    this.digitalReadoutInstrumentInverterLearnResults1.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText3"));
    this.digitalReadoutInstrumentInverterLearnResults1.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText4"));
    this.digitalReadoutInstrumentInverterLearnResults1.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText5"));
    this.digitalReadoutInstrumentInverterLearnResults1.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText6"));
    this.digitalReadoutInstrumentInverterLearnResults1.Gradient.Initialize((ValueState) 0, 6);
    this.digitalReadoutInstrumentInverterLearnResults1.Gradient.Modify(1, 0.0, (ValueState) 5);
    this.digitalReadoutInstrumentInverterLearnResults1.Gradient.Modify(2, 1.0, (ValueState) 2);
    this.digitalReadoutInstrumentInverterLearnResults1.Gradient.Modify(3, 2.0, (ValueState) 1);
    this.digitalReadoutInstrumentInverterLearnResults1.Gradient.Modify(4, 3.0, (ValueState) 3);
    this.digitalReadoutInstrumentInverterLearnResults1.Gradient.Modify(5, 4.0, (ValueState) 0);
    this.digitalReadoutInstrumentInverterLearnResults1.Gradient.Modify(6, (double) byte.MaxValue, (ValueState) 0);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentInverterLearnResults1).Instrument = new Qualifier((QualifierTypes) 64 /*0x40*/, "ECPC01T", "RT_TI_ResolverTeach_In_Request_Results_Resolver_tech_in_PT1");
    ((Control) this.digitalReadoutInstrumentInverterLearnResults1).Name = "digitalReadoutInstrumentInverterLearnResults1";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentInverterLearnResults1).ShowValueReadout = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentInverterLearnResults1).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.labelResolver1LearnRoutineHeader, "labelResolver1LearnRoutineHeader");
    this.labelResolver1LearnRoutineHeader.BorderStyle = BorderStyle.FixedSingle;
    ((TableLayoutPanel) this.tableLayoutPanelInverter1LearnRoutine).SetColumnSpan((Control) this.labelResolver1LearnRoutineHeader, 5);
    this.labelResolver1LearnRoutineHeader.Name = "labelResolver1LearnRoutineHeader";
    this.labelResolver1LearnRoutineHeader.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelInverter2LearnRoutine, "tableLayoutPanelInverter2LearnRoutine");
    ((TableLayoutPanel) this.tableLayoutPanelInverter2LearnRoutine).Controls.Add((Control) this.buttonInverter2Start, 4, 2);
    ((TableLayoutPanel) this.tableLayoutPanelInverter2LearnRoutine).Controls.Add((Control) this.labelStatusLabelInverter2, 2, 2);
    ((TableLayoutPanel) this.tableLayoutPanelInverter2LearnRoutine).Controls.Add((Control) this.labelStatus2, 1, 2);
    ((TableLayoutPanel) this.tableLayoutPanelInverter2LearnRoutine).Controls.Add((Control) this.checkmarkInverter2, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanelInverter2LearnRoutine).Controls.Add((Control) this.digitalReadoutInstrumentMotorSpeed2, 2, 1);
    ((TableLayoutPanel) this.tableLayoutPanelInverter2LearnRoutine).Controls.Add((Control) this.labelResolver2LearnRoutineHeader, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelInverter2LearnRoutine).Controls.Add((Control) this.digitalReadoutInstrumentInverterLearnResults2, 0, 1);
    ((Control) this.tableLayoutPanelInverter2LearnRoutine).Name = "tableLayoutPanelInverter2LearnRoutine";
    componentResourceManager.ApplyResources((object) this.buttonInverter2Start, "buttonInverter2Start");
    this.buttonInverter2Start.Name = "buttonInverter2Start";
    this.buttonInverter2Start.UseCompatibleTextRendering = true;
    this.buttonInverter2Start.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.labelStatusLabelInverter2, "labelStatusLabelInverter2");
    ((TableLayoutPanel) this.tableLayoutPanelInverter2LearnRoutine).SetColumnSpan((Control) this.labelStatusLabelInverter2, 2);
    this.labelStatusLabelInverter2.Name = "labelStatusLabelInverter2";
    this.labelStatusLabelInverter2.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.labelStatus2, "labelStatus2");
    this.labelStatus2.Name = "labelStatus2";
    this.labelStatus2.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.checkmarkInverter2, "checkmarkInverter2");
    ((Control) this.checkmarkInverter2).Name = "checkmarkInverter2";
    ((TableLayoutPanel) this.tableLayoutPanelInverter2LearnRoutine).SetColumnSpan((Control) this.digitalReadoutInstrumentMotorSpeed2, 2);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentMotorSpeed2, "digitalReadoutInstrumentMotorSpeed2");
    this.digitalReadoutInstrumentMotorSpeed2.FontGroup = "Instruments";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentMotorSpeed2).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentMotorSpeed2).Instrument = new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS125_Actual_E_Motor_Speed_E_Motor_2_Actual_E_Motor_Speed_E_Motor_2");
    ((Control) this.digitalReadoutInstrumentMotorSpeed2).Name = "digitalReadoutInstrumentMotorSpeed2";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentMotorSpeed2).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.labelResolver2LearnRoutineHeader, "labelResolver2LearnRoutineHeader");
    this.labelResolver2LearnRoutineHeader.BorderStyle = BorderStyle.FixedSingle;
    ((TableLayoutPanel) this.tableLayoutPanelInverter2LearnRoutine).SetColumnSpan((Control) this.labelResolver2LearnRoutineHeader, 5);
    this.labelResolver2LearnRoutineHeader.Name = "labelResolver2LearnRoutineHeader";
    this.labelResolver2LearnRoutineHeader.UseCompatibleTextRendering = true;
    ((TableLayoutPanel) this.tableLayoutPanelInverter2LearnRoutine).SetColumnSpan((Control) this.digitalReadoutInstrumentInverterLearnResults2, 3);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentInverterLearnResults2, "digitalReadoutInstrumentInverterLearnResults2");
    this.digitalReadoutInstrumentInverterLearnResults2.FontGroup = "Instruments";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentInverterLearnResults2).FreezeValue = false;
    this.digitalReadoutInstrumentInverterLearnResults2.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText7"));
    this.digitalReadoutInstrumentInverterLearnResults2.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText8"));
    this.digitalReadoutInstrumentInverterLearnResults2.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText9"));
    this.digitalReadoutInstrumentInverterLearnResults2.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText10"));
    this.digitalReadoutInstrumentInverterLearnResults2.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText11"));
    this.digitalReadoutInstrumentInverterLearnResults2.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText12"));
    this.digitalReadoutInstrumentInverterLearnResults2.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText13"));
    this.digitalReadoutInstrumentInverterLearnResults2.Gradient.Initialize((ValueState) 0, 6);
    this.digitalReadoutInstrumentInverterLearnResults2.Gradient.Modify(1, 0.0, (ValueState) 5);
    this.digitalReadoutInstrumentInverterLearnResults2.Gradient.Modify(2, 1.0, (ValueState) 2);
    this.digitalReadoutInstrumentInverterLearnResults2.Gradient.Modify(3, 2.0, (ValueState) 1);
    this.digitalReadoutInstrumentInverterLearnResults2.Gradient.Modify(4, 3.0, (ValueState) 3);
    this.digitalReadoutInstrumentInverterLearnResults2.Gradient.Modify(5, 4.0, (ValueState) 0);
    this.digitalReadoutInstrumentInverterLearnResults2.Gradient.Modify(6, (double) byte.MaxValue, (ValueState) 0);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentInverterLearnResults2).Instrument = new Qualifier((QualifierTypes) 64 /*0x40*/, "ECPC01T", "RT_TI_ResolverTeach_In_Request_Results_Resolver_tech_in_PT2");
    ((Control) this.digitalReadoutInstrumentInverterLearnResults2).Name = "digitalReadoutInstrumentInverterLearnResults2";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentInverterLearnResults2).ShowValueReadout = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentInverterLearnResults2).UnitAlignment = StringAlignment.Near;
    ((Control) this.tableLayoutPanelInverter3LearnRoutine).BackColor = SystemColors.Control;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelInverter3LearnRoutine, "tableLayoutPanelInverter3LearnRoutine");
    ((TableLayoutPanel) this.tableLayoutPanelInverter3LearnRoutine).Controls.Add((Control) this.buttonInverter3Start, 4, 2);
    ((TableLayoutPanel) this.tableLayoutPanelInverter3LearnRoutine).Controls.Add((Control) this.labelStatusLabelInverter3, 2, 2);
    ((TableLayoutPanel) this.tableLayoutPanelInverter3LearnRoutine).Controls.Add((Control) this.labelStatus3, 1, 2);
    ((TableLayoutPanel) this.tableLayoutPanelInverter3LearnRoutine).Controls.Add((Control) this.checkmarkInverter3, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanelInverter3LearnRoutine).Controls.Add((Control) this.digitalReadoutInstrumentMotorSpeed3, 3, 1);
    ((TableLayoutPanel) this.tableLayoutPanelInverter3LearnRoutine).Controls.Add((Control) this.digitalReadoutInstrumentInverterLearnResults3, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanelInverter3LearnRoutine).Controls.Add((Control) this.labelResolver3LearnRoutineHeader, 0, 0);
    ((Control) this.tableLayoutPanelInverter3LearnRoutine).Name = "tableLayoutPanelInverter3LearnRoutine";
    componentResourceManager.ApplyResources((object) this.buttonInverter3Start, "buttonInverter3Start");
    this.buttonInverter3Start.Name = "buttonInverter3Start";
    this.buttonInverter3Start.UseCompatibleTextRendering = true;
    this.buttonInverter3Start.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.labelStatusLabelInverter3, "labelStatusLabelInverter3");
    ((TableLayoutPanel) this.tableLayoutPanelInverter3LearnRoutine).SetColumnSpan((Control) this.labelStatusLabelInverter3, 2);
    this.labelStatusLabelInverter3.Name = "labelStatusLabelInverter3";
    this.labelStatusLabelInverter3.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.labelStatus3, "labelStatus3");
    this.labelStatus3.Name = "labelStatus3";
    this.labelStatus3.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.checkmarkInverter3, "checkmarkInverter3");
    ((Control) this.checkmarkInverter3).Name = "checkmarkInverter3";
    ((TableLayoutPanel) this.tableLayoutPanelInverter3LearnRoutine).SetColumnSpan((Control) this.digitalReadoutInstrumentMotorSpeed3, 2);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentMotorSpeed3, "digitalReadoutInstrumentMotorSpeed3");
    this.digitalReadoutInstrumentMotorSpeed3.FontGroup = "Instruments";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentMotorSpeed3).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentMotorSpeed3).Instrument = new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS126_Actual_E_Motor_Speed_E_Motor_3_Actual_E_Motor_Speed_E_Motor_3");
    ((Control) this.digitalReadoutInstrumentMotorSpeed3).Name = "digitalReadoutInstrumentMotorSpeed3";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentMotorSpeed3).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanelInverter3LearnRoutine).SetColumnSpan((Control) this.digitalReadoutInstrumentInverterLearnResults3, 3);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentInverterLearnResults3, "digitalReadoutInstrumentInverterLearnResults3");
    this.digitalReadoutInstrumentInverterLearnResults3.FontGroup = "Instruments";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentInverterLearnResults3).FreezeValue = false;
    this.digitalReadoutInstrumentInverterLearnResults3.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText14"));
    this.digitalReadoutInstrumentInverterLearnResults3.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText15"));
    this.digitalReadoutInstrumentInverterLearnResults3.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText16"));
    this.digitalReadoutInstrumentInverterLearnResults3.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText17"));
    this.digitalReadoutInstrumentInverterLearnResults3.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText18"));
    this.digitalReadoutInstrumentInverterLearnResults3.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText19"));
    this.digitalReadoutInstrumentInverterLearnResults3.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText20"));
    this.digitalReadoutInstrumentInverterLearnResults3.Gradient.Initialize((ValueState) 0, 6);
    this.digitalReadoutInstrumentInverterLearnResults3.Gradient.Modify(1, 0.0, (ValueState) 5);
    this.digitalReadoutInstrumentInverterLearnResults3.Gradient.Modify(2, 1.0, (ValueState) 2);
    this.digitalReadoutInstrumentInverterLearnResults3.Gradient.Modify(3, 2.0, (ValueState) 1);
    this.digitalReadoutInstrumentInverterLearnResults3.Gradient.Modify(4, 3.0, (ValueState) 3);
    this.digitalReadoutInstrumentInverterLearnResults3.Gradient.Modify(5, 4.0, (ValueState) 0);
    this.digitalReadoutInstrumentInverterLearnResults3.Gradient.Modify(6, (double) byte.MaxValue, (ValueState) 0);
    ((SingleInstrumentBase) this.digitalReadoutInstrumentInverterLearnResults3).Instrument = new Qualifier((QualifierTypes) 64 /*0x40*/, "ECPC01T", "RT_TI_ResolverTeach_In_Request_Results_Resolver_tech_in_PT3");
    ((Control) this.digitalReadoutInstrumentInverterLearnResults3).Name = "digitalReadoutInstrumentInverterLearnResults3";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentInverterLearnResults3).ShowValueReadout = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentInverterLearnResults3).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.labelResolver3LearnRoutineHeader, "labelResolver3LearnRoutineHeader");
    this.labelResolver3LearnRoutineHeader.BorderStyle = BorderStyle.FixedSingle;
    ((TableLayoutPanel) this.tableLayoutPanelInverter3LearnRoutine).SetColumnSpan((Control) this.labelResolver3LearnRoutineHeader, 5);
    this.labelResolver3LearnRoutineHeader.Name = "labelResolver3LearnRoutineHeader";
    this.labelResolver3LearnRoutineHeader.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelTop, "tableLayoutPanelTop");
    ((TableLayoutPanel) this.tableLayoutPanelTop).Controls.Add((Control) this.webBrowserWarning, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanelTop).Controls.Add((Control) this.pictureBox1, 0, 0);
    ((Control) this.tableLayoutPanelTop).Name = "tableLayoutPanelTop";
    componentResourceManager.ApplyResources((object) this.webBrowserWarning, "webBrowserWarning");
    this.webBrowserWarning.Name = "webBrowserWarning";
    this.webBrowserWarning.Url = new Uri("about: blank", UriKind.Absolute);
    this.pictureBox1.BackColor = Color.White;
    componentResourceManager.ApplyResources((object) this.pictureBox1, "pictureBox1");
    this.pictureBox1.Name = "pictureBox1";
    this.pictureBox1.TabStop = false;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelBottom, "tableLayoutPanelBottom");
    ((TableLayoutPanel) this.tableLayoutPanelBottom).Controls.Add((Control) this.seekTimeListView1, 1, 0);
    ((TableLayoutPanel) this.tableLayoutPanelBottom).Controls.Add((Control) this.sharedProcedureSelectionInverter3, 1, 3);
    ((TableLayoutPanel) this.tableLayoutPanelBottom).Controls.Add((Control) this.digitalReadoutInstrument4, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelBottom).Controls.Add((Control) this.sharedProcedureSelectionInverter2, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanelBottom).Controls.Add((Control) this.digitalReadoutInstrument5, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanelBottom).Controls.Add((Control) this.digitalReadoutInstrument6, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanelBottom).Controls.Add((Control) this.sharedProcedureSelectionInverter1, 2, 3);
    ((Control) this.tableLayoutPanelBottom).Name = "tableLayoutPanelBottom";
    ((TableLayoutPanel) this.tableLayoutPanelBottom).SetColumnSpan((Control) this.seekTimeListView1, 3);
    componentResourceManager.ApplyResources((object) this.seekTimeListView1, "seekTimeListView1");
    this.seekTimeListView1.FilterUserLabels = true;
    ((Control) this.seekTimeListView1).Name = "seekTimeListView1";
    this.seekTimeListView1.RequiredUserLabelPrefix = "InverterResolverLearn";
    ((TableLayoutPanel) this.tableLayoutPanelBottom).SetRowSpan((Control) this.seekTimeListView1, 3);
    this.seekTimeListView1.SelectedTime = new DateTime?();
    this.seekTimeListView1.ShowChannelLabels = false;
    this.seekTimeListView1.ShowCommunicationsState = false;
    this.seekTimeListView1.ShowControlPanel = false;
    componentResourceManager.ApplyResources((object) this.sharedProcedureSelectionInverter3, "sharedProcedureSelectionInverter3");
    ((Control) this.sharedProcedureSelectionInverter3).Name = "sharedProcedureSelectionInverter3";
    this.sharedProcedureSelectionInverter3.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>) new string[1]
    {
      "ResolverTeach_Inverter3"
    });
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument4, "digitalReadoutInstrument4");
    this.digitalReadoutInstrument4.FontGroup = "AllwaysDisplayedInstruments";
    ((SingleInstrumentBase) this.digitalReadoutInstrument4).FreezeValue = false;
    this.digitalReadoutInstrument4.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText21"));
    this.digitalReadoutInstrument4.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText22"));
    this.digitalReadoutInstrument4.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText23"));
    this.digitalReadoutInstrument4.Gradient.Initialize((ValueState) 0, 2);
    this.digitalReadoutInstrument4.Gradient.Modify(1, 0.0, (ValueState) 3);
    this.digitalReadoutInstrument4.Gradient.Modify(2, 1.0, (ValueState) 1);
    ((SingleInstrumentBase) this.digitalReadoutInstrument4).Instrument = new Qualifier((QualifierTypes) 1, "ETCM01T", "DT_Environmental_Conditions_RoutineControl_transmission_in_neutral");
    ((Control) this.digitalReadoutInstrument4).Name = "digitalReadoutInstrument4";
    ((SingleInstrumentBase) this.digitalReadoutInstrument4).ShowValueReadout = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument4).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.sharedProcedureSelectionInverter2, "sharedProcedureSelectionInverter2");
    ((Control) this.sharedProcedureSelectionInverter2).Name = "sharedProcedureSelectionInverter2";
    this.sharedProcedureSelectionInverter2.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>) new string[1]
    {
      "ResolverTeach_Inverter2"
    });
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument5, "digitalReadoutInstrument5");
    this.digitalReadoutInstrument5.FontGroup = "AllwaysDisplayedInstruments";
    ((SingleInstrumentBase) this.digitalReadoutInstrument5).FreezeValue = false;
    this.digitalReadoutInstrument5.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText24"));
    this.digitalReadoutInstrument5.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText25"));
    this.digitalReadoutInstrument5.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText26"));
    this.digitalReadoutInstrument5.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText27"));
    this.digitalReadoutInstrument5.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText28"));
    this.digitalReadoutInstrument5.Gradient.Initialize((ValueState) 0, 4);
    this.digitalReadoutInstrument5.Gradient.Modify(1, 0.0, (ValueState) 3);
    this.digitalReadoutInstrument5.Gradient.Modify(2, 1.0, (ValueState) 1);
    this.digitalReadoutInstrument5.Gradient.Modify(3, 2.0, (ValueState) 0);
    this.digitalReadoutInstrument5.Gradient.Modify(4, 3.0, (ValueState) 0);
    ((SingleInstrumentBase) this.digitalReadoutInstrument5).Instrument = new Qualifier((QualifierTypes) 1, "SSAM02T", "DT_BSC_Diagnostic_Displayables_DDBSC_PkBk_Master_Stat");
    ((Control) this.digitalReadoutInstrument5).Name = "digitalReadoutInstrument5";
    ((SingleInstrumentBase) this.digitalReadoutInstrument5).ShowValueReadout = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument5).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument6, "digitalReadoutInstrument6");
    this.digitalReadoutInstrument6.FontGroup = "AllwaysDisplayedInstruments";
    ((SingleInstrumentBase) this.digitalReadoutInstrument6).FreezeValue = false;
    this.digitalReadoutInstrument6.Gradient.Initialize((ValueState) 3, 2, "mph");
    this.digitalReadoutInstrument6.Gradient.Modify(1, 0.0, (ValueState) 1);
    this.digitalReadoutInstrument6.Gradient.Modify(2, 1.0, (ValueState) 3);
    ((SingleInstrumentBase) this.digitalReadoutInstrument6).Instrument = new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS012_CurrentVehicleSpeed_CurrentVehicleSpeed");
    ((Control) this.digitalReadoutInstrument6).Name = "digitalReadoutInstrument6";
    ((SingleInstrumentBase) this.digitalReadoutInstrument6).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.sharedProcedureSelectionInverter1, "sharedProcedureSelectionInverter1");
    ((Control) this.sharedProcedureSelectionInverter1).Name = "sharedProcedureSelectionInverter1";
    this.sharedProcedureSelectionInverter1.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>) new string[1]
    {
      "ResolverTeach_Inverter1"
    });
    this.sharedProcedureIntegrationComponentInverter1.ProceduresDropDown = this.sharedProcedureSelectionInverter1;
    this.sharedProcedureIntegrationComponentInverter1.ProcedureStatusMessageTarget = this.labelStatusLabelInverter1;
    this.sharedProcedureIntegrationComponentInverter1.ProcedureStatusStateTarget = this.checkmarkInverter1;
    this.sharedProcedureIntegrationComponentInverter1.ResultsTarget = (TextBoxBase) null;
    this.sharedProcedureIntegrationComponentInverter1.StartStopButton = this.buttonInverter1Start;
    this.sharedProcedureIntegrationComponentInverter1.StopAllButton = (Button) null;
    this.sharedProcedureCreatorComponentInverter1.Suspend();
    this.sharedProcedureCreatorComponentInverter1.MonitorCall = new ServiceCall("ECPC01T", "RT_TI_ResolverTeach_In_Request_Results_Resolver_tech_in_PT1");
    componentResourceManager.ApplyResources((object) this.sharedProcedureCreatorComponentInverter1, "sharedProcedureCreatorComponentInverter1");
    this.sharedProcedureCreatorComponentInverter1.Qualifier = "ResolverTeach_Inverter1";
    this.sharedProcedureCreatorComponentInverter1.StartCall = new ServiceCall("ECPC01T", "RT_TI_ResolverTeach_In_Start_Resolver_tech_in_PT1", (IEnumerable<string>) new string[3]
    {
      "Resolver_teach_in_Var1=1",
      "Resolver_teach_in_Var2=0",
      "Resolver_teach_in_Var3=0"
    });
    dataItemCondition1.Gradient.Initialize((ValueState) 3, 2, "mph");
    dataItemCondition1.Gradient.Modify(1, 0.0, (ValueState) 1);
    dataItemCondition1.Gradient.Modify(2, 1.0, (ValueState) 3);
    dataItemCondition1.Qualifier = new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS012_CurrentVehicleSpeed_CurrentVehicleSpeed");
    dataItemCondition2.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText29"));
    dataItemCondition2.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText30"));
    dataItemCondition2.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText31"));
    dataItemCondition2.Gradient.Initialize((ValueState) 0, 2);
    dataItemCondition2.Gradient.Modify(1, 0.0, (ValueState) 3);
    dataItemCondition2.Gradient.Modify(2, 1.0, (ValueState) 1);
    dataItemCondition2.Qualifier = new Qualifier((QualifierTypes) 1, "ETCM01T", "DT_Environmental_Conditions_RoutineControl_transmission_in_neutral");
    this.sharedProcedureCreatorComponentInverter1.StartConditions.Add(dataItemCondition1);
    this.sharedProcedureCreatorComponentInverter1.StartConditions.Add(dataItemCondition2);
    this.sharedProcedureCreatorComponentInverter1.StopCall = new ServiceCall("ECPC01T", "RT_TI_ResolverTeach_In_Stop_Resolver_tech_in_PT1", (IEnumerable<string>) new string[3]
    {
      "Resolver_teach_in_Var1=1",
      "Resolver_teach_in_Var2=0",
      "Resolver_teach_in_Var3=0"
    });
    this.sharedProcedureCreatorComponentInverter1.StartServiceComplete += new EventHandler<SingleServiceResultEventArgs>(this.sharedProcedureCreatorComponentInverter1_StartServiceComplete);
    this.sharedProcedureCreatorComponentInverter1.StopServiceComplete += new EventHandler<SingleServiceResultEventArgs>(this.sharedProcedureCreatorComponentInverter1_StopServiceComplete);
    this.sharedProcedureCreatorComponentInverter1.Resume();
    this.sharedProcedureIntegrationComponentInverter2.ProceduresDropDown = this.sharedProcedureSelectionInverter2;
    this.sharedProcedureIntegrationComponentInverter2.ProcedureStatusMessageTarget = this.labelStatusLabelInverter2;
    this.sharedProcedureIntegrationComponentInverter2.ProcedureStatusStateTarget = this.checkmarkInverter2;
    this.sharedProcedureIntegrationComponentInverter2.ResultsTarget = (TextBoxBase) null;
    this.sharedProcedureIntegrationComponentInverter2.StartStopButton = this.buttonInverter2Start;
    this.sharedProcedureIntegrationComponentInverter2.StopAllButton = (Button) null;
    this.sharedProcedureCreatorComponentInverter2.Suspend();
    this.sharedProcedureCreatorComponentInverter2.MonitorCall = new ServiceCall("ECPC01T", "RT_TI_ResolverTeach_In_Request_Results_Resolver_tech_in_PT2");
    componentResourceManager.ApplyResources((object) this.sharedProcedureCreatorComponentInverter2, "sharedProcedureCreatorComponentInverter2");
    this.sharedProcedureCreatorComponentInverter2.Qualifier = "ResolverTeach_Inverter2";
    this.sharedProcedureCreatorComponentInverter2.StartCall = new ServiceCall("ECPC01T", "RT_TI_ResolverTeach_In_Start_Resolver_tech_in_PT2", (IEnumerable<string>) new string[3]
    {
      "Resolver_teach_in_Var1=0",
      "Resolver_teach_in_Var2=1",
      "Resolver_teach_in_Var3=0"
    });
    dataItemCondition3.Gradient.Initialize((ValueState) 3, 2, "mph");
    dataItemCondition3.Gradient.Modify(1, 0.0, (ValueState) 1);
    dataItemCondition3.Gradient.Modify(2, 1.0, (ValueState) 3);
    dataItemCondition3.Qualifier = new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS012_CurrentVehicleSpeed_CurrentVehicleSpeed");
    dataItemCondition4.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText32"));
    dataItemCondition4.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText33"));
    dataItemCondition4.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText34"));
    dataItemCondition4.Gradient.Initialize((ValueState) 0, 2);
    dataItemCondition4.Gradient.Modify(1, 0.0, (ValueState) 3);
    dataItemCondition4.Gradient.Modify(2, 1.0, (ValueState) 1);
    dataItemCondition4.Qualifier = new Qualifier((QualifierTypes) 1, "ETCM01T", "DT_Environmental_Conditions_RoutineControl_transmission_in_neutral");
    this.sharedProcedureCreatorComponentInverter2.StartConditions.Add(dataItemCondition3);
    this.sharedProcedureCreatorComponentInverter2.StartConditions.Add(dataItemCondition4);
    this.sharedProcedureCreatorComponentInverter2.StopCall = new ServiceCall("ECPC01T", "RT_TI_ResolverTeach_In_Stop_Resolver_tech_in_PT2", (IEnumerable<string>) new string[3]
    {
      "Resolver_teach_in_Var1=0",
      "Resolver_teach_in_Var2=1",
      "Resolver_teach_in_Var3=0"
    });
    this.sharedProcedureCreatorComponentInverter2.StartServiceComplete += new EventHandler<SingleServiceResultEventArgs>(this.sharedProcedureCreatorComponentInverter2_StartServiceComplete);
    this.sharedProcedureCreatorComponentInverter2.StopServiceComplete += new EventHandler<SingleServiceResultEventArgs>(this.sharedProcedureCreatorComponentInverter2_StopServiceComplete);
    this.sharedProcedureCreatorComponentInverter2.Resume();
    this.sharedProcedureIntegrationComponentInverter3.ProceduresDropDown = this.sharedProcedureSelectionInverter3;
    this.sharedProcedureIntegrationComponentInverter3.ProcedureStatusMessageTarget = this.labelStatusLabelInverter3;
    this.sharedProcedureIntegrationComponentInverter3.ProcedureStatusStateTarget = this.checkmarkInverter3;
    this.sharedProcedureIntegrationComponentInverter3.ResultsTarget = (TextBoxBase) null;
    this.sharedProcedureIntegrationComponentInverter3.StartStopButton = this.buttonInverter3Start;
    this.sharedProcedureIntegrationComponentInverter3.StopAllButton = (Button) null;
    this.sharedProcedureCreatorComponentInverter3.Suspend();
    this.sharedProcedureCreatorComponentInverter3.MonitorCall = new ServiceCall("ECPC01T", "RT_TI_ResolverTeach_In_Request_Results_Resolver_tech_in_PT3");
    componentResourceManager.ApplyResources((object) this.sharedProcedureCreatorComponentInverter3, "sharedProcedureCreatorComponentInverter3");
    this.sharedProcedureCreatorComponentInverter3.Qualifier = "ResolverTeach_Inverter3";
    this.sharedProcedureCreatorComponentInverter3.StartCall = new ServiceCall("ECPC01T", "RT_TI_ResolverTeach_In_Start_Resolver_tech_in_PT3", (IEnumerable<string>) new string[3]
    {
      "Resolver_teach_in_Var1=0",
      "Resolver_teach_in_Var2=0",
      "Resolver_teach_in_Var3=1"
    });
    dataItemCondition5.Gradient.Initialize((ValueState) 3, 2, "mph");
    dataItemCondition5.Gradient.Modify(1, 0.0, (ValueState) 1);
    dataItemCondition5.Gradient.Modify(2, 1.0, (ValueState) 3);
    dataItemCondition5.Qualifier = new Qualifier((QualifierTypes) 1, "ECPC01T", "DT_AS012_CurrentVehicleSpeed_CurrentVehicleSpeed");
    dataItemCondition6.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText35"));
    dataItemCondition6.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText36"));
    dataItemCondition6.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText37"));
    dataItemCondition6.Gradient.Initialize((ValueState) 0, 2);
    dataItemCondition6.Gradient.Modify(1, 0.0, (ValueState) 3);
    dataItemCondition6.Gradient.Modify(2, 1.0, (ValueState) 1);
    dataItemCondition6.Qualifier = new Qualifier((QualifierTypes) 1, "ETCM01T", "DT_Environmental_Conditions_RoutineControl_transmission_in_neutral");
    this.sharedProcedureCreatorComponentInverter3.StartConditions.Add(dataItemCondition5);
    this.sharedProcedureCreatorComponentInverter3.StartConditions.Add(dataItemCondition6);
    this.sharedProcedureCreatorComponentInverter3.StopCall = new ServiceCall("ECPC01T", "RT_TI_ResolverTeach_In_Stop_Resolver_tech_in_PT3", (IEnumerable<string>) new string[3]
    {
      "Resolver_teach_in_Var1=0",
      "Resolver_teach_in_Var2=0",
      "Resolver_teach_in_Var3=1"
    });
    this.sharedProcedureCreatorComponentInverter3.StartServiceComplete += new EventHandler<SingleServiceResultEventArgs>(this.sharedProcedureCreatorComponentInverter3_StartServiceComplete);
    this.sharedProcedureCreatorComponentInverter3.StopServiceComplete += new EventHandler<SingleServiceResultEventArgs>(this.sharedProcedureCreatorComponentInverter3_StopServiceComplete);
    this.sharedProcedureCreatorComponentInverter3.Resume();
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("_DDDL.chm_Inverter_Resolver_Learn");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel1);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this.tableLayoutPanelInverter1LearnRoutine).ResumeLayout(false);
    ((Control) this.tableLayoutPanelInverter1LearnRoutine).PerformLayout();
    ((Control) this.tableLayoutPanelInverter2LearnRoutine).ResumeLayout(false);
    ((Control) this.tableLayoutPanelInverter2LearnRoutine).PerformLayout();
    ((Control) this.tableLayoutPanelInverter3LearnRoutine).ResumeLayout(false);
    ((Control) this.tableLayoutPanelInverter3LearnRoutine).PerformLayout();
    ((Control) this.tableLayoutPanelTop).ResumeLayout(false);
    ((ISupportInitialize) this.pictureBox1).EndInit();
    ((Control) this.tableLayoutPanelBottom).ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }
}
