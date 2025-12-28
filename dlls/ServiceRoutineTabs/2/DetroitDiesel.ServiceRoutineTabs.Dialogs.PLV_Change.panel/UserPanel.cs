using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.PLV_Change.panel;

public class UserPanel : CustomPanel
{
	private const string plvOpenCounter = "PLV_Open_Counter";

	private const string setEolDefaultService = "RT_SR014_SET_EOL_Default_Values_Start";

	private Channel channel;

	private TableLayoutPanel tableLayoutPanel1;

	private Button buttonResetLearntData;

	private DigitalReadoutInstrument digitalReadoutInstrument1;

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
		((ContainerControl)this).ParentForm.FormClosing += OnFormClosing;
		((UserControl)this).OnLoad(e);
	}

	private void OnFormClosing(object sender, FormClosingEventArgs e)
	{
		if (!e.Cancel)
		{
			((ContainerControl)this).ParentForm.FormClosing -= OnFormClosing;
			buttonResetLearntData.Click -= OnResetLearntDataClick;
			readAccumulatorsButton.Click -= OnReadAccumulatorsClick;
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
			ReadParameter(channel.Parameters["PLV_Open_Counter"]);
		}
	}

	private void ReadParameter(Parameter parameter)
	{
		if (parameter != null && parameter.Channel.Online)
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
		if (channel != null && DialogResult.Yes == MessageBox.Show(Resources.Message_AreYouSureYouWantToResetTheLearnedData0, ApplicationInformation.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2))
		{
			Service service = channel.Services["RT_SR014_SET_EOL_Default_Values_Start"];
			if (service != null)
			{
				service.InputValues[0].Value = service.InputValues[0].Choices.GetItemFromRawValue(26);
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
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel1 = new TableLayoutPanel();
		buttonResetLearntData = new Button();
		digitalReadoutInstrument1 = new DigitalReadoutInstrument();
		buttonClose = new Button();
		readAccumulatorsButton = new Button();
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonResetLearntData, 2, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument1, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonClose, 3, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(readAccumulatorsButton, 1, 1);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		componentResourceManager.ApplyResources(buttonResetLearntData, "buttonResetLearntData");
		buttonResetLearntData.Name = "buttonResetLearntData";
		buttonResetLearntData.UseCompatibleTextRendering = true;
		buttonResetLearntData.UseVisualStyleBackColor = true;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)digitalReadoutInstrument1, 4);
		componentResourceManager.ApplyResources(digitalReadoutInstrument1, "digitalReadoutInstrument1");
		digitalReadoutInstrument1.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument1).FreezeValue = false;
		digitalReadoutInstrument1.Gradient.Initialize((ValueState)0, 1);
		digitalReadoutInstrument1.Gradient.Modify(1, 50.0, (ValueState)2);
		((SingleInstrumentBase)digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes)4, "MCM", "PLV_Open_Counter");
		((Control)(object)digitalReadoutInstrument1).Name = "digitalReadoutInstrument1";
		((SingleInstrumentBase)digitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
		buttonClose.DialogResult = DialogResult.OK;
		componentResourceManager.ApplyResources(buttonClose, "buttonClose");
		buttonClose.Name = "buttonClose";
		buttonClose.UseCompatibleTextRendering = true;
		buttonClose.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(readAccumulatorsButton, "readAccumulatorsButton");
		readAccumulatorsButton.Name = "readAccumulatorsButton";
		readAccumulatorsButton.UseCompatibleTextRendering = true;
		readAccumulatorsButton.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("Panel_PLVChange");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel1).PerformLayout();
		((Control)this).ResumeLayout(performLayout: false);
	}
}
