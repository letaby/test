using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DetroitDiesel.Help;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Engine_Idle_Shutdown.panel;

public class UserPanel : CustomPanel
{
	private readonly string startServiceQualifier = "RT_Prevent_Engine_Shutdown_Start";

	private readonly string checkServiceQualifier = "RT_Prevent_Engine_Shutdown_Request_Results_Status";

	private readonly string stopServiceQualifier = "RT_Prevent_Engine_Shutdown_Stop";

	private Channel ecu = null;

	private TableLayoutPanel tableMainLayout;

	private ScalingLabel labelStatus;

	private Button preventButton;

	private Button allowButton;

	private Button buttonClose;

	private Label labelTestStatus;

	private System.Windows.Forms.Label label1;

	private Service StartService
	{
		get
		{
			if (ecu == null)
			{
				return null;
			}
			return ecu.Services[startServiceQualifier];
		}
	}

	private Service CheckService
	{
		get
		{
			if (ecu == null)
			{
				return null;
			}
			return ecu.Services[checkServiceQualifier];
		}
	}

	private Service StopService
	{
		get
		{
			if (ecu == null)
			{
				return null;
			}
			return ecu.Services[stopServiceQualifier];
		}
	}

	private bool CanCheckStatus => Online && CheckService != null;

	private bool Online => ecu != null && ecu.CommunicationsState == CommunicationsState.Online;

	private bool CanStart => Online && CanStop && StartService != null && CanCheckStatus;

	private bool CanStop => Online && StopService != null && CanCheckStatus;

	public UserPanel()
	{
		InitializeComponent();
		allowButton.Click += OnAllowButtonClick;
		preventButton.Click += OnPreventButtonClick;
	}

	protected override void OnLoad(EventArgs e)
	{
		((ContainerControl)this).ParentForm.FormClosing += OnFormClosing;
		UpdateStatus(Resources.Message_NotSupported);
		((UserControl)this).OnLoad(e);
	}

	private void OnFormClosing(object sender, FormClosingEventArgs e)
	{
		if (!e.Cancel)
		{
			((ContainerControl)this).ParentForm.FormClosing -= OnFormClosing;
		}
	}

	public override void OnChannelsChanged()
	{
		SetECU(((CustomPanel)this).GetChannel("CPC02T"));
	}

	private void SetECU(Channel ecu)
	{
		if (this.ecu != ecu)
		{
			if (this.ecu != null)
			{
				this.ecu.CommunicationsStateUpdateEvent -= OnCommunicationsStateUpdate;
			}
			this.ecu = ecu;
			if (this.ecu != null)
			{
				this.ecu.CommunicationsStateUpdateEvent += OnCommunicationsStateUpdate;
				CheckEngineIdleShutdownStatus();
			}
			UpdateUserInterface();
		}
	}

	private void UpdateUserInterface()
	{
		preventButton.Enabled = CanStart;
		allowButton.Enabled = CanStop;
	}

	private void UpdateStatus(string status)
	{
		((Control)(object)labelStatus).Text = status;
		((Control)(object)labelStatus).Update();
	}

	private void ReportResult(string text)
	{
		((CustomPanel)this).AddStatusMessage(text);
	}

	private void OnPreventButtonClick(object sender, EventArgs e)
	{
		if (CanStart)
		{
			Service startService = StartService;
			startService.ServiceCompleteEvent += OnPreventComplete;
			startService.Execute(synchronous: false);
			UpdateUserInterface();
		}
	}

	private void OnPreventComplete(object sender, ResultEventArgs e)
	{
		Service service = (Service)sender;
		service.ServiceCompleteEvent -= OnPreventComplete;
		if (e.Succeeded)
		{
			ReportResult(Resources.Message_RequestedEngineIdleShutdownPrevention);
		}
		else
		{
			ReportResult(Resources.Message_FailureWhileTryingToPreventEngineIdleShutdown + e.Exception.Message);
		}
		CheckEngineIdleShutdownStatus();
	}

	private void OnCheckStatusComplete(object sender, ResultEventArgs e)
	{
		Service service = (Service)sender;
		service.ServiceCompleteEvent -= OnCheckStatusComplete;
		string status = Resources.Message_NotDefined;
		if (service.OutputValues.Count <= 0)
		{
			return;
		}
		Choice choice = service.OutputValues[0].Value as Choice;
		byte b = byte.MaxValue;
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
			case 0:
				status = choice.Name.ToString();
				break;
			case 1:
				status = choice.Name.ToString();
				break;
			default:
				ReportResult(string.Format(Resources.MessageFormat_EngineIdleShutdownPreventionHasFailed0, choice.Name));
				status = choice.Name.ToString();
				break;
			}
		}
		UpdateStatus(status);
	}

	private void OnAllowButtonClick(object sender, EventArgs e)
	{
		if (CanStop)
		{
			Service stopService = StopService;
			stopService.ServiceCompleteEvent += OnAllowComplete;
			stopService.Execute(synchronous: false);
			UpdateUserInterface();
		}
	}

	private void OnAllowComplete(object sender, ResultEventArgs e)
	{
		Service service = (Service)sender;
		service.ServiceCompleteEvent -= OnAllowComplete;
		if (e.Succeeded)
		{
			ReportResult(Resources.Message_RequestedEngineIdleShutdownAllowingRequested);
		}
		else
		{
			ReportResult(Resources.Message_FailureWhileTryingToAllowEngineIdleShutdown + e.Exception.Message);
		}
		CheckEngineIdleShutdownStatus();
	}

	private void OnCommunicationsStateUpdate(object sender, CommunicationsStateEventArgs e)
	{
		UpdateUserInterface();
	}

	private void CheckEngineIdleShutdownStatus()
	{
		if (CanCheckStatus)
		{
			Service checkService = CheckService;
			checkService.ServiceCompleteEvent += OnCheckStatusComplete;
			checkService.Execute(synchronous: false);
		}
	}

	private void InitializeComponent()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableMainLayout = new TableLayoutPanel();
		labelTestStatus = new Label();
		buttonClose = new Button();
		label1 = new System.Windows.Forms.Label();
		labelStatus = new ScalingLabel();
		preventButton = new Button();
		allowButton = new Button();
		((Control)(object)tableMainLayout).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableMainLayout, "tableMainLayout");
		((TableLayoutPanel)(object)tableMainLayout).Controls.Add((Control)(object)labelTestStatus, 0, 1);
		((TableLayoutPanel)(object)tableMainLayout).Controls.Add(buttonClose, 2, 3);
		((TableLayoutPanel)(object)tableMainLayout).Controls.Add(label1, 0, 0);
		((TableLayoutPanel)(object)tableMainLayout).Controls.Add((Control)(object)labelStatus, 0, 2);
		((TableLayoutPanel)(object)tableMainLayout).Controls.Add(preventButton, 1, 3);
		((TableLayoutPanel)(object)tableMainLayout).Controls.Add(allowButton, 0, 3);
		((Control)(object)tableMainLayout).Name = "tableMainLayout";
		labelTestStatus.Alignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableMainLayout).SetColumnSpan((Control)(object)labelTestStatus, 3);
		componentResourceManager.ApplyResources(labelTestStatus, "labelTestStatus");
		((Control)(object)labelTestStatus).Name = "labelTestStatus";
		labelTestStatus.Orientation = (TextOrientation)1;
		buttonClose.DialogResult = DialogResult.OK;
		componentResourceManager.ApplyResources(buttonClose, "buttonClose");
		buttonClose.Name = "buttonClose";
		buttonClose.UseCompatibleTextRendering = true;
		buttonClose.UseVisualStyleBackColor = true;
		label1.BackColor = Color.White;
		label1.BorderStyle = BorderStyle.Fixed3D;
		((TableLayoutPanel)(object)tableMainLayout).SetColumnSpan((Control)label1, 3);
		componentResourceManager.ApplyResources(label1, "label1");
		label1.ForeColor = Color.Black;
		label1.Name = "label1";
		label1.UseCompatibleTextRendering = true;
		labelStatus.Alignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableMainLayout).SetColumnSpan((Control)(object)labelStatus, 3);
		componentResourceManager.ApplyResources(labelStatus, "labelStatus");
		labelStatus.FontGroup = "";
		labelStatus.LineAlignment = StringAlignment.Center;
		((Control)(object)labelStatus).Name = "labelStatus";
		componentResourceManager.ApplyResources(preventButton, "preventButton");
		preventButton.Name = "preventButton";
		preventButton.UseCompatibleTextRendering = true;
		preventButton.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(allowButton, "allowButton");
		allowButton.Name = "allowButton";
		allowButton.UseCompatibleTextRendering = true;
		allowButton.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("Panel_EngineIdleShutdown");
		((Control)this).Controls.Add((Control)(object)tableMainLayout);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableMainLayout).ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
