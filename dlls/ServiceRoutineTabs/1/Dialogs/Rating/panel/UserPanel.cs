// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Rating.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Help;
using DetroitDiesel.Security;
using DetroitDiesel.UnitConversion;
using DetroitDiesel.Windows.Forms;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Rating.panel;

public class UserPanel : CustomPanel
{
  private const string CruisePowerParameterName = "Cruise_Power";
  private const string Rating0BrakePowerQualifier = "DT_STO_ID_Rated_brake_power_for_rat_0_Rated_brake_power_for_rat_0";
  private const string Rating0EngineSpeedQualifier = "DT_STO_ID_Rated_engine_speed_for_rat_0_Rated_engine_speed_for_rat_0";
  private const string Rating1BrakePowerQualifier = "DT_STO_ID_Rated_brake_power_for_rat_1_Rated_brake_power_for_rat_1";
  private const string Rating1EngineSpeedQualifier = "DT_STO_ID_Rated_engine_speed_for_rat_1_Rated_engine_speed_for_rat_1";
  private const string MaxEngineTorqueQualifier = "DT_STO_ID_Maximum_Engine_Torque_Maximum_Engine_Torque";
  private const string MaxTorqueSpeedQualifier = "DT_STO_ID_Maximum_Torque_Speed_Maximum_Torque_Speed";
  private const string FuelmapDescriptionQualifier = "CO_FuelmapDescription";
  private const string RatingCodeQualifier = "CO_RatingCode";
  private const string CertificationNumberQualifier = "CO_CertificationNumber";
  private const double MinimumRatingThreshold = 50.0;
  private Channel mcm = (Channel) null;
  private Channel cpc2 = (Channel) null;
  private Parameter cruisePowerParameter;
  private bool working;
  private Button buttonHighPower;
  private Button buttonLowPower;
  private Button buttonCruisePower;
  private Button buttonReadRating;
  private Button buttonWriteRating;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label labelSelection;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label label5;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label labelCruisePowerDescription;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label label3;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label labelLowPowerDescription;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label label2;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label labelHighPowerDescription;
  private DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label label1;
  private TableLayoutPanel tableLayoutPanel2;
  private Button buttonClose;
  private System.Windows.Forms.Label labelWarning;
  private TextBox textboxResults;

  public UserPanel()
  {
    this.InitializeComponent();
    this.buttonHighPower.Click += new EventHandler(this.OnHighPowerClick);
    this.buttonLowPower.Click += new EventHandler(this.OnLowPowerClick);
    this.buttonCruisePower.Click += new EventHandler(this.OnCruisePowerClick);
    this.buttonReadRating.Click += new EventHandler(this.OnReadRatingClick);
    this.buttonWriteRating.Click += new EventHandler(this.OnWriteRatingClick);
  }

  protected virtual void OnLoad(EventArgs e)
  {
    this.UpdateUserInterface();
    ((ContainerControl) this).ParentForm.FormClosing += new FormClosingEventHandler(this.OnParentFormClosing);
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
  }

  private void OnParentFormClosing(object sender, FormClosingEventArgs e)
  {
    if (e.CloseReason == CloseReason.UserClosing && !this.CanClose)
      e.Cancel = true;
    if (e.Cancel)
      return;
    ((ContainerControl) this).ParentForm.FormClosing -= new FormClosingEventHandler(this.OnParentFormClosing);
    this.CleanUpChannels();
  }

  public virtual void OnChannelsChanged()
  {
    this.UpdateChannels();
    this.UpdateUserInterface();
  }

  private void UpdateChannels()
  {
    if (!(this.SetCPC2(this.GetChannel("CPC2")) | this.SetMCM(this.GetChannel("MCM"))))
      return;
    this.UpdateWarningMessage();
  }

  private void CleanUpChannels()
  {
    this.SetCPC2((Channel) null);
    this.SetMCM((Channel) null);
    this.UpdateWarningMessage();
  }

  private bool SetMCM(Channel mcm)
  {
    bool flag = false;
    if (this.mcm != mcm)
    {
      flag = true;
      this.Working = false;
      if (this.mcm != null)
        this.mcm.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnChannelStateUpdate);
      this.mcm = mcm;
      if (this.mcm != null)
        this.mcm.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnChannelStateUpdate);
    }
    return flag;
  }

  private bool SetCPC2(Channel cpc2)
  {
    bool flag = false;
    if (this.cpc2 != cpc2)
    {
      flag = true;
      this.Working = false;
      if (this.cpc2 != null)
      {
        this.cpc2.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnChannelStateUpdate);
        if (this.cruisePowerParameter != null)
          this.cruisePowerParameter.ParameterUpdateEvent -= new ParameterUpdateEventHandler(this.OnCruisePowerParameterUpdate);
        this.cruisePowerParameter = (Parameter) null;
      }
      this.cpc2 = cpc2;
      if (this.cpc2 != null)
      {
        this.cpc2.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnChannelStateUpdate);
        this.cruisePowerParameter = this.cpc2.Parameters["Cruise_Power"];
        this.cruisePowerParameter.ParameterUpdateEvent += new ParameterUpdateEventHandler(this.OnCruisePowerParameterUpdate);
      }
    }
    return flag;
  }

  private void UpdateWarningMessage()
  {
    bool flag = false;
    if (this.cpc2 != null && UserPanel.HasUnsentChanges(this.cpc2))
      flag = true;
    if (this.mcm != null && UserPanel.HasUnsentChanges(this.mcm))
      flag = true;
    this.labelWarning.Visible = flag;
  }

  private static bool HasUnsentChanges(Channel channel)
  {
    bool flag = false;
    foreach (Parameter parameter in (ReadOnlyCollection<Parameter>) channel.Parameters)
    {
      if ((parameter.Qualifier != "Cruise_Power" || channel.Ecu.Name != "CPC2") && !object.Equals(parameter.Value, parameter.OriginalValue))
      {
        flag = true;
        break;
      }
    }
    return flag;
  }

  private bool HaveReadCruisePower
  {
    get => this.cruisePowerParameter != null && this.cruisePowerParameter.HasBeenReadFromEcu;
  }

  private bool Busy => this.Online && this.cpc2.CommunicationsState != CommunicationsState.Online;

  private bool Working
  {
    get => this.working;
    set
    {
      if (this.working == value)
        return;
      this.working = value;
      if (!this.working)
        this.UpdateWarningMessage();
      this.UpdateUserInterface();
    }
  }

  private bool CanRead => this.Online && !this.Working && !this.Busy && !this.HaveReadCruisePower;

  private bool CanWrite
  {
    get
    {
      return this.Online && !this.Working && !this.Busy && this.HaveReadCruisePower && !object.Equals(this.cruisePowerParameter.Value, this.cruisePowerParameter.OriginalValue);
    }
  }

  private bool CanClose => !this.Working;

  private bool Online => this.cpc2 != null && this.cpc2.Online;

  private bool HaveReadDescriptions
  {
    get
    {
      return this.mcm != null && this.mcm.Online && this.mcm.CommunicationsState != CommunicationsState.ReadEcuInfo;
    }
  }

  private string EcuInfoValue(Channel channel, string Qualifier)
  {
    string str = string.Empty;
    EcuInfo ecuInfo = channel.EcuInfos[Qualifier] ?? channel.EcuInfos.GetItemContaining(Qualifier);
    if (ecuInfo != null && !string.IsNullOrEmpty(ecuInfo.Value))
    {
      bool flag = false;
      Conversion conversion = Converter.GlobalInstance.GetConversion(ecuInfo.Units);
      if (conversion != null && Conversion.CanConvert((object) ecuInfo.Value))
      {
        str = $"{Math.Round(conversion.Convert((object) ecuInfo.Value))} {conversion.OutputUnit}";
        flag = true;
      }
      if (!flag)
        str = $"{ecuInfo.Value.ToString()} {ecuInfo.Units}";
    }
    return str;
  }

  private bool IsValidPowerRating(string powerRating)
  {
    bool flag = false;
    Conversion conversion = Converter.GlobalInstance.GetConversion("KW");
    double num = 50.0;
    if (conversion != null)
      num = conversion.Convert(num);
    double result;
    if (double.TryParse(powerRating.Remove(powerRating.IndexOf(" ")), out result))
      flag = result > num;
    return flag;
  }

  private void UpdateUserInterface()
  {
    bool flag1 = this.HaveReadCruisePower && this.cruisePowerParameter.Choices.Count > 0 && !this.Busy;
    bool flag2 = this.HaveReadCruisePower && this.cruisePowerParameter.Choices.Count > 1 && !this.Busy;
    bool flag3 = this.HaveReadCruisePower && this.cruisePowerParameter.Choices.Count > 2 && !this.Busy;
    bool canRead = this.CanRead;
    bool canWrite = this.CanWrite;
    ((Control) this.labelHighPowerDescription).Text = "";
    ((Control) this.labelLowPowerDescription).Text = "";
    ((Control) this.labelCruisePowerDescription).Text = "";
    ((Control) this.labelSelection).Text = "";
    if (this.HaveReadCruisePower)
    {
      Choice choice = this.cruisePowerParameter.Value as Choice;
      switch (Convert.ToInt32(choice.RawValue))
      {
        case 0:
          flag1 = false;
          break;
        case 1:
          flag2 = false;
          break;
        case 2:
          flag3 = false;
          break;
      }
      ((Control) this.labelSelection).Text = choice.ToString();
    }
    if (this.HaveReadDescriptions)
    {
      string powerRating1 = this.EcuInfoValue(this.mcm, "DT_STO_ID_Rated_brake_power_for_rat_0_Rated_brake_power_for_rat_0");
      string powerRating2 = this.EcuInfoValue(this.mcm, "DT_STO_ID_Rated_brake_power_for_rat_1_Rated_brake_power_for_rat_1");
      flag1 = flag1 && !this.Busy && this.IsValidPowerRating(powerRating1);
      flag2 = flag2 && !this.Busy && this.IsValidPowerRating(powerRating2);
      flag3 = flag3 && !this.Busy && this.IsValidPowerRating(powerRating1) && this.IsValidPowerRating(powerRating2);
      ((Control) this.labelHighPowerDescription).Text = string.Format(Resources.MessageFormat_Power01Torque231, (object) powerRating1, (object) this.EcuInfoValue(this.mcm, "DT_STO_ID_Rated_engine_speed_for_rat_0_Rated_engine_speed_for_rat_0"), (object) this.EcuInfoValue(this.mcm, "DT_STO_ID_Maximum_Engine_Torque_Maximum_Engine_Torque"), (object) this.EcuInfoValue(this.mcm, "DT_STO_ID_Maximum_Torque_Speed_Maximum_Torque_Speed"));
      ((Control) this.labelLowPowerDescription).Text = string.Format(Resources.MessageFormat_Power01Torque23, (object) powerRating2, (object) this.EcuInfoValue(this.mcm, "DT_STO_ID_Rated_engine_speed_for_rat_1_Rated_engine_speed_for_rat_1"), (object) this.EcuInfoValue(this.mcm, "DT_STO_ID_Maximum_Engine_Torque_Maximum_Engine_Torque"), (object) this.EcuInfoValue(this.mcm, "DT_STO_ID_Maximum_Torque_Speed_Maximum_Torque_Speed"));
      ((Control) this.labelCruisePowerDescription).Text = string.Format(Resources.MessageFormat_Power012Torque34, (object) powerRating2, (object) powerRating1, (object) this.EcuInfoValue(this.mcm, "DT_STO_ID_Rated_engine_speed_for_rat_0_Rated_engine_speed_for_rat_0"), (object) this.EcuInfoValue(this.mcm, "DT_STO_ID_Maximum_Engine_Torque_Maximum_Engine_Torque"), (object) this.EcuInfoValue(this.mcm, "DT_STO_ID_Maximum_Torque_Speed_Maximum_Torque_Speed"));
    }
    this.buttonClose.Enabled = this.CanClose;
    this.buttonHighPower.Enabled = flag1;
    this.buttonLowPower.Enabled = flag2;
    this.buttonCruisePower.Enabled = flag3;
    this.buttonReadRating.Enabled = canRead;
    this.buttonWriteRating.Enabled = canWrite;
  }

  private void ClearResults() => this.textboxResults.Text = string.Empty;

  private void ReportResult(string text)
  {
    this.textboxResults.Text = $"{this.textboxResults.Text}{text}\r\n";
    this.textboxResults.SelectionStart = this.textboxResults.TextLength;
    this.textboxResults.SelectionLength = 0;
    this.textboxResults.ScrollToCaret();
    this.AddStatusMessage(text);
  }

  private bool Unlock(Channel channel)
  {
    bool flag1 = true;
    if (PasswordManager.HasPasswords(channel))
    {
      PasswordManager passwordManager = PasswordManager.Create(channel);
      if (passwordManager != null && passwordManager.Valid)
      {
        try
        {
          this.ReportResult(Resources.Message_AcquiringDeviceLockStatus);
          bool[] flagArray = passwordManager.AcquireRelevantListStatus((ProgressBar) null);
          bool flag2 = false;
          foreach (bool flag3 in flagArray)
          {
            if (flag3)
            {
              flag2 = true;
              break;
            }
          }
          if (flag2)
          {
            this.ReportResult(Resources.Message_DeviceIsLocked);
            if (((Form) new PasswordEntryDialog(channel, flagArray, passwordManager)).ShowDialog() != DialogResult.OK)
            {
              flag1 = false;
              this.ReportResult(Resources.Message_DeviceWasNotUnlockedByUser);
            }
            else
              this.ReportResult(Resources.Message_DeviceWasUnlocked);
          }
          else
            this.ReportResult(Resources.Message_DeviceIsUnlocked);
        }
        catch (CaesarException ex)
        {
          flag1 = false;
          this.ReportResult(Resources.Message_ErrorWhileUnlockingDevice);
        }
      }
    }
    return flag1;
  }

  private void UpdateEcuInfo(string qualifier) => this.mcm.EcuInfos[qualifier]?.Read(false);

  private void OnParametersReadComplete(object sender, ResultEventArgs e)
  {
    if (e.Succeeded)
      this.ReportResult(Resources.Message_SettingSuccessfullyRead);
    else
      this.ReportResult(Resources.Message_ErrorWhileReadingSetting);
    this.cpc2.Parameters.ParametersReadCompleteEvent -= new ParametersReadCompleteEventHandler(this.OnParametersReadComplete);
    this.Working = false;
  }

  private void OnParametersWriteComplete(object sender, ResultEventArgs e)
  {
    if (e.Succeeded)
    {
      this.ReportResult(Resources.Message_SettingSuccessfullySent0);
      if (this.mcm != null && this.mcm.CommunicationsState == CommunicationsState.Online)
      {
        this.UpdateEcuInfo("CO_FuelmapDescription");
        this.UpdateEcuInfo("CO_RatingCode");
        this.UpdateEcuInfo("CO_CertificationNumber");
      }
    }
    else
      this.ReportResult(Resources.Message_ErrorWhileSendingSetting);
    this.cpc2.Parameters.ParametersWriteCompleteEvent -= new ParametersWriteCompleteEventHandler(this.OnParametersWriteComplete);
    this.Working = false;
  }

  private void OnReadRatingClick(object sender, EventArgs e)
  {
    if (!this.CanRead)
      return;
    this.Working = true;
    this.ClearResults();
    this.ReportResult(Resources.Message_ReadingSettingFromCPC2);
    this.cpc2.Parameters.ParametersReadCompleteEvent += new ParametersReadCompleteEventHandler(this.OnParametersReadComplete);
    this.cpc2.Parameters.ReadGroup(this.cruisePowerParameter.GroupQualifier, true, false);
  }

  private void OnWriteRatingClick(object sender, EventArgs e)
  {
    if (!this.CanWrite)
      return;
    this.Working = true;
    this.ClearResults();
    if (this.Unlock(this.cpc2))
    {
      this.ReportResult(Resources.Message_SendingSettingToCPC2);
      this.cpc2.Parameters.ParametersWriteCompleteEvent += new ParametersWriteCompleteEventHandler(this.OnParametersWriteComplete);
      this.cpc2.Parameters.Write(false);
    }
    else
      this.Working = false;
  }

  private void OnChannelStateUpdate(object sender, CommunicationsStateEventArgs e)
  {
    this.UpdateUserInterface();
  }

  private void OnCruisePowerParameterUpdate(object sender, ResultEventArgs e)
  {
    this.UpdateUserInterface();
  }

  private void OnHighPowerClick(object sender, EventArgs e)
  {
    if (this.cruisePowerParameter == null)
      return;
    this.cruisePowerParameter.Value = (object) this.cruisePowerParameter.Choices[0];
    this.UpdateUserInterface();
  }

  private void OnLowPowerClick(object sender, EventArgs e)
  {
    if (this.cruisePowerParameter == null)
      return;
    this.cruisePowerParameter.Value = (object) this.cruisePowerParameter.Choices[1];
    this.UpdateUserInterface();
  }

  private void OnCruisePowerClick(object sender, EventArgs e)
  {
    if (this.cruisePowerParameter == null)
      return;
    this.cruisePowerParameter.Value = (object) this.cruisePowerParameter.Choices[2];
    this.UpdateUserInterface();
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.tableLayoutPanel2 = new TableLayoutPanel();
    this.buttonWriteRating = new Button();
    this.labelSelection = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.buttonReadRating = new Button();
    this.textboxResults = new TextBox();
    this.label1 = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.label5 = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.labelCruisePowerDescription = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.labelHighPowerDescription = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.label2 = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.labelLowPowerDescription = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.label3 = new DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label();
    this.buttonClose = new Button();
    this.labelWarning = new System.Windows.Forms.Label();
    this.buttonHighPower = new Button();
    this.buttonLowPower = new Button();
    this.buttonCruisePower = new Button();
    ((Control) this.tableLayoutPanel2).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel2, "tableLayoutPanel2");
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.buttonWriteRating, 2, 8);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.labelSelection, 1, 6);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.buttonReadRating, 1, 8);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.textboxResults, 0, 9);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.label1, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.label5, 0, 6);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.labelCruisePowerDescription, 0, 5);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.labelHighPowerDescription, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.label2, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.labelLowPowerDescription, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.label3, 0, 4);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.buttonClose, 2, 10);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.labelWarning, 0, 7);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.buttonHighPower, 2, 0);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.buttonLowPower, 2, 2);
    ((TableLayoutPanel) this.tableLayoutPanel2).Controls.Add((Control) this.buttonCruisePower, 2, 4);
    ((Control) this.tableLayoutPanel2).Name = "tableLayoutPanel2";
    componentResourceManager.ApplyResources((object) this.buttonWriteRating, "buttonWriteRating");
    this.buttonWriteRating.Name = "buttonWriteRating";
    this.buttonWriteRating.UseCompatibleTextRendering = true;
    this.buttonWriteRating.UseVisualStyleBackColor = true;
    this.labelSelection.Alignment = StringAlignment.Far;
    componentResourceManager.ApplyResources((object) this.labelSelection, "labelSelection");
    ((TableLayoutPanel) this.tableLayoutPanel2).SetColumnSpan((Control) this.labelSelection, 2);
    ((Control) this.labelSelection).Name = "labelSelection";
    this.labelSelection.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.labelSelection.UseSystemColors = true;
    componentResourceManager.ApplyResources((object) this.buttonReadRating, "buttonReadRating");
    this.buttonReadRating.Name = "buttonReadRating";
    this.buttonReadRating.UseCompatibleTextRendering = true;
    this.buttonReadRating.UseVisualStyleBackColor = true;
    ((TableLayoutPanel) this.tableLayoutPanel2).SetColumnSpan((Control) this.textboxResults, 3);
    componentResourceManager.ApplyResources((object) this.textboxResults, "textboxResults");
    this.textboxResults.Name = "textboxResults";
    this.textboxResults.ReadOnly = true;
    this.label1.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.label1, "label1");
    ((Control) this.label1).Name = "label1";
    this.label1.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.label1.UseSystemColors = true;
    this.label5.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.label5, "label5");
    ((Control) this.label5).Name = "label5";
    this.label5.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.label5.UseSystemColors = true;
    this.labelCruisePowerDescription.Alignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel2).SetColumnSpan((Control) this.labelCruisePowerDescription, 3);
    componentResourceManager.ApplyResources((object) this.labelCruisePowerDescription, "labelCruisePowerDescription");
    ((Control) this.labelCruisePowerDescription).Name = "labelCruisePowerDescription";
    this.labelCruisePowerDescription.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.labelCruisePowerDescription.UseSystemColors = true;
    this.labelHighPowerDescription.Alignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel2).SetColumnSpan((Control) this.labelHighPowerDescription, 3);
    componentResourceManager.ApplyResources((object) this.labelHighPowerDescription, "labelHighPowerDescription");
    ((Control) this.labelHighPowerDescription).Name = "labelHighPowerDescription";
    this.labelHighPowerDescription.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.labelHighPowerDescription.UseSystemColors = true;
    this.label2.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.label2, "label2");
    ((Control) this.label2).Name = "label2";
    this.label2.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.label2.UseSystemColors = true;
    this.labelLowPowerDescription.Alignment = StringAlignment.Near;
    ((TableLayoutPanel) this.tableLayoutPanel2).SetColumnSpan((Control) this.labelLowPowerDescription, 3);
    componentResourceManager.ApplyResources((object) this.labelLowPowerDescription, "labelLowPowerDescription");
    ((Control) this.labelLowPowerDescription).Name = "labelLowPowerDescription";
    this.labelLowPowerDescription.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.labelLowPowerDescription.UseSystemColors = true;
    this.label3.Alignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.label3, "label3");
    ((Control) this.label3).Name = "label3";
    this.label3.Orientation = (DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments.Label.TextOrientation) 1;
    this.label3.UseSystemColors = true;
    componentResourceManager.ApplyResources((object) this.buttonClose, "buttonClose");
    this.buttonClose.DialogResult = DialogResult.OK;
    this.buttonClose.Name = "buttonClose";
    this.buttonClose.UseCompatibleTextRendering = true;
    this.buttonClose.UseVisualStyleBackColor = true;
    this.labelWarning.BackColor = SystemColors.Control;
    ((TableLayoutPanel) this.tableLayoutPanel2).SetColumnSpan((Control) this.labelWarning, 3);
    componentResourceManager.ApplyResources((object) this.labelWarning, "labelWarning");
    this.labelWarning.ForeColor = Color.Red;
    this.labelWarning.Name = "labelWarning";
    this.labelWarning.UseCompatibleTextRendering = true;
    this.labelWarning.UseMnemonic = false;
    componentResourceManager.ApplyResources((object) this.buttonHighPower, "buttonHighPower");
    this.buttonHighPower.Name = "buttonHighPower";
    this.buttonHighPower.UseCompatibleTextRendering = true;
    this.buttonHighPower.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.buttonLowPower, "buttonLowPower");
    this.buttonLowPower.Name = "buttonLowPower";
    this.buttonLowPower.UseCompatibleTextRendering = true;
    this.buttonLowPower.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.buttonCruisePower, "buttonCruisePower");
    this.buttonCruisePower.Name = "buttonCruisePower";
    this.buttonCruisePower.UseCompatibleTextRendering = true;
    this.buttonCruisePower.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("Panel_Rating");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel2);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanel2).ResumeLayout(false);
    ((Control) this.tableLayoutPanel2).PerformLayout();
    ((Control) this).ResumeLayout(false);
  }
}
