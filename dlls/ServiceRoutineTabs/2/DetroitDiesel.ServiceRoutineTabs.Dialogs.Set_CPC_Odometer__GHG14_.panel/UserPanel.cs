using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Help;
using DetroitDiesel.UnitConversion;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Set_CPC_Odometer__GHG14_.panel;

public class UserPanel : CustomPanel
{
	private enum OdometerSetState
	{
		NotSet,
		Set,
		Error
	}

	private const string CpcName = "CPC04T";

	private const string SetOdometerRoutine = "DL_ID_Odometer";

	private const string KeyOffOnResetFunction = "FN_KeyOffOnReset";

	private const string OdometerQualifier = "CO_Odometer";

	private const string VersionQualifier = "CO_SoftwareVersion";

	private Tuple<string, bool>[] ProhibitedVersions = new Tuple<string, bool>[2]
	{
		Tuple.Create("R34_00_000A", item2: true),
		Tuple.Create("R36_00_000A", item2: false)
	};

	private EcuInfo odometerInfo;

	private Channel cpc;

	private Conversion valueToDisplay;

	private Conversion displayToValue;

	private OdometerSetState odometerSetState = OdometerSetState.NotSet;

	private string errorString;

	private double PreviousOdometerValue = -1.0;

	private TableLayoutPanel tableLayoutPanel1;

	private DigitalReadoutInstrument digitalReadoutInstrument1;

	private Checkmark checkmark1;

	private Button buttonSet;

	private Button buttonClose;

	private TextBox textBox1;

	private System.Windows.Forms.Label labelCondition;

	private bool Online => cpc != null && cpc.Online;

	private bool EcuIsBusy => Online && cpc.CommunicationsState != CommunicationsState.Online;

	private double? OdometerValue
	{
		get
		{
			if (odometerInfo != null)
			{
				return double.Parse(odometerInfo.Value, CultureInfo.CurrentCulture);
			}
			return null;
		}
	}

	public UserPanel()
	{
		InitializeComponent();
	}

	protected override void OnLoad(EventArgs e)
	{
		((ContainerControl)this).ParentForm.FormClosing += OnFormClosing;
		textBox1.TextChanged += TextBox1TextChanged;
		textBox1.KeyPress += TextBox1KeyPress;
		((UserControl)this).OnLoad(e);
		UpdateChannels();
		if (CheckProhibited())
		{
			((ContainerControl)this).ParentForm.Close();
		}
		UpdateUserInterface();
	}

	private bool CheckProhibited()
	{
		if (cpc != null && cpc.EcuInfos["CO_SoftwareVersion"] != null)
		{
			string value = cpc.EcuInfos["CO_SoftwareVersion"].Value;
			Tuple<string, bool>[] prohibitedVersions = ProhibitedVersions;
			foreach (Tuple<string, bool> tuple in prohibitedVersions)
			{
				if (string.Compare(value, tuple.Item1, StringComparison.OrdinalIgnoreCase) == 0)
				{
					if (tuple.Item2)
					{
						MessageBox.Show(string.Format(Resources.MessageFormat_UnsupportedSoftwareVersion, cpc.Ecu.Name), Resources.Message_UnsupportedSoftwareVersionTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
					}
					else
					{
						MessageBox.Show(string.Format(Resources.MessageFormat_UnsupportedSoftwareVersionNoRepair, cpc.Ecu.Name), Resources.Message_UnsupportedSoftwareVersionTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
					}
					return true;
				}
			}
		}
		return false;
	}

	private void OnFormClosing(object sender, FormClosingEventArgs e)
	{
		if (!e.Cancel && ((ContainerControl)this).ParentForm != null)
		{
			((ContainerControl)this).ParentForm.FormClosing -= OnFormClosing;
		}
	}

	public override void OnChannelsChanged()
	{
		UpdateChannels();
	}

	private void UpdateChannels()
	{
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		Channel channel = ((CustomPanel)this).GetChannel("CPC04T");
		if (cpc == channel)
		{
			return;
		}
		if (cpc != null)
		{
			cpc.CommunicationsStateUpdateEvent -= OnCommunicationsStateUpdate;
			if (odometerInfo != null)
			{
				odometerInfo.EcuInfoUpdateEvent -= OdometerEcuInfoUpdateEvent;
			}
		}
		cpc = channel;
		if (cpc == null)
		{
			return;
		}
		cpc.CommunicationsStateUpdateEvent += OnCommunicationsStateUpdate;
		odometerInfo = channel.EcuInfos["CO_Odometer"];
		if (odometerInfo != null)
		{
			valueToDisplay = Converter.GlobalInstance.GetConversion(odometerInfo.Units.ToString(), Converter.GlobalInstance.SelectedUnitSystem);
			displayToValue = Converter.GlobalInstance.GetConversion(valueToDisplay.OutputUnit, odometerInfo.Units.ToString());
			odometerInfo.EcuInfoUpdateEvent += OdometerEcuInfoUpdateEvent;
			if (OdometerValue.HasValue)
			{
				textBox1.Text = valueToDisplay.Convert((object)OdometerValue).ToString(CultureInfo.CurrentCulture);
			}
			UpdateUserInterface();
		}
	}

	private void OdometerEcuInfoUpdateEvent(object sender, ResultEventArgs e)
	{
		if (!OdometerValue.HasValue || !OdometerValuesAreEqual(PreviousOdometerValue, OdometerValue.Value))
		{
			PreviousOdometerValue = (OdometerValue.HasValue ? OdometerValue.Value : (-1.0));
			odometerSetState = OdometerSetState.NotSet;
			UpdateUserInterface();
		}
	}

	private void TextBox1ValueChanged(object sender, EventArgs e)
	{
		odometerSetState = OdometerSetState.NotSet;
		UpdateUserInterface();
	}

	private void buttonSet_Click(object sender, EventArgs e)
	{
		if (DialogResult.OK == MessageBox.Show(Resources.Message_DDECReportsLifeToDateDataWillBeResetClickOkToContinueOrClickCancelToAbortChangesAndExit, Application.ProductName, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2))
		{
			SetOdometerEcuInfoValue();
		}
	}

	private void SetOdometerEcuInfoValue()
	{
		Service service = cpc.Services["DL_ID_Odometer"];
		if (service != null)
		{
			service.InputValues[0].Value = displayToValue.Convert((object)textBox1.Text);
			service.ServiceCompleteEvent += SetOdometerServiceCompleteEvent;
			service.Execute(synchronous: false);
		}
	}

	private void SetOdometerServiceCompleteEvent(object sender, ResultEventArgs e)
	{
		Service service = sender as Service;
		if (service != null)
		{
			service.ServiceCompleteEvent -= SetOdometerServiceCompleteEvent;
		}
		if (e.Succeeded)
		{
			Thread.Sleep(100);
			CPCKeyOffOnReset();
		}
		else
		{
			odometerSetState = OdometerSetState.Error;
			errorString = e.Exception.Message;
		}
		UpdateUserInterface();
	}

	private void CPCKeyOffOnReset()
	{
		Service service = cpc.Services["FN_KeyOffOnReset"];
		if (service != null)
		{
			service.ServiceCompleteEvent += CPCKeyOffOnResetFunctionCompleteEvent;
			service.Execute(synchronous: false);
		}
	}

	private void CPCKeyOffOnResetFunctionCompleteEvent(object sender, ResultEventArgs e)
	{
		Service service = sender as Service;
		if (service != null)
		{
			service.ServiceCompleteEvent -= CPCKeyOffOnResetFunctionCompleteEvent;
		}
		if (e.Succeeded)
		{
			Thread.Sleep(100);
			odometerInfo.Read(synchronous: false);
			if (OdometerValue.HasValue)
			{
				textBox1.Text = valueToDisplay.Convert((object)OdometerValue).ToString(CultureInfo.CurrentCulture);
			}
			odometerSetState = OdometerSetState.Set;
		}
		else
		{
			odometerSetState = OdometerSetState.Error;
			errorString = e.Exception.Message;
		}
		UpdateUserInterface();
	}

	private void OnCommunicationsStateUpdate(object sender, CommunicationsStateEventArgs e)
	{
		UpdateUserInterface();
	}

	private bool OdometerValuesAreEqual(double v1, double v2)
	{
		return Math.Abs(v1 - v2) < 0.1;
	}

	private void UpdateUserInterface()
	{
		switch (odometerSetState)
		{
		case OdometerSetState.Set:
			buttonSet.Enabled = false;
			checkmark1.Checked = !EcuIsBusy;
			labelCondition.Text = (EcuIsBusy ? Resources.Message_WritingOdometerValue : Resources.Message_TheOdometerHasBeenSetTheValueShownMayBeSlightlyDifferentThanTheOneEnteredDueToRoundingIssues);
			labelCondition.BackColor = SystemColors.Control;
			break;
		case OdometerSetState.Error:
			buttonSet.Enabled = false;
			checkmark1.Checked = false;
			labelCondition.Text = string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_AnErrorOccurredSettingTheOdometer0, errorString);
			break;
		case OdometerSetState.NotSet:
		{
			double num = ((displayToValue != null) ? displayToValue.Convert((object)textBox1.Text) : double.Parse(textBox1.Text, CultureInfo.CurrentCulture));
			if (!OdometerValue.HasValue)
			{
				buttonSet.Enabled = false;
				checkmark1.Checked = false;
				labelCondition.Text = Resources.Message_TheOdometerValueIsUnknownAndCannotBeSet;
			}
			else if (OdometerValuesAreEqual(num, OdometerValue.Value))
			{
				buttonSet.Enabled = false;
				checkmark1.Checked = false;
				labelCondition.Text = Resources.Message_TheOdometerCannotBeSetBecauseTheProposedValueIsEffectivelyEqualToCurrentOdometer;
				labelCondition.BackColor = SystemColors.Control;
			}
			else if (num > OdometerValue)
			{
				buttonSet.Enabled = true;
				checkmark1.Checked = true;
				labelCondition.Text = Resources.Message_TheOdometerIsReadyToBeSetBecauseTheProposedValueIsGreaterThanTheCurrentOdometer;
				labelCondition.BackColor = SystemColors.Control;
			}
			else
			{
				buttonSet.Enabled = false;
				checkmark1.Checked = false;
				labelCondition.Text = Resources.Message_ErrorTheOdometerCannotBeSetBecauseTheProposedValueIsLessThanTheCurrentOdometer;
				labelCondition.BackColor = Color.Red;
			}
			break;
		}
		default:
			throw new InvalidOperationException();
		}
	}

	private void TextBox1TextChanged(object sender, EventArgs e)
	{
		odometerSetState = OdometerSetState.NotSet;
		UpdateUserInterface();
	}

	private void TextBox1KeyPress(object sender, KeyPressEventArgs e)
	{
		e.Handled = !char.IsDigit(e.KeyChar) && e.KeyChar != '.' && !char.IsControl(e.KeyChar);
	}

	private void InitializeComponent()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		tableLayoutPanel1 = new TableLayoutPanel();
		digitalReadoutInstrument1 = new DigitalReadoutInstrument();
		buttonSet = new Button();
		buttonClose = new Button();
		checkmark1 = new Checkmark();
		labelCondition = new System.Windows.Forms.Label();
		textBox1 = new TextBox();
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument1, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonSet, 2, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonClose, 3, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)checkmark1, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(labelCondition, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(textBox1, 0, 2);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)digitalReadoutInstrument1, 4);
		componentResourceManager.ApplyResources(digitalReadoutInstrument1, "digitalReadoutInstrument1");
		digitalReadoutInstrument1.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument1).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes)8, "CPC04T", "CO_Odometer");
		((Control)(object)digitalReadoutInstrument1).Name = "digitalReadoutInstrument1";
		((SingleInstrumentBase)digitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(buttonSet, "buttonSet");
		buttonSet.Name = "buttonSet";
		buttonSet.UseCompatibleTextRendering = true;
		buttonSet.UseVisualStyleBackColor = true;
		buttonSet.Click += buttonSet_Click;
		buttonClose.DialogResult = DialogResult.OK;
		componentResourceManager.ApplyResources(buttonClose, "buttonClose");
		buttonClose.Name = "buttonClose";
		buttonClose.UseCompatibleTextRendering = true;
		buttonClose.UseVisualStyleBackColor = true;
		componentResourceManager.ApplyResources(checkmark1, "checkmark1");
		((Control)(object)checkmark1).Name = "checkmark1";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)labelCondition, 3);
		componentResourceManager.ApplyResources(labelCondition, "labelCondition");
		labelCondition.Name = "labelCondition";
		labelCondition.UseCompatibleTextRendering = true;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)textBox1, 2);
		componentResourceManager.ApplyResources(textBox1, "textBox1");
		textBox1.Name = "textBox1";
		componentResourceManager.ApplyResources(this, "$this");
		((CustomPanel)this).ContextLink = new Link("Panel_SetOdometer");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)(object)tableLayoutPanel1).PerformLayout();
		((Control)this).ResumeLayout(performLayout: false);
	}
}
