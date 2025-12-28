using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Help;
using DetroitDiesel.Utilities;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.TCM_Learn_Procedure.panel;

public class UserPanel : CustomPanel
{
	private Channel tcmChannel = null;

	private Checkmark startCheckmark;

	private TableLayoutPanel tableLayoutPanel1;

	private SeekTimeListView seekTimeListView1;

	private DigitalReadoutInstrument digitalReadoutInstrument1;

	private DigitalReadoutInstrument digitalReadoutInstrument3;

	private DigitalReadoutInstrument digitalReadoutInstrument2;

	private System.Windows.Forms.Label results;

	private TableLayoutPanel tableLayoutPanel2;

	private Button Start;

	private SharedProcedureSelection sharedProcedureSelection;

	private ComboBox comboBoxType;

	private DigitalReadoutInstrument digitalReadoutInstrument4;

	private DigitalReadoutInstrument digitalReadoutInstrument5;

	private DigitalReadoutInstrument digitalReadoutInstrumentEngineStartReq;

	private DigitalReadoutInstrument digitalReadoutInstrumentIgnitionReq;

	private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponent1;

	public UserPanel()
	{
		InitializeComponent();
		comboBoxType.SelectedIndex = 1;
	}

	protected override void OnLoad(EventArgs e)
	{
		((UserControl)this).OnLoad(e);
		((ContainerControl)this).ParentForm.FormClosing += OnParentFormClosing;
	}

	private void OnParentFormClosing(object sender, FormClosingEventArgs e)
	{
		if (sharedProcedureSelection.AnyProcedureInProgress)
		{
			e.Cancel = true;
			return;
		}
		((CustomPanel)this).ParentFormClosing -= OnParentFormClosing;
		SetTcm(null);
		if (((SingleInstrumentBase)digitalReadoutInstrument5).DataItem != null && ((SingleInstrumentBase)digitalReadoutInstrument5).DataItem.Value != null)
		{
			((Control)this).Tag = new object[2]
			{
				((Choice)((SingleInstrumentBase)digitalReadoutInstrument5).DataItem.Value).Index == 0,
				((SingleInstrumentBase)digitalReadoutInstrument5).DataItem.Value.ToString()
			};
		}
		else
		{
			((Control)this).Tag = new object[2]
			{
				false,
				Resources.Message_ProcedureWasNotRun
			};
		}
	}

	public override void OnChannelsChanged()
	{
		SetTcm(((CustomPanel)this).GetChannel("TCM01T", (ChannelLookupOptions)7));
	}

	private void SetTcm(Channel tcm)
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Expected O, but got Unknown
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		if (tcmChannel == tcm)
		{
			return;
		}
		tcmChannel = tcm;
		if (tcm == null)
		{
			return;
		}
		foreach (SingleInstrumentBase item in CustomPanel.GetControlsOfType(((Control)this).Controls, typeof(SingleInstrumentBase)))
		{
			SingleInstrumentBase val = item;
			Qualifier instrument = val.Instrument;
			Ecu ecuByName = SapiManager.GetEcuByName(((Qualifier)(ref instrument)).Ecu);
			if (ecuByName != null && ecuByName.Identifier == tcm.Ecu.Identifier && ecuByName.Name != tcm.Ecu.Name)
			{
				instrument = val.Instrument;
				QualifierTypes type = ((Qualifier)(ref instrument)).Type;
				string name = tcm.Ecu.Name;
				instrument = val.Instrument;
				val.Instrument = new Qualifier(type, name, ((Qualifier)(ref instrument)).Name);
			}
		}
	}

	private void comboBoxType_SelectedIndexChanged(object sender, EventArgs e)
	{
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Expected O, but got Unknown
		string text = ((object)sharedProcedureSelection.SharedProcedureQualifiers).ToString();
		if (text.Contains("("))
		{
			text = text.Substring(0, text.IndexOf("("));
		}
		try
		{
			text += string.Format(CultureInfo.InvariantCulture, "({0})", comboBoxType.SelectedIndex + 1);
		}
		catch (FormatException)
		{
			MessageBox.Show(Resources.Message_PleaseEnterANumericValue, ApplicationInformation.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1, (MessageBoxOptions)0);
		}
		sharedProcedureSelection.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>)new string[1] { text });
	}

	private void InitializeComponent()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Expected O, but got Unknown
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
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
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Expected O, but got Unknown
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Expected O, but got Unknown
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Expected O, but got Unknown
		//IL_0363: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0537: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0712: Unknown result type (might be due to invalid IL or missing references)
		//IL_071c: Expected O, but got Unknown
		//IL_07fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0879: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a1a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad2: Unknown result type (might be due to invalid IL or missing references)
		base.components = new Container();
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel1 = new TableLayoutPanel();
		digitalReadoutInstrumentIgnitionReq = new DigitalReadoutInstrument();
		startCheckmark = new Checkmark();
		seekTimeListView1 = new SeekTimeListView();
		digitalReadoutInstrument1 = new DigitalReadoutInstrument();
		digitalReadoutInstrument3 = new DigitalReadoutInstrument();
		digitalReadoutInstrument2 = new DigitalReadoutInstrument();
		results = new System.Windows.Forms.Label();
		tableLayoutPanel2 = new TableLayoutPanel();
		Start = new Button();
		sharedProcedureSelection = new SharedProcedureSelection();
		comboBoxType = new ComboBox();
		digitalReadoutInstrument4 = new DigitalReadoutInstrument();
		digitalReadoutInstrument5 = new DigitalReadoutInstrument();
		digitalReadoutInstrumentEngineStartReq = new DigitalReadoutInstrument();
		sharedProcedureIntegrationComponent1 = new SharedProcedureIntegrationComponent(base.components);
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)(object)tableLayoutPanel2).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentIgnitionReq, 0, 5);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)startCheckmark, 0, 6);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)seekTimeListView1, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument1, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument3, 2, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument2, 2, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(results, 1, 6);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)tableLayoutPanel2, 2, 6);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument4, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument5, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentEngineStartReq, 0, 4);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)digitalReadoutInstrumentIgnitionReq, 3);
		componentResourceManager.ApplyResources(digitalReadoutInstrumentIgnitionReq, "digitalReadoutInstrumentIgnitionReq");
		digitalReadoutInstrumentIgnitionReq.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentIgnitionReq).FreezeValue = false;
		digitalReadoutInstrumentIgnitionReq.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText"));
		digitalReadoutInstrumentIgnitionReq.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText1"));
		digitalReadoutInstrumentIgnitionReq.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText2"));
		digitalReadoutInstrumentIgnitionReq.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText3"));
		digitalReadoutInstrumentIgnitionReq.Gradient.Initialize((ValueState)0, 3);
		digitalReadoutInstrumentIgnitionReq.Gradient.Modify(1, 0.0, (ValueState)2);
		digitalReadoutInstrumentIgnitionReq.Gradient.Modify(2, 1.0, (ValueState)4);
		digitalReadoutInstrumentIgnitionReq.Gradient.Modify(3, 2.0, (ValueState)0);
		((SingleInstrumentBase)digitalReadoutInstrumentIgnitionReq).Instrument = new Qualifier((QualifierTypes)16, "fake", "IgnitionStatusRequest");
		((Control)(object)digitalReadoutInstrumentIgnitionReq).Name = "digitalReadoutInstrumentIgnitionReq";
		((SingleInstrumentBase)digitalReadoutInstrumentIgnitionReq).ShowValueReadout = false;
		((SingleInstrumentBase)digitalReadoutInstrumentIgnitionReq).TitlePosition = (LabelPosition)0;
		((SingleInstrumentBase)digitalReadoutInstrumentIgnitionReq).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(startCheckmark, "startCheckmark");
		((Control)(object)startCheckmark).Name = "startCheckmark";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)seekTimeListView1, 3);
		componentResourceManager.ApplyResources(seekTimeListView1, "seekTimeListView1");
		seekTimeListView1.FilterUserLabels = true;
		((Control)(object)seekTimeListView1).Name = "seekTimeListView1";
		seekTimeListView1.RequiredUserLabelPrefix = "TCM Learn";
		seekTimeListView1.SelectedTime = null;
		seekTimeListView1.ShowChannelLabels = false;
		seekTimeListView1.ShowCommunicationsState = false;
		seekTimeListView1.ShowControlPanel = false;
		seekTimeListView1.ShowDeviceColumn = false;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)digitalReadoutInstrument1, 2);
		componentResourceManager.ApplyResources(digitalReadoutInstrument1, "digitalReadoutInstrument1");
		digitalReadoutInstrument1.FontGroup = "TCMLearn_instruments";
		((SingleInstrumentBase)digitalReadoutInstrument1).FreezeValue = false;
		digitalReadoutInstrument1.Gradient.Initialize((ValueState)1, 0);
		((SingleInstrumentBase)digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS010_Engine_Speed");
		((Control)(object)digitalReadoutInstrument1).Name = "digitalReadoutInstrument1";
		((SingleInstrumentBase)digitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument3, "digitalReadoutInstrument3");
		digitalReadoutInstrument3.FontGroup = "TCMLearn_statuses";
		((SingleInstrumentBase)digitalReadoutInstrument3).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument3).Instrument = new Qualifier((QualifierTypes)1, "TCM01T", "DT_231B_Status_Einlernen_Kupplung_Status_Einlernen_Kupplung");
		((Control)(object)digitalReadoutInstrument3).Name = "digitalReadoutInstrument3";
		((SingleInstrumentBase)digitalReadoutInstrument3).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument2, "digitalReadoutInstrument2");
		digitalReadoutInstrument2.FontGroup = "TCMLearn_instruments";
		((SingleInstrumentBase)digitalReadoutInstrument2).FreezeValue = false;
		digitalReadoutInstrument2.Gradient.Initialize((ValueState)3, 1, "psi");
		digitalReadoutInstrument2.Gradient.Modify(1, 90.0, (ValueState)1);
		((SingleInstrumentBase)digitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes)1, "TCM01T", "DT_2311_Versorgungsdruck_Getriebe_Versorgungsdruck");
		((Control)(object)digitalReadoutInstrument2).Name = "digitalReadoutInstrument2";
		((SingleInstrumentBase)digitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(results, "results");
		results.Name = "results";
		results.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(tableLayoutPanel2, "tableLayoutPanel2");
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(Start, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)sharedProcedureSelection, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(comboBoxType, 0, 0);
		((Control)(object)tableLayoutPanel2).Name = "tableLayoutPanel2";
		componentResourceManager.ApplyResources(Start, "Start");
		Start.Name = "Start";
		Start.UseCompatibleTextRendering = true;
		Start.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(sharedProcedureSelection, "sharedProcedureSelection");
		((Control)(object)sharedProcedureSelection).Name = "sharedProcedureSelection";
		sharedProcedureSelection.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>)new string[1] { "SP_TCM_Learn" });
		componentResourceManager.ApplyResources(comboBoxType, "comboBoxType");
		comboBoxType.DropDownStyle = ComboBoxStyle.DropDownList;
		comboBoxType.FormattingEnabled = true;
		comboBoxType.Items.AddRange(new object[2]
		{
			componentResourceManager.GetString("comboBoxType.Items"),
			componentResourceManager.GetString("comboBoxType.Items1")
		});
		comboBoxType.Name = "comboBoxType";
		comboBoxType.SelectedIndexChanged += comboBoxType_SelectedIndexChanged;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)digitalReadoutInstrument4, 2);
		componentResourceManager.ApplyResources(digitalReadoutInstrument4, "digitalReadoutInstrument4");
		digitalReadoutInstrument4.FontGroup = "TCMLearn_statuses";
		((SingleInstrumentBase)digitalReadoutInstrument4).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument4).Instrument = new Qualifier((QualifierTypes)1, "TCM01T", "DT_2111_Status_Einlernen_Getriebe_stGbLrn");
		((Control)(object)digitalReadoutInstrument4).Name = "digitalReadoutInstrument4";
		((SingleInstrumentBase)digitalReadoutInstrument4).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)digitalReadoutInstrument5, 3);
		componentResourceManager.ApplyResources(digitalReadoutInstrument5, "digitalReadoutInstrument5");
		digitalReadoutInstrument5.FontGroup = "TCMLearn_statuses";
		((SingleInstrumentBase)digitalReadoutInstrument5).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument5).Instrument = new Qualifier((QualifierTypes)64, "TCM01T", "RT_0400_Einlernvorgang_Service_Request_Results_Fehler_Lernvorgang");
		((Control)(object)digitalReadoutInstrument5).Name = "digitalReadoutInstrument5";
		((SingleInstrumentBase)digitalReadoutInstrument5).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)digitalReadoutInstrumentEngineStartReq, 3);
		componentResourceManager.ApplyResources(digitalReadoutInstrumentEngineStartReq, "digitalReadoutInstrumentEngineStartReq");
		digitalReadoutInstrumentEngineStartReq.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentEngineStartReq).FreezeValue = false;
		digitalReadoutInstrumentEngineStartReq.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText4"));
		digitalReadoutInstrumentEngineStartReq.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText5"));
		digitalReadoutInstrumentEngineStartReq.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText6"));
		digitalReadoutInstrumentEngineStartReq.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText7"));
		digitalReadoutInstrumentEngineStartReq.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText8"));
		digitalReadoutInstrumentEngineStartReq.Gradient.Initialize((ValueState)0, 4);
		digitalReadoutInstrumentEngineStartReq.Gradient.Modify(1, 0.0, (ValueState)0);
		digitalReadoutInstrumentEngineStartReq.Gradient.Modify(2, 1.0, (ValueState)2);
		digitalReadoutInstrumentEngineStartReq.Gradient.Modify(3, 2.0, (ValueState)0);
		digitalReadoutInstrumentEngineStartReq.Gradient.Modify(4, 255.0, (ValueState)0);
		((SingleInstrumentBase)digitalReadoutInstrumentEngineStartReq).Instrument = new Qualifier((QualifierTypes)1, "TCM01T", "DT_2112_Anforderung_zum_Motorstart_waehrend_des_Einlernvorgangs_Anforderung_Motorstart");
		((Control)(object)digitalReadoutInstrumentEngineStartReq).Name = "digitalReadoutInstrumentEngineStartReq";
		((SingleInstrumentBase)digitalReadoutInstrumentEngineStartReq).ShowValueReadout = false;
		((SingleInstrumentBase)digitalReadoutInstrumentEngineStartReq).TitlePosition = (LabelPosition)0;
		((SingleInstrumentBase)digitalReadoutInstrumentEngineStartReq).UnitAlignment = StringAlignment.Near;
		sharedProcedureIntegrationComponent1.ProceduresDropDown = sharedProcedureSelection;
		sharedProcedureIntegrationComponent1.ProcedureStatusMessageTarget = results;
		sharedProcedureIntegrationComponent1.ProcedureStatusStateTarget = startCheckmark;
		sharedProcedureIntegrationComponent1.ResultsTarget = null;
		sharedProcedureIntegrationComponent1.StartStopButton = Start;
		sharedProcedureIntegrationComponent1.StopAllButton = null;
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("Panel_TransmissionLearnProcedure");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel2).ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
