using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using CaesarAbstraction;
using McdAbstraction;

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

	public bool Visible
	{
		get
		{
			return visible;
		}
		internal set
		{
			visible = value;
		}
	}

	internal XElement XElement
	{
		get
		{
			LogFileFormatTagCollection currentFormat = LogFile.CurrentFormat;
			XElement xElement = new XElement(currentFormat[TagName.EnvironmentData], Presentation.FormatForLog(Value), new XAttribute(currentFormat[TagName.Qualifier], Qualifier));
			if (recordIndex != 0)
			{
				xElement.Add(new XAttribute(currentFormat[TagName.RecordIndex], recordIndex.ToString(CultureInfo.InvariantCulture)));
			}
			return xElement;
		}
	}

	public FaultCodeIncident FaultCodeIncident => faultCodeIncident;

	public string Qualifier
	{
		get
		{
			if (cBFDescription == null)
			{
				if (presentationDescription == null)
				{
					return ecuInfoDescription.Qualifier;
				}
				return "ENV_" + presentationDescription.ParameterQualifier;
			}
			return cBFDescription.Qualifier;
		}
	}

	public string Name
	{
		get
		{
			if (cBFDescription == null)
			{
				if (presentationDescription == null)
				{
					return ecuInfoDescription.Name;
				}
				return presentationDescription.Name;
			}
			return cBFDescription.Name;
		}
	}

	public string Units
	{
		get
		{
			if (cBFDescription == null)
			{
				if (presentationDescription == null)
				{
					return ecuInfoDescription.Units;
				}
				return presentationDescription.Units;
			}
			return cBFDescription.Units;
		}
	}

	public object Precision
	{
		get
		{
			if (cBFDescription == null)
			{
				if (presentationDescription == null)
				{
					return null;
				}
				return presentationDescription.Precision;
			}
			return cBFDescription.Precision;
		}
	}

	public object Value
	{
		get
		{
			if (referenced != null && referenced.Length == 1 && referenced[0] != null)
			{
				return referenced[0].Value;
			}
			if (value == null && ecuInfoDescription != null && referenced != null)
			{
				object[] array = new object[referenced.Length];
				for (int i = 0; i < referenced.Length; i++)
				{
					EnvironmentData environmentData = referenced[i];
					if (environmentData != null)
					{
						array[i] = environmentData.Value;
						continue;
					}
					array[i] = string.Empty;
					if (string.Equals(ecuInfoDescription.Referenced[i], "UDSCODESPN"))
					{
						array[i] = faultCodeIncident.FaultCode.Number;
					}
					else if (string.Equals(ecuInfoDescription.Referenced[i], "UDSCODEFMI"))
					{
						array[i] = faultCodeIncident.FaultCode.Mode;
					}
				}
				try
				{
					return string.Format(CultureInfo.InvariantCulture, ecuInfoDescription.FormatString, array);
				}
				catch (FormatException innerException)
				{
					throw new InvalidOperationException("Error formatting environment data " + faultCodeIncident.FaultCode.Channel.Ecu.Name + "." + Qualifier, innerException);
				}
			}
			return value;
		}
	}

	public int RecordIndex => recordIndex;

	internal CompoundEnvironmentData CompoundDescription => ecuInfoDescription;

	internal EnvironmentData(FaultCodeIncident faultcode, Service description, object value)
	{
		faultCodeIncident = faultcode;
		cBFDescription = description;
		visible = true;
		this.value = value;
	}

	internal EnvironmentData(FaultCodeIncident faultcode, ServiceOutputValue description, int recordIndex, object value)
	{
		faultCodeIncident = faultcode;
		presentationDescription = description;
		visible = true;
		this.value = value;
		this.recordIndex = recordIndex;
	}

	internal EnvironmentData(FaultCodeIncident snapshot, IDiogenesDataItem description, int recordIndex, object value)
	{
		this.recordIndex = recordIndex;
		faultCodeIncident = snapshot;
		cBFDescription = description;
		visible = true;
		this.value = value;
	}

	internal EnvironmentData(FaultCodeIncident faultcode, CompoundEnvironmentData description, EnvironmentData[] referenced)
	{
		faultCodeIncident = faultcode;
		ecuInfoDescription = description;
		visible = true;
		this.referenced = referenced;
	}

	internal EnvironmentData(FaultCodeIncident faultcode, CompoundEnvironmentData description, string environmentDataValue)
	{
		faultCodeIncident = faultcode;
		ecuInfoDescription = description;
		visible = true;
		value = environmentDataValue;
	}

	internal void Read(CaesarDiagServiceIO diagServiceIO, ushort faultCodeIndex, ushort environmentDataIndex)
	{
		string errorEnvText = diagServiceIO.GetErrorEnvText(faultCodeIndex, environmentDataIndex);
		if (errorEnvText == null)
		{
			return;
		}
		if (cBFDescription != null)
		{
			Service service = cBFDescription as Service;
			if (service.OutputValues[0].Type == typeof(Choice))
			{
				value = service.OutputValues[0].Choices.GetItemFromOriginalName(errorEnvText);
			}
			else
			{
				value = service.OutputValues[0].ParseFromLog(errorEnvText);
			}
		}
		if (value == null)
		{
			value = errorEnvText;
		}
	}

	internal void Read(McdResponseParameter envItem)
	{
		if (envItem.Value == null)
		{
			return;
		}
		if (presentationDescription != null)
		{
			if (presentationDescription.Type == typeof(Choice))
			{
				value = presentationDescription.Choices.GetItemFromRawValue(envItem.Value.CodedValue);
			}
			else if (presentationDescription.Type == typeof(Dump))
			{
				value = new Dump((byte[])envItem.Value.Value);
			}
		}
		if (value == null)
		{
			value = envItem.Value.Value;
		}
	}

	internal static EnvironmentData FromXElement(XElement element, LogFileFormatTagCollection format, FaultCodeIncident incident)
	{
		EnvironmentData environmentData = null;
		Channel channel = incident.FaultCode.Channel;
		string qualifier = element.Attribute(format[TagName.Qualifier]).Value;
		if (incident.Functions == ReadFunctions.Snapshot)
		{
			int num = Convert.ToInt32(element.Attribute(format[TagName.RecordIndex]).Value, CultureInfo.InvariantCulture);
			if (!channel.IsRollCall)
			{
				try
				{
					CaesarDiagService val = channel.EcuHandle.OpenDiagServiceHandle(qualifier);
					try
					{
						if (val != null)
						{
							Service service = new Service(incident.FaultCode.Channel, ServiceTypes.None, qualifier);
							service.Acquire(val);
							environmentData = new EnvironmentData(incident, service, num, service.OutputValues[0].ParseFromLog(element.Value));
						}
					}
					finally
					{
						((IDisposable)val)?.Dispose();
					}
				}
				catch (CaesarErrorException)
				{
				}
			}
			else
			{
				Instrument rollCallSnapshotDescription = channel.FaultCodes.GetRollCallSnapshotDescription(qualifier);
				if (rollCallSnapshotDescription != null)
				{
					environmentData = new EnvironmentData(incident, rollCallSnapshotDescription, num, rollCallSnapshotDescription.ParseFromLog(element.Value));
				}
			}
		}
		else
		{
			if (channel.McdEcuHandle != null)
			{
				XAttribute xAttribute = element.Attribute(format[TagName.RecordIndex]);
				int num2 = ((xAttribute != null) ? Convert.ToInt32(xAttribute.Value, CultureInfo.InvariantCulture) : 0);
				ServiceOutputValue serviceOutputValue = ((num2 == 0) ? incident.FaultCode.Channel.FaultCodes.McdEnvironmentDataDescriptions : incident.FaultCode.Channel.FaultCodes.McdSnapshotDescriptions);
				if (serviceOutputValue != null)
				{
					ServiceOutputValue serviceOutputValue2 = serviceOutputValue.Service.OutputValues.Union(serviceOutputValue.StructuredOutputValues).FirstOrDefault((ServiceOutputValue ov) => qualifier == "ENV_" + ov.ParameterQualifier);
					if (serviceOutputValue2 != null)
					{
						environmentData = new EnvironmentData(incident, serviceOutputValue2, num2, serviceOutputValue2.ParseFromLog(element.Value));
					}
				}
			}
			else
			{
				Service service2 = channel.FaultCodes.EnvironmentDataDescriptions[qualifier];
				if (service2 != null)
				{
					environmentData = new EnvironmentData(incident, service2, service2.OutputValues[0].ParseFromLog(element.Value));
				}
			}
			if (environmentData == null)
			{
				CompoundEnvironmentData compoundEnvironmentData = channel.Ecu.CompoundEnvironmentDatas.Where((CompoundEnvironmentData item) => item.Qualifier == qualifier).FirstOrDefault();
				if (compoundEnvironmentData != null)
				{
					environmentData = new EnvironmentData(incident, compoundEnvironmentData, element.Value);
				}
			}
		}
		return environmentData;
	}

	internal static KeyValuePair<string, string> ExtractMetadata(XmlReader xmlReader)
	{
		LogFileFormatTagCollection currentFormat = LogFile.CurrentFormat;
		return new KeyValuePair<string, string>(xmlReader.GetAttribute(currentFormat[TagName.Qualifier].LocalName), xmlReader.ReadElementContentAsString());
	}

	public override string ToString()
	{
		if (RecordIndex != 0)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}_{1}:{2}", RecordIndex, Qualifier, value);
		}
		return string.Format(CultureInfo.InvariantCulture, "{0}:{1}", Qualifier, value);
	}

	public override int GetHashCode()
	{
		return ToString().GetHashCode();
	}

	public override bool Equals(object obj)
	{
		if (obj != null)
		{
			return string.Equals(ToString(), obj.ToString());
		}
		return false;
	}
}
