using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

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

	public int? NegativeResponseCode
	{
		get
		{
			return negativeResponseCode;
		}
		internal set
		{
			negativeResponseCode = value;
		}
	}

	public string Error
	{
		get
		{
			return error;
		}
		internal set
		{
			error = value;
		}
	}

	public Service Service => service;

	public DateTime StartTime
	{
		get
		{
			return startTime;
		}
		internal set
		{
			startTime = ((startTime == DateTime.MinValue) ? value : startTime);
		}
	}

	public DateTime EndTime
	{
		get
		{
			return endTime;
		}
		internal set
		{
			endTime = value;
		}
	}

	public IList<ServiceArgumentValue> InputArgumentValues => inputArgumentValues.AsReadOnly();

	public IList<ServiceArgumentValue> OutputArgumentValues => outputArgumentValues.AsReadOnly();

	private ServiceExecution(DateTime startTime, DateTime endTime, int? negativeResponseCode, string error, IEnumerable<ServiceArgumentValue> inputArgumentValues, IEnumerable<ServiceArgumentValue> outputArgumentValues, Service service)
	{
		this.startTime = startTime;
		this.endTime = endTime;
		this.negativeResponseCode = negativeResponseCode;
		this.error = error;
		this.inputArgumentValues = inputArgumentValues.ToList();
		this.outputArgumentValues = outputArgumentValues.ToList();
		this.service = service;
	}

	internal ServiceExecution(DateTime startTime, Service service)
	{
		this.startTime = startTime;
		inputArgumentValues = new List<ServiceArgumentValue>();
		outputArgumentValues = new List<ServiceArgumentValue>();
		this.service = service;
	}

	internal ServiceExecution(Service service)
	{
		inputArgumentValues = service.InputValues.Select((ServiceInputValue iv) => iv.StoreArgumentValue()).ToList();
		outputArgumentValues = new List<ServiceArgumentValue>();
		this.service = service;
	}

	internal void AddOutputArgumentValue(ServiceArgumentValue serviceArgumentValue)
	{
		outputArgumentValues.Add(serviceArgumentValue);
	}

	internal Label CreateLabel()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0}(", service.Name);
		stringBuilder.Append(string.Join(",", inputArgumentValues.Select(delegate(ServiceArgumentValue iav)
		{
			if (iav.Value == null)
			{
				return string.Empty;
			}
			return (!iav.ValuePreprocessed) ? iav.Value.ToString() : "*****";
		})));
		stringBuilder.Append(")={");
		if (!negativeResponseCode.HasValue && string.IsNullOrEmpty(error))
		{
			stringBuilder.Append(string.Join(",", outputArgumentValues.Select((ServiceArgumentValue oav) => (oav.Value == null) ? string.Empty : (oav.Value.ToString() + ((oav.OutputValue.Units.Length > 0) ? (" " + oav.OutputValue.Units) : string.Empty)))));
		}
		else
		{
			stringBuilder.Append(negativeResponseCode.HasValue ? Sapi.GetSapi().DiagnosisProtocols["UDS"].GetNegativeResponseCodeDescription((byte)negativeResponseCode.Value) : error);
		}
		stringBuilder.Append("}");
		return new Label(stringBuilder.ToString(), endTime, service.Channel.Ecu, service.Channel);
	}

	internal static ServiceExecution ParseFromLog(string label, DateTime startTime, DateTime endTime, Service service)
	{
		int num = label.IndexOf(")={", StringComparison.OrdinalIgnoreCase) + ")={".Length;
		ServiceExecution serviceExecution = new ServiceExecution(startTime, service);
		serviceExecution.endTime = endTime;
		if (service.InputValues.Count > 0)
		{
			string text = label.Substring(0, num - ")={".Length);
			int num2 = text.LastIndexOf('(');
			text = text.Substring(num2 + 1);
			if (service.InputValues.Count == 1)
			{
				ServiceInputValue serviceInputValue = service.InputValues[0];
				serviceExecution.inputArgumentValues.Add(serviceInputValue.ArgumentValues.Add(serviceInputValue.ParseFromLog(text), startTime, fromLog: true, serviceInputValue));
			}
			else
			{
				string[] array = text.Split(",".ToCharArray());
				for (int i = 0; i < service.InputValues.Count && i < array.Length; i++)
				{
					ServiceInputValue serviceInputValue2 = service.InputValues[i];
					serviceExecution.inputArgumentValues.Add(serviceInputValue2.ArgumentValues.Add(serviceInputValue2.ParseFromLog(array[i]), startTime, fromLog: true, serviceInputValue2));
				}
			}
		}
		string text2 = label.Substring(num, label.Length - num - 1);
		if (service.OutputValues.Count > 0)
		{
			if (service.OutputValues.Count == 1)
			{
				ServiceOutputValue serviceOutputValue = service.OutputValues[0];
				serviceExecution.outputArgumentValues.Add(serviceOutputValue.ArgumentValues.Add(serviceOutputValue.ParseFromLog(text2), endTime, fromLog: true, serviceOutputValue));
			}
			else
			{
				string[] array2 = text2.Split(",".ToCharArray());
				for (int j = 0; j < service.OutputValues.Count && j < array2.Length; j++)
				{
					ServiceOutputValue serviceOutputValue2 = service.OutputValues[j];
					serviceExecution.outputArgumentValues.Add(serviceOutputValue2.ArgumentValues.Add(serviceOutputValue2.ParseFromLog(array2[j]), endTime, fromLog: true, serviceOutputValue2));
				}
			}
		}
		else if (text2.Length > 0)
		{
			serviceExecution.error = text2;
		}
		return serviceExecution;
	}

	internal void WriteXmlTo(XmlWriter writer)
	{
		LogFileFormatTagCollection currentFormat = LogFile.CurrentFormat;
		writer.WriteStartElement(currentFormat[TagName.Execution].LocalName);
		writer.WriteAttributeString(currentFormat[TagName.StartTime].LocalName, Sapi.TimeToString(StartTime));
		writer.WriteAttributeString(currentFormat[TagName.EndTime].LocalName, Sapi.TimeToString(EndTime));
		if (NegativeResponseCode.HasValue)
		{
			writer.WriteAttributeString(currentFormat[TagName.NegativeResponseCode].LocalName, NegativeResponseCode.Value.ToString("X2", CultureInfo.InvariantCulture));
		}
		if (!string.IsNullOrEmpty(Error))
		{
			writer.WriteAttributeString(currentFormat[TagName.Error].LocalName, Error);
		}
		foreach (ServiceArgumentValue inputArgumentValue in inputArgumentValues)
		{
			writer.WriteStartElement(currentFormat[TagName.Value].LocalName);
			writer.WriteAttributeString(currentFormat[TagName.Qualifier].LocalName, inputArgumentValue.InputValue.ParameterQualifier);
			writer.WriteAttributeString(currentFormat[TagName.Direction].LocalName, "I");
			if (inputArgumentValue.ValuePreprocessed)
			{
				writer.WriteAttributeString(currentFormat[TagName.Preprocessed].LocalName, inputArgumentValue.ValuePreprocessed.ToString());
			}
			writer.WriteValue(Presentation.FormatForLog(inputArgumentValue.Value));
			writer.WriteEndElement();
		}
		foreach (ServiceArgumentValue outputArgumentValue in outputArgumentValues)
		{
			writer.WriteStartElement(currentFormat[TagName.Value].LocalName);
			writer.WriteAttributeString(currentFormat[TagName.Qualifier].LocalName, outputArgumentValue.OutputValue.ParameterQualifier);
			writer.WriteAttributeString(currentFormat[TagName.Direction].LocalName, "O");
			writer.WriteValue(Presentation.FormatForLog(outputArgumentValue.Value));
			writer.WriteEndElement();
		}
		writer.WriteEndElement();
	}

	internal static ServiceExecution FromXElement(XElement element, LogFileFormatTagCollection format, Service service, List<string> missingQualifierList, object missingInfoLock)
	{
		DateTime time = Sapi.TimeFromString(element.Attribute(format[TagName.StartTime]).Value);
		DateTime time2 = Sapi.TimeFromString(element.Attribute(format[TagName.EndTime]).Value);
		string text = element.Attribute(format[TagName.NegativeResponseCode])?.Value;
		string text2 = element.Attribute(format[TagName.Error])?.Value;
		int? num = ((text != null) ? new int?(Convert.ToInt32(text, 16)) : ((int?)null));
		List<ServiceArgumentValue> list = new List<ServiceArgumentValue>();
		List<ServiceArgumentValue> list2 = new List<ServiceArgumentValue>();
		foreach (XElement item in element.Elements(format[TagName.Value]))
		{
			string parameterQualifier = item.Attribute(format[TagName.Qualifier]).Value;
			string value = item.Attribute(format[TagName.Direction]).Value;
			bool flag = false;
			if (value == "I")
			{
				ServiceInputValue serviceInputValue = service.InputValues.FirstOrDefault((ServiceInputValue iv) => iv.ParameterQualifier == parameterQualifier);
				if (serviceInputValue != null)
				{
					list.Add(serviceInputValue.ArgumentValues.Add(serviceInputValue.ParseFromLog(item.Value), time, fromLog: true, serviceInputValue, bool.TryParse(item.Attribute(format[TagName.Preprocessed])?.Value, out var result) && result));
					flag = true;
				}
			}
			else if (value == "O")
			{
				ServiceOutputValue serviceOutputValue = service.OutputValues.FirstOrDefault((ServiceOutputValue iv) => iv.ParameterQualifier == parameterQualifier);
				if (serviceOutputValue != null)
				{
					list2.Add(serviceOutputValue.ArgumentValues.Add(serviceOutputValue.ParseFromLog(item.Value), time2, fromLog: true, serviceOutputValue));
					flag = true;
				}
			}
			if (!flag)
			{
				lock (missingInfoLock)
				{
					missingQualifierList.Add(string.Format(CultureInfo.InvariantCulture, "{0}.{1}.{2} [{3}]", service.Channel.Ecu.Name, service.Qualifier, parameterQualifier, value));
				}
			}
		}
		return new ServiceExecution(time, time2, num, text2, list, list2, service);
	}
}
