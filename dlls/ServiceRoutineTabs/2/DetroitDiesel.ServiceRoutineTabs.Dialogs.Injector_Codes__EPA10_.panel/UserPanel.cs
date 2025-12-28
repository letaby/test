using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DetroitDiesel.Collections;
using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Interfaces;
using DetroitDiesel.Windows.Forms;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Injector_Codes__EPA10_.panel;

public class UserPanel : CustomPanel, IProvideHtml
{
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
		ResetAllComplete
	}

	[Flags]
	private enum Features
	{
		None = 0,
		ChecksumAtStart = 1,
		ChecksumAtEnd = 2,
		Isc = 4,
		Pir = 8,
		RequirePartNumbers = 0x10
	}

	private class SetupInformation
	{
		private static class ETrimCRC
		{
			private const byte Polynomial = 43;

			private const string CharacterRange = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

			private static byte[] lookupTable;

			public static readonly Regex InvalidCharacters;

			private static int backShift;

			private static byte adjustedPolynomial;

			static ETrimCRC()
			{
				lookupTable = new byte[256];
				InvalidCharacters = new Regex(string.Format("[^{0}]", "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567"));
				InitialiseLookupTable(43);
			}

			private static void InitialiseLookupTable(byte polynomial)
			{
				adjustedPolynomial = polynomial;
				backShift = 1;
				while ((adjustedPolynomial & 0x80) == 0)
				{
					adjustedPolynomial <<= 1;
					backShift++;
				}
				for (int i = 0; i < 256; i++)
				{
					byte b = (byte)i;
					for (int j = 0; j < 8; j++)
					{
						if ((b & 0x80) == 128)
						{
							b ^= adjustedPolynomial;
						}
						b = (byte)((b << 1) & 0xFF);
					}
					lookupTable[i] = b;
				}
			}

			public static char CalculateCRC(string trimCode)
			{
				if (InvalidCharacters.IsMatch(trimCode))
				{
					throw new ArgumentException(string.Format("Trim code can only contain characters \"{0}\".", "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567"), "trimCode");
				}
				char c = '?';
				ASCIIEncoding aSCIIEncoding = new ASCIIEncoding();
				byte[] bytes = aSCIIEncoding.GetBytes(trimCode);
				byte b = 0;
				for (int i = 0; i < trimCode.Length; i++)
				{
					b = (byte)(b ^ (bytes[i] & 0xFF));
					b = lookupTable[b];
				}
				b = (byte)(b >> backShift);
				if (b < "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567".Length)
				{
					return "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567"[b & 0xFF];
				}
				throw new InvalidOperationException("Calculated CRC falls outside of expected range.");
			}
		}

		public readonly string EquipmentType;

		public readonly int CodeLengthWithoutCRC;

		public readonly PictureBox Picture;

		private readonly Features features;

		private static readonly Dictionary<char, char> CommonSubstitutions;

		public int CodeLengthWithCRC => RequiresChecksum ? (CodeLengthWithoutCRC + 1) : CodeLengthWithoutCRC;

		public bool RequiresChecksum => SupportsFeature(Features.ChecksumAtEnd) || SupportsFeature(Features.ChecksumAtStart);

		public bool SupportsFeature(Features feature)
		{
			return (features & feature) != 0;
		}

		public SetupInformation(string targetEquipmentName, int codeLengthWithoutCRC, Features features, PictureBox picture)
		{
			if ((this.features & Features.ChecksumAtStart) != Features.None && (this.features & Features.ChecksumAtEnd) != Features.None)
			{
				throw new ArgumentException();
			}
			EquipmentType = targetEquipmentName;
			CodeLengthWithoutCRC = codeLengthWithoutCRC;
			this.features = features;
			Picture = picture;
		}

		public bool ValidTrimCode(string trimCode)
		{
			bool result = false;
			if (!string.IsNullOrEmpty(trimCode) && trimCode.Length == CodeLengthWithCRC && !ETrimCRC.InvalidCharacters.IsMatch(trimCode))
			{
				result = (SupportsFeature(Features.ChecksumAtStart) ? (trimCode[0] == ETrimCRC.CalculateCRC(StripCrc(trimCode))) : (!SupportsFeature(Features.ChecksumAtEnd) || trimCode[trimCode.Length - 1] == ETrimCRC.CalculateCRC(StripCrc(trimCode))));
			}
			return result;
		}

		public string StripCrc(string trimCodeWithCrc)
		{
			if (trimCodeWithCrc.Length != CodeLengthWithCRC)
			{
				throw new ArgumentException();
			}
			if (SupportsFeature(Features.ChecksumAtStart))
			{
				return trimCodeWithCrc.Substring(1);
			}
			if (SupportsFeature(Features.ChecksumAtEnd))
			{
				return trimCodeWithCrc.Substring(0, CodeLengthWithoutCRC);
			}
			return trimCodeWithCrc;
		}

		public string AddCrc(string trimCode)
		{
			if (trimCode.Length == CodeLengthWithoutCRC)
			{
				if (!ETrimCRC.InvalidCharacters.IsMatch(trimCode))
				{
					return AddCrcCharacter(trimCode, ETrimCRC.CalculateCRC(trimCode));
				}
				return AddCrcCharacter(trimCode, '?');
			}
			return new string('?', CodeLengthWithCRC);
		}

		private string AddCrcCharacter(string trimCodeWithoutCrc, char checksum)
		{
			if (trimCodeWithoutCrc.Length != CodeLengthWithoutCRC)
			{
				throw new ArgumentException();
			}
			if (SupportsFeature(Features.ChecksumAtStart))
			{
				return checksum + trimCodeWithoutCrc;
			}
			if (SupportsFeature(Features.ChecksumAtEnd))
			{
				return trimCodeWithoutCrc + checksum;
			}
			return trimCodeWithoutCrc;
		}

		static SetupInformation()
		{
			CommonSubstitutions = new Dictionary<char, char>();
			CommonSubstitutions.Add('0', 'O');
			CommonSubstitutions.Add('1', 'I');
			CommonSubstitutions.Add('8', 'B');
		}

		public bool ValidateAndCorrect(ref char trimCharacter)
		{
			if (ETrimCRC.InvalidCharacters.IsMatch(trimCharacter.ToString().ToUpperInvariant()))
			{
				if (CommonSubstitutions.TryGetValue(trimCharacter, out trimCharacter))
				{
					return true;
				}
				return false;
			}
			return true;
		}
	}

	private class SetupInformationCollection : KeyedCollection<string, SetupInformation>
	{
		public SetupInformationCollection(SetupInformation[] setups)
		{
			foreach (SetupInformation item in setups)
			{
				Add(item);
			}
		}

		protected override string GetKeyForItem(SetupInformation item)
		{
			return item.EquipmentType;
		}
	}

	private const int CylinderCount = 6;

	private const int ResetRpgLeakValuesRawValue = 24;

	private const string ParameterLeakLearnedValue = "HP_Leak_Learned_Value";

	private const string McmChannelName = "MCM02T";

	private const string ReadService = "DJ_Read_E_Trim";

	private const string WriteService = "DJ_Write_E_Trim";

	private const string ResetService = "RT_SR070_Injector_Change_Start";

	private const string ResetAllService = "RT_SR074_EcuInitPIR_Start";

	private const string ResetLeakValuesService = "RT_SR014_SET_EOL_Default_Values_Start";

	private static string[] pirQualifiers;

	private SetupInformation currentSetup = null;

	private string[] oldTrimCodes = new string[6];

	private bool[] needReset = new bool[6];

	private TextBox[] trimCodes;

	private TextBox[] partNumbers;

	private Checkmark[] checkMarks;

	private Checkmark[] partNumberCheckMarks;

	private System.Windows.Forms.Label[] cylinderLabels;

	private Channel mcm;

	private State nextState = State.None;

	private int currentCylinder = -1;

	private Service currentService;

	private readonly SetupInformationCollection Setups;

	private bool readOperationSuccessful = true;

	private bool writeOperationSuccessful = true;

	private readonly Regex dd13InjectorPartValidator = new Regex("^[rR]?[aA]471\\d{7}$", RegexOptions.CultureInvariant);

	private readonly Regex dd15Or16InjectorPartValidator = new Regex("^[rR]?[aA]472\\d{7}$", RegexOptions.CultureInvariant);

	private TextBox textTrimCode1;

	private TextBox textTrimCode2;

	private TextBox textTrimCode3;

	private TextBox textTrimCode4;

	private TextBox textTrimCode5;

	private TextBox textTrimCode6;

	private Panel panelPictures;

	private Button buttonWrite;

	private Button buttonRead;

	private System.Windows.Forms.Label label1;

	private System.Windows.Forms.Label label2;

	private System.Windows.Forms.Label label3;

	private System.Windows.Forms.Label label4;

	private System.Windows.Forms.Label label5;

	private System.Windows.Forms.Label label6;

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

	private Button buttonReset;

	private DigitalReadoutInstrument pirInstrument1;

	private DigitalReadoutInstrument pirInstrument2;

	private DigitalReadoutInstrument pirInstrument3;

	private DigitalReadoutInstrument pirInstrument4;

	private DigitalReadoutInstrument pirInstrument5;

	private DigitalReadoutInstrument pirInstrument6;

	private PictureBox pictureBoxHDEP;

	private System.Windows.Forms.Label labelEntryTips;

	private TableLayoutPanel mainLayoutPanel;

	private Checkmark checkMark1;

	private Checkmark checkMark2;

	private Checkmark checkMark3;

	private Checkmark checkMark4;

	private Checkmark checkMark5;

	private Checkmark checkMark6;

	private FlowLayoutPanel flowLayoutPanel1;

	private Button buttonPrint;

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

	private DigitalReadoutInstrument digitalReadoutLeakLearnedValue;

	private TextBox textboxResults;

	private bool CanClose => !Working;

	private bool Working => nextState != State.None;

	private bool Online => currentSetup != null && mcm.CommunicationsState == CommunicationsState.Online;

	private bool EngineOK => (int)digitalReadoutInstrumentEngineState.RepresentedState == 1;

	private bool CanReadTrimCodes => !Working && Online;

	private bool CanWriteTrimCodes
	{
		get
		{
			bool result = false;
			if (!Working && Online && EngineOK)
			{
				bool flag = true;
				bool flag2 = true;
				for (int i = 0; i < trimCodes.Count(); i++)
				{
					if (!string.IsNullOrEmpty(trimCodes[i].Text))
					{
						flag2 = false;
						if (!ValidTrimCode(trimCodes[i].Text))
						{
							flag = false;
							break;
						}
					}
					if (!string.IsNullOrEmpty(partNumbers[i].Text) && !ValidatePartNumber(i))
					{
						flag = false;
						break;
					}
				}
				result = flag && !flag2;
			}
			return result;
		}
	}

	private bool CanResetPirValues => !Working && Online && ((CustomPanel)this).GetService("MCM02T", "RT_SR074_EcuInitPIR_Start") != null;

	private bool CanPrintCodes => ((CustomPanel)this).CanProvideHtml;

	private int MaxTextLength
	{
		get
		{
			if (currentSetup != null)
			{
				return currentSetup.CodeLengthWithCRC;
			}
			return 0;
		}
	}

	public override bool CanProvideHtml => !Working && textboxResults.TextLength > 0 && (Online || currentSetup == null);

	static UserPanel()
	{
		pirQualifiers = new string[6];
		pirQualifiers[0] = "e2p_pir_adapt_cyl_1_NOP0";
		pirQualifiers[1] = "e2p_pir_adapt_cyl_2_NOP0";
		pirQualifiers[2] = "e2p_pir_adapt_cyl_3_NOP0";
		pirQualifiers[3] = "e2p_pir_adapt_cyl_4_NOP0";
		pirQualifiers[4] = "e2p_pir_adapt_cyl_5_NOP0";
		pirQualifiers[5] = "e2p_pir_adapt_cyl_6_NOP0";
	}

	public UserPanel()
	{
		InitializeComponent();
		Setups = new SetupInformationCollection(new SetupInformation[3]
		{
			new SetupInformation("DD15", 5, Features.ChecksumAtEnd | Features.Isc | Features.RequirePartNumbers, pictureBoxHDEP),
			new SetupInformation("DD13", 5, Features.ChecksumAtEnd | Features.Isc | Features.RequirePartNumbers, pictureBoxHDEP),
			new SetupInformation("DD16", 5, Features.ChecksumAtEnd | Features.Isc | Features.RequirePartNumbers, pictureBoxHDEP)
		});
		buttonRead.Click += OnReadClick;
		buttonWrite.Click += OnWriteClick;
		buttonReset.Click += OnResetClick;
		buttonPrint.Click += OnPrintClick;
		digitalReadoutInstrumentEngineState.RepresentedStateChanged += OnEngineStateChanged;
		trimCodes = new TextBox[6] { textTrimCode1, textTrimCode2, textTrimCode3, textTrimCode4, textTrimCode5, textTrimCode6 };
		checkMarks = (Checkmark[])(object)new Checkmark[6] { checkMark1, checkMark2, checkMark3, checkMark4, checkMark5, checkMark6 };
		partNumbers = new TextBox[6] { textBoxPartNumber1, textBoxPartNumber2, textBoxPartNumber3, textBoxPartNumber4, textBoxPartNumber5, textBoxPartNumber6 };
		partNumberCheckMarks = (Checkmark[])(object)new Checkmark[6] { checkmarkPartNumber1, checkmarkPartNumber2, checkmarkPartNumber3, checkmarkPartNumber4, checkmarkPartNumber5, checkmarkPartNumber6 };
		cylinderLabels = new System.Windows.Forms.Label[7] { lblCylinderPosition, label1, label2, label3, label4, label5, label6 };
		for (int i = 0; i < 6; i++)
		{
			trimCodes[i].Tag = i;
			trimCodes[i].TextChanged += OnTextChanged;
			trimCodes[i].KeyPress += OnKeyPress;
			partNumbers[i].TextChanged += OnPartNumberChanged;
		}
		UpdateControlVisibility();
	}

	protected override void OnLoad(EventArgs e)
	{
		((ContainerControl)this).ParentForm.FormClosing += OnFormClosing;
		((UserControl)this).OnLoad(e);
		if (((Control)this).Tag != null)
		{
			ReadTrimCodes();
		}
	}

	private void OnFormClosing(object sender, FormClosingEventArgs e)
	{
		((Control)this).Tag = new object[2]
		{
			readOperationSuccessful && writeOperationSuccessful,
			textboxResults.Text
		};
		if (e.CloseReason == CloseReason.UserClosing && !CanClose)
		{
			e.Cancel = true;
		}
		if (!e.Cancel)
		{
			((ContainerControl)this).ParentForm.FormClosing -= OnFormClosing;
			SetMCM(null);
		}
	}

	public override void OnChannelsChanged()
	{
		SetMCM(((CustomPanel)this).GetChannel("MCM02T"));
		UpdateUserInterface();
	}

	private void SetMCM(Channel mcm)
	{
		if (this.mcm != mcm)
		{
			ResetData();
			if (this.mcm != null)
			{
				this.mcm.CommunicationsStateUpdateEvent -= OnChannelStateUpdate;
			}
			this.mcm = mcm;
			if (this.mcm != null)
			{
				this.mcm.CommunicationsStateUpdateEvent += OnChannelStateUpdate;
			}
		}
		UpdateCurrentSetup();
	}

	private void AdvanceState()
	{
		switch (nextState)
		{
		case State.Read:
			ClearResults();
			oldTrimCodes = new string[6];
			ReportResult(Resources.Message_ReadingInjectorCodes);
			currentCylinder = 0;
			nextState = State.ReadTrimCodes;
			AdvanceState();
			break;
		case State.ReadTrimCodes:
			NextCylinder();
			if (currentCylinder < 0)
			{
				ReportResult(Resources.Message_ReadingPIRValues);
				currentCylinder = 0;
				nextState = State.ReadPirValues;
				AdvanceState();
			}
			else
			{
				ReadCurrentTrimCode();
			}
			break;
		case State.ReadPirValues:
			NextCylinder();
			if (currentCylinder < 0)
			{
				ReportResult(Resources.Message_PIRParametersRead);
				nextState = State.ReadRgpLeakValues;
				AdvanceState();
			}
			else
			{
				ReadCurrentPirValue();
			}
			break;
		case State.ReadRgpLeakValues:
			nextState = State.ReadComplete;
			ReadRgpLeakValues();
			AdvanceState();
			break;
		case State.ReadComplete:
			ReportResult(Resources.Message_FinishedReadingInjectorCodes);
			ClearPartNumbers();
			nextState = State.None;
			break;
		case State.Write:
			ClearResults();
			needReset = new bool[6];
			ReportResult(Resources.Message_WritingInjectorCodes);
			nextState = State.WriteTrimCodes;
			currentCylinder = 0;
			AdvanceState();
			break;
		case State.WriteTrimCodes:
			NextCylinder();
			if (currentCylinder < 0)
			{
				currentCylinder = 0;
				nextState = State.ResetPirValuesAfterWrite;
				AdvanceState();
			}
			else
			{
				WriteCurrentTrimCode();
			}
			break;
		case State.ResetPirValuesAfterWrite:
			NextCylinder();
			if (currentCylinder < 0)
			{
				ReportResult(Resources.Message_ReadingPIRValues1);
				currentCylinder = 0;
				nextState = State.ReadPirValuesAfterWrite;
				AdvanceState();
			}
			else
			{
				ResetCurrentPir();
			}
			break;
		case State.ReadPirValuesAfterWrite:
			NextCylinder();
			if (currentCylinder < 0)
			{
				ReportResult(Resources.Message_PIRParametersRead1);
				nextState = State.WriteComplete;
				AdvanceState();
			}
			else
			{
				ReadCurrentPirValue();
			}
			break;
		case State.WriteComplete:
			ReportResult(Resources.Message_FinishedSendingInjectorCodes);
			((CustomPanel)this).CommitToPermanentMemory("MCM02T");
			nextState = State.ResetAllAfterWrite;
			AdvanceState();
			break;
		case State.ResetAllAfterWrite:
			ReportResult(Resources.Message_ResetAllPIRData);
			nextState = State.ResetAllRun;
			AdvanceState();
			break;
		case State.ResetAll:
			ClearResults();
			ReportResult(Resources.Message_ResetAllPIRData);
			nextState = State.ResetAllRun;
			AdvanceState();
			break;
		case State.ResetAllRun:
			nextState = State.ResetRpgLeakValues;
			ResetAllPir();
			break;
		case State.ResetRpgLeakValues:
			if (writeOperationSuccessful)
			{
				nextState = State.SaveResetParameterValues;
				ResetRpgLeakValues();
			}
			else
			{
				nextState = State.None;
				AdvanceState();
			}
			break;
		case State.SaveResetParameterValues:
			if (writeOperationSuccessful)
			{
				nextState = State.ResetAllComplete;
				((CustomPanel)this).CommitToPermanentMemory("MCM02T");
				ReadRgpLeakValues();
			}
			else
			{
				nextState = State.None;
			}
			AdvanceState();
			break;
		case State.ResetAllComplete:
			ReportResult(Resources.Message_FinishedResettingPIRData);
			nextState = State.None;
			break;
		}
		UpdateUserInterface();
	}

	private void NextCylinder()
	{
		if (currentCylinder >= 0 && currentCylinder < 6)
		{
			currentCylinder++;
		}
		else
		{
			currentCylinder = -1;
		}
	}

	private string GetEngineTypeName()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		IEnumerable<EquipmentType> enumerable = EquipmentType.ConnectedEquipmentTypes("Engine");
		if (CollectionExtensions.Exactly<EquipmentType>(enumerable, 1))
		{
			EquipmentType val = enumerable.First();
			return ((EquipmentType)(ref val)).Name;
		}
		return null;
	}

	private void UpdateCurrentSetup()
	{
		SetupInformation setupInformation = null;
		if (mcm != null)
		{
			string engineTypeName = GetEngineTypeName();
			if (!string.IsNullOrEmpty(engineTypeName) && Setups.Contains(engineTypeName))
			{
				setupInformation = Setups[engineTypeName];
			}
		}
		if (setupInformation != currentSetup)
		{
			currentSetup = setupInformation;
			UpdateControlVisibility();
		}
	}

	private void UpdateControlVisibility()
	{
		foreach (SetupInformation setup in Setups)
		{
			if (currentSetup == null || setup.Picture != currentSetup.Picture)
			{
				SetControlVisibility(setup.Picture, visible: false);
			}
			else
			{
				SetControlVisibility(setup.Picture, visible: true);
			}
		}
		SetIscControlVisibility(currentSetup != null && currentSetup.SupportsFeature(Features.Isc));
		SetPirControlVisibility(currentSetup != null && currentSetup.SupportsFeature(Features.Pir));
		SetPartNumberControlVisibility(currentSetup != null && currentSetup.SupportsFeature(Features.RequirePartNumbers));
		SetControlVisibility(buttonReset, currentSetup != null && (currentSetup.SupportsFeature(Features.Isc) || currentSetup.SupportsFeature(Features.Pir)));
	}

	private void SetIscControlVisibility(bool visible)
	{
		SetControlVisibility(iscLabel, visible);
		SetControlVisibility((Control)(object)iscInstrument1, visible);
		SetControlVisibility((Control)(object)iscInstrument2, visible);
		SetControlVisibility((Control)(object)iscInstrument3, visible);
		SetControlVisibility((Control)(object)iscInstrument4, visible);
		SetControlVisibility((Control)(object)iscInstrument5, visible);
		SetControlVisibility((Control)(object)iscInstrument6, visible);
	}

	private void SetPirControlVisibility(bool visible)
	{
		SetControlVisibility(minLabel, visible);
		SetControlVisibility(minLabel1, visible);
		SetControlVisibility(minLabel2, visible);
		SetControlVisibility(minLabel3, visible);
		SetControlVisibility(minLabel4, visible);
		SetControlVisibility(minLabel5, visible);
		SetControlVisibility(minLabel6, visible);
		SetControlVisibility(maxLabel, visible);
		SetControlVisibility(maxLabel1, visible);
		SetControlVisibility(maxLabel2, visible);
		SetControlVisibility(maxLabel3, visible);
		SetControlVisibility(maxLabel4, visible);
		SetControlVisibility(maxLabel5, visible);
		SetControlVisibility(maxLabel6, visible);
		SetControlVisibility(pirLabel, visible);
		SetControlVisibility((Control)(object)pirInstrument1, visible);
		SetControlVisibility((Control)(object)pirInstrument2, visible);
		SetControlVisibility((Control)(object)pirInstrument3, visible);
		SetControlVisibility((Control)(object)pirInstrument4, visible);
		SetControlVisibility((Control)(object)pirInstrument5, visible);
		SetControlVisibility((Control)(object)pirInstrument6, visible);
	}

	private void SetPartNumberControlVisibility(bool visible)
	{
		SetControlVisibility(partNumberLabel, visible);
		SetControlVisibility(pictureBoxExamplePartNumber, visible);
		Checkmark[] array = partNumberCheckMarks;
		for (int i = 0; i < array.Length; i++)
		{
			Control control = (Control)(object)array[i];
			SetControlVisibility(control, visible);
		}
		TextBox[] array2 = partNumbers;
		foreach (Control control2 in array2)
		{
			SetControlVisibility(control2, visible);
		}
		System.Windows.Forms.Label[] array3 = cylinderLabels;
		foreach (System.Windows.Forms.Label control3 in array3)
		{
			((TableLayoutPanel)(object)mainLayoutPanel).SetColumnSpan((Control)control3, visible ? 1 : 3);
		}
	}

	private static void SetControlVisibility(Control control, bool visible)
	{
		if (control != null && control.Visible != visible)
		{
			control.Visible = visible;
		}
	}

	private void OnChannelStateUpdate(object sender, CommunicationsStateEventArgs e)
	{
		UpdateCurrentSetup();
		UpdateUserInterface();
	}

	private void OnEngineStateChanged(object sender, EventArgs e)
	{
		UpdateUserInterface();
	}

	private void UpdateUserInterface()
	{
		buttonRead.Enabled = CanReadTrimCodes;
		buttonWrite.Enabled = CanWriteTrimCodes;
		buttonReset.Enabled = CanResetPirValues;
		buttonPrint.Enabled = CanPrintCodes;
		buttonClose.Enabled = CanClose;
		TextBox[] array = trimCodes;
		foreach (TextBox textBox in array)
		{
			textBox.ReadOnly = !Online || Working;
			bool flag = UpdatePartNumber((int)textBox.Tag);
			textBox.Enabled = !textBox.ReadOnly && flag;
			textBox.MaxLength = MaxTextLength;
			Checkmark val = checkMarks[(int)textBox.Tag];
			if (Working || !Online)
			{
				textBox.BackColor = SystemColors.ButtonFace;
				val.CheckState = CheckState.Indeterminate;
			}
			else if (string.IsNullOrEmpty(textBox.Text) && !flag)
			{
				textBox.BackColor = SystemColors.Window;
				val.CheckState = CheckState.Indeterminate;
			}
			else if (ValidTrimCode(textBox.Text))
			{
				textBox.BackColor = Color.PaleGreen;
				val.CheckState = CheckState.Checked;
			}
			else
			{
				textBox.BackColor = Color.LightPink;
				val.CheckState = CheckState.Unchecked;
			}
		}
	}

	private bool UpdatePartNumber(int partNumberIndex)
	{
		bool flag = ValidatePartNumber(partNumberIndex);
		if (flag)
		{
			partNumbers[partNumberIndex].BackColor = SystemColors.Window;
			partNumberCheckMarks[partNumberIndex].CheckState = CheckState.Checked;
		}
		else if (string.IsNullOrEmpty(partNumbers[partNumberIndex].Text))
		{
			partNumbers[partNumberIndex].BackColor = SystemColors.Window;
			partNumberCheckMarks[partNumberIndex].CheckState = CheckState.Indeterminate;
		}
		else
		{
			partNumbers[partNumberIndex].BackColor = Color.LightPink;
			partNumberCheckMarks[partNumberIndex].CheckState = CheckState.Unchecked;
		}
		return flag;
	}

	private void ResetData()
	{
		nextState = State.None;
		ClearPartNumbers();
		TextBox[] array = trimCodes;
		foreach (TextBox textBox in array)
		{
			textBox.Text = string.Empty;
		}
		ClearResults();
	}

	private void ClearResults()
	{
		if (textboxResults != null)
		{
			textboxResults.Text = string.Empty;
		}
	}

	private void ClearPartNumbers()
	{
		TextBox[] array = partNumbers;
		foreach (TextBox textBox in array)
		{
			textBox.Text = string.Empty;
		}
	}

	private void ReportResult(string text)
	{
		if (textboxResults != null)
		{
			textboxResults.AppendText(text + "\r\n");
			textboxResults.SelectionStart = textboxResults.TextLength;
			textboxResults.SelectionLength = 0;
			textboxResults.ScrollToCaret();
		}
		((CustomPanel)this).AddStatusMessage(text);
	}

	private bool ValidTrimCode(string trimCode)
	{
		if (currentSetup != null)
		{
			return currentSetup.ValidTrimCode(trimCode);
		}
		return false;
	}

	private void OnTextChanged(object sender, EventArgs e)
	{
		UpdateUserInterface();
	}

	private void OnKeyPress(object sender, KeyPressEventArgs e)
	{
		if (e.KeyChar == '\b')
		{
			return;
		}
		char trimCharacter = e.KeyChar;
		if (currentSetup != null)
		{
			if (currentSetup.ValidateAndCorrect(ref trimCharacter))
			{
				e.KeyChar = trimCharacter;
			}
			else
			{
				e.Handled = true;
			}
		}
	}

	private void OnReadClick(object sender, EventArgs e)
	{
		ReadTrimCodes();
	}

	private void ReadTrimCodes()
	{
		if (CanReadTrimCodes)
		{
			nextState = State.Read;
			AdvanceState();
		}
	}

	private void OnWriteClick(object sender, EventArgs e)
	{
		if (CanWriteTrimCodes)
		{
			nextState = State.Write;
			AdvanceState();
		}
	}

	private void ReadCurrentTrimCode()
	{
		currentService = ((CustomPanel)this).GetService("MCM02T", "DJ_Read_E_Trim");
		if (currentService != null)
		{
			currentService.InputValues[0].Value = currentCylinder;
			currentService.ServiceCompleteEvent += OnReadServiceComplete;
			currentService.Execute(synchronous: false);
		}
		else
		{
			AdvanceState();
		}
	}

	private void OnReadServiceComplete(object sender, ResultEventArgs e)
	{
		currentService.ServiceCompleteEvent -= OnReadServiceComplete;
		if (e.Succeeded)
		{
			string text = currentService.OutputValues[0].Value.ToString();
			string text2 = currentSetup.AddCrc(text);
			if (text.Length == currentSetup.CodeLengthWithoutCRC)
			{
				if (text2.Contains("?"))
				{
					ReportResult(string.Format(Resources.MessageFormat_Cylinder0InjectorCodeHasAnInvalidValueOf1, currentCylinder, text2));
					readOperationSuccessful = false;
				}
				else
				{
					oldTrimCodes[currentCylinder - 1] = text2;
					ReportResult(string.Format(Resources.MessageFormat_Cylinder0InjectorCodeIs1, currentCylinder, text2));
					readOperationSuccessful = true;
				}
			}
			else
			{
				ReportResult(string.Format(Resources.MessageFormat_Cylinder0InjectorCodeCouldNotBeRead1, currentCylinder, currentService.OutputValues[0].Value.ToString()));
				readOperationSuccessful = false;
			}
			trimCodes[currentCylinder - 1].Text = text2;
			currentService = null;
		}
		else
		{
			trimCodes[currentCylinder - 1].Text = new string('?', currentSetup.CodeLengthWithCRC);
			ReportResult(string.Format(Resources.MessageFormat_Cylinder0InjectorCodeCouldNotBeReadServiceFailedToExecute, currentCylinder));
			readOperationSuccessful = false;
		}
		AdvanceState();
	}

	private void ReadRgpLeakValues()
	{
		Channel channel = ((CustomPanel)this).GetChannel("MCM02T");
		if (channel != null)
		{
			Parameter parameter = channel.Parameters["HP_Leak_Learned_Value"];
			if (parameter != null && parameter.Channel.Online)
			{
				parameter.Channel.Parameters.ReadGroup(parameter.GroupQualifier, fromCache: false, synchronous: false);
			}
		}
	}

	private void WriteCurrentTrimCode()
	{
		string text = trimCodes[currentCylinder - 1].Text;
		if (ValidTrimCode(text))
		{
			currentService = ((CustomPanel)this).GetService("MCM02T", "DJ_Write_E_Trim");
			if (currentService != null)
			{
				ReportResult(string.Format(Resources.MessageFormat_SendingCylinder0InjectorCodeAs1, currentCylinder, text));
				currentService.InputValues[0].Value = currentCylinder;
				currentService.InputValues[1].Value = currentSetup.StripCrc(text);
				currentService.ServiceCompleteEvent += OnWriteServiceComplete;
				currentService.Execute(synchronous: false);
				needReset[currentCylinder - 1] = text != oldTrimCodes[currentCylinder - 1];
			}
			else
			{
				ReportResult(string.Format(Resources.MessageFormat_Cylinder0InjectorCodeCouldNotBeSentServiceWasUnavailable, currentCylinder));
				writeOperationSuccessful = false;
				AdvanceState();
			}
		}
		else
		{
			AdvanceState();
		}
	}

	private void OnWriteServiceComplete(object sender, ResultEventArgs e)
	{
		currentService.ServiceCompleteEvent -= OnWriteServiceComplete;
		if (e.Succeeded)
		{
			string arg = currentService.OutputValues[0].Value.ToString();
			ReportResult(string.Format(Resources.MessageFormat_Cylinder01, currentCylinder, arg));
			currentService = null;
			writeOperationSuccessful = true;
		}
		else
		{
			ReportResult(string.Format(Resources.MessageFormat_Cylinder0InjectorCodeCouldNotBeSentServiceFailedToExecute, currentCylinder));
			writeOperationSuccessful = false;
		}
		AdvanceState();
	}

	private void OnParametersReadComplete(object sender, ResultEventArgs e)
	{
		mcm.Parameters.ParametersReadCompleteEvent -= OnParametersReadComplete;
		if (e.Succeeded && mcm != null)
		{
			Parameter parameter = mcm.Parameters[pirQualifiers[currentCylinder - 1]];
			if (parameter != null)
			{
				ReportResult(string.Format(Resources.MessageFormat_PIRForCylinder0Read1, currentCylinder, parameter.Value));
				readOperationSuccessful = true;
			}
			else
			{
				ReportResult(string.Format(Resources.MessageFormat_PIRForCylinder0CouldNotBeRead, currentCylinder));
				readOperationSuccessful = false;
			}
		}
		else
		{
			ReportResult(string.Format(Resources.MessageFormat_AnErrorOccurredWhileReadingParameters0, e.Exception.Message));
			readOperationSuccessful = false;
		}
		AdvanceState();
	}

	private void ReadCurrentPirValue()
	{
		bool flag = false;
		if (mcm != null)
		{
			Parameter parameter = mcm.Parameters[pirQualifiers[currentCylinder - 1]];
			if (parameter != null)
			{
				mcm.Parameters.ParametersReadCompleteEvent += OnParametersReadComplete;
				mcm.Parameters.ReadGroup(parameter.GroupQualifier, fromCache: false, synchronous: false);
				flag = true;
			}
		}
		if (!flag)
		{
			AdvanceState();
		}
	}

	private void ResetCurrentPir()
	{
		bool flag = false;
		if (needReset[currentCylinder - 1])
		{
			currentService = ((CustomPanel)this).GetService("MCM02T", "RT_SR070_Injector_Change_Start");
			if (currentService != null)
			{
				currentService.ServiceCompleteEvent += OnResetServiceCompleteEvent;
				currentService.InputValues[0].Value = currentService.InputValues[0].Choices[currentCylinder - 1];
				currentService.Execute(synchronous: false);
				flag = true;
			}
		}
		if (!flag)
		{
			AdvanceState();
		}
	}

	private void OnResetServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		currentService.ServiceCompleteEvent -= OnResetServiceCompleteEvent;
		if (e.Succeeded)
		{
			ReportResult(string.Format(Resources.MessageFormat_SuccessfullyResetPIRDataForCylinder0, currentCylinder));
			writeOperationSuccessful = true;
		}
		else
		{
			ReportResult(string.Format(Resources.MessageFormat_Cylinder0FailedToResetPIRDataServiceFailedToExecute, currentCylinder));
			writeOperationSuccessful = false;
		}
		AdvanceState();
	}

	private void OnResetClick(object sender, EventArgs e)
	{
		nextState = State.ResetAll;
		AdvanceState();
	}

	public override string ToHtml()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("<div class=\"standard\">");
		stringBuilder.Append("<p class=\"standard\">");
		string[] lines = textboxResults.Lines;
		foreach (string arg in lines)
		{
			stringBuilder.AppendFormat("{0}<br/>", arg);
		}
		stringBuilder.Append("</p></div>");
		return stringBuilder.ToString();
	}

	private void OnPartNumberChanged(object sender, EventArgs e)
	{
		UpdateUserInterface();
	}

	private bool ValidatePartNumber(int partNumberIndex)
	{
		if (currentSetup != null && !currentSetup.SupportsFeature(Features.RequirePartNumbers))
		{
			return true;
		}
		Regex regex;
		switch ((GetEngineTypeName() ?? "").ToUpper())
		{
		case "DD13":
			regex = dd13InjectorPartValidator;
			break;
		case "DD15":
		case "DD16":
			regex = dd15Or16InjectorPartValidator;
			break;
		default:
			return false;
		}
		return regex.IsMatch(partNumbers[partNumberIndex].Text);
	}

	private void OnPrintClick(object sender, EventArgs e)
	{
		if (CanPrintCodes)
		{
			PrintHelper.ShowPrintDialog(((ContainerControl)this).ParentForm.Text, (IProvideHtml)(object)this, (IncludeInfo)3);
		}
	}

	private void ResetAllPir()
	{
		currentService = ((CustomPanel)this).GetService("MCM02T", "RT_SR074_EcuInitPIR_Start");
		if (currentService != null)
		{
			currentService.ServiceCompleteEvent += OnResetAllServiceCompleteEvent;
			currentService.Execute(synchronous: false);
		}
		else
		{
			AdvanceState();
		}
	}

	private void OnResetAllServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		currentService.ServiceCompleteEvent -= OnResetAllServiceCompleteEvent;
		if (e.Succeeded)
		{
			ReportResult(Resources.Message_SuccessfullyResetPIRDataForAllCylinders);
			writeOperationSuccessful = true;
		}
		else
		{
			ReportResult(Resources.Message_FailedToResetPIRDataForAllCylindersServiceFailedToExecute);
			writeOperationSuccessful = false;
		}
		AdvanceState();
	}

	private void ResetRpgLeakValues()
	{
		currentService = ((CustomPanel)this).GetService("MCM02T", "RT_SR014_SET_EOL_Default_Values_Start");
		if (currentService != null)
		{
			currentService.ServiceCompleteEvent += OnResetRpgLeakValuesCompleteEvent;
			currentService.InputValues[0].Value = currentService.InputValues[0].Choices.GetItemFromRawValue(24);
			currentService.Execute(synchronous: false);
		}
		else
		{
			AdvanceState();
		}
	}

	private void OnResetRpgLeakValuesCompleteEvent(object sender, ResultEventArgs e)
	{
		currentService.ServiceCompleteEvent -= OnResetRpgLeakValuesCompleteEvent;
		if (e.Succeeded)
		{
			ReportResult(Resources.Message_RPGLeakValuesHaveBeenReset);
		}
		else
		{
			writeOperationSuccessful = false;
			ReportResult(Resources.Message_FailedToResetRPGLeakValues);
		}
		AdvanceState();
	}

	private void InitializeComponent()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Expected O, but got Unknown
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Expected O, but got Unknown
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Expected O, but got Unknown
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Expected O, but got Unknown
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Expected O, but got Unknown
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Expected O, but got Unknown
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Expected O, but got Unknown
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Expected O, but got Unknown
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Expected O, but got Unknown
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Expected O, but got Unknown
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Expected O, but got Unknown
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Expected O, but got Unknown
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Expected O, but got Unknown
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Expected O, but got Unknown
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Expected O, but got Unknown
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Expected O, but got Unknown
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Expected O, but got Unknown
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Expected O, but got Unknown
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Expected O, but got Unknown
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Expected O, but got Unknown
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Expected O, but got Unknown
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Expected O, but got Unknown
		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Expected O, but got Unknown
		//IL_0340: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Expected O, but got Unknown
		//IL_1224: Unknown result type (might be due to invalid IL or missing references)
		//IL_1325: Unknown result type (might be due to invalid IL or missing references)
		//IL_13e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_14a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_155f: Unknown result type (might be due to invalid IL or missing references)
		//IL_161d: Unknown result type (might be due to invalid IL or missing references)
		//IL_16db: Unknown result type (might be due to invalid IL or missing references)
		//IL_1799: Unknown result type (might be due to invalid IL or missing references)
		//IL_186a: Unknown result type (might be due to invalid IL or missing references)
		//IL_193b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a3c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b0d: Unknown result type (might be due to invalid IL or missing references)
		//IL_212f: Unknown result type (might be due to invalid IL or missing references)
		//IL_21a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_21e4: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		mainLayoutPanel = new TableLayoutPanel();
		flowLayoutPanel1 = new FlowLayoutPanel();
		panelPictures = new Panel();
		pictureBoxExamplePartNumber = new PictureBox();
		partNumberLabel = new System.Windows.Forms.Label();
		textBoxPartNumber1 = new TextBox();
		checkmarkPartNumber1 = new Checkmark();
		checkmarkPartNumber2 = new Checkmark();
		textBoxPartNumber2 = new TextBox();
		checkmarkPartNumber3 = new Checkmark();
		textBoxPartNumber3 = new TextBox();
		checkmarkPartNumber4 = new Checkmark();
		textBoxPartNumber4 = new TextBox();
		checkmarkPartNumber5 = new Checkmark();
		textBoxPartNumber5 = new TextBox();
		checkmarkPartNumber6 = new Checkmark();
		textBoxPartNumber6 = new TextBox();
		maxLabel6 = new System.Windows.Forms.Label();
		textTrimCode6 = new TextBox();
		textTrimCode5 = new TextBox();
		textTrimCode4 = new TextBox();
		textTrimCode3 = new TextBox();
		textTrimCode2 = new TextBox();
		textTrimCode1 = new TextBox();
		checkMark6 = new Checkmark();
		checkMark4 = new Checkmark();
		checkMark5 = new Checkmark();
		maxLabel5 = new System.Windows.Forms.Label();
		maxLabel4 = new System.Windows.Forms.Label();
		maxLabel3 = new System.Windows.Forms.Label();
		checkMark3 = new Checkmark();
		maxLabel2 = new System.Windows.Forms.Label();
		checkMark2 = new Checkmark();
		maxLabel1 = new System.Windows.Forms.Label();
		checkMark1 = new Checkmark();
		minLabel6 = new System.Windows.Forms.Label();
		minLabel5 = new System.Windows.Forms.Label();
		minLabel4 = new System.Windows.Forms.Label();
		minLabel3 = new System.Windows.Forms.Label();
		minLabel2 = new System.Windows.Forms.Label();
		minLabel1 = new System.Windows.Forms.Label();
		maxLabel = new System.Windows.Forms.Label();
		iscInstrument6 = new DigitalReadoutInstrument();
		pirLabel = new System.Windows.Forms.Label();
		iscInstrument1 = new DigitalReadoutInstrument();
		pirInstrument6 = new DigitalReadoutInstrument();
		pirInstrument5 = new DigitalReadoutInstrument();
		pirInstrument4 = new DigitalReadoutInstrument();
		pirInstrument3 = new DigitalReadoutInstrument();
		pirInstrument2 = new DigitalReadoutInstrument();
		pirInstrument1 = new DigitalReadoutInstrument();
		iscInstrument5 = new DigitalReadoutInstrument();
		iscInstrument4 = new DigitalReadoutInstrument();
		lblCylinderPosition = new System.Windows.Forms.Label();
		iscInstrument3 = new DigitalReadoutInstrument();
		iscInstrument2 = new DigitalReadoutInstrument();
		pictureBoxHDEP = new PictureBox();
		minLabel = new System.Windows.Forms.Label();
		textboxResults = new TextBox();
		labelEntryTips = new System.Windows.Forms.Label();
		label1 = new System.Windows.Forms.Label();
		label6 = new System.Windows.Forms.Label();
		label5 = new System.Windows.Forms.Label();
		label4 = new System.Windows.Forms.Label();
		label3 = new System.Windows.Forms.Label();
		label2 = new System.Windows.Forms.Label();
		label20 = new System.Windows.Forms.Label();
		iscLabel = new System.Windows.Forms.Label();
		buttonClose = new Button();
		buttonRead = new Button();
		buttonWrite = new Button();
		buttonReset = new Button();
		buttonPrint = new Button();
		digitalReadoutInstrumentEngineState = new DigitalReadoutInstrument();
		digitalReadoutLeakLearnedValue = new DigitalReadoutInstrument();
		((Control)(object)mainLayoutPanel).SuspendLayout();
		((ISupportInitialize)pictureBoxExamplePartNumber).BeginInit();
		panelPictures.SuspendLayout();
		((ISupportInitialize)pictureBoxHDEP).BeginInit();
		flowLayoutPanel1.SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(mainLayoutPanel, "mainLayoutPanel");
		((TableLayoutPanel)(object)mainLayoutPanel).Controls.Add(pictureBoxExamplePartNumber, 2, 9);
		((TableLayoutPanel)(object)mainLayoutPanel).Controls.Add(partNumberLabel, 2, 2);
		((TableLayoutPanel)(object)mainLayoutPanel).Controls.Add(textBoxPartNumber1, 2, 3);
		((TableLayoutPanel)(object)mainLayoutPanel).Controls.Add((Control)(object)checkmarkPartNumber1, 1, 3);
		((TableLayoutPanel)(object)mainLayoutPanel).Controls.Add((Control)(object)checkmarkPartNumber2, 1, 4);
		((TableLayoutPanel)(object)mainLayoutPanel).Controls.Add(textBoxPartNumber2, 2, 4);
		((TableLayoutPanel)(object)mainLayoutPanel).Controls.Add((Control)(object)checkmarkPartNumber3, 1, 5);
		((TableLayoutPanel)(object)mainLayoutPanel).Controls.Add(textBoxPartNumber3, 2, 5);
		((TableLayoutPanel)(object)mainLayoutPanel).Controls.Add((Control)(object)checkmarkPartNumber4, 1, 6);
		((TableLayoutPanel)(object)mainLayoutPanel).Controls.Add(textBoxPartNumber4, 2, 6);
		((TableLayoutPanel)(object)mainLayoutPanel).Controls.Add((Control)(object)checkmarkPartNumber5, 1, 7);
		((TableLayoutPanel)(object)mainLayoutPanel).Controls.Add(textBoxPartNumber5, 2, 7);
		((TableLayoutPanel)(object)mainLayoutPanel).Controls.Add((Control)(object)checkmarkPartNumber6, 1, 8);
		((TableLayoutPanel)(object)mainLayoutPanel).Controls.Add(textBoxPartNumber6, 2, 8);
		((TableLayoutPanel)(object)mainLayoutPanel).Controls.Add(maxLabel6, 9, 8);
		((TableLayoutPanel)(object)mainLayoutPanel).Controls.Add(textTrimCode6, 4, 8);
		((TableLayoutPanel)(object)mainLayoutPanel).Controls.Add(textTrimCode5, 4, 7);
		((TableLayoutPanel)(object)mainLayoutPanel).Controls.Add(textTrimCode4, 4, 6);
		((TableLayoutPanel)(object)mainLayoutPanel).Controls.Add(textTrimCode3, 4, 5);
		((TableLayoutPanel)(object)mainLayoutPanel).Controls.Add(textTrimCode2, 4, 4);
		((TableLayoutPanel)(object)mainLayoutPanel).Controls.Add(textTrimCode1, 4, 3);
		((TableLayoutPanel)(object)mainLayoutPanel).Controls.Add((Control)(object)checkMark6, 3, 8);
		((TableLayoutPanel)(object)mainLayoutPanel).Controls.Add((Control)(object)checkMark4, 3, 6);
		((TableLayoutPanel)(object)mainLayoutPanel).Controls.Add((Control)(object)checkMark5, 3, 7);
		((TableLayoutPanel)(object)mainLayoutPanel).Controls.Add(maxLabel5, 9, 7);
		((TableLayoutPanel)(object)mainLayoutPanel).Controls.Add(maxLabel4, 9, 6);
		((TableLayoutPanel)(object)mainLayoutPanel).Controls.Add(maxLabel3, 9, 5);
		((TableLayoutPanel)(object)mainLayoutPanel).Controls.Add((Control)(object)checkMark3, 3, 5);
		((TableLayoutPanel)(object)mainLayoutPanel).Controls.Add(maxLabel2, 9, 4);
		((TableLayoutPanel)(object)mainLayoutPanel).Controls.Add((Control)(object)checkMark2, 3, 4);
		((TableLayoutPanel)(object)mainLayoutPanel).Controls.Add(maxLabel1, 9, 3);
		((TableLayoutPanel)(object)mainLayoutPanel).Controls.Add((Control)(object)checkMark1, 3, 3);
		((TableLayoutPanel)(object)mainLayoutPanel).Controls.Add(minLabel6, 7, 8);
		((TableLayoutPanel)(object)mainLayoutPanel).Controls.Add(minLabel5, 7, 7);
		((TableLayoutPanel)(object)mainLayoutPanel).Controls.Add(minLabel4, 7, 6);
		((TableLayoutPanel)(object)mainLayoutPanel).Controls.Add(minLabel3, 7, 5);
		((TableLayoutPanel)(object)mainLayoutPanel).Controls.Add(minLabel2, 7, 4);
		((TableLayoutPanel)(object)mainLayoutPanel).Controls.Add(minLabel1, 7, 3);
		((TableLayoutPanel)(object)mainLayoutPanel).Controls.Add(maxLabel, 9, 2);
		((TableLayoutPanel)(object)mainLayoutPanel).Controls.Add((Control)(object)iscInstrument6, 5, 8);
		((TableLayoutPanel)(object)mainLayoutPanel).Controls.Add(pirLabel, 8, 2);
		((TableLayoutPanel)(object)mainLayoutPanel).Controls.Add((Control)(object)iscInstrument1, 5, 3);
		((TableLayoutPanel)(object)mainLayoutPanel).Controls.Add((Control)(object)pirInstrument6, 8, 8);
		((TableLayoutPanel)(object)mainLayoutPanel).Controls.Add((Control)(object)pirInstrument5, 8, 7);
		((TableLayoutPanel)(object)mainLayoutPanel).Controls.Add((Control)(object)pirInstrument4, 8, 6);
		((TableLayoutPanel)(object)mainLayoutPanel).Controls.Add((Control)(object)pirInstrument3, 8, 5);
		((TableLayoutPanel)(object)mainLayoutPanel).Controls.Add((Control)(object)pirInstrument2, 8, 4);
		((TableLayoutPanel)(object)mainLayoutPanel).Controls.Add((Control)(object)pirInstrument1, 8, 3);
		((TableLayoutPanel)(object)mainLayoutPanel).Controls.Add((Control)(object)iscInstrument5, 5, 7);
		((TableLayoutPanel)(object)mainLayoutPanel).Controls.Add((Control)(object)iscInstrument4, 5, 6);
		((TableLayoutPanel)(object)mainLayoutPanel).Controls.Add(lblCylinderPosition, 0, 2);
		((TableLayoutPanel)(object)mainLayoutPanel).Controls.Add((Control)(object)iscInstrument3, 5, 5);
		((TableLayoutPanel)(object)mainLayoutPanel).Controls.Add((Control)(object)iscInstrument2, 5, 4);
		((TableLayoutPanel)(object)mainLayoutPanel).Controls.Add(panelPictures, 4, 9);
		((TableLayoutPanel)(object)mainLayoutPanel).Controls.Add(minLabel, 7, 2);
		((TableLayoutPanel)(object)mainLayoutPanel).Controls.Add(textboxResults, 6, 9);
		((TableLayoutPanel)(object)mainLayoutPanel).Controls.Add(labelEntryTips, 0, 0);
		((TableLayoutPanel)(object)mainLayoutPanel).Controls.Add(label1, 0, 3);
		((TableLayoutPanel)(object)mainLayoutPanel).Controls.Add(label6, 0, 8);
		((TableLayoutPanel)(object)mainLayoutPanel).Controls.Add(label5, 0, 7);
		((TableLayoutPanel)(object)mainLayoutPanel).Controls.Add(label4, 0, 6);
		((TableLayoutPanel)(object)mainLayoutPanel).Controls.Add(label3, 0, 5);
		((TableLayoutPanel)(object)mainLayoutPanel).Controls.Add(label2, 0, 4);
		((TableLayoutPanel)(object)mainLayoutPanel).Controls.Add(label20, 4, 2);
		((TableLayoutPanel)(object)mainLayoutPanel).Controls.Add(iscLabel, 5, 2);
		((TableLayoutPanel)(object)mainLayoutPanel).Controls.Add(buttonClose, 9, 11);
		((TableLayoutPanel)(object)mainLayoutPanel).Controls.Add(flowLayoutPanel1, 0, 10);
		((TableLayoutPanel)(object)mainLayoutPanel).Controls.Add((Control)(object)digitalReadoutInstrumentEngineState, 7, 0);
		((TableLayoutPanel)(object)mainLayoutPanel).Controls.Add((Control)(object)digitalReadoutLeakLearnedValue, 7, 1);
		((TableLayoutPanel)(object)mainLayoutPanel).GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
		((Control)(object)mainLayoutPanel).Name = "mainLayoutPanel";
		componentResourceManager.ApplyResources(pictureBoxExamplePartNumber, "pictureBoxExamplePartNumber");
		pictureBoxExamplePartNumber.Name = "pictureBoxExamplePartNumber";
		pictureBoxExamplePartNumber.TabStop = false;
		componentResourceManager.ApplyResources(partNumberLabel, "partNumberLabel");
		partNumberLabel.Name = "partNumberLabel";
		partNumberLabel.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(textBoxPartNumber1, "textBoxPartNumber1");
		textBoxPartNumber1.Name = "textBoxPartNumber1";
		checkmarkPartNumber1.CheckState = CheckState.Indeterminate;
		componentResourceManager.ApplyResources(checkmarkPartNumber1, "checkmarkPartNumber1");
		((Control)(object)checkmarkPartNumber1).Name = "checkmarkPartNumber1";
		checkmarkPartNumber2.CheckState = CheckState.Indeterminate;
		componentResourceManager.ApplyResources(checkmarkPartNumber2, "checkmarkPartNumber2");
		((Control)(object)checkmarkPartNumber2).Name = "checkmarkPartNumber2";
		componentResourceManager.ApplyResources(textBoxPartNumber2, "textBoxPartNumber2");
		textBoxPartNumber2.Name = "textBoxPartNumber2";
		checkmarkPartNumber3.CheckState = CheckState.Indeterminate;
		componentResourceManager.ApplyResources(checkmarkPartNumber3, "checkmarkPartNumber3");
		((Control)(object)checkmarkPartNumber3).Name = "checkmarkPartNumber3";
		componentResourceManager.ApplyResources(textBoxPartNumber3, "textBoxPartNumber3");
		textBoxPartNumber3.Name = "textBoxPartNumber3";
		checkmarkPartNumber4.CheckState = CheckState.Indeterminate;
		componentResourceManager.ApplyResources(checkmarkPartNumber4, "checkmarkPartNumber4");
		((Control)(object)checkmarkPartNumber4).Name = "checkmarkPartNumber4";
		componentResourceManager.ApplyResources(textBoxPartNumber4, "textBoxPartNumber4");
		textBoxPartNumber4.Name = "textBoxPartNumber4";
		checkmarkPartNumber5.CheckState = CheckState.Indeterminate;
		componentResourceManager.ApplyResources(checkmarkPartNumber5, "checkmarkPartNumber5");
		((Control)(object)checkmarkPartNumber5).Name = "checkmarkPartNumber5";
		componentResourceManager.ApplyResources(textBoxPartNumber5, "textBoxPartNumber5");
		textBoxPartNumber5.Name = "textBoxPartNumber5";
		checkmarkPartNumber6.CheckState = CheckState.Indeterminate;
		componentResourceManager.ApplyResources(checkmarkPartNumber6, "checkmarkPartNumber6");
		((Control)(object)checkmarkPartNumber6).Name = "checkmarkPartNumber6";
		componentResourceManager.ApplyResources(textBoxPartNumber6, "textBoxPartNumber6");
		textBoxPartNumber6.Name = "textBoxPartNumber6";
		componentResourceManager.ApplyResources(maxLabel6, "maxLabel6");
		maxLabel6.Name = "maxLabel6";
		maxLabel6.UseCompatibleTextRendering = true;
		textTrimCode6.CharacterCasing = CharacterCasing.Upper;
		componentResourceManager.ApplyResources(textTrimCode6, "textTrimCode6");
		textTrimCode6.Name = "textTrimCode6";
		textTrimCode5.CharacterCasing = CharacterCasing.Upper;
		componentResourceManager.ApplyResources(textTrimCode5, "textTrimCode5");
		textTrimCode5.Name = "textTrimCode5";
		textTrimCode4.CharacterCasing = CharacterCasing.Upper;
		componentResourceManager.ApplyResources(textTrimCode4, "textTrimCode4");
		textTrimCode4.Name = "textTrimCode4";
		textTrimCode3.CharacterCasing = CharacterCasing.Upper;
		componentResourceManager.ApplyResources(textTrimCode3, "textTrimCode3");
		textTrimCode3.Name = "textTrimCode3";
		textTrimCode2.CharacterCasing = CharacterCasing.Upper;
		componentResourceManager.ApplyResources(textTrimCode2, "textTrimCode2");
		textTrimCode2.Name = "textTrimCode2";
		textTrimCode1.CharacterCasing = CharacterCasing.Upper;
		componentResourceManager.ApplyResources(textTrimCode1, "textTrimCode1");
		textTrimCode1.Name = "textTrimCode1";
		checkMark6.CheckState = CheckState.Indeterminate;
		componentResourceManager.ApplyResources(checkMark6, "checkMark6");
		((Control)(object)checkMark6).Name = "checkMark6";
		checkMark4.CheckState = CheckState.Indeterminate;
		componentResourceManager.ApplyResources(checkMark4, "checkMark4");
		((Control)(object)checkMark4).Name = "checkMark4";
		checkMark5.CheckState = CheckState.Indeterminate;
		componentResourceManager.ApplyResources(checkMark5, "checkMark5");
		((Control)(object)checkMark5).Name = "checkMark5";
		componentResourceManager.ApplyResources(maxLabel5, "maxLabel5");
		maxLabel5.Name = "maxLabel5";
		maxLabel5.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(maxLabel4, "maxLabel4");
		maxLabel4.Name = "maxLabel4";
		maxLabel4.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(maxLabel3, "maxLabel3");
		maxLabel3.Name = "maxLabel3";
		maxLabel3.UseCompatibleTextRendering = true;
		checkMark3.CheckState = CheckState.Indeterminate;
		componentResourceManager.ApplyResources(checkMark3, "checkMark3");
		((Control)(object)checkMark3).Name = "checkMark3";
		componentResourceManager.ApplyResources(maxLabel2, "maxLabel2");
		maxLabel2.Name = "maxLabel2";
		maxLabel2.UseCompatibleTextRendering = true;
		checkMark2.CheckState = CheckState.Indeterminate;
		componentResourceManager.ApplyResources(checkMark2, "checkMark2");
		((Control)(object)checkMark2).Name = "checkMark2";
		componentResourceManager.ApplyResources(maxLabel1, "maxLabel1");
		maxLabel1.Name = "maxLabel1";
		maxLabel1.UseCompatibleTextRendering = true;
		checkMark1.CheckState = CheckState.Indeterminate;
		componentResourceManager.ApplyResources(checkMark1, "checkMark1");
		((Control)(object)checkMark1).Name = "checkMark1";
		componentResourceManager.ApplyResources(minLabel6, "minLabel6");
		minLabel6.Name = "minLabel6";
		minLabel6.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(minLabel5, "minLabel5");
		minLabel5.Name = "minLabel5";
		minLabel5.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(minLabel4, "minLabel4");
		minLabel4.Name = "minLabel4";
		minLabel4.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(minLabel3, "minLabel3");
		minLabel3.Name = "minLabel3";
		minLabel3.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(minLabel2, "minLabel2");
		minLabel2.Name = "minLabel2";
		minLabel2.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(minLabel1, "minLabel1");
		minLabel1.Name = "minLabel1";
		minLabel1.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(maxLabel, "maxLabel");
		maxLabel.Name = "maxLabel";
		maxLabel.UseCompatibleTextRendering = true;
		((TableLayoutPanel)(object)mainLayoutPanel).SetColumnSpan((Control)(object)iscInstrument6, 2);
		componentResourceManager.ApplyResources(iscInstrument6, "iscInstrument6");
		iscInstrument6.FontGroup = null;
		((SingleInstrumentBase)iscInstrument6).FreezeValue = false;
		iscInstrument6.Gradient.Initialize((ValueState)3, 2);
		iscInstrument6.Gradient.Modify(1, -100.0, (ValueState)1);
		iscInstrument6.Gradient.Modify(2, 99.5, (ValueState)3);
		((SingleInstrumentBase)iscInstrument6).Instrument = new Qualifier((QualifierTypes)1, "MCM02T", "DT_Idle_Speed_Balance_Values_Cylinder_6");
		((Control)(object)iscInstrument6).Name = "iscInstrument6";
		((SingleInstrumentBase)iscInstrument6).TitlePosition = (LabelPosition)0;
		((SingleInstrumentBase)iscInstrument6).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(pirLabel, "pirLabel");
		pirLabel.Name = "pirLabel";
		pirLabel.UseCompatibleTextRendering = true;
		((TableLayoutPanel)(object)mainLayoutPanel).SetColumnSpan((Control)(object)iscInstrument1, 2);
		componentResourceManager.ApplyResources(iscInstrument1, "iscInstrument1");
		iscInstrument1.FontGroup = null;
		((SingleInstrumentBase)iscInstrument1).FreezeValue = false;
		iscInstrument1.Gradient.Initialize((ValueState)3, 2);
		iscInstrument1.Gradient.Modify(1, -100.0, (ValueState)1);
		iscInstrument1.Gradient.Modify(2, 99.5, (ValueState)3);
		((SingleInstrumentBase)iscInstrument1).Instrument = new Qualifier((QualifierTypes)1, "MCM02T", "DT_Idle_Speed_Balance_Values_Cylinder_1");
		((Control)(object)iscInstrument1).Name = "iscInstrument1";
		((SingleInstrumentBase)iscInstrument1).TitlePosition = (LabelPosition)0;
		((SingleInstrumentBase)iscInstrument1).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(pirInstrument6, "pirInstrument6");
		pirInstrument6.FontGroup = null;
		((SingleInstrumentBase)pirInstrument6).FreezeValue = false;
		pirInstrument6.Gradient.Initialize((ValueState)3, 2);
		pirInstrument6.Gradient.Modify(1, 0.478, (ValueState)1);
		pirInstrument6.Gradient.Modify(2, 0.57, (ValueState)3);
		((SingleInstrumentBase)pirInstrument6).Instrument = new Qualifier((QualifierTypes)4, "MCM02T", "e2p_pir_adapt_cyl_6_NOP0");
		((Control)(object)pirInstrument6).Name = "pirInstrument6";
		((SingleInstrumentBase)pirInstrument6).TitlePosition = (LabelPosition)0;
		((SingleInstrumentBase)pirInstrument6).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(pirInstrument5, "pirInstrument5");
		pirInstrument5.FontGroup = null;
		((SingleInstrumentBase)pirInstrument5).FreezeValue = false;
		pirInstrument5.Gradient.Initialize((ValueState)3, 2);
		pirInstrument5.Gradient.Modify(1, 0.478, (ValueState)1);
		pirInstrument5.Gradient.Modify(2, 0.57, (ValueState)3);
		((SingleInstrumentBase)pirInstrument5).Instrument = new Qualifier((QualifierTypes)4, "MCM02T", "e2p_pir_adapt_cyl_5_NOP0");
		((Control)(object)pirInstrument5).Name = "pirInstrument5";
		((SingleInstrumentBase)pirInstrument5).TitlePosition = (LabelPosition)0;
		((SingleInstrumentBase)pirInstrument5).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(pirInstrument4, "pirInstrument4");
		pirInstrument4.FontGroup = null;
		((SingleInstrumentBase)pirInstrument4).FreezeValue = false;
		pirInstrument4.Gradient.Initialize((ValueState)3, 2);
		pirInstrument4.Gradient.Modify(1, 0.478, (ValueState)1);
		pirInstrument4.Gradient.Modify(2, 0.57, (ValueState)3);
		((SingleInstrumentBase)pirInstrument4).Instrument = new Qualifier((QualifierTypes)4, "MCM02T", "e2p_pir_adapt_cyl_4_NOP0");
		((Control)(object)pirInstrument4).Name = "pirInstrument4";
		((SingleInstrumentBase)pirInstrument4).TitlePosition = (LabelPosition)0;
		((SingleInstrumentBase)pirInstrument4).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(pirInstrument3, "pirInstrument3");
		pirInstrument3.FontGroup = null;
		((SingleInstrumentBase)pirInstrument3).FreezeValue = false;
		pirInstrument3.Gradient.Initialize((ValueState)3, 2);
		pirInstrument3.Gradient.Modify(1, 0.478, (ValueState)1);
		pirInstrument3.Gradient.Modify(2, 0.57, (ValueState)3);
		((SingleInstrumentBase)pirInstrument3).Instrument = new Qualifier((QualifierTypes)4, "MCM02T", "e2p_pir_adapt_cyl_3_NOP0");
		((Control)(object)pirInstrument3).Name = "pirInstrument3";
		((SingleInstrumentBase)pirInstrument3).TitlePosition = (LabelPosition)0;
		((SingleInstrumentBase)pirInstrument3).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(pirInstrument2, "pirInstrument2");
		pirInstrument2.FontGroup = null;
		((SingleInstrumentBase)pirInstrument2).FreezeValue = false;
		pirInstrument2.Gradient.Initialize((ValueState)3, 2);
		pirInstrument2.Gradient.Modify(1, 0.478, (ValueState)1);
		pirInstrument2.Gradient.Modify(2, 0.57, (ValueState)3);
		((SingleInstrumentBase)pirInstrument2).Instrument = new Qualifier((QualifierTypes)4, "MCM02T", "e2p_pir_adapt_cyl_2_NOP0");
		((Control)(object)pirInstrument2).Name = "pirInstrument2";
		((SingleInstrumentBase)pirInstrument2).TitlePosition = (LabelPosition)0;
		((SingleInstrumentBase)pirInstrument2).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(pirInstrument1, "pirInstrument1");
		pirInstrument1.FontGroup = null;
		((SingleInstrumentBase)pirInstrument1).FreezeValue = false;
		pirInstrument1.Gradient.Initialize((ValueState)3, 2);
		pirInstrument1.Gradient.Modify(1, 0.478, (ValueState)1);
		pirInstrument1.Gradient.Modify(2, 0.57, (ValueState)3);
		((SingleInstrumentBase)pirInstrument1).Instrument = new Qualifier((QualifierTypes)4, "MCM02T", "e2p_pir_adapt_cyl_1_NOP0");
		((Control)(object)pirInstrument1).Name = "pirInstrument1";
		((SingleInstrumentBase)pirInstrument1).TitlePosition = (LabelPosition)0;
		((SingleInstrumentBase)pirInstrument1).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)mainLayoutPanel).SetColumnSpan((Control)(object)iscInstrument5, 2);
		componentResourceManager.ApplyResources(iscInstrument5, "iscInstrument5");
		iscInstrument5.FontGroup = null;
		((SingleInstrumentBase)iscInstrument5).FreezeValue = false;
		iscInstrument5.Gradient.Initialize((ValueState)3, 2);
		iscInstrument5.Gradient.Modify(1, -100.0, (ValueState)1);
		iscInstrument5.Gradient.Modify(2, 99.5, (ValueState)3);
		((SingleInstrumentBase)iscInstrument5).Instrument = new Qualifier((QualifierTypes)1, "MCM02T", "DT_Idle_Speed_Balance_Values_Cylinder_5");
		((Control)(object)iscInstrument5).Name = "iscInstrument5";
		((SingleInstrumentBase)iscInstrument5).TitlePosition = (LabelPosition)0;
		((SingleInstrumentBase)iscInstrument5).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)mainLayoutPanel).SetColumnSpan((Control)(object)iscInstrument4, 2);
		componentResourceManager.ApplyResources(iscInstrument4, "iscInstrument4");
		iscInstrument4.FontGroup = null;
		((SingleInstrumentBase)iscInstrument4).FreezeValue = false;
		iscInstrument4.Gradient.Initialize((ValueState)3, 2);
		iscInstrument4.Gradient.Modify(1, -100.0, (ValueState)1);
		iscInstrument4.Gradient.Modify(2, 99.5, (ValueState)3);
		((SingleInstrumentBase)iscInstrument4).Instrument = new Qualifier((QualifierTypes)1, "MCM02T", "DT_Idle_Speed_Balance_Values_Cylinder_4");
		((Control)(object)iscInstrument4).Name = "iscInstrument4";
		((SingleInstrumentBase)iscInstrument4).TitlePosition = (LabelPosition)0;
		((SingleInstrumentBase)iscInstrument4).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(lblCylinderPosition, "lblCylinderPosition");
		lblCylinderPosition.Name = "lblCylinderPosition";
		lblCylinderPosition.UseCompatibleTextRendering = true;
		((TableLayoutPanel)(object)mainLayoutPanel).SetColumnSpan((Control)(object)iscInstrument3, 2);
		componentResourceManager.ApplyResources(iscInstrument3, "iscInstrument3");
		iscInstrument3.FontGroup = null;
		((SingleInstrumentBase)iscInstrument3).FreezeValue = false;
		iscInstrument3.Gradient.Initialize((ValueState)3, 2);
		iscInstrument3.Gradient.Modify(1, -100.0, (ValueState)1);
		iscInstrument3.Gradient.Modify(2, 99.5, (ValueState)3);
		((SingleInstrumentBase)iscInstrument3).Instrument = new Qualifier((QualifierTypes)1, "MCM02T", "DT_Idle_Speed_Balance_Values_Cylinder_3");
		((Control)(object)iscInstrument3).Name = "iscInstrument3";
		((SingleInstrumentBase)iscInstrument3).TitlePosition = (LabelPosition)0;
		((SingleInstrumentBase)iscInstrument3).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)mainLayoutPanel).SetColumnSpan((Control)(object)iscInstrument2, 2);
		componentResourceManager.ApplyResources(iscInstrument2, "iscInstrument2");
		iscInstrument2.FontGroup = null;
		((SingleInstrumentBase)iscInstrument2).FreezeValue = false;
		iscInstrument2.Gradient.Initialize((ValueState)3, 2);
		iscInstrument2.Gradient.Modify(1, -100.0, (ValueState)1);
		iscInstrument2.Gradient.Modify(2, 99.5, (ValueState)3);
		((SingleInstrumentBase)iscInstrument2).Instrument = new Qualifier((QualifierTypes)1, "MCM02T", "DT_Idle_Speed_Balance_Values_Cylinder_2");
		((Control)(object)iscInstrument2).Name = "iscInstrument2";
		((SingleInstrumentBase)iscInstrument2).TitlePosition = (LabelPosition)0;
		((SingleInstrumentBase)iscInstrument2).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(panelPictures, "panelPictures");
		panelPictures.Controls.Add(pictureBoxHDEP);
		panelPictures.Name = "panelPictures";
		componentResourceManager.ApplyResources(pictureBoxHDEP, "pictureBoxHDEP");
		pictureBoxHDEP.Name = "pictureBoxHDEP";
		pictureBoxHDEP.TabStop = false;
		componentResourceManager.ApplyResources(minLabel, "minLabel");
		minLabel.Name = "minLabel";
		minLabel.UseCompatibleTextRendering = true;
		((TableLayoutPanel)(object)mainLayoutPanel).SetColumnSpan((Control)textboxResults, 4);
		componentResourceManager.ApplyResources(textboxResults, "textboxResults");
		textboxResults.Name = "textboxResults";
		textboxResults.ReadOnly = true;
		((TableLayoutPanel)(object)mainLayoutPanel).SetRowSpan((Control)textboxResults, 2);
		((TableLayoutPanel)(object)mainLayoutPanel).SetColumnSpan((Control)labelEntryTips, 6);
		componentResourceManager.ApplyResources(labelEntryTips, "labelEntryTips");
		labelEntryTips.Name = "labelEntryTips";
		((TableLayoutPanel)(object)mainLayoutPanel).SetRowSpan((Control)labelEntryTips, 2);
		labelEntryTips.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(label1, "label1");
		label1.Name = "label1";
		label1.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(label6, "label6");
		label6.Name = "label6";
		label6.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(label5, "label5");
		label5.Name = "label5";
		label5.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(label4, "label4");
		label4.Name = "label4";
		label4.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(label3, "label3");
		label3.Name = "label3";
		label3.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(label2, "label2");
		label2.Name = "label2";
		label2.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(label20, "label20");
		label20.Name = "label20";
		label20.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(iscLabel, "iscLabel");
		((TableLayoutPanel)(object)mainLayoutPanel).SetColumnSpan((Control)iscLabel, 2);
		iscLabel.Name = "iscLabel";
		iscLabel.UseCompatibleTextRendering = true;
		buttonClose.DialogResult = DialogResult.OK;
		componentResourceManager.ApplyResources(buttonClose, "buttonClose");
		buttonClose.Name = "buttonClose";
		buttonClose.UseCompatibleTextRendering = true;
		buttonClose.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(flowLayoutPanel1, "flowLayoutPanel1");
		((TableLayoutPanel)(object)mainLayoutPanel).SetColumnSpan((Control)flowLayoutPanel1, 6);
		flowLayoutPanel1.Controls.Add(buttonRead);
		flowLayoutPanel1.Controls.Add(buttonWrite);
		flowLayoutPanel1.Controls.Add(buttonReset);
		flowLayoutPanel1.Controls.Add(buttonPrint);
		flowLayoutPanel1.Name = "flowLayoutPanel1";
		componentResourceManager.ApplyResources(buttonRead, "buttonRead");
		buttonRead.Name = "buttonRead";
		buttonRead.UseCompatibleTextRendering = true;
		buttonRead.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(buttonWrite, "buttonWrite");
		buttonWrite.Name = "buttonWrite";
		buttonWrite.UseCompatibleTextRendering = true;
		buttonWrite.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(buttonReset, "buttonReset");
		buttonReset.Name = "buttonReset";
		buttonReset.UseCompatibleTextRendering = true;
		buttonReset.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(buttonPrint, "buttonPrint");
		buttonPrint.Name = "buttonPrint";
		buttonPrint.UseCompatibleTextRendering = true;
		buttonPrint.UseVisualStyleBackColor = true;
		((TableLayoutPanel)(object)mainLayoutPanel).SetColumnSpan((Control)(object)digitalReadoutInstrumentEngineState, 3);
		digitalReadoutInstrumentEngineState.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentEngineState).FreezeValue = false;
		digitalReadoutInstrumentEngineState.Gradient.Initialize((ValueState)3, 8);
		digitalReadoutInstrumentEngineState.Gradient.Modify(1, -1.0, (ValueState)3);
		digitalReadoutInstrumentEngineState.Gradient.Modify(2, 0.0, (ValueState)1);
		digitalReadoutInstrumentEngineState.Gradient.Modify(3, 1.0, (ValueState)3);
		digitalReadoutInstrumentEngineState.Gradient.Modify(4, 2.0, (ValueState)3);
		digitalReadoutInstrumentEngineState.Gradient.Modify(5, 3.0, (ValueState)3);
		digitalReadoutInstrumentEngineState.Gradient.Modify(6, 4.0, (ValueState)3);
		digitalReadoutInstrumentEngineState.Gradient.Modify(7, 5.0, (ValueState)3);
		digitalReadoutInstrumentEngineState.Gradient.Modify(8, 6.0, (ValueState)3);
		componentResourceManager.ApplyResources(digitalReadoutInstrumentEngineState, "digitalReadoutInstrumentEngineState");
		((SingleInstrumentBase)digitalReadoutInstrumentEngineState).Instrument = new Qualifier((QualifierTypes)1, "MCM02T", "DT_AS023_Engine_State");
		((Control)(object)digitalReadoutInstrumentEngineState).Name = "digitalReadoutInstrumentEngineState";
		((SingleInstrumentBase)digitalReadoutInstrumentEngineState).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)mainLayoutPanel).SetColumnSpan((Control)(object)digitalReadoutLeakLearnedValue, 3);
		digitalReadoutLeakLearnedValue.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutLeakLearnedValue).FreezeValue = false;
		componentResourceManager.ApplyResources(digitalReadoutLeakLearnedValue, "digitalReadoutLeakLearnedValue");
		((SingleInstrumentBase)digitalReadoutLeakLearnedValue).Instrument = new Qualifier((QualifierTypes)4, "MCM02T", "HP_Leak_Learned_Value");
		((Control)(object)digitalReadoutLeakLearnedValue).Name = "digitalReadoutLeakLearnedValue";
		((SingleInstrumentBase)digitalReadoutLeakLearnedValue).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("Panel_InjectorCodes");
		((Control)this).Controls.Add((Control)(object)mainLayoutPanel);
		((Control)this).Name = "UserPanel";
		((Control)(object)mainLayoutPanel).ResumeLayout(performLayout: false);
		((Control)(object)mainLayoutPanel).PerformLayout();
		((ISupportInitialize)pictureBoxExamplePartNumber).EndInit();
		panelPictures.ResumeLayout(performLayout: false);
		((ISupportInitialize)pictureBoxHDEP).EndInit();
		flowLayoutPanel1.ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
