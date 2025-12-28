using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.UnitConversion;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Accumulated_Fuel_Mass_Sync__MY17_.panel;

public class UserPanel : CustomPanel
{
	private class SetupInformation
	{
		public readonly string ReadService;

		public readonly string WriteService;

		public readonly string CommitService;

		public SetupInformation(string writeService, string commitService, string readService)
		{
			WriteService = writeService;
			CommitService = commitService;
			ReadService = readService;
		}
	}

	private const int MaxDistanceInKm = 1310700;

	private Dictionary<string, SetupInformation> atsServices = new Dictionary<string, SetupInformation>();

	private string serviceRoutineInputUnits;

	private bool newValueWritten = false;

	private Channel mcmChannel;

	private Channel acmChannel;

	private Conversion uiToSr;

	private Conversion srToUi;

	private TableLayoutPanel tableLayoutPanelWholePanel;

	private Button buttonClose;

	private Button buttonSetAtsDistance;

	private TextBox textBoxNewAtsDistance;

	private TableLayoutPanel tableLayoutPanelDistance;

	private System.Windows.Forms.Label label1;

	private TableLayoutPanel tableLayoutPanelDirections;

	private TableLayoutPanel tableLayoutPanelCurrentValues;

	private DigitalReadoutInstrument digitalReadoutInstrumentCurrentMCMAtsDistance;

	private System.Windows.Forms.Label labelDirections;

	private Checkmark checkmarkReady;

	private RadioButton radioButtonCopyValueFromMCM;

	private RadioButton radioButtonCopyValueFromACM;

	private RadioButton radioButtonResetATSToZero;

	private RadioButton radioButtonEnterATSDistance;

	private System.Windows.Forms.Label label2;

	private DigitalReadoutInstrument digitalReadoutInstrumentCurrentACMAtsDistance;

	private float CustomDistance
	{
		get
		{
			if (!float.TryParse(textBoxNewAtsDistance.Text, out var result) || result < 0f || (double)result > srToUi.Convert(1310700.0))
			{
				return -1f;
			}
			return result;
		}
	}

	private string UiDistanceUnits => Converter.GlobalInstance.GetConversion(serviceRoutineInputUnits).OutputUnit;

	public UserPanel()
	{
		InitializeComponent();
		atsServices.Add("MCM21T", new SetupInformation("RT_SR0EB_ATS_lifetime_ageing_strategy_Start(Distance_value={0})", "RT_SR012_Save_EOL_Data_Request_Start(save=2,e2p_block_nr=30)", "RT_SR0EB_ATS_lifetime_ageing_strategy_Request_Results_Distance_driven"));
		atsServices.Add("ACM21T", new SetupInformation("RT_SR02EB_ATS_lifetime_ageing_strategy_Start(Distance_value={0})", "RT_Save_EOL_Data_Start(Save_Type=2,Block_Type=30)", "RT_SR02EB_ATS_lifetime_ageing_strategy_Request_Results_Distance_driven"));
	}

	protected override void OnLoad(EventArgs e)
	{
		((ContainerControl)this).ParentForm.FormClosing += OnParentFormClosing;
		textBoxNewAtsDistance.TextChanged += textBoxNewAtsDistance_TextChanged;
		textBoxNewAtsDistance.KeyPress += textBoxNewAtsDistance_KeyPress;
		((UserControl)this).OnLoad(e);
		UpdateUI();
	}

	public override void OnChannelsChanged()
	{
		UpdateChannel(((CustomPanel)this).GetChannel("MCM21T"), ref mcmChannel);
		UpdateChannel(((CustomPanel)this).GetChannel("ACM21T"), ref acmChannel);
		UpdateUI();
	}

	private void UpdateChannel(Channel newChannel, ref Channel channel)
	{
		if (channel != newChannel)
		{
			if (channel != null)
			{
				channel.Services.ServiceCompleteEvent -= channel_ServiceCompleteEvent;
			}
			channel = newChannel;
			if (channel != null)
			{
				channel.Services.ServiceCompleteEvent += channel_ServiceCompleteEvent;
				serviceRoutineInputUnits = channel.Services[atsServices[channel.Ecu.Name].ReadService].Units;
				srToUi = Converter.GlobalInstance.GetConversion(serviceRoutineInputUnits, UiDistanceUnits);
				uiToSr = Converter.GlobalInstance.GetConversion(UiDistanceUnits, serviceRoutineInputUnits);
				RunService(channel, atsServices[channel.Ecu.Name].ReadService);
			}
		}
	}

	private void channel_ServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		Service service = sender as Service;
		if (service != null)
		{
			SetupInformation setupInformation = atsServices[service.Channel.Ecu.Name];
			if (setupInformation.WriteService.StartsWith(service.Qualifier))
			{
				Thread.Sleep(500);
				RunService(service.Channel, setupInformation.CommitService);
				RunService(service.Channel, setupInformation.ReadService);
				newValueWritten = true;
			}
		}
		UpdateUI();
	}

	private void RunService(Channel channel, string serviceQualifier)
	{
		if (channel != null)
		{
			Service service = channel.Services[serviceQualifier];
			if (service != null)
			{
				service.InputValues.ParseValues(serviceQualifier);
				service.Execute(synchronous: false);
			}
		}
	}

	private void OnParentFormClosing(object sender, FormClosingEventArgs e)
	{
		if (!e.Cancel)
		{
			((ContainerControl)this).ParentForm.FormClosing -= OnParentFormClosing;
			textBoxNewAtsDistance.TextChanged -= textBoxNewAtsDistance_TextChanged;
			textBoxNewAtsDistance.KeyPress -= textBoxNewAtsDistance_KeyPress;
			UpdateChannel(null, ref mcmChannel);
			UpdateChannel(null, ref acmChannel);
		}
	}

	private void UpdateUI()
	{
		bool flag = false;
		bool flag2 = false;
		if (mcmChannel == null || !mcmChannel.Online || ((SingleInstrumentBase)digitalReadoutInstrumentCurrentMCMAtsDistance).DataItem.Value == null)
		{
			labelDirections.Text = Resources.Message_MCMNotOnline;
			newValueWritten = false;
		}
		else if (acmChannel == null || !acmChannel.Online || ((SingleInstrumentBase)digitalReadoutInstrumentCurrentACMAtsDistance).DataItem.Value == null)
		{
			labelDirections.Text = Resources.Message_ACMNotOnline;
			newValueWritten = false;
		}
		else
		{
			flag2 = true;
			labelDirections.Text = (newValueWritten ? Resources.Message_ATSSet : Resources.Message_ChooseAction);
			if (CustomDistance >= 0f)
			{
				textBoxNewAtsDistance.BackColor = SystemColors.Window;
				flag = true;
			}
			else
			{
				textBoxNewAtsDistance.BackColor = Color.LightPink;
				if (radioButtonEnterATSDistance.Checked)
				{
					labelDirections.Text = string.Format(Resources.MessageFormat_PleaseSpecifyAnATSDistanceBetween0And01, (int)srToUi.Convert(1310700.0), UiDistanceUnits);
				}
			}
		}
		RadioButton radioButton = radioButtonCopyValueFromACM;
		RadioButton radioButton2 = radioButtonCopyValueFromMCM;
		RadioButton radioButton3 = radioButtonEnterATSDistance;
		bool flag3 = (radioButtonResetATSToZero.Enabled = flag2);
		flag3 = (radioButton3.Enabled = flag3);
		flag3 = (radioButton2.Enabled = flag3);
		radioButton.Enabled = flag3;
		textBoxNewAtsDistance.Enabled = radioButtonEnterATSDistance.Checked;
		buttonSetAtsDistance.Enabled = flag;
		checkmarkReady.CheckState = (flag ? CheckState.Checked : CheckState.Unchecked);
	}

	private void textBoxNewAtsDistance_KeyPress(object sender, KeyPressEventArgs e)
	{
		e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && (e.KeyChar != '.' || textBoxNewAtsDistance.Text.Contains('.'));
	}

	private void textBoxNewAtsDistance_TextChanged(object sender, EventArgs e)
	{
		newValueWritten = false;
		UpdateUI();
	}

	private void buttonSetAtsDistance_Click(object sender, EventArgs e)
	{
		if (CustomDistance >= 0f)
		{
			labelDirections.Text = string.Format(Resources.MessageFormat_SettingDistanceTo01, CustomDistance, UiDistanceUnits);
			RunService(mcmChannel, string.Format(atsServices[mcmChannel.Ecu.Name].WriteService, (int)uiToSr.Convert((double)CustomDistance)));
			RunService(acmChannel, string.Format(atsServices[acmChannel.Ecu.Name].WriteService, (int)uiToSr.Convert((double)CustomDistance)));
		}
	}

	private void radioButtonCopyValueFromACM_CheckedChanged(object sender, EventArgs e)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Invalid comparison between Unknown and I4
		RadioButton radioButton = sender as RadioButton;
		if (radioButton.Checked && (int)digitalReadoutInstrumentCurrentACMAtsDistance.RepresentedState == 1)
		{
			textBoxNewAtsDistance.Text = srToUi.Convert(((SingleInstrumentBase)digitalReadoutInstrumentCurrentACMAtsDistance).DataItem.Value).ToString("F1");
		}
	}

	private void radioButtonCopyValueFromMCM_CheckedChanged(object sender, EventArgs e)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Invalid comparison between Unknown and I4
		RadioButton radioButton = sender as RadioButton;
		if (radioButton.Checked && (int)digitalReadoutInstrumentCurrentMCMAtsDistance.RepresentedState == 1)
		{
			textBoxNewAtsDistance.Text = srToUi.Convert(((SingleInstrumentBase)digitalReadoutInstrumentCurrentMCMAtsDistance).DataItem.Value).ToString("F1");
		}
	}

	private void radioButtonResetATSToZero_CheckedChanged(object sender, EventArgs e)
	{
		RadioButton radioButton = sender as RadioButton;
		if (radioButton.Checked)
		{
			textBoxNewAtsDistance.Text = "0";
		}
	}

	private void radioButtonEnterATSDistance_CheckedChanged(object sender, EventArgs e)
	{
		textBoxNewAtsDistance.Enabled = radioButtonEnterATSDistance.Checked;
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
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Expected O, but got Unknown
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Expected O, but got Unknown
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Expected O, but got Unknown
		//IL_06e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_077e: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanelCurrentValues = new TableLayoutPanel();
		tableLayoutPanelDirections = new TableLayoutPanel();
		tableLayoutPanelDistance = new TableLayoutPanel();
		tableLayoutPanelWholePanel = new TableLayoutPanel();
		buttonSetAtsDistance = new Button();
		textBoxNewAtsDistance = new TextBox();
		label1 = new System.Windows.Forms.Label();
		buttonClose = new Button();
		radioButtonCopyValueFromMCM = new RadioButton();
		radioButtonCopyValueFromACM = new RadioButton();
		radioButtonResetATSToZero = new RadioButton();
		radioButtonEnterATSDistance = new RadioButton();
		label2 = new System.Windows.Forms.Label();
		labelDirections = new System.Windows.Forms.Label();
		checkmarkReady = new Checkmark();
		digitalReadoutInstrumentCurrentMCMAtsDistance = new DigitalReadoutInstrument();
		digitalReadoutInstrumentCurrentACMAtsDistance = new DigitalReadoutInstrument();
		((Control)(object)tableLayoutPanelWholePanel).SuspendLayout();
		((Control)(object)tableLayoutPanelDistance).SuspendLayout();
		((Control)(object)tableLayoutPanelDirections).SuspendLayout();
		((Control)(object)tableLayoutPanelCurrentValues).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanelWholePanel, "tableLayoutPanelWholePanel");
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add((Control)(object)tableLayoutPanelDistance, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add((Control)(object)tableLayoutPanelDirections, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanelWholePanel).Controls.Add((Control)(object)tableLayoutPanelCurrentValues, 0, 0);
		((Control)(object)tableLayoutPanelWholePanel).Name = "tableLayoutPanelWholePanel";
		componentResourceManager.ApplyResources(tableLayoutPanelDistance, "tableLayoutPanelDistance");
		((TableLayoutPanel)(object)tableLayoutPanelDistance).Controls.Add(buttonSetAtsDistance, 2, 5);
		((TableLayoutPanel)(object)tableLayoutPanelDistance).Controls.Add(textBoxNewAtsDistance, 1, 5);
		((TableLayoutPanel)(object)tableLayoutPanelDistance).Controls.Add(label1, 0, 5);
		((TableLayoutPanel)(object)tableLayoutPanelDistance).Controls.Add(buttonClose, 3, 5);
		((TableLayoutPanel)(object)tableLayoutPanelDistance).Controls.Add(radioButtonCopyValueFromMCM, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanelDistance).Controls.Add(radioButtonCopyValueFromACM, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanelDistance).Controls.Add(radioButtonResetATSToZero, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanelDistance).Controls.Add(radioButtonEnterATSDistance, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanelDistance).Controls.Add(label2, 0, 0);
		((Control)(object)tableLayoutPanelDistance).Name = "tableLayoutPanelDistance";
		componentResourceManager.ApplyResources(buttonSetAtsDistance, "buttonSetAtsDistance");
		buttonSetAtsDistance.Name = "buttonSetAtsDistance";
		buttonSetAtsDistance.UseCompatibleTextRendering = true;
		buttonSetAtsDistance.UseVisualStyleBackColor = true;
		buttonSetAtsDistance.Click += buttonSetAtsDistance_Click;
		componentResourceManager.ApplyResources(textBoxNewAtsDistance, "textBoxNewAtsDistance");
		textBoxNewAtsDistance.Name = "textBoxNewAtsDistance";
		componentResourceManager.ApplyResources(label1, "label1");
		label1.Name = "label1";
		label1.UseCompatibleTextRendering = true;
		buttonClose.DialogResult = DialogResult.OK;
		componentResourceManager.ApplyResources(buttonClose, "buttonClose");
		buttonClose.Name = "buttonClose";
		buttonClose.UseCompatibleTextRendering = true;
		buttonClose.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(radioButtonCopyValueFromMCM, "radioButtonCopyValueFromMCM");
		((TableLayoutPanel)(object)tableLayoutPanelDistance).SetColumnSpan((Control)radioButtonCopyValueFromMCM, 4);
		radioButtonCopyValueFromMCM.Name = "radioButtonCopyValueFromMCM";
		radioButtonCopyValueFromMCM.TabStop = true;
		radioButtonCopyValueFromMCM.UseCompatibleTextRendering = true;
		radioButtonCopyValueFromMCM.UseVisualStyleBackColor = true;
		radioButtonCopyValueFromMCM.CheckedChanged += radioButtonCopyValueFromMCM_CheckedChanged;
		componentResourceManager.ApplyResources(radioButtonCopyValueFromACM, "radioButtonCopyValueFromACM");
		((TableLayoutPanel)(object)tableLayoutPanelDistance).SetColumnSpan((Control)radioButtonCopyValueFromACM, 4);
		radioButtonCopyValueFromACM.Name = "radioButtonCopyValueFromACM";
		radioButtonCopyValueFromACM.TabStop = true;
		radioButtonCopyValueFromACM.UseCompatibleTextRendering = true;
		radioButtonCopyValueFromACM.UseVisualStyleBackColor = true;
		radioButtonCopyValueFromACM.CheckedChanged += radioButtonCopyValueFromACM_CheckedChanged;
		componentResourceManager.ApplyResources(radioButtonResetATSToZero, "radioButtonResetATSToZero");
		((TableLayoutPanel)(object)tableLayoutPanelDistance).SetColumnSpan((Control)radioButtonResetATSToZero, 4);
		radioButtonResetATSToZero.Name = "radioButtonResetATSToZero";
		radioButtonResetATSToZero.TabStop = true;
		radioButtonResetATSToZero.UseCompatibleTextRendering = true;
		radioButtonResetATSToZero.UseVisualStyleBackColor = true;
		radioButtonResetATSToZero.CheckedChanged += radioButtonResetATSToZero_CheckedChanged;
		componentResourceManager.ApplyResources(radioButtonEnterATSDistance, "radioButtonEnterATSDistance");
		((TableLayoutPanel)(object)tableLayoutPanelDistance).SetColumnSpan((Control)radioButtonEnterATSDistance, 4);
		radioButtonEnterATSDistance.Name = "radioButtonEnterATSDistance";
		radioButtonEnterATSDistance.TabStop = true;
		radioButtonEnterATSDistance.UseCompatibleTextRendering = true;
		radioButtonEnterATSDistance.UseVisualStyleBackColor = true;
		radioButtonEnterATSDistance.CheckedChanged += radioButtonEnterATSDistance_CheckedChanged;
		componentResourceManager.ApplyResources(label2, "label2");
		((TableLayoutPanel)(object)tableLayoutPanelDistance).SetColumnSpan((Control)label2, 4);
		label2.Name = "label2";
		label2.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(tableLayoutPanelDirections, "tableLayoutPanelDirections");
		((TableLayoutPanel)(object)tableLayoutPanelDirections).Controls.Add(labelDirections, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanelDirections).Controls.Add((Control)(object)checkmarkReady, 0, 0);
		((Control)(object)tableLayoutPanelDirections).Name = "tableLayoutPanelDirections";
		componentResourceManager.ApplyResources(labelDirections, "labelDirections");
		labelDirections.Name = "labelDirections";
		labelDirections.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(checkmarkReady, "checkmarkReady");
		((Control)(object)checkmarkReady).Name = "checkmarkReady";
		componentResourceManager.ApplyResources(tableLayoutPanelCurrentValues, "tableLayoutPanelCurrentValues");
		((TableLayoutPanel)(object)tableLayoutPanelCurrentValues).Controls.Add((Control)(object)digitalReadoutInstrumentCurrentMCMAtsDistance, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelCurrentValues).Controls.Add((Control)(object)digitalReadoutInstrumentCurrentACMAtsDistance, 2, 0);
		((Control)(object)tableLayoutPanelCurrentValues).Name = "tableLayoutPanelCurrentValues";
		componentResourceManager.ApplyResources(digitalReadoutInstrumentCurrentMCMAtsDistance, "digitalReadoutInstrumentCurrentMCMAtsDistance");
		digitalReadoutInstrumentCurrentMCMAtsDistance.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentCurrentMCMAtsDistance).FreezeValue = false;
		digitalReadoutInstrumentCurrentMCMAtsDistance.Gradient.Initialize((ValueState)0, 1);
		digitalReadoutInstrumentCurrentMCMAtsDistance.Gradient.Modify(1, 0.0, (ValueState)1);
		((SingleInstrumentBase)digitalReadoutInstrumentCurrentMCMAtsDistance).Instrument = new Qualifier((QualifierTypes)64, "MCM21T", "RT_SR0EB_ATS_lifetime_ageing_strategy_Request_Results_Distance_driven");
		((Control)(object)digitalReadoutInstrumentCurrentMCMAtsDistance).Name = "digitalReadoutInstrumentCurrentMCMAtsDistance";
		((SingleInstrumentBase)digitalReadoutInstrumentCurrentMCMAtsDistance).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentCurrentACMAtsDistance, "digitalReadoutInstrumentCurrentACMAtsDistance");
		digitalReadoutInstrumentCurrentACMAtsDistance.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentCurrentACMAtsDistance).FreezeValue = false;
		digitalReadoutInstrumentCurrentACMAtsDistance.Gradient.Initialize((ValueState)0, 1);
		digitalReadoutInstrumentCurrentACMAtsDistance.Gradient.Modify(1, 0.0, (ValueState)1);
		((SingleInstrumentBase)digitalReadoutInstrumentCurrentACMAtsDistance).Instrument = new Qualifier((QualifierTypes)64, "ACM21T", "RT_SR02EB_ATS_lifetime_ageing_strategy_Request_Results_Distance_driven");
		((Control)(object)digitalReadoutInstrumentCurrentACMAtsDistance).Name = "digitalReadoutInstrumentCurrentACMAtsDistance";
		((SingleInstrumentBase)digitalReadoutInstrumentCurrentACMAtsDistance).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(this, "$this");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanelWholePanel);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanelWholePanel).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelWholePanel).PerformLayout();
		((Control)(object)tableLayoutPanelDistance).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelDistance).PerformLayout();
		((Control)(object)tableLayoutPanelDirections).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelCurrentValues).ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
