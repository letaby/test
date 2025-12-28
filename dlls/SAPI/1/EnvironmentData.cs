// Decompiled with JetBrains decompiler
// Type: SapiLayer1.EnvironmentData
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using CaesarAbstraction;
using McdAbstraction;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

#nullable disable
namespace SapiLayer1;

public sealed class EnvironmentData
{
  private IDiogenesDataItem cBFDescription;
  private ServiceOutputValue presentationDescription;
  private CompoundEnvironmentData ecuInfoDescription;
  private EnvironmentData[] referenced;
  private FaultCodeIncident faultCodeIncident;
  private int recordIndex;
  private object value;
  private bool visible;

  internal EnvironmentData(FaultCodeIncident faultcode, Service description, object value)
  {
    this.faultCodeIncident = faultcode;
    this.cBFDescription = (IDiogenesDataItem) description;
    this.visible = true;
    this.value = value;
  }

  internal EnvironmentData(
    FaultCodeIncident faultcode,
    ServiceOutputValue description,
    int recordIndex,
    object value)
  {
    this.faultCodeIncident = faultcode;
    this.presentationDescription = description;
    this.visible = true;
    this.value = value;
    this.recordIndex = recordIndex;
  }

  internal EnvironmentData(
    FaultCodeIncident snapshot,
    IDiogenesDataItem description,
    int recordIndex,
    object value)
  {
    this.recordIndex = recordIndex;
    this.faultCodeIncident = snapshot;
    this.cBFDescription = description;
    this.visible = true;
    this.value = value;
  }

  internal EnvironmentData(
    FaultCodeIncident faultcode,
    CompoundEnvironmentData description,
    EnvironmentData[] referenced)
  {
    this.faultCodeIncident = faultcode;
    this.ecuInfoDescription = description;
    this.visible = true;
    this.referenced = referenced;
  }

  internal EnvironmentData(
    FaultCodeIncident faultcode,
    CompoundEnvironmentData description,
    string environmentDataValue)
  {
    this.faultCodeIncident = faultcode;
    this.ecuInfoDescription = description;
    this.visible = true;
    this.value = (object) environmentDataValue;
  }

  internal void Read(
    CaesarDiagServiceIO diagServiceIO,
    ushort faultCodeIndex,
    ushort environmentDataIndex)
  {
    string errorEnvText = diagServiceIO.GetErrorEnvText(faultCodeIndex, environmentDataIndex);
    if (errorEnvText == null)
      return;
    if (this.cBFDescription != null)
    {
      Service cBfDescription = this.cBFDescription as Service;
      this.value = !(cBfDescription.OutputValues[0].Type == typeof (Choice)) ? cBfDescription.OutputValues[0].ParseFromLog(errorEnvText) : (object) cBfDescription.OutputValues[0].Choices.GetItemFromOriginalName(errorEnvText);
    }
    if (this.value != null)
      return;
    this.value = (object) errorEnvText;
  }

  internal void Read(McdResponseParameter envItem)
  {
    if (envItem.Value == null)
      return;
    if (this.presentationDescription != null)
    {
      if (this.presentationDescription.Type == typeof (Choice))
        this.value = (object) this.presentationDescription.Choices.GetItemFromRawValue(envItem.Value.CodedValue);
      else if (this.presentationDescription.Type == typeof (Dump))
        this.value = (object) new Dump((IEnumerable<byte>) (byte[]) envItem.Value.Value);
    }
    if (this.value != null)
      return;
    this.value = envItem.Value.Value;
  }

  public bool Visible
  {
    get => this.visible;
    internal set => this.visible = value;
  }

  internal XElement XElement
  {
    get
    {
      LogFileFormatTagCollection currentFormat = LogFile.CurrentFormat;
      XElement xelement = new XElement(currentFormat[TagName.EnvironmentData], new object[2]
      {
        (object) Presentation.FormatForLog(this.Value),
        (object) new XAttribute(currentFormat[TagName.Qualifier], (object) this.Qualifier)
      });
      if (this.recordIndex != 0)
        xelement.Add((object) new XAttribute(currentFormat[TagName.RecordIndex], (object) this.recordIndex.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
      return xelement;
    }
  }

  internal static EnvironmentData FromXElement(
    XElement element,
    LogFileFormatTagCollection format,
    FaultCodeIncident incident)
  {
    EnvironmentData environmentData = (EnvironmentData) null;
    Channel channel = incident.FaultCode.Channel;
    string qualifier = element.Attribute(format[TagName.Qualifier]).Value;
    if (incident.Functions == ReadFunctions.Snapshot)
    {
      int int32 = Convert.ToInt32(element.Attribute(format[TagName.RecordIndex]).Value, (IFormatProvider) CultureInfo.InvariantCulture);
      if (!channel.IsRollCall)
      {
        try
        {
          using (CaesarDiagService diagService = channel.EcuHandle.OpenDiagServiceHandle(qualifier))
          {
            if (diagService != null)
            {
              Service description = new Service(incident.FaultCode.Channel, ServiceTypes.None, qualifier);
              description.Acquire(diagService);
              environmentData = new EnvironmentData(incident, (IDiogenesDataItem) description, int32, description.OutputValues[0].ParseFromLog(element.Value));
            }
          }
        }
        catch (CaesarErrorException ex)
        {
        }
      }
      else
      {
        Instrument snapshotDescription = channel.FaultCodes.GetRollCallSnapshotDescription(qualifier);
        if (snapshotDescription != (Instrument) null)
          environmentData = new EnvironmentData(incident, (IDiogenesDataItem) snapshotDescription, int32, snapshotDescription.ParseFromLog(element.Value));
      }
    }
    else
    {
      if (channel.McdEcuHandle != null)
      {
        XAttribute xattribute = element.Attribute(format[TagName.RecordIndex]);
        int int32 = xattribute != null ? Convert.ToInt32(xattribute.Value, (IFormatProvider) CultureInfo.InvariantCulture) : 0;
        ServiceOutputValue serviceOutputValue = int32 == 0 ? incident.FaultCode.Channel.FaultCodes.McdEnvironmentDataDescriptions : incident.FaultCode.Channel.FaultCodes.McdSnapshotDescriptions;
        if (serviceOutputValue != null)
        {
          ServiceOutputValue description = serviceOutputValue.Service.OutputValues.Union<ServiceOutputValue>((IEnumerable<ServiceOutputValue>) serviceOutputValue.StructuredOutputValues).FirstOrDefault<ServiceOutputValue>((Func<ServiceOutputValue, bool>) (ov => qualifier == "ENV_" + ov.ParameterQualifier));
          if (description != null)
            environmentData = new EnvironmentData(incident, description, int32, description.ParseFromLog(element.Value));
        }
      }
      else
      {
        Service environmentDataDescription = channel.FaultCodes.EnvironmentDataDescriptions[qualifier];
        if (environmentDataDescription != (Service) null)
          environmentData = new EnvironmentData(incident, environmentDataDescription, environmentDataDescription.OutputValues[0].ParseFromLog(element.Value));
      }
      if (environmentData == null)
      {
        CompoundEnvironmentData description = channel.Ecu.CompoundEnvironmentDatas.Where<CompoundEnvironmentData>((Func<CompoundEnvironmentData, bool>) (item => item.Qualifier == qualifier)).FirstOrDefault<CompoundEnvironmentData>();
        if (description != null)
          environmentData = new EnvironmentData(incident, description, element.Value);
      }
    }
    return environmentData;
  }

  internal static KeyValuePair<string, string> ExtractMetadata(XmlReader xmlReader)
  {
    LogFileFormatTagCollection currentFormat = LogFile.CurrentFormat;
    return new KeyValuePair<string, string>(xmlReader.GetAttribute(currentFormat[TagName.Qualifier].LocalName), xmlReader.ReadElementContentAsString());
  }

  public FaultCodeIncident FaultCodeIncident => this.faultCodeIncident;

  public string Qualifier
  {
    get
    {
      if (this.cBFDescription != null)
        return this.cBFDescription.Qualifier;
      return this.presentationDescription == null ? this.ecuInfoDescription.Qualifier : "ENV_" + this.presentationDescription.ParameterQualifier;
    }
  }

  public string Name
  {
    get
    {
      if (this.cBFDescription != null)
        return this.cBFDescription.Name;
      return this.presentationDescription == null ? this.ecuInfoDescription.Name : this.presentationDescription.Name;
    }
  }

  public string Units
  {
    get
    {
      if (this.cBFDescription != null)
        return this.cBFDescription.Units;
      return this.presentationDescription == null ? this.ecuInfoDescription.Units : this.presentationDescription.Units;
    }
  }

  public object Precision
  {
    get
    {
      if (this.cBFDescription != null)
        return this.cBFDescription.Precision;
      return this.presentationDescription == null ? (object) null : this.presentationDescription.Precision;
    }
  }

  public object Value
  {
    get
    {
      if (this.referenced != null && this.referenced.Length == 1 && this.referenced[0] != null)
        return this.referenced[0].Value;
      if (this.value != null || this.ecuInfoDescription == null || this.referenced == null)
        return this.value;
      object[] objArray = new object[this.referenced.Length];
      for (int index = 0; index < this.referenced.Length; ++index)
      {
        EnvironmentData environmentData = this.referenced[index];
        if (environmentData != null)
        {
          objArray[index] = environmentData.Value;
        }
        else
        {
          objArray[index] = (object) string.Empty;
          if (string.Equals(this.ecuInfoDescription.Referenced[index], "UDSCODESPN"))
            objArray[index] = (object) this.faultCodeIncident.FaultCode.Number;
          else if (string.Equals(this.ecuInfoDescription.Referenced[index], "UDSCODEFMI"))
            objArray[index] = (object) this.faultCodeIncident.FaultCode.Mode;
        }
      }
      try
      {
        return (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, this.ecuInfoDescription.FormatString, objArray);
      }
      catch (FormatException ex)
      {
        throw new InvalidOperationException($"Error formatting environment data {this.faultCodeIncident.FaultCode.Channel.Ecu.Name}.{this.Qualifier}", (Exception) ex);
      }
    }
  }

  public int RecordIndex => this.recordIndex;

  internal CompoundEnvironmentData CompoundDescription => this.ecuInfoDescription;

  public override string ToString()
  {
    return this.RecordIndex != 0 ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_{1}:{2}", (object) this.RecordIndex, (object) this.Qualifier, this.value) : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1}", (object) this.Qualifier, this.value);
  }

  public override int GetHashCode() => this.ToString().GetHashCode();

  public override bool Equals(object obj)
  {
    return obj != null && string.Equals(this.ToString(), obj.ToString());
  }
}
