// Decompiled with JetBrains decompiler
// Type: SapiLayer1.LogFileFormatTagCollection
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Linq;

#nullable disable
namespace SapiLayer1;

internal class LogFileFormatTagCollection : ReadOnlyCollection<XName>
{
  private static string[] OldTags = new string[54]
  {
    "Channels",
    "Channel",
    "StartTime",
    "EndTime",
    "EcuName",
    "EcuVariant",
    "Resource",
    "System",
    "Information",
    "Name",
    "Value",
    "Qualifier",
    "EcuInfos",
    "EcuInfo",
    "Instruments",
    "Instrument",
    "FaultCodes",
    "FaultCode",
    "Code",
    "EnvironmentDatas",
    "EnvironmentData",
    "MIL",
    "Stored",
    "Active",
    "Labels",
    "Label",
    "Time",
    "LSTime",
    "SCount",
    "Type",
    "Precision",
    "Description",
    "TestNotComplete",
    "Pending",
    "RecordIndex",
    "ReadFunctions",
    "TestFailedSinceLastClear",
    "CommunicationsStates",
    "CommunicationsState",
    "Additional",
    "ContiguousSegmentStart",
    "Immediate",
    "Parameter",
    "Parameters",
    "Group",
    "Services",
    "Service",
    "Execution",
    "NegativeResponseCode",
    "Error",
    "Direction",
    "Preprocessed",
    "Fixed",
    "ChannelOptions"
  };
  private static string[] NewTags = new string[54]
  {
    "Cs",
    "C",
    "ST",
    "ET",
    "E",
    "V",
    "R",
    "S",
    "I",
    "N",
    "V",
    "Q",
    "EIs",
    "EI",
    "Is",
    "I",
    "FCs",
    "FC",
    "C",
    "EDs",
    "ED",
    "M",
    "S",
    "A",
    "Ls",
    "L",
    "T",
    "LST",
    "SC",
    "T",
    "P",
    "D",
    "TNC",
    "P",
    "RI",
    "RFN",
    "TFS",
    "CSs",
    "CS",
    "A",
    "CSS",
    "IM",
    "P",
    "Ps",
    "G",
    "Ss",
    "S",
    "E",
    "NRC",
    "E",
    "D",
    "Pp",
    "F",
    "CO"
  };

  internal LogFileFormatTagCollection(int version)
    : base((IList<XName>) new List<XName>())
  {
    if (version < 7)
    {
      foreach (string oldTag in LogFileFormatTagCollection.OldTags)
        this.Items.Add((XName) oldTag);
    }
    else
    {
      foreach (string newTag in LogFileFormatTagCollection.NewTags)
        this.Items.Add((XName) newTag);
    }
  }

  public XName this[TagName index] => this.Items[(int) index];
}
