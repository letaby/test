// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Periodic_CARB_Smoke_Inspection_Program_OBD_Data_Report.panel.UserPanel
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using DetroitDiesel.Common;
using DetroitDiesel.Windows.Forms;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Periodic_CARB_Smoke_Inspection_Program_OBD_Data_Report.panel;

public class UserPanel : CustomPanel
{
  private const string McmName = "J1939-1";
  private const string AcmName = "J1939-61";
  private const string CpcName = "J1939-0";
  private const string H1Style = "text-align: center; color: black; font: bold 14pt segoe ui,sans-serif";
  private const string H2Style = "text-align: center; color: black; font: bold 12pt segoe ui,sans-serif";
  private const string TableStyle = "width:100%;margin-top:5px;margin-bottom:10px;border:1px solid white;";
  private const string TableHeaderElementName = "TH";
  private const string TableDataElementName = "TD";
  private const string QualifierVin = "237";
  private const string QualifierEsn = "588";
  private WebBrowserList webBrowserList;
  private UserPanel.Identification identification;
  private UserPanel.Readiness readiness;
  private UserPanel.FaultCodes faultCodes;
  private bool isGenerated = false;
  private DateTime generatedDate;
  private TableLayoutPanel tableLayoutPanel1;
  private Button buttonExport;
  private Button buttonOK;
  private Panel panelObd;

  public UserPanel()
  {
    this.InitializeComponent();
    this.generatedDate = !SapiManager.GlobalInstance.LogFileIsOpen ? DateTime.Now : SapiManager.GlobalInstance.LogFileAllChannels.First<Channel>().LogFile.CurrentTime;
    this.webBrowserList = new WebBrowserList();
    this.webBrowserList.SetWriterFunction(new Action<XmlWriter>(this.UpdateContent));
    ((Control) this.webBrowserList).Dock = DockStyle.Fill;
    this.panelObd.Controls.Add((Control) this.webBrowserList);
  }

  private void UpdateContent(XmlWriter writer)
  {
    if (this.isGenerated)
      return;
    this.CreateHeader(writer);
    this.identification = new UserPanel.Identification();
    this.identification.UpdateContent(writer);
    this.readiness = new UserPanel.Readiness();
    this.readiness.UpdateContent(writer);
    this.faultCodes = new UserPanel.FaultCodes();
    this.faultCodes.UpdateContent(writer);
    this.isGenerated = true;
  }

  private string GetValue(Dictionary<string, string> obdReportData, string item, bool addQuotes)
  {
    string str = string.Empty;
    if (obdReportData == null)
      str = item;
    else if (obdReportData.ContainsKey(item))
      str = obdReportData[item];
    if (string.IsNullOrEmpty(str))
      return ",";
    return addQuotes ? $"\"{str}\"," : $"{str},";
  }

  private void CreateHeader(XmlWriter writer)
  {
    writer.WriteStartElement("h1");
    writer.WriteAttributeString("style", "text-align: center; color: black; font: bold 14pt segoe ui,sans-serif");
    writer.WriteAttributeString("text-align", "center");
    writer.WriteString(Resources.Message_ReportName);
    writer.WriteEndElement();
    writer.WriteStartElement("h2");
    writer.WriteAttributeString("style", "text-align: center; color: black; font: bold 12pt segoe ui,sans-serif");
    writer.WriteAttributeString("text-align", "center");
    writer.WriteString($"{this.generatedDate.ToLongDateString()} {this.generatedDate.ToLongTimeString()}");
    writer.WriteEndElement();
  }

  private void buttonExport_Click(object sender, EventArgs e)
  {
    Qualifier qualifier;
    // ISSUE: explicit constructor call
    ((Qualifier) ref qualifier).\u002Ector((QualifierTypes) 8, "J1939-0", "237");
    string str = $"CARB Smoke Report {this.identification.QualifierValue(qualifier)} {this.generatedDate.ToString("yyyyMMdd", (IFormatProvider) DateTimeFormatInfo.InvariantInfo)} {this.generatedDate.ToString("HHmmss", (IFormatProvider) DateTimeFormatInfo.InvariantInfo)}.csv";
    SaveFileDialog saveFileDialog1 = new SaveFileDialog();
    saveFileDialog1.DefaultExt = ".csv";
    saveFileDialog1.FileName = str;
    saveFileDialog1.Filter = Resources.CSVFileFilter;
    saveFileDialog1.AddExtension = true;
    saveFileDialog1.SupportMultiDottedExtensions = true;
    saveFileDialog1.Title = Resources.DialogTitleSelectExportDestination;
    saveFileDialog1.ValidateNames = true;
    saveFileDialog1.AutoUpgradeEnabled = true;
    saveFileDialog1.OverwritePrompt = true;
    using (SaveFileDialog saveFileDialog2 = saveFileDialog1)
    {
      if (saveFileDialog2.ShowDialog() != DialogResult.OK)
        return;
      using (TextWriter writer = (TextWriter) new StreamWriter(saveFileDialog2.FileName))
      {
        this.WriteHeader(writer);
        this.WriteBody(writer, "J1939-61", this.faultCodes.obdReportDataAcm, this.identification.obdReportDataAcm, this.readiness.obdReportDataAcm);
        this.WriteBody(writer, "J1939-1", this.faultCodes.obdReportDataMcm, this.identification.obdReportDataMcm, this.readiness.obdReportDataMcm);
        this.WriteBody(writer, "J1939-0", this.faultCodes.obdReportDataCpc, this.identification.obdReportDataCpc, this.readiness.obdReportDataCpc);
      }
    }
  }

  private void WriteHeader(TextWriter writer)
  {
    writer.Write($"{Resources.Message_SourceAddress},");
    writer.Write($"{Resources.Message_DM1MILStatusSPN1213},");
    writer.Write($"{Resources.Message_ActiveFaultCodes},");
    writer.Write($"{Resources.Message_PreviouslyActiveFaultCodes},");
    writer.Write($"{Resources.Message_PendingFaultCodes},");
    writer.Write($"{Resources.Message_PermanentFaultCodes},");
    writer.Write($"{Resources.Message_ComprehensiveComponent},");
    writer.Write($"{Resources.Message_EGR},");
    writer.Write($"{Resources.Message_ExhaustGasSensor},");
    writer.Write($"{Resources.Message_ExhaustGasSensorHeater},");
    writer.Write($"{Resources.Message_EngineFuelSystem},");
    writer.Write($"{Resources.Message_Misfire},");
    writer.Write($"{Resources.Message_NMHCCatalyst},");
    writer.Write($"{Resources.Message_NOxConvertingCatalyst},");
    writer.Write($"{Resources.Message_DieselParticulateFilter},");
    writer.Write($"{Resources.Message_BoostPressureControlSystem},");
    writer.Write($"{Resources.Message_VehicleIdentificationNumber},");
    writer.Write($"{Resources.Message_EngineSerialNumber},");
    writer.Write($"{Resources.Message_CalibrationIdentificationNumber},");
    writer.Write($"{Resources.Message_CalibrationVerificationNumber},");
    writer.Write($"{Resources.Message_OBDRequirementsToWhichEngineIsCertified},");
    writer.Write($"{Resources.Message_DistanceTraveledWithMILActivated},");
    writer.Write($"{Resources.Message_EngineRunTimeWithMILActivated},");
    writer.Write($"{Resources.Message_DistanceTraveledSinceMemoryLastCleared},");
    writer.Write($"{Resources.Message_EngineRunTimeSinceMemoryLastCleared},");
    writer.Write($"{Resources.Message_DM26NumberWarmupCyclesSinceDTCClearSPN3302},");
    writer.WriteLine();
  }

  private void WriteBody(
    TextWriter writer,
    string sourceAddress,
    Dictionary<string, string> faultCodeData,
    Dictionary<string, string> identificationData,
    Dictionary<string, string> readinessData)
  {
    writer.Write(this.GetValue((Dictionary<string, string>) null, sourceAddress, false));
    writer.Write(this.GetValue(faultCodeData, Resources.Message_DM1MILStatusSPN1213, false));
    writer.Write(this.GetValue(faultCodeData, Resources.Message_ActiveFaultCodes, true));
    writer.Write(this.GetValue(faultCodeData, Resources.Message_PreviouslyActiveFaultCodes, true));
    writer.Write(this.GetValue(faultCodeData, Resources.Message_PendingFaultCodes, true));
    writer.Write(this.GetValue(faultCodeData, Resources.Message_PermanentFaultCodes, true));
    writer.Write(this.GetValue(readinessData, Resources.Message_ComprehensiveComponent, false));
    writer.Write(this.GetValue(readinessData, Resources.Message_EGR, false));
    writer.Write(this.GetValue(readinessData, Resources.Message_ExhaustGasSensor, false));
    writer.Write(this.GetValue(readinessData, Resources.Message_ExhaustGasSensorHeater, false));
    writer.Write(this.GetValue(readinessData, Resources.Message_EngineFuelSystem, false));
    writer.Write(this.GetValue(readinessData, Resources.Message_Misfire, false));
    writer.Write(this.GetValue(readinessData, Resources.Message_NMHCCatalyst, false));
    writer.Write(this.GetValue(readinessData, Resources.Message_NOxConvertingCatalyst, false));
    writer.Write(this.GetValue(readinessData, Resources.Message_DieselParticulateFilter, false));
    writer.Write(this.GetValue(readinessData, Resources.Message_BoostPressureControlSystem, false));
    writer.Write(this.GetValue(identificationData, Resources.Message_VehicleIdentificationNumber, false));
    writer.Write(this.GetValue(identificationData, Resources.Message_EngineSerialNumber, false));
    writer.Write(this.GetValue(identificationData, Resources.Message_CalibrationIdentificationNumber, false));
    writer.Write(this.GetValue(identificationData, Resources.Message_CalibrationVerificationNumber, false));
    writer.Write(this.GetValue(readinessData, Resources.Message_OBDRequirementsToWhichEngineIsCertified, false));
    writer.Write(this.GetValue(readinessData, Resources.Message_DistanceTraveledWithMILActivated, false));
    writer.Write(this.GetValue(readinessData, Resources.Message_EngineRunTimeWithMILActivated, false));
    writer.Write(this.GetValue(readinessData, Resources.Message_DistanceTraveledSinceMemoryLastCleared, false));
    writer.Write(this.GetValue(readinessData, Resources.Message_EngineRunTimeSinceMemoryLastCleared, false));
    writer.Write(this.GetValue(faultCodeData, Resources.Message_DM26NumberWarmupCyclesSinceDTCClearSPN3302, false));
    writer.WriteLine();
  }

  private void InitializeComponent()
  {
    this.tableLayoutPanel1 = new TableLayoutPanel();
    this.buttonOK = new Button();
    this.panelObd = new Panel();
    this.buttonExport = new Button();
    ((Control) this.tableLayoutPanel1).SuspendLayout();
    ((Control) this).SuspendLayout();
    ((TableLayoutPanel) this.tableLayoutPanel1).ColumnCount = 4;
    ((TableLayoutPanel) this.tableLayoutPanel1).ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80f));
    ((TableLayoutPanel) this.tableLayoutPanel1).ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80f));
    ((TableLayoutPanel) this.tableLayoutPanel1).ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
    ((TableLayoutPanel) this.tableLayoutPanel1).ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80f));
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonOK, 3, 1);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.panelObd, 0, 0);
    ((TableLayoutPanel) this.tableLayoutPanel1).Controls.Add((Control) this.buttonExport, 0, 1);
    ((Control) this.tableLayoutPanel1).Dock = DockStyle.Fill;
    ((Control) this.tableLayoutPanel1).Location = new Point(0, 0);
    ((Control) this.tableLayoutPanel1).Name = "tableLayoutPanel1";
    ((TableLayoutPanel) this.tableLayoutPanel1).RowCount = 2;
    ((TableLayoutPanel) this.tableLayoutPanel1).RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
    ((TableLayoutPanel) this.tableLayoutPanel1).RowStyles.Add(new RowStyle(SizeType.Absolute, 30f));
    ((Control) this.tableLayoutPanel1).Size = new Size(799, 571);
    ((Control) this.tableLayoutPanel1).TabIndex = 5;
    this.buttonOK.DialogResult = DialogResult.OK;
    this.buttonOK.Location = new Point(722, 544);
    this.buttonOK.Name = "buttonOK";
    this.buttonOK.Size = new Size(74, 23);
    this.buttonOK.TabIndex = 2;
    this.buttonOK.Text = "OK";
    this.buttonOK.UseVisualStyleBackColor = true;
    ((TableLayoutPanel) this.tableLayoutPanel1).SetColumnSpan((Control) this.panelObd, 4);
    this.panelObd.Dock = DockStyle.Fill;
    this.panelObd.Location = new Point(3, 3);
    this.panelObd.Name = "panelObd";
    this.panelObd.Size = new Size(793, 535);
    this.panelObd.TabIndex = 3;
    this.buttonExport.Location = new Point(3, 544);
    this.buttonExport.Name = "buttonExport";
    this.buttonExport.Size = new Size(74, 23);
    this.buttonExport.TabIndex = 1;
    this.buttonExport.Text = "Export...";
    this.buttonExport.UseVisualStyleBackColor = true;
    this.buttonExport.Click += new EventHandler(this.buttonExport_Click);
    this.AutoScaleDimensions = new SizeF(6f, 13f);
    ((Control) this).Controls.Add((Control) this.tableLayoutPanel1);
    ((Control) this).Margin = new Padding(5);
    ((Control) this).Name = nameof (UserPanel);
    ((Control) this.tableLayoutPanel1).ResumeLayout(false);
    ((Control) this).ResumeLayout(false);
  }

  private class OBD
  {
    public Dictionary<string, string> obdReportDataAcm = new Dictionary<string, string>();
    public Dictionary<string, string> obdReportDataMcm = new Dictionary<string, string>();
    public Dictionary<string, string> obdReportDataCpc = new Dictionary<string, string>();
    public string[] sourceBackgroundColors = new string[4]
    {
      "#E4F4F4",
      "#FFFCCF",
      "#CFFBA6",
      "#D1ECC1"
    };
    private readonly Regex removeUnprintables = new Regex("[\\x01-\\x1F]", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);

    public virtual void UpdateContent(XmlWriter writer)
    {
    }

    public string QualifierValue(Qualifier qualifier)
    {
      IEnumerable<Channel> activeChannels = (IEnumerable<Channel>) SapiManager.GlobalInstance.ActiveChannels;
      using (DataItem dataItem = DataItem.Create(qualifier, activeChannels))
      {
        if (dataItem != null)
          return this.removeUnprintables.Replace(dataItem.ValueAsString(dataItem.Value), "");
      }
      return (string) null;
    }

    protected double QualifierValueAsDouble(Qualifier qualifier)
    {
      IEnumerable<Channel> activeChannels = (IEnumerable<Channel>) SapiManager.GlobalInstance.ActiveChannels;
      using (DataItem dataItem = DataItem.Create(qualifier, activeChannels))
      {
        if (dataItem != null)
          return dataItem.ValueAsDouble(dataItem.Value);
      }
      return double.NaN;
    }

    protected bool IsQualifierValid(Qualifier qualifier)
    {
      return DataItem.Create(qualifier, (IEnumerable<Channel>) SapiManager.GlobalInstance.ActiveChannels) != null;
    }

    protected static string QualifierUnits(Qualifier qualifier)
    {
      string str = string.Empty;
      using (DataItem dataItem = DataItem.Create(qualifier, (IEnumerable<Channel>) SapiManager.GlobalInstance.ActiveChannels))
      {
        if (dataItem != null)
          str = dataItem.Units;
      }
      return str;
    }

    protected Channel GetChannelByEcu(string ecuName)
    {
      return SapiManager.GlobalInstance.ActiveChannels.Where<Channel>((Func<Channel, bool>) (ch => ch.Ecu.Name == ecuName)).FirstOrDefault<Channel>();
    }

    protected void WriteTableCell(
      XmlWriter writer,
      string elementType,
      UserPanel.OBD.SourceBackgroundColorsIndex backgroundColor,
      string text,
      string style = null)
    {
      writer.WriteStartElement(elementType);
      if (!string.IsNullOrEmpty(style))
        writer.WriteAttributeString(nameof (style), style);
      writer.WriteAttributeString(nameof (style), "background:" + this.sourceBackgroundColors[(int) backgroundColor]);
      writer.WriteString(text);
      writer.WriteEndElement();
    }

    public enum SourceBackgroundColorsIndex
    {
      header,
      row,
      value,
      unit,
    }

    public struct QualifierItem(
      QualifierTypes type,
      string channelName,
      string qualifierNumber,
      string displayText = "",
      string dm = "")
    {
      public QualifierTypes qualifierType = type;
      public string qualifierNumber = qualifierNumber;
      public string displayText = displayText;
      public string channelName = channelName;
      public string dm = dm;
    }

    public struct QualifierGroup(
      string groupname,
      KeyValuePair<string, UserPanel.OBD.QualifierItem>[] data,
      string dm = "")
    {
      public string name = groupname;
      public KeyValuePair<string, UserPanel.OBD.QualifierItem>[] data = data;
      public string dm = dm;
    }
  }

  private class Identification : UserPanel.OBD
  {
    private static UserPanel.OBD.QualifierGroup[] groupIdentification = new UserPanel.OBD.QualifierGroup[1]
    {
      new UserPanel.OBD.QualifierGroup(Resources.Message_DM19CalibrationInformation, new KeyValuePair<string, UserPanel.OBD.QualifierItem>[4]
      {
        new KeyValuePair<string, UserPanel.OBD.QualifierItem>(Resources.Message_CalibrationIdentificationNumber, new UserPanel.OBD.QualifierItem((QualifierTypes) 8, (string) null, "1635[0]", Resources.Message_CalibrationIdentificationNumber, "19")),
        new KeyValuePair<string, UserPanel.OBD.QualifierItem>(Resources.Message_CalibrationVerificationNumber, new UserPanel.OBD.QualifierItem((QualifierTypes) 8, (string) null, "1634[0]", Resources.Message_CalibrationVerificationNumber, "19")),
        new KeyValuePair<string, UserPanel.OBD.QualifierItem>(Resources.Message_VehicleIdentificationNumber, new UserPanel.OBD.QualifierItem((QualifierTypes) 8, (string) null, "237", Resources.Message_VehicleIdentificationNumber)),
        new KeyValuePair<string, UserPanel.OBD.QualifierItem>(Resources.Message_EngineSerialNumber, new UserPanel.OBD.QualifierItem((QualifierTypes) 8, (string) null, "588", Resources.Message_EngineSerialNumber))
      })
    };

    public override void UpdateContent(XmlWriter writer)
    {
      string[] strArray = new string[5]
      {
        "#EBF5FB",
        "#FEF9E7",
        "#E8F6F3",
        "#EAFAF1",
        "#F4ECF7"
      };
      WebBrowserList.WriteExpandableContent(writer, true, "ecu", "heading2", nameof (Identification), (Action) (() =>
      {
        WebBrowserList.WriteExpandableContent(writer, true, "ecu", "heading2", "J1939-61", (Action) (() => this.OutputIdentification(writer, UserPanel.Identification.groupIdentification, "J1939-61", this.obdReportDataAcm)));
        WebBrowserList.WriteExpandableContent(writer, true, "ecu", "heading2", "J1939-1", (Action) (() => this.OutputIdentification(writer, UserPanel.Identification.groupIdentification, "J1939-1", this.obdReportDataMcm)));
        WebBrowserList.WriteExpandableContent(writer, true, "ecu", "heading2", "J1939-0", (Action) (() => this.OutputIdentification(writer, UserPanel.Identification.groupIdentification, "J1939-0", this.obdReportDataCpc)));
      }));
    }

    private void OutputIdentification(
      XmlWriter writer,
      UserPanel.OBD.QualifierGroup[] qualifierGroup,
      string channelName,
      Dictionary<string, string> obdReportData)
    {
      writer.WriteStartElement("table");
      writer.WriteAttributeString("style", "width:100%;margin-top:5px;margin-bottom:10px;border:1px solid white;");
      writer.WriteStartElement("tr");
      this.WriteTableCell(writer, "TH", UserPanel.OBD.SourceBackgroundColorsIndex.header, "DM");
      this.WriteTableCell(writer, "TH", UserPanel.OBD.SourceBackgroundColorsIndex.header, "SPN");
      this.WriteTableCell(writer, "TH", UserPanel.OBD.SourceBackgroundColorsIndex.header, "Name");
      this.WriteTableCell(writer, "TH", UserPanel.OBD.SourceBackgroundColorsIndex.header, "Value");
      writer.WriteEndElement();
      foreach (UserPanel.OBD.QualifierGroup qualifierGroup1 in qualifierGroup)
      {
        foreach (KeyValuePair<string, UserPanel.OBD.QualifierItem> keyValuePair in qualifierGroup1.data)
        {
          writer.WriteStartElement("tr");
          UserPanel.OBD.QualifierItem qualifierItem = keyValuePair.Value;
          Qualifier qualifier;
          // ISSUE: explicit constructor call
          ((Qualifier) ref qualifier).\u002Ector(qualifierItem.qualifierType, channelName, qualifierItem.qualifierNumber);
          string text = this.QualifierValue(qualifier);
          this.WriteTableCell(writer, "TD", UserPanel.OBD.SourceBackgroundColorsIndex.row, qualifierItem.dm);
          this.WriteTableCell(writer, "TD", UserPanel.OBD.SourceBackgroundColorsIndex.row, ((Qualifier) ref qualifier).Name);
          this.WriteTableCell(writer, "TD", UserPanel.OBD.SourceBackgroundColorsIndex.row, string.IsNullOrEmpty(qualifierItem.displayText) ? keyValuePair.Key : qualifierItem.displayText);
          this.WriteTableCell(writer, "TD", UserPanel.OBD.SourceBackgroundColorsIndex.value, text);
          writer.WriteEndElement();
          obdReportData.Add(keyValuePair.Key, text);
        }
      }
      writer.WriteEndElement();
    }
  }

  private class Readiness : UserPanel.OBD
  {
    private static string OBDComplianceQualifier = "1220";
    private static UserPanel.Readiness.QualifierMap[] data = new UserPanel.Readiness.QualifierMap[10]
    {
      new UserPanel.Readiness.QualifierMap(Resources.Message_ComprehensiveComponent, "DT_1221_4_7", "DT_1221_4_3"),
      new UserPanel.Readiness.QualifierMap(Resources.Message_EGR, "DT_1223_7_8", "DT_1222_5_8"),
      new UserPanel.Readiness.QualifierMap(Resources.Message_ExhaustGasSensor, "DT_1223_7_6", "DT_1222_5_6"),
      new UserPanel.Readiness.QualifierMap(Resources.Message_ExhaustGasSensorHeater, "DT_1223_7_7", "DT_1222_5_7"),
      new UserPanel.Readiness.QualifierMap(Resources.Message_EngineFuelSystem, "DT_1221_4_6", "DT_1221_4_2"),
      new UserPanel.Readiness.QualifierMap(Resources.Message_Misfire, "DT_1221_4_5", "DT_1221_4_1"),
      new UserPanel.Readiness.QualifierMap(Resources.Message_NMHCCatalyst, "DT_1223_8_5", "DT_1222_6_5"),
      new UserPanel.Readiness.QualifierMap(Resources.Message_NOxConvertingCatalyst, "DT_1223_8_4", "DT_1222_6_4"),
      new UserPanel.Readiness.QualifierMap(Resources.Message_DieselParticulateFilter, "DT_1223_8_3", "DT_1222_6_3"),
      new UserPanel.Readiness.QualifierMap(Resources.Message_BoostPressureControlSystem, "DT_1223_8_2", "DT_1222_6_2")
    };
    private static UserPanel.OBD.QualifierGroup[] groupReadiness = new UserPanel.OBD.QualifierGroup[2]
    {
      new UserPanel.OBD.QualifierGroup(Resources.Message_EngineTotals, new KeyValuePair<string, UserPanel.OBD.QualifierItem>[1]
      {
        new KeyValuePair<string, UserPanel.OBD.QualifierItem>(Resources.Message_OBDRequirementsToWhichEngineIsCertified, new UserPanel.OBD.QualifierItem((QualifierTypes) 8, "J1939-61", UserPanel.Readiness.OBDComplianceQualifier, Resources.Message_OBDCompliance))
      }, "5"),
      new UserPanel.OBD.QualifierGroup(Resources.Message_EngineTotals, new KeyValuePair<string, UserPanel.OBD.QualifierItem>[4]
      {
        new KeyValuePair<string, UserPanel.OBD.QualifierItem>(Resources.Message_DistanceTraveledWithMILActivated, new UserPanel.OBD.QualifierItem((QualifierTypes) 1, (string) null, "DT_3069", Resources.Message_DistanceTravelledWhileMILIsActivated)),
        new KeyValuePair<string, UserPanel.OBD.QualifierItem>(Resources.Message_DistanceTraveledSinceMemoryLastCleared, new UserPanel.OBD.QualifierItem((QualifierTypes) 1, (string) null, "DT_3294", Resources.Message_DistanceSinceDiagnosticTroubleCodesCleared)),
        new KeyValuePair<string, UserPanel.OBD.QualifierItem>(Resources.Message_EngineRunTimeWithMILActivated, new UserPanel.OBD.QualifierItem((QualifierTypes) 1, (string) null, "DT_3295", Resources.Message_MinutesRunByEngineWhileMILActivated)),
        new KeyValuePair<string, UserPanel.OBD.QualifierItem>(Resources.Message_EngineRunTimeSinceMemoryLastCleared, new UserPanel.OBD.QualifierItem((QualifierTypes) 1, (string) null, "DT_3296", Resources.Message_TimeSinceDiagnosticTroubleCodesCleared))
      }, "21")
    };

    public override void UpdateContent(XmlWriter writer)
    {
      WebBrowserList.WriteExpandableContent(writer, true, "ecu", "heading2", "Diagnostic Readiness", (Action) (() =>
      {
        WebBrowserList.WriteExpandableContent(writer, true, "ecu", "heading2", "J1939-61", (Action) (() =>
        {
          this.OutputReadiness(writer, UserPanel.Readiness.groupReadiness, "J1939-61", "ACM", this.obdReportDataAcm);
          this.OutputReadinessMap(writer, UserPanel.Readiness.data, "J1939-61", this.obdReportDataAcm);
        }));
        WebBrowserList.WriteExpandableContent(writer, true, "ecu", "heading2", "J1939-1", (Action) (() =>
        {
          this.OutputReadiness(writer, UserPanel.Readiness.groupReadiness, "J1939-1", "MCM", this.obdReportDataMcm);
          this.OutputReadinessMap(writer, UserPanel.Readiness.data, "J1939-1", this.obdReportDataMcm);
        }));
        WebBrowserList.WriteExpandableContent(writer, true, "ecu", "heading2", "J1939-0", (Action) (() =>
        {
          this.OutputReadiness(writer, UserPanel.Readiness.groupReadiness, "J1939-0", "CPC", this.obdReportDataCpc);
          this.OutputReadinessMap(writer, UserPanel.Readiness.data, "J1939-0", this.obdReportDataCpc);
        }));
      }));
    }

    private void OutputReadiness(
      XmlWriter writer,
      UserPanel.OBD.QualifierGroup[] qualifierGroup,
      string channelName,
      string ecu,
      Dictionary<string, string> obdReportData)
    {
      writer.WriteStartElement("table");
      writer.WriteAttributeString("style", "width:100%;margin-top:5px;margin-bottom:10px;border:1px solid white;");
      writer.WriteStartElement("tr");
      this.WriteTableCell(writer, "TH", UserPanel.OBD.SourceBackgroundColorsIndex.header, "DM");
      this.WriteTableCell(writer, "TH", UserPanel.OBD.SourceBackgroundColorsIndex.header, "SPN");
      this.WriteTableCell(writer, "TH", UserPanel.OBD.SourceBackgroundColorsIndex.header, "Name");
      this.WriteTableCell(writer, "TH", UserPanel.OBD.SourceBackgroundColorsIndex.header, "Value");
      this.WriteTableCell(writer, "TH", UserPanel.OBD.SourceBackgroundColorsIndex.header, "Units");
      writer.WriteEndElement();
      foreach (UserPanel.OBD.QualifierGroup qualifierGroup1 in qualifierGroup)
      {
        foreach (KeyValuePair<string, UserPanel.OBD.QualifierItem> keyValuePair in qualifierGroup1.data)
        {
          UserPanel.OBD.QualifierItem qualifierItem = keyValuePair.Value;
          Qualifier qualifier;
          // ISSUE: explicit constructor call
          ((Qualifier) ref qualifier).\u002Ector(qualifierItem.qualifierType, channelName, qualifierItem.qualifierNumber);
          string text1 = this.QualifierValue(qualifier);
          string text2 = UserPanel.OBD.QualifierUnits(qualifier);
          if (this.IsQualifierValid(qualifier))
          {
            writer.WriteStartElement("tr");
            writer.WriteAttributeString("style", "background:white");
            this.WriteTableCell(writer, "TD", UserPanel.OBD.SourceBackgroundColorsIndex.row, qualifierGroup1.dm);
            this.WriteTableCell(writer, "TD", UserPanel.OBD.SourceBackgroundColorsIndex.row, ((Qualifier) ref qualifier).Name);
            this.WriteTableCell(writer, "TD", UserPanel.OBD.SourceBackgroundColorsIndex.row, string.IsNullOrEmpty(qualifierItem.displayText) ? keyValuePair.Key : $"{qualifierItem.displayText}({ecu})");
            this.WriteTableCell(writer, "TD", UserPanel.OBD.SourceBackgroundColorsIndex.value, text1);
            this.WriteTableCell(writer, "TD", UserPanel.OBD.SourceBackgroundColorsIndex.value, text2);
            writer.WriteEndElement();
            obdReportData.Add(keyValuePair.Key, text1);
          }
        }
      }
      writer.WriteEndElement();
    }

    private void OutputReadinessMap(
      XmlWriter writer,
      UserPanel.Readiness.QualifierMap[] data,
      string channelName,
      Dictionary<string, string> obdReportData)
    {
      writer.WriteStartElement("table");
      writer.WriteAttributeString("style", "width:100%;margin-top:5px;margin-bottom:10px;border:1px solid white;");
      writer.WriteStartElement("tr");
      this.WriteTableCell(writer, "TH", UserPanel.OBD.SourceBackgroundColorsIndex.header, "System");
      this.WriteTableCell(writer, "TH", UserPanel.OBD.SourceBackgroundColorsIndex.header, "DM5 Ready");
      writer.WriteEndElement();
      foreach (UserPanel.Readiness.QualifierMap qualifierMap in data)
      {
        writer.WriteStartElement("tr");
        this.WriteTableCell(writer, "TD", UserPanel.OBD.SourceBackgroundColorsIndex.row, qualifierMap.description);
        Qualifier qualifier;
        // ISSUE: explicit constructor call
        ((Qualifier) ref qualifier).\u002Ector((QualifierTypes) 1, channelName, qualifierMap.supportedQualifier);
        double supported = this.QualifierValueAsDouble(qualifier);
        // ISSUE: explicit constructor call
        ((Qualifier) ref qualifier).\u002Ector((QualifierTypes) 1, channelName, qualifierMap.readyQualifier);
        double status = this.QualifierValueAsDouble(qualifier);
        string readinessText = this.GetReadinessText(supported, status);
        this.WriteTableCell(writer, "TD", UserPanel.OBD.SourceBackgroundColorsIndex.value, readinessText);
        if (!string.IsNullOrEmpty(readinessText))
          this.AddReadinessItems(obdReportData, qualifierMap.description, readinessText);
        writer.WriteEndElement();
      }
      writer.WriteEndElement();
    }

    private void AddReadinessItems(
      Dictionary<string, string> obdReportData,
      string key,
      string value)
    {
      if (!obdReportData.ContainsKey(key))
        obdReportData.Add(key, value);
      else
        obdReportData[key] = value;
    }

    private string GetReadinessText(double supported, double status)
    {
      string readinessText = string.Empty;
      if (supported == 0.0)
        readinessText = Resources.Message_TestNotSupported;
      else if (supported == 1.0 && status == 1.0)
        readinessText = Resources.Message_TestNotComplete;
      else if (supported == 1.0 && status == 0.0)
        readinessText = Resources.Message_TestComplete;
      return readinessText;
    }

    private struct QualifierMap(string description, string ready, string supported)
    {
      public string description = description;
      public string readyQualifier = ready;
      public string supportedQualifier = supported;
    }
  }

  private class FaultCodes : UserPanel.OBD
  {
    private string MILStatusQualifier = "DT_1213";
    private string NumberOfWarmUpCyclesQualifier = "DT_3302";

    private Collection<FaultCode> GetCodes(string channelName)
    {
      Collection<FaultCode> output = new Collection<FaultCode>();
      this.GetChannelByEcu(channelName)?.FaultCodes.CopyCurrent(ReadFunctions.NonPermanent | ReadFunctions.Permanent, output);
      return output;
    }

    public override void UpdateContent(XmlWriter writer)
    {
      WebBrowserList.WriteExpandableContent(writer, true, "ecu", "heading2", "Fault Codes", (Action) (() =>
      {
        WebBrowserList.WriteExpandableContent(writer, true, "ecu", "heading2", "J1939-61", (Action) (() =>
        {
          this.OutputFaultCodeHeader(writer, "J1939-61", this.obdReportDataAcm);
          this.OutputFaultCodes(writer, this.GetCodes("J1939-61"), this.obdReportDataAcm);
        }));
        WebBrowserList.WriteExpandableContent(writer, true, "ecu", "heading2", "J1939-1", (Action) (() =>
        {
          this.OutputFaultCodeHeader(writer, "J1939-1", this.obdReportDataMcm);
          this.OutputFaultCodes(writer, this.GetCodes("J1939-1"), this.obdReportDataMcm);
        }));
        WebBrowserList.WriteExpandableContent(writer, true, "ecu", "heading2", "J1939-0", (Action) (() =>
        {
          this.OutputFaultCodeHeader(writer, "J1939-0", this.obdReportDataCpc);
          this.OutputFaultCodes(writer, this.GetCodes("J1939-0"), this.obdReportDataCpc);
        }));
      }));
    }

    private void OutputFaultCodeHeader(
      XmlWriter writer,
      string channelName,
      Dictionary<string, string> obdReportData)
    {
      writer.WriteStartElement("table");
      writer.WriteAttributeString("style", "width:100%;margin-top:5px;margin-bottom:10px;border:1px solid white;");
      writer.WriteStartElement("tr");
      this.WriteTableCell(writer, "TH", UserPanel.OBD.SourceBackgroundColorsIndex.header, Resources.Message_DM1MILStatusSPN1213);
      this.WriteTableCell(writer, "TH", UserPanel.OBD.SourceBackgroundColorsIndex.header, Resources.Message_DM26NumberWarmupCyclesSinceDTCClearSPN3302);
      writer.WriteEndElement();
      writer.WriteStartElement("tr");
      Qualifier qualifier1;
      // ISSUE: explicit constructor call
      ((Qualifier) ref qualifier1).\u002Ector((QualifierTypes) 1, channelName, this.MILStatusQualifier);
      string text1 = this.QualifierValue(qualifier1);
      this.WriteTableCell(writer, "TD", UserPanel.OBD.SourceBackgroundColorsIndex.value, text1);
      obdReportData.Add(Resources.Message_DM1MILStatusSPN1213, text1);
      Qualifier qualifier2;
      // ISSUE: explicit constructor call
      ((Qualifier) ref qualifier2).\u002Ector((QualifierTypes) 1, channelName, this.NumberOfWarmUpCyclesQualifier);
      string text2 = this.QualifierValue(qualifier2);
      this.WriteTableCell(writer, "TD", UserPanel.OBD.SourceBackgroundColorsIndex.value, text2);
      obdReportData.Add(Resources.Message_DM26NumberWarmupCyclesSinceDTCClearSPN3302, text2);
      writer.WriteEndElement();
      writer.WriteEndElement();
    }

    private void OutputFaultCodes(
      XmlWriter writer,
      Collection<FaultCode> codes,
      Dictionary<string, string> obdReportData)
    {
      bool flag1 = false;
      foreach (FaultCode code in codes)
      {
        FaultCodeIncident currentByFunction = code.FaultCodeIncidents.GetCurrentByFunction(ReadFunctions.NonPermanent | ReadFunctions.Permanent);
        if (currentByFunction != null)
        {
          bool flag2 = currentByFunction.Mil == MilStatus.On;
          bool flag3 = !flag2 && currentByFunction.Stored == StoredStatus.Stored;
          bool flag4 = (currentByFunction.Functions & ReadFunctions.Permanent) == ReadFunctions.Permanent;
          bool flag5 = currentByFunction.Pending == PendingStatus.Pending;
          if (flag2 || flag3 || flag4 || flag5)
          {
            if (!flag1)
            {
              this.AddFaultCodesHeader(writer);
              flag1 = true;
            }
            writer.WriteStartElement("tr");
            this.WriteTableCell(writer, "TD", UserPanel.OBD.SourceBackgroundColorsIndex.row, code.Number);
            this.WriteTableCell(writer, "TD", UserPanel.OBD.SourceBackgroundColorsIndex.row, code.Mode);
            this.WriteTableCell(writer, "TD", UserPanel.OBD.SourceBackgroundColorsIndex.row, code.Text);
            this.WriteTableCell(writer, "TD", UserPanel.OBD.SourceBackgroundColorsIndex.row, flag5.ToString());
            this.WriteTableCell(writer, "TD", UserPanel.OBD.SourceBackgroundColorsIndex.row, flag2.ToString());
            this.WriteTableCell(writer, "TD", UserPanel.OBD.SourceBackgroundColorsIndex.row, flag4.ToString());
            this.WriteTableCell(writer, "TD", UserPanel.OBD.SourceBackgroundColorsIndex.row, flag3.ToString());
            writer.WriteEndElement();
          }
          if (flag2)
            this.AddFaultCode(obdReportData, code, Resources.Message_ActiveFaultCodes);
          if (flag3)
            this.AddFaultCode(obdReportData, code, Resources.Message_PreviouslyActiveFaultCodes);
          if (flag5)
            this.AddFaultCode(obdReportData, code, Resources.Message_PendingFaultCodes);
          if (flag4)
            this.AddFaultCode(obdReportData, code, Resources.Message_PermanentFaultCodes);
        }
      }
      if (flag1)
      {
        writer.WriteEndElement();
      }
      else
      {
        writer.WriteStartElement("span");
        writer.WriteString(Resources.Message_NoOBDRelevantFaultCodesCurrentlyReportedForThisDevice);
        writer.WriteEndElement();
      }
    }

    private void AddFaultCode(
      Dictionary<string, string> obdReportData,
      FaultCode faultCode,
      string item)
    {
      if (!obdReportData.ContainsKey(item))
      {
        obdReportData.Add(item, $"{faultCode.Number} {faultCode.Mode}");
      }
      else
      {
        string str = obdReportData[item] + $", {faultCode.Number} {faultCode.Mode}";
        obdReportData[item] = str;
      }
    }

    private void AddFaultCodesHeader(XmlWriter writer)
    {
      writer.WriteStartElement("table");
      writer.WriteAttributeString("style", "width:100%;margin-top:5px;margin-bottom:10px;border:1px solid white;");
      writer.WriteStartElement("tr");
      this.WriteTableCell(writer, "TH", UserPanel.OBD.SourceBackgroundColorsIndex.header, "SPN");
      this.WriteTableCell(writer, "TH", UserPanel.OBD.SourceBackgroundColorsIndex.header, "FMI");
      this.WriteTableCell(writer, "TH", UserPanel.OBD.SourceBackgroundColorsIndex.header, "Description");
      this.WriteTableCell(writer, "TH", UserPanel.OBD.SourceBackgroundColorsIndex.header, Resources.Message_DM6Pending);
      this.WriteTableCell(writer, "TH", UserPanel.OBD.SourceBackgroundColorsIndex.header, Resources.Message_DM12MILON);
      this.WriteTableCell(writer, "TH", UserPanel.OBD.SourceBackgroundColorsIndex.header, Resources.Message_DM28Permanent);
      this.WriteTableCell(writer, "TH", UserPanel.OBD.SourceBackgroundColorsIndex.header, Resources.Message_DM23PreviouslyMILON);
      writer.WriteEndElement();
    }
  }
}
