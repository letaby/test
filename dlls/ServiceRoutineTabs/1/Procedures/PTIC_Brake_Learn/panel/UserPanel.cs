// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Procedures.PTIC_Brake_Learn.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Extensions.FakeInstruments;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

#nullable disable
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
    this.slopeAdjBrakeTotal = RuntimeFakeInstrument.Create(nameof (slopeAdjBrakeTotal), "Slope Adj Brake Home", "deg");
    this.homeTotal = RuntimeFakeInstrument.Create(nameof (homeTotal), "Home", "deg");
    this.actualBrakeTotal = RuntimeFakeInstrument.Create(nameof (actualBrakeTotal), "Actual Brake", "deg");
    this.InitializeComponent();
  }

  public virtual void OnChannelsChanged()
  {
    this.SetPTIC(this.GetChannel("PTIC", (CustomPanel.ChannelLookupOptions) 3));
  }

  private bool SetPTIC(Channel ptic)
  {
    bool flag = false;
    if (this.ptic != ptic)
    {
      if (this.ptic != null)
        this.ptic.Instruments.InstrumentUpdateEvent -= new InstrumentUpdateEventHandler(this.PticInstrumentUpdateEvent);
      this.ptic = ptic;
      flag = true;
      if (this.ptic != null)
      {
        this.PticInstrumentUpdateEvent((object) null, (ResultEventArgs) null);
        this.ptic.Instruments.InstrumentUpdateEvent += new InstrumentUpdateEventHandler(this.PticInstrumentUpdateEvent);
      }
    }
    return flag;
  }

  private void PticInstrumentUpdateEvent(object sender, ResultEventArgs e)
  {
    this.slopeAdjBrakeTotal.SetValue((object) this.FormatFloat("DT_Slope_Adj_Brake_Whole_Number", "DT_Slope_Adj_Brake_Remainder"));
    this.homeTotal.SetValue((object) this.FormatFloat("DT_Home_Whole_Number", "DT_Home_Remainder"));
    this.actualBrakeTotal.SetValue((object) this.FormatFloat("DT_Actual_Brake_Whole_Number", "DT_Actual_Brake_Remainder"));
  }

  private string FormatFloat(string wholeNumberQualifier, string remainderQualifier)
  {
    string str = "sna";
    int? nullable1 = this.ReadInstrument(wholeNumberQualifier);
    int? nullable2 = this.ReadInstrument(remainderQualifier);
    if (nullable1.HasValue && nullable2.HasValue)
      str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}", (object) nullable1, (object) nullable2);
    return str;
  }

  private int? ReadInstrument(string instrumentQualifier)
  {
    int? nullable = new int?();
    if (this.ptic != null && this.ptic.Instruments[instrumentQualifier] != (Instrument) null && this.ptic.Instruments[instrumentQualifier].InstrumentValues != null && this.ptic.Instruments[instrumentQualifier].InstrumentValues.Current != null && this.ptic.Instruments[instrumentQualifier].InstrumentValues.Current.Value != null)
      nullable = new int?((int) (byte) this.ptic.Instruments[instrumentQualifier].InstrumentValues.Current.Value);
    return nullable;
  }

  private void InitializeComponent()
  {
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (UserPanel));
    this.digitalReadoutInstrument1 = new DigitalReadoutInstrument();
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.seekTimeListView1 = new SeekTimeListView();
    this.digitalReadoutInstrumentNormalizedBrakePedalPositionTotal = new DigitalReadoutInstrument();
    this.runServiceButton1 = new RunServiceButton();
    this.digitalReadoutInstrumentSlopeAdjBrakeTotal = new DigitalReadoutInstrument();
    this.digitalReadoutInstraumentActualBrakeTotal = new DigitalReadoutInstrument();
    this.digitalReadoutInstrumentHomeTotal = new DigitalReadoutInstrument();
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this).SuspendLayout();
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrument1, "digitalReadoutInstrument1");
    this.digitalReadoutInstrument1.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).Instrument = new Qualifier((QualifierTypes) 1, "PTIC", "DT_Brake_Home_Learn_Status");
    ((Control) this.digitalReadoutInstrument1).Name = "digitalReadoutInstrument1";
    ((SingleInstrumentBase) this.digitalReadoutInstrument1).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.tableLayoutPanel1, "tableLayoutPanel1");
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrument1, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.seekTimeListView1, 0, 3);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrumentNormalizedBrakePedalPositionTotal, 0, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrumentSlopeAdjBrakeTotal, 1, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstrumentHomeTotal, 0, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.digitalReadoutInstraumentActualBrakeTotal, 1, 2);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.runServiceButton1, 1, 0);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.seekTimeListView1, 3);
    componentResourceManager.ApplyResources((object) this.seekTimeListView1, "seekTimeListView1");
    ((Control) this.seekTimeListView1).Name = "seekTimeListView1";
    this.seekTimeListView1.SelectedTime = new DateTime?();
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentNormalizedBrakePedalPositionTotal, "digitalReadoutInstrumentNormalizedBrakePedalPositionTotal");
    this.digitalReadoutInstrumentNormalizedBrakePedalPositionTotal.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentNormalizedBrakePedalPositionTotal).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentNormalizedBrakePedalPositionTotal).Instrument = new Qualifier((QualifierTypes) 1, "PTIC", "DT_521");
    ((Control) this.digitalReadoutInstrumentNormalizedBrakePedalPositionTotal).Name = "digitalReadoutInstrumentNormalizedBrakePedalPositionTotal";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentNormalizedBrakePedalPositionTotal).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.runServiceButton1, "runServiceButton1");
    ((Control) this.runServiceButton1).Name = "runServiceButton1";
    this.runServiceButton1.ServiceCall = new ServiceCall("PTIC", "RT_Begin_Brake_Home_Learn");
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentSlopeAdjBrakeTotal, "digitalReadoutInstrumentSlopeAdjBrakeTotal");
    this.digitalReadoutInstrumentSlopeAdjBrakeTotal.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentSlopeAdjBrakeTotal).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentSlopeAdjBrakeTotal).Instrument = new Qualifier((QualifierTypes) 16 /*0x10*/, "fake", "slopeAdjBrakeTotal");
    ((Control) this.digitalReadoutInstrumentSlopeAdjBrakeTotal).Name = "digitalReadoutInstrumentSlopeAdjBrakeTotal";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentSlopeAdjBrakeTotal).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstraumentActualBrakeTotal, "digitalReadoutInstraumentActualBrakeTotal");
    this.digitalReadoutInstraumentActualBrakeTotal.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstraumentActualBrakeTotal).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstraumentActualBrakeTotal).Instrument = new Qualifier((QualifierTypes) 16 /*0x10*/, "fake", "actualBrakeTotal");
    ((Control) this.digitalReadoutInstraumentActualBrakeTotal).Name = "digitalReadoutInstraumentActualBrakeTotal";
    ((SingleInstrumentBase) this.digitalReadoutInstraumentActualBrakeTotal).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this.digitalReadoutInstrumentHomeTotal, "digitalReadoutInstrumentHomeTotal");
    this.digitalReadoutInstrumentHomeTotal.FontGroup = (string) null;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentHomeTotal).FreezeValue = false;
    ((SingleInstrumentBase) this.digitalReadoutInstrumentHomeTotal).Instrument = new Qualifier((QualifierTypes) 16 /*0x10*/, "fake", "homeTotal");
    ((Control) this.digitalReadoutInstrumentHomeTotal).Name = "digitalReadoutInstrumentHomeTotal";
    ((SingleInstrumentBase) this.digitalReadoutInstrumentHomeTotal).UnitAlignment = StringAlignment.Near;
    componentResourceManager.ApplyResources((object) this, "$this");
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel1);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }
}
