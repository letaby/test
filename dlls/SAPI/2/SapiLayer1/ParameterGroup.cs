using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace SapiLayer1;

public sealed class ParameterGroup
{
	private Dump codingStringMask;

	private string qualifier;

	private CodingStringValueCollection codingStringValues;

	private List<Parameter> parameters;

	private int? groupLength;

	public string Qualifier => qualifier;

	public bool CommunicatedViaJob
	{
		get
		{
			Service service = Parameters.FirstOrDefault()?.WriteService;
			if (service != null)
			{
				if (!service.Channel.Ecu.IsMcd)
				{
					return service.Arguments != null;
				}
				return VarcodeMcd.IsSplittedParameterGroup(service.SpecialData);
			}
			return false;
		}
	}

	public Dump CodingStringMask
	{
		get
		{
			if (codingStringMask == null && GroupLength.HasValue)
			{
				codingStringMask = Parameter.CreateCodingStringMask(GroupLength.Value, Parameters, includeExclude: true);
			}
			return codingStringMask;
		}
	}

	public bool ParametersCoverGroup
	{
		get
		{
			if (GroupLength.HasValue && GroupLength.Value > 0)
			{
				return CodingStringMask.Data.All((byte b) => b == byte.MaxValue);
			}
			return false;
		}
	}

	public IEnumerable<Parameter> Parameters => parameters.AsReadOnly();

	public int? GroupLength => groupLength;

	public bool ServiceAsParameter => parameters.First().ServiceAsParameter;

	public CodingStringValueCollection CodingStringValues => codingStringValues;

	internal ParameterGroup(string qualifier, List<Parameter> parameters)
	{
		codingStringValues = new CodingStringValueCollection();
		this.qualifier = qualifier;
		this.parameters = parameters;
		groupLength = this.parameters.FirstOrDefault()?.GroupLength;
	}

	internal static bool LoadFromLog(XElement element, string groupQualifier, LogFileFormatTagCollection format, Channel channel, List<string> missingQualifierList, object missingInfoLock)
	{
		ParameterGroup parameterGroup = channel.ParameterGroups[groupQualifier];
		if (parameterGroup != null)
		{
			bool flag = false;
			foreach (XElement item in element.Elements(format[TagName.Value]))
			{
				try
				{
					parameterGroup.codingStringValues.Add(CodingStringValue.FromXElement(item, format), setCurrent: false);
					flag = true;
				}
				catch (ArgumentOutOfRangeException)
				{
					Sapi.GetSapi().RaiseDebugInfoEvent(channel, string.Format(CultureInfo.InvariantCulture, "ArgumentOutOfRangeException while loading {0} value '{1}' from log file", groupQualifier, item.Value));
				}
				catch (FormatException)
				{
					Sapi.GetSapi().RaiseDebugInfoEvent(channel, string.Format(CultureInfo.InvariantCulture, "FormatException while loading {0} value '{1}' from log file", groupQualifier, item.Value));
				}
			}
			{
				foreach (XElement item2 in element.Elements(format[TagName.Parameter]))
				{
					flag |= Parameter.LoadFromLog(item2, groupQualifier, format, channel, missingQualifierList, missingInfoLock);
				}
				return flag;
			}
		}
		if (!channel.Ecu.IgnoreQualifier(groupQualifier))
		{
			lock (missingInfoLock)
			{
				missingQualifierList.Add(string.Format(CultureInfo.InvariantCulture, "{0}.{1}", channel.Ecu.Name, groupQualifier));
			}
		}
		return false;
	}

	internal void WriteXmlTo(DateTime startTime, DateTime endTime, XmlWriter writer, bool all)
	{
		LogFileFormatTagCollection currentFormat = LogFile.CurrentFormat;
		if (!Parameters.Any((Parameter p) => p.ParameterValues.Count > 0 && (all || p.Summary)) && !(CodingStringValues.Any() && all))
		{
			return;
		}
		writer.WriteStartElement(currentFormat[TagName.Group].LocalName);
		writer.WriteAttributeString(currentFormat[TagName.Qualifier].LocalName, Qualifier);
		if (all)
		{
			foreach (CodingStringValue codingStringValue in CodingStringValues)
			{
				codingStringValue.WriteXmlTo(startTime, writer);
			}
		}
		foreach (Parameter parameter in Parameters)
		{
			if (parameter.ParameterValues.Count > 0 && (all || parameter.Summary))
			{
				parameter.WriteXmlTo(startTime, endTime, writer);
			}
		}
		writer.WriteEndElement();
	}
}
