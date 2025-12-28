using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Extensions.SharedProcedures;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.DOC_Lightoff_Temperature_Reset.panel;

public class UserPanel : CustomPanel
{
	private RunServiceButton runServiceButton;

	private TableLayoutPanel tableLayoutPanel1;

	private Button buttonClose;

	private DigitalReadoutInstrument digitalRedoutInstrumentEngineSpeed;

	private TableLayoutPanel tableLayoutPanel2;

	private Checkmark checkmark1;

	private Label labelStatus;

	private System.Windows.Forms.Label label1;

	private bool CanStart => !((RunSharedProcedureButtonBase)runServiceButton).IsBusy && (int)digitalRedoutInstrumentEngineSpeed.RepresentedState == 1;

	private bool CanClose => !((RunSharedProcedureButtonBase)runServiceButton).IsBusy;

	public UserPanel()
	{
		InitializeComponent();
	}

	private void runServiceButton_ServiceComplete(object sender, SingleServiceResultEventArgs e)
	{
		if (((ResultEventArgs)(object)e).Exception != null)
		{
			MessageBox.Show((IWin32Window)this, ((ResultEventArgs)(object)e).Exception.Message, ApplicationInformation.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
		else
		{
			MessageBox.Show((IWin32Window)this, Resources.Message_TheValueWasSuccessfullyChanged, ApplicationInformation.ProductName, MessageBoxButtons.OK);
		}
	}

	private void UserPanel_ParentFormClosing(object sender, FormClosingEventArgs e)
	{
		e.Cancel = !CanClose;
	}

	private void UpdateUserInterface()
	{
		((Control)(object)runServiceButton).Enabled = CanStart;
		checkmark1.Checked = CanStart;
		((Control)(object)labelStatus).Text = (CanStart ? Resources.Message_ProcedureReady : Resources.Message_TurnEngineOff);
		buttonClose.Enabled = CanClose;
	}

	private void digitalRedoutInstrumentEngineSpeed_RepresentedStateChanged(object sender, EventArgs e)
	{
		UpdateUserInterface();
	}

	private void InitializeComponent()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel1 = new TableLayoutPanel();
		runServiceButton = new RunServiceButton();
		label1 = new System.Windows.Forms.Label();
		buttonClose = new Button();
		digitalRedoutInstrumentEngineSpeed = new DigitalReadoutInstrument();
		tableLayoutPanel2 = new TableLayoutPanel();
		checkmark1 = new Checkmark();
		labelStatus = new Label();
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)(object)tableLayoutPanel2).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)runServiceButton, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(label1, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonClose, 1, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalRedoutInstrumentEngineSpeed, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)tableLayoutPanel2, 0, 2);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		((ContainerControl)(object)runServiceButton).AutoValidate = AutoValidate.EnablePreventFocusChange;
		componentResourceManager.ApplyResources(runServiceButton, "runServiceButton");
		((Control)(object)runServiceButton).Name = "runServiceButton";
		runServiceButton.ServiceCall = new ServiceCall("ACM21T", "RT_SR0D4_Reset_Lightoff_Enhancer_Temp_Start");
		runServiceButton.ServiceComplete += runServiceButton_ServiceComplete;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)label1, 2);
		componentResourceManager.ApplyResources(label1, "label1");
		label1.Name = "label1";
		label1.UseCompatibleTextRendering = true;
		buttonClose.DialogResult = DialogResult.OK;
		componentResourceManager.ApplyResources(buttonClose, "buttonClose");
		buttonClose.Name = "buttonClose";
		buttonClose.UseCompatibleTextRendering = true;
		buttonClose.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(digitalRedoutInstrumentEngineSpeed, "digitalRedoutInstrumentEngineSpeed");
		digitalRedoutInstrumentEngineSpeed.FontGroup = null;
		((SingleInstrumentBase)digitalRedoutInstrumentEngineSpeed).FreezeValue = false;
		digitalRedoutInstrumentEngineSpeed.Gradient.Initialize((ValueState)1, 1);
		digitalRedoutInstrumentEngineSpeed.Gradient.Modify(1, 1.0, (ValueState)3);
		((SingleInstrumentBase)digitalRedoutInstrumentEngineSpeed).Instrument = new Qualifier((QualifierTypes)1, "ACM21T", "DT_AS001_Engine_Speed");
		((Control)(object)digitalRedoutInstrumentEngineSpeed).Name = "digitalRedoutInstrumentEngineSpeed";
		((SingleInstrumentBase)digitalRedoutInstrumentEngineSpeed).UnitAlignment = StringAlignment.Near;
		digitalRedoutInstrumentEngineSpeed.RepresentedStateChanged += digitalRedoutInstrumentEngineSpeed_RepresentedStateChanged;
		componentResourceManager.ApplyResources(tableLayoutPanel2, "tableLayoutPanel2");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)tableLayoutPanel2, 2);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)checkmark1, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)labelStatus, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
		((Control)(object)tableLayoutPanel2).Name = "tableLayoutPanel2";
		componentResourceManager.ApplyResources(checkmark1, "checkmark1");
		((Control)(object)checkmark1).Name = "checkmark1";
		labelStatus.Alignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(labelStatus, "labelStatus");
		((Control)(object)labelStatus).Name = "labelStatus";
		labelStatus.Orientation = (TextOrientation)1;
		labelStatus.UseSystemColors = true;
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("_DDDL.chm_DOC_Lightoff_Temeperature_Reset");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Name = "UserPanel";
		((CustomPanel)this).ParentFormClosing += UserPanel_ParentFormClosing;
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel2).ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
