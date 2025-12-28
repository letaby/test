// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Tests.Cylinder_Cutout_Test__Automatic_.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Tests.Cylinder_Cutout_Test__Automatic_.panel;

public class UserPanel : CustomPanel
{
  private const string EngineSpeedInstrumentQualifier = "DT_AS010_Engine_Speed";
  private const string ActualTorqueQualifier = "DT_AS003_Actual_Torque";
  private const double RequiredEngineSpeed = 1000.0;
  private static string instructionText = Resources.Message_ToProceed;
  private static string DpfZoneTestConditionNotMet = Resources.Message_TheVehicleNeedsToBeInZone0PerformAParkedRegenerationAndTryAgain;
  private static string DpfZoneTestConditionMet = Resources.Message_TheDPFZoneIsZero;
  private readonly ReadOnlyCollection<CylinderGroup> cylinderGroups;
  private Channel channel;
  private Instrument dPfZoneInstrument;
  private Dictionary<string, CacheInfo> snapshot;
  private DateTime initialTimeBeforeCylindercut;
  private UserPanel.State state = UserPanel.State.Start;
  private Timer timer = new Timer();
  private bool adrReturnValue = true;
  private int currentGroupIndex = -1;
  private static readonly TimeSpan AverageSampleSpan = new TimeSpan(0, 0, 3);
  private ScalingLabel ScalingLabel5;
  private ScalingLabel ScalingLabel4;
  private ScalingLabel ScalingLabel3;
  private ScalingLabel ScalingLabel2;
  private ScalingLabel ScalingLabel1;
  private ChartInstrument ChartInstrument1;
  private DigitalReadoutInstrument DigitalReadoutInstrument3;
  private DigitalReadoutInstrument DigitalReadoutInstrument2;
  private DigitalReadoutInstrument DigitalReadoutInstrument1;
  private DigitalReadoutInstrument DigitalReadoutInstrument4;
  private TableLayoutPanel tableLayoutPanelBase;
  private TabControl tabControl2;
  private TabPage tabPage3;
  private TableLayoutPanel tableLayoutPanel2;
  private TabPage tabPage4;
  private Button buttonCancel;
  private Button buttonExecute;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label label1;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label label8;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label label9;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label label10;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label label11;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label label12;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label labelCheckmarkDpfZoneZero;
  private Checkmark checkmarkDpfZone;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label unitsLabel1and6;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label unitsLabel12and3;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label unitsLabel45and6;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label unitsLabel2and5;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label unitsLabel3and4;
  private TextBox textboxResults;

  public UserPanel()
  {
    WarningManager.SetJobName(Resources.Message_CylinderCutoutTest);
    this.InitializeComponent();
    this.timer.Interval = 1000;
    this.timer.Tick += new EventHandler(this.Advance);
    this.buttonExecute.Click += new EventHandler(this.OnExecuteButtonClick);
    this.buttonCancel.Click += new EventHandler(this.OnCancelButtonClick);
    this.cylinderGroups = new List<CylinderGroup>((IEnumerable<CylinderGroup>) new CylinderGroup[5]
    {
      new CylinderGroup((IEnumerable<int>) new int[2]
      {
        1,
        6
      }, this.ScalingLabel1, this.unitsLabel1and6),
      new CylinderGroup((IEnumerable<int>) new int[2]
      {
        2,
        5
      }, this.ScalingLabel2, this.unitsLabel2and5),
      new CylinderGroup((IEnumerable<int>) new int[2]
      {
        3,
        4
      }, this.ScalingLabel3, this.unitsLabel3and4),
      new CylinderGroup((IEnumerable<int>) new int[3]
      {
        1,
        2,
        3
      }, this.ScalingLabel4, this.unitsLabel12and3),
      new CylinderGroup((IEnumerable<int>) new int[3]
      {
        4,
        5,
        6
      }, this.ScalingLabel5, this.unitsLabel45and6)
    }).AsReadOnly();
  }

  private DateTime EndOfInitialIdleTimeBeforeCylinderCut
  {
    get => this.initialTimeBeforeCylindercut + new TimeSpan(0, 0, 5);
  }

  public virtual bool CanProvideHtml
  {
    get => this.state == UserPanel.State.Start && this.textboxResults.TextLength > 0;
  }

  public virtual string ToHtml()
  {
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.Append("<table>");
    stringBuilder.Append($"<tr><td class=\"group\">{Resources.Message_TorqueLostWhileCuttingCylinders}</td><td class=\"standard\"></td></tr>");
    foreach (CylinderGroup cylinderGroup in this.cylinderGroups)
    {
      string str = cylinderGroup.Result.HasValue ? cylinderGroup.ResultAsString : "&lt;No Value&gt;";
      stringBuilder.AppendFormat($"<tr><td class=\"standard\">{Resources.MessageFormat_Cylinder0}</td><td class=\"standard\">{{1}}</td></tr>", (object) cylinderGroup.GetCuttingStatusText(), (object) str);
    }
    stringBuilder.Append($"<tr><td class=\"group\">{Resources.Message_History}</td><td class=\"standard\"></td></tr>");
    foreach (string line in this.textboxResults.Lines)
      stringBuilder.AppendFormat("<tr><td class=\"standard\">{0}</td><td class=\"standard\"></td></tr>", (object) line);
    stringBuilder.Append("</table>");
    return stringBuilder.ToString();
  }

  protected virtual void OnLoad(EventArgs e)
  {
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
    ((ContainerControl) this).ParentForm.FormClosing += new FormClosingEventHandler(this.OnParentFormClosing);
  }

  private void OnParentFormClosing(object sender, FormClosingEventArgs e)
  {
    ((Control) this).Tag = (object) new object[2]
    {
      (object) this.adrReturnValue,
      (object) this.textboxResults.Text
    };
  }

  private void OnExecuteButtonClick(object sender, EventArgs e)
  {
    if (!WarningManager.RequestContinue() || DialogResult.OK != MessageBox.Show(UserPanel.instructionText, ApplicationInformation.ProductName, MessageBoxButtons.OKCancel, MessageBoxIcon.None, MessageBoxDefaultButton.Button2))
      return;
    this.timer.Start();
    this.Advance();
  }

  private void OnCancelButtonClick(object sender, EventArgs e)
  {
    this.state = UserPanel.State.End;
    this.WriteLine(Resources.Message_CancelingTest);
    this.Advance();
    this.WriteLine(Resources.Message_TheTestWasCancelledByTheUser);
  }

  private void Advance(object sender, EventArgs e) => this.Advance();

  private void Advance()
  {
    bool flag = false;
    switch (this.state)
    {
      case UserPanel.State.Start:
        this.currentGroupIndex = -1;
        flag = true;
        this.ClearResults();
        this.SetupInstruments();
        this.state = UserPanel.State.ChangeIdleSpeed;
        break;
      case UserPanel.State.ChangeIdleSpeed:
        flag = this.SetIdleStart(1000.0);
        if (flag)
        {
          this.state = UserPanel.State.WaitForIdleSpeed;
          break;
        }
        break;
      case UserPanel.State.WaitForIdleSpeed:
        if (this.channel != null)
        {
          Instrument instrument = this.channel.Instruments["DT_AS010_Engine_Speed"];
          if (instrument != (Instrument) null && instrument.InstrumentValues.Current != null && instrument.InstrumentValues.Current.Value != null)
          {
            try
            {
              double num = Convert.ToDouble(instrument.InstrumentValues.Current.Value);
              flag = true;
              if (num >= 1000.0)
                this.state = UserPanel.State.Init;
            }
            catch (InvalidCastException ex)
            {
            }
          }
          break;
        }
        break;
      case UserPanel.State.Init:
        this.initialTimeBeforeCylindercut = DateTime.Now;
        this.WriteLine(Resources.Message_Idle);
        this.state = UserPanel.State.WaitToGetBaseline;
        flag = true;
        break;
      case UserPanel.State.WaitToGetBaseline:
        flag = true;
        DateTime now = DateTime.Now;
        if (this.currentGroupIndex >= 0)
        {
          if (now >= this.cylinderGroups[this.currentGroupIndex].EndOfWaitTimeAfterGroupWasTurnedOn)
          {
            this.state = UserPanel.State.CylinderOff;
            break;
          }
          break;
        }
        if (now >= this.EndOfInitialIdleTimeBeforeCylinderCut)
        {
          this.state = UserPanel.State.CylinderOff;
          break;
        }
        break;
      case UserPanel.State.CylinderOff:
        ++this.currentGroupIndex;
        if (this.currentGroupIndex < this.cylinderGroups.Count)
        {
          this.WriteLine(string.Format(Resources.MessageFormat_CuttingCylinders0, (object) this.cylinderGroups[this.currentGroupIndex].GetCuttingStatusText()));
          flag = this.SwitchCylindersOff(this.cylinderGroups[this.currentGroupIndex]);
          this.cylinderGroups[this.currentGroupIndex].TimeGroupWasTurnedOff = DateTime.Now;
          this.state = UserPanel.State.WaitWhileCylinderOff;
          break;
        }
        flag = false;
        break;
      case UserPanel.State.WaitWhileCylinderOff:
        flag = true;
        if (DateTime.Now >= this.cylinderGroups[this.currentGroupIndex].EndOfWaitTimeAfterGroupWasTurnedOff)
        {
          this.state = UserPanel.State.CylinderOn;
          break;
        }
        break;
      case UserPanel.State.CylinderOn:
        this.WriteLine(Resources.Message_Idle);
        flag = this.SwitchAllCylindersOn();
        this.cylinderGroups[this.currentGroupIndex].TimeGroupWasTurnedOn = DateTime.Now;
        this.state = this.currentGroupIndex != this.cylinderGroups.Count - 1 ? UserPanel.State.WaitToGetBaseline : UserPanel.State.End;
        break;
      case UserPanel.State.End:
        flag = this.SwitchAllCylindersOn();
        this.state = UserPanel.State.Start;
        this.currentGroupIndex = -1;
        this.timer.Stop();
        this.SetIdleStop();
        this.DisplayResults();
        this.RestoreInstruments();
        this.WriteLine(Resources.Message_CylinderCutoutTestCompleted);
        break;
    }
    if (!flag)
    {
      this.adrReturnValue = false;
      if (this.state != UserPanel.State.Start)
      {
        this.WriteLine(Resources.Message_AnErrorOccurredTheTestWillTerminateEarly);
        if (this.state != UserPanel.State.ChangeIdleSpeed || this.state != UserPanel.State.WaitForIdleSpeed)
          this.SwitchAllCylindersOn();
        this.state = UserPanel.State.End;
      }
    }
    this.UpdateButtonState();
  }

  private bool SwitchCylindersOff(CylinderGroup group)
  {
    bool flag = false;
    if (this.channel != null)
    {
      try
      {
        flag = this.channel.Services.Execute(group.GetServiceExecuteList(), true) == group.Cylinders.Count;
      }
      catch (CaesarException ex)
      {
        this.WriteLine(ex.Message);
      }
    }
    return flag;
  }

  private bool SwitchAllCylindersOn()
  {
    bool flag = false;
    Service service = this.GetService("MCM", "RT_SR004_Engine_Cylinder_Cut_Off_Stop");
    if (service != (Service) null)
      flag = CustomPanel.ExecuteService(service);
    return flag;
  }

  public virtual void OnChannelsChanged()
  {
    WarningManager.Reset();
    this.SetChannel(this.GetChannel("MCM"));
    this.UpdateButtonState();
  }

  private void OnDpfZoneInstrumentUpdateEvent(object sender, ResultEventArgs e)
  {
    this.UpdateButtonState();
  }

  private void SetChannel(Channel channel)
  {
    if (this.channel == channel)
      return;
    if (this.channel != null)
    {
      this.channel.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnChannelCommunicationsStateUpdateEvent);
      if (this.dPfZoneInstrument != (Instrument) null)
        this.dPfZoneInstrument.InstrumentUpdateEvent -= new InstrumentUpdateEventHandler(this.OnDpfZoneInstrumentUpdateEvent);
    }
    this.channel = channel;
    if (this.channel != null)
    {
      this.channel.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnChannelCommunicationsStateUpdateEvent);
      this.dPfZoneInstrument = this.channel.Instruments["DT_AS072_DPF_Zone"];
      if (this.dPfZoneInstrument != (Instrument) null)
        this.dPfZoneInstrument.InstrumentUpdateEvent += new InstrumentUpdateEventHandler(this.OnDpfZoneInstrumentUpdateEvent);
    }
  }

  private void OnChannelCommunicationsStateUpdateEvent(
    object sender,
    CommunicationsStateEventArgs e)
  {
    this.UpdateButtonState();
  }

  private bool InDpfZoneZero
  {
    get
    {
      bool inDpfZoneZero = false;
      if (this.dPfZoneInstrument != (Instrument) null && this.dPfZoneInstrument.InstrumentValues != null && this.dPfZoneInstrument.InstrumentValues.Current != null && this.dPfZoneInstrument.InstrumentValues.Current.Value == (object) this.dPfZoneInstrument.Choices.GetItemFromRawValue((object) 0))
        inDpfZoneZero = true;
      return inDpfZoneZero;
    }
  }

  private void UpdateButtonState()
  {
    bool flag1 = false;
    bool flag2 = false;
    this.checkmarkDpfZone.Checked = this.InDpfZoneZero;
    ((Control) this.labelCheckmarkDpfZoneZero).Text = this.checkmarkDpfZone.Checked ? UserPanel.DpfZoneTestConditionMet : UserPanel.DpfZoneTestConditionNotMet;
    if (this.channel != null && this.channel.Instruments["DT_AS010_Engine_Speed"] != (Instrument) null)
    {
      if (this.state == UserPanel.State.Start)
      {
        if (this.checkmarkDpfZone.Checked)
          flag1 = true;
      }
      else
        flag2 = true;
    }
    else
    {
      this.state = UserPanel.State.Start;
      this.timer.Stop();
    }
    this.buttonExecute.Enabled = flag1;
    this.buttonCancel.Enabled = flag2;
  }

  private void ClearResults()
  {
    foreach (CylinderGroup cylinderGroup in this.cylinderGroups)
      cylinderGroup.SetResult(new double?(), string.Empty);
    this.textboxResults.Text = string.Empty;
  }

  private void DisplayResults()
  {
    Instrument instrument = this.GetInstrument("MCM", "DT_AS003_Actual_Torque");
    if (!(instrument != (Instrument) null))
      return;
    List<double?> nullableList = new List<double?>();
    SpatialAverageCalculator averageCalculator = new SpatialAverageCalculator(instrument);
    DateTime dateTime = this.initialTimeBeforeCylindercut;
    foreach (CylinderGroup cylinderGroup in this.cylinderGroups)
    {
      double average1 = averageCalculator.GetAverage(dateTime + UserPanel.AverageSampleSpan, cylinderGroup.TimeGroupWasTurnedOff);
      double average2 = averageCalculator.GetAverage(cylinderGroup.TimeGroupWasTurnedOff + UserPanel.AverageSampleSpan, cylinderGroup.TimeGroupWasTurnedOn);
      nullableList.Add(new double?(average2 - average1));
      dateTime = cylinderGroup.TimeGroupWasTurnedOn;
    }
    for (int index = 0; index < this.cylinderGroups.Count; ++index)
      this.cylinderGroups[index].SetResult(nullableList[index], instrument.Units);
  }

  private bool SetIdleStart(double idle)
  {
    bool flag = false;
    Service service = this.GetService("MCM", "RT_SR015_Idle_Speed_Modification_Start");
    if (service != (Service) null)
    {
      this.WriteLine(Resources.Message_SettingRPM);
      service.InputValues[0].Value = (object) idle;
      flag = CustomPanel.ExecuteService(service);
    }
    return flag;
  }

  private bool SetIdleStop()
  {
    bool flag = false;
    Service service = this.GetService("MCM", "RT_SR015_Idle_Speed_Modification_Stop");
    if (service != (Service) null)
      flag = CustomPanel.ExecuteService(service);
    return flag;
  }

  private void SetupInstruments()
  {
    if (this.channel == null)
      return;
    this.snapshot = InstrumentCacheManager.GenerateSnapshot(this.channel);
    InstrumentCacheManager.UnmarkAllInstruments(this.channel);
    InstrumentCacheManager.MarkInstrument(this.channel, "DT_AS003_Actual_Torque", (ushort) 10);
    InstrumentCacheManager.MarkInstrument(this.channel, "DT_AS010_Engine_Speed", (ushort) 10);
    InstrumentCacheManager.MarkInstrument(this.channel, "DT_AS013_Coolant_Temperature", (ushort) 10);
    InstrumentCacheManager.MarkInstrument(this.channel, "DT_AS032_EGR_Actual_Valve_Position", (ushort) 10);
    InstrumentCacheManager.MarkInstrument(this.channel, "DT_AS034_Throttle_Valve_Actual_Position", (ushort) 10);
    InstrumentCacheManager.MarkInstrument(this.channel, "DT_AS117_Percentage_of_current_VGT_position", (ushort) 10);
  }

  private void RestoreInstruments()
  {
    if (this.channel == null)
      return;
    InstrumentCacheManager.ApplySnapshot(this.channel, this.snapshot);
  }

  private void WriteLine(string text)
  {
    if (this.textboxResults != null)
    {
      StringBuilder stringBuilder = new StringBuilder(this.textboxResults.Text);
      stringBuilder.Append(text);
      stringBuilder.Append("\r\n");
      this.textboxResults.Text = stringBuilder.ToString();
      this.textboxResults.SelectionStart = this.textboxResults.TextLength;
      this.textboxResults.SelectionLength = 0;
      this.textboxResults.ScrollToCaret();
    }
    this.AddStatusMessage(text);
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanelBase = new TableLayoutPanel();
    this.DigitalReadoutInstrument4 = new DigitalReadoutInstrument();
    this.DigitalReadoutInstrument1 = new DigitalReadoutInstrument();
    this.DigitalReadoutInstrument2 = new DigitalReadoutInstrument();
    this.DigitalReadoutInstrument3 = new DigitalReadoutInstrument();
    this.tabControl2 = new TabControl();
    this.tabPage4 = new TabPage();
    this.ChartInstrument1 = new ChartInstrument();
    this.tabPage3 = new TabPage();
    this.tableLayoutPanel2 = new TableLayoutPanel();
    this.ScalingLabel3 = new ScalingLabel();
    this.ScalingLabel2 = new ScalingLabel();
    this.ScalingLabel1 = new ScalingLabel();
    this.ScalingLabel4 = new ScalingLabel();
    this.ScalingLabel5 = new ScalingLabel();
    this.label1 = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.label8 = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.label9 = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.label10 = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.label11 = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.label12 = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.unitsLabel1and6 = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.unitsLabel12and3 = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.unitsLabel45and6 = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.unitsLabel2and5 = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.unitsLabel3and4 = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.textboxResults = new TextBox();
    this.buttonCancel = new Button();
    this.buttonExecute = new Button();
    this.labelCheckmarkDpfZoneZero = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.checkmarkDpfZone = new Checkmark();
    ((Control) this.tableLayoutPanelBase).SuspendLayout();
    this.tabControl2.SuspendLayout();
    this.tabPage4.SuspendLayout();
    this.tabPage3.SuspendLayout();
    ((Control) this.tableLayoutPanel2).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanelBase, "tableLayoutPanelBase");
    ((TableLayoutPanel) this.tableLayoutPanelBase).Controls.Add((Control) this.DigitalReadoutInstrument4, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanelBase).Controls.Add((Control) this.DigitalReadoutInstrument1, 2, 1);
    ((TableLayoutPanel) this.tableLayoutPanelBase).Controls.Add((Control) this.DigitalReadoutInstrument2, 3, 1);
    ((TableLayoutPanel) this.tableLayoutPanelBase).Controls.Add((Control) this.DigitalReadoutInstrument3, 4, 1);
    ((TableLayoutPanel) this.tableLayoutPanelBase).Controls.Add((Control) this.tabControl2, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanelBase).Controls.Add((Control) this.textboxResults, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanelBase).Controls.Add((Control) this.buttonCancel, 4, 3);
    ((TableLayoutPanel) this.tableLayoutPanelBase).Controls.Add((Control) this.buttonExecute, 3, 3);
    ((TableLayoutPanel) this.tableLayoutPanelBase).Controls.Add((Control) this.labelCheckmarkDpfZoneZero, 1, 3);
    ((TableLayoutPanel) this.tableLayoutPanelBase).Controls.Add((Control) this.checkmarkDpfZone, 0, 3);
    ((Control) this.tableLayoutPanelBase).Name = "tableLayoutPanelBase";
    ((TableLayoutPanel) this.tableLayoutPanelBase).SetColumnSpan((Control) this.DigitalReadoutInstrument4, 2);
    componentResourceManager.ApplyResources((object) this.DigitalReadoutInstrument4, "DigitalReadoutInstrument4");
    this.DigitalReadoutInstrument4.FontGroup = (string) null;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument4).FreezeValue = false;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument4).Instrument = new Qualifier((QualifierTypes) 1, "MCM", "DT_DS019_Vehicle_Check_Status");
    ((Control) this.DigitalReadoutInstrument4).Name = "DigitalReadoutInstrument4";
    ((SingleInstrumentBase) this.DigitalReadoutInstrument4).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.DigitalReadoutInstrument1, "DigitalReadoutInstrument1");
    this.DigitalReadoutInstrument1.FontGroup = (string) null;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument1).FreezeValue = false;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes) 1, "MCM", "DT_AS013_Coolant_Temperature");
    ((Control) this.DigitalReadoutInstrument1).Name = "DigitalReadoutInstrument1";
    ((SingleInstrumentBase) this.DigitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.DigitalReadoutInstrument2, "DigitalReadoutInstrument2");
    this.DigitalReadoutInstrument2.FontGroup = (string) null;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument2).FreezeValue = false;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes) 1, "MCM", "DT_AS010_Engine_Speed");
    ((Control) this.DigitalReadoutInstrument2).Name = "DigitalReadoutInstrument2";
    ((SingleInstrumentBase) this.DigitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.DigitalReadoutInstrument3, "DigitalReadoutInstrument3");
    this.DigitalReadoutInstrument3.FontGroup = (string) null;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument3).FreezeValue = false;
    ((SingleInstrumentBase) this.DigitalReadoutInstrument3).Instrument = new Qualifier((QualifierTypes) 1, "MCM", "DT_AS003_Actual_Torque");
    ((Control) this.DigitalReadoutInstrument3).Name = "DigitalReadoutInstrument3";
    ((SingleInstrumentBase) this.DigitalReadoutInstrument3).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.tabControl2, "tabControl2");
    ((TableLayoutPanel) this.tableLayoutPanelBase).SetColumnSpan((Control) this.tabControl2, 5);
    this.tabControl2.Controls.Add((Control) this.tabPage4);
    this.tabControl2.Controls.Add((Control) this.tabPage3);
    this.tabControl2.Name = "tabControl2";
    this.tabControl2.SelectedIndex = 0;
    this.tabPage4.Controls.Add((Control) this.ChartInstrument1);
    componentResourceManager.ApplyResources((object) this.tabPage4, "tabPage4");
    this.tabPage4.Name = "tabPage4";
    this.tabPage4.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.ChartInstrument1, "ChartInstrument1");
    ((Collection<Qualifier>) this.ChartInstrument1.Instruments).Add(new Qualifier((QualifierTypes) 1, "MCM", "DT_AS003_Actual_Torque"));
    ((Control) this.ChartInstrument1).Name = "ChartInstrument1";
    this.ChartInstrument1.SelectedTime = new DateTime?();
    this.ChartInstrument1.ShowEvents = false;
    this.ChartInstrument1.ShowLegend = false;
    this.tabPage3.Controls.Add((Control) this.tableLayoutPanel2);
    componentResourceManager.ApplyResources((object) this.tabPage3, "tabPage3");
    this.tabPage3.Name = "tabPage3";
    this.tabPage3.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel2, "tableLayoutPanel2");
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.ScalingLabel3, 4, 2);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.ScalingLabel2, 2, 2);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.ScalingLabel1, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.ScalingLabel4, 0, 4);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.ScalingLabel5, 2, 4);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.label1, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.label8, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.label9, 2, 1);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.label10, 4, 1);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.label11, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.label12, 2, 3);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.unitsLabel1and6, 1, 1);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.unitsLabel12and3, 1, 3);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.unitsLabel45and6, 3, 3);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.unitsLabel2and5, 3, 1);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.unitsLabel3and4, 5, 1);
    ((Control) this.tableLayoutPanel2).Name = "tableLayoutPanel2";
    this.ScalingLabel3.Alignment = StringAlignment.Far;
    ((TableLayoutPanel) this.tableLayoutPanel2).SetColumnSpan((Control) this.ScalingLabel3, 2);
    componentResourceManager.ApplyResources((object) this.ScalingLabel3, "ScalingLabel3");
    this.ScalingLabel3.FontGroup = "AutomaticCOResults";
    this.ScalingLabel3.LineAlignment = StringAlignment.Center;
    ((Control) this.ScalingLabel3).Name = "ScalingLabel3";
    this.ScalingLabel2.Alignment = StringAlignment.Far;
    ((TableLayoutPanel) this.tableLayoutPanel2).SetColumnSpan((Control) this.ScalingLabel2, 2);
    componentResourceManager.ApplyResources((object) this.ScalingLabel2, "ScalingLabel2");
    this.ScalingLabel2.FontGroup = "AutomaticCOResults";
    this.ScalingLabel2.LineAlignment = StringAlignment.Center;
    ((Control) this.ScalingLabel2).Name = "ScalingLabel2";
    this.ScalingLabel1.Alignment = StringAlignment.Far;
    ((TableLayoutPanel) this.tableLayoutPanel2).SetColumnSpan((Control) this.ScalingLabel1, 2);
    componentResourceManager.ApplyResources((object) this.ScalingLabel1, "ScalingLabel1");
    this.ScalingLabel1.FontGroup = "AutomaticCOResults";
    this.ScalingLabel1.LineAlignment = StringAlignment.Center;
    ((Control) this.ScalingLabel1).Name = "ScalingLabel1";
    this.ScalingLabel4.Alignment = StringAlignment.Far;
    ((TableLayoutPanel) this.tableLayoutPanel2).SetColumnSpan((Control) this.ScalingLabel4, 2);
    componentResourceManager.ApplyResources((object) this.ScalingLabel4, "ScalingLabel4");
    this.ScalingLabel4.FontGroup = "AutomaticCOResults";
    this.ScalingLabel4.LineAlignment = StringAlignment.Center;
    ((Control) this.ScalingLabel4).Name = "ScalingLabel4";
    this.ScalingLabel5.Alignment = StringAlignment.Far;
    ((TableLayoutPanel) this.tableLayoutPanel2).SetColumnSpan((Control) this.ScalingLabel5, 2);
    componentResourceManager.ApplyResources((object) this.ScalingLabel5, "ScalingLabel5");
    this.ScalingLabel5.FontGroup = "AutomaticCOResults";
    this.ScalingLabel5.LineAlignment = StringAlignment.Center;
    ((Control) this.ScalingLabel5).Name = "ScalingLabel5";
    this.label1.Alignment = StringAlignment.Center;
    componentResourceManager.ApplyResources((object) this.label1, "label1");
    ((TableLayoutPanel) this.tableLayoutPanel2).SetColumnSpan((Control) this.label1, 6);
    ((Control) this.label1).Name = "label1";
    this.label1.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.label8.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.label8, "label8");
    ((Control) this.label8).Name = "label8";
    this.label8.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.label9.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.label9, "label9");
    ((Control) this.label9).Name = "label9";
    this.label9.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.label10.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.label10, "label10");
    ((Control) this.label10).Name = "label10";
    this.label10.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.label11.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.label11, "label11");
    ((Control) this.label11).Name = "label11";
    this.label11.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.label12.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.label12, "label12");
    ((Control) this.label12).Name = "label12";
    this.label12.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.unitsLabel1and6.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.unitsLabel1and6, "unitsLabel1and6");
    ((Control) this.unitsLabel1and6).Name = "unitsLabel1and6";
    this.unitsLabel1and6.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.unitsLabel12and3.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.unitsLabel12and3, "unitsLabel12and3");
    ((Control) this.unitsLabel12and3).Name = "unitsLabel12and3";
    this.unitsLabel12and3.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.unitsLabel45and6.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.unitsLabel45and6, "unitsLabel45and6");
    ((Control) this.unitsLabel45and6).Name = "unitsLabel45and6";
    this.unitsLabel45and6.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.unitsLabel2and5.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.unitsLabel2and5, "unitsLabel2and5");
    ((Control) this.unitsLabel2and5).Name = "unitsLabel2and5";
    this.unitsLabel2and5.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.unitsLabel3and4.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.unitsLabel3and4, "unitsLabel3and4");
    ((Control) this.unitsLabel3and4).Name = "unitsLabel3and4";
    this.unitsLabel3and4.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    ((TableLayoutPanel) this.tableLayoutPanelBase).SetColumnSpan((Control) this.textboxResults, 5);
    componentResourceManager.ApplyResources((object) this.textboxResults, "textboxResults");
    this.textboxResults.Name = "textboxResults";
    this.textboxResults.ReadOnly = true;
    componentResourceManager.ApplyResources((object) this.buttonCancel, "buttonCancel");
    this.buttonCancel.Name = "buttonCancel";
    this.buttonCancel.UseCompatibleTextRendering = true;
    this.buttonCancel.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.buttonExecute, "buttonExecute");
    this.buttonExecute.Name = "buttonExecute";
    this.buttonExecute.UseCompatibleTextRendering = true;
    this.buttonExecute.UseVisualStyleBackColor = true;
    this.labelCheckmarkDpfZoneZero.Alignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanelBase).SetColumnSpan((Control) this.labelCheckmarkDpfZoneZero, 2);
    componentResourceManager.ApplyResources((object) this.labelCheckmarkDpfZoneZero, "labelCheckmarkDpfZoneZero");
    ((Control) this.labelCheckmarkDpfZoneZero).Name = "labelCheckmarkDpfZoneZero";
    this.labelCheckmarkDpfZoneZero.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.labelCheckmarkDpfZoneZero.UseSystemColors = true;
    componentResourceManager.ApplyResources((object) this.checkmarkDpfZone, "checkmarkDpfZone");
    ((Control) this.checkmarkDpfZone).Name = "checkmarkDpfZone";
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("Panel_CylinderCutoutTestAuto");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanelBase);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanelBase).ResumeLayout(false);
    ((Control) this.tableLayoutPanelBase).PerformLayout();
    this.tabControl2.ResumeLayout(false);
    this.tabPage4.ResumeLayout(false);
    this.tabPage3.ResumeLayout(false);
    ((Control) this.tableLayoutPanel2).ResumeLayout(false);
    ((Control) this.tableLayoutPanel2).PerformLayout();
    ((Control) this).ResumeLayout(false);
  }

  private enum State
  {
    Start,
    ChangeIdleSpeed,
    WaitForIdleSpeed,
    Init,
    WaitToGetBaseline,
    CylinderOff,
    WaitWhileCylinderOff,
    CylinderOn,
    End,
  }
}
