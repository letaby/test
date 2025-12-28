// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Injector_Codes__MY13_.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Collections;
using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Interfaces;
using DetroitDiesel.Windows.Forms;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Injector_Codes__MY13_.panel;

public class UserPanel : CustomPanel, IProvideHtml
{
  private const int MaxCylinderCount = 6;
  private const int ResetRpgLeakValuesRawValue = 24;
  private const string ParameterLeakLearnedValue = "DT_STO_ACC047_OP_Data_4_HP_Leak_Learned_Value";
  private const string McmChannelName = "MCM21T";
  private const string ReadService = "DJ_Read_E_Trim";
  private const string WriteService = "DJ_Write_E_Trim";
  private const string ResetService = "RT_SR070_Injector_Change_Start";
  private const string ResetAllService = "RT_SR074_EcuInitPIR_Start";
  private const string ResetLeakValuesService = "RT_SR014_SET_EOL_Default_Values_Start";
  private static string[] pirQualifiers = new string[6];
  private UserPanel.SetupInformation currentSetup = (UserPanel.SetupInformation) null;
  private string[] oldTrimCodes = new string[6];
  private bool[] needReset = new bool[6];
  private TextBox[] trimCodes;
  private TextBox[] partNumbers;
  private Checkmark[] checkMarks;
  private Checkmark[] partNumberCheckMarks;
  private System.Windows.Forms.Label[] cylinderLabels;
  private Channel mcm;
  private UserPanel.State nextState = UserPanel.State.None;
  private int currentCylinder = -1;
  private Service currentService;
  private readonly UserPanel.SetupInformationCollection Setups;
  private bool readOperationSuccessful = true;
  private bool writeOperationSuccessful = true;
  private readonly Regex dd13InjectorPartValidator = new Regex("^[rR]?[aA]471\\d{7}$", RegexOptions.CultureInvariant);
  private readonly Regex dd15Or16InjectorPartValidator = new Regex("^[rR]?[aA]472\\d{7}$", RegexOptions.CultureInvariant);
  private readonly Regex dd16Tier4InjectorPartValidator = new Regex("^[rR]?[aA]47[23]\\d{7}$", RegexOptions.CultureInvariant);
  private TextBox textTrimCode1;
  private TextBox textTrimCode2;
  private TextBox textTrimCode3;
  private TextBox textTrimCode4;
  private TextBox textTrimCode5;
  private TextBox textTrimCode6;
  private Panel panelPictures;
  private System.Windows.Forms.Label label1;
  private System.Windows.Forms.Label label2;
  private System.Windows.Forms.Label label3;
  private System.Windows.Forms.Label label4;
  private System.Windows.Forms.Label label5;
  private Button buttonClose;
  private System.Windows.Forms.Label lblCylinderPosition;
  private DigitalReadoutInstrument iscInstrument1;
  private DigitalReadoutInstrument iscInstrument6;
  private DigitalReadoutInstrument iscInstrument5;
  private DigitalReadoutInstrument iscInstrument4;
  private DigitalReadoutInstrument iscInstrument3;
  private DigitalReadoutInstrument iscInstrument2;
  private System.Windows.Forms.Label minLabel1;
  private System.Windows.Forms.Label minLabel2;
  private System.Windows.Forms.Label minLabel3;
  private System.Windows.Forms.Label minLabel4;
  private System.Windows.Forms.Label minLabel5;
  private System.Windows.Forms.Label minLabel6;
  private System.Windows.Forms.Label maxLabel1;
  private System.Windows.Forms.Label maxLabel2;
  private System.Windows.Forms.Label maxLabel3;
  private System.Windows.Forms.Label maxLabel4;
  private System.Windows.Forms.Label maxLabel5;
  private System.Windows.Forms.Label maxLabel6;
  private System.Windows.Forms.Label label20;
  private System.Windows.Forms.Label iscLabel;
  private System.Windows.Forms.Label minLabel;
  private System.Windows.Forms.Label maxLabel;
  private System.Windows.Forms.Label pirLabel;
  private DigitalReadoutInstrument pirInstrument1;
  private DigitalReadoutInstrument pirInstrument2;
  private DigitalReadoutInstrument pirInstrument3;
  private DigitalReadoutInstrument pirInstrument4;
  private DigitalReadoutInstrument pirInstrument5;
  private DigitalReadoutInstrument pirInstrument6;
  private PictureBox pictureBoxHDEP;
  private System.Windows.Forms.Label labelPartNumberEntryTips;
  private TableLayoutPanel mainLayoutPanel;
  private Checkmark checkMark1;
  private Checkmark checkMark2;
  private Checkmark checkMark3;
  private Checkmark checkMark4;
  private Checkmark checkMark5;
  private Checkmark checkMark6;
  private DigitalReadoutInstrument digitalReadoutInstrumentEngineState;
  private System.Windows.Forms.Label partNumberLabel;
  private TextBox textBoxPartNumber1;
  private Checkmark checkmarkPartNumber1;
  private Checkmark checkmarkPartNumber2;
  private TextBox textBoxPartNumber2;
  private Checkmark checkmarkPartNumber3;
  private TextBox textBoxPartNumber3;
  private Checkmark checkmarkPartNumber4;
  private TextBox textBoxPartNumber4;
  private Checkmark checkmarkPartNumber5;
  private TextBox textBoxPartNumber5;
  private Checkmark checkmarkPartNumber6;
  private TextBox textBoxPartNumber6;
  private PictureBox pictureBoxExamplePartNumber;
  private FlowLayoutPanel flowLayoutPanel1;
  private Button buttonRead;
  private Button buttonWrite;
  private Button buttonReset;
  private Button buttonPrint;
  private DigitalReadoutInstrument digitalReadoutLeakLearnedValue;
  private System.Windows.Forms.Label labelDD15Warning;
  private System.Windows.Forms.Label label6;
  private System.Windows.Forms.Label labelEntryTips;
  private TextBox textboxResults;

  static UserPanel()
  {
    UserPanel.pirQualifiers[0] = "DT_STO_ACC046_OP_Data_3_e2p_pir_adapt_cyl_1_NOP0";
    UserPanel.pirQualifiers[1] = "DT_STO_ACC046_OP_Data_3_e2p_pir_adapt_cyl_2_NOP0";
    UserPanel.pirQualifiers[2] = "DT_STO_ACC046_OP_Data_3_e2p_pir_adapt_cyl_3_NOP0";
    UserPanel.pirQualifiers[3] = "DT_STO_ACC046_OP_Data_3_e2p_pir_adapt_cyl_4_NOP0";
    UserPanel.pirQualifiers[4] = "DT_STO_ACC046_OP_Data_3_e2p_pir_adapt_cyl_5_NOP0";
    UserPanel.pirQualifiers[5] = "DT_STO_ACC046_OP_Data_3_e2p_pir_adapt_cyl_6_NOP0";
  }

  private int currentCylinderCount
  {
    get => this.currentSetup != null ? this.currentSetup.CylinderCount : 6;
  }

  public UserPanel()
  {
    this.InitializeComponent();
    this.Setups = new UserPanel.SetupInformationCollection(new UserPanel.SetupInformation[17]
    {
      new UserPanel.SetupInformation("DD15", 5, UserPanel.Features.ChecksumAtEnd | UserPanel.Features.Isc | UserPanel.Features.RequirePartNumbers, this.pictureBoxHDEP, 6),
      new UserPanel.SetupInformation("DD13", 5, UserPanel.Features.ChecksumAtEnd | UserPanel.Features.Isc | UserPanel.Features.RequirePartNumbers, this.pictureBoxHDEP, 6),
      new UserPanel.SetupInformation("DD13 Tier4", 5, UserPanel.Features.ChecksumAtEnd | UserPanel.Features.Isc | UserPanel.Features.RequirePartNumbers, this.pictureBoxHDEP, 6),
      new UserPanel.SetupInformation("DD13 StageV", 5, UserPanel.Features.ChecksumAtEnd | UserPanel.Features.Isc | UserPanel.Features.RequirePartNumbers, this.pictureBoxHDEP, 6),
      new UserPanel.SetupInformation("DD13EURO5", 5, UserPanel.Features.ChecksumAtEnd | UserPanel.Features.Isc | UserPanel.Features.RequirePartNumbers, this.pictureBoxHDEP, 6),
      new UserPanel.SetupInformation("DD16", 5, UserPanel.Features.ChecksumAtEnd | UserPanel.Features.Isc | UserPanel.Features.RequirePartNumbers, this.pictureBoxHDEP, 6),
      new UserPanel.SetupInformation("DD16 Tier4", 5, UserPanel.Features.ChecksumAtEnd | UserPanel.Features.Isc | UserPanel.Features.RequirePartNumbers, this.pictureBoxHDEP, 6),
      new UserPanel.SetupInformation("DD16 StageV", 5, UserPanel.Features.ChecksumAtEnd | UserPanel.Features.Isc | UserPanel.Features.RequirePartNumbers, this.pictureBoxHDEP, 6),
      new UserPanel.SetupInformation("DD16EURO5", 5, UserPanel.Features.ChecksumAtEnd | UserPanel.Features.Isc | UserPanel.Features.RequirePartNumbers, this.pictureBoxHDEP, 6),
      new UserPanel.SetupInformation("DD11 Tier4", 5, UserPanel.Features.ChecksumAtEnd | UserPanel.Features.Isc, this.pictureBoxHDEP, 6),
      new UserPanel.SetupInformation("DD11 StageV", 5, UserPanel.Features.ChecksumAtEnd | UserPanel.Features.Isc, this.pictureBoxHDEP, 6),
      new UserPanel.SetupInformation("MDEG 4-Cylinder Tier4", 5, UserPanel.Features.ChecksumAtEnd | UserPanel.Features.Isc, this.pictureBoxHDEP, 4),
      new UserPanel.SetupInformation("MDEG 6-Cylinder Tier4", 5, UserPanel.Features.ChecksumAtEnd | UserPanel.Features.Isc, this.pictureBoxHDEP, 6),
      new UserPanel.SetupInformation("MDEG 4-Cylinder StageV", 5, UserPanel.Features.ChecksumAtEnd | UserPanel.Features.Isc, this.pictureBoxHDEP, 4),
      new UserPanel.SetupInformation("MDEG 6-Cylinder StageV", 5, UserPanel.Features.ChecksumAtEnd | UserPanel.Features.Isc, this.pictureBoxHDEP, 6),
      new UserPanel.SetupInformation("DD5", 5, UserPanel.Features.ChecksumAtEnd | UserPanel.Features.Isc, this.pictureBoxHDEP, 4),
      new UserPanel.SetupInformation("DD8", 5, UserPanel.Features.ChecksumAtEnd | UserPanel.Features.Isc, this.pictureBoxHDEP, 6)
    });
    this.buttonRead.Click += new EventHandler(this.OnReadClick);
    this.buttonWrite.Click += new EventHandler(this.OnWriteClick);
    this.buttonReset.Click += new EventHandler(this.OnResetClick);
    this.buttonPrint.Click += new EventHandler(this.OnPrintClick);
    this.digitalReadoutInstrumentEngineState.RepresentedStateChanged += new EventHandler(this.OnEngineStateChanged);
    this.trimCodes = new TextBox[6]
    {
      this.textTrimCode1,
      this.textTrimCode2,
      this.textTrimCode3,
      this.textTrimCode4,
      this.textTrimCode5,
      this.textTrimCode6
    };
    this.checkMarks = new Checkmark[6]
    {
      this.checkMark1,
      this.checkMark2,
      this.checkMark3,
      this.checkMark4,
      this.checkMark5,
      this.checkMark6
    };
    this.partNumbers = new TextBox[6]
    {
      this.textBoxPartNumber1,
      this.textBoxPartNumber2,
      this.textBoxPartNumber3,
      this.textBoxPartNumber4,
      this.textBoxPartNumber5,
      this.textBoxPartNumber6
    };
    this.partNumberCheckMarks = new Checkmark[6]
    {
      this.checkmarkPartNumber1,
      this.checkmarkPartNumber2,
      this.checkmarkPartNumber3,
      this.checkmarkPartNumber4,
      this.checkmarkPartNumber5,
      this.checkmarkPartNumber6
    };
    this.cylinderLabels = new System.Windows.Forms.Label[7]
    {
      this.lblCylinderPosition,
      this.label1,
      this.label2,
      this.label3,
      this.label4,
      this.label5,
      this.label6
    };
    for (int index = 0; index < 6; ++index)
    {
      this.trimCodes[index].Tag = (object) index;
      this.trimCodes[index].TextChanged += new EventHandler(this.OnTextChanged);
      this.trimCodes[index].KeyPress += new KeyPressEventHandler(this.OnKeyPress);
      this.partNumbers[index].TextChanged += new EventHandler(this.OnPartNumberChanged);
    }
    this.UpdateControlVisibility();
  }

  protected virtual void OnLoad(EventArgs e)
  {
    ((ContainerControl) this).ParentForm.FormClosing += new FormClosingEventHandler(this.OnFormClosing);
    // ISSUE: explicit non-virtual call
    __nonvirtual (((UserControl) this).OnLoad(e));
    if (((Control) this).Tag == null)
      return;
    this.ReadTrimCodes();
  }

  private void OnFormClosing(object sender, FormClosingEventArgs e)
  {
    ((Control) this).Tag = (object) new object[2]
    {
      (object) (bool) (!this.readOperationSuccessful ? 0 : (this.writeOperationSuccessful ? 1 : 0)),
      (object) this.textboxResults.Text
    };
    if (e.CloseReason == CloseReason.UserClosing && !this.CanClose)
      e.Cancel = true;
    if (e.Cancel)
      return;
    ((ContainerControl) this).ParentForm.FormClosing -= new FormClosingEventHandler(this.OnFormClosing);
    this.SetMCM((Channel) null);
  }

  public virtual void OnChannelsChanged()
  {
    this.SetMCM(this.GetChannel("MCM21T"));
    this.UpdateUserInterface();
  }

  private void SetMCM(Channel mcm)
  {
    if (this.mcm != mcm)
    {
      this.ResetData();
      if (this.mcm != null)
        this.mcm.CommunicationsStateUpdateEvent -= new CommunicationsStateUpdateEventHandler(this.OnChannelStateUpdate);
      this.mcm = mcm;
      if (this.mcm != null)
        this.mcm.CommunicationsStateUpdateEvent += new CommunicationsStateUpdateEventHandler(this.OnChannelStateUpdate);
    }
    this.UpdateCurrentSetup();
  }

  private bool CanClose => !this.Working;

  private bool Working => this.nextState != UserPanel.State.None;

  private bool Online
  {
    get => this.currentSetup != null && this.mcm.CommunicationsState == CommunicationsState.Online;
  }

  private bool EngineOK => this.digitalReadoutInstrumentEngineState.RepresentedState == 1;

  private bool CanReadTrimCodes => !this.Working && this.Online;

  private bool CanWriteTrimCodes
  {
    get
    {
      bool canWriteTrimCodes = false;
      if (!this.Working && this.Online && this.EngineOK)
      {
        bool flag1 = true;
        bool flag2 = true;
        for (int partNumberIndex = 0; partNumberIndex < this.currentCylinderCount; ++partNumberIndex)
        {
          if (!string.IsNullOrEmpty(this.trimCodes[partNumberIndex].Text))
          {
            flag2 = false;
            if (!this.ValidTrimCode(this.trimCodes[partNumberIndex].Text))
            {
              flag1 = false;
              break;
            }
          }
          if (!string.IsNullOrEmpty(this.partNumbers[partNumberIndex].Text) && !this.ValidatePartNumber(partNumberIndex))
          {
            flag1 = false;
            break;
          }
        }
        canWriteTrimCodes = flag1 && !flag2;
      }
      return canWriteTrimCodes;
    }
  }

  private bool CanResetPirValues
  {
    get
    {
      return !this.Working && this.Online && this.GetService("MCM21T", "RT_SR074_EcuInitPIR_Start") != (Service) null;
    }
  }

  private bool CanPrintCodes => base.CanProvideHtml;

  private int MaxTextLength => this.currentSetup != null ? this.currentSetup.CodeLengthWithCRC : 0;

  private void AdvanceState()
  {
    switch (this.nextState)
    {
      case UserPanel.State.Read:
        this.ClearResults();
        this.oldTrimCodes = new string[this.currentCylinderCount];
        this.ReportResult(Resources.Message_ReadingInjectorCodes);
        this.currentCylinder = 0;
        this.nextState = UserPanel.State.ReadTrimCodes;
        this.AdvanceState();
        break;
      case UserPanel.State.ReadTrimCodes:
        this.NextCylinder();
        if (this.currentCylinder < 0)
        {
          this.ReportResult(Resources.Message_ReadingPIRValues);
          this.currentCylinder = 0;
          this.nextState = UserPanel.State.ReadPirValues;
          this.AdvanceState();
          break;
        }
        this.ReadCurrentTrimCode();
        break;
      case UserPanel.State.ReadPirValues:
        this.NextCylinder();
        if (this.currentCylinder < 0)
        {
          this.ReportResult(Resources.Message_PIRParametersRead);
          this.nextState = UserPanel.State.ReadRgpLeakValues;
          this.AdvanceState();
          break;
        }
        this.ReadCurrentPirValue();
        break;
      case UserPanel.State.ReadRgpLeakValues:
        this.nextState = UserPanel.State.ReadComplete;
        this.ReadRgpLeakValues();
        this.AdvanceState();
        break;
      case UserPanel.State.ReadComplete:
        this.ReportResult(Resources.Message_FinishedReadingInjectorCodes);
        this.ClearPartNumbers();
        this.nextState = UserPanel.State.None;
        break;
      case UserPanel.State.Write:
        this.ClearResults();
        this.needReset = new bool[this.currentCylinderCount];
        this.ReportResult(Resources.Message_WritingInjectorCodes);
        this.nextState = UserPanel.State.WriteTrimCodes;
        this.currentCylinder = 0;
        this.AdvanceState();
        break;
      case UserPanel.State.WriteTrimCodes:
        this.NextCylinder();
        if (this.currentCylinder < 0)
        {
          this.currentCylinder = 0;
          this.nextState = UserPanel.State.ResetPirValuesAfterWrite;
          this.AdvanceState();
          break;
        }
        this.WriteCurrentTrimCode();
        break;
      case UserPanel.State.ResetPirValuesAfterWrite:
        this.NextCylinder();
        if (this.currentCylinder < 0)
        {
          this.ReportResult(Resources.Message_ReadingPIRValues1);
          this.currentCylinder = 0;
          this.nextState = UserPanel.State.ReadPirValuesAfterWrite;
          this.AdvanceState();
          break;
        }
        this.ResetCurrentPir();
        break;
      case UserPanel.State.ReadPirValuesAfterWrite:
        this.NextCylinder();
        if (this.currentCylinder < 0)
        {
          this.ReportResult(Resources.Message_PIRParametersRead1);
          this.nextState = UserPanel.State.WriteComplete;
          this.AdvanceState();
          break;
        }
        this.ReadCurrentPirValue();
        break;
      case UserPanel.State.WriteComplete:
        this.ReportResult(Resources.Message_FinishedSendingInjectorCodes);
        this.nextState = UserPanel.State.ResetAllAfterWrite;
        this.CommitToPermanentMemory();
        break;
      case UserPanel.State.ResetAll:
        this.ClearResults();
        this.ReportResult(Resources.Message_ResetAllPIRData);
        this.nextState = UserPanel.State.ResetAllRun;
        this.AdvanceState();
        break;
      case UserPanel.State.ResetAllAfterWrite:
        this.ReportResult(Resources.Message_ResetAllPIRData);
        this.nextState = UserPanel.State.ResetAllRun;
        this.AdvanceState();
        break;
      case UserPanel.State.ResetAllRun:
        this.nextState = UserPanel.State.ResetRpgLeakValues;
        this.ResetAllPir();
        break;
      case UserPanel.State.ResetRpgLeakValues:
        if (this.writeOperationSuccessful)
        {
          this.nextState = UserPanel.State.SaveResetParameterValues;
          this.ResetRpgLeakValues();
          break;
        }
        this.nextState = UserPanel.State.None;
        this.AdvanceState();
        break;
      case UserPanel.State.SaveResetParameterValues:
        if (this.writeOperationSuccessful)
        {
          this.nextState = UserPanel.State.ReadRgpLeakValuesAfterReset;
          this.CommitToPermanentMemory();
          break;
        }
        this.nextState = UserPanel.State.None;
        this.AdvanceState();
        break;
      case UserPanel.State.ReadRgpLeakValuesAfterReset:
        this.nextState = UserPanel.State.ResetAllComplete;
        this.ReadRgpLeakValues();
        this.AdvanceState();
        break;
      case UserPanel.State.ResetAllComplete:
        this.ReportResult(Resources.Message_FinishedResettingPIRData);
        this.nextState = UserPanel.State.None;
        break;
    }
    this.UpdateUserInterface();
  }

  private void NextCylinder()
  {
    if (this.currentCylinder >= 0 && this.currentCylinder < this.currentCylinderCount)
      ++this.currentCylinder;
    else
      this.currentCylinder = -1;
  }

  private string GetEngineTypeName()
  {
    IEnumerable<EquipmentType> source = EquipmentType.ConnectedEquipmentTypes("Engine");
    if (!CollectionExtensions.Exactly<EquipmentType>(source, 1))
      return (string) null;
    EquipmentType equipmentType = source.First<EquipmentType>();
    return ((EquipmentType) ref equipmentType).Name;
  }

  private void UpdateCurrentSetup()
  {
    UserPanel.SetupInformation setupInformation = (UserPanel.SetupInformation) null;
    if (this.mcm != null)
    {
      string engineTypeName = this.GetEngineTypeName();
      if (!string.IsNullOrEmpty(engineTypeName) && this.Setups.Contains(engineTypeName))
        setupInformation = this.Setups[engineTypeName];
    }
    if (setupInformation == this.currentSetup)
      return;
    this.currentSetup = setupInformation;
    this.UpdateControlVisibility();
  }

  private void UpdateControlVisibility()
  {
    foreach (UserPanel.SetupInformation setup in (Collection<UserPanel.SetupInformation>) this.Setups)
    {
      if (this.currentSetup == null || setup.Picture != this.currentSetup.Picture)
        UserPanel.SetControlVisibility((Control) setup.Picture, false);
      else
        UserPanel.SetControlVisibility((Control) setup.Picture, true);
    }
    UserPanel.SetControlVisibility((Control) this.labelDD15Warning, this.currentSetup != null && this.currentSetup.EquipmentType.Equals("DD15", StringComparison.InvariantCultureIgnoreCase));
    this.SetIscControlVisibility(this.currentSetup != null && this.currentSetup.SupportsFeature(UserPanel.Features.Isc));
    this.SetPirControlVisibility(this.currentSetup != null && this.currentSetup.SupportsFeature(UserPanel.Features.Pir));
    this.SetPartNumberControlVisibility(this.currentSetup != null && this.currentSetup.SupportsFeature(UserPanel.Features.RequirePartNumbers));
    this.SetInjectorCodeControlVisibility();
    UserPanel.SetControlVisibility((Control) this.buttonReset, this.currentSetup != null && (this.currentSetup.SupportsFeature(UserPanel.Features.Isc) || this.currentSetup.SupportsFeature(UserPanel.Features.Pir)));
  }

  private void SetInjectorCodeControlVisibility()
  {
    UserPanel.SetControlVisibility((Control) this.textTrimCode5, this.currentCylinderCount >= 5);
    UserPanel.SetControlVisibility((Control) this.checkMark5, this.currentCylinderCount >= 5);
    UserPanel.SetControlVisibility((Control) this.label5, this.currentCylinderCount >= 5);
    UserPanel.SetControlVisibility((Control) this.textTrimCode6, this.currentCylinderCount >= 6);
    UserPanel.SetControlVisibility((Control) this.checkMark6, this.currentCylinderCount >= 6);
    UserPanel.SetControlVisibility((Control) this.label6, this.currentCylinderCount >= 6);
  }

  private void SetIscControlVisibility(bool visible)
  {
    UserPanel.SetControlVisibility((Control) this.iscLabel, visible);
    UserPanel.SetControlVisibility((Control) this.iscInstrument1, visible);
    UserPanel.SetControlVisibility((Control) this.iscInstrument2, visible);
    UserPanel.SetControlVisibility((Control) this.iscInstrument3, visible);
    UserPanel.SetControlVisibility((Control) this.iscInstrument4, visible);
    UserPanel.SetControlVisibility((Control) this.iscInstrument5, visible && this.currentCylinderCount >= 5);
    UserPanel.SetControlVisibility((Control) this.iscInstrument6, visible && this.currentCylinderCount >= 6);
  }

  private void SetPirControlVisibility(bool visible)
  {
    UserPanel.SetControlVisibility((Control) this.minLabel, visible);
    UserPanel.SetControlVisibility((Control) this.minLabel1, visible);
    UserPanel.SetControlVisibility((Control) this.minLabel2, visible);
    UserPanel.SetControlVisibility((Control) this.minLabel3, visible);
    UserPanel.SetControlVisibility((Control) this.minLabel4, visible);
    UserPanel.SetControlVisibility((Control) this.minLabel5, visible && this.currentCylinderCount >= 5);
    UserPanel.SetControlVisibility((Control) this.minLabel6, visible && this.currentCylinderCount >= 6);
    UserPanel.SetControlVisibility((Control) this.maxLabel, visible);
    UserPanel.SetControlVisibility((Control) this.maxLabel1, visible);
    UserPanel.SetControlVisibility((Control) this.maxLabel2, visible);
    UserPanel.SetControlVisibility((Control) this.maxLabel3, visible);
    UserPanel.SetControlVisibility((Control) this.maxLabel4, visible);
    UserPanel.SetControlVisibility((Control) this.maxLabel5, visible && this.currentCylinderCount >= 5);
    UserPanel.SetControlVisibility((Control) this.maxLabel6, visible && this.currentCylinderCount >= 6);
    UserPanel.SetControlVisibility((Control) this.pirLabel, visible);
    UserPanel.SetControlVisibility((Control) this.pirInstrument1, visible);
    UserPanel.SetControlVisibility((Control) this.pirInstrument2, visible);
    UserPanel.SetControlVisibility((Control) this.pirInstrument3, visible);
    UserPanel.SetControlVisibility((Control) this.pirInstrument4, visible);
    UserPanel.SetControlVisibility((Control) this.pirInstrument5, visible && this.currentCylinderCount >= 5);
    UserPanel.SetControlVisibility((Control) this.pirInstrument6, visible && this.currentCylinderCount >= 6);
  }

  private void SetPartNumberControlVisibility(bool visible)
  {
    UserPanel.SetControlVisibility((Control) this.partNumberLabel, visible);
    UserPanel.SetControlVisibility((Control) this.pictureBoxExamplePartNumber, visible);
    UserPanel.SetControlVisibility((Control) this.labelPartNumberEntryTips, visible);
    int num1 = 1;
    foreach (Control partNumberCheckMark in this.partNumberCheckMarks)
    {
      UserPanel.SetControlVisibility(partNumberCheckMark, visible && this.currentCylinderCount >= num1);
      ++num1;
    }
    int num2 = 1;
    foreach (Control partNumber in this.partNumbers)
    {
      UserPanel.SetControlVisibility(partNumber, visible && this.currentCylinderCount >= num2);
      ++num2;
    }
    foreach (Control cylinderLabel in this.cylinderLabels)
      ((TableLayoutPanel) this.mainLayoutPanel).SetColumnSpan(cylinderLabel, visible ? 1 : 3);
  }

  private static void SetControlVisibility(Control control, bool visible)
  {
    if (control == null || control.Visible == visible)
      return;
    control.Visible = visible;
  }

  private void OnChannelStateUpdate(object sender, CommunicationsStateEventArgs e)
  {
    this.UpdateCurrentSetup();
    this.UpdateUserInterface();
  }

  private void OnEngineStateChanged(object sender, EventArgs e) => this.UpdateUserInterface();

  private void UpdateUserInterface()
  {
    this.buttonRead.Enabled = this.CanReadTrimCodes;
    this.buttonWrite.Enabled = this.CanWriteTrimCodes;
    this.buttonReset.Enabled = this.CanResetPirValues;
    this.buttonPrint.Enabled = this.CanPrintCodes;
    this.buttonClose.Enabled = this.CanClose;
    foreach (TextBox trimCode in this.trimCodes)
    {
      trimCode.ReadOnly = !this.Online || this.Working;
      bool flag = this.UpdatePartNumber((int) trimCode.Tag);
      trimCode.Enabled = !trimCode.ReadOnly && flag;
      trimCode.MaxLength = this.MaxTextLength;
      Checkmark checkMark = this.checkMarks[(int) trimCode.Tag];
      if (this.Working || !this.Online)
      {
        trimCode.BackColor = SystemColors.ButtonFace;
        checkMark.CheckState = CheckState.Indeterminate;
      }
      else if (string.IsNullOrEmpty(trimCode.Text) && !flag)
      {
        trimCode.BackColor = SystemColors.Window;
        checkMark.CheckState = CheckState.Indeterminate;
      }
      else if (this.ValidTrimCode(trimCode.Text))
      {
        trimCode.BackColor = Color.PaleGreen;
        checkMark.CheckState = CheckState.Checked;
      }
      else
      {
        trimCode.BackColor = Color.LightPink;
        checkMark.CheckState = CheckState.Unchecked;
      }
    }
  }

  private bool UpdatePartNumber(int partNumberIndex)
  {
    bool flag = this.ValidatePartNumber(partNumberIndex);
    if (flag)
    {
      this.partNumbers[partNumberIndex].BackColor = SystemColors.Window;
      this.partNumberCheckMarks[partNumberIndex].CheckState = CheckState.Checked;
    }
    else if (string.IsNullOrEmpty(this.partNumbers[partNumberIndex].Text))
    {
      this.partNumbers[partNumberIndex].BackColor = SystemColors.Window;
      this.partNumberCheckMarks[partNumberIndex].CheckState = CheckState.Indeterminate;
    }
    else
    {
      this.partNumbers[partNumberIndex].BackColor = Color.LightPink;
      this.partNumberCheckMarks[partNumberIndex].CheckState = CheckState.Unchecked;
    }
    return flag;
  }

  private void ResetData()
  {
    this.nextState = UserPanel.State.None;
    this.ClearPartNumbers();
    foreach (Control trimCode in this.trimCodes)
      trimCode.Text = string.Empty;
    this.ClearResults();
  }

  private void ClearResults()
  {
    if (this.textboxResults == null)
      return;
    this.textboxResults.Text = string.Empty;
  }

  private void ClearPartNumbers()
  {
    foreach (Control partNumber in this.partNumbers)
      partNumber.Text = string.Empty;
  }

  private void ReportResult(string text)
  {
    if (this.textboxResults != null)
    {
      this.textboxResults.AppendText(text + "\r\n");
      this.textboxResults.SelectionStart = this.textboxResults.TextLength;
      this.textboxResults.SelectionLength = 0;
      this.textboxResults.ScrollToCaret();
    }
    this.AddStatusMessage(text);
  }

  private bool ValidTrimCode(string trimCode)
  {
    return this.currentSetup != null && this.currentSetup.ValidTrimCode(trimCode);
  }

  private void OnTextChanged(object sender, EventArgs e) => this.UpdateUserInterface();

  private void OnKeyPress(object sender, KeyPressEventArgs e)
  {
    if (e.KeyChar == '\b')
      return;
    char keyChar = e.KeyChar;
    if (this.currentSetup != null)
    {
      if (this.currentSetup.ValidateAndCorrect(ref keyChar))
        e.KeyChar = keyChar;
      else
        e.Handled = true;
    }
  }

  private void OnReadClick(object sender, EventArgs e) => this.ReadTrimCodes();

  private void ReadTrimCodes()
  {
    if (!this.CanReadTrimCodes)
      return;
    this.nextState = UserPanel.State.Read;
    this.AdvanceState();
  }

  private void OnWriteClick(object sender, EventArgs e)
  {
    if (!this.CanWriteTrimCodes)
      return;
    this.nextState = UserPanel.State.Write;
    this.AdvanceState();
  }

  private void CommitToPermanentMemory()
  {
    if (this.mcm != null && this.mcm.Ecu.Properties.ContainsKey("CommitToPermanentMemoryService"))
    {
      this.mcm.Services.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.Services_ServiceCompleteEvent);
      this.mcm.Services.Execute(this.mcm.Ecu.Properties["CommitToPermanentMemoryService"], false);
    }
    else
    {
      this.writeOperationSuccessful = false;
      this.AdvanceState();
    }
  }

  private void Services_ServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    this.mcm.Services.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.Services_ServiceCompleteEvent);
    this.AdvanceState();
  }

  private void ReadCurrentTrimCode()
  {
    this.currentService = this.GetService("MCM21T", "DJ_Read_E_Trim");
    if (this.currentService != (Service) null)
    {
      this.currentService.InputValues[0].Value = (object) this.currentCylinder;
      this.currentService.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.OnReadServiceComplete);
      this.currentService.Execute(false);
    }
    else
      this.AdvanceState();
  }

  private void OnReadServiceComplete(object sender, ResultEventArgs e)
  {
    this.currentService.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.OnReadServiceComplete);
    if (e.Succeeded)
    {
      string trimCode = this.currentService.OutputValues[0].Value.ToString();
      string str = this.currentSetup.AddCrc(trimCode);
      if (trimCode.Length == this.currentSetup.CodeLengthWithoutCRC)
      {
        if (str.Contains("?"))
        {
          this.ReportResult(string.Format(Resources.MessageFormat_Cylinder0InjectorCodeHasAnInvalidValueOf1, (object) this.currentCylinder, (object) str));
          this.readOperationSuccessful = false;
        }
        else
        {
          this.oldTrimCodes[this.currentCylinder - 1] = str;
          this.ReportResult(string.Format(Resources.MessageFormat_Cylinder0InjectorCodeIs1, (object) this.currentCylinder, (object) str));
          this.readOperationSuccessful = true;
        }
      }
      else
      {
        this.ReportResult(string.Format(Resources.MessageFormat_Cylinder0InjectorCodeCouldNotBeRead1, (object) this.currentCylinder, (object) this.currentService.OutputValues[0].Value.ToString()));
        this.readOperationSuccessful = false;
      }
      this.trimCodes[this.currentCylinder - 1].Text = str;
      this.currentService = (Service) null;
    }
    else
    {
      this.trimCodes[this.currentCylinder - 1].Text = new string('?', this.currentSetup.CodeLengthWithCRC);
      this.ReportResult(string.Format(Resources.MessageFormat_Cylinder0InjectorCodeCouldNotBeReadServiceFailedToExecute, (object) this.currentCylinder));
      this.readOperationSuccessful = false;
    }
    this.AdvanceState();
  }

  private void ReadRgpLeakValues()
  {
    Channel channel = this.GetChannel("MCM21T");
    if (channel == null)
      return;
    EcuInfo ecuInfo = channel.EcuInfos["DT_STO_ACC047_OP_Data_4_HP_Leak_Learned_Value"];
    if (ecuInfo != null && ecuInfo.Channel.Online)
      ecuInfo.Read(false);
  }

  private void WriteCurrentTrimCode()
  {
    string text = this.trimCodes[this.currentCylinder - 1].Text;
    if (this.ValidTrimCode(text))
    {
      this.currentService = this.GetService("MCM21T", "DJ_Write_E_Trim");
      if (this.currentService != (Service) null)
      {
        this.ReportResult(string.Format(Resources.MessageFormat_SendingCylinder0InjectorCodeAs1, (object) this.currentCylinder, (object) text));
        this.currentService.InputValues[0].Value = (object) this.currentCylinder;
        this.currentService.InputValues[1].Value = (object) this.currentSetup.StripCrc(text);
        this.currentService.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.OnWriteServiceComplete);
        this.currentService.Execute(false);
        this.needReset[this.currentCylinder - 1] = text != this.oldTrimCodes[this.currentCylinder - 1];
      }
      else
      {
        this.ReportResult(string.Format(Resources.MessageFormat_Cylinder0InjectorCodeCouldNotBeSentServiceWasUnavailable, (object) this.currentCylinder));
        this.writeOperationSuccessful = false;
        this.AdvanceState();
      }
    }
    else
      this.AdvanceState();
  }

  private void OnWriteServiceComplete(object sender, ResultEventArgs e)
  {
    this.currentService.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.OnWriteServiceComplete);
    if (e.Succeeded)
    {
      this.ReportResult(string.Format(Resources.MessageFormat_Cylinder01, (object) this.currentCylinder, (object) this.currentService.OutputValues[0].Value.ToString()));
      this.currentService = (Service) null;
      this.writeOperationSuccessful = true;
    }
    else
    {
      this.ReportResult(string.Format(Resources.MessageFormat_Cylinder0InjectorCodeCouldNotBeSentServiceFailedToExecute, (object) this.currentCylinder));
      this.writeOperationSuccessful = false;
    }
    this.AdvanceState();
  }

  private void OnEcuInfosReadCompleteEvent(object sender, ResultEventArgs e)
  {
    this.mcm.EcuInfos.EcuInfosReadCompleteEvent -= new EcuInfosReadCompleteEventHandler(this.OnEcuInfosReadCompleteEvent);
    if (e.Succeeded)
    {
      this.ReportPirResult();
    }
    else
    {
      this.ReportResult(string.Format(Resources.MessageFormat_AnErrorOccurredWhileReadingParameters, (object) e.Exception.Message));
      this.readOperationSuccessful = false;
    }
    this.AdvanceState();
  }

  private void ReportPirResult()
  {
    EcuInfo ecuInfo;
    if (this.mcm != null && (ecuInfo = this.mcm.EcuInfos[UserPanel.pirQualifiers[this.currentCylinder - 1]]) != null)
    {
      this.ReportResult(string.Format(Resources.MessageFormat_PIRForCylinder0Read1, (object) this.currentCylinder, (object) ecuInfo.Value));
      this.readOperationSuccessful = true;
    }
    else
    {
      this.ReportResult(string.Format(Resources.MessageFormat_PIRForCylinder0CouldNotBeRead, (object) this.currentCylinder));
      this.readOperationSuccessful = false;
    }
  }

  private void ReadCurrentPirValue()
  {
    bool flag = false;
    if (this.mcm != null)
    {
      if (this.currentCylinder == 0)
      {
        if (this.mcm.EcuInfos[UserPanel.pirQualifiers[this.currentCylinder - 1]] != null)
        {
          this.mcm.EcuInfos.EcuInfosReadCompleteEvent += new EcuInfosReadCompleteEventHandler(this.OnEcuInfosReadCompleteEvent);
          this.mcm.EcuInfos.Read(false);
          flag = true;
        }
      }
      else
        this.ReportPirResult();
    }
    if (flag)
      return;
    this.AdvanceState();
  }

  private void ResetCurrentPir()
  {
    bool flag = false;
    if (this.needReset[this.currentCylinder - 1])
    {
      this.currentService = this.GetService("MCM21T", "RT_SR070_Injector_Change_Start");
      if (this.currentService != (Service) null)
      {
        this.currentService.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.OnResetServiceCompleteEvent);
        this.currentService.InputValues[0].Value = (object) this.currentService.InputValues[0].Choices[this.currentCylinder - 1];
        this.currentService.Execute(false);
        flag = true;
      }
    }
    if (flag)
      return;
    this.AdvanceState();
  }

  private void OnResetServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    this.currentService.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.OnResetServiceCompleteEvent);
    if (e.Succeeded)
    {
      this.ReportResult(string.Format(Resources.MessageFormat_SuccessfullyResetPIRDataForCylinder0, (object) this.currentCylinder));
      this.writeOperationSuccessful = true;
    }
    else
    {
      this.ReportResult(string.Format(Resources.MessageFormat_Cylinder0FailedToResetPIRDataServiceFailedToExecute, (object) this.currentCylinder));
      this.writeOperationSuccessful = false;
    }
    this.AdvanceState();
  }

  private void OnResetClick(object sender, EventArgs e)
  {
    this.nextState = UserPanel.State.ResetAll;
    this.AdvanceState();
  }

  public virtual bool CanProvideHtml
  {
    get
    {
      return !this.Working && this.textboxResults.TextLength > 0 && (this.Online || this.currentSetup == null);
    }
  }

  public virtual string ToHtml()
  {
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.Append("<div class=\"standard\">");
    stringBuilder.Append("<p class=\"standard\">");
    foreach (string line in this.textboxResults.Lines)
      stringBuilder.AppendFormat("{0}<br/>", (object) line);
    stringBuilder.Append("</p></div>");
    return stringBuilder.ToString();
  }

  private void OnPartNumberChanged(object sender, EventArgs e) => this.UpdateUserInterface();

  private bool ValidatePartNumber(int partNumberIndex)
  {
    if (this.currentSetup != null && !this.currentSetup.SupportsFeature(UserPanel.Features.RequirePartNumbers))
      return true;
    Regex injectorPartValidator;
    switch ((this.GetEngineTypeName() ?? "").ToUpper())
    {
      case "DD13":
      case "DD13 TIER4":
      case "DD13EURO5":
      case "DD13 STAGEV":
        injectorPartValidator = this.dd13InjectorPartValidator;
        break;
      case "DD15":
      case "DD16":
      case "DD16EURO5":
      case "DD16 STAGEV":
        injectorPartValidator = this.dd15Or16InjectorPartValidator;
        break;
      case "DD16 TIER4":
        injectorPartValidator = this.dd16Tier4InjectorPartValidator;
        break;
      default:
        return false;
    }
    return injectorPartValidator.IsMatch(this.partNumbers[partNumberIndex].Text);
  }

  private void OnPrintClick(object sender, EventArgs e)
  {
    if (!this.CanPrintCodes)
      return;
    PrintHelper.ShowPrintDialog(((ContainerControl) this).ParentForm.Text, (IProvideHtml) this, (IncludeInfo) 3);
  }

  private void ResetAllPir()
  {
    this.currentService = this.GetService("MCM21T", "RT_SR074_EcuInitPIR_Start");
    if (this.currentService != (Service) null)
    {
      this.currentService.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.OnResetAllServiceCompleteEvent);
      this.currentService.Execute(false);
    }
    else
      this.AdvanceState();
  }

  private void OnResetAllServiceCompleteEvent(object sender, ResultEventArgs e)
  {
    this.currentService.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.OnResetAllServiceCompleteEvent);
    if (e.Succeeded)
    {
      this.ReportResult(Resources.Message_SuccessfullyResetPIRDataForAllCylinders);
      this.writeOperationSuccessful = true;
    }
    else
    {
      this.ReportResult(Resources.Message_FailedToResetPIRDataForAllCylindersServiceFailedToExecute);
      this.writeOperationSuccessful = false;
    }
    this.AdvanceState();
  }

  private void ResetRpgLeakValues()
  {
    this.currentService = this.GetService("MCM21T", "RT_SR014_SET_EOL_Default_Values_Start");
    if (this.currentService != (Service) null)
    {
      this.currentService.ServiceCompleteEvent += new ServiceCompleteEventHandler(this.OnResetRpgLeakValuesCompleteEvent);
      this.currentService.InputValues[0].Value = (object) this.currentService.InputValues[0].Choices.GetItemFromRawValue((object) 24);
      this.currentService.Execute(false);
    }
    else
      this.AdvanceState();
  }

  private void OnResetRpgLeakValuesCompleteEvent(object sender, ResultEventArgs e)
  {
    this.currentService.ServiceCompleteEvent -= new ServiceCompleteEventHandler(this.OnResetRpgLeakValuesCompleteEvent);
    if (e.Succeeded)
    {
      this.ReportResult(Resources.Message_RPGLeakValuesHaveBeenReset);
    }
    else
    {
      this.writeOperationSuccessful = false;
      this.ReportResult(Resources.Message_FailedToResetRPGLeakValues);
    }
    this.AdvanceState();
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.mainLayoutPanel = new TableLayoutPanel();
    this.labelEntryTips = new System.Windows.Forms.Label();
    this.pictureBoxExamplePartNumber = new PictureBox();
    this.partNumberLabel = new System.Windows.Forms.Label();
    this.textBoxPartNumber1 = new TextBox();
    this.checkmarkPartNumber1 = new Checkmark();
    this.checkmarkPartNumber2 = new Checkmark();
    this.textBoxPartNumber2 = new TextBox();
    this.checkmarkPartNumber3 = new Checkmark();
    this.textBoxPartNumber3 = new TextBox();
    this.checkmarkPartNumber4 = new Checkmark();
    this.textBoxPartNumber4 = new TextBox();
    this.checkmarkPartNumber5 = new Checkmark();
    this.textBoxPartNumber5 = new TextBox();
    this.checkmarkPartNumber6 = new Checkmark();
    this.textBoxPartNumber6 = new TextBox();
    this.maxLabel6 = new System.Windows.Forms.Label();
    this.textTrimCode6 = new TextBox();
    this.textTrimCode5 = new TextBox();
    this.textTrimCode4 = new TextBox();
    this.textTrimCode3 = new TextBox();
    this.textTrimCode2 = new TextBox();
    this.textTrimCode1 = new TextBox();
    this.checkMark6 = new Checkmark();
    this.checkMark4 = new Checkmark();
    this.checkMark5 = new Checkmark();
    this.maxLabel5 = new System.Windows.Forms.Label();
    this.maxLabel4 = new System.Windows.Forms.Label();
    this.maxLabel3 = new System.Windows.Forms.Label();
    this.checkMark3 = new Checkmark();
    this.maxLabel2 = new System.Windows.Forms.Label();
    this.checkMark2 = new Checkmark();
    this.maxLabel1 = new System.Windows.Forms.Label();
    this.checkMark1 = new Checkmark();
    this.minLabel6 = new System.Windows.Forms.Label();
    this.minLabel5 = new System.Windows.Forms.Label();
    this.minLabel4 = new System.Windows.Forms.Label();
    this.minLabel3 = new System.Windows.Forms.Label();
    this.minLabel2 = new System.Windows.Forms.Label();
    this.minLabel1 = new System.Windows.Forms.Label();
    this.maxLabel = new System.Windows.Forms.Label();
    this.iscInstrument6 = new DigitalReadoutInstrument();
    this.pirLabel = new System.Windows.Forms.Label();
    this.iscInstrument1 = new DigitalReadoutInstrument();
    this.pirInstrument6 = new DigitalReadoutInstrument();
    this.pirInstrument5 = new DigitalReadoutInstrument();
    this.pirInstrument4 = new DigitalReadoutInstrument();
    this.pirInstrument3 = new DigitalReadoutInstrument();
    this.pirInstrument2 = new DigitalReadoutInstrument();
    this.pirInstrument1 = new DigitalReadoutInstrument();
    this.iscInstrument5 = new DigitalReadoutInstrument();
    this.iscInstrument4 = new DigitalReadoutInstrument();
    this.lblCylinderPosition = new System.Windows.Forms.Label();
    this.iscInstrument3 = new DigitalReadoutInstrument();
    this.iscInstrument2 = new DigitalReadoutInstrument();
    this.minLabel = new System.Windows.Forms.Label();
    this.textboxResults = new TextBox();
    this.label1 = new System.Windows.Forms.Label();
    this.label6 = new System.Windows.Forms.Label();
    this.label5 = new System.Windows.Forms.Label();
    this.label4 = new System.Windows.Forms.Label();
    this.label3 = new System.Windows.Forms.Label();
    this.label2 = new System.Windows.Forms.Label();
    this.label20 = new System.Windows.Forms.Label();
    this.iscLabel = new System.Windows.Forms.Label();
    this.buttonClose = new Button();
    this.flowLayoutPanel1 = new FlowLayoutPanel();
    this.buttonRead = new Button();
    this.buttonWrite = new Button();
    this.buttonReset = new Button();
    this.buttonPrint = new Button();
    this.digitalReadoutInstrumentEngineState = new DigitalReadoutInstrument();
    this.digitalReadoutLeakLearnedValue = new DigitalReadoutInstrument();
    this.labelPartNumberEntryTips = new System.Windows.Forms.Label();
    this.panelPictures = new Panel();
    this.pictureBoxHDEP = new PictureBox();
    this.labelDD15Warning = new System.Windows.Forms.Label();
    ((Control) this.mainLayoutPanel).SuspendLayout();
    ((ISupportInitialize) this.pictureBoxExamplePartNumber).BeginInit();
    this.flowLayoutPanel1.SuspendLayout();
    this.panelPictures.SuspendLayout();
    ((ISupportInitialize) this.pictureBoxHDEP).BeginInit();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.mainLayoutPanel, "mainLayoutPanel");
    ((TableLayoutPanel) this.mainLayoutPanel).Controls.Add((Control) this.labelEntryTips, 3, 0);
    ((TableLayoutPanel) this.mainLayoutPanel).Controls.Add((Control) this.pictureBoxExamplePartNumber, 2, 9);
    ((TableLayoutPanel) this.mainLayoutPanel).Controls.Add((Control) this.partNumberLabel, 2, 2);
    ((TableLayoutPanel) this.mainLayoutPanel).Controls.Add((Control) this.textBoxPartNumber1, 2, 3);
    ((TableLayoutPanel) this.mainLayoutPanel).Controls.Add((Control) this.checkmarkPartNumber1, 1, 3);
    ((TableLayoutPanel) this.mainLayoutPanel).Controls.Add((Control) this.checkmarkPartNumber2, 1, 4);
    ((TableLayoutPanel) this.mainLayoutPanel).Controls.Add((Control) this.textBoxPartNumber2, 2, 4);
    ((TableLayoutPanel) this.mainLayoutPanel).Controls.Add((Control) this.checkmarkPartNumber3, 1, 5);
    ((TableLayoutPanel) this.mainLayoutPanel).Controls.Add((Control) this.textBoxPartNumber3, 2, 5);
    ((TableLayoutPanel) this.mainLayoutPanel).Controls.Add((Control) this.checkmarkPartNumber4, 1, 6);
    ((TableLayoutPanel) this.mainLayoutPanel).Controls.Add((Control) this.textBoxPartNumber4, 2, 6);
    ((TableLayoutPanel) this.mainLayoutPanel).Controls.Add((Control) this.checkmarkPartNumber5, 1, 7);
    ((TableLayoutPanel) this.mainLayoutPanel).Controls.Add((Control) this.textBoxPartNumber5, 2, 7);
    ((TableLayoutPanel) this.mainLayoutPanel).Controls.Add((Control) this.checkmarkPartNumber6, 1, 8);
    ((TableLayoutPanel) this.mainLayoutPanel).Controls.Add((Control) this.textBoxPartNumber6, 2, 8);
    ((TableLayoutPanel) this.mainLayoutPanel).Controls.Add((Control) this.maxLabel6, 9, 8);
    ((TableLayoutPanel) this.mainLayoutPanel).Controls.Add((Control) this.textTrimCode6, 4, 8);
    ((TableLayoutPanel) this.mainLayoutPanel).Controls.Add((Control) this.textTrimCode5, 4, 7);
    ((TableLayoutPanel) this.mainLayoutPanel).Controls.Add((Control) this.textTrimCode4, 4, 6);
    ((TableLayoutPanel) this.mainLayoutPanel).Controls.Add((Control) this.textTrimCode3, 4, 5);
    ((TableLayoutPanel) this.mainLayoutPanel).Controls.Add((Control) this.textTrimCode2, 4, 4);
    ((TableLayoutPanel) this.mainLayoutPanel).Controls.Add((Control) this.textTrimCode1, 4, 3);
    ((TableLayoutPanel) this.mainLayoutPanel).Controls.Add((Control) this.checkMark6, 3, 8);
    ((TableLayoutPanel) this.mainLayoutPanel).Controls.Add((Control) this.checkMark4, 3, 6);
    ((TableLayoutPanel) this.mainLayoutPanel).Controls.Add((Control) this.checkMark5, 3, 7);
    ((TableLayoutPanel) this.mainLayoutPanel).Controls.Add((Control) this.maxLabel5, 9, 7);
    ((TableLayoutPanel) this.mainLayoutPanel).Controls.Add((Control) this.maxLabel4, 9, 6);
    ((TableLayoutPanel) this.mainLayoutPanel).Controls.Add((Control) this.maxLabel3, 9, 5);
    ((TableLayoutPanel) this.mainLayoutPanel).Controls.Add((Control) this.checkMark3, 3, 5);
    ((TableLayoutPanel) this.mainLayoutPanel).Controls.Add((Control) this.maxLabel2, 9, 4);
    ((TableLayoutPanel) this.mainLayoutPanel).Controls.Add((Control) this.checkMark2, 3, 4);
    ((TableLayoutPanel) this.mainLayoutPanel).Controls.Add((Control) this.maxLabel1, 9, 3);
    ((TableLayoutPanel) this.mainLayoutPanel).Controls.Add((Control) this.checkMark1, 3, 3);
    ((TableLayoutPanel) this.mainLayoutPanel).Controls.Add((Control) this.minLabel6, 7, 8);
    ((TableLayoutPanel) this.mainLayoutPanel).Controls.Add((Control) this.minLabel5, 7, 7);
    ((TableLayoutPanel) this.mainLayoutPanel).Controls.Add((Control) this.minLabel4, 7, 6);
    ((TableLayoutPanel) this.mainLayoutPanel).Controls.Add((Control) this.minLabel3, 7, 5);
    ((TableLayoutPanel) this.mainLayoutPanel).Controls.Add((Control) this.minLabel2, 7, 4);
    ((TableLayoutPanel) this.mainLayoutPanel).Controls.Add((Control) this.minLabel1, 7, 3);
    ((TableLayoutPanel) this.mainLayoutPanel).Controls.Add((Control) this.maxLabel, 9, 2);
    ((TableLayoutPanel) this.mainLayoutPanel).Controls.Add((Control) this.iscInstrument6, 5, 8);
    ((TableLayoutPanel) this.mainLayoutPanel).Controls.Add((Control) this.pirLabel, 8, 2);
    ((TableLayoutPanel) this.mainLayoutPanel).Controls.Add((Control) this.iscInstrument1, 5, 3);
    ((TableLayoutPanel) this.mainLayoutPanel).Controls.Add((Control) this.pirInstrument6, 8, 8);
    ((TableLayoutPanel) this.mainLayoutPanel).Controls.Add((Control) this.pirInstrument5, 8, 7);
    ((TableLayoutPanel) this.mainLayoutPanel).Controls.Add((Control) this.pirInstrument4, 8, 6);
    ((TableLayoutPanel) this.mainLayoutPanel).Controls.Add((Control) this.pirInstrument3, 8, 5);
    ((TableLayoutPanel) this.mainLayoutPanel).Controls.Add((Control) this.pirInstrument2, 8, 4);
    ((TableLayoutPanel) this.mainLayoutPanel).Controls.Add((Control) this.pirInstrument1, 8, 3);
    ((TableLayoutPanel) this.mainLayoutPanel).Controls.Add((Control) this.iscInstrument5, 5, 7);
    ((TableLayoutPanel) this.mainLayoutPanel).Controls.Add((Control) this.iscInstrument4, 5, 6);
    ((TableLayoutPanel) this.mainLayoutPanel).Controls.Add((Control) this.lblCylinderPosition, 0, 2);
    ((TableLayoutPanel) this.mainLayoutPanel).Controls.Add((Control) this.iscInstrument3, 5, 5);
    ((TableLayoutPanel) this.mainLayoutPanel).Controls.Add((Control) this.iscInstrument2, 5, 4);
    ((TableLayoutPanel) this.mainLayoutPanel).Controls.Add((Control) this.minLabel, 7, 2);
    ((TableLayoutPanel) this.mainLayoutPanel).Controls.Add((Control) this.textboxResults, 6, 9);
    ((TableLayoutPanel) this.mainLayoutPanel).Controls.Add((Control) this.label1, 0, 3);
    ((TableLayoutPanel) this.mainLayoutPanel).Controls.Add((Control) this.label6, 0, 8);
    ((TableLayoutPanel) this.mainLayoutPanel).Controls.Add((Control) this.label5, 0, 7);
    ((TableLayoutPanel) this.mainLayoutPanel).Controls.Add((Control) this.label4, 0, 6);
    ((TableLayoutPanel) this.mainLayoutPanel).Controls.Add((Control) this.label3, 0, 5);
    ((TableLayoutPanel) this.mainLayoutPanel).Controls.Add((Control) this.label2, 0, 4);
    ((TableLayoutPanel) this.mainLayoutPanel).Controls.Add((Control) this.label20, 4, 2);
    ((TableLayoutPanel) this.mainLayoutPanel).Controls.Add((Control) this.iscLabel, 5, 2);
    ((TableLayoutPanel) this.mainLayoutPanel).Controls.Add((Control) this.buttonClose, 9, 13);
    ((TableLayoutPanel) this.mainLayoutPanel).Controls.Add((Control) this.flowLayoutPanel1, 0, 12);
    ((TableLayoutPanel) this.mainLayoutPanel).Controls.Add((Control) this.digitalReadoutInstrumentEngineState, 7, 0);
    ((TableLayoutPanel) this.mainLayoutPanel).Controls.Add((Control) this.digitalReadoutLeakLearnedValue, 7, 1);
    ((TableLayoutPanel) this.mainLayoutPanel).Controls.Add((Control) this.labelPartNumberEntryTips, 0, 0);
    ((TableLayoutPanel) this.mainLayoutPanel).Controls.Add((Control) this.panelPictures, 4, 9);
    ((TableLayoutPanel) this.mainLayoutPanel).Controls.Add((Control) this.labelDD15Warning, 0, 11);
    ((Control) this.mainLayoutPanel).Name = "mainLayoutPanel";
    ((TableLayoutPanel) this.mainLayoutPanel).SetColumnSpan((Control) this.labelEntryTips, 4);
    componentResourceManager.ApplyResources((object) this.labelEntryTips, "labelEntryTips");
    this.labelEntryTips.Name = "labelEntryTips";
    ((TableLayoutPanel) this.mainLayoutPanel).SetRowSpan((Control) this.labelEntryTips, 2);
    this.labelEntryTips.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.pictureBoxExamplePartNumber, "pictureBoxExamplePartNumber");
    this.pictureBoxExamplePartNumber.Name = "pictureBoxExamplePartNumber";
    this.pictureBoxExamplePartNumber.TabStop = false;
    componentResourceManager.ApplyResources((object) this.partNumberLabel, "partNumberLabel");
    this.partNumberLabel.Name = "partNumberLabel";
    this.partNumberLabel.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.textBoxPartNumber1, "textBoxPartNumber1");
    this.textBoxPartNumber1.Name = "textBoxPartNumber1";
    this.checkmarkPartNumber1.CheckState = CheckState.Indeterminate;
    componentResourceManager.ApplyResources((object) this.checkmarkPartNumber1, "checkmarkPartNumber1");
    ((Control) this.checkmarkPartNumber1).Name = "checkmarkPartNumber1";
    this.checkmarkPartNumber2.CheckState = CheckState.Indeterminate;
    componentResourceManager.ApplyResources((object) this.checkmarkPartNumber2, "checkmarkPartNumber2");
    ((Control) this.checkmarkPartNumber2).Name = "checkmarkPartNumber2";
    componentResourceManager.ApplyResources((object) this.textBoxPartNumber2, "textBoxPartNumber2");
    this.textBoxPartNumber2.Name = "textBoxPartNumber2";
    this.checkmarkPartNumber3.CheckState = CheckState.Indeterminate;
    componentResourceManager.ApplyResources((object) this.checkmarkPartNumber3, "checkmarkPartNumber3");
    ((Control) this.checkmarkPartNumber3).Name = "checkmarkPartNumber3";
    componentResourceManager.ApplyResources((object) this.textBoxPartNumber3, "textBoxPartNumber3");
    this.textBoxPartNumber3.Name = "textBoxPartNumber3";
    this.checkmarkPartNumber4.CheckState = CheckState.Indeterminate;
    componentResourceManager.ApplyResources((object) this.checkmarkPartNumber4, "checkmarkPartNumber4");
    ((Control) this.checkmarkPartNumber4).Name = "checkmarkPartNumber4";
    componentResourceManager.ApplyResources((object) this.textBoxPartNumber4, "textBoxPartNumber4");
    this.textBoxPartNumber4.Name = "textBoxPartNumber4";
    this.checkmarkPartNumber5.CheckState = CheckState.Indeterminate;
    componentResourceManager.ApplyResources((object) this.checkmarkPartNumber5, "checkmarkPartNumber5");
    ((Control) this.checkmarkPartNumber5).Name = "checkmarkPartNumber5";
    componentResourceManager.ApplyResources((object) this.textBoxPartNumber5, "textBoxPartNumber5");
    this.textBoxPartNumber5.Name = "textBoxPartNumber5";
    this.checkmarkPartNumber6.CheckState = CheckState.Indeterminate;
    componentResourceManager.ApplyResources((object) this.checkmarkPartNumber6, "checkmarkPartNumber6");
    ((Control) this.checkmarkPartNumber6).Name = "checkmarkPartNumber6";
    componentResourceManager.ApplyResources((object) this.textBoxPartNumber6, "textBoxPartNumber6");
    this.textBoxPartNumber6.Name = "textBoxPartNumber6";
    componentResourceManager.ApplyResources((object) this.maxLabel6, "maxLabel6");
    this.maxLabel6.Name = "maxLabel6";
    this.maxLabel6.UseCompatibleTextRendering = true;
    this.textTrimCode6.CharacterCasing = CharacterCasing.Upper;
    componentResourceManager.ApplyResources((object) this.textTrimCode6, "textTrimCode6");
    this.textTrimCode6.Name = "textTrimCode6";
    this.textTrimCode5.CharacterCasing = CharacterCasing.Upper;
    componentResourceManager.ApplyResources((object) this.textTrimCode5, "textTrimCode5");
    this.textTrimCode5.Name = "textTrimCode5";
    this.textTrimCode4.CharacterCasing = CharacterCasing.Upper;
    componentResourceManager.ApplyResources((object) this.textTrimCode4, "textTrimCode4");
    this.textTrimCode4.Name = "textTrimCode4";
    this.textTrimCode3.CharacterCasing = CharacterCasing.Upper;
    componentResourceManager.ApplyResources((object) this.textTrimCode3, "textTrimCode3");
    this.textTrimCode3.Name = "textTrimCode3";
    this.textTrimCode2.CharacterCasing = CharacterCasing.Upper;
    componentResourceManager.ApplyResources((object) this.textTrimCode2, "textTrimCode2");
    this.textTrimCode2.Name = "textTrimCode2";
    this.textTrimCode1.CharacterCasing = CharacterCasing.Upper;
    componentResourceManager.ApplyResources((object) this.textTrimCode1, "textTrimCode1");
    this.textTrimCode1.Name = "textTrimCode1";
    this.checkMark6.CheckState = CheckState.Indeterminate;
    componentResourceManager.ApplyResources((object) this.checkMark6, "checkMark6");
    ((Control) this.checkMark6).Name = "checkMark6";
    this.checkMark4.CheckState = CheckState.Indeterminate;
    componentResourceManager.ApplyResources((object) this.checkMark4, "checkMark4");
    ((Control) this.checkMark4).Name = "checkMark4";
    this.checkMark5.CheckState = CheckState.Indeterminate;
    componentResourceManager.ApplyResources((object) this.checkMark5, "checkMark5");
    ((Control) this.checkMark5).Name = "checkMark5";
    componentResourceManager.ApplyResources((object) this.maxLabel5, "maxLabel5");
    this.maxLabel5.Name = "maxLabel5";
    this.maxLabel5.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.maxLabel4, "maxLabel4");
    this.maxLabel4.Name = "maxLabel4";
    this.maxLabel4.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.maxLabel3, "maxLabel3");
    this.maxLabel3.Name = "maxLabel3";
    this.maxLabel3.UseCompatibleTextRendering = true;
    this.checkMark3.CheckState = CheckState.Indeterminate;
    componentResourceManager.ApplyResources((object) this.checkMark3, "checkMark3");
    ((Control) this.checkMark3).Name = "checkMark3";
    componentResourceManager.ApplyResources((object) this.maxLabel2, "maxLabel2");
    this.maxLabel2.Name = "maxLabel2";
    this.maxLabel2.UseCompatibleTextRendering = true;
    this.checkMark2.CheckState = CheckState.Indeterminate;
    componentResourceManager.ApplyResources((object) this.checkMark2, "checkMark2");
    ((Control) this.checkMark2).Name = "checkMark2";
    componentResourceManager.ApplyResources((object) this.maxLabel1, "maxLabel1");
    this.maxLabel1.Name = "maxLabel1";
    this.maxLabel1.UseCompatibleTextRendering = true;
    this.checkMark1.CheckState = CheckState.Indeterminate;
    componentResourceManager.ApplyResources((object) this.checkMark1, "checkMark1");
    ((Control) this.checkMark1).Name = "checkMark1";
    componentResourceManager.ApplyResources((object) this.minLabel6, "minLabel6");
    this.minLabel6.Name = "minLabel6";
    this.minLabel6.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.minLabel5, "minLabel5");
    this.minLabel5.Name = "minLabel5";
    this.minLabel5.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.minLabel4, "minLabel4");
    this.minLabel4.Name = "minLabel4";
    this.minLabel4.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.minLabel3, "minLabel3");
    this.minLabel3.Name = "minLabel3";
    this.minLabel3.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.minLabel2, "minLabel2");
    this.minLabel2.Name = "minLabel2";
    this.minLabel2.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.minLabel1, "minLabel1");
    this.minLabel1.Name = "minLabel1";
    this.minLabel1.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.maxLabel, "maxLabel");
    this.maxLabel.Name = "maxLabel";
    this.maxLabel.UseCompatibleTextRendering = true;
    ((TableLayoutPanel) this.mainLayoutPanel).SetColumnSpan((Control) this.iscInstrument6, 2);
    componentResourceManager.ApplyResources((object) this.iscInstrument6, "iscInstrument6");
    this.iscInstrument6.FontGroup = (string) null;
    ((SingleInstrumentBase) this.iscInstrument6).FreezeValue = false;
    this.iscInstrument6.Gradient.Initialize((ValueState) 3, 2);
    this.iscInstrument6.Gradient.Modify(1, -100.0, (ValueState) 1);
    this.iscInstrument6.Gradient.Modify(2, 99.5, (ValueState) 3);
    ((SingleInstrumentBase) this.iscInstrument6).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "DT_Idle_Speed_Balance_Values_Cylinder_6");
    ((Control) this.iscInstrument6).Name = "iscInstrument6";
    ((SingleInstrumentBase) this.iscInstrument6).TitlePosition = (LabelPosition) 0;
    ((SingleInstrumentBase) this.iscInstrument6).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.pirLabel, "pirLabel");
    this.pirLabel.Name = "pirLabel";
    this.pirLabel.UseCompatibleTextRendering = true;
    ((TableLayoutPanel) this.mainLayoutPanel).SetColumnSpan((Control) this.iscInstrument1, 2);
    componentResourceManager.ApplyResources((object) this.iscInstrument1, "iscInstrument1");
    this.iscInstrument1.FontGroup = (string) null;
    ((SingleInstrumentBase) this.iscInstrument1).FreezeValue = false;
    this.iscInstrument1.Gradient.Initialize((ValueState) 3, 2);
    this.iscInstrument1.Gradient.Modify(1, -100.0, (ValueState) 1);
    this.iscInstrument1.Gradient.Modify(2, 99.5, (ValueState) 3);
    ((SingleInstrumentBase) this.iscInstrument1).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "DT_Idle_Speed_Balance_Values_Cylinder_1");
    ((Control) this.iscInstrument1).Name = "iscInstrument1";
    ((SingleInstrumentBase) this.iscInstrument1).TitlePosition = (LabelPosition) 0;
    ((SingleInstrumentBase) this.iscInstrument1).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.pirInstrument6, "pirInstrument6");
    this.pirInstrument6.FontGroup = (string) null;
    ((SingleInstrumentBase) this.pirInstrument6).FreezeValue = false;
    this.pirInstrument6.Gradient.Initialize((ValueState) 3, 2);
    this.pirInstrument6.Gradient.Modify(1, 0.478, (ValueState) 1);
    this.pirInstrument6.Gradient.Modify(2, 0.57, (ValueState) 3);
    ((SingleInstrumentBase) this.pirInstrument6).Instrument = new Qualifier((QualifierTypes) 8, "MCM21T", "DT_STO_ACC046_OP_Data_3_e2p_pir_adapt_cyl_6_NOP0");
    ((Control) this.pirInstrument6).Name = "pirInstrument6";
    ((SingleInstrumentBase) this.pirInstrument6).TitlePosition = (LabelPosition) 0;
    ((SingleInstrumentBase) this.pirInstrument6).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.pirInstrument5, "pirInstrument5");
    this.pirInstrument5.FontGroup = (string) null;
    ((SingleInstrumentBase) this.pirInstrument5).FreezeValue = false;
    this.pirInstrument5.Gradient.Initialize((ValueState) 3, 2);
    this.pirInstrument5.Gradient.Modify(1, 0.478, (ValueState) 1);
    this.pirInstrument5.Gradient.Modify(2, 0.57, (ValueState) 3);
    ((SingleInstrumentBase) this.pirInstrument5).Instrument = new Qualifier((QualifierTypes) 8, "MCM21T", "DT_STO_ACC046_OP_Data_3_e2p_pir_adapt_cyl_5_NOP0");
    ((Control) this.pirInstrument5).Name = "pirInstrument5";
    ((SingleInstrumentBase) this.pirInstrument5).TitlePosition = (LabelPosition) 0;
    ((SingleInstrumentBase) this.pirInstrument5).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.pirInstrument4, "pirInstrument4");
    this.pirInstrument4.FontGroup = (string) null;
    ((SingleInstrumentBase) this.pirInstrument4).FreezeValue = false;
    this.pirInstrument4.Gradient.Initialize((ValueState) 3, 2);
    this.pirInstrument4.Gradient.Modify(1, 0.478, (ValueState) 1);
    this.pirInstrument4.Gradient.Modify(2, 0.57, (ValueState) 3);
    ((SingleInstrumentBase) this.pirInstrument4).Instrument = new Qualifier((QualifierTypes) 8, "MCM21T", "DT_STO_ACC046_OP_Data_3_e2p_pir_adapt_cyl_4_NOP0");
    ((Control) this.pirInstrument4).Name = "pirInstrument4";
    ((SingleInstrumentBase) this.pirInstrument4).TitlePosition = (LabelPosition) 0;
    ((SingleInstrumentBase) this.pirInstrument4).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.pirInstrument3, "pirInstrument3");
    this.pirInstrument3.FontGroup = (string) null;
    ((SingleInstrumentBase) this.pirInstrument3).FreezeValue = false;
    this.pirInstrument3.Gradient.Initialize((ValueState) 3, 2);
    this.pirInstrument3.Gradient.Modify(1, 0.478, (ValueState) 1);
    this.pirInstrument3.Gradient.Modify(2, 0.57, (ValueState) 3);
    ((SingleInstrumentBase) this.pirInstrument3).Instrument = new Qualifier((QualifierTypes) 8, "MCM21T", "DT_STO_ACC046_OP_Data_3_e2p_pir_adapt_cyl_3_NOP0");
    ((Control) this.pirInstrument3).Name = "pirInstrument3";
    ((SingleInstrumentBase) this.pirInstrument3).TitlePosition = (LabelPosition) 0;
    ((SingleInstrumentBase) this.pirInstrument3).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.pirInstrument2, "pirInstrument2");
    this.pirInstrument2.FontGroup = (string) null;
    ((SingleInstrumentBase) this.pirInstrument2).FreezeValue = false;
    this.pirInstrument2.Gradient.Initialize((ValueState) 3, 2);
    this.pirInstrument2.Gradient.Modify(1, 0.478, (ValueState) 1);
    this.pirInstrument2.Gradient.Modify(2, 0.57, (ValueState) 3);
    ((SingleInstrumentBase) this.pirInstrument2).Instrument = new Qualifier((QualifierTypes) 8, "MCM21T", "DT_STO_ACC046_OP_Data_3_e2p_pir_adapt_cyl_2_NOP0");
    ((Control) this.pirInstrument2).Name = "pirInstrument2";
    ((SingleInstrumentBase) this.pirInstrument2).TitlePosition = (LabelPosition) 0;
    ((SingleInstrumentBase) this.pirInstrument2).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.pirInstrument1, "pirInstrument1");
    this.pirInstrument1.FontGroup = (string) null;
    ((SingleInstrumentBase) this.pirInstrument1).FreezeValue = false;
    this.pirInstrument1.Gradient.Initialize((ValueState) 3, 2);
    this.pirInstrument1.Gradient.Modify(1, 0.478, (ValueState) 1);
    this.pirInstrument1.Gradient.Modify(2, 0.57, (ValueState) 3);
    ((SingleInstrumentBase) this.pirInstrument1).Instrument = new Qualifier((QualifierTypes) 8, "MCM21T", "DT_STO_ACC046_OP_Data_3_e2p_pir_adapt_cyl_1_NOP0");
    ((Control) this.pirInstrument1).Name = "pirInstrument1";
    ((SingleInstrumentBase) this.pirInstrument1).TitlePosition = (LabelPosition) 0;
    ((SingleInstrumentBase) this.pirInstrument1).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.mainLayoutPanel).SetColumnSpan((Control) this.iscInstrument5, 2);
    componentResourceManager.ApplyResources((object) this.iscInstrument5, "iscInstrument5");
    this.iscInstrument5.FontGroup = (string) null;
    ((SingleInstrumentBase) this.iscInstrument5).FreezeValue = false;
    this.iscInstrument5.Gradient.Initialize((ValueState) 3, 2);
    this.iscInstrument5.Gradient.Modify(1, -100.0, (ValueState) 1);
    this.iscInstrument5.Gradient.Modify(2, 99.5, (ValueState) 3);
    ((SingleInstrumentBase) this.iscInstrument5).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "DT_Idle_Speed_Balance_Values_Cylinder_5");
    ((Control) this.iscInstrument5).Name = "iscInstrument5";
    ((SingleInstrumentBase) this.iscInstrument5).TitlePosition = (LabelPosition) 0;
    ((SingleInstrumentBase) this.iscInstrument5).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.mainLayoutPanel).SetColumnSpan((Control) this.iscInstrument4, 2);
    componentResourceManager.ApplyResources((object) this.iscInstrument4, "iscInstrument4");
    this.iscInstrument4.FontGroup = (string) null;
    ((SingleInstrumentBase) this.iscInstrument4).FreezeValue = false;
    this.iscInstrument4.Gradient.Initialize((ValueState) 3, 2);
    this.iscInstrument4.Gradient.Modify(1, -100.0, (ValueState) 1);
    this.iscInstrument4.Gradient.Modify(2, 99.5, (ValueState) 3);
    ((SingleInstrumentBase) this.iscInstrument4).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "DT_Idle_Speed_Balance_Values_Cylinder_4");
    ((Control) this.iscInstrument4).Name = "iscInstrument4";
    ((SingleInstrumentBase) this.iscInstrument4).TitlePosition = (LabelPosition) 0;
    ((SingleInstrumentBase) this.iscInstrument4).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.lblCylinderPosition, "lblCylinderPosition");
    ((TableLayoutPanel) this.mainLayoutPanel).SetColumnSpan((Control) this.lblCylinderPosition, 2);
    this.lblCylinderPosition.Name = "lblCylinderPosition";
    this.lblCylinderPosition.UseCompatibleTextRendering = true;
    ((TableLayoutPanel) this.mainLayoutPanel).SetColumnSpan((Control) this.iscInstrument3, 2);
    componentResourceManager.ApplyResources((object) this.iscInstrument3, "iscInstrument3");
    this.iscInstrument3.FontGroup = (string) null;
    ((SingleInstrumentBase) this.iscInstrument3).FreezeValue = false;
    this.iscInstrument3.Gradient.Initialize((ValueState) 3, 2);
    this.iscInstrument3.Gradient.Modify(1, -100.0, (ValueState) 1);
    this.iscInstrument3.Gradient.Modify(2, 99.5, (ValueState) 3);
    ((SingleInstrumentBase) this.iscInstrument3).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "DT_Idle_Speed_Balance_Values_Cylinder_3");
    ((Control) this.iscInstrument3).Name = "iscInstrument3";
    ((SingleInstrumentBase) this.iscInstrument3).TitlePosition = (LabelPosition) 0;
    ((SingleInstrumentBase) this.iscInstrument3).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.mainLayoutPanel).SetColumnSpan((Control) this.iscInstrument2, 2);
    componentResourceManager.ApplyResources((object) this.iscInstrument2, "iscInstrument2");
    this.iscInstrument2.FontGroup = (string) null;
    ((SingleInstrumentBase) this.iscInstrument2).FreezeValue = false;
    this.iscInstrument2.Gradient.Initialize((ValueState) 3, 2);
    this.iscInstrument2.Gradient.Modify(1, -100.0, (ValueState) 1);
    this.iscInstrument2.Gradient.Modify(2, 99.5, (ValueState) 3);
    ((SingleInstrumentBase) this.iscInstrument2).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "DT_Idle_Speed_Balance_Values_Cylinder_2");
    ((Control) this.iscInstrument2).Name = "iscInstrument2";
    ((SingleInstrumentBase) this.iscInstrument2).TitlePosition = (LabelPosition) 0;
    ((SingleInstrumentBase) this.iscInstrument2).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.minLabel, "minLabel");
    this.minLabel.Name = "minLabel";
    this.minLabel.UseCompatibleTextRendering = true;
    ((TableLayoutPanel) this.mainLayoutPanel).SetColumnSpan((Control) this.textboxResults, 4);
    componentResourceManager.ApplyResources((object) this.textboxResults, "textboxResults");
    this.textboxResults.Name = "textboxResults";
    this.textboxResults.ReadOnly = true;
    ((TableLayoutPanel) this.mainLayoutPanel).SetRowSpan((Control) this.textboxResults, 4);
    componentResourceManager.ApplyResources((object) this.label1, "label1");
    this.label1.Name = "label1";
    this.label1.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.label6, "label6");
    this.label6.Name = "label6";
    this.label6.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.label5, "label5");
    this.label5.Name = "label5";
    this.label5.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.label4, "label4");
    this.label4.Name = "label4";
    this.label4.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.label3, "label3");
    this.label3.Name = "label3";
    this.label3.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.label2, "label2");
    this.label2.Name = "label2";
    this.label2.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.label20, "label20");
    this.label20.Name = "label20";
    this.label20.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.iscLabel, "iscLabel");
    ((TableLayoutPanel) this.mainLayoutPanel).SetColumnSpan((Control) this.iscLabel, 2);
    this.iscLabel.Name = "iscLabel";
    this.iscLabel.UseCompatibleTextRendering = true;
    this.buttonClose.DialogResult = DialogResult.OK;
    componentResourceManager.ApplyResources((object) this.buttonClose, "buttonClose");
    this.buttonClose.Name = "buttonClose";
    this.buttonClose.UseCompatibleTextRendering = true;
    this.buttonClose.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.flowLayoutPanel1, "flowLayoutPanel1");
    ((TableLayoutPanel) this.mainLayoutPanel).SetColumnSpan((Control) this.flowLayoutPanel1, 6);
    this.flowLayoutPanel1.Controls.Add((Control) this.buttonRead);
    this.flowLayoutPanel1.Controls.Add((Control) this.buttonWrite);
    this.flowLayoutPanel1.Controls.Add((Control) this.buttonReset);
    this.flowLayoutPanel1.Controls.Add((Control) this.buttonPrint);
    this.flowLayoutPanel1.Name = "flowLayoutPanel1";
    componentResourceManager.ApplyResources((object) this.buttonRead, "buttonRead");
    this.buttonRead.Name = "buttonRead";
    this.buttonRead.UseCompatibleTextRendering = true;
    this.buttonRead.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.buttonWrite, "buttonWrite");
    this.buttonWrite.Name = "buttonWrite";
    this.buttonWrite.UseCompatibleTextRendering = true;
    this.buttonWrite.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.buttonReset, "buttonReset");
    this.buttonReset.Name = "buttonReset";
    this.buttonReset.UseCompatibleTextRendering = true;
    this.buttonReset.UseVisualStyleBackColor = true;
    componentResourceManager.ApplyResources((object) this.buttonPrint, "buttonPrint");
    this.buttonPrint.Name = "buttonPrint";
    this.buttonPrint.UseCompatibleTextRendering = true;
    this.buttonPrint.UseVisualStyleBackColor = true;
    ((TableLayoutPanel) this.mainLayoutPanel).SetColumnSpan((Control) this.digitalReadoutInstrumentEngineState, 3);
    this.digitalReadoutInstrumentEngineState.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEngineState).FreezeValue = false;
    this.digitalReadoutInstrumentEngineState.Gradient.Initialize((ValueState) 3, 8);
    this.digitalReadoutInstrumentEngineState.Gradient.Modify(1, -1.0, (ValueState) 3);
    this.digitalReadoutInstrumentEngineState.Gradient.Modify(2, 0.0, (ValueState) 1);
    this.digitalReadoutInstrumentEngineState.Gradient.Modify(3, 1.0, (ValueState) 3);
    this.digitalReadoutInstrumentEngineState.Gradient.Modify(4, 2.0, (ValueState) 3);
    this.digitalReadoutInstrumentEngineState.Gradient.Modify(5, 3.0, (ValueState) 3);
    this.digitalReadoutInstrumentEngineState.Gradient.Modify(6, 4.0, (ValueState) 3);
    this.digitalReadoutInstrumentEngineState.Gradient.Modify(7, 5.0, (ValueState) 3);
    this.digitalReadoutInstrumentEngineState.Gradient.Modify(8, 6.0, (ValueState) 3);
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentEngineState, "digitalReadoutInstrumentEngineState");
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEngineState).Instrument = new Qualifier((QualifierTypes) 1, "MCM21T", "DT_AS023_Engine_State");
    ((Control) this.digitalReadoutInstrumentEngineState).Name = "digitalReadoutInstrumentEngineState";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentEngineState).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.mainLayoutPanel).SetColumnSpan((Control) this.digitalReadoutLeakLearnedValue, 3);
    this.digitalReadoutLeakLearnedValue.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutLeakLearnedValue).FreezeValue = false;
    componentResourceManager.ApplyResources((object) this.digitalReadoutLeakLearnedValue, "digitalReadoutLeakLearnedValue");
    ((SingleInstrumentBase) this.digitalReadoutLeakLearnedValue).Instrument = new Qualifier((QualifierTypes) 8, "MCM21T", "DT_STO_ACC047_OP_Data_4_HP_Leak_Learned_Value");
    ((Control) this.digitalReadoutLeakLearnedValue).Name = "digitalReadoutLeakLearnedValue";
    ((SingleInstrumentBase) this.digitalReadoutLeakLearnedValue).UnitAlignment = StringAlignment.Near;
    ((TableLayoutPanel) this.mainLayoutPanel).SetColumnSpan((Control) this.labelPartNumberEntryTips, 3);
    componentResourceManager.ApplyResources((object) this.labelPartNumberEntryTips, "labelPartNumberEntryTips");
    this.labelPartNumberEntryTips.Name = "labelPartNumberEntryTips";
    ((TableLayoutPanel) this.mainLayoutPanel).SetRowSpan((Control) this.labelPartNumberEntryTips, 2);
    this.labelPartNumberEntryTips.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this.panelPictures, "panelPictures");
    this.panelPictures.Controls.Add((Control) this.pictureBoxHDEP);
    this.panelPictures.Name = "panelPictures";
    ((TableLayoutPanel) this.mainLayoutPanel).SetRowSpan((Control) this.panelPictures, 3);
    componentResourceManager.ApplyResources((object) this.pictureBoxHDEP, "pictureBoxHDEP");
    this.pictureBoxHDEP.Name = "pictureBoxHDEP";
    this.pictureBoxHDEP.TabStop = false;
    ((TableLayoutPanel) this.mainLayoutPanel).SetColumnSpan((Control) this.labelDD15Warning, 6);
    componentResourceManager.ApplyResources((object) this.labelDD15Warning, "labelDD15Warning");
    this.labelDD15Warning.ForeColor = Color.Red;
    this.labelDD15Warning.Name = "labelDD15Warning";
    this.labelDD15Warning.UseCompatibleTextRendering = true;
    componentResourceManager.ApplyResources((object) this, "$this");
    this.ContextLink = new Link("Panel_InjectorCodes");
    ((Control) this).Controls.Add((Control) this.mainLayoutPanel);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.mainLayoutPanel).ResumeLayout(false);
    ((Control) this.mainLayoutPanel).PerformLayout();
    ((ISupportInitialize) this.pictureBoxExamplePartNumber).EndInit();
    this.flowLayoutPanel1.ResumeLayout(false);
    this.panelPictures.ResumeLayout(false);
    ((ISupportInitialize) this.pictureBoxHDEP).EndInit();
    ((Control) this).ResumeLayout(false);
  }

  private enum State
  {
    None,
    Read,
    ReadTrimCodes,
    ReadPirValues,
    ReadRgpLeakValues,
    ReadComplete,
    Write,
    WriteTrimCodes,
    ResetPirValuesAfterWrite,
    ReadPirValuesAfterWrite,
    WriteComplete,
    ResetAll,
    ResetAllAfterWrite,
    ResetAllRun,
    ResetRpgLeakValues,
    SaveResetParameterValues,
    ReadRgpLeakValuesAfterReset,
    ResetAllComplete,
  }

  [Flags]
  private enum Features
  {
    None = 0,
    ChecksumAtStart = 1,
    ChecksumAtEnd = 2,
    Isc = 4,
    Pir = 8,
    RequirePartNumbers = 16, // 0x00000010
  }

  private class SetupInformation
  {
    public readonly string EquipmentType;
    public readonly int CodeLengthWithoutCRC;
    public readonly PictureBox Picture;
    public readonly int CylinderCount;
    private readonly UserPanel.Features features;
    private static readonly Dictionary<char, char> CommonSubstitutions = new Dictionary<char, char>();

    public int CodeLengthWithCRC
    {
      get => this.RequiresChecksum ? this.CodeLengthWithoutCRC + 1 : this.CodeLengthWithoutCRC;
    }

    public bool RequiresChecksum
    {
      get
      {
        return this.SupportsFeature(UserPanel.Features.ChecksumAtEnd) || this.SupportsFeature(UserPanel.Features.ChecksumAtStart);
      }
    }

    public bool SupportsFeature(UserPanel.Features feature)
    {
      return (this.features & feature) != UserPanel.Features.None;
    }

    public SetupInformation(
      string targetEquipmentName,
      int codeLengthWithoutCRC,
      UserPanel.Features features,
      PictureBox picture,
      int cylinderCount)
    {
      if ((this.features & UserPanel.Features.ChecksumAtStart) != UserPanel.Features.None && (this.features & UserPanel.Features.ChecksumAtEnd) != UserPanel.Features.None)
        throw new ArgumentException();
      this.EquipmentType = targetEquipmentName;
      this.CodeLengthWithoutCRC = codeLengthWithoutCRC;
      this.features = features;
      this.Picture = picture;
      this.CylinderCount = cylinderCount;
    }

    public bool ValidTrimCode(string trimCode)
    {
      bool flag = false;
      if (!string.IsNullOrEmpty(trimCode) && trimCode.Length == this.CodeLengthWithCRC && !UserPanel.SetupInformation.ETrimCRC.InvalidCharacters.IsMatch(trimCode))
        flag = !this.SupportsFeature(UserPanel.Features.ChecksumAtStart) ? !this.SupportsFeature(UserPanel.Features.ChecksumAtEnd) || (int) trimCode[trimCode.Length - 1] == (int) UserPanel.SetupInformation.ETrimCRC.CalculateCRC(this.StripCrc(trimCode)) : (int) trimCode[0] == (int) UserPanel.SetupInformation.ETrimCRC.CalculateCRC(this.StripCrc(trimCode));
      return flag;
    }

    public string StripCrc(string trimCodeWithCrc)
    {
      if (trimCodeWithCrc.Length != this.CodeLengthWithCRC)
        throw new ArgumentException();
      if (this.SupportsFeature(UserPanel.Features.ChecksumAtStart))
        return trimCodeWithCrc.Substring(1);
      return this.SupportsFeature(UserPanel.Features.ChecksumAtEnd) ? trimCodeWithCrc.Substring(0, this.CodeLengthWithoutCRC) : trimCodeWithCrc;
    }

    public string AddCrc(string trimCode)
    {
      if (trimCode.Length != this.CodeLengthWithoutCRC)
        return new string('?', this.CodeLengthWithCRC);
      return !UserPanel.SetupInformation.ETrimCRC.InvalidCharacters.IsMatch(trimCode) ? this.AddCrcCharacter(trimCode, UserPanel.SetupInformation.ETrimCRC.CalculateCRC(trimCode)) : this.AddCrcCharacter(trimCode, '?');
    }

    private string AddCrcCharacter(string trimCodeWithoutCrc, char checksum)
    {
      if (trimCodeWithoutCrc.Length != this.CodeLengthWithoutCRC)
        throw new ArgumentException();
      if (this.SupportsFeature(UserPanel.Features.ChecksumAtStart))
        return checksum.ToString() + trimCodeWithoutCrc;
      return this.SupportsFeature(UserPanel.Features.ChecksumAtEnd) ? trimCodeWithoutCrc + (object) checksum : trimCodeWithoutCrc;
    }

    static SetupInformation()
    {
      UserPanel.SetupInformation.CommonSubstitutions.Add('0', 'O');
      UserPanel.SetupInformation.CommonSubstitutions.Add('1', 'I');
      UserPanel.SetupInformation.CommonSubstitutions.Add('8', 'B');
    }

    public bool ValidateAndCorrect(ref char trimCharacter)
    {
      return !UserPanel.SetupInformation.ETrimCRC.InvalidCharacters.IsMatch(trimCharacter.ToString().ToUpperInvariant()) || UserPanel.SetupInformation.CommonSubstitutions.TryGetValue(trimCharacter, out trimCharacter);
    }

    private static class ETrimCRC
    {
      private const byte Polynomial = 43;
      private const string CharacterRange = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
      private static byte[] lookupTable = new byte[256 /*0x0100*/];
      public static readonly Regex InvalidCharacters = new Regex($"[^{"ABCDEFGHIJKLMNOPQRSTUVWXYZ234567"}]");
      private static int backShift;
      private static byte adjustedPolynomial;

      static ETrimCRC() => UserPanel.SetupInformation.ETrimCRC.InitialiseLookupTable((byte) 43);

      private static void InitialiseLookupTable(byte polynomial)
      {
        UserPanel.SetupInformation.ETrimCRC.adjustedPolynomial = polynomial;
        UserPanel.SetupInformation.ETrimCRC.backShift = 1;
        while (((int) UserPanel.SetupInformation.ETrimCRC.adjustedPolynomial & 128 /*0x80*/) == 0)
        {
          UserPanel.SetupInformation.ETrimCRC.adjustedPolynomial <<= 1;
          ++UserPanel.SetupInformation.ETrimCRC.backShift;
        }
        for (int index1 = 0; index1 < 256 /*0x0100*/; ++index1)
        {
          byte num = (byte) index1;
          for (int index2 = 0; index2 < 8; ++index2)
          {
            if (((int) num & 128 /*0x80*/) == 128 /*0x80*/)
              num ^= UserPanel.SetupInformation.ETrimCRC.adjustedPolynomial;
            num = (byte) ((int) num << 1 & (int) byte.MaxValue);
          }
          UserPanel.SetupInformation.ETrimCRC.lookupTable[index1] = num;
        }
      }

      public static char CalculateCRC(string trimCode)
      {
        byte[] numArray = !UserPanel.SetupInformation.ETrimCRC.InvalidCharacters.IsMatch(trimCode) ? new ASCIIEncoding().GetBytes(trimCode) : throw new ArgumentException($"Trim code can only contain characters \"{"ABCDEFGHIJKLMNOPQRSTUVWXYZ234567"}\".", nameof (trimCode));
        byte num1 = 0;
        for (int index1 = 0; index1 < trimCode.Length; ++index1)
        {
          byte index2 = (byte) ((uint) num1 ^ (uint) numArray[index1] & (uint) byte.MaxValue);
          num1 = UserPanel.SetupInformation.ETrimCRC.lookupTable[(int) index2];
        }
        byte num2 = (byte) ((uint) num1 >> UserPanel.SetupInformation.ETrimCRC.backShift);
        return (int) num2 < "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567".Length ? "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567"[(int) num2 & (int) byte.MaxValue] : throw new InvalidOperationException("Calculated CRC falls outside of expected range.");
      }
    }
  }

  private class SetupInformationCollection : KeyedCollection<string, UserPanel.SetupInformation>
  {
    public SetupInformationCollection(UserPanel.SetupInformation[] setups)
    {
      foreach (UserPanel.SetupInformation setup in setups)
        this.Add(setup);
    }

    protected override string GetKeyForItem(UserPanel.SetupInformation item) => item.EquipmentType;
  }
}
