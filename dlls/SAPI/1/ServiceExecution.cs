// Decompiled with JetBrains decompiler
// Type: SapiLayer1.ServiceExecution
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

#nullable disable
namespace SapiLayer1;

public sealed class ServiceExecution
{
  private DateTime startTime;
  private DateTime endTime;
  private int? negativeResponseCode;
  private string error;
  private List<ServiceArgumentValue> inputArgumentValues;
  private List<ServiceArgumentValue> outputArgumentValues;
  private Service service;

  private ServiceExecution(
    DateTime startTime,
    DateTime endTime,
    int? negativeResponseCode,
    string error,
    IEnumerable<ServiceArgumentValue> inputArgumentValues,
    IEnumerable<ServiceArgumentValue> outputArgumentValues,
    Service service)
  {
    this.startTime = startTime;
    this.endTime = endTime;
    this.negativeResponseCode = negativeResponseCode;
    this.error = error;
    this.inputArgumentValues = inputArgumentValues.ToList<ServiceArgumentValue>();
    this.outputArgumentValues = outputArgumentValues.ToList<ServiceArgumentValue>();
    this.service = service;
  }

  internal ServiceExecution(DateTime startTime, Service service)
  {
    this.startTime = startTime;
    this.inputArgumentValues = new List<ServiceArgumentValue>();
    this.outputArgumentValues = new List<ServiceArgumentValue>();
    this.service = service;
  }

  internal ServiceExecution(Service service)
  {
    this.inputArgumentValues = service.InputValues.Select<ServiceInputValue, ServiceArgumentValue>((Func<ServiceInputValue, ServiceArgumentValue>) (iv => iv.StoreArgumentValue())).ToList<ServiceArgumentValue>();
    this.outputArgumentValues = new List<ServiceArgumentValue>();
    this.service = service;
  }

  public int? NegativeResponseCode
  {
    get => this.negativeResponseCode;
    internal set => this.negativeResponseCode = value;
  }

  public string Error
  {
    get => this.error;
    internal set => this.error = value;
  }

  public Service Service => this.service;

  public DateTime StartTime
  {
    get => this.startTime;
    internal set => this.startTime = this.startTime == DateTime.MinValue ? value : this.startTime;
  }

  public DateTime EndTime
  {
    get => this.endTime;
    internal set => this.endTime = value;
  }

  public IList<ServiceArgumentValue> InputArgumentValues
  {
    get => (IList<ServiceArgumentValue>) this.inputArgumentValues.AsReadOnly();
  }

  public IList<ServiceArgumentValue> OutputArgumentValues
  {
    get => (IList<ServiceArgumentValue>) this.outputArgumentValues.AsReadOnly();
  }

  internal void AddOutputArgumentValue(ServiceArgumentValue serviceArgumentValue)
  {
    this.outputArgumentValues.Add(serviceArgumentValue);
  }

  internal Label CreateLabel()
  {
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0}(", (object) this.service.Name);
    stringBuilder.Append(string.Join(",", this.inputArgumentValues.Select<ServiceArgumentValue, string>((Func<ServiceArgumentValue, string>) (iav =>
    {
      if (iav.Value == null)
        return string.Empty;
      return !iav.ValuePreprocessed ? iav.Value.ToString() : "*****";
    }))));
    stringBuilder.Append(")={");
    if (!this.negativeResponseCode.HasValue && string.IsNullOrEmpty(this.error))
      stringBuilder.Append(string.Join(",", this.outputArgumentValues.Select<ServiceArgumentValue, string>((Func<ServiceArgumentValue, string>) (oav => oav.Value == null ? string.Empty : oav.Value.ToString() + (oav.OutputValue.Units.Length > 0 ? " " + oav.OutputValue.Units : string.Empty)))));
    else
      stringBuilder.Append(this.negativeResponseCode.HasValue ? Sapi.GetSapi().DiagnosisProtocols["UDS"].GetNegativeResponseCodeDescription((byte) this.negativeResponseCode.Value) : this.error);
    stringBuilder.Append("}");
    return new Label(stringBuilder.ToString(), this.endTime, this.service.Channel.Ecu, this.service.Channel);
  }

  internal static ServiceExecution ParseFromLog(
    string label,
    DateTime startTime,
    DateTime endTime,
    Service service)
  {
    int startIndex = label.IndexOf(")={", StringComparison.OrdinalIgnoreCase) + ")={".Length;
    ServiceExecution fromLog = new ServiceExecution(startTime, service);
    fromLog.endTime = endTime;
    if (service.InputValues.Count > 0)
    {
      string str1 = label.Substring(0, startIndex - ")={".Length);
      int num = str1.LastIndexOf('(');
      string str2 = str1.Substring(num + 1);
      if (service.InputValues.Count == 1)
      {
        ServiceInputValue inputValue = service.InputValues[0];
        fromLog.inputArgumentValues.Add(inputValue.ArgumentValues.Add(inputValue.ParseFromLog(str2), startTime, true, (object) inputValue));
      }
      else
      {
        string[] strArray = str2.Split(",".ToCharArray());
        for (int index = 0; index < service.InputValues.Count && index < strArray.Length; ++index)
        {
          ServiceInputValue inputValue = service.InputValues[index];
          fromLog.inputArgumentValues.Add(inputValue.ArgumentValues.Add(inputValue.ParseFromLog(strArray[index]), startTime, true, (object) inputValue));
        }
      }
    }
    string str = label.Substring(startIndex, label.Length - startIndex - 1);
    if (service.OutputValues.Count > 0)
    {
      if (service.OutputValues.Count == 1)
      {
        ServiceOutputValue outputValue = service.OutputValues[0];
        fromLog.outputArgumentValues.Add(outputValue.ArgumentValues.Add(outputValue.ParseFromLog(str), endTime, true, (object) outputValue));
      }
      else
      {
        string[] strArray = str.Split(",".ToCharArray());
        for (int index = 0; index < service.OutputValues.Count && index < strArray.Length; ++index)
        {
          ServiceOutputValue outputValue = service.OutputValues[index];
          fromLog.outputArgumentValues.Add(outputValue.ArgumentValues.Add(outputValue.ParseFromLog(strArray[index]), endTime, true, (object) outputValue));
        }
      }
    }
    else if (str.Length > 0)
      fromLog.error = str;
    return fromLog;
  }

  internal void WriteXmlTo(XmlWriter writer)
  {
    LogFileFormatTagCollection currentFormat = LogFile.CurrentFormat;
    writer.WriteStartElement(currentFormat[TagName.Execution].LocalName);
    writer.WriteAttributeString(currentFormat[TagName.StartTime].LocalName, Sapi.TimeToString(this.StartTime));
    writer.WriteAttributeString(currentFormat[TagName.EndTime].LocalName, Sapi.TimeToString(this.EndTime));
    if (this.NegativeResponseCode.HasValue)
      writer.WriteAttributeString(currentFormat[TagName.NegativeResponseCode].LocalName, this.NegativeResponseCode.Value.ToString("X2", (IFormatProvider) CultureInfo.InvariantCulture));
    if (!string.IsNullOrEmpty(this.Error))
      writer.WriteAttributeString(currentFormat[TagName.Error].LocalName, this.Error);
    foreach (ServiceArgumentValue inputArgumentValue in this.inputArgumentValues)
    {
      writer.WriteStartElement(currentFormat[TagName.Value].LocalName);
      writer.WriteAttributeString(currentFormat[TagName.Qualifier].LocalName, inputArgumentValue.InputValue.ParameterQualifier);
      writer.WriteAttributeString(currentFormat[TagName.Direction].LocalName, "I");
      if (inputArgumentValue.ValuePreprocessed)
        writer.WriteAttributeString(currentFormat[TagName.Preprocessed].LocalName, inputArgumentValue.ValuePreprocessed.ToString());
      writer.WriteValue(Presentation.FormatForLog(inputArgumentValue.Value));
      writer.WriteEndElement();
    }
    foreach (ServiceArgumentValue outputArgumentValue in this.outputArgumentValues)
    {
      writer.WriteStartElement(currentFormat[TagName.Value].LocalName);
      writer.WriteAttributeString(currentFormat[TagName.Qualifier].LocalName, outputArgumentValue.OutputValue.ParameterQualifier);
      writer.WriteAttributeString(currentFormat[TagName.Direction].LocalName, "O");
      writer.WriteValue(Presentation.FormatForLog(outputArgumentValue.Value));
      writer.WriteEndElement();
    }
    writer.WriteEndElement();
  }

  internal static ServiceExecution FromXElement(
    XElement element,
    LogFileFormatTagCollection format,
    Service service,
    List<string> missingQualifierList,
    object missingInfoLock)
  {
    DateTime dateTime1 = Sapi.TimeFromString(element.Attribute(format[TagName.StartTime]).Value);
    DateTime dateTime2 = Sapi.TimeFromString(element.Attribute(format[TagName.EndTime]).Value);
    string str1 = element.Attribute(format[TagName.NegativeResponseCode])?.Value;
    string error = element.Attribute(format[TagName.Error])?.Value;
    int? negativeResponseCode = str1 != null ? new int?(Convert.ToInt32(str1, 16 /*0x10*/)) : new int?();
    List<ServiceArgumentValue> inputArgumentValues = new List<ServiceArgumentValue>();
    List<ServiceArgumentValue> outputArgumentValues = new List<ServiceArgumentValue>();
    foreach (XElement element1 in element.Elements(format[TagName.Value]))
    {
      string parameterQualifier = element1.Attribute(format[TagName.Qualifier]).Value;
      string str2 = element1.Attribute(format[TagName.Direction]).Value;
      bool flag = false;
      switch (str2)
      {
        case "I":
          ServiceInputValue parent1 = service.InputValues.FirstOrDefault<ServiceInputValue>((Func<ServiceInputValue, bool>) (iv => iv.ParameterQualifier == parameterQualifier));
          if (parent1 != null)
          {
            bool result;
            inputArgumentValues.Add(parent1.ArgumentValues.Add(parent1.ParseFromLog(element1.Value), dateTime1, true, (object) parent1, bool.TryParse(element1.Attribute(format[TagName.Preprocessed])?.Value, out result) & result));
            flag = true;
            break;
          }
          break;
        case "O":
          ServiceOutputValue parent2 = service.OutputValues.FirstOrDefault<ServiceOutputValue>((Func<ServiceOutputValue, bool>) (iv => iv.ParameterQualifier == parameterQualifier));
          if (parent2 != null)
          {
            outputArgumentValues.Add(parent2.ArgumentValues.Add(parent2.ParseFromLog(element1.Value), dateTime2, true, (object) parent2));
            flag = true;
            break;
          }
          break;
      }
      if (!flag)
      {
        lock (missingInfoLock)
          missingQualifierList.Add(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}.{2} [{3}]", (object) service.Channel.Ecu.Name, (object) service.Qualifier, (object) parameterQualifier, (object) str2));
      }
    }
    return new ServiceExecution(dateTime1, dateTime2, negativeResponseCode, error, (IEnumerable<ServiceArgumentValue>) inputArgumentValues, (IEnumerable<ServiceArgumentValue>) outputArgumentValues, service);
  }
}
