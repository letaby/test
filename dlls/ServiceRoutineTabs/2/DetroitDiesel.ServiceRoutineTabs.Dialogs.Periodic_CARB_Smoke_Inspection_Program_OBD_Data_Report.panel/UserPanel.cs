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
using DetroitDiesel.Common;
using DetroitDiesel.Windows.Forms;
using DetroitDiesel.Windows.Forms.Diagnostics.Panels.Instruments;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Periodic_CARB_Smoke_Inspection_Program_OBD_Data_Report.panel;

public class UserPanel : CustomPanel
{
	private class OBD
	{
		public enum SourceBackgroundColorsIndex
		{
			header,
			row,
			value,
			unit
		}

		public struct QualifierItem
		{
			public QualifierTypes qualifierType;

			public string qualifierNumber;

			public string displayText;

			public string channelName;

			public string dm;

			public QualifierItem(QualifierTypes type, string channelName, string qualifierNumber, string displayText = "", string dm = "")
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0003: Unknown result type (might be due to invalid IL or missing references)
				qualifierType = type;
				this.qualifierNumber = qualifierNumber;
				this.channelName = channelName;
				this.displayText = displayText;
				this.dm = dm;
			}
		}

		public struct QualifierGroup
		{
			public string name;

			public KeyValuePair<string, QualifierItem>[] data;

			public string dm;

			public QualifierGroup(string groupname, KeyValuePair<string, QualifierItem>[] data, string dm = "")
			{
				name = groupname;
				this.data = data;
				this.dm = dm;
			}
		}

		public Dictionary<string, string> obdReportDataAcm = new Dictionary<string, string>();

		public Dictionary<string, string> obdReportDataMcm = new Dictionary<string, string>();

		public Dictionary<string, string> obdReportDataCpc = new Dictionary<string, string>();

		public string[] sourceBackgroundColors = new string[4] { "#E4F4F4", "#FFFCCF", "#CFFBA6", "#D1ECC1" };

		private readonly Regex removeUnprintables = new Regex("[\\x01-\\x1F]", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);

		public virtual void UpdateContent(XmlWriter writer)
		{
		}

		public string QualifierValue(Qualifier qualifier)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			IEnumerable<Channel> activeChannels = SapiManager.GlobalInstance.ActiveChannels;
			DataItem val = DataItem.Create(qualifier, activeChannels);
			try
			{
				if (val != null)
				{
					string input = val.ValueAsString(val.Value);
					return removeUnprintables.Replace(input, "");
				}
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
			return null;
		}

		protected double QualifierValueAsDouble(Qualifier qualifier)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			IEnumerable<Channel> activeChannels = SapiManager.GlobalInstance.ActiveChannels;
			DataItem val = DataItem.Create(qualifier, activeChannels);
			try
			{
				if (val != null)
				{
					return val.ValueAsDouble(val.Value);
				}
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
			return double.NaN;
		}

		protected bool IsQualifierValid(Qualifier qualifier)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return DataItem.Create(qualifier, (IEnumerable<Channel>)SapiManager.GlobalInstance.ActiveChannels) != null;
		}

		protected static string QualifierUnits(Qualifier qualifier)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			string result = string.Empty;
			DataItem val = DataItem.Create(qualifier, (IEnumerable<Channel>)SapiManager.GlobalInstance.ActiveChannels);
			try
			{
				if (val != null)
				{
					result = val.Units;
				}
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
			return result;
		}

		protected Channel GetChannelByEcu(string ecuName)
		{
			return SapiManager.GlobalInstance.ActiveChannels.Where((Channel ch) => ch.Ecu.Name == ecuName).FirstOrDefault();
		}

		protected void WriteTableCell(XmlWriter writer, string elementType, SourceBackgroundColorsIndex backgroundColor, string text, string style = null)
		{
			writer.WriteStartElement(elementType);
			if (!string.IsNullOrEmpty(style))
			{
				writer.WriteAttributeString("style", style);
			}
			writer.WriteAttributeString("style", "background:" + sourceBackgroundColors[(int)backgroundColor]);
			writer.WriteString(text);
			writer.WriteEndElement();
		}
	}

	private class Identification : OBD
	{
		private static QualifierGroup[] groupIdentification = new QualifierGroup[1]
		{
			new QualifierGroup(Resources.Message_DM19CalibrationInformation, new KeyValuePair<string, QualifierItem>[4]
			{
				new KeyValuePair<string, QualifierItem>(Resources.Message_CalibrationIdentificationNumber, new QualifierItem((QualifierTypes)8, null, "1635[0]", Resources.Message_CalibrationIdentificationNumber, "19")),
				new KeyValuePair<string, QualifierItem>(Resources.Message_CalibrationVerificationNumber, new QualifierItem((QualifierTypes)8, null, "1634[0]", Resources.Message_CalibrationVerificationNumber, "19")),
				new KeyValuePair<string, QualifierItem>(Resources.Message_VehicleIdentificationNumber, new QualifierItem((QualifierTypes)8, null, "237", Resources.Message_VehicleIdentificationNumber)),
				new KeyValuePair<string, QualifierItem>(Resources.Message_EngineSerialNumber, new QualifierItem((QualifierTypes)8, null, "588", Resources.Message_EngineSerialNumber))
			})
		};

		public override void UpdateContent(XmlWriter writer)
		{
			string[] array = new string[5] { "#EBF5FB", "#FEF9E7", "#E8F6F3", "#EAFAF1", "#F4ECF7" };
			WebBrowserList.WriteExpandableContent(writer, true, "ecu", "heading2", "Identification", (Action)delegate
			{
				WebBrowserList.WriteExpandableContent(writer, true, "ecu", "heading2", "J1939-61", (Action)delegate
				{
					OutputIdentification(writer, groupIdentification, "J1939-61", obdReportDataAcm);
				});
				WebBrowserList.WriteExpandableContent(writer, true, "ecu", "heading2", "J1939-1", (Action)delegate
				{
					OutputIdentification(writer, groupIdentification, "J1939-1", obdReportDataMcm);
				});
				WebBrowserList.WriteExpandableContent(writer, true, "ecu", "heading2", "J1939-0", (Action)delegate
				{
					OutputIdentification(writer, groupIdentification, "J1939-0", obdReportDataCpc);
				});
			});
		}

		private void OutputIdentification(XmlWriter writer, QualifierGroup[] qualifierGroup, string channelName, Dictionary<string, string> obdReportData)
		{
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			writer.WriteStartElement("table");
			writer.WriteAttributeString("style", "width:100%;margin-top:5px;margin-bottom:10px;border:1px solid white;");
			writer.WriteStartElement("tr");
			WriteTableCell(writer, "TH", SourceBackgroundColorsIndex.header, "DM");
			WriteTableCell(writer, "TH", SourceBackgroundColorsIndex.header, "SPN");
			WriteTableCell(writer, "TH", SourceBackgroundColorsIndex.header, "Name");
			WriteTableCell(writer, "TH", SourceBackgroundColorsIndex.header, "Value");
			writer.WriteEndElement();
			Qualifier qualifier = default(Qualifier);
			for (int i = 0; i < qualifierGroup.Length; i++)
			{
				QualifierGroup qualifierGroup2 = qualifierGroup[i];
				KeyValuePair<string, QualifierItem>[] data = qualifierGroup2.data;
				for (int j = 0; j < data.Length; j++)
				{
					KeyValuePair<string, QualifierItem> keyValuePair = data[j];
					writer.WriteStartElement("tr");
					QualifierItem value = keyValuePair.Value;
					((Qualifier)(ref qualifier))._002Ector(value.qualifierType, channelName, value.qualifierNumber);
					string text = QualifierValue(qualifier);
					WriteTableCell(writer, "TD", SourceBackgroundColorsIndex.row, value.dm);
					WriteTableCell(writer, "TD", SourceBackgroundColorsIndex.row, ((Qualifier)(ref qualifier)).Name);
					WriteTableCell(writer, "TD", SourceBackgroundColorsIndex.row, string.IsNullOrEmpty(value.displayText) ? keyValuePair.Key : value.displayText);
					WriteTableCell(writer, "TD", SourceBackgroundColorsIndex.value, text);
					writer.WriteEndElement();
					obdReportData.Add(keyValuePair.Key, text);
				}
			}
			writer.WriteEndElement();
		}
	}

	private class Readiness : OBD
	{
		private struct QualifierMap
		{
			public string description;

			public string readyQualifier;

			public string supportedQualifier;

			public QualifierMap(string description, string ready, string supported)
			{
				this.description = description;
				readyQualifier = ready;
				supportedQualifier = supported;
			}
		}

		private static string OBDComplianceQualifier = "1220";

		private static QualifierMap[] data = new QualifierMap[10]
		{
			new QualifierMap(Resources.Message_ComprehensiveComponent, "DT_1221_4_7", "DT_1221_4_3"),
			new QualifierMap(Resources.Message_EGR, "DT_1223_7_8", "DT_1222_5_8"),
			new QualifierMap(Resources.Message_ExhaustGasSensor, "DT_1223_7_6", "DT_1222_5_6"),
			new QualifierMap(Resources.Message_ExhaustGasSensorHeater, "DT_1223_7_7", "DT_1222_5_7"),
			new QualifierMap(Resources.Message_EngineFuelSystem, "DT_1221_4_6", "DT_1221_4_2"),
			new QualifierMap(Resources.Message_Misfire, "DT_1221_4_5", "DT_1221_4_1"),
			new QualifierMap(Resources.Message_NMHCCatalyst, "DT_1223_8_5", "DT_1222_6_5"),
			new QualifierMap(Resources.Message_NOxConvertingCatalyst, "DT_1223_8_4", "DT_1222_6_4"),
			new QualifierMap(Resources.Message_DieselParticulateFilter, "DT_1223_8_3", "DT_1222_6_3"),
			new QualifierMap(Resources.Message_BoostPressureControlSystem, "DT_1223_8_2", "DT_1222_6_2")
		};

		private static QualifierGroup[] groupReadiness = new QualifierGroup[2]
		{
			new QualifierGroup(Resources.Message_EngineTotals, new KeyValuePair<string, QualifierItem>[1]
			{
				new KeyValuePair<string, QualifierItem>(Resources.Message_OBDRequirementsToWhichEngineIsCertified, new QualifierItem((QualifierTypes)8, "J1939-61", OBDComplianceQualifier, Resources.Message_OBDCompliance))
			}, "5"),
			new QualifierGroup(Resources.Message_EngineTotals, new KeyValuePair<string, QualifierItem>[4]
			{
				new KeyValuePair<string, QualifierItem>(Resources.Message_DistanceTraveledWithMILActivated, new QualifierItem((QualifierTypes)1, null, "DT_3069", Resources.Message_DistanceTravelledWhileMILIsActivated)),
				new KeyValuePair<string, QualifierItem>(Resources.Message_DistanceTraveledSinceMemoryLastCleared, new QualifierItem((QualifierTypes)1, null, "DT_3294", Resources.Message_DistanceSinceDiagnosticTroubleCodesCleared)),
				new KeyValuePair<string, QualifierItem>(Resources.Message_EngineRunTimeWithMILActivated, new QualifierItem((QualifierTypes)1, null, "DT_3295", Resources.Message_MinutesRunByEngineWhileMILActivated)),
				new KeyValuePair<string, QualifierItem>(Resources.Message_EngineRunTimeSinceMemoryLastCleared, new QualifierItem((QualifierTypes)1, null, "DT_3296", Resources.Message_TimeSinceDiagnosticTroubleCodesCleared))
			}, "21")
		};

		public override void UpdateContent(XmlWriter writer)
		{
			WebBrowserList.WriteExpandableContent(writer, true, "ecu", "heading2", "Diagnostic Readiness", (Action)delegate
			{
				WebBrowserList.WriteExpandableContent(writer, true, "ecu", "heading2", "J1939-61", (Action)delegate
				{
					OutputReadiness(writer, groupReadiness, "J1939-61", "ACM", obdReportDataAcm);
					OutputReadinessMap(writer, data, "J1939-61", obdReportDataAcm);
				});
				WebBrowserList.WriteExpandableContent(writer, true, "ecu", "heading2", "J1939-1", (Action)delegate
				{
					OutputReadiness(writer, groupReadiness, "J1939-1", "MCM", obdReportDataMcm);
					OutputReadinessMap(writer, data, "J1939-1", obdReportDataMcm);
				});
				WebBrowserList.WriteExpandableContent(writer, true, "ecu", "heading2", "J1939-0", (Action)delegate
				{
					OutputReadiness(writer, groupReadiness, "J1939-0", "CPC", obdReportDataCpc);
					OutputReadinessMap(writer, data, "J1939-0", obdReportDataCpc);
				});
			});
		}

		private void OutputReadiness(XmlWriter writer, QualifierGroup[] qualifierGroup, string channelName, string ecu, Dictionary<string, string> obdReportData)
		{
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			writer.WriteStartElement("table");
			writer.WriteAttributeString("style", "width:100%;margin-top:5px;margin-bottom:10px;border:1px solid white;");
			writer.WriteStartElement("tr");
			WriteTableCell(writer, "TH", SourceBackgroundColorsIndex.header, "DM");
			WriteTableCell(writer, "TH", SourceBackgroundColorsIndex.header, "SPN");
			WriteTableCell(writer, "TH", SourceBackgroundColorsIndex.header, "Name");
			WriteTableCell(writer, "TH", SourceBackgroundColorsIndex.header, "Value");
			WriteTableCell(writer, "TH", SourceBackgroundColorsIndex.header, "Units");
			writer.WriteEndElement();
			Qualifier qualifier = default(Qualifier);
			for (int i = 0; i < qualifierGroup.Length; i++)
			{
				QualifierGroup qualifierGroup2 = qualifierGroup[i];
				KeyValuePair<string, QualifierItem>[] array = qualifierGroup2.data;
				for (int j = 0; j < array.Length; j++)
				{
					KeyValuePair<string, QualifierItem> keyValuePair = array[j];
					QualifierItem value = keyValuePair.Value;
					((Qualifier)(ref qualifier))._002Ector(value.qualifierType, channelName, value.qualifierNumber);
					string text = QualifierValue(qualifier);
					string text2 = OBD.QualifierUnits(qualifier);
					if (IsQualifierValid(qualifier))
					{
						writer.WriteStartElement("tr");
						writer.WriteAttributeString("style", "background:white");
						WriteTableCell(writer, "TD", SourceBackgroundColorsIndex.row, qualifierGroup2.dm);
						WriteTableCell(writer, "TD", SourceBackgroundColorsIndex.row, ((Qualifier)(ref qualifier)).Name);
						WriteTableCell(writer, "TD", SourceBackgroundColorsIndex.row, string.IsNullOrEmpty(value.displayText) ? keyValuePair.Key : (value.displayText + "(" + ecu + ")"));
						WriteTableCell(writer, "TD", SourceBackgroundColorsIndex.value, text);
						WriteTableCell(writer, "TD", SourceBackgroundColorsIndex.value, text2);
						writer.WriteEndElement();
						obdReportData.Add(keyValuePair.Key, text);
					}
				}
			}
			writer.WriteEndElement();
		}

		private void OutputReadinessMap(XmlWriter writer, QualifierMap[] data, string channelName, Dictionary<string, string> obdReportData)
		{
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			writer.WriteStartElement("table");
			writer.WriteAttributeString("style", "width:100%;margin-top:5px;margin-bottom:10px;border:1px solid white;");
			writer.WriteStartElement("tr");
			WriteTableCell(writer, "TH", SourceBackgroundColorsIndex.header, "System");
			WriteTableCell(writer, "TH", SourceBackgroundColorsIndex.header, "DM5 Ready");
			writer.WriteEndElement();
			Qualifier qualifier = default(Qualifier);
			for (int i = 0; i < data.Length; i++)
			{
				QualifierMap qualifierMap = data[i];
				writer.WriteStartElement("tr");
				WriteTableCell(writer, "TD", SourceBackgroundColorsIndex.row, qualifierMap.description);
				((Qualifier)(ref qualifier))._002Ector((QualifierTypes)1, channelName, qualifierMap.supportedQualifier);
				double supported = QualifierValueAsDouble(qualifier);
				((Qualifier)(ref qualifier))._002Ector((QualifierTypes)1, channelName, qualifierMap.readyQualifier);
				double status = QualifierValueAsDouble(qualifier);
				string readinessText = GetReadinessText(supported, status);
				WriteTableCell(writer, "TD", SourceBackgroundColorsIndex.value, readinessText);
				if (!string.IsNullOrEmpty(readinessText))
				{
					AddReadinessItems(obdReportData, qualifierMap.description, readinessText);
				}
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}

		private void AddReadinessItems(Dictionary<string, string> obdReportData, string key, string value)
		{
			if (!obdReportData.ContainsKey(key))
			{
				obdReportData.Add(key, value);
			}
			else
			{
				obdReportData[key] = value;
			}
		}

		private string GetReadinessText(double supported, double status)
		{
			string result = string.Empty;
			if (supported == 0.0)
			{
				result = Resources.Message_TestNotSupported;
			}
			else if (supported == 1.0 && status == 1.0)
			{
				result = Resources.Message_TestNotComplete;
			}
			else if (supported == 1.0 && status == 0.0)
			{
				result = Resources.Message_TestComplete;
			}
			return result;
		}
	}

	private class FaultCodes : OBD
	{
		private string MILStatusQualifier = "DT_1213";

		private string NumberOfWarmUpCyclesQualifier = "DT_3302";

		private Collection<FaultCode> GetCodes(string channelName)
		{
			Collection<FaultCode> collection = new Collection<FaultCode>();
			GetChannelByEcu(channelName)?.FaultCodes.CopyCurrent(ReadFunctions.NonPermanent | ReadFunctions.Permanent, collection);
			return collection;
		}

		public override void UpdateContent(XmlWriter writer)
		{
			WebBrowserList.WriteExpandableContent(writer, true, "ecu", "heading2", "Fault Codes", (Action)delegate
			{
				WebBrowserList.WriteExpandableContent(writer, true, "ecu", "heading2", "J1939-61", (Action)delegate
				{
					OutputFaultCodeHeader(writer, "J1939-61", obdReportDataAcm);
					OutputFaultCodes(writer, GetCodes("J1939-61"), obdReportDataAcm);
				});
				WebBrowserList.WriteExpandableContent(writer, true, "ecu", "heading2", "J1939-1", (Action)delegate
				{
					OutputFaultCodeHeader(writer, "J1939-1", obdReportDataMcm);
					OutputFaultCodes(writer, GetCodes("J1939-1"), obdReportDataMcm);
				});
				WebBrowserList.WriteExpandableContent(writer, true, "ecu", "heading2", "J1939-0", (Action)delegate
				{
					OutputFaultCodeHeader(writer, "J1939-0", obdReportDataCpc);
					OutputFaultCodes(writer, GetCodes("J1939-0"), obdReportDataCpc);
				});
			});
		}

		private void OutputFaultCodeHeader(XmlWriter writer, string channelName, Dictionary<string, string> obdReportData)
		{
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			writer.WriteStartElement("table");
			writer.WriteAttributeString("style", "width:100%;margin-top:5px;margin-bottom:10px;border:1px solid white;");
			writer.WriteStartElement("tr");
			WriteTableCell(writer, "TH", SourceBackgroundColorsIndex.header, Resources.Message_DM1MILStatusSPN1213);
			WriteTableCell(writer, "TH", SourceBackgroundColorsIndex.header, Resources.Message_DM26NumberWarmupCyclesSinceDTCClearSPN3302);
			writer.WriteEndElement();
			writer.WriteStartElement("tr");
			Qualifier qualifier = default(Qualifier);
			((Qualifier)(ref qualifier))._002Ector((QualifierTypes)1, channelName, MILStatusQualifier);
			string text = QualifierValue(qualifier);
			WriteTableCell(writer, "TD", SourceBackgroundColorsIndex.value, text);
			obdReportData.Add(Resources.Message_DM1MILStatusSPN1213, text);
			Qualifier qualifier2 = default(Qualifier);
			((Qualifier)(ref qualifier2))._002Ector((QualifierTypes)1, channelName, NumberOfWarmUpCyclesQualifier);
			text = QualifierValue(qualifier2);
			WriteTableCell(writer, "TD", SourceBackgroundColorsIndex.value, text);
			obdReportData.Add(Resources.Message_DM26NumberWarmupCyclesSinceDTCClearSPN3302, text);
			writer.WriteEndElement();
			writer.WriteEndElement();
		}

		private void OutputFaultCodes(XmlWriter writer, Collection<FaultCode> codes, Dictionary<string, string> obdReportData)
		{
			bool flag = false;
			foreach (FaultCode code in codes)
			{
				FaultCodeIncident currentByFunction = code.FaultCodeIncidents.GetCurrentByFunction(ReadFunctions.NonPermanent | ReadFunctions.Permanent);
				if (currentByFunction == null)
				{
					continue;
				}
				bool flag2 = currentByFunction.Mil == MilStatus.On;
				bool flag3 = !flag2 && currentByFunction.Stored == StoredStatus.Stored;
				bool flag4 = (currentByFunction.Functions & ReadFunctions.Permanent) == ReadFunctions.Permanent;
				bool flag5 = currentByFunction.Pending == PendingStatus.Pending;
				if (flag2 || flag3 || flag4 || flag5)
				{
					if (!flag)
					{
						AddFaultCodesHeader(writer);
						flag = true;
					}
					writer.WriteStartElement("tr");
					WriteTableCell(writer, "TD", SourceBackgroundColorsIndex.row, code.Number);
					WriteTableCell(writer, "TD", SourceBackgroundColorsIndex.row, code.Mode);
					WriteTableCell(writer, "TD", SourceBackgroundColorsIndex.row, code.Text);
					WriteTableCell(writer, "TD", SourceBackgroundColorsIndex.row, flag5.ToString());
					WriteTableCell(writer, "TD", SourceBackgroundColorsIndex.row, flag2.ToString());
					WriteTableCell(writer, "TD", SourceBackgroundColorsIndex.row, flag4.ToString());
					WriteTableCell(writer, "TD", SourceBackgroundColorsIndex.row, flag3.ToString());
					writer.WriteEndElement();
				}
				if (flag2)
				{
					AddFaultCode(obdReportData, code, Resources.Message_ActiveFaultCodes);
				}
				if (flag3)
				{
					AddFaultCode(obdReportData, code, Resources.Message_PreviouslyActiveFaultCodes);
				}
				if (flag5)
				{
					AddFaultCode(obdReportData, code, Resources.Message_PendingFaultCodes);
				}
				if (flag4)
				{
					AddFaultCode(obdReportData, code, Resources.Message_PermanentFaultCodes);
				}
			}
			if (flag)
			{
				writer.WriteEndElement();
				return;
			}
			writer.WriteStartElement("span");
			writer.WriteString(Resources.Message_NoOBDRelevantFaultCodesCurrentlyReportedForThisDevice);
			writer.WriteEndElement();
		}

		private void AddFaultCode(Dictionary<string, string> obdReportData, FaultCode faultCode, string item)
		{
			if (!obdReportData.ContainsKey(item))
			{
				obdReportData.Add(item, $"{faultCode.Number} {faultCode.Mode}");
				return;
			}
			string text = obdReportData[item];
			text += $", {faultCode.Number} {faultCode.Mode}";
			obdReportData[item] = text;
		}

		private void AddFaultCodesHeader(XmlWriter writer)
		{
			writer.WriteStartElement("table");
			writer.WriteAttributeString("style", "width:100%;margin-top:5px;margin-bottom:10px;border:1px solid white;");
			writer.WriteStartElement("tr");
			WriteTableCell(writer, "TH", SourceBackgroundColorsIndex.header, "SPN");
			WriteTableCell(writer, "TH", SourceBackgroundColorsIndex.header, "FMI");
			WriteTableCell(writer, "TH", SourceBackgroundColorsIndex.header, "Description");
			WriteTableCell(writer, "TH", SourceBackgroundColorsIndex.header, Resources.Message_DM6Pending);
			WriteTableCell(writer, "TH", SourceBackgroundColorsIndex.header, Resources.Message_DM12MILON);
			WriteTableCell(writer, "TH", SourceBackgroundColorsIndex.header, Resources.Message_DM28Permanent);
			WriteTableCell(writer, "TH", SourceBackgroundColorsIndex.header, Resources.Message_DM23PreviouslyMILON);
			writer.WriteEndElement();
		}
	}

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

	private Identification identification;

	private Readiness readiness;

	private FaultCodes faultCodes;

	private bool isGenerated = false;

	private DateTime generatedDate;

	private TableLayoutPanel tableLayoutPanel1;

	private Button buttonExport;

	private Button buttonOK;

	private Panel panelObd;

	public UserPanel()
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Expected O, but got Unknown
		InitializeComponent();
		if (SapiManager.GlobalInstance.LogFileIsOpen)
		{
			LogFile logFile = SapiManager.GlobalInstance.LogFileAllChannels.First().LogFile;
			generatedDate = logFile.CurrentTime;
		}
		else
		{
			generatedDate = DateTime.Now;
		}
		webBrowserList = new WebBrowserList();
		webBrowserList.SetWriterFunction((Action<XmlWriter>)UpdateContent);
		((Control)(object)webBrowserList).Dock = DockStyle.Fill;
		panelObd.Controls.Add((Control)(object)webBrowserList);
	}

	private void UpdateContent(XmlWriter writer)
	{
		if (!isGenerated)
		{
			CreateHeader(writer);
			identification = new Identification();
			identification.UpdateContent(writer);
			readiness = new Readiness();
			readiness.UpdateContent(writer);
			faultCodes = new FaultCodes();
			faultCodes.UpdateContent(writer);
			isGenerated = true;
		}
	}

	private string GetValue(Dictionary<string, string> obdReportData, string item, bool addQuotes)
	{
		string text = string.Empty;
		if (obdReportData == null)
		{
			text = item;
		}
		else if (obdReportData.ContainsKey(item))
		{
			text = obdReportData[item];
		}
		if (string.IsNullOrEmpty(text))
		{
			return ",";
		}
		if (addQuotes)
		{
			return $"\"{text}\",";
		}
		return $"{text},";
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
		writer.WriteString($"{generatedDate.ToLongDateString()} {generatedDate.ToLongTimeString()}");
		writer.WriteEndElement();
	}

	private void buttonExport_Click(object sender, EventArgs e)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		Qualifier qualifier = default(Qualifier);
		((Qualifier)(ref qualifier))._002Ector((QualifierTypes)8, "J1939-0", "237");
		string arg = identification.QualifierValue(qualifier);
		string fileName = string.Format("CARB Smoke Report {0} {1} {2}.csv", arg, generatedDate.ToString("yyyyMMdd", DateTimeFormatInfo.InvariantInfo), generatedDate.ToString("HHmmss", DateTimeFormatInfo.InvariantInfo));
		SaveFileDialog saveFileDialog = new SaveFileDialog();
		saveFileDialog.DefaultExt = ".csv";
		saveFileDialog.FileName = fileName;
		saveFileDialog.Filter = Resources.CSVFileFilter;
		saveFileDialog.AddExtension = true;
		saveFileDialog.SupportMultiDottedExtensions = true;
		saveFileDialog.Title = Resources.DialogTitleSelectExportDestination;
		saveFileDialog.ValidateNames = true;
		saveFileDialog.AutoUpgradeEnabled = true;
		saveFileDialog.OverwritePrompt = true;
		using SaveFileDialog saveFileDialog2 = saveFileDialog;
		if (saveFileDialog2.ShowDialog() == DialogResult.OK)
		{
			using (TextWriter writer = new StreamWriter(saveFileDialog2.FileName))
			{
				WriteHeader(writer);
				WriteBody(writer, "J1939-61", faultCodes.obdReportDataAcm, identification.obdReportDataAcm, readiness.obdReportDataAcm);
				WriteBody(writer, "J1939-1", faultCodes.obdReportDataMcm, identification.obdReportDataMcm, readiness.obdReportDataMcm);
				WriteBody(writer, "J1939-0", faultCodes.obdReportDataCpc, identification.obdReportDataCpc, readiness.obdReportDataCpc);
				return;
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

	private void WriteBody(TextWriter writer, string sourceAddress, Dictionary<string, string> faultCodeData, Dictionary<string, string> identificationData, Dictionary<string, string> readinessData)
	{
		writer.Write(GetValue(null, sourceAddress, addQuotes: false));
		writer.Write(GetValue(faultCodeData, Resources.Message_DM1MILStatusSPN1213, addQuotes: false));
		writer.Write(GetValue(faultCodeData, Resources.Message_ActiveFaultCodes, addQuotes: true));
		writer.Write(GetValue(faultCodeData, Resources.Message_PreviouslyActiveFaultCodes, addQuotes: true));
		writer.Write(GetValue(faultCodeData, Resources.Message_PendingFaultCodes, addQuotes: true));
		writer.Write(GetValue(faultCodeData, Resources.Message_PermanentFaultCodes, addQuotes: true));
		writer.Write(GetValue(readinessData, Resources.Message_ComprehensiveComponent, addQuotes: false));
		writer.Write(GetValue(readinessData, Resources.Message_EGR, addQuotes: false));
		writer.Write(GetValue(readinessData, Resources.Message_ExhaustGasSensor, addQuotes: false));
		writer.Write(GetValue(readinessData, Resources.Message_ExhaustGasSensorHeater, addQuotes: false));
		writer.Write(GetValue(readinessData, Resources.Message_EngineFuelSystem, addQuotes: false));
		writer.Write(GetValue(readinessData, Resources.Message_Misfire, addQuotes: false));
		writer.Write(GetValue(readinessData, Resources.Message_NMHCCatalyst, addQuotes: false));
		writer.Write(GetValue(readinessData, Resources.Message_NOxConvertingCatalyst, addQuotes: false));
		writer.Write(GetValue(readinessData, Resources.Message_DieselParticulateFilter, addQuotes: false));
		writer.Write(GetValue(readinessData, Resources.Message_BoostPressureControlSystem, addQuotes: false));
		writer.Write(GetValue(identificationData, Resources.Message_VehicleIdentificationNumber, addQuotes: false));
		writer.Write(GetValue(identificationData, Resources.Message_EngineSerialNumber, addQuotes: false));
		writer.Write(GetValue(identificationData, Resources.Message_CalibrationIdentificationNumber, addQuotes: false));
		writer.Write(GetValue(identificationData, Resources.Message_CalibrationVerificationNumber, addQuotes: false));
		writer.Write(GetValue(readinessData, Resources.Message_OBDRequirementsToWhichEngineIsCertified, addQuotes: false));
		writer.Write(GetValue(readinessData, Resources.Message_DistanceTraveledWithMILActivated, addQuotes: false));
		writer.Write(GetValue(readinessData, Resources.Message_EngineRunTimeWithMILActivated, addQuotes: false));
		writer.Write(GetValue(readinessData, Resources.Message_DistanceTraveledSinceMemoryLastCleared, addQuotes: false));
		writer.Write(GetValue(readinessData, Resources.Message_EngineRunTimeSinceMemoryLastCleared, addQuotes: false));
		writer.Write(GetValue(faultCodeData, Resources.Message_DM26NumberWarmupCyclesSinceDTCClearSPN3302, addQuotes: false));
		writer.WriteLine();
	}

	private void InitializeComponent()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Expected O, but got Unknown
		tableLayoutPanel1 = new TableLayoutPanel();
		buttonOK = new Button();
		panelObd = new Panel();
		buttonExport = new Button();
		((Control)(object)tableLayoutPanel1).SuspendLayout();
		((Control)this).SuspendLayout();
		((TableLayoutPanel)(object)tableLayoutPanel1).ColumnCount = 4;
		((TableLayoutPanel)(object)tableLayoutPanel1).ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80f));
		((TableLayoutPanel)(object)tableLayoutPanel1).ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80f));
		((TableLayoutPanel)(object)tableLayoutPanel1).ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
		((TableLayoutPanel)(object)tableLayoutPanel1).ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80f));
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonOK, 3, 1);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(panelObd, 0, 0);
		((TableLayoutPanel)(object)tableLayoutPanel1).Controls.Add(buttonExport, 0, 1);
		((Control)(object)tableLayoutPanel1).Dock = DockStyle.Fill;
		((Control)(object)tableLayoutPanel1).Location = new Point(0, 0);
		((Control)(object)tableLayoutPanel1).Name = "tableLayoutPanel1";
		((TableLayoutPanel)(object)tableLayoutPanel1).RowCount = 2;
		((TableLayoutPanel)(object)tableLayoutPanel1).RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
		((TableLayoutPanel)(object)tableLayoutPanel1).RowStyles.Add(new RowStyle(SizeType.Absolute, 30f));
		((Control)(object)tableLayoutPanel1).Size = new Size(799, 571);
		((Control)(object)tableLayoutPanel1).TabIndex = 5;
		buttonOK.DialogResult = DialogResult.OK;
		buttonOK.Location = new Point(722, 544);
		buttonOK.Name = "buttonOK";
		buttonOK.Size = new Size(74, 23);
		buttonOK.TabIndex = 2;
		buttonOK.Text = "OK";
		buttonOK.UseVisualStyleBackColor = true;
		((TableLayoutPanel)(object)tableLayoutPanel1).SetColumnSpan((Control)panelObd, 4);
		panelObd.Dock = DockStyle.Fill;
		panelObd.Location = new Point(3, 3);
		panelObd.Name = "panelObd";
		panelObd.Size = new Size(793, 535);
		panelObd.TabIndex = 3;
		buttonExport.Location = new Point(3, 544);
		buttonExport.Name = "buttonExport";
		buttonExport.Size = new Size(74, 23);
		buttonExport.TabIndex = 1;
		buttonExport.Text = "Export...";
		buttonExport.UseVisualStyleBackColor = true;
		buttonExport.Click += buttonExport_Click;
		((CustomPanel)this).AutoScaleDimensions = new SizeF(6f, 13f);
		((Control)this).Controls.Add((Control)(object)tableLayoutPanel1);
		((Control)this).Margin = new Padding(5);
		((Control)this).Name = "UserPanel";
		((Control)(object)tableLayoutPanel1).ResumeLayout(performLayout: false);
		((Control)this).ResumeLayout(performLayout: false);
	}
}
