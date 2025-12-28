using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Linq;

namespace SapiLayer1;

internal class LogFileFormatTagCollection : ReadOnlyCollection<XName>
{
	private static string[] OldTags = new string[54]
	{
		"Channels", "Channel", "StartTime", "EndTime", "EcuName", "EcuVariant", "Resource", "System", "Information", "Name",
		"Value", "Qualifier", "EcuInfos", "EcuInfo", "Instruments", "Instrument", "FaultCodes", "FaultCode", "Code", "EnvironmentDatas",
		"EnvironmentData", "MIL", "Stored", "Active", "Labels", "Label", "Time", "LSTime", "SCount", "Type",
		"Precision", "Description", "TestNotComplete", "Pending", "RecordIndex", "ReadFunctions", "TestFailedSinceLastClear", "CommunicationsStates", "CommunicationsState", "Additional",
		"ContiguousSegmentStart", "Immediate", "Parameter", "Parameters", "Group", "Services", "Service", "Execution", "NegativeResponseCode", "Error",
		"Direction", "Preprocessed", "Fixed", "ChannelOptions"
	};

	private static string[] NewTags = new string[54]
	{
		"Cs", "C", "ST", "ET", "E", "V", "R", "S", "I", "N",
		"V", "Q", "EIs", "EI", "Is", "I", "FCs", "FC", "C", "EDs",
		"ED", "M", "S", "A", "Ls", "L", "T", "LST", "SC", "T",
		"P", "D", "TNC", "P", "RI", "RFN", "TFS", "CSs", "CS", "A",
		"CSS", "IM", "P", "Ps", "G", "Ss", "S", "E", "NRC", "E",
		"D", "Pp", "F", "CO"
	};

	public XName this[TagName index] => base.Items[(int)index];

	internal LogFileFormatTagCollection(int version)
		: base((IList<XName>)new List<XName>())
	{
		if (version < 7)
		{
			string[] oldTags = OldTags;
			foreach (string text in oldTags)
			{
				base.Items.Add(text);
			}
		}
		else
		{
			string[] oldTags = NewTags;
			foreach (string text2 in oldTags)
			{
				base.Items.Add(text2);
			}
		}
	}
}
