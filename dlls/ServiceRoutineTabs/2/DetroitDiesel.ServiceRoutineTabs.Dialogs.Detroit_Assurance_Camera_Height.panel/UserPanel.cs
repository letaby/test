using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Detroit_Assurance_Camera_Height.panel;

public class UserPanel : CustomPanel
{
	private const string VRDUName = "VRDU01T";

	private const string HeightFaultCode = "00FBED";

	private const string HeightParameterGroupQualifier = "VCD_Camera_Parameter";

	private const string DefaultCameraConfigPartNumber = "A0004472751001";

	private Channel vrdu = null;

	private bool waitingForParameterWrite;

	private bool waitingForFaultStatusChange;

	private bool waitingForFaultClear;

	private DigitalReadoutInstrument digitalReadoutInstrument1;

	private TableLayoutPanel tableLayoutPanel1;

	private Button buttonSetHeight;

	private System.Windows.Forms.Label labelStatus;

	private DigitalReadoutInstrument digitalReadoutInstrumentCameraHeight;

	private bool Online => vrdu != null && vrdu.CommunicationsState == CommunicationsState.Online;

	private bool Busy => waitingForParameterWrite || waitingForFaultClear || waitingForFaultStatusChange;

	public UserPanel()
	{
		InitializeComponent();
		UpdateUserInterface();
	}

	protected override void OnLoad(EventArgs e)
	{
		((ContainerControl)this).ParentForm.FormClosing += OnFormClosing;
		((UserControl)this).OnLoad(e);
		UpdateChannel();
	}

	private void OnFormClosing(object sender, FormClosingEventArgs e)
	{
		if (Busy)
		{
			e.Cancel = true;
		}
		if (!e.Cancel)
		{
			((ContainerControl)this).ParentForm.FormClosing -= OnFormClosing;
			if (vrdu != null)
			{
				vrdu.CommunicationsStateUpdateEvent -= OnCommunicationsStateUpdate;
				vrdu.FaultCodes.FaultCodesUpdateEvent -= FaultCodes_FaultCodesUpdateEvent;
			}
		}
	}

	public override void OnChannelsChanged()
	{
		UpdateChannel();
	}

	private void UpdateChannel()
	{
		Channel channel = SapiManager.GlobalInstance.ActiveChannels.FirstOrDefault((Channel c) => c.Ecu.Name == "VRDU01T");
		if (vrdu != channel)
		{
			if (vrdu != null)
			{
				vrdu.CommunicationsStateUpdateEvent -= OnCommunicationsStateUpdate;
				vrdu.FaultCodes.FaultCodesUpdateEvent -= FaultCodes_FaultCodesUpdateEvent;
			}
			vrdu = channel;
			if (vrdu != null)
			{
				vrdu.CommunicationsStateUpdateEvent += OnCommunicationsStateUpdate;
				vrdu.FaultCodes.FaultCodesUpdateEvent += FaultCodes_FaultCodesUpdateEvent;
				UpdateParameterState();
				UpdateStatus(Resources.Message_Ready);
			}
		}
	}

	private void OnCommunicationsStateUpdate(object sender, CommunicationsStateEventArgs e)
	{
		UpdateUserInterface();
	}

	private void UpdateStatus(string message)
	{
		labelStatus.Text = message;
	}

	private void UpdateUserInterface()
	{
		if (Online && !Busy)
		{
			buttonSetHeight.Enabled = true;
			Cursor.Current = Cursors.Default;
		}
		else
		{
			buttonSetHeight.Enabled = false;
			Cursor.Current = Cursors.WaitCursor;
		}
	}

	private void UpdateParameterState()
	{
		if (Online)
		{
			vrdu.Parameters.ReadGroup("VCD_Camera_Parameter", fromCache: false, synchronous: false);
		}
	}

	private void ResetCameraHeightFault()
	{
		if (Online)
		{
			FaultCode faultCode = vrdu.FaultCodes["00FBED"];
			if (faultCode != null)
			{
				vrdu.FaultCodes.FaultCodesUpdateEvent += FaultCodes_FaultCodesUpdateEvent;
				UpdateStatus(Resources.Message_ClearingFault);
				waitingForFaultClear = true;
				faultCode.Reset(synchronous: false);
			}
		}
		UpdateUserInterface();
	}

	private void buttonSetHeight_Click(object sender, EventArgs e)
	{
		if (Online)
		{
			CodingChoice codingForPart = vrdu.CodingParameterGroups.GetCodingForPart("A0004472751001");
			if (codingForPart != null)
			{
				UpdateStatus(Resources.Message_WritingParameter);
				waitingForParameterWrite = true;
				try
				{
					codingForPart.SetAsValue();
					vrdu.Parameters.ParametersWriteCompleteEvent += Parameters_ParametersWriteCompleteEvent;
					vrdu.Parameters.Write(synchronous: false);
				}
				catch (CaesarException ex)
				{
					waitingForParameterWrite = false;
					if (ex.ErrorNumber == 5098)
					{
						UpdateStatus(Resources.Message_DefaultSettingNotFound);
					}
					else
					{
						UpdateStatus(Resources.Message_FailedToWriteParameters);
					}
				}
			}
			else
			{
				UpdateStatus(Resources.Message_DefaultSettingNotFound);
			}
		}
		UpdateUserInterface();
	}

	private void Parameters_ParametersWriteCompleteEvent(object sender, ResultEventArgs e)
	{
		vrdu.Parameters.ParametersWriteCompleteEvent -= Parameters_ParametersWriteCompleteEvent;
		if (e.Succeeded)
		{
			FaultCode faultCode = vrdu.FaultCodes.Current.FirstOrDefault((FaultCode f) => f.Code == "00FBED");
			if (faultCode != null)
			{
				if (faultCode.FaultCodeIncidents.Current.Active == ActiveStatus.Active)
				{
					UpdateStatus(Resources.Message_WaitingForFaultToGoInactive);
					waitingForFaultStatusChange = true;
				}
				else
				{
					ResetCameraHeightFault();
				}
			}
			else
			{
				UpdateStatus(Resources.Message_Ready1);
			}
		}
		else
		{
			UpdateStatus(Resources.Message_FailedToWriteParameters);
		}
		waitingForParameterWrite = false;
		UpdateUserInterface();
	}

	private void FaultCodes_FaultCodesUpdateEvent(object sender, ResultEventArgs e)
	{
		if (waitingForFaultStatusChange)
		{
			waitingForFaultStatusChange = false;
			ResetCameraHeightFault();
		}
		else if (waitingForFaultClear)
		{
			UpdateStatus(Resources.Message_Ready2a);
			waitingForFaultClear = false;
		}
		UpdateUserInterface();
	}

	private void InitializeComponent()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel1 = new TableLayoutPanel();
		digitalReadoutInstrument1 = new DigitalReadoutInstrument();
		digitalReadoutInstrumentCameraHeight = new DigitalReadoutInstrument();
		buttonSetHeight = new Button();
		labelStatus = new System.Windows.Forms.Label();
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument1, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentCameraHeight, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonSetHeight, 2, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(labelStatus, 0, 2);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		componentResourceManager.ApplyResources(digitalReadoutInstrument1, "digitalReadoutInstrument1");
		digitalReadoutInstrument1.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument1).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes)32, "VRDU01T", "00FBED");
		((Control)(object)digitalReadoutInstrument1).Name = "digitalReadoutInstrument1";
		((SingleInstrumentBase)digitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)digitalReadoutInstrumentCameraHeight, 2);
		componentResourceManager.ApplyResources(digitalReadoutInstrumentCameraHeight, "digitalReadoutInstrumentCameraHeight");
		digitalReadoutInstrumentCameraHeight.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentCameraHeight).FreezeValue = false;
		digitalReadoutInstrumentCameraHeight.Gradient.Initialize((ValueState)3, 2);
		digitalReadoutInstrumentCameraHeight.Gradient.Modify(1, 2.549, (ValueState)1);
		digitalReadoutInstrumentCameraHeight.Gradient.Modify(2, 2.56, (ValueState)2);
		((SingleInstrumentBase)digitalReadoutInstrumentCameraHeight).Instrument = new Qualifier((QualifierTypes)4, "VRDU01T", "camera_height");
		((Control)(object)digitalReadoutInstrumentCameraHeight).Name = "digitalReadoutInstrumentCameraHeight";
		((SingleInstrumentBase)digitalReadoutInstrumentCameraHeight).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(buttonSetHeight, "buttonSetHeight");
		buttonSetHeight.Name = "buttonSetHeight";
		buttonSetHeight.UseCompatibleTextRendering = true;
		buttonSetHeight.UseVisualStyleBackColor = true;
		buttonSetHeight.Click += buttonSetHeight_Click;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)labelStatus, 2);
		componentResourceManager.ApplyResources(labelStatus, "labelStatus");
		labelStatus.Name = "labelStatus";
		labelStatus.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(this, "$this");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
