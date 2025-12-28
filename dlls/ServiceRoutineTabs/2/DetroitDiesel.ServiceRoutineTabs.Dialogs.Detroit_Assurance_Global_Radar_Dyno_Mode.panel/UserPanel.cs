using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Help;
using DetroitDiesel.Utilities;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Detroit_Assurance_Global_Radar_Dyno_Mode.panel;

public class UserPanel : CustomPanel
{
	private readonly string GlobalRadarStatusQualifier = "RT_ABA_release_Request_Results_release_state";

	private readonly string DisplayMessageToEnable = Resources.Message_TheDetroitAssuranceAdvancedBrakeAssistFunctionIsCurrentlyDisabledToEnableDetroitAssuranceOnThisVehicleClickTheStartButton;

	private readonly string DisplayMessageToDisable = Resources.Message_TheDetroitAssuranceAdvancedBrakeAssistFunctionIsCurrentlyEnabledToDisableDetroitAssuranceOnThisVehicleClickTheStartButton;

	private SubjectCollection DisableGlobalRadar = new SubjectCollection((IEnumerable<string>)new string[1] { "SP_GlobalRadarDisable" });

	private SubjectCollection EnableGlobalRadar = new SubjectCollection((IEnumerable<string>)new string[1] { "SP_GlobalRadarEnable" });

	private SharedProcedureBase selectedProcedure;

	private Channel ecu;

	private TableLayoutPanel tableLayoutPanel1;

	private DigitalReadoutInstrument digitalReadoutInstrument1;

	private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponent1;

	private Checkmark checkmark1;

	private Button button1;

	private SharedProcedureSelection sharedProcedureSelection1;

	private System.Windows.Forms.Label label1;

	private System.Windows.Forms.Label DisplayMessageLabel;

	private SeekTimeListView seekTimeListView1;

	private bool CanClose => !sharedProcedureSelection1.AnyProcedureInProgress;

	private bool Online => ecu != null && ecu.CommunicationsState == CommunicationsState.Online;

	private Service GetStatusService
	{
		get
		{
			if (ecu == null)
			{
				return null;
			}
			return ecu.Services[GlobalRadarStatusQualifier];
		}
	}

	public UserPanel()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected O, but got Unknown
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Expected O, but got Unknown
		InitializeComponent();
	}

	protected override void OnLoad(EventArgs e)
	{
		((UserControl)this).OnLoad(e);
		((ContainerControl)this).ParentForm.FormClosing += OnFormClosing;
		SubscribeToEvents(sharedProcedureSelection1.SelectedProcedure);
		SetECU(((CustomPanel)this).GetChannel("VRDU01T"));
	}

	public override void OnChannelsChanged()
	{
		Channel channel = SapiManager.GlobalInstance.ActiveChannels.FirstOrDefault((Channel c) => c.Ecu.Name == "VRDU01T");
		if (ecu != channel)
		{
			SetECU(channel);
		}
	}

	private void OnFormClosing(object sender, FormClosingEventArgs e)
	{
		if (!CanClose)
		{
			e.Cancel = true;
		}
		if (!e.Cancel)
		{
			((ContainerControl)this).ParentForm.FormClosing -= OnFormClosing;
			if (selectedProcedure != null)
			{
				selectedProcedure.StopComplete -= OnStopComplete;
			}
			if (ecu != null)
			{
				ecu.CommunicationsStateUpdateEvent -= OnChannelStateUpdate;
			}
		}
	}

	private void OnStopComplete(object sender, PassFailResultEventArgs e)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Invalid comparison between Unknown and I4
		if ((int)e.Result == 1)
		{
			RequestGlobalRadarStatus();
		}
	}

	private void OnCheckStatusComplete(object sender, ResultEventArgs e)
	{
		Service service = (Service)sender;
		service.ServiceCompleteEvent -= OnCheckStatusComplete;
		if (service.OutputValues.Count <= 0)
		{
			return;
		}
		Choice choice = service.OutputValues[0].Value as Choice;
		byte b = 3;
		if (choice != null)
		{
			b = Convert.ToByte(choice.RawValue);
		}
		else
		{
			choice = service.OutputValues[0].Choices.GetItemFromRawValue(b);
		}
		if (choice != null)
		{
			switch (b)
			{
			case 1:
				sharedProcedureSelection1.SharedProcedureQualifiers = DisableGlobalRadar;
				break;
			case 2:
				sharedProcedureSelection1.SharedProcedureQualifiers = EnableGlobalRadar;
				break;
			}
			SubscribeToEvents(sharedProcedureSelection1.SelectedProcedure);
		}
	}

	private void OnChannelStateUpdate(object sender, CommunicationsStateEventArgs e)
	{
		Channel channel = (Channel)sender;
		if (channel.Ecu.Name.Equals("VRDU01T") && e.CommunicationsState == CommunicationsState.Online)
		{
			RequestGlobalRadarStatus();
			ecu.CommunicationsStateUpdateEvent -= OnChannelStateUpdate;
		}
	}

	private void SetECU(Channel ecu)
	{
		if (this.ecu == ecu)
		{
			return;
		}
		this.ecu = ecu;
		if (this.ecu != null)
		{
			if (this.ecu.CommunicationsState == CommunicationsState.Online)
			{
				RequestGlobalRadarStatus();
			}
			else
			{
				this.ecu.CommunicationsStateUpdateEvent += OnChannelStateUpdate;
			}
		}
	}

	private void SubscribeToEvents(SharedProcedureBase procedure)
	{
		if (procedure != selectedProcedure)
		{
			if (selectedProcedure != null)
			{
				selectedProcedure.StopComplete -= OnStopComplete;
			}
			selectedProcedure = procedure;
			if (selectedProcedure != null)
			{
				selectedProcedure.StopComplete += OnStopComplete;
			}
		}
	}

	private void RequestGlobalRadarStatus()
	{
		Service getStatusService = GetStatusService;
		if (getStatusService != null && Online)
		{
			getStatusService.ServiceCompleteEvent += OnCheckStatusComplete;
			getStatusService.Execute(synchronous: false);
		}
	}

	private void digitalReadoutInstrument1_RepresentedStateChanged(object sender, EventArgs e)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Invalid comparison between Unknown and I4
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Invalid comparison between Unknown and I4
		if ((int)digitalReadoutInstrument1.RepresentedState == 1)
		{
			DisplayMessageLabel.Text = DisplayMessageToDisable;
		}
		else if ((int)digitalReadoutInstrument1.RepresentedState == 2)
		{
			DisplayMessageLabel.Text = DisplayMessageToEnable;
		}
	}

	private void InitializeComponent()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Expected O, but got Unknown
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Expected O, but got Unknown
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_0467: Unknown result type (might be due to invalid IL or missing references)
		//IL_0471: Expected O, but got Unknown
		//IL_055a: Unknown result type (might be due to invalid IL or missing references)
		base.components = new Container();
		tableLayoutPanel1 = new TableLayoutPanel();
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		checkmark1 = new Checkmark();
		digitalReadoutInstrument1 = new DigitalReadoutInstrument();
		seekTimeListView1 = new SeekTimeListView();
		button1 = new Button();
		sharedProcedureSelection1 = new SharedProcedureSelection();
		label1 = new System.Windows.Forms.Label();
		DisplayMessageLabel = new System.Windows.Forms.Label();
		sharedProcedureIntegrationComponent1 = new SharedProcedureIntegrationComponent(base.components);
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)checkmark1, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument1, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)seekTimeListView1, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(button1, 3, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)sharedProcedureSelection1, 2, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(label1, 1, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(DisplayMessageLabel, 0, 2);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		componentResourceManager.ApplyResources(checkmark1, "checkmark1");
		((Control)(object)checkmark1).Name = "checkmark1";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)digitalReadoutInstrument1, 4);
		componentResourceManager.ApplyResources(digitalReadoutInstrument1, "digitalReadoutInstrument1");
		digitalReadoutInstrument1.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument1).FreezeValue = false;
		digitalReadoutInstrument1.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText"));
		digitalReadoutInstrument1.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText1"));
		digitalReadoutInstrument1.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText2"));
		digitalReadoutInstrument1.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText3"));
		digitalReadoutInstrument1.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText4"));
		digitalReadoutInstrument1.Gradient.Initialize((ValueState)0, 4);
		digitalReadoutInstrument1.Gradient.Modify(1, 0.0, (ValueState)3);
		digitalReadoutInstrument1.Gradient.Modify(2, 1.0, (ValueState)1);
		digitalReadoutInstrument1.Gradient.Modify(3, 2.0, (ValueState)2);
		digitalReadoutInstrument1.Gradient.Modify(4, 255.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes)64, "VRDU01T", "RT_ABA_release_Request_Results_release_state");
		((Control)(object)digitalReadoutInstrument1).Name = "digitalReadoutInstrument1";
		((SingleInstrumentBase)digitalReadoutInstrument1).ShowValueReadout = false;
		((SingleInstrumentBase)digitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
		digitalReadoutInstrument1.RepresentedStateChanged += digitalReadoutInstrument1_RepresentedStateChanged;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)seekTimeListView1, 4);
		componentResourceManager.ApplyResources(seekTimeListView1, "seekTimeListView1");
		seekTimeListView1.FilterUserLabels = true;
		((Control)(object)seekTimeListView1).Name = "seekTimeListView1";
		seekTimeListView1.RequiredUserLabelPrefix = "Global Radar";
		seekTimeListView1.SelectedTime = null;
		seekTimeListView1.ShowChannelLabels = false;
		seekTimeListView1.ShowCommunicationsState = false;
		seekTimeListView1.ShowControlPanel = false;
		seekTimeListView1.ShowDeviceColumn = false;
		componentResourceManager.ApplyResources(button1, "button1");
		button1.Name = "button1";
		button1.UseCompatibleTextRendering = true;
		button1.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(sharedProcedureSelection1, "sharedProcedureSelection1");
		((Control)(object)sharedProcedureSelection1).Name = "sharedProcedureSelection1";
		sharedProcedureSelection1.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>)new string[1] { "SP_GlobalRadarDisable" });
		componentResourceManager.ApplyResources(label1, "label1");
		label1.Name = "label1";
		label1.UseCompatibleTextRendering = true;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)DisplayMessageLabel, 4);
		componentResourceManager.ApplyResources(DisplayMessageLabel, "DisplayMessageLabel");
		DisplayMessageLabel.Name = "DisplayMessageLabel";
		DisplayMessageLabel.UseCompatibleTextRendering = true;
		sharedProcedureIntegrationComponent1.ProceduresDropDown = sharedProcedureSelection1;
		sharedProcedureIntegrationComponent1.ProcedureStatusMessageTarget = label1;
		sharedProcedureIntegrationComponent1.ProcedureStatusStateTarget = checkmark1;
		sharedProcedureIntegrationComponent1.ResultsTarget = null;
		sharedProcedureIntegrationComponent1.StartStopButton = button1;
		sharedProcedureIntegrationComponent1.StopAllButton = null;
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("_DDDL.chm_Active_Brake_Assist_-_Enable_Disable");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
