using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Extensions.FakeInstruments;
using DetroitDiesel.UnitConversion;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Set_Odometers__NGC_.panel;

public class UserPanel : CustomPanel
{
	private enum OdometerSetState
	{
		NotSet,
		Set,
		Working,
		Error
	}

	private enum Step
	{
		Ready = 0,
		Warning = 1,
		SetCpc = 2,
		SetInstrumentCluster = 3,
		Done = 4,
		Error = 255
	}

	private enum Operation
	{
		SetBoth,
		SyncToCpc,
		SyncToInstrumentCluster
	}

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

	private Conversion instrumentClusterValueToDisplay = null;

	private Conversion instrumentClusterDisplayToValue = null;

	private double instrumentClusterToSupportedConversionFactor = 1.0;

	private double instrumentClusterFromSupportedConversionFactor = 1.0;

	private string instrumentClusterSupportedConversionUnit = "km";

	private Channel cpc;

	private Conversion cpcValueToDisplay = null;

	private Conversion cpcDisplayToValue = null;

	private Service cpcOdometerService = null;

	private Channel j1939Cpc;

	private Instrument j1939CpcOdometerInstrument;

	private Conversion j1939CpcValueToDisplay = null;

	private Conversion j1939CpcDisplayToValue = null;

	private Conversion kmToDisplay;

	private Tuple<string, bool>[] ProhibitedVersions = new Tuple<string, bool>[2]
	{
		Tuple.Create("27.03.01", item2: true),
		Tuple.Create("26.83.54", item2: true)
	};

	private RuntimeFakeInstrument fakeIcOdometer;

	private string proposedNewValue = "";

	private OdometerSetState odometerSetState = OdometerSetState.NotSet;

	private Step currentStep = Step.Ready;

	private Operation currentOperation = Operation.SetBoth;

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

	private double? J1939CpcOdometerValue
	{
		get
		{
			if (j1939CpcOdometerInstrument != null && j1939CpcOdometerInstrument.InstrumentValues.Current != null)
			{
				double? num = double.Parse(j1939CpcOdometerInstrument.InstrumentValues.Current.Value.ToString(), CultureInfo.CurrentCulture);
				return (!num.HasValue) ? ((double?)null) : new double?(j1939CpcValueToDisplay.Convert((object)num));
			}
			return null;
		}
	}

	private double? CpcOdometerValue => CpcOnline ? J1939CpcOdometerValue : ((double?)null);

	private double? InstrumentClusterOdometerValue
	{
		get
		{
			if (InstrumentClusterOnline && instrumentClusterOdometerParameter != null && instrumentClusterOdometerParameter.Value != null)
			{
				double? num = double.Parse(instrumentClusterOdometerParameter.Value.ToString(), CultureInfo.CurrentCulture);
				return (!num.HasValue) ? ((double?)null) : new double?(instrumentClusterValueToDisplay.Convert((object)(num * instrumentClusterToSupportedConversionFactor)));
			}
			return null;
		}
	}

	private bool CpcOnline => cpc != null && cpc.Online && j1939Cpc != null && j1939Cpc.Online;

	private bool InstrumentClusterOnline => instrumentCluster != null && instrumentCluster.Online;

	private bool CpcEcuIsBusy => CpcOnline && cpc.CommunicationsState != CommunicationsState.Online && j1939Cpc.CommunicationsState != CommunicationsState.Online;

	private bool IcucEcuIsBusy => InstrumentClusterOnline && instrumentCluster.CommunicationsState != CommunicationsState.Online;

	private bool CanClose => odometerSetState != OdometerSetState.Working || (cpc == null && instrumentCluster == null);

	private void AppendDisplayMessage(string txt)
	{
		((CustomPanel)this).LabelLog(seekTimeListView.RequiredUserLabelPrefix, txt);
		((CustomPanel)this).AddStatusMessage(txt);
	}

	public UserPanel()
	{
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Invalid comparison between Unknown and I4
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Invalid comparison between Unknown and I4
		CreateFakeIcOdometer(((int)Converter.GlobalInstance.SelectedUnitSystem == 1) ? "miles" : "km");
		kmToDisplay = Converter.GlobalInstance.GetConversion("km", ((int)Converter.GlobalInstance.SelectedUnitSystem == 1) ? "miles" : "km");
		InitializeComponent();
	}

	protected override void OnLoad(EventArgs e)
	{
		((ContainerControl)this).ParentForm.FormClosing += OnFormClosing;
		textBoxCustomValue.TextChanged += textBoxCustomValueTextChanged;
		textBoxCustomValue.KeyPress += textBoxCustomValueKeyPress;
		((UserControl)this).OnLoad(e);
		UpdateChannels();
		if (CheckProhibited())
		{
			((ContainerControl)this).ParentForm.Close();
		}
		UpdateUserInterface();
	}

	private void OnFormClosing(object sender, FormClosingEventArgs e)
	{
		if (!CanClose)
		{
			e.Cancel = true;
		}
		if (e.Cancel)
		{
			return;
		}
		DisposeFakeIcOdometer();
		if (instrumentCluster != null)
		{
			instrumentCluster.CommunicationsStateUpdateEvent -= OnCommunicationsStateUpdate;
			if (instrumentClusterOdometerParameter != null)
			{
				instrumentClusterOdometerParameter.ParameterUpdateEvent -= OdometerParameterUpdateEvent;
				instrumentClusterOdometerParameter = null;
			}
		}
		if (cpc != null)
		{
			cpc.CommunicationsStateUpdateEvent -= OnCommunicationsStateUpdate;
		}
		if (((ContainerControl)this).ParentForm != null)
		{
			((ContainerControl)this).ParentForm.FormClosing -= OnFormClosing;
		}
	}

	private bool CheckProhibited()
	{
		if (cpc != null && cpc.EcuInfos["CO_SoftwareVersion"] != null && cpc.Ecu.Name.Equals("CPC302T", StringComparison.OrdinalIgnoreCase))
		{
			string value = cpc.EcuInfos["CO_SoftwareVersion"].Value;
			Tuple<string, bool>[] prohibitedVersions = ProhibitedVersions;
			foreach (Tuple<string, bool> tuple in prohibitedVersions)
			{
				if (string.Compare(value, tuple.Item1, StringComparison.OrdinalIgnoreCase) == 0)
				{
					if (tuple.Item2)
					{
						MessageBox.Show(string.Format(Resources.MessageFormat_UnsupportedSoftwareVersion, cpc.Ecu.Name), Resources.Message_UnsupportedSoftwareVersionTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
					}
					else
					{
						MessageBox.Show(string.Format(Resources.MessageFormat_UnsupportedSoftwareVersionNoRepair, cpc.Ecu.Name), Resources.Message_UnsupportedSoftwareVersionTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
					}
					return true;
				}
			}
		}
		return false;
	}

	private void GoMachine()
	{
		switch (currentStep)
		{
		case Step.Warning:
			if (cpc != null && cpcOdometerService != null && currentOperation != Operation.SyncToCpc)
			{
				if (DialogResult.OK == MessageBox.Show(Resources.Message_DDECReportsLifeToDateDataWillBeResetClickOkToContinueOrClickCancelToAbortChangesAndExit, Application.ProductName, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2))
				{
					currentStep = Step.SetCpc;
				}
				else
				{
					currentStep = Step.Done;
				}
			}
			else
			{
				currentStep = Step.SetInstrumentCluster;
			}
			GoMachine();
			break;
		case Step.SetCpc:
			currentStep++;
			if (currentOperation != Operation.SyncToCpc)
			{
				AppendDisplayMessage(Resources.Message_SettingCPC);
				SetCpcOdometerEcuInfoValue();
			}
			else
			{
				GoMachine();
			}
			break;
		case Step.SetInstrumentCluster:
			currentStep++;
			if (instrumentCluster != null && currentOperation != Operation.SyncToInstrumentCluster)
			{
				AppendDisplayMessage(Resources.Message_SettingInstrumentCluster);
				SetInstrumentClusterOdometerParameter();
			}
			else
			{
				GoMachine();
			}
			break;
		case Step.Done:
			AppendDisplayMessage(Resources.Message_Done);
			odometerSetState = OdometerSetState.Set;
			break;
		case Step.Error:
			odometerSetState = OdometerSetState.Error;
			AppendDisplayMessage(Resources.Message_Error);
			AppendDisplayMessage(errorString);
			break;
		}
		UpdateUserInterface();
	}

	private void SetCpcOdometerEcuInfoValue()
	{
		if (cpcOdometerService != null)
		{
			odometerSetState = OdometerSetState.Working;
			if (cpcDisplayToValue != null)
			{
				cpcOdometerService.InputValues[0].Value = cpcDisplayToValue.Convert((object)proposedNewValue);
			}
			else
			{
				cpcOdometerService.InputValues[0].Value = proposedNewValue;
			}
			cpcOdometerService.ServiceCompleteEvent += SetCpcOdometerServiceCompleteEvent;
			cpcOdometerService.Execute(synchronous: false);
		}
		else
		{
			currentStep = Step.Error;
			errorString = Resources.Message_CPCServiceNA;
			GoMachine();
		}
	}

	private void SetCpcOdometerServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		Service service = sender as Service;
		service.ServiceCompleteEvent -= SetCpcOdometerServiceCompleteEvent;
		if (!e.Succeeded)
		{
			currentStep = Step.Error;
			errorString = e.Exception.Message;
		}
		GoMachine();
	}

	private void SetInstrumentClusterOdometerParameter()
	{
		if (instrumentClusterOdometerParameter != null)
		{
			odometerSetState = OdometerSetState.Working;
			instrumentClusterOdometerParameter.Value = instrumentClusterDisplayToValue.Convert((object)proposedNewValue) * instrumentClusterFromSupportedConversionFactor;
			instrumentCluster.Parameters.ParametersWriteCompleteEvent += InstrumentClusterOdometerWriteCompleteEvent;
			UpdateUserInterface();
			instrumentCluster.Parameters.Write(synchronous: false);
		}
	}

	private void InstrumentClusterOdometerWriteCompleteEvent(object sender, ResultEventArgs e)
	{
		if (instrumentCluster != null)
		{
			instrumentCluster.Parameters.ParametersWriteCompleteEvent -= InstrumentClusterOdometerWriteCompleteEvent;
		}
		if (!e.Succeeded)
		{
			odometerSetState = OdometerSetState.Error;
			errorString = e.Exception.Message;
		}
		GoMachine();
	}

	public override void OnChannelsChanged()
	{
		UpdateChannels();
	}

	private string DetermineInputUnits()
	{
		return (j1939CpcValueToDisplay != null) ? j1939CpcValueToDisplay.OutputUnit : ((instrumentClusterValueToDisplay != null) ? instrumentClusterValueToDisplay.OutputUnit : "km");
	}

	private void UpdateChannels()
	{
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0451: Unknown result type (might be due to invalid IL or missing references)
		string arg = DetermineInputUnits();
		Channel channel = ((CustomPanel)this).GetChannel("CPC302T", (ChannelLookupOptions)5);
		if (channel == null)
		{
			channel = ((CustomPanel)this).GetChannel("CPC501T", (ChannelLookupOptions)5);
		}
		if (cpc != channel)
		{
			if (cpc != null)
			{
				cpc.CommunicationsStateUpdateEvent -= OnCommunicationsStateUpdate;
				cpcDisplayToValue = null;
			}
			cpc = channel;
			if (cpc != null)
			{
				cpc.CommunicationsStateUpdateEvent += OnCommunicationsStateUpdate;
				cpcOdometerService = channel.Services["DL_ID_Odometer"];
				if (cpcOdometerService != null && cpcOdometerService.InputValues[0] != null)
				{
					cpcValueToDisplay = Converter.GlobalInstance.GetConversion(cpcOdometerService.InputValues[0].Units.ToString(), Converter.GlobalInstance.SelectedUnitSystem);
					cpcDisplayToValue = ((cpcValueToDisplay != null) ? Converter.GlobalInstance.GetConversion(cpcValueToDisplay.OutputUnit, cpcOdometerService.InputValues[0].Units.ToString()) : null);
					PopulatetextBoxCustomValueText(LargestOdometer());
				}
			}
		}
		Channel channel2 = ((CustomPanel)this).GetChannel("J1939-0");
		if (j1939Cpc != channel2)
		{
			if (j1939Cpc != null)
			{
				j1939Cpc.CommunicationsStateUpdateEvent -= OnCommunicationsStateUpdate;
			}
			j1939Cpc = channel2;
			if (j1939Cpc != null)
			{
				j1939CpcOdometerInstrument = channel2.Instruments["DT_917"];
				if (j1939CpcOdometerInstrument != null)
				{
					j1939CpcValueToDisplay = Converter.GlobalInstance.GetConversion(j1939CpcOdometerInstrument.Units.ToString(), Converter.GlobalInstance.SelectedUnitSystem);
					j1939CpcDisplayToValue = Converter.GlobalInstance.GetConversion(j1939CpcValueToDisplay.OutputUnit, j1939CpcOdometerInstrument.Units.ToString());
				}
				arg = DetermineInputUnits();
				PopulatetextBoxCustomValueText(LargestOdometer());
			}
		}
		Channel channel3 = ((CustomPanel)this).GetChannel("ICUC01T", (ChannelLookupOptions)5);
		if (instrumentCluster != channel3)
		{
			if (instrumentCluster != null)
			{
				instrumentCluster.CommunicationsStateUpdateEvent -= OnCommunicationsStateUpdate;
				if (instrumentClusterOdometerParameter != null)
				{
					instrumentClusterOdometerParameter.ParameterUpdateEvent -= OdometerParameterUpdateEvent;
					instrumentClusterOdometerParameter = null;
				}
			}
			instrumentCluster = channel3;
			if (channel3 != null && channel3.CommunicationsState == CommunicationsState.Online && (channel3.Ecu.Name.Equals("ICUC01T", StringComparison.OrdinalIgnoreCase) || channel3.Ecu.Name.Equals("ICC501T", StringComparison.OrdinalIgnoreCase)))
			{
				instrumentCluster.CommunicationsStateUpdateEvent += OnCommunicationsStateUpdate;
				string qualifier = (channel3.Ecu.Name.Equals("ICC501T", StringComparison.OrdinalIgnoreCase) ? "ET_ODO_Odometer" : "paramStoredOdometer");
				instrumentClusterOdometerParameter = instrumentCluster.Parameters[qualifier];
				if (instrumentClusterOdometerParameter != null)
				{
					if (instrumentClusterOdometerParameter.Units == "m")
					{
						instrumentClusterToSupportedConversionFactor = 0.001;
						instrumentClusterFromSupportedConversionFactor = 1000.0;
						instrumentClusterSupportedConversionUnit = "km";
					}
					else
					{
						instrumentClusterToSupportedConversionFactor = 1.0;
						instrumentClusterFromSupportedConversionFactor = 1.0;
						instrumentClusterSupportedConversionUnit = instrumentClusterOdometerParameter.Units;
					}
					instrumentCluster.Parameters.ReadGroup(instrumentClusterOdometerParameter.GroupQualifier, fromCache: false, synchronous: true);
					instrumentClusterValueToDisplay = Converter.GlobalInstance.GetConversion(instrumentClusterSupportedConversionUnit, Converter.GlobalInstance.SelectedUnitSystem);
					instrumentClusterDisplayToValue = Converter.GlobalInstance.GetConversion(instrumentClusterValueToDisplay.OutputUnit, instrumentClusterSupportedConversionUnit);
					instrumentClusterOdometerParameter.ParameterUpdateEvent += OdometerParameterUpdateEvent;
					SetFakeIcOdometer(InstrumentClusterOdometerValue);
					arg = DetermineInputUnits();
					PopulatetextBoxCustomValueText(LargestOdometer());
				}
			}
			UpdateUserInterface();
		}
		labelCustomValue.Text = $"Custom Value({arg}):";
		if (cpc == null && instrumentCluster == null)
		{
			((ContainerControl)this).ParentForm.Close();
		}
	}

	private void CreateFakeIcOdometer(string units)
	{
		if (fakeIcOdometer == null)
		{
			fakeIcOdometer = RuntimeFakeInstrument.Create("fakeIcOdometer", Resources.Message_InstrumentClusterOdometer, units);
		}
	}

	private void SetFakeIcOdometer(double? v)
	{
		if (fakeIcOdometer != null)
		{
			fakeIcOdometer.SetValue((object)(v.HasValue ? string.Format(CultureInfo.CurrentCulture, "{0:0.00}", InstrumentClusterOdometerValue) : null));
		}
	}

	private void DisposeFakeIcOdometer()
	{
		if (fakeIcOdometer != null)
		{
			((FakeInstrument)fakeIcOdometer).Dispose();
		}
	}

	private double LargestOdometer()
	{
		double num = ((!CpcOdometerValue.HasValue) ? 0.0 : CpcOdometerValue.Value);
		double num2 = ((!InstrumentClusterOdometerValue.HasValue) ? 0.0 : InstrumentClusterOdometerValue.Value);
		return (num2 > num) ? num2 : num;
	}

	private void OdometerParameterUpdateEvent(object sender, ResultEventArgs e)
	{
		SetFakeIcOdometer(InstrumentClusterOdometerValue);
		if (!InstrumentClusterOdometerValue.HasValue || (!OdometerValuesAreEqual(previousOdometerValue, InstrumentClusterOdometerValue.Value) && odometerSetState != OdometerSetState.Working))
		{
			previousOdometerValue = (InstrumentClusterOdometerValue.HasValue ? InstrumentClusterOdometerValue.Value : (-1.0));
			odometerSetState = OdometerSetState.NotSet;
		}
		UpdateUserInterface();
	}

	private void buttonSet_Click(object sender, EventArgs e)
	{
		proposedNewValue = textBoxCustomValue.Text;
		currentOperation = Operation.SetBoth;
		currentStep = Step.Warning;
		GoMachine();
	}

	private void OnCommunicationsStateUpdate(object sender, CommunicationsStateEventArgs e)
	{
		UpdateUserInterface();
	}

	private bool OdometerValuesAreEqual(double? v1, double? v2)
	{
		if (!v1.HasValue || !v2.HasValue)
		{
			return v1 == v2;
		}
		return Math.Abs(v1.Value - v2.Value) < 0.25;
	}

	private void UpdateUserInterface()
	{
		((Control)(object)tableLayoutPanelOdometerDisplay).SuspendLayout();
		((TableLayoutPanel)(object)tableLayoutPanelOdometerDisplay).ColumnStyles[2].Width = ((instrumentCluster == null) ? 0f : 50f);
		((TableLayoutPanel)(object)tableLayoutPanelOdometerDisplay).ColumnStyles[0].Width = ((cpc == null && instrumentCluster != null) ? 0f : 50f);
		((TableLayoutPanel)(object)tableLayoutPanelOdometerDisplay).ColumnStyles[1].Width = ((cpc != null && instrumentCluster != null) ? 4 : 0);
		((Control)(object)tableLayoutPanelOdometerDisplay).ResumeLayout();
		if ((odometerSetState == OdometerSetState.Set || odometerSetState == OdometerSetState.Error || odometerSetState == OdometerSetState.NotSet) && InstrumentClusterOdometerValue.HasValue && J1939CpcOdometerValue.HasValue && cpc != null && !OdometerValuesAreEqual(CpcOdometerValue, InstrumentClusterOdometerValue))
		{
			if (InstrumentClusterOdometerValue > CpcOdometerValue)
			{
				buttonSyncToInstrumentCluster.Visible = true;
				buttonSyncToCpc.Visible = false;
			}
			else
			{
				buttonSyncToInstrumentCluster.Visible = false;
				buttonSyncToCpc.Visible = true;
			}
		}
		else
		{
			buttonSyncToInstrumentCluster.Visible = false;
			buttonSyncToCpc.Visible = false;
		}
		switch (odometerSetState)
		{
		case OdometerSetState.Set:
			buttonSet.Enabled = false;
			checkmark1.Checked = !CpcEcuIsBusy || !IcucEcuIsBusy;
			labelCondition.Text = ((CpcEcuIsBusy || IcucEcuIsBusy) ? Resources.Message_WritingOdometerValue : Resources.Message_TheOdometerHasBeenSetTheValueShownMayBeSlightlyDifferentThanTheOneEnteredDueToRoundingIssues);
			labelCondition.BackColor = SystemColors.Control;
			break;
		case OdometerSetState.Working:
		{
			Checkmark obj = checkmark1;
			Button button = buttonSet;
			Button button2 = buttonSyncToCpc;
			bool flag = (buttonSyncToInstrumentCluster.Enabled = false);
			flag = (button2.Enabled = flag);
			flag = (button.Enabled = flag);
			obj.Checked = flag;
			labelCondition.Text = Resources.Message_WritingOdometerValue;
			labelCondition.BackColor = SystemColors.Control;
			break;
		}
		case OdometerSetState.Error:
			buttonSet.Enabled = false;
			checkmark1.Checked = false;
			labelCondition.Text = string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_AnErrorOccurredSettingTheOdometer0, errorString);
			break;
		case OdometerSetState.NotSet:
		{
			double? num = (string.IsNullOrEmpty(textBoxCustomValue.Text) ? ((double?)null) : new double?(double.Parse(textBoxCustomValue.Text, CultureInfo.CurrentCulture)));
			if (!InstrumentClusterOdometerValue.HasValue && !J1939CpcOdometerValue.HasValue)
			{
				SetFakeIcOdometer(null);
				buttonSet.Enabled = false;
				checkmark1.Checked = false;
				labelCondition.Text = Resources.Message_TheOdometerValueIsUnknownAndCannotBeSet;
			}
			else if (OdometerValuesAreEqual(num, LargestOdometer()))
			{
				buttonSet.Enabled = false;
				checkmark1.Checked = false;
				labelCondition.Text = Resources.Message_TheOdometerCannotBeSetBecauseTheProposedValueIsEffectivelyEqualToCurrentOdometer;
				labelCondition.BackColor = SystemColors.Control;
			}
			else if (num > LargestOdometer())
			{
				buttonSet.Enabled = true;
				checkmark1.Checked = true;
				labelCondition.Text = Resources.Message_TheOdometerIsReadyToBeSetBecauseTheProposedValueIsGreaterThanTheCurrentOdometer;
				labelCondition.BackColor = SystemColors.Control;
			}
			else
			{
				buttonSet.Enabled = false;
				checkmark1.Checked = false;
				labelCondition.Text = Resources.Message_ErrorTheOdometerCannotBeSetBecauseTheProposedValueIsLessThanTheCurrentOdometer;
				labelCondition.BackColor = Color.Red;
			}
			break;
		}
		default:
			throw new InvalidOperationException();
		}
	}

	private void PopulatetextBoxCustomValueText(double value)
	{
		textBoxCustomValue.Text = string.Format(CultureInfo.CurrentCulture, "{0:0.00}", value);
	}

	private void textBoxCustomValueTextChanged(object sender, EventArgs e)
	{
		odometerSetState = OdometerSetState.NotSet;
		UpdateUserInterface();
	}

	private void textBoxCustomValueKeyPress(object sender, KeyPressEventArgs e)
	{
		e.Handled = !char.IsDigit(e.KeyChar) && (textBoxCustomValue.Text.Length <= 0 || e.KeyChar != '.') && !char.IsControl(e.KeyChar);
	}

	private void buttonSyncToCpc_Click(object sender, EventArgs e)
	{
		proposedNewValue = J1939CpcOdometerValue.ToString();
		currentOperation = Operation.SyncToCpc;
		currentStep = Step.SetInstrumentCluster;
		GoMachine();
	}

	private void buttonSyncToInstrumentCluster_Click(object sender, EventArgs e)
	{
		proposedNewValue = InstrumentClusterOdometerValue.ToString();
		currentOperation = Operation.SyncToInstrumentCluster;
		currentStep = Step.Warning;
		GoMachine();
	}

	private void digitalReadoutInstrumentCpcOdometer_DataChanged(object sender, EventArgs e)
	{
		UpdateUserInterface();
	}

	private void InitializeComponent()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Expected O, but got Unknown
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Expected O, but got Unknown
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Expected O, but got Unknown
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Expected O, but got Unknown
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Expected O, but got Unknown
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Expected O, but got Unknown
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Expected O, but got Unknown
		//IL_04d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0596: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanelMain = new TableLayoutPanel();
		tableLayoutPanelStatus = new TableLayoutPanel();
		labelCondition = new System.Windows.Forms.Label();
		checkmark1 = new Checkmark();
		textBoxCustomValue = new TextBox();
		buttonSet = new Button();
		buttonClose = new Button();
		labelCustomValue = new System.Windows.Forms.Label();
		tableLayoutPanelOdometerDisplay = new TableLayoutPanel();
		tableLayoutPanel5 = new TableLayoutPanel();
		buttonSyncToInstrumentCluster = new Button();
		digitalReadoutInstrumentIcucOdometer = new DigitalReadoutInstrument();
		tableLayoutPanel4 = new TableLayoutPanel();
		digitalReadoutInstrumentCpcOdometer = new DigitalReadoutInstrument();
		buttonSyncToCpc = new Button();
		seekTimeListView = new SeekTimeListView();
		((Control)(object)tableLayoutPanelMain).SuspendLayout();
		((Control)(object)tableLayoutPanelStatus).SuspendLayout();
		((Control)(object)tableLayoutPanelOdometerDisplay).SuspendLayout();
		((Control)(object)tableLayoutPanel5).SuspendLayout();
		((Control)(object)tableLayoutPanel4).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanelMain, "tableLayoutPanelMain");
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)tableLayoutPanelStatus, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)tableLayoutPanelOdometerDisplay, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)seekTimeListView, 0, 1);
		((Control)(object)tableLayoutPanelMain).Name = "tableLayoutPanelMain";
		componentResourceManager.ApplyResources(tableLayoutPanelStatus, "tableLayoutPanelStatus");
		((TableLayoutPanel)(object)tableLayoutPanelStatus).Controls.Add(labelCondition, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanelStatus).Controls.Add((Control)(object)checkmark1, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelStatus).Controls.Add(textBoxCustomValue, 2, 1);
		((TableLayoutPanel)(object)tableLayoutPanelStatus).Controls.Add(buttonSet, 3, 1);
		((TableLayoutPanel)(object)tableLayoutPanelStatus).Controls.Add(buttonClose, 5, 1);
		((TableLayoutPanel)(object)tableLayoutPanelStatus).Controls.Add(labelCustomValue, 0, 1);
		((Control)(object)tableLayoutPanelStatus).Name = "tableLayoutPanelStatus";
		((TableLayoutPanel)(object)tableLayoutPanelStatus).SetColumnSpan((Control)labelCondition, 5);
		componentResourceManager.ApplyResources(labelCondition, "labelCondition");
		labelCondition.Name = "labelCondition";
		labelCondition.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(checkmark1, "checkmark1");
		((Control)(object)checkmark1).Name = "checkmark1";
		componentResourceManager.ApplyResources(textBoxCustomValue, "textBoxCustomValue");
		textBoxCustomValue.Name = "textBoxCustomValue";
		componentResourceManager.ApplyResources(buttonSet, "buttonSet");
		buttonSet.Name = "buttonSet";
		buttonSet.UseCompatibleTextRendering = true;
		buttonSet.UseVisualStyleBackColor = true;
		buttonSet.Click += buttonSet_Click;
		componentResourceManager.ApplyResources(buttonClose, "buttonClose");
		buttonClose.DialogResult = DialogResult.OK;
		buttonClose.Name = "buttonClose";
		buttonClose.UseCompatibleTextRendering = true;
		buttonClose.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(labelCustomValue, "labelCustomValue");
		((TableLayoutPanel)(object)tableLayoutPanelStatus).SetColumnSpan((Control)labelCustomValue, 2);
		labelCustomValue.Name = "labelCustomValue";
		labelCustomValue.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(tableLayoutPanelOdometerDisplay, "tableLayoutPanelOdometerDisplay");
		((TableLayoutPanel)(object)tableLayoutPanelOdometerDisplay).Controls.Add((Control)(object)tableLayoutPanel5, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanelOdometerDisplay).Controls.Add((Control)(object)tableLayoutPanel4, 0, 0);
		((Control)(object)tableLayoutPanelOdometerDisplay).Name = "tableLayoutPanelOdometerDisplay";
		componentResourceManager.ApplyResources(tableLayoutPanel5, "tableLayoutPanel5");
		((TableLayoutPanel)(object)tableLayoutPanel5).Controls.Add(buttonSyncToInstrumentCluster, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel5).Controls.Add((Control)(object)digitalReadoutInstrumentIcucOdometer, 0, 0);
		((Control)(object)tableLayoutPanel5).Name = "tableLayoutPanel5";
		componentResourceManager.ApplyResources(buttonSyncToInstrumentCluster, "buttonSyncToInstrumentCluster");
		buttonSyncToInstrumentCluster.Name = "buttonSyncToInstrumentCluster";
		buttonSyncToInstrumentCluster.UseCompatibleTextRendering = true;
		buttonSyncToInstrumentCluster.UseVisualStyleBackColor = true;
		buttonSyncToInstrumentCluster.Click += buttonSyncToInstrumentCluster_Click;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentIcucOdometer, "digitalReadoutInstrumentIcucOdometer");
		digitalReadoutInstrumentIcucOdometer.FontGroup = "SetOdometers";
		((SingleInstrumentBase)digitalReadoutInstrumentIcucOdometer).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentIcucOdometer).Instrument = new Qualifier((QualifierTypes)16, "fake", "fakeIcOdometer");
		((Control)(object)digitalReadoutInstrumentIcucOdometer).Name = "digitalReadoutInstrumentIcucOdometer";
		((SingleInstrumentBase)digitalReadoutInstrumentIcucOdometer).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(tableLayoutPanel4, "tableLayoutPanel4");
		((TableLayoutPanel)(object)tableLayoutPanel4).Controls.Add((Control)(object)digitalReadoutInstrumentCpcOdometer, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel4).Controls.Add(buttonSyncToCpc, 0, 1);
		((Control)(object)tableLayoutPanel4).Name = "tableLayoutPanel4";
		componentResourceManager.ApplyResources(digitalReadoutInstrumentCpcOdometer, "digitalReadoutInstrumentCpcOdometer");
		digitalReadoutInstrumentCpcOdometer.FontGroup = "SetOdometers";
		((SingleInstrumentBase)digitalReadoutInstrumentCpcOdometer).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentCpcOdometer).Instrument = new Qualifier((QualifierTypes)1, "J1939-0", "DT_917");
		((Control)(object)digitalReadoutInstrumentCpcOdometer).Name = "digitalReadoutInstrumentCpcOdometer";
		((SingleInstrumentBase)digitalReadoutInstrumentCpcOdometer).UnitAlignment = StringAlignment.Near;
		((SingleInstrumentBase)digitalReadoutInstrumentCpcOdometer).DataChanged += digitalReadoutInstrumentCpcOdometer_DataChanged;
		componentResourceManager.ApplyResources(buttonSyncToCpc, "buttonSyncToCpc");
		buttonSyncToCpc.Name = "buttonSyncToCpc";
		buttonSyncToCpc.UseCompatibleTextRendering = true;
		buttonSyncToCpc.UseVisualStyleBackColor = true;
		buttonSyncToCpc.Click += buttonSyncToCpc_Click;
		componentResourceManager.ApplyResources(seekTimeListView, "seekTimeListView");
		seekTimeListView.FilterUserLabels = true;
		((Control)(object)seekTimeListView).Name = "seekTimeListView";
		seekTimeListView.RequiredUserLabelPrefix = "SetOdometer";
		seekTimeListView.SelectedTime = null;
		seekTimeListView.ShowChannelLabels = false;
		seekTimeListView.ShowCommunicationsState = false;
		seekTimeListView.ShowControlPanel = false;
		seekTimeListView.ShowDeviceColumn = false;
		componentResourceManager.ApplyResources(this, "$this");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanelMain);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanelMain).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelMain).PerformLayout();
		((Control)(object)tableLayoutPanelStatus).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelStatus).PerformLayout();
		((Control)(object)tableLayoutPanelOdometerDisplay).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel5).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel5).PerformLayout();
		((Control)(object)tableLayoutPanel4).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel4).PerformLayout();
		((Control)this).ResumeLayout(performLayout: false);
	}
}
