using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Utilities;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Maintenance_System_Transfer_Data.panel;

public class UserPanel : CustomPanel
{
	private SeekTimeListView seekTimeListView1;

	private Button button1;

	private SharedProcedureCreatorComponent sharedProcedureCreatorComponentTransferData;

	private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponent1;

	private SharedProcedureSelection sharedProcedureSelection1;

	private System.Windows.Forms.Label label1;

	private Checkmark checkmark1;

	private DigitalReadoutInstrument digitalReadoutRequestResults;

	private TableLayoutPanel tableLayoutPanel1;

	private TableLayoutPanel tableLayoutPanel2;

	private Button button2;

	private bool testStoppedByUser { get; set; }

	public UserPanel()
	{
		InitializeComponent();
	}

	private void UserPanel_ParentFormClosing(object sender, FormClosingEventArgs e)
	{
		if (sharedProcedureIntegrationComponent1.ProceduresDropDown.AnyProcedureInProgress)
		{
			e.Cancel = true;
		}
		if (!e.Cancel)
		{
			((CustomPanel)this).ParentFormClosing -= UserPanel_ParentFormClosing;
		}
	}

	private void button1_Click(object sender, EventArgs e)
	{
		testStoppedByUser = sharedProcedureIntegrationComponent1.ProceduresDropDown.AnyProcedureInProgress;
	}

	private void digitalReadoutRequestResults_RepresentedStateChanged(object sender, EventArgs e)
	{
		if (((SingleInstrumentBase)digitalReadoutRequestResults).DataItem != null)
		{
			LogText(((SingleInstrumentBase)digitalReadoutRequestResults).DataItem.ValueAsString(((SingleInstrumentBase)digitalReadoutRequestResults).DataItem.Value));
		}
	}

	private void LogText(string text)
	{
		((CustomPanel)this).LabelLog(seekTimeListView1.RequiredUserLabelPrefix, text);
	}

	private void sharedProcedureCreatorComponentTransferData_StartServiceComplete(object sender, SingleServiceResultEventArgs e)
	{
		LogText(Resources.Message_Started);
	}

	private void sharedProcedureCreatorComponentTransferData_StopServiceComplete(object sender, SingleServiceResultEventArgs e)
	{
		if (((ResultEventArgs)(object)e).Succeeded)
		{
			if (testStoppedByUser)
			{
				LogText(Resources.Message_Stopped);
			}
			else if (((SingleInstrumentBase)digitalReadoutRequestResults).DataItem.Choices != null && ((SingleInstrumentBase)digitalReadoutRequestResults).DataItem.Value == ((SingleInstrumentBase)digitalReadoutRequestResults).DataItem.Choices.GetItemFromRawValue(0))
			{
				LogText(Resources.Message_CompleteSuccess);
			}
			else
			{
				LogText(Resources.Message_Error);
			}
		}
		else
		{
			LogText(Resources.Message_Error);
			LogText(((ResultEventArgs)(object)e).Exception.Message);
		}
	}

	private void InitializeComponent()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Expected O, but got Unknown
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Expected O, but got Unknown
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Expected O, but got Unknown
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Expected O, but got Unknown
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Expected O, but got Unknown
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Expected O, but got Unknown
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Expected O, but got Unknown
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03de: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e8: Expected O, but got Unknown
		//IL_0655: Unknown result type (might be due to invalid IL or missing references)
		base.components = new Container();
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		seekTimeListView1 = new SeekTimeListView();
		button1 = new Button();
		sharedProcedureCreatorComponentTransferData = new SharedProcedureCreatorComponent(base.components);
		sharedProcedureIntegrationComponent1 = new SharedProcedureIntegrationComponent(base.components);
		sharedProcedureSelection1 = new SharedProcedureSelection();
		label1 = new System.Windows.Forms.Label();
		checkmark1 = new Checkmark();
		tableLayoutPanel1 = new TableLayoutPanel();
		button2 = new Button();
		digitalReadoutRequestResults = new DigitalReadoutInstrument();
		tableLayoutPanel2 = new TableLayoutPanel();
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)(object)tableLayoutPanel2).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(seekTimeListView1, "seekTimeListView1");
		seekTimeListView1.FilterUserLabels = true;
		((Control)(object)seekTimeListView1).Name = "seekTimeListView1";
		seekTimeListView1.RequiredUserLabelPrefix = "MaintenanceSystemTransfer";
		seekTimeListView1.SelectedTime = null;
		seekTimeListView1.ShowChannelLabels = false;
		seekTimeListView1.ShowCommunicationsState = false;
		seekTimeListView1.ShowControlPanel = false;
		seekTimeListView1.ShowDeviceColumn = false;
		seekTimeListView1.TimeFormat = "MM.dd.yyyy HH:mm:ss";
		componentResourceManager.ApplyResources(button1, "button1");
		button1.Name = "button1";
		button1.UseCompatibleTextRendering = true;
		button1.UseVisualStyleBackColor = true;
		button1.Click += button1_Click;
		sharedProcedureCreatorComponentTransferData.Suspend();
		sharedProcedureCreatorComponentTransferData.MonitorCall = new ServiceCall("MS01T", "RT_Status_of_mirror_memory_data_transfer_Request_Results_Status_of_data_transfer_from_the_mirror_memory");
		sharedProcedureCreatorComponentTransferData.MonitorGradient.Initialize((ValueState)0, 7);
		sharedProcedureCreatorComponentTransferData.MonitorGradient.Modify(1, 0.0, (ValueState)1);
		sharedProcedureCreatorComponentTransferData.MonitorGradient.Modify(2, 1.0, (ValueState)0);
		sharedProcedureCreatorComponentTransferData.MonitorGradient.Modify(3, 2.0, (ValueState)3);
		sharedProcedureCreatorComponentTransferData.MonitorGradient.Modify(4, 3.0, (ValueState)3);
		sharedProcedureCreatorComponentTransferData.MonitorGradient.Modify(5, 4.0, (ValueState)3);
		sharedProcedureCreatorComponentTransferData.MonitorGradient.Modify(6, 5.0, (ValueState)3);
		sharedProcedureCreatorComponentTransferData.MonitorGradient.Modify(7, 6.0, (ValueState)3);
		componentResourceManager.ApplyResources(sharedProcedureCreatorComponentTransferData, "sharedProcedureCreatorComponentTransferData");
		sharedProcedureCreatorComponentTransferData.Qualifier = "SP_TransferDataFromMirrorMemory";
		sharedProcedureCreatorComponentTransferData.StartCall = new ServiceCall("MS01T", "RT_Transfer_data_from_the_mirror_memory_Start");
		sharedProcedureCreatorComponentTransferData.StopCall = new ServiceCall("MS01T", "RT_Status_of_mirror_memory_data_transfer_Request_Results_Status_of_data_transfer_from_the_mirror_memory");
		sharedProcedureCreatorComponentTransferData.StartServiceComplete += sharedProcedureCreatorComponentTransferData_StartServiceComplete;
		sharedProcedureCreatorComponentTransferData.StopServiceComplete += sharedProcedureCreatorComponentTransferData_StopServiceComplete;
		sharedProcedureCreatorComponentTransferData.Resume();
		sharedProcedureIntegrationComponent1.ProceduresDropDown = sharedProcedureSelection1;
		sharedProcedureIntegrationComponent1.ProcedureStatusMessageTarget = label1;
		sharedProcedureIntegrationComponent1.ProcedureStatusStateTarget = checkmark1;
		sharedProcedureIntegrationComponent1.ResultsTarget = null;
		sharedProcedureIntegrationComponent1.StartStopButton = button1;
		sharedProcedureIntegrationComponent1.StopAllButton = null;
		componentResourceManager.ApplyResources(sharedProcedureSelection1, "sharedProcedureSelection1");
		((Control)(object)sharedProcedureSelection1).Name = "sharedProcedureSelection1";
		sharedProcedureSelection1.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>)new string[1] { "SP_TransferDataFromMirrorMemory" });
		((Control)(object)sharedProcedureSelection1).TabStop = false;
		componentResourceManager.ApplyResources(label1, "label1");
		label1.Name = "label1";
		label1.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(checkmark1, "checkmark1");
		((Control)(object)checkmark1).Name = "checkmark1";
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(button1, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)checkmark1, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(label1, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)sharedProcedureSelection1, 3, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(button2, 4, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		button2.DialogResult = DialogResult.OK;
		componentResourceManager.ApplyResources(button2, "button2");
		button2.Name = "button2";
		button2.UseCompatibleTextRendering = true;
		button2.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(digitalReadoutRequestResults, "digitalReadoutRequestResults");
		digitalReadoutRequestResults.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutRequestResults).FreezeValue = false;
		digitalReadoutRequestResults.Gradient.Initialize((ValueState)0, 7);
		digitalReadoutRequestResults.Gradient.Modify(1, 0.0, (ValueState)0);
		digitalReadoutRequestResults.Gradient.Modify(2, 1.0, (ValueState)1);
		digitalReadoutRequestResults.Gradient.Modify(3, 2.0, (ValueState)3);
		digitalReadoutRequestResults.Gradient.Modify(4, 3.0, (ValueState)3);
		digitalReadoutRequestResults.Gradient.Modify(5, 4.0, (ValueState)3);
		digitalReadoutRequestResults.Gradient.Modify(6, 5.0, (ValueState)3);
		digitalReadoutRequestResults.Gradient.Modify(7, 6.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutRequestResults).Instrument = new Qualifier((QualifierTypes)64, "MS01T", "RT_Status_of_mirror_memory_data_transfer_Request_Results_Status_of_data_transfer_from_the_mirror_memory");
		((Control)(object)digitalReadoutRequestResults).Name = "digitalReadoutRequestResults";
		((SingleInstrumentBase)digitalReadoutRequestResults).UnitAlignment = StringAlignment.Near;
		digitalReadoutRequestResults.RepresentedStateChanged += digitalReadoutRequestResults_RepresentedStateChanged;
		componentResourceManager.ApplyResources(tableLayoutPanel2, "tableLayoutPanel2");
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)seekTimeListView1, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)tableLayoutPanel1, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)digitalReadoutRequestResults, 0, 1);
		((Control)(object)tableLayoutPanel2).Name = "tableLayoutPanel2";
		componentResourceManager.ApplyResources(this, "$this");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel2);
		((Control)this).Name = "UserPanel";
		((CustomPanel)this).ParentFormClosing += UserPanel_ParentFormClosing;
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel2).ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
