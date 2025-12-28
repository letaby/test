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

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Inverter_Resolver_Learn__EMG_.panel;

public class UserPanel : CustomPanel
{
	private const string PtfConfPTransGroup = "VCD_PID_222_ptconf_p_Trans";

	private const string PtfConfPTransEmotNum = "ptconf_p_Trans_EmotNum_u8";

	private Channel eCpcChannel;

	private bool isEmot2Num = false;

	private bool isEmot3Num = false;

	private bool isStopped1 = true;

	private bool isStopped2 = true;

	private bool isStopped3 = true;

	private Parameter emotNumParameter = null;

	private TableLayoutPanel tableLayoutPanel1;

	private DigitalReadoutInstrument digitalReadoutInstrument4;

	private DigitalReadoutInstrument digitalReadoutInstrument5;

	private DigitalReadoutInstrument digitalReadoutInstrument6;

	private TableLayoutPanel tableLayoutPanelInverter1LearnRoutine;

	private Button buttonInverter1Start;

	private System.Windows.Forms.Label labelStatusLabelInverter1;

	private Checkmark checkmarkInverter1;

	private DigitalReadoutInstrument digitalReadoutInstrument7;

	private DigitalReadoutInstrument digitalReadoutInstrumentInverterLearnResults1;

	private System.Windows.Forms.Label labelResolver1LearnRoutineHeader;

	private TableLayoutPanel tableLayoutPanelInverter2LearnRoutine;

	private Button buttonInverter2Start;

	private System.Windows.Forms.Label labelStatusLabelInverter2;

	private Checkmark checkmarkInverter2;

	private DigitalReadoutInstrument digitalReadoutInstrumentMotorSpeed2;

	private System.Windows.Forms.Label labelResolver2LearnRoutineHeader;

	private DigitalReadoutInstrument digitalReadoutInstrumentInverterLearnResults2;

	private TableLayoutPanel tableLayoutPanelInverter3LearnRoutine;

	private Button buttonInverter3Start;

	private System.Windows.Forms.Label labelStatusLabelInverter3;

	private Checkmark checkmarkInverter3;

	private DigitalReadoutInstrument digitalReadoutInstrumentMotorSpeed3;

	private DigitalReadoutInstrument digitalReadoutInstrumentInverterLearnResults3;

	private System.Windows.Forms.Label labelResolver3LearnRoutineHeader;

	private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponentInverter1;

	private SharedProcedureCreatorComponent sharedProcedureCreatorComponentInverter1;

	private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponentInverter2;

	private SharedProcedureCreatorComponent sharedProcedureCreatorComponentInverter2;

	private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponentInverter3;

	private SharedProcedureCreatorComponent sharedProcedureCreatorComponentInverter3;

	private SharedProcedureSelection sharedProcedureSelectionInverter3;

	private SharedProcedureSelection sharedProcedureSelectionInverter2;

	private SharedProcedureSelection sharedProcedureSelectionInverter1;

	private TableLayoutPanel tableLayoutPanelTop;

	private WebBrowser webBrowserWarning;

	private PictureBox pictureBox1;

	private TableLayoutPanel tableLayoutPanelBottom;

	private System.Windows.Forms.Label label5;

	private System.Windows.Forms.Label labelStatus2;

	private System.Windows.Forms.Label labelStatus3;

	private SeekTimeListView seekTimeListView1;

	private bool Learning1InProcess => (int)digitalReadoutInstrumentInverterLearnResults1.RepresentedState == 2;

	private bool Learning2InProcess => (int)digitalReadoutInstrumentInverterLearnResults2.RepresentedState == 2;

	private bool Learning3InProcess => (int)digitalReadoutInstrumentInverterLearnResults3.RepresentedState == 2;

	protected override void OnLoad(EventArgs e)
	{
		((ContainerControl)this).ParentForm.FormClosing += OnParentFormClosing;
		((UserControl)this).OnLoad(e);
		isStopped1 = true;
		isStopped2 = true;
		isStopped3 = true;
		digitalReadoutInstrumentInverterLearnResults1.RepresentedStateChanged += digitalReadoutInstrumentInverterLearnResults1_RepresentedStateChanged;
		digitalReadoutInstrumentInverterLearnResults2.RepresentedStateChanged += digitalReadoutInstrumentInverterLearnResults2_RepresentedStateChanged;
		digitalReadoutInstrumentInverterLearnResults3.RepresentedStateChanged += digitalReadoutInstrumentInverterLearnResults3_RepresentedStateChanged;
		UpdateUI();
	}

	public UserPanel()
	{
		InitializeComponent();
	}

	public override void OnChannelsChanged()
	{
		SetECPC01TChannel("ECPC01T");
	}

	private void OnParentFormClosing(object sender, FormClosingEventArgs e)
	{
		e.Cancel = Learning1InProcess || Learning2InProcess || Learning3InProcess;
		if (!e.Cancel)
		{
			((ContainerControl)this).ParentForm.FormClosing -= OnParentFormClosing;
		}
	}

	private void SetECPC01TChannel(string ecuName)
	{
		if (eCpcChannel != ((CustomPanel)this).GetChannel(ecuName))
		{
			eCpcChannel = ((CustomPanel)this).GetChannel(ecuName);
			if (eCpcChannel != null)
			{
				isStopped1 = true;
				isStopped2 = true;
				isStopped3 = true;
				emotNumParameter = eCpcChannel.Parameters["ptconf_p_Trans_EmotNum_u8"];
				if (emotNumParameter != null && eCpcChannel.CommunicationsState == CommunicationsState.Online)
				{
					ReadParameterGroup("VCD_PID_222_ptconf_p_Trans");
				}
			}
		}
		UpdateUI();
	}

	private void ReadParameterGroup(string group)
	{
		if (eCpcChannel != null && eCpcChannel.Parameters != null)
		{
			eCpcChannel.Parameters.ParametersReadCompleteEvent += Parameters_ParametersInitialReadCompleteEvent;
			eCpcChannel.Parameters.ReadGroup(group, fromCache: false, synchronous: false);
		}
	}

	private void Parameters_ParametersInitialReadCompleteEvent(object sender, ResultEventArgs e)
	{
		eCpcChannel.Parameters.ParametersReadCompleteEvent -= Parameters_ParametersInitialReadCompleteEvent;
		if (e.Succeeded)
		{
			int num = Convert.ToInt32(emotNumParameter.Value);
			isEmot2Num = num == 2;
			isEmot3Num = num == 3;
		}
		UpdateUI();
	}

	private void AddLogLabel(string text)
	{
		if (text != string.Empty)
		{
			((CustomPanel)this).LabelLog(seekTimeListView1.RequiredUserLabelPrefix, text);
		}
	}

	private void UpdateWarningMessage(bool displayInverter3Message)
	{
		string text = "html { height:100%; display: table; } ";
		text += "body { margin: 0px; padding: 0px; display: table-cell; vertical-align: middle; } ";
		text += ".scaled { font-size: calc(0.1vw + 8.75vh); font-family: Segoe UI; padding: 0px; margin: 4px; }  ";
		text += ".bold { font-weight: bold; }";
		text += ".red { color: red; }";
		string format = "<html><style>{0}</style><body><span class='scaled bold red'>{1}</span><span class='scaled bold'>{2}</span><br><span class='scaled'>{3}{4}</span></body></html>";
		webBrowserWarning.DocumentText = string.Format(CultureInfo.InvariantCulture, format, text, Resources.RedWarning, Resources.BlackWarning, Resources.WarningText, displayInverter3Message ? Resources.WarningText_Inverter_3 : string.Empty);
		webBrowserWarning.Update();
	}

	private void UpdateUI()
	{
		sharedProcedureIntegrationComponentInverter2.ProceduresDropDown = ((isEmot2Num || isEmot3Num) ? sharedProcedureSelectionInverter2 : null);
		((Control)(object)tableLayoutPanelInverter2LearnRoutine).Visible = isEmot2Num || isEmot3Num;
		((TableLayoutPanel)(object)tableLayoutPanel1).RowStyles[3].Height = ((isEmot2Num || isEmot3Num) ? 23f : 0f);
		sharedProcedureIntegrationComponentInverter3.ProceduresDropDown = (isEmot3Num ? sharedProcedureSelectionInverter3 : null);
		((Control)(object)tableLayoutPanelInverter3LearnRoutine).Visible = isEmot3Num;
		((TableLayoutPanel)(object)tableLayoutPanel1).RowStyles[5].Height = (isEmot3Num ? 23f : 0f);
		UpdateWarningMessage(isEmot3Num);
	}

	private void sharedProcedureCreatorComponentInverter1_StartServiceComplete(object sender, SingleServiceResultEventArgs e)
	{
		isStopped1 = false;
		if (((ResultEventArgs)(object)e).Succeeded)
		{
			AddLogLabel(Resources.Message_Resolver_1_Learn_Routine_Started);
		}
		else
		{
			AddLogLabel(Resources.Message_Resolver_1_Learn_Routine_FailedToStart);
			AddLogLabel(((ResultEventArgs)(object)e).Exception.Message);
		}
		UpdateUI();
	}

	private void sharedProcedureCreatorComponentInverter1_StopServiceComplete(object sender, SingleServiceResultEventArgs e)
	{
		isStopped1 = true;
		AddLogLabel(Resources.Message_Resolver_1_Learn_Routine_Stopped);
		UpdateUI();
	}

	private void sharedProcedureCreatorComponentInverter2_StartServiceComplete(object sender, SingleServiceResultEventArgs e)
	{
		isStopped1 = false;
		if (((ResultEventArgs)(object)e).Succeeded)
		{
			AddLogLabel(Resources.Message_Resolver_2_Learn_Routine_Started);
		}
		else
		{
			AddLogLabel(Resources.Message_Resolver_2_Learn_Routine_FailedToStart);
			AddLogLabel(((ResultEventArgs)(object)e).Exception.Message);
		}
		UpdateUI();
	}

	private void sharedProcedureCreatorComponentInverter2_StopServiceComplete(object sender, SingleServiceResultEventArgs e)
	{
		isStopped1 = true;
		AddLogLabel(Resources.Message_Resolver_2_Learn_Routine_Stopped);
		UpdateUI();
	}

	private void sharedProcedureCreatorComponentInverter3_StartServiceComplete(object sender, SingleServiceResultEventArgs e)
	{
		isStopped1 = false;
		if (((ResultEventArgs)(object)e).Succeeded)
		{
			AddLogLabel(Resources.Message_Resolver_3_Learn_Routine_Started);
		}
		else
		{
			AddLogLabel(Resources.Message_Resolver_3_Learn_Routine_FailedToStart);
			AddLogLabel(((ResultEventArgs)(object)e).Exception.Message);
		}
		UpdateUI();
	}

	private void sharedProcedureCreatorComponentInverter3_StopServiceComplete(object sender, SingleServiceResultEventArgs e)
	{
		isStopped1 = true;
		AddLogLabel(Resources.Message_Resolver_3_Learn_Routine_Stopped);
		UpdateUI();
	}

	private void digitalReadoutInstrumentInverterLearnResults1_RepresentedStateChanged(object sender, EventArgs e)
	{
		if (!Learning1InProcess && !isStopped1)
		{
			buttonInverter1Start.PerformClick();
		}
		UpdateUI();
	}

	private void digitalReadoutInstrumentInverterLearnResults2_RepresentedStateChanged(object sender, EventArgs e)
	{
		if (!Learning2InProcess && !isStopped2)
		{
			buttonInverter2Start.PerformClick();
		}
		UpdateUI();
	}

	private void digitalReadoutInstrumentInverterLearnResults3_RepresentedStateChanged(object sender, EventArgs e)
	{
		if (!Learning3InProcess && !isStopped3)
		{
			buttonInverter3Start.PerformClick();
		}
		UpdateUI();
	}

	private void InitializeComponent()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected O, but got Unknown
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected O, but got Unknown
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Expected O, but got Unknown
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected O, but got Unknown
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Expected O, but got Unknown
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Expected O, but got Unknown
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Expected O, but got Unknown
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Expected O, but got Unknown
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Expected O, but got Unknown
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Expected O, but got Unknown
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Expected O, but got Unknown
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Expected O, but got Unknown
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Expected O, but got Unknown
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Expected O, but got Unknown
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Expected O, but got Unknown
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Expected O, but got Unknown
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Expected O, but got Unknown
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Expected O, but got Unknown
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Expected O, but got Unknown
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Expected O, but got Unknown
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Expected O, but got Unknown
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Expected O, but got Unknown
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Expected O, but got Unknown
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Expected O, but got Unknown
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Expected O, but got Unknown
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Expected O, but got Unknown
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Expected O, but got Unknown
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Expected O, but got Unknown
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Expected O, but got Unknown
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Expected O, but got Unknown
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Expected O, but got Unknown
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Expected O, but got Unknown
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Expected O, but got Unknown
		//IL_0529: Unknown result type (might be due to invalid IL or missing references)
		//IL_0749: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c38: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e78: Unknown result type (might be due to invalid IL or missing references)
		//IL_1098: Unknown result type (might be due to invalid IL or missing references)
		//IL_13a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_13aa: Expected O, but got Unknown
		//IL_149a: Unknown result type (might be due to invalid IL or missing references)
		//IL_150c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1516: Expected O, but got Unknown
		//IL_1680: Unknown result type (might be due to invalid IL or missing references)
		//IL_1747: Unknown result type (might be due to invalid IL or missing references)
		//IL_17ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_17b6: Expected O, but got Unknown
		//IL_1835: Unknown result type (might be due to invalid IL or missing references)
		//IL_1898: Unknown result type (might be due to invalid IL or missing references)
		//IL_18f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_1997: Unknown result type (might be due to invalid IL or missing references)
		//IL_19fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ac0: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b23: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b7b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c29: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c8e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d53: Unknown result type (might be due to invalid IL or missing references)
		//IL_1db6: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e12: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ec0: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f26: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f80: Unknown result type (might be due to invalid IL or missing references)
		base.components = new Container();
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		DataItemCondition val = new DataItemCondition();
		DataItemCondition val2 = new DataItemCondition();
		DataItemCondition val3 = new DataItemCondition();
		DataItemCondition val4 = new DataItemCondition();
		DataItemCondition val5 = new DataItemCondition();
		DataItemCondition val6 = new DataItemCondition();
		tableLayoutPanel1 = new TableLayoutPanel();
		tableLayoutPanelInverter1LearnRoutine = new TableLayoutPanel();
		buttonInverter1Start = new Button();
		labelStatusLabelInverter1 = new System.Windows.Forms.Label();
		label5 = new System.Windows.Forms.Label();
		checkmarkInverter1 = new Checkmark();
		digitalReadoutInstrument7 = new DigitalReadoutInstrument();
		digitalReadoutInstrumentInverterLearnResults1 = new DigitalReadoutInstrument();
		labelResolver1LearnRoutineHeader = new System.Windows.Forms.Label();
		tableLayoutPanelInverter2LearnRoutine = new TableLayoutPanel();
		buttonInverter2Start = new Button();
		labelStatusLabelInverter2 = new System.Windows.Forms.Label();
		labelStatus2 = new System.Windows.Forms.Label();
		checkmarkInverter2 = new Checkmark();
		digitalReadoutInstrumentMotorSpeed2 = new DigitalReadoutInstrument();
		labelResolver2LearnRoutineHeader = new System.Windows.Forms.Label();
		digitalReadoutInstrumentInverterLearnResults2 = new DigitalReadoutInstrument();
		tableLayoutPanelInverter3LearnRoutine = new TableLayoutPanel();
		buttonInverter3Start = new Button();
		labelStatusLabelInverter3 = new System.Windows.Forms.Label();
		labelStatus3 = new System.Windows.Forms.Label();
		checkmarkInverter3 = new Checkmark();
		digitalReadoutInstrumentMotorSpeed3 = new DigitalReadoutInstrument();
		digitalReadoutInstrumentInverterLearnResults3 = new DigitalReadoutInstrument();
		labelResolver3LearnRoutineHeader = new System.Windows.Forms.Label();
		tableLayoutPanelTop = new TableLayoutPanel();
		webBrowserWarning = new WebBrowser();
		pictureBox1 = new PictureBox();
		tableLayoutPanelBottom = new TableLayoutPanel();
		seekTimeListView1 = new SeekTimeListView();
		sharedProcedureSelectionInverter3 = new SharedProcedureSelection();
		digitalReadoutInstrument4 = new DigitalReadoutInstrument();
		sharedProcedureSelectionInverter2 = new SharedProcedureSelection();
		digitalReadoutInstrument5 = new DigitalReadoutInstrument();
		digitalReadoutInstrument6 = new DigitalReadoutInstrument();
		sharedProcedureSelectionInverter1 = new SharedProcedureSelection();
		sharedProcedureIntegrationComponentInverter1 = new SharedProcedureIntegrationComponent(base.components);
		sharedProcedureCreatorComponentInverter1 = new SharedProcedureCreatorComponent(base.components);
		sharedProcedureIntegrationComponentInverter2 = new SharedProcedureIntegrationComponent(base.components);
		sharedProcedureCreatorComponentInverter2 = new SharedProcedureCreatorComponent(base.components);
		sharedProcedureIntegrationComponentInverter3 = new SharedProcedureIntegrationComponent(base.components);
		sharedProcedureCreatorComponentInverter3 = new SharedProcedureCreatorComponent(base.components);
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)(object)tableLayoutPanelInverter1LearnRoutine).SuspendLayout();
		((Control)(object)tableLayoutPanelInverter2LearnRoutine).SuspendLayout();
		((Control)(object)tableLayoutPanelInverter3LearnRoutine).SuspendLayout();
		((Control)(object)tableLayoutPanelTop).SuspendLayout();
		((ISupportInitialize)pictureBox1).BeginInit();
		((Control)(object)tableLayoutPanelBottom).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)tableLayoutPanelInverter1LearnRoutine, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)tableLayoutPanelInverter2LearnRoutine, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)tableLayoutPanelInverter3LearnRoutine, 0, 5);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)tableLayoutPanelTop, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)tableLayoutPanelBottom, 0, 7);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		componentResourceManager.ApplyResources(tableLayoutPanelInverter1LearnRoutine, "tableLayoutPanelInverter1LearnRoutine");
		((TableLayoutPanel)(object)tableLayoutPanelInverter1LearnRoutine).Controls.Add(buttonInverter1Start, 4, 2);
		((TableLayoutPanel)(object)tableLayoutPanelInverter1LearnRoutine).Controls.Add(labelStatusLabelInverter1, 2, 2);
		((TableLayoutPanel)(object)tableLayoutPanelInverter1LearnRoutine).Controls.Add(label5, 1, 2);
		((TableLayoutPanel)(object)tableLayoutPanelInverter1LearnRoutine).Controls.Add((Control)(object)checkmarkInverter1, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanelInverter1LearnRoutine).Controls.Add((Control)(object)digitalReadoutInstrument7, 2, 1);
		((TableLayoutPanel)(object)tableLayoutPanelInverter1LearnRoutine).Controls.Add((Control)(object)digitalReadoutInstrumentInverterLearnResults1, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanelInverter1LearnRoutine).Controls.Add(labelResolver1LearnRoutineHeader, 0, 0);
		((Control)(object)tableLayoutPanelInverter1LearnRoutine).Name = "tableLayoutPanelInverter1LearnRoutine";
		componentResourceManager.ApplyResources(buttonInverter1Start, "buttonInverter1Start");
		buttonInverter1Start.Name = "buttonInverter1Start";
		buttonInverter1Start.UseCompatibleTextRendering = true;
		buttonInverter1Start.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(labelStatusLabelInverter1, "labelStatusLabelInverter1");
		((TableLayoutPanel)(object)tableLayoutPanelInverter1LearnRoutine).SetColumnSpan((Control)labelStatusLabelInverter1, 2);
		labelStatusLabelInverter1.Name = "labelStatusLabelInverter1";
		labelStatusLabelInverter1.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(label5, "label5");
		label5.Name = "label5";
		label5.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(checkmarkInverter1, "checkmarkInverter1");
		((Control)(object)checkmarkInverter1).Name = "checkmarkInverter1";
		((TableLayoutPanel)(object)tableLayoutPanelInverter1LearnRoutine).SetColumnSpan((Control)(object)digitalReadoutInstrument7, 2);
		componentResourceManager.ApplyResources(digitalReadoutInstrument7, "digitalReadoutInstrument7");
		digitalReadoutInstrument7.FontGroup = "AllwaysDisplayedInstruments";
		((SingleInstrumentBase)digitalReadoutInstrument7).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument7).Instrument = new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS124_Actual_E_Motor_Speed_E_Motor_1_Actual_E_Motor_Speed_E_Motor_1");
		((Control)(object)digitalReadoutInstrument7).Name = "digitalReadoutInstrument7";
		((SingleInstrumentBase)digitalReadoutInstrument7).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanelInverter1LearnRoutine).SetColumnSpan((Control)(object)digitalReadoutInstrumentInverterLearnResults1, 3);
		componentResourceManager.ApplyResources(digitalReadoutInstrumentInverterLearnResults1, "digitalReadoutInstrumentInverterLearnResults1");
		digitalReadoutInstrumentInverterLearnResults1.FontGroup = "AllwaysDisplayedInstruments";
		((SingleInstrumentBase)digitalReadoutInstrumentInverterLearnResults1).FreezeValue = false;
		digitalReadoutInstrumentInverterLearnResults1.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText"));
		digitalReadoutInstrumentInverterLearnResults1.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText1"));
		digitalReadoutInstrumentInverterLearnResults1.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText2"));
		digitalReadoutInstrumentInverterLearnResults1.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText3"));
		digitalReadoutInstrumentInverterLearnResults1.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText4"));
		digitalReadoutInstrumentInverterLearnResults1.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText5"));
		digitalReadoutInstrumentInverterLearnResults1.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText6"));
		digitalReadoutInstrumentInverterLearnResults1.Gradient.Initialize((ValueState)0, 6);
		digitalReadoutInstrumentInverterLearnResults1.Gradient.Modify(1, 0.0, (ValueState)5);
		digitalReadoutInstrumentInverterLearnResults1.Gradient.Modify(2, 1.0, (ValueState)2);
		digitalReadoutInstrumentInverterLearnResults1.Gradient.Modify(3, 2.0, (ValueState)1);
		digitalReadoutInstrumentInverterLearnResults1.Gradient.Modify(4, 3.0, (ValueState)3);
		digitalReadoutInstrumentInverterLearnResults1.Gradient.Modify(5, 4.0, (ValueState)0);
		digitalReadoutInstrumentInverterLearnResults1.Gradient.Modify(6, 255.0, (ValueState)0);
		((SingleInstrumentBase)digitalReadoutInstrumentInverterLearnResults1).Instrument = new Qualifier((QualifierTypes)64, "ECPC01T", "RT_TI_ResolverTeach_In_Request_Results_Resolver_tech_in_PT1");
		((Control)(object)digitalReadoutInstrumentInverterLearnResults1).Name = "digitalReadoutInstrumentInverterLearnResults1";
		((SingleInstrumentBase)digitalReadoutInstrumentInverterLearnResults1).ShowValueReadout = false;
		((SingleInstrumentBase)digitalReadoutInstrumentInverterLearnResults1).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(labelResolver1LearnRoutineHeader, "labelResolver1LearnRoutineHeader");
		labelResolver1LearnRoutineHeader.BorderStyle = BorderStyle.FixedSingle;
		((TableLayoutPanel)(object)tableLayoutPanelInverter1LearnRoutine).SetColumnSpan((Control)labelResolver1LearnRoutineHeader, 5);
		labelResolver1LearnRoutineHeader.Name = "labelResolver1LearnRoutineHeader";
		labelResolver1LearnRoutineHeader.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(tableLayoutPanelInverter2LearnRoutine, "tableLayoutPanelInverter2LearnRoutine");
		((TableLayoutPanel)(object)tableLayoutPanelInverter2LearnRoutine).Controls.Add(buttonInverter2Start, 4, 2);
		((TableLayoutPanel)(object)tableLayoutPanelInverter2LearnRoutine).Controls.Add(labelStatusLabelInverter2, 2, 2);
		((TableLayoutPanel)(object)tableLayoutPanelInverter2LearnRoutine).Controls.Add(labelStatus2, 1, 2);
		((TableLayoutPanel)(object)tableLayoutPanelInverter2LearnRoutine).Controls.Add((Control)(object)checkmarkInverter2, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanelInverter2LearnRoutine).Controls.Add((Control)(object)digitalReadoutInstrumentMotorSpeed2, 2, 1);
		((TableLayoutPanel)(object)tableLayoutPanelInverter2LearnRoutine).Controls.Add(labelResolver2LearnRoutineHeader, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelInverter2LearnRoutine).Controls.Add((Control)(object)digitalReadoutInstrumentInverterLearnResults2, 0, 1);
		((Control)(object)tableLayoutPanelInverter2LearnRoutine).Name = "tableLayoutPanelInverter2LearnRoutine";
		componentResourceManager.ApplyResources(buttonInverter2Start, "buttonInverter2Start");
		buttonInverter2Start.Name = "buttonInverter2Start";
		buttonInverter2Start.UseCompatibleTextRendering = true;
		buttonInverter2Start.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(labelStatusLabelInverter2, "labelStatusLabelInverter2");
		((TableLayoutPanel)(object)tableLayoutPanelInverter2LearnRoutine).SetColumnSpan((Control)labelStatusLabelInverter2, 2);
		labelStatusLabelInverter2.Name = "labelStatusLabelInverter2";
		labelStatusLabelInverter2.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(labelStatus2, "labelStatus2");
		labelStatus2.Name = "labelStatus2";
		labelStatus2.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(checkmarkInverter2, "checkmarkInverter2");
		((Control)(object)checkmarkInverter2).Name = "checkmarkInverter2";
		((TableLayoutPanel)(object)tableLayoutPanelInverter2LearnRoutine).SetColumnSpan((Control)(object)digitalReadoutInstrumentMotorSpeed2, 2);
		componentResourceManager.ApplyResources(digitalReadoutInstrumentMotorSpeed2, "digitalReadoutInstrumentMotorSpeed2");
		digitalReadoutInstrumentMotorSpeed2.FontGroup = "Instruments";
		((SingleInstrumentBase)digitalReadoutInstrumentMotorSpeed2).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentMotorSpeed2).Instrument = new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS125_Actual_E_Motor_Speed_E_Motor_2_Actual_E_Motor_Speed_E_Motor_2");
		((Control)(object)digitalReadoutInstrumentMotorSpeed2).Name = "digitalReadoutInstrumentMotorSpeed2";
		((SingleInstrumentBase)digitalReadoutInstrumentMotorSpeed2).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(labelResolver2LearnRoutineHeader, "labelResolver2LearnRoutineHeader");
		labelResolver2LearnRoutineHeader.BorderStyle = BorderStyle.FixedSingle;
		((TableLayoutPanel)(object)tableLayoutPanelInverter2LearnRoutine).SetColumnSpan((Control)labelResolver2LearnRoutineHeader, 5);
		labelResolver2LearnRoutineHeader.Name = "labelResolver2LearnRoutineHeader";
		labelResolver2LearnRoutineHeader.UseCompatibleTextRendering = true;
		((TableLayoutPanel)(object)tableLayoutPanelInverter2LearnRoutine).SetColumnSpan((Control)(object)digitalReadoutInstrumentInverterLearnResults2, 3);
		componentResourceManager.ApplyResources(digitalReadoutInstrumentInverterLearnResults2, "digitalReadoutInstrumentInverterLearnResults2");
		digitalReadoutInstrumentInverterLearnResults2.FontGroup = "Instruments";
		((SingleInstrumentBase)digitalReadoutInstrumentInverterLearnResults2).FreezeValue = false;
		digitalReadoutInstrumentInverterLearnResults2.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText7"));
		digitalReadoutInstrumentInverterLearnResults2.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText8"));
		digitalReadoutInstrumentInverterLearnResults2.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText9"));
		digitalReadoutInstrumentInverterLearnResults2.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText10"));
		digitalReadoutInstrumentInverterLearnResults2.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText11"));
		digitalReadoutInstrumentInverterLearnResults2.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText12"));
		digitalReadoutInstrumentInverterLearnResults2.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText13"));
		digitalReadoutInstrumentInverterLearnResults2.Gradient.Initialize((ValueState)0, 6);
		digitalReadoutInstrumentInverterLearnResults2.Gradient.Modify(1, 0.0, (ValueState)5);
		digitalReadoutInstrumentInverterLearnResults2.Gradient.Modify(2, 1.0, (ValueState)2);
		digitalReadoutInstrumentInverterLearnResults2.Gradient.Modify(3, 2.0, (ValueState)1);
		digitalReadoutInstrumentInverterLearnResults2.Gradient.Modify(4, 3.0, (ValueState)3);
		digitalReadoutInstrumentInverterLearnResults2.Gradient.Modify(5, 4.0, (ValueState)0);
		digitalReadoutInstrumentInverterLearnResults2.Gradient.Modify(6, 255.0, (ValueState)0);
		((SingleInstrumentBase)digitalReadoutInstrumentInverterLearnResults2).Instrument = new Qualifier((QualifierTypes)64, "ECPC01T", "RT_TI_ResolverTeach_In_Request_Results_Resolver_tech_in_PT2");
		((Control)(object)digitalReadoutInstrumentInverterLearnResults2).Name = "digitalReadoutInstrumentInverterLearnResults2";
		((SingleInstrumentBase)digitalReadoutInstrumentInverterLearnResults2).ShowValueReadout = false;
		((SingleInstrumentBase)digitalReadoutInstrumentInverterLearnResults2).UnitAlignment = StringAlignment.Near;
		((Control)(object)tableLayoutPanelInverter3LearnRoutine).BackColor = SystemColors.Control;
		componentResourceManager.ApplyResources(tableLayoutPanelInverter3LearnRoutine, "tableLayoutPanelInverter3LearnRoutine");
		((TableLayoutPanel)(object)tableLayoutPanelInverter3LearnRoutine).Controls.Add(buttonInverter3Start, 4, 2);
		((TableLayoutPanel)(object)tableLayoutPanelInverter3LearnRoutine).Controls.Add(labelStatusLabelInverter3, 2, 2);
		((TableLayoutPanel)(object)tableLayoutPanelInverter3LearnRoutine).Controls.Add(labelStatus3, 1, 2);
		((TableLayoutPanel)(object)tableLayoutPanelInverter3LearnRoutine).Controls.Add((Control)(object)checkmarkInverter3, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanelInverter3LearnRoutine).Controls.Add((Control)(object)digitalReadoutInstrumentMotorSpeed3, 3, 1);
		((TableLayoutPanel)(object)tableLayoutPanelInverter3LearnRoutine).Controls.Add((Control)(object)digitalReadoutInstrumentInverterLearnResults3, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanelInverter3LearnRoutine).Controls.Add(labelResolver3LearnRoutineHeader, 0, 0);
		((Control)(object)tableLayoutPanelInverter3LearnRoutine).Name = "tableLayoutPanelInverter3LearnRoutine";
		componentResourceManager.ApplyResources(buttonInverter3Start, "buttonInverter3Start");
		buttonInverter3Start.Name = "buttonInverter3Start";
		buttonInverter3Start.UseCompatibleTextRendering = true;
		buttonInverter3Start.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(labelStatusLabelInverter3, "labelStatusLabelInverter3");
		((TableLayoutPanel)(object)tableLayoutPanelInverter3LearnRoutine).SetColumnSpan((Control)labelStatusLabelInverter3, 2);
		labelStatusLabelInverter3.Name = "labelStatusLabelInverter3";
		labelStatusLabelInverter3.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(labelStatus3, "labelStatus3");
		labelStatus3.Name = "labelStatus3";
		labelStatus3.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(checkmarkInverter3, "checkmarkInverter3");
		((Control)(object)checkmarkInverter3).Name = "checkmarkInverter3";
		((TableLayoutPanel)(object)tableLayoutPanelInverter3LearnRoutine).SetColumnSpan((Control)(object)digitalReadoutInstrumentMotorSpeed3, 2);
		componentResourceManager.ApplyResources(digitalReadoutInstrumentMotorSpeed3, "digitalReadoutInstrumentMotorSpeed3");
		digitalReadoutInstrumentMotorSpeed3.FontGroup = "Instruments";
		((SingleInstrumentBase)digitalReadoutInstrumentMotorSpeed3).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentMotorSpeed3).Instrument = new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS126_Actual_E_Motor_Speed_E_Motor_3_Actual_E_Motor_Speed_E_Motor_3");
		((Control)(object)digitalReadoutInstrumentMotorSpeed3).Name = "digitalReadoutInstrumentMotorSpeed3";
		((SingleInstrumentBase)digitalReadoutInstrumentMotorSpeed3).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanelInverter3LearnRoutine).SetColumnSpan((Control)(object)digitalReadoutInstrumentInverterLearnResults3, 3);
		componentResourceManager.ApplyResources(digitalReadoutInstrumentInverterLearnResults3, "digitalReadoutInstrumentInverterLearnResults3");
		digitalReadoutInstrumentInverterLearnResults3.FontGroup = "Instruments";
		((SingleInstrumentBase)digitalReadoutInstrumentInverterLearnResults3).FreezeValue = false;
		digitalReadoutInstrumentInverterLearnResults3.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText14"));
		digitalReadoutInstrumentInverterLearnResults3.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText15"));
		digitalReadoutInstrumentInverterLearnResults3.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText16"));
		digitalReadoutInstrumentInverterLearnResults3.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText17"));
		digitalReadoutInstrumentInverterLearnResults3.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText18"));
		digitalReadoutInstrumentInverterLearnResults3.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText19"));
		digitalReadoutInstrumentInverterLearnResults3.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText20"));
		digitalReadoutInstrumentInverterLearnResults3.Gradient.Initialize((ValueState)0, 6);
		digitalReadoutInstrumentInverterLearnResults3.Gradient.Modify(1, 0.0, (ValueState)5);
		digitalReadoutInstrumentInverterLearnResults3.Gradient.Modify(2, 1.0, (ValueState)2);
		digitalReadoutInstrumentInverterLearnResults3.Gradient.Modify(3, 2.0, (ValueState)1);
		digitalReadoutInstrumentInverterLearnResults3.Gradient.Modify(4, 3.0, (ValueState)3);
		digitalReadoutInstrumentInverterLearnResults3.Gradient.Modify(5, 4.0, (ValueState)0);
		digitalReadoutInstrumentInverterLearnResults3.Gradient.Modify(6, 255.0, (ValueState)0);
		((SingleInstrumentBase)digitalReadoutInstrumentInverterLearnResults3).Instrument = new Qualifier((QualifierTypes)64, "ECPC01T", "RT_TI_ResolverTeach_In_Request_Results_Resolver_tech_in_PT3");
		((Control)(object)digitalReadoutInstrumentInverterLearnResults3).Name = "digitalReadoutInstrumentInverterLearnResults3";
		((SingleInstrumentBase)digitalReadoutInstrumentInverterLearnResults3).ShowValueReadout = false;
		((SingleInstrumentBase)digitalReadoutInstrumentInverterLearnResults3).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(labelResolver3LearnRoutineHeader, "labelResolver3LearnRoutineHeader");
		labelResolver3LearnRoutineHeader.BorderStyle = BorderStyle.FixedSingle;
		((TableLayoutPanel)(object)tableLayoutPanelInverter3LearnRoutine).SetColumnSpan((Control)labelResolver3LearnRoutineHeader, 5);
		labelResolver3LearnRoutineHeader.Name = "labelResolver3LearnRoutineHeader";
		labelResolver3LearnRoutineHeader.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(tableLayoutPanelTop, "tableLayoutPanelTop");
		((TableLayoutPanel)(object)tableLayoutPanelTop).Controls.Add(webBrowserWarning, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanelTop).Controls.Add(pictureBox1, 0, 0);
		((Control)(object)tableLayoutPanelTop).Name = "tableLayoutPanelTop";
		componentResourceManager.ApplyResources(webBrowserWarning, "webBrowserWarning");
		webBrowserWarning.Name = "webBrowserWarning";
		webBrowserWarning.Url = new Uri("about: blank", UriKind.Absolute);
		pictureBox1.BackColor = Color.White;
		componentResourceManager.ApplyResources(pictureBox1, "pictureBox1");
		pictureBox1.Name = "pictureBox1";
		pictureBox1.TabStop = false;
		componentResourceManager.ApplyResources(tableLayoutPanelBottom, "tableLayoutPanelBottom");
		((TableLayoutPanel)(object)tableLayoutPanelBottom).Controls.Add((Control)(object)seekTimeListView1, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanelBottom).Controls.Add((Control)(object)sharedProcedureSelectionInverter3, 1, 3);
		((TableLayoutPanel)(object)tableLayoutPanelBottom).Controls.Add((Control)(object)digitalReadoutInstrument4, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanelBottom).Controls.Add((Control)(object)sharedProcedureSelectionInverter2, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanelBottom).Controls.Add((Control)(object)digitalReadoutInstrument5, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanelBottom).Controls.Add((Control)(object)digitalReadoutInstrument6, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanelBottom).Controls.Add((Control)(object)sharedProcedureSelectionInverter1, 2, 3);
		((Control)(object)tableLayoutPanelBottom).Name = "tableLayoutPanelBottom";
		((TableLayoutPanel)(object)tableLayoutPanelBottom).SetColumnSpan((Control)(object)seekTimeListView1, 3);
		componentResourceManager.ApplyResources(seekTimeListView1, "seekTimeListView1");
		seekTimeListView1.FilterUserLabels = true;
		((Control)(object)seekTimeListView1).Name = "seekTimeListView1";
		seekTimeListView1.RequiredUserLabelPrefix = "InverterResolverLearn";
		((TableLayoutPanel)(object)tableLayoutPanelBottom).SetRowSpan((Control)(object)seekTimeListView1, 3);
		seekTimeListView1.SelectedTime = null;
		seekTimeListView1.ShowChannelLabels = false;
		seekTimeListView1.ShowCommunicationsState = false;
		seekTimeListView1.ShowControlPanel = false;
		componentResourceManager.ApplyResources(sharedProcedureSelectionInverter3, "sharedProcedureSelectionInverter3");
		((Control)(object)sharedProcedureSelectionInverter3).Name = "sharedProcedureSelectionInverter3";
		sharedProcedureSelectionInverter3.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>)new string[1] { "ResolverTeach_Inverter3" });
		componentResourceManager.ApplyResources(digitalReadoutInstrument4, "digitalReadoutInstrument4");
		digitalReadoutInstrument4.FontGroup = "AllwaysDisplayedInstruments";
		((SingleInstrumentBase)digitalReadoutInstrument4).FreezeValue = false;
		digitalReadoutInstrument4.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText21"));
		digitalReadoutInstrument4.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText22"));
		digitalReadoutInstrument4.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText23"));
		digitalReadoutInstrument4.Gradient.Initialize((ValueState)0, 2);
		digitalReadoutInstrument4.Gradient.Modify(1, 0.0, (ValueState)3);
		digitalReadoutInstrument4.Gradient.Modify(2, 1.0, (ValueState)1);
		((SingleInstrumentBase)digitalReadoutInstrument4).Instrument = new Qualifier((QualifierTypes)1, "ETCM01T", "DT_Environmental_Conditions_RoutineControl_transmission_in_neutral");
		((Control)(object)digitalReadoutInstrument4).Name = "digitalReadoutInstrument4";
		((SingleInstrumentBase)digitalReadoutInstrument4).ShowValueReadout = false;
		((SingleInstrumentBase)digitalReadoutInstrument4).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(sharedProcedureSelectionInverter2, "sharedProcedureSelectionInverter2");
		((Control)(object)sharedProcedureSelectionInverter2).Name = "sharedProcedureSelectionInverter2";
		sharedProcedureSelectionInverter2.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>)new string[1] { "ResolverTeach_Inverter2" });
		componentResourceManager.ApplyResources(digitalReadoutInstrument5, "digitalReadoutInstrument5");
		digitalReadoutInstrument5.FontGroup = "AllwaysDisplayedInstruments";
		((SingleInstrumentBase)digitalReadoutInstrument5).FreezeValue = false;
		digitalReadoutInstrument5.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText24"));
		digitalReadoutInstrument5.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText25"));
		digitalReadoutInstrument5.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText26"));
		digitalReadoutInstrument5.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText27"));
		digitalReadoutInstrument5.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText28"));
		digitalReadoutInstrument5.Gradient.Initialize((ValueState)0, 4);
		digitalReadoutInstrument5.Gradient.Modify(1, 0.0, (ValueState)3);
		digitalReadoutInstrument5.Gradient.Modify(2, 1.0, (ValueState)1);
		digitalReadoutInstrument5.Gradient.Modify(3, 2.0, (ValueState)0);
		digitalReadoutInstrument5.Gradient.Modify(4, 3.0, (ValueState)0);
		((SingleInstrumentBase)digitalReadoutInstrument5).Instrument = new Qualifier((QualifierTypes)1, "SSAM02T", "DT_BSC_Diagnostic_Displayables_DDBSC_PkBk_Master_Stat");
		((Control)(object)digitalReadoutInstrument5).Name = "digitalReadoutInstrument5";
		((SingleInstrumentBase)digitalReadoutInstrument5).ShowValueReadout = false;
		((SingleInstrumentBase)digitalReadoutInstrument5).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument6, "digitalReadoutInstrument6");
		digitalReadoutInstrument6.FontGroup = "AllwaysDisplayedInstruments";
		((SingleInstrumentBase)digitalReadoutInstrument6).FreezeValue = false;
		digitalReadoutInstrument6.Gradient.Initialize((ValueState)3, 2, "mph");
		digitalReadoutInstrument6.Gradient.Modify(1, 0.0, (ValueState)1);
		digitalReadoutInstrument6.Gradient.Modify(2, 1.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrument6).Instrument = new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS012_CurrentVehicleSpeed_CurrentVehicleSpeed");
		((Control)(object)digitalReadoutInstrument6).Name = "digitalReadoutInstrument6";
		((SingleInstrumentBase)digitalReadoutInstrument6).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(sharedProcedureSelectionInverter1, "sharedProcedureSelectionInverter1");
		((Control)(object)sharedProcedureSelectionInverter1).Name = "sharedProcedureSelectionInverter1";
		sharedProcedureSelectionInverter1.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>)new string[1] { "ResolverTeach_Inverter1" });
		sharedProcedureIntegrationComponentInverter1.ProceduresDropDown = sharedProcedureSelectionInverter1;
		sharedProcedureIntegrationComponentInverter1.ProcedureStatusMessageTarget = labelStatusLabelInverter1;
		sharedProcedureIntegrationComponentInverter1.ProcedureStatusStateTarget = checkmarkInverter1;
		sharedProcedureIntegrationComponentInverter1.ResultsTarget = null;
		sharedProcedureIntegrationComponentInverter1.StartStopButton = buttonInverter1Start;
		sharedProcedureIntegrationComponentInverter1.StopAllButton = null;
		sharedProcedureCreatorComponentInverter1.Suspend();
		sharedProcedureCreatorComponentInverter1.MonitorCall = new ServiceCall("ECPC01T", "RT_TI_ResolverTeach_In_Request_Results_Resolver_tech_in_PT1");
		componentResourceManager.ApplyResources(sharedProcedureCreatorComponentInverter1, "sharedProcedureCreatorComponentInverter1");
		sharedProcedureCreatorComponentInverter1.Qualifier = "ResolverTeach_Inverter1";
		sharedProcedureCreatorComponentInverter1.StartCall = new ServiceCall("ECPC01T", "RT_TI_ResolverTeach_In_Start_Resolver_tech_in_PT1", (IEnumerable<string>)new string[3] { "Resolver_teach_in_Var1=1", "Resolver_teach_in_Var2=0", "Resolver_teach_in_Var3=0" });
		val.Gradient.Initialize((ValueState)3, 2, "mph");
		val.Gradient.Modify(1, 0.0, (ValueState)1);
		val.Gradient.Modify(2, 1.0, (ValueState)3);
		val.Qualifier = new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS012_CurrentVehicleSpeed_CurrentVehicleSpeed");
		val2.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText29"));
		val2.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText30"));
		val2.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText31"));
		val2.Gradient.Initialize((ValueState)0, 2);
		val2.Gradient.Modify(1, 0.0, (ValueState)3);
		val2.Gradient.Modify(2, 1.0, (ValueState)1);
		val2.Qualifier = new Qualifier((QualifierTypes)1, "ETCM01T", "DT_Environmental_Conditions_RoutineControl_transmission_in_neutral");
		sharedProcedureCreatorComponentInverter1.StartConditions.Add(val);
		sharedProcedureCreatorComponentInverter1.StartConditions.Add(val2);
		sharedProcedureCreatorComponentInverter1.StopCall = new ServiceCall("ECPC01T", "RT_TI_ResolverTeach_In_Stop_Resolver_tech_in_PT1", (IEnumerable<string>)new string[3] { "Resolver_teach_in_Var1=1", "Resolver_teach_in_Var2=0", "Resolver_teach_in_Var3=0" });
		sharedProcedureCreatorComponentInverter1.StartServiceComplete += sharedProcedureCreatorComponentInverter1_StartServiceComplete;
		sharedProcedureCreatorComponentInverter1.StopServiceComplete += sharedProcedureCreatorComponentInverter1_StopServiceComplete;
		sharedProcedureCreatorComponentInverter1.Resume();
		sharedProcedureIntegrationComponentInverter2.ProceduresDropDown = sharedProcedureSelectionInverter2;
		sharedProcedureIntegrationComponentInverter2.ProcedureStatusMessageTarget = labelStatusLabelInverter2;
		sharedProcedureIntegrationComponentInverter2.ProcedureStatusStateTarget = checkmarkInverter2;
		sharedProcedureIntegrationComponentInverter2.ResultsTarget = null;
		sharedProcedureIntegrationComponentInverter2.StartStopButton = buttonInverter2Start;
		sharedProcedureIntegrationComponentInverter2.StopAllButton = null;
		sharedProcedureCreatorComponentInverter2.Suspend();
		sharedProcedureCreatorComponentInverter2.MonitorCall = new ServiceCall("ECPC01T", "RT_TI_ResolverTeach_In_Request_Results_Resolver_tech_in_PT2");
		componentResourceManager.ApplyResources(sharedProcedureCreatorComponentInverter2, "sharedProcedureCreatorComponentInverter2");
		sharedProcedureCreatorComponentInverter2.Qualifier = "ResolverTeach_Inverter2";
		sharedProcedureCreatorComponentInverter2.StartCall = new ServiceCall("ECPC01T", "RT_TI_ResolverTeach_In_Start_Resolver_tech_in_PT2", (IEnumerable<string>)new string[3] { "Resolver_teach_in_Var1=0", "Resolver_teach_in_Var2=1", "Resolver_teach_in_Var3=0" });
		val3.Gradient.Initialize((ValueState)3, 2, "mph");
		val3.Gradient.Modify(1, 0.0, (ValueState)1);
		val3.Gradient.Modify(2, 1.0, (ValueState)3);
		val3.Qualifier = new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS012_CurrentVehicleSpeed_CurrentVehicleSpeed");
		val4.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText32"));
		val4.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText33"));
		val4.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText34"));
		val4.Gradient.Initialize((ValueState)0, 2);
		val4.Gradient.Modify(1, 0.0, (ValueState)3);
		val4.Gradient.Modify(2, 1.0, (ValueState)1);
		val4.Qualifier = new Qualifier((QualifierTypes)1, "ETCM01T", "DT_Environmental_Conditions_RoutineControl_transmission_in_neutral");
		sharedProcedureCreatorComponentInverter2.StartConditions.Add(val3);
		sharedProcedureCreatorComponentInverter2.StartConditions.Add(val4);
		sharedProcedureCreatorComponentInverter2.StopCall = new ServiceCall("ECPC01T", "RT_TI_ResolverTeach_In_Stop_Resolver_tech_in_PT2", (IEnumerable<string>)new string[3] { "Resolver_teach_in_Var1=0", "Resolver_teach_in_Var2=1", "Resolver_teach_in_Var3=0" });
		sharedProcedureCreatorComponentInverter2.StartServiceComplete += sharedProcedureCreatorComponentInverter2_StartServiceComplete;
		sharedProcedureCreatorComponentInverter2.StopServiceComplete += sharedProcedureCreatorComponentInverter2_StopServiceComplete;
		sharedProcedureCreatorComponentInverter2.Resume();
		sharedProcedureIntegrationComponentInverter3.ProceduresDropDown = sharedProcedureSelectionInverter3;
		sharedProcedureIntegrationComponentInverter3.ProcedureStatusMessageTarget = labelStatusLabelInverter3;
		sharedProcedureIntegrationComponentInverter3.ProcedureStatusStateTarget = checkmarkInverter3;
		sharedProcedureIntegrationComponentInverter3.ResultsTarget = null;
		sharedProcedureIntegrationComponentInverter3.StartStopButton = buttonInverter3Start;
		sharedProcedureIntegrationComponentInverter3.StopAllButton = null;
		sharedProcedureCreatorComponentInverter3.Suspend();
		sharedProcedureCreatorComponentInverter3.MonitorCall = new ServiceCall("ECPC01T", "RT_TI_ResolverTeach_In_Request_Results_Resolver_tech_in_PT3");
		componentResourceManager.ApplyResources(sharedProcedureCreatorComponentInverter3, "sharedProcedureCreatorComponentInverter3");
		sharedProcedureCreatorComponentInverter3.Qualifier = "ResolverTeach_Inverter3";
		sharedProcedureCreatorComponentInverter3.StartCall = new ServiceCall("ECPC01T", "RT_TI_ResolverTeach_In_Start_Resolver_tech_in_PT3", (IEnumerable<string>)new string[3] { "Resolver_teach_in_Var1=0", "Resolver_teach_in_Var2=0", "Resolver_teach_in_Var3=1" });
		val5.Gradient.Initialize((ValueState)3, 2, "mph");
		val5.Gradient.Modify(1, 0.0, (ValueState)1);
		val5.Gradient.Modify(2, 1.0, (ValueState)3);
		val5.Qualifier = new Qualifier((QualifierTypes)1, "ECPC01T", "DT_AS012_CurrentVehicleSpeed_CurrentVehicleSpeed");
		val6.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText35"));
		val6.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText36"));
		val6.Gradient.DisplayText.Add(componentResourceManager.GetString("resource.DisplayText37"));
		val6.Gradient.Initialize((ValueState)0, 2);
		val6.Gradient.Modify(1, 0.0, (ValueState)3);
		val6.Gradient.Modify(2, 1.0, (ValueState)1);
		val6.Qualifier = new Qualifier((QualifierTypes)1, "ETCM01T", "DT_Environmental_Conditions_RoutineControl_transmission_in_neutral");
		sharedProcedureCreatorComponentInverter3.StartConditions.Add(val5);
		sharedProcedureCreatorComponentInverter3.StartConditions.Add(val6);
		sharedProcedureCreatorComponentInverter3.StopCall = new ServiceCall("ECPC01T", "RT_TI_ResolverTeach_In_Stop_Resolver_tech_in_PT3", (IEnumerable<string>)new string[3] { "Resolver_teach_in_Var1=0", "Resolver_teach_in_Var2=0", "Resolver_teach_in_Var3=1" });
		sharedProcedureCreatorComponentInverter3.StartServiceComplete += sharedProcedureCreatorComponentInverter3_StartServiceComplete;
		sharedProcedureCreatorComponentInverter3.StopServiceComplete += sharedProcedureCreatorComponentInverter3_StopServiceComplete;
		sharedProcedureCreatorComponentInverter3.Resume();
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("_DDDL.chm_Inverter_Resolver_Learn");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelInverter1LearnRoutine).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelInverter1LearnRoutine).PerformLayout();
		((Control)(object)tableLayoutPanelInverter2LearnRoutine).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelInverter2LearnRoutine).PerformLayout();
		((Control)(object)tableLayoutPanelInverter3LearnRoutine).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanelInverter3LearnRoutine).PerformLayout();
		((Control)(object)tableLayoutPanelTop).ResumeLayout(performLayout: false);
		((ISupportInitialize)pictureBox1).EndInit();
		((Control)(object)tableLayoutPanelBottom).ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
