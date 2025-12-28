using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Replace_Quantity_Control_Valve.panel;

public class UserPanel : CustomPanel
{
	private const string fmuAdaptationPositive = "Quantity_Control_Valve_Adaptation_Positive";

	private const string fmuAdaptationNegative = "Quantity_Control_Valve_Adaptation_Negative";

	private const string setEolDefaultService = "RT_SR014_SET_EOL_Default_Values_Start";

	private const int resetChoice = 25;

	private Channel channel;

	private TableLayoutPanel tableLayoutPanel1;

	private DigitalReadoutInstrument digitalReadoutInstrument1;

	private DigitalReadoutInstrument digitalReadoutInstrument2;

	private TableLayoutPanel tableLayoutPanel2;

	private Button buttonResetLearntData;

	private Button readAccumulatorsButton;

	private Button buttonClose;

	public UserPanel()
	{
		InitializeComponent();
		buttonResetLearntData.Click += OnResetLearntDataClick;
		readAccumulatorsButton.Click += OnReadAccumulatorsClick;
	}

	protected override void OnLoad(EventArgs e)
	{
		((UserControl)this).OnLoad(e);
		((ContainerControl)this).ParentForm.FormClosing += OnParentFormClosing;
	}

	private void OnParentFormClosing(object sender, FormClosingEventArgs e)
	{
		if (!e.Cancel)
		{
			((ContainerControl)this).ParentForm.FormClosing -= OnParentFormClosing;
			channel = null;
		}
	}

	public override void OnChannelsChanged()
	{
		if (channel != null)
		{
			channel.CommunicationsStateUpdateEvent -= OnCommunicationsStateUpdate;
		}
		channel = ((CustomPanel)this).GetChannel("MCM");
		if (channel != null)
		{
			channel.CommunicationsStateUpdateEvent += OnCommunicationsStateUpdate;
			ReadParameters();
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (channel != null)
		{
			channel.CommunicationsStateUpdateEvent -= OnCommunicationsStateUpdate;
		}
	}

	private void OnCommunicationsStateUpdate(object sender, CommunicationsStateEventArgs e)
	{
		bool enabled = false;
		if (channel != null)
		{
			enabled = channel.CommunicationsState == CommunicationsState.Online;
		}
		buttonResetLearntData.Enabled = enabled;
		readAccumulatorsButton.Enabled = enabled;
	}

	private void ReadParameters()
	{
		if (channel != null)
		{
			ReadParameter(channel.Parameters["Quantity_Control_Valve_Adaptation_Positive"]);
			ReadParameter(channel.Parameters["Quantity_Control_Valve_Adaptation_Negative"]);
		}
	}

	private void ReadParameter(Parameter parameter)
	{
		if (parameter != null)
		{
			string groupQualifier = parameter.GroupQualifier;
			parameter.Channel.Parameters.ReadGroup(groupQualifier, fromCache: false, synchronous: false);
		}
	}

	private void OnReadAccumulatorsClick(object sender, EventArgs e)
	{
		ReadParameters();
	}

	private void OnResetLearntDataClick(object sender, EventArgs e)
	{
		if (channel != null && DialogResult.Yes == MessageBox.Show(Resources.Message_AreYouSureYouWantToResetTheLearnedData, ApplicationInformation.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2))
		{
			Service service = channel.Services["RT_SR014_SET_EOL_Default_Values_Start"];
			if (service != null)
			{
				service.InputValues[0].Value = service.InputValues[0].Choices.GetItemFromRawValue(25);
				service.ServiceCompleteEvent += OnServiceComplete;
				service.Execute(synchronous: false);
			}
		}
	}

	private void OnServiceComplete(object sender, ResultEventArgs e)
	{
		if (channel != null)
		{
			Service service = channel.Services["RT_SR014_SET_EOL_Default_Values_Start"];
			if (service != null)
			{
				service.ServiceCompleteEvent -= OnServiceComplete;
			}
		}
		ReadParameters();
	}

	private void InitializeComponent()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel1 = new TableLayoutPanel();
		tableLayoutPanel2 = new TableLayoutPanel();
		buttonResetLearntData = new Button();
		buttonClose = new Button();
		readAccumulatorsButton = new Button();
		digitalReadoutInstrument1 = new DigitalReadoutInstrument();
		digitalReadoutInstrument2 = new DigitalReadoutInstrument();
		((Control)(object)tableLayoutPanel2).SuspendLayout();
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel2, "tableLayoutPanel2");
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(buttonResetLearntData, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(buttonClose, 3, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(readAccumulatorsButton, 1, 0);
		((Control)(object)tableLayoutPanel2).Name = "tableLayoutPanel2";
		componentResourceManager.ApplyResources(buttonResetLearntData, "buttonResetLearntData");
		buttonResetLearntData.Name = "buttonResetLearntData";
		buttonResetLearntData.UseCompatibleTextRendering = true;
		buttonResetLearntData.UseVisualStyleBackColor = true;
		buttonClose.DialogResult = DialogResult.OK;
		componentResourceManager.ApplyResources(buttonClose, "buttonClose");
		buttonClose.Name = "buttonClose";
		buttonClose.UseCompatibleTextRendering = true;
		buttonClose.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(readAccumulatorsButton, "readAccumulatorsButton");
		readAccumulatorsButton.Name = "readAccumulatorsButton";
		readAccumulatorsButton.UseCompatibleTextRendering = true;
		readAccumulatorsButton.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument1, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument2, 1, 0);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		componentResourceManager.ApplyResources(digitalReadoutInstrument1, "digitalReadoutInstrument1");
		digitalReadoutInstrument1.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument1).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes)4, "MCM", "Quantity_Control_Valve_Adaptation_Positive");
		((Control)(object)digitalReadoutInstrument1).Name = "digitalReadoutInstrument1";
		((SingleInstrumentBase)digitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument2, "digitalReadoutInstrument2");
		digitalReadoutInstrument2.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument2).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes)4, "MCM", "Quantity_Control_Valve_Adaptation_Negative");
		((Control)(object)digitalReadoutInstrument2).Name = "digitalReadoutInstrument2";
		((SingleInstrumentBase)digitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("Panel_ReplaceQuantityControlValve");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel2);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel2).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel2).PerformLayout();
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
		((Control)this).PerformLayout();
	}
}
