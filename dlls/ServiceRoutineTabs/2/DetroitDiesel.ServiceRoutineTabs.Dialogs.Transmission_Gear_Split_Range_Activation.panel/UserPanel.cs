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
using DetroitDiesel.Windows.Forms;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Transmission_Gear_Split_Range_Activation.panel;

public class UserPanel : CustomPanel
{
	public enum ActivationMode
	{
		Gear,
		Split,
		Range
	}

	public enum SplitValue
	{
		Low,
		High
	}

	public enum RangeValue
	{
		Low,
		High,
		Other
	}

	private const string defaultValue = "0";

	private bool canClose = true;

	private Channel tcm;

	private TableLayoutPanel tableLayoutPanel1;

	private SeekTimeListView seekTimeListView1;

	private DigitalReadoutInstrument digitalReadoutInstrument1;

	private DigitalReadoutInstrument digitalReadoutInstrument2;

	private DigitalReadoutInstrument digitalReadoutInstrument3;

	private DigitalReadoutInstrument rangeActuatorPosition;

	private Checkmark checkmark1;

	private System.Windows.Forms.Label messageTarget;

	private SharedProcedureSelection sharedProcedureSelection1;

	private ComboBox activationComboBoxMode;

	private DigitalReadoutInstrument splitActuatorPosition;

	private SharedProcedureIntegrationComponent sharedProcedureIntegrationComponent1;

	private ComboBox splitRangeEnterBox;

	private DigitalReadoutInstrument digitalReadoutInstrument4;

	private Button startButton;

	private DecimalTextBox gearEnterBox;

	private Panel panel1;

	private DigitalReadoutInstrument gearEngagedDigitalReadout;

	private Button buttonReturnControl;

	private TableLayoutPanel tableLayoutPanel2;

	private RangeValue CurrentRange
	{
		get
		{
			RangeValue result = RangeValue.Other;
			if (((SingleInstrumentBase)rangeActuatorPosition).DataItem != null && ((SingleInstrumentBase)rangeActuatorPosition).DataItem.Value is InstrumentValue instrumentValue)
			{
				Choice choice = instrumentValue.Value as Choice;
				if (choice != null)
				{
					result = choice.RawValue.ToString() switch
					{
						"0" => RangeValue.Low, 
						"2" => RangeValue.High, 
						_ => RangeValue.Other, 
					};
				}
			}
			return result;
		}
	}

	public UserPanel()
	{
		InitializeComponent();
		((Control)(object)gearEnterBox).TextChanged += gearEnterBox_TextChanged;
		splitRangeEnterBox.TextChanged += splitRangeEnterBox_TextChanged;
		sharedProcedureSelection1.StatusReport += sharedProcedureSelection1_StatusReport;
	}

	private void sharedProcedureSelection1_StatusReport(object sender, StatusReportEventArgs e)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Invalid comparison between Unknown and I4
		SharedProcedureSelection val = (SharedProcedureSelection)((sender is SharedProcedureSelection) ? sender : null);
		if (val.SelectedProcedure.CanStart)
		{
			if ((int)val.SelectedProcedure.Result == 1)
			{
				canClose = false;
				if (activationComboBoxMode.SelectedIndex == 0 && string.Compare(((Control)(object)gearEnterBox).Text, "0", StringComparison.OrdinalIgnoreCase) == 0)
				{
					canClose = true;
				}
			}
		}
		else
		{
			canClose = true;
		}
		buttonReturnControl.Enabled = !canClose;
	}

	protected override void OnLoad(EventArgs e)
	{
		((UserControl)this).OnLoad(e);
		((CustomPanel)this).ParentFormClosing += this_ParentFormClosing;
		activationComboBoxMode.SelectedIndex = 0;
		((Control)(object)gearEnterBox).Text = "0";
	}

	public override void OnChannelsChanged()
	{
		((CustomPanel)this).OnChannelsChanged();
		SetTcm(((CustomPanel)this).GetChannel("TCM01T", (ChannelLookupOptions)7));
	}

	private void SetTcm(Channel tcm)
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Expected O, but got Unknown
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Expected O, but got Unknown
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Invalid comparison between Unknown and I4
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Invalid comparison between Unknown and I4
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		if (this.tcm == tcm)
		{
			return;
		}
		this.tcm = tcm;
		if (this.tcm == null)
		{
			return;
		}
		Ecu ecu = null;
		foreach (SingleInstrumentBase item in CustomPanel.GetControlsOfType(((Control)this).Controls, typeof(SingleInstrumentBase)))
		{
			SingleInstrumentBase val = item;
			Qualifier instrument = val.Instrument;
			ecu = SapiManager.GetEcuByName(((Qualifier)(ref instrument)).Ecu);
			if (ecu == null || !(ecu.Identifier == tcm.Ecu.Identifier) || !(ecu.Name != tcm.Ecu.Name))
			{
				continue;
			}
			instrument = val.Instrument;
			QualifierTypes type = ((Qualifier)(ref instrument)).Type;
			string name = tcm.Ecu.Name;
			instrument = val.Instrument;
			val.Instrument = new Qualifier(type, name, ((Qualifier)(ref instrument)).Name);
			if (val.DataItem != null)
			{
				continue;
			}
			QualifierTypes val2 = (QualifierTypes)0;
			instrument = val.Instrument;
			if ((int)((Qualifier)(ref instrument)).Type == 1)
			{
				val2 = (QualifierTypes)8;
			}
			else
			{
				instrument = val.Instrument;
				if ((int)((Qualifier)(ref instrument)).Type == 8)
				{
					val2 = (QualifierTypes)1;
				}
			}
			QualifierTypes val3 = val2;
			string name2 = tcm.Ecu.Name;
			instrument = val.Instrument;
			val.Instrument = new Qualifier(val3, name2, ((Qualifier)(ref instrument)).Name);
		}
		sharedProcedureSelection1.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>)new string[1] { "SP_TCM_Gear_Split_Range_Select_" + this.tcm.Ecu.Name });
	}

	private void SetGearSplitRangeQualifierValue()
	{
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Expected O, but got Unknown
		string text = ((object)sharedProcedureSelection1.SharedProcedureQualifiers).ToString();
		if (text.Contains("("))
		{
			text = text.Substring(0, text.IndexOf("("));
		}
		int num = 0;
		int num2 = 2;
		int num3 = 2;
		switch (activationComboBoxMode.SelectedIndex)
		{
		case 0:
			num = (int)gearEnterBox.Value;
			if (num == 0)
			{
				num3 = (int)CurrentRange;
			}
			break;
		case 1:
			num2 = splitRangeEnterBox.SelectedIndex;
			num3 = (int)CurrentRange;
			break;
		case 2:
			num3 = splitRangeEnterBox.SelectedIndex;
			break;
		default:
			throw new IndexOutOfRangeException();
		}
		text += string.Format(CultureInfo.InvariantCulture, "({0},{1},{2})", num, num2, num3);
		sharedProcedureSelection1.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>)new string[1] { text });
	}

	private void activationComboBoxMode_SelectedIndexChanged(object sender, EventArgs e)
	{
		SetActivationComboBoxModeControl();
	}

	private void SetActivationComboBoxModeControl()
	{
		switch (activationComboBoxMode.SelectedIndex)
		{
		case 0:
			((Control)(object)gearEnterBox).Visible = true;
			splitRangeEnterBox.Visible = false;
			((Control)(object)gearEnterBox).Text = "0";
			break;
		case 1:
			((Control)(object)gearEnterBox).Visible = false;
			splitRangeEnterBox.Visible = true;
			splitRangeEnterBox.Items.Clear();
			splitRangeEnterBox.Items.AddRange(new object[2]
			{
				SplitValue.Low,
				SplitValue.High
			});
			splitRangeEnterBox.SelectedIndex = 0;
			break;
		case 2:
			((Control)(object)gearEnterBox).Visible = false;
			splitRangeEnterBox.Visible = true;
			splitRangeEnterBox.Items.Clear();
			splitRangeEnterBox.Items.AddRange(new object[2]
			{
				RangeValue.Low,
				RangeValue.High
			});
			splitRangeEnterBox.SelectedIndex = 0;
			break;
		default:
			throw new IndexOutOfRangeException();
		}
		SetGearSplitRangeQualifierValue();
	}

	private void gearEnterBox_TextChanged(object sender, EventArgs e)
	{
		SetGearSplitRangeQualifierValue();
	}

	private void splitRangeEnterBox_TextChanged(object sender, EventArgs e)
	{
		SetGearSplitRangeQualifierValue();
	}

	private void this_ParentFormClosing(object sender, FormClosingEventArgs e)
	{
		if (sharedProcedureSelection1.AnyProcedureInProgress)
		{
			e.Cancel = true;
		}
		else if (!canClose)
		{
			e.Cancel = true;
			MessageBox.Show(Resources.Message_CanNotExitUntilControlIsReturnedToTheVehicle, ApplicationInformation.ProductName, MessageBoxButtons.OK, MessageBoxIcon.None);
		}
		if (!e.Cancel)
		{
			((CustomPanel)this).ParentFormClosing -= this_ParentFormClosing;
			((Control)(object)gearEnterBox).TextChanged -= gearEnterBox_TextChanged;
			splitRangeEnterBox.TextChanged -= splitRangeEnterBox_TextChanged;
			sharedProcedureSelection1.StatusReport -= sharedProcedureSelection1_StatusReport;
		}
	}

	private void buttonReturnControl_Click(object sender, EventArgs e)
	{
		activationComboBoxMode.SelectedIndex = 0;
		gearEnterBox.Value = 0.0;
		SetGearSplitRangeQualifierValue();
		sharedProcedureSelection1.StartSelectedProcedure();
	}

	private void gearEnterBox_Leave(object sender, EventArgs e)
	{
		if (string.IsNullOrEmpty(((Control)(object)gearEnterBox).Text))
		{
			((Control)(object)gearEnterBox).Text = "0";
		}
	}

	private void InitializeComponent()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Expected O, but got Unknown
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Expected O, but got Unknown
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Expected O, but got Unknown
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Expected O, but got Unknown
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Expected O, but got Unknown
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Expected O, but got Unknown
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Expected O, but got Unknown
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Expected O, but got Unknown
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Expected O, but got Unknown
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Expected O, but got Unknown
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Expected O, but got Unknown
		//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c1: Expected O, but got Unknown
		//IL_07a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0842: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0958: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a45: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b61: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bfa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c98: Unknown result type (might be due to invalid IL or missing references)
		base.components = new Container();
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel2 = new TableLayoutPanel();
		checkmark1 = new Checkmark();
		messageTarget = new System.Windows.Forms.Label();
		startButton = new Button();
		activationComboBoxMode = new ComboBox();
		panel1 = new Panel();
		gearEnterBox = new DecimalTextBox();
		splitRangeEnterBox = new ComboBox();
		sharedProcedureSelection1 = new SharedProcedureSelection();
		buttonReturnControl = new Button();
		tableLayoutPanel1 = new TableLayoutPanel();
		seekTimeListView1 = new SeekTimeListView();
		splitActuatorPosition = new DigitalReadoutInstrument();
		digitalReadoutInstrument3 = new DigitalReadoutInstrument();
		digitalReadoutInstrument1 = new DigitalReadoutInstrument();
		digitalReadoutInstrument2 = new DigitalReadoutInstrument();
		digitalReadoutInstrument4 = new DigitalReadoutInstrument();
		rangeActuatorPosition = new DigitalReadoutInstrument();
		gearEngagedDigitalReadout = new DigitalReadoutInstrument();
		sharedProcedureIntegrationComponent1 = new SharedProcedureIntegrationComponent(base.components);
		((Control)(object)tableLayoutPanel2).SuspendLayout();
		panel1.SuspendLayout();
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel2, "tableLayoutPanel2");
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)tableLayoutPanel2, 3);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)checkmark1, 1, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(messageTarget, 2, 0);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(startButton, 4, 1);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(activationComboBoxMode, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(panel1, 3, 1);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add((Control)(object)sharedProcedureSelection1, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel2).Controls.Add(buttonReturnControl, 5, 1);
		((Control)(object)tableLayoutPanel2).Name = "tableLayoutPanel2";
		componentResourceManager.ApplyResources(checkmark1, "checkmark1");
		((Control)(object)checkmark1).Name = "checkmark1";
		((TableLayoutPanel)(object)tableLayoutPanel2).SetColumnSpan((Control)messageTarget, 4);
		componentResourceManager.ApplyResources(messageTarget, "messageTarget");
		messageTarget.Name = "messageTarget";
		messageTarget.UseCompatibleTextRendering = true;
		componentResourceManager.ApplyResources(startButton, "startButton");
		startButton.Name = "startButton";
		startButton.UseCompatibleTextRendering = true;
		startButton.UseVisualStyleBackColor = true;
		((TableLayoutPanel)(object)tableLayoutPanel2).SetColumnSpan((Control)activationComboBoxMode, 2);
		componentResourceManager.ApplyResources(activationComboBoxMode, "activationComboBoxMode");
		activationComboBoxMode.DropDownStyle = ComboBoxStyle.DropDownList;
		activationComboBoxMode.FormattingEnabled = true;
		activationComboBoxMode.Items.AddRange(new object[3]
		{
			componentResourceManager.GetString("activationComboBoxMode.Items"),
			componentResourceManager.GetString("activationComboBoxMode.Items1"),
			componentResourceManager.GetString("activationComboBoxMode.Items2")
		});
		activationComboBoxMode.Name = "activationComboBoxMode";
		activationComboBoxMode.SelectedIndexChanged += activationComboBoxMode_SelectedIndexChanged;
		panel1.Controls.Add((Control)(object)gearEnterBox);
		panel1.Controls.Add(splitRangeEnterBox);
		componentResourceManager.ApplyResources(panel1, "panel1");
		panel1.Name = "panel1";
		componentResourceManager.ApplyResources(gearEnterBox, "gearEnterBox");
		gearEnterBox.MaximumValue = 12.0;
		gearEnterBox.MinimumValue = 0.0;
		((Control)(object)gearEnterBox).Name = "gearEnterBox";
		gearEnterBox.Precision = 0;
		gearEnterBox.Value = 0.0;
		((Control)(object)gearEnterBox).Leave += gearEnterBox_Leave;
		componentResourceManager.ApplyResources(splitRangeEnterBox, "splitRangeEnterBox");
		splitRangeEnterBox.DropDownStyle = ComboBoxStyle.DropDownList;
		splitRangeEnterBox.FormattingEnabled = true;
		splitRangeEnterBox.Name = "splitRangeEnterBox";
		componentResourceManager.ApplyResources(sharedProcedureSelection1, "sharedProcedureSelection1");
		((Control)(object)sharedProcedureSelection1).Name = "sharedProcedureSelection1";
		sharedProcedureSelection1.SharedProcedureQualifiers = new SubjectCollection((IEnumerable<string>)new string[2] { "SP_TCM_Gear_Split_Range_Select_TCM01T", "SP_TCM_Gear_Split_Range_Select_TCM05T" });
		componentResourceManager.ApplyResources(buttonReturnControl, "buttonReturnControl");
		buttonReturnControl.Name = "buttonReturnControl";
		buttonReturnControl.UseCompatibleTextRendering = true;
		buttonReturnControl.UseVisualStyleBackColor = true;
		buttonReturnControl.Click += buttonReturnControl_Click;
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)seekTimeListView1, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)tableLayoutPanel2, 0, 4);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)splitActuatorPosition, 2, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument3, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument1, 2, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument2, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument4, 2, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)rangeActuatorPosition, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)gearEngagedDigitalReadout, 1, 1);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)seekTimeListView1, 3);
		componentResourceManager.ApplyResources(seekTimeListView1, "seekTimeListView1");
		seekTimeListView1.FilterUserLabels = true;
		((Control)(object)seekTimeListView1).Name = "seekTimeListView1";
		seekTimeListView1.RequiredUserLabelPrefix = "Gear Split Range Select Activation";
		seekTimeListView1.SelectedTime = null;
		seekTimeListView1.ShowChannelLabels = false;
		seekTimeListView1.ShowCommunicationsState = false;
		seekTimeListView1.ShowControlPanel = false;
		seekTimeListView1.ShowDeviceColumn = false;
		seekTimeListView1.TimeFormat = "HH:mm:ss.f";
		componentResourceManager.ApplyResources(splitActuatorPosition, "splitActuatorPosition");
		splitActuatorPosition.FontGroup = "";
		((SingleInstrumentBase)splitActuatorPosition).FreezeValue = false;
		splitActuatorPosition.Gradient.Initialize((ValueState)0, 5);
		splitActuatorPosition.Gradient.Modify(1, 0.0, (ValueState)1);
		splitActuatorPosition.Gradient.Modify(2, 1.0, (ValueState)0);
		splitActuatorPosition.Gradient.Modify(3, 2.0, (ValueState)1);
		splitActuatorPosition.Gradient.Modify(4, 3.0, (ValueState)3);
		splitActuatorPosition.Gradient.Modify(5, 255.0, (ValueState)3);
		((SingleInstrumentBase)splitActuatorPosition).Instrument = new Qualifier((QualifierTypes)1, "TCM01T", "DT_2306_Aktuator_Stellung_Split_Aktuator_Stellung_Split");
		((Control)(object)splitActuatorPosition).Name = "splitActuatorPosition";
		((SingleInstrumentBase)splitActuatorPosition).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument3, "digitalReadoutInstrument3");
		digitalReadoutInstrument3.FontGroup = "";
		((SingleInstrumentBase)digitalReadoutInstrument3).FreezeValue = false;
		digitalReadoutInstrument3.Gradient.Initialize((ValueState)1, 1);
		digitalReadoutInstrument3.Gradient.Modify(1, 1.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrument3).Instrument = new Qualifier((QualifierTypes)1, "MCM21T", "DT_AS012_Vehicle_Speed");
		((Control)(object)digitalReadoutInstrument3).Name = "digitalReadoutInstrument3";
		((SingleInstrumentBase)digitalReadoutInstrument3).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument1, "digitalReadoutInstrument1");
		digitalReadoutInstrument1.FontGroup = "";
		((SingleInstrumentBase)digitalReadoutInstrument1).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes)8, "TCM01T", "CO_TransType");
		((Control)(object)digitalReadoutInstrument1).Name = "digitalReadoutInstrument1";
		((SingleInstrumentBase)digitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)digitalReadoutInstrument2, 2);
		componentResourceManager.ApplyResources(digitalReadoutInstrument2, "digitalReadoutInstrument2");
		digitalReadoutInstrument2.FontGroup = "";
		((SingleInstrumentBase)digitalReadoutInstrument2).FreezeValue = false;
		digitalReadoutInstrument2.Gradient.Initialize((ValueState)3, 1);
		digitalReadoutInstrument2.Gradient.Modify(1, 655.0, (ValueState)1);
		((SingleInstrumentBase)digitalReadoutInstrument2).Instrument = new Qualifier((QualifierTypes)1, "TCM01T", "DT_2311_Versorgungsdruck_Getriebe_Versorgungsdruck");
		((Control)(object)digitalReadoutInstrument2).Name = "digitalReadoutInstrument2";
		((SingleInstrumentBase)digitalReadoutInstrument2).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrument4, "digitalReadoutInstrument4");
		digitalReadoutInstrument4.FontGroup = "";
		((SingleInstrumentBase)digitalReadoutInstrument4).FreezeValue = false;
		digitalReadoutInstrument4.Gradient.Initialize((ValueState)3, 4);
		digitalReadoutInstrument4.Gradient.Modify(1, 0.0, (ValueState)3);
		digitalReadoutInstrument4.Gradient.Modify(2, 1.0, (ValueState)1);
		digitalReadoutInstrument4.Gradient.Modify(3, 2.0, (ValueState)3);
		digitalReadoutInstrument4.Gradient.Modify(4, 3.0, (ValueState)3);
		((SingleInstrumentBase)digitalReadoutInstrument4).Instrument = new Qualifier((QualifierTypes)1, "virtual", "ParkingBrake");
		((Control)(object)digitalReadoutInstrument4).Name = "digitalReadoutInstrument4";
		((SingleInstrumentBase)digitalReadoutInstrument4).UnitAlignment = StringAlignment.Near;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)rangeActuatorPosition, 2);
		componentResourceManager.ApplyResources(rangeActuatorPosition, "rangeActuatorPosition");
		rangeActuatorPosition.FontGroup = "";
		((SingleInstrumentBase)rangeActuatorPosition).FreezeValue = false;
		rangeActuatorPosition.Gradient.Initialize((ValueState)0, 5);
		rangeActuatorPosition.Gradient.Modify(1, 0.0, (ValueState)1);
		rangeActuatorPosition.Gradient.Modify(2, 1.0, (ValueState)0);
		rangeActuatorPosition.Gradient.Modify(3, 2.0, (ValueState)1);
		rangeActuatorPosition.Gradient.Modify(4, 3.0, (ValueState)3);
		rangeActuatorPosition.Gradient.Modify(5, 255.0, (ValueState)3);
		((SingleInstrumentBase)rangeActuatorPosition).Instrument = new Qualifier((QualifierTypes)1, "TCM01T", "DT_2309_Aktuator_Stellung_Range_Aktuator_Stellung_Range");
		((Control)(object)rangeActuatorPosition).Name = "rangeActuatorPosition";
		((SingleInstrumentBase)rangeActuatorPosition).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(gearEngagedDigitalReadout, "gearEngagedDigitalReadout");
		gearEngagedDigitalReadout.FontGroup = "";
		((SingleInstrumentBase)gearEngagedDigitalReadout).FreezeValue = false;
		gearEngagedDigitalReadout.Gradient.Initialize((ValueState)1, 1);
		gearEngagedDigitalReadout.Gradient.Modify(1, 1.0, (ValueState)0);
		((SingleInstrumentBase)gearEngagedDigitalReadout).Instrument = new Qualifier((QualifierTypes)1, "TCM01T", "DT_msd08_Istgang_Istgang");
		((Control)(object)gearEngagedDigitalReadout).Name = "gearEngagedDigitalReadout";
		((SingleInstrumentBase)gearEngagedDigitalReadout).UnitAlignment = StringAlignment.Near;
		sharedProcedureIntegrationComponent1.ProceduresDropDown = sharedProcedureSelection1;
		sharedProcedureIntegrationComponent1.ProcedureStatusMessageTarget = messageTarget;
		sharedProcedureIntegrationComponent1.ProcedureStatusStateTarget = checkmark1;
		sharedProcedureIntegrationComponent1.ResultsTarget = null;
		sharedProcedureIntegrationComponent1.StartStopButton = startButton;
		sharedProcedureIntegrationComponent1.StopAllButton = null;
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("Panel_TransmissionGearSplitRangeActivation");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel2).ResumeLayout(performLayout: false);
		panel1.ResumeLayout(performLayout: false);
		panel1.PerformLayout();
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
