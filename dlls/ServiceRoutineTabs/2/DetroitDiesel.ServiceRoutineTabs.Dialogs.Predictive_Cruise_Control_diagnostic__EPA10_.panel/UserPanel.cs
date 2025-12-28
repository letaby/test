using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Parameters;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Predictive_Cruise_Control_diagnostic__EPA10_.panel;

public class UserPanel : CustomPanel
{
	private const string PccEcuName = "J1939-17";

	private Channel cpc;

	private Parameter pccParameter;

	private TableLayoutPanel tableLayoutPanel1;

	private Checkmark checkmark1;

	private System.Windows.Forms.Label labelStatus;

	private TableLayoutPanel tableLayoutPanel2;

	private Label labelPccTitle;

	private ParameterEditor parameterEditor;

	private TableLayoutPanel tableLayoutPanel3;

	private Button button1;

	private ScalingLabel scalingLabelPcc;

	private PccPresenceStatus PccPresence
	{
		get
		{
			if (SapiManager.GlobalInstance.RollCallEnabled)
			{
				if ((double?)ChannelCollection.GetManager(Protocol.J1939).Load > 0.0)
				{
					return SapiManager.GlobalInstance.Sapi.Channels.Any((Channel c) => c.Online && c.Ecu.Name == "J1939-17") ? PccPresenceStatus.Detected : PccPresenceStatus.NotDetected;
				}
				return PccPresenceStatus.NoDataLink;
			}
			return PccPresenceStatus.NoRollCall;
		}
	}

	private CpcStatus CpcState
	{
		get
		{
			if (cpc != null && cpc.Online && pccParameter != null)
			{
				if (cpc.CommunicationsState != CommunicationsState.WriteParameters)
				{
					return pccParameter.HasBeenReadFromEcu ? CpcStatus.Ready : CpcStatus.ParameterNotRead;
				}
				return CpcStatus.WritingParameters;
			}
			return CpcStatus.OfflineOrNoParameter;
		}
	}

	private bool PccConfigured
	{
		get
		{
			if (pccParameter != null && pccParameter.HasBeenReadFromEcu)
			{
				Choice choice = pccParameter.OriginalValue as Choice;
				if (choice != null)
				{
					byte b = Convert.ToByte(choice.RawValue);
					return b == 2 || b == 3;
				}
			}
			return false;
		}
	}

	public UserPanel()
	{
		InitializeComponent();
	}

	public override void OnChannelsChanged()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		Qualifier instrument = ((SingleInstrumentBase)parameterEditor).Instrument;
		SetCPC(((CustomPanel)this).GetChannel(((Qualifier)(ref instrument)).Ecu));
		UpdateUserInterface();
	}

	private void UserPanel_ParentFormClosing(object sender, FormClosingEventArgs e)
	{
		if (!e.Cancel)
		{
			SetCPC(null);
		}
	}

	private void button1_Click(object sender, EventArgs e)
	{
		if (cpc != null)
		{
			cpc.Parameters.Write(synchronous: false);
		}
	}

	private void cpc_CommunicationsStateUpdateEvent(object sender, CommunicationsStateEventArgs e)
	{
		if (pccParameter != null && !pccParameter.HasBeenReadFromEcu && e.CommunicationsState == CommunicationsState.Online)
		{
			cpc.Parameters.ReadGroup(pccParameter.GroupQualifier, fromCache: true, synchronous: false);
		}
		UpdateUserInterface();
	}

	private void pccParameter_ParameterUpdateEvent(object sender, ResultEventArgs e)
	{
		UpdateSendButton();
	}

	private void Parameters_ParametersReadCompleteEvent(object sender, ResultEventArgs e)
	{
		UpdateUserInterface();
	}

	private void UpdateUserInterface()
	{
		PccPresenceStatus pccPresence = PccPresence;
		CpcStatus cpcState = CpcState;
		switch (pccPresence)
		{
		case PccPresenceStatus.NoRollCall:
			((Control)(object)scalingLabelPcc).Text = Resources.Message_UnableToDetect;
			labelStatus.Text = Resources.Message_ThePredictiveCruiseControlConfigurationCanOnlyBeValidatedWhenAutomaticConnectionToStandardSAEJ1939DevicesIsEnabledInToolsOptionsConnections;
			checkmark1.CheckState = CheckState.Indeterminate;
			break;
		case PccPresenceStatus.NoDataLink:
			((Control)(object)scalingLabelPcc).Text = Resources.Message_NoDataLink;
			labelStatus.Text = Resources.Message_ThePredictiveCruiseControlConfigurationCanOnlyBeValidatedWhenTheDiagnosticToolIsConnectedToAJ1939Vehicle;
			checkmark1.CheckState = CheckState.Indeterminate;
			break;
		default:
			((Control)(object)scalingLabelPcc).Text = ((pccPresence == PccPresenceStatus.Detected) ? Resources.Message_Detected0 : Resources.Message_NotDetected);
			switch (cpcState)
			{
			case CpcStatus.OfflineOrNoParameter:
				labelStatus.Text = Resources.Message_ThePredictiveCruiseControlConfigurationCanOnlyBeValidatedWhenTheCPCIsOnlineAndInAnApplicationMode;
				checkmark1.CheckState = CheckState.Indeterminate;
				break;
			case CpcStatus.ParameterNotRead:
				labelStatus.Text = Resources.Message_ThePredictiveCruiseControlConfigurationCanOnlyBeValidatedOnceParametersHaveBeenReadFromTheCPCPleaseWait;
				checkmark1.CheckState = CheckState.Indeterminate;
				break;
			case CpcStatus.WritingParameters:
				labelStatus.Text = Resources.Message_ThePredictiveCruiseControlConfigurationIsBeingWrittenToTheCPCPleaseWait;
				checkmark1.CheckState = CheckState.Indeterminate;
				break;
			case CpcStatus.Ready:
				if (pccPresence == PccPresenceStatus.Detected)
				{
					if (PccConfigured)
					{
						checkmark1.CheckState = CheckState.Checked;
						labelStatus.Text = Resources.Message_ThePredictiveCruiseControlDeviceIsDetectedOnTheDataLinkAndTheCPCAppearsToBeConfiguredToUseIt;
					}
					else
					{
						checkmark1.CheckState = CheckState.Unchecked;
						labelStatus.Text = Resources.Message_ThePredictiveCruiseControlDeviceIsDetectedOnTheDataLinkButTheCPCIsNotConfiguredToUseItUseTheDropDownToChangeTheCPCConfigurationThenPressTheSendButtonToCommit;
					}
				}
				else if (PccConfigured)
				{
					checkmark1.CheckState = CheckState.Unchecked;
					labelStatus.Text = Resources.Message_TheCPCIsConfiguredForPredictiveCruiseControlOperationButTheDeviceIsNotDetectedOnTheDataLinkCheckTheVehicleWiring;
				}
				else
				{
					checkmark1.CheckState = CheckState.Indeterminate;
					labelStatus.Text = Resources.Message_TheCPCIsNotConfiguredForPredictiveCruiseControlOperationAndTheDeviceIsNotDetectedOnTheDataLink;
				}
				break;
			}
			break;
		}
		((Control)(object)parameterEditor).Enabled = cpcState == CpcStatus.Ready;
		UpdateSendButton();
	}

	private void UpdateSendButton()
	{
		button1.Enabled = CpcState == CpcStatus.Ready && pccParameter.OriginalValue != pccParameter.Value;
	}

	private void SetCPC(Channel channel)
	{
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		if (cpc == channel)
		{
			return;
		}
		if (cpc != null)
		{
			cpc.Parameters.ParametersReadCompleteEvent -= Parameters_ParametersReadCompleteEvent;
			cpc.CommunicationsStateUpdateEvent -= cpc_CommunicationsStateUpdateEvent;
			if (pccParameter != null)
			{
				pccParameter.ParameterUpdateEvent -= pccParameter_ParameterUpdateEvent;
				pccParameter = null;
			}
		}
		cpc = channel;
		if (cpc == null)
		{
			return;
		}
		cpc.Parameters.ParametersReadCompleteEvent += Parameters_ParametersReadCompleteEvent;
		cpc.CommunicationsStateUpdateEvent += cpc_CommunicationsStateUpdateEvent;
		ParameterCollection parameters = cpc.Parameters;
		Qualifier instrument = ((SingleInstrumentBase)parameterEditor).Instrument;
		pccParameter = parameters[((Qualifier)(ref instrument)).Name];
		if (pccParameter != null)
		{
			pccParameter.ParameterUpdateEvent += pccParameter_ParameterUpdateEvent;
			if (!pccParameter.HasBeenReadFromEcu && cpc.CommunicationsState == CommunicationsState.Online)
			{
				cpc.Parameters.ReadGroup(pccParameter.GroupQualifier, fromCache: true, synchronous: false);
			}
		}
	}

	private void InitializeComponent()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Expected O, but got Unknown
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel1 = new TableLayoutPanel();
		tableLayoutPanel2 = new TableLayoutPanel();
		tableLayoutPanel3 = new TableLayoutPanel();
		parameterEditor = new ParameterEditor();
		button1 = new Button();
		labelPccTitle = new Label();
		scalingLabelPcc = new ScalingLabel();
		checkmark1 = new Checkmark();
		labelStatus = new System.Windows.Forms.Label();
		((Control)(object)tableLayoutPanel3).SuspendLayout();
		((Control)(object)tableLayoutPanel2).SuspendLayout();
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel3, "tableLayoutPanel3");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)tableLayoutPanel3, 2);
		((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add((Control)(object)parameterEditor, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel3).Controls.Add(button1, 1, 2);
		((Control)(object)tableLayoutPanel3).Name = "tableLayoutPanel3";
		((TableLayoutPanel)(object)tableLayoutPanel3).SetColumnSpan((Control)(object)parameterEditor, 2);
		componentResourceManager.ApplyResources(parameterEditor, "parameterEditor");
		((SingleInstrumentBase)parameterEditor).FreezeValue = false;
		((SingleInstrumentBase)parameterEditor).Instrument = new Qualifier((QualifierTypes)4, "CPC02T", "CC_Mode_Selection");
		((Control)(object)parameterEditor).Name = "parameterEditor";
		((TableLayoutPanel)(object)tableLayoutPanel3).SetRowSpan((Control)(object)parameterEditor, 2);
		((SingleInstrumentBase)parameterEditor).ShowUnits = false;
		((SingleInstrumentBase)parameterEditor).TitleLengthPercentOfControl = 30;
		parameterEditor.TitleWordWrap = true;
		parameterEditor.UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(button1, "button1");
		button1.Name = "button1";
		button1.UseCompatibleTextRendering = true;
		button1.UseVisualStyleBackColor = true;
		button1.Click += button1_Click;
		componentResourceManager.ApplyResources(tableLayoutPanel2, "tableLayoutPanel2");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)tableLayoutPanel2, 2);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)labelPccTitle, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)scalingLabelPcc, 0, 1);
		((Control)(object)tableLayoutPanel2).Name = "tableLayoutPanel2";
		labelPccTitle.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(labelPccTitle, "labelPccTitle");
		((Control)(object)labelPccTitle).Name = "labelPccTitle";
		labelPccTitle.Orientation = (TextOrientation)1;
		scalingLabelPcc.Alignment = StringAlignment.Far;
		componentResourceManager.ApplyResources(scalingLabelPcc, "scalingLabelPcc");
		scalingLabelPcc.FontGroup = "";
		scalingLabelPcc.LineAlignment = StringAlignment.Center;
		((Control)(object)scalingLabelPcc).Name = "scalingLabelPcc";
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)checkmark1, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(labelStatus, 1, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)tableLayoutPanel2, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)tableLayoutPanel3, 0, 1);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		componentResourceManager.ApplyResources(checkmark1, "checkmark1");
		((Control)(object)checkmark1).Name = "checkmark1";
		componentResourceManager.ApplyResources(labelStatus, "labelStatus");
		labelStatus.Name = "labelStatus";
		labelStatus.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("Panel_PredictiveCruiseControlDiagnostic");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Name = "UserPanel";
		((CustomPanel)this).ParentFormClosing += UserPanel_ParentFormClosing;
		((Control)(object)tableLayoutPanel3).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel2).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
