using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.Extensions.FakeInstruments;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Procedures.PTIC_Brake_Learn.panel;

public class UserPanel : CustomPanel
{
	private const string slopeAdjBrakeWholeQualifier = "DT_Slope_Adj_Brake_Whole_Number";

	private const string slopeAdjBrakeRemainderQualifier = "DT_Slope_Adj_Brake_Remainder";

	private const string homeWholeQualifier = "DT_Home_Whole_Number";

	private const string homeRemainderQualifier = "DT_Home_Remainder";

	private const string actualBrakeWholeQualifier = "DT_Actual_Brake_Whole_Number";

	private const string actualBrakeRemainderQualifier = "DT_Actual_Brake_Remainder";

	private const string enableTroubleshootingQualifier = "RT_Enable_Troubleshooting_Manual";

	private RuntimeFakeInstrument slopeAdjBrakeTotal;

	private RuntimeFakeInstrument homeTotal;

	private RuntimeFakeInstrument actualBrakeTotal;

	private Channel ptic;

	private DigitalReadoutInstrument digitalReadoutInstrument1;

	private TableLayoutPanel tableLayoutPanel1;

	private RunServiceButton runServiceButton1;

	private SeekTimeListView seekTimeListView1;

	private DigitalReadoutInstrument digitalReadoutInstrumentNormalizedBrakePedalPositionTotal;

	private DigitalReadoutInstrument digitalReadoutInstrumentHomeTotal;

	private DigitalReadoutInstrument digitalReadoutInstraumentActualBrakeTotal;

	private DigitalReadoutInstrument digitalReadoutInstrumentSlopeAdjBrakeTotal;

	public UserPanel()
	{
		slopeAdjBrakeTotal = RuntimeFakeInstrument.Create("slopeAdjBrakeTotal", "Slope Adj Brake Home", "deg");
		homeTotal = RuntimeFakeInstrument.Create("homeTotal", "Home", "deg");
		actualBrakeTotal = RuntimeFakeInstrument.Create("actualBrakeTotal", "Actual Brake", "deg");
		InitializeComponent();
	}

	public override void OnChannelsChanged()
	{
		SetPTIC(((CustomPanel)this).GetChannel("PTIC", (ChannelLookupOptions)3));
	}

	private bool SetPTIC(Channel ptic)
	{
		bool result = false;
		if (this.ptic != ptic)
		{
			if (this.ptic != null)
			{
				this.ptic.Instruments.InstrumentUpdateEvent -= PticInstrumentUpdateEvent;
			}
			this.ptic = ptic;
			result = true;
			if (this.ptic != null)
			{
				PticInstrumentUpdateEvent(null, null);
				this.ptic.Instruments.InstrumentUpdateEvent += PticInstrumentUpdateEvent;
			}
		}
		return result;
	}

	private void PticInstrumentUpdateEvent(object sender, ResultEventArgs e)
	{
		slopeAdjBrakeTotal.SetValue((object)FormatFloat("DT_Slope_Adj_Brake_Whole_Number", "DT_Slope_Adj_Brake_Remainder"));
		homeTotal.SetValue((object)FormatFloat("DT_Home_Whole_Number", "DT_Home_Remainder"));
		actualBrakeTotal.SetValue((object)FormatFloat("DT_Actual_Brake_Whole_Number", "DT_Actual_Brake_Remainder"));
	}

	private string FormatFloat(string wholeNumberQualifier, string remainderQualifier)
	{
		string result = "sna";
		int? num = ReadInstrument(wholeNumberQualifier);
		int? num2 = ReadInstrument(remainderQualifier);
		if (num.HasValue && num2.HasValue)
		{
			result = string.Format(CultureInfo.InvariantCulture, "{0}.{1}", num, num2);
		}
		return result;
	}

	private int? ReadInstrument(string instrumentQualifier)
	{
		int? result = null;
		if (ptic != null && ptic.Instruments[instrumentQualifier] != null && ptic.Instruments[instrumentQualifier].InstrumentValues != null && ptic.Instruments[instrumentQualifier].InstrumentValues.Current != null && ptic.Instruments[instrumentQualifier].InstrumentValues.Current.Value != null)
		{
			result = (byte)ptic.Instruments[instrumentQualifier].InstrumentValues.Current.Value;
		}
		return result;
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
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_03af: Unknown result type (might be due to invalid IL or missing references)
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(UserPanel));
		digitalReadoutInstrument1 = new DigitalReadoutInstrument();
		tableLayoutPanel1 = new TableLayoutPanel();
		seekTimeListView1 = new SeekTimeListView();
		digitalReadoutInstrumentNormalizedBrakePedalPositionTotal = new DigitalReadoutInstrument();
		runServiceButton1 = new RunServiceButton();
		digitalReadoutInstrumentSlopeAdjBrakeTotal = new DigitalReadoutInstrument();
		digitalReadoutInstraumentActualBrakeTotal = new DigitalReadoutInstrument();
		digitalReadoutInstrumentHomeTotal = new DigitalReadoutInstrument();
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)this).SuspendLayout();
		componentResourceManager.ApplyResources(digitalReadoutInstrument1, "digitalReadoutInstrument1");
		digitalReadoutInstrument1.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrument1).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes)1, "PTIC", "DT_Brake_Home_Learn_Status");
		((Control)(object)digitalReadoutInstrument1).Name = "digitalReadoutInstrument1";
		((SingleInstrumentBase)digitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(tableLayoutPanel1, "tableLayoutPanel1");
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrument1, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)seekTimeListView1, 0, 3);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentNormalizedBrakePedalPositionTotal, 0, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentSlopeAdjBrakeTotal, 1, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstrumentHomeTotal, 0, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)digitalReadoutInstraumentActualBrakeTotal, 1, 2);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add((Control)(object)runServiceButton1, 1, 0);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)(object)seekTimeListView1, 3);
		componentResourceManager.ApplyResources(seekTimeListView1, "seekTimeListView1");
		((Control)(object)seekTimeListView1).Name = "seekTimeListView1";
		seekTimeListView1.SelectedTime = null;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentNormalizedBrakePedalPositionTotal, "digitalReadoutInstrumentNormalizedBrakePedalPositionTotal");
		digitalReadoutInstrumentNormalizedBrakePedalPositionTotal.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentNormalizedBrakePedalPositionTotal).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentNormalizedBrakePedalPositionTotal).Instrument = new Qualifier((QualifierTypes)1, "PTIC", "DT_521");
		((Control)(object)digitalReadoutInstrumentNormalizedBrakePedalPositionTotal).Name = "digitalReadoutInstrumentNormalizedBrakePedalPositionTotal";
		((SingleInstrumentBase)digitalReadoutInstrumentNormalizedBrakePedalPositionTotal).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(runServiceButton1, "runServiceButton1");
		((Control)(object)runServiceButton1).Name = "runServiceButton1";
		runServiceButton1.ServiceCall = new ServiceCall("PTIC", "RT_Begin_Brake_Home_Learn");
		componentResourceManager.ApplyResources(digitalReadoutInstrumentSlopeAdjBrakeTotal, "digitalReadoutInstrumentSlopeAdjBrakeTotal");
		digitalReadoutInstrumentSlopeAdjBrakeTotal.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentSlopeAdjBrakeTotal).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentSlopeAdjBrakeTotal).Instrument = new Qualifier((QualifierTypes)16, "fake", "slopeAdjBrakeTotal");
		((Control)(object)digitalReadoutInstrumentSlopeAdjBrakeTotal).Name = "digitalReadoutInstrumentSlopeAdjBrakeTotal";
		((SingleInstrumentBase)digitalReadoutInstrumentSlopeAdjBrakeTotal).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstraumentActualBrakeTotal, "digitalReadoutInstraumentActualBrakeTotal");
		digitalReadoutInstraumentActualBrakeTotal.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstraumentActualBrakeTotal).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstraumentActualBrakeTotal).Instrument = new Qualifier((QualifierTypes)16, "fake", "actualBrakeTotal");
		((Control)(object)digitalReadoutInstraumentActualBrakeTotal).Name = "digitalReadoutInstraumentActualBrakeTotal";
		((SingleInstrumentBase)digitalReadoutInstraumentActualBrakeTotal).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(digitalReadoutInstrumentHomeTotal, "digitalReadoutInstrumentHomeTotal");
		digitalReadoutInstrumentHomeTotal.FontGroup = null;
		((SingleInstrumentBase)digitalReadoutInstrumentHomeTotal).FreezeValue = false;
		((SingleInstrumentBase)digitalReadoutInstrumentHomeTotal).Instrument = new Qualifier((QualifierTypes)16, "fake", "homeTotal");
		((Control)(object)digitalReadoutInstrumentHomeTotal).Name = "digitalReadoutInstrumentHomeTotal";
		((SingleInstrumentBase)digitalReadoutInstrumentHomeTotal).UnitAlignment = StringAlignment.Near;
		componentResourceManager.ApplyResources(this, "$this");
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
