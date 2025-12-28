using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Utilities;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.HV_Active_Discharge__EMG_.panel;

public class UserPanel : CustomPanel
{
	private SharedProcedureCreatorComponent sharedProcedureCreatorComponentHVActiveDischarge;

	private SharedProcedureSelection sharedProcedureSelection;

	private Label labelStatus;

	private Checkmark checkmarkStatus;

	private Button buttonStartStop;

	private TableLayoutPanel tableLayoutPanelMain;

	private TableLayoutPanel tableLayoutPanelStatusIndicators;

	private DigitalReadoutInstrument digitalReadoutInstrumentHVReady;

	private TableLayoutPanel tableLayoutPanelTop;

	private PictureBox pictureBoxWarningIcon;

	private WebBrowser webBrowserWarning;

	private DigitalReadoutInstrument digitalReadoutInstrumentHVActiveDischargeRequestResults;

	private DigitalReadoutInstrument digitalReadoutInstrumentGlobalHVIL;

	private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponentHVActiveDischarge;

	public UserPanel()
	{
		InitializeComponent();
		((CustomPanel)this).ParentFormClosing += this_ParentFormClosing;
	}

	protected override void OnLoad(EventArgs e)
	{
		((UserControl)this).OnLoad(e);
		string text = "html { height:100%; display: table; } ";
		text += "body { margin: 0px; padding: 0px; display: table-cell; vertical-align: middle; } ";
		text += ".scaled { font-size: calc(0.62vw + 8.0vh); font-family: Segoe UI; padding: 0px; margin: 0px; }  ";
		text += ".bold { font-weight: bold; }";
		text += ".red { color: red; }";
		string format = "<html><style>{0}</style><body><span class='scaled bold red'>{1}</span><span class='scaled bold'>{2}</span><br><span class='scaled'>{3}</span><span class='scaled bold'>{4}</span></body><span class='scaled'>).</span></html>";
		webBrowserWarning.DocumentText = string.Format(CultureInfo.InvariantCulture, format, text, Resources.RedWarning, Resources.BlackWarning, Resources.WarningText, Resources.ReferenceChecklist);
	}

	private void this_ParentFormClosing(object sender, FormClosingEventArgs e)
	{
		if (sharedProcedureSelection.AnyProcedureInProgress)
		{
			e.Cancel = true;
		}
		if (!e.Cancel)
		{
			((ContainerControl)this).ParentForm.FormClosing -= this_ParentFormClosing;
		}
	}

	private void InitializeComponent()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected O, but got Unknown
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Expected O, but got Unknown
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Expected O, but got Unknown
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Expected O, but got Unknown
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Expected O, but got Unknown
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Expected O, but got Unknown
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Expected O, but got Unknown
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Expected O, but got Unknown
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Expected O, but got Unknown
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Expected O, but got Unknown
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_037e: Expected O, but got Unknown
		//IL_07a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_082b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a09: Unknown result type (might be due to invalid IL or missing references)
		base.components = new Container();
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		DataItemCondition val = new DataItemCondition();
		sharedProcedureCreatorComponentHVActiveDischarge = new SharedProcedureCreatorComponent(base.components);
		sharedProcedureIntegrationComponentHVActiveDischarge = new SharedProcedureIntegrationComponent(base.components);
		sharedProcedureSelection = new SharedProcedureSelection();
		labelStatus = new Label();
		checkmarkStatus = new Checkmark();
		buttonStartStop = new Button();
		tableLayoutPanelStatusIndicators = new TableLayoutPanel();
		tableLayoutPanelMain = new TableLayoutPanel();
		tableLayoutPanelTop = new TableLayoutPanel();
		pictureBoxWarningIcon = new PictureBox();
		webBrowserWarning = new WebBrowser();
		digitalReadoutInstrumentHVReady = new DigitalReadoutInstrument();
		digitalReadoutInstrumentHVActiveDischargeRequestResults = new DigitalReadoutInstrument();
		digitalReadoutInstrumentGlobalHVIL = new DigitalReadoutInstrument();
		((Control)(object)tableLayoutPanelStatusIndicators).SuspendLayout();
		((Control)(object)tableLayoutPanelMain).SuspendLayout();
		((Control)(object)tableLayoutPanelTop).SuspendLayout();
		((ISupportInitialize)pictureBoxWarningIcon).BeginInit();
		((Control)this).SuspendLayout();
		sharedProcedureCreatorComponentHVActiveDischarge.Suspend();
		sharedProcedureCreatorComponentHVActiveDischarge.MonitorCall = new ServiceCall("ECPC01T", "RT_OTF_HV_ActiveDischarge_Request_Results_Active_Discharge_Status");
		sharedProcedureCreatorComponentHVActiveDischarge.MonitorGradient.Initialize((ValueState)0, 6);
		sharedProcedureCreatorComponentHVActiveDischarge.MonitorGradient.Modify(1, 0.0, (ValueState)0);
		sharedProcedureCreatorComponentHVActiveDischarge.MonitorGradient.Modify(2, 1.0, (ValueState)0);
		sharedProcedureCreatorComponentHVActiveDischarge.MonitorGradient.Modify(3, 2.0, (ValueState)1);
		sharedProcedureCreatorComponentHVActiveDischarge.MonitorGradient.Modify(4, 3.0, (ValueState)3);
		sharedProcedureCreatorComponentHVActiveDischarge.MonitorGradient.Modify(5, 4.0, (ValueState)0);
		sharedProcedureCreatorComponentHVActiveDischarge.MonitorGradient.Modify(6, 15.0, (ValueState)0);
		componentResourceManager.ApplyResources(sharedProcedureCreatorComponentHVActiveDischarge, "sharedProcedureCreatorComponentHVActiveDischarge");
		sharedProcedureCreatorComponentHVActiveDischarge.Qualifier = "SP_OTF_HV_ActiveDischarge";
		sharedProcedureCreatorComponentHVActiveDischarge.StartCall = new ServiceCall("ECPC01T", "RT_OTF_HV_ActiveDischarge_Start");
		val.Gradient.Initialize((ValueState)3, 4);
		val.Gradient.Modify(1, 0.0, (ValueState)3);
		val.Gradient.Modify(2, 1.0, (ValueState)1);
		val.Gradient.Modify(3, 2.0, (ValueState)0);
		val.Gradient.Modify(4, 3.0, (ValueState)0);
		val.Qualifier = new Qualifier((QualifierTypes)1, "ECPC01T", "DT_DS008_HV_Ready");
		sharedProcedureCreatorComponentHVActiveDischarge.StartConditions.Add(val);
		sharedProcedureCreatorComponentHVActiveDischarge.StopCall = new ServiceCall("ECPC01T", "RT_OTF_HV_ActiveDischarge_Stop");
		sharedProcedureCreatorComponentHVActiveDischarge.Resume();
		sharedProcedureIntegrationComponentHVActiveDischarge.ProceduresDropDown = sharedProcedureSelection;
		sharedProcedureIntegrationComponentHVActiveDischarge.ProcedureStatusMessageTarget = labelStatus;
		sharedProcedureIntegrationComponentHVActiveDischarge.ProcedureStatusStateTarget = checkmarkStatus;
		sharedProcedureIntegrationComponentHVActiveDischarge.ResultsTarget = null;
		sharedProcedureIntegrationComponentHVActiveDischarge.StartStopButton = buttonStartStop;
		sharedProcedureIntegrationComponentHVActiveDischarge.StopAllButton = null;
		componentResourceManager.ApplyResources(sharedProcedureSelection, "sharedProcedureSelection");
		((Control)(object)sharedProcedureSelection).Name = "sharedProcedureSelection";
		sharedProcedureSelection.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>)new string[1] { "SP_OTF_HV_ActiveDischarge" });
		componentResourceManager.ApplyResources(labelStatus, "labelStatus");
		labelStatus.Name = "labelStatus";
		labelStatus.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(checkmarkStatus, "checkmarkStatus");
		((Control)(object)checkmarkStatus).Name = "checkmarkStatus";
		componentResourceManager.ApplyResources(buttonStartStop, "buttonStartStop");
		buttonStartStop.Name = "buttonStartStop";
		buttonStartStop.UseCompatibleTextRendering = true;
		buttonStartStop.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(tableLayoutPanelStatusIndicators, "tableLayoutPanelStatusIndicators");
		((TableLayoutPanel)(object)tableLayoutPanelMain).SetColumnSpan((Control)(object)tableLayoutPanelStatusIndicators, 2);
		((TableLayoutPanel)(object)tableLayoutPanelStatusIndicators).Controls.Add((Control)(object)sharedProcedureSelection, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanelStatusIndicators).Controls.Add(buttonStartStop, 3, 0);
		((TableLayoutPanel)(object)tableLayoutPanelStatusIndicators).Controls.Add(labelStatus, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanelStatusIndicators).Controls.Add((Control)(object)checkmarkStatus, 0, 0);
		((Control)(object)tableLayoutPanelStatusIndicators).Name = "tableLayoutPanelStatusIndicators";
		componentResourceManager.ApplyResources(tableLayoutPanelMain, "tableLayoutPanelMain");
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)tableLayoutPanelStatusIndicators, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)tableLayoutPanelTop, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)digitalReadoutInstrumentHVReady, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)digitalReadoutInstrumentHVActiveDischargeRequestResults, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanelMain).Controls.Add((Control)(object)digitalReadoutInstrumentGlobalHVIL, 0, 2);
		((Control)(object)tableLayoutPanelMain).Name = "tableLayoutPanelMain";
		componentResourceManager.ApplyResources(tableLayoutPanelTop, "tableLayoutPanelTop");
		((TableLayoutPanel)(object)tableLayoutPanelMain).SetColumnSpan((Control)(object)tableLayoutPanelTop, 2);
		((TableLayoutPanel)(object)tableLayoutPanelTop).Controls.Add(pictureBoxWarningIcon, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelTop).Controls.Add(webBrowserWarning, 1, 0);
		((Control)(object)tableLayoutPanelTop).Name = "tableLayoutPanelTop";
		pictureBoxWarningIcon.BackColor = Color.White;
		componentResourceManager.ApplyResources(pictureBoxWarningIcon, "pictureBoxWarningIcon");
		pictureBoxWarningIcon.Name = "pictureBoxWarningIcon";
		pictureBoxWarningIcon.TabStop = false;
		componentResourceManager.ApplyResources(webBrowserWarning, "webBrowserWarning");
		webBrowserWarning.Name = "webBrowserWarning";
		webBrowserWarning.Url = new Uri("about: blank", UriKind.Absolute);
		((TableLayoutPanel)(object)tableLayoutPanelMain).SetColumnSpan((Control)(object)digitalReadoutInstrumentHVReady, 2);
		componentResourceManager.ApplyResources(digitalReadoutInstrumentHVReady, "digitalReadoutInstrumentHVReady");
		digitalReadoutInstrumentHVReady.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentHVReady).FreezeValue = false;
		digitalReadoutInstrumentHVReady.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText"));
		digitalReadoutInstrumentHVReady.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText1"));
		digitalReadoutInstrumentHVReady.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText2"));
		digitalReadoutInstrumentHVReady.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText3"));
		digitalReadoutInstrumentHVReady.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText4"));
		digitalReadoutInstrumentHVReady.Gradient.Initialize((ValueState)0, 4);
		digitalReadoutInstrumentHVReady.Gradient.Modify(1, 0.0, (ValueState)2);
		digitalReadoutInstrumentHVReady.Gradient.Modify(2, 1.0, (ValueState)1);
		digitalReadoutInstrumentHVReady.Gradient.Modify(3, 2.0, (ValueState)0);
		digitalReadoutInstrumentHVReady.Gradient.Modify(4, 3.0, (ValueState)0);
		((SingleInstrumentBase)digitalReadoutInstrumentHVReady).Instrument = new Qualifier((QualifierTypes)1, "ECPC01T", "DT_DS008_HV_Ready");
		((Control)(object)digitalReadoutInstrumentHVReady).Name = "digitalReadoutInstrumentHVReady";
		((SingleInstrumentBase)digitalReadoutInstrumentHVReady).ShowValueReadout = false;
		((SingleInstrumentBase)digitalReadoutInstrumentHVReady).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanelMain).SetColumnSpan((Control)(object)digitalReadoutInstrumentHVActiveDischargeRequestResults, 2);
		componentResourceManager.ApplyResources(digitalReadoutInstrumentHVActiveDischargeRequestResults, "digitalReadoutInstrumentHVActiveDischargeRequestResults");
		digitalReadoutInstrumentHVActiveDischargeRequestResults.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentHVActiveDischargeRequestResults).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentHVActiveDischargeRequestResults).Instrument = new Qualifier((QualifierTypes)64, "ECPC01T", "RT_OTF_HV_ActiveDischarge_Request_Results_Active_Discharge_Status");
		((Control)(object)digitalReadoutInstrumentHVActiveDischargeRequestResults).Name = "digitalReadoutInstrumentHVActiveDischargeRequestResults";
		((SingleInstrumentBase)digitalReadoutInstrumentHVActiveDischargeRequestResults).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanelMain).SetColumnSpan((Control)(object)digitalReadoutInstrumentGlobalHVIL, 2);
		componentResourceManager.ApplyResources(digitalReadoutInstrumentGlobalHVIL, "digitalReadoutInstrumentGlobalHVIL");
		digitalReadoutInstrumentGlobalHVIL.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentGlobalHVIL).FreezeValue = false;
		digitalReadoutInstrumentGlobalHVIL.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText5"));
		digitalReadoutInstrumentGlobalHVIL.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText6"));
		digitalReadoutInstrumentGlobalHVIL.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText7"));
		digitalReadoutInstrumentGlobalHVIL.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText8"));
		digitalReadoutInstrumentGlobalHVIL.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText9"));
		digitalReadoutInstrumentGlobalHVIL.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText10"));
		digitalReadoutInstrumentGlobalHVIL.Gradient.Initialize((ValueState)0, 5);
		digitalReadoutInstrumentGlobalHVIL.Gradient.Modify(1, 0.0, (ValueState)0);
		digitalReadoutInstrumentGlobalHVIL.Gradient.Modify(2, 1.0, (ValueState)0);
		digitalReadoutInstrumentGlobalHVIL.Gradient.Modify(3, 2.0, (ValueState)0);
		digitalReadoutInstrumentGlobalHVIL.Gradient.Modify(4, 3.0, (ValueState)0);
		digitalReadoutInstrumentGlobalHVIL.Gradient.Modify(5, 2147483647.0, (ValueState)0);
		((SingleInstrumentBase)digitalReadoutInstrumentGlobalHVIL).Instrument = new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS219_globalhvil_globalhvil");
		((Control)(object)digitalReadoutInstrumentGlobalHVIL).Name = "digitalReadoutInstrumentGlobalHVIL";
		((SingleInstrumentBase)digitalReadoutInstrumentGlobalHVIL).ShowValueReadout = false;
		((SingleInstrumentBase)digitalReadoutInstrumentGlobalHVIL).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(this, "$this");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanelMain);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanelStatusIndicators).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelStatusIndicators).PerformLayout();
		((Control)(object)tableLayoutPanelMain).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelMain).PerformLayout();
		((Control)(object)tableLayoutPanelTop).ResumeLayout(performLayout: false);
		((ISupportInitialize)pictureBoxWarningIcon).EndInit();
		((Control)this).ResumeLayout(performLayout: false);
	}
}
