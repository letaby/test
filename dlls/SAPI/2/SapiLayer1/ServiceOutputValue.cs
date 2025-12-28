using System;
using System.Collections.Generic;
using System.Linq;
using CaesarAbstraction;
using McdAbstraction;

namespace SapiLayer1;

public sealed class ServiceOutputValue : Presentation
{
	private object heldValue;

	private object manipulatedValue;

	private string parameterQualifier;

	private string parameterName;

	private string singleServiceQualifier;

	private string singleServiceName;

	private ServiceOutputValueCollection structuredOutputValues;

	private Service service;

	private ServiceArgumentValueCollection argumentValues;

	public Service Service
	{
		get
		{
			return service;
		}
		internal set
		{
			service = value;
		}
	}

	public object Value => argumentValues.Current?.Value;

	public override object ManipulatedValue
	{
		get
		{
			return manipulatedValue;
		}
		set
		{
			if (value != manipulatedValue)
			{
				manipulatedValue = value;
				object value2 = manipulatedValue ?? heldValue;
				argumentValues.Add(value2, Sapi.Now, fromLog: false, this);
				Service.Channel.SetManipulatedState(service.Qualifier, value != null);
			}
		}
	}

	public ServiceArgumentValueCollection ArgumentValues => argumentValues;

	public string ParameterQualifier => parameterQualifier;

	public string ParameterName => parameterName;

	public ServiceOutputValueCollection StructuredOutputValues => structuredOutputValues;

	internal string SingleServiceQualifier => singleServiceQualifier;

	internal string SingleServiceName => singleServiceName;

	internal bool CanBeCaesarEquivalent
	{
		get
		{
			if (base.Type != null && !base.IsStructure)
			{
				return !base.IsArrayDefinition;
			}
			return false;
		}
	}

	internal ServiceOutputValue(Service service, ushort i)
		: base(i)
	{
		this.service = service;
		argumentValues = new ServiceArgumentValueCollection();
	}

	internal void Acquire(Channel channel, McdDBDiagComPrimitive diagService, McdDBResponseParameter response, IEnumerable<string> parentParameterNames, Dictionary<string, int> caesarEquivalentQualifiers)
	{
		base.Acquire(channel, response, service.ServiceTypes == ServiceTypes.DiagJob || (service.IsNoTransmission.HasValue && service.IsNoTransmission.Value));
		parameterQualifier = response.Qualifier;
		if (service.ServiceTypes == ServiceTypes.Environment)
		{
			parameterName = response.Name;
			singleServiceName = response.Name;
			singleServiceQualifier = "ENV_" + McdCaesarEquivalence.MakeQualifier(parameterName);
			return;
		}
		string serviceQualifier = service.Qualifier;
		string serviceName = service.Name;
		McdCaesarEquivalence.AdjustServiceQualifierName(diagService, response, ref serviceQualifier, ref serviceName);
		string element = ((service.Channel.Ecu.CaesarEquivalentResponseParameterQualifierSource != Ecu.ResponseParameterQualifierSource.Qualifier) ? response.Name : response.Qualifier);
		string[] responseParameterNames = parentParameterNames.Concat(Enumerable.Repeat(response.Name, 1)).ToArray();
		singleServiceQualifier = McdCaesarEquivalence.MakeQualifier(serviceQualifier + "_" + string.Join("_", parentParameterNames.Concat(Enumerable.Repeat(element, 1))), CanBeCaesarEquivalent ? caesarEquivalentQualifiers : null);
		singleServiceName = service.Channel.Ecu.Translate(Sapi.MakeTranslationIdentifier(singleServiceQualifier, "Name"), McdCaesarEquivalence.GetSingleServiceEquivalentName(serviceName, responseParameterNames));
		serviceName = Service.GetServiceName(singleServiceName, McdCaesarEquivalence.GetRetainedSuffix(serviceName) != null);
		parameterName = McdCaesarEquivalence.GetResponsePart(singleServiceName, serviceName, isName: true);
	}

	internal new void Acquire(Channel channel, CaesarDiagService diagService)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Invalid comparison between Unknown and I4
		base.Acquire(channel, diagService);
		singleServiceName = service.Name;
		singleServiceQualifier = service.Qualifier;
		if (diagService.PresParamCount == 1)
		{
			if ((int)diagService.ServiceType != 128)
			{
				string responsePart = McdCaesarEquivalence.GetResponsePart(service.Qualifier, service.ServiceQualifier, isName: false);
				int num = responsePart.IndexOf("_physical", StringComparison.Ordinal);
				parameterQualifier = ((num != -1) ? responsePart.Substring(0, num) : responsePart);
				parameterName = McdCaesarEquivalence.GetResponsePart(service.Name, service.ServiceName, isName: true);
			}
			else
			{
				parameterQualifier = service.Qualifier.Substring(4);
				parameterName = service.Name;
			}
		}
	}

	internal void GetPresentation(CaesarDiagServiceIO diagServiceIO, ServiceExecution execution)
	{
		AddArgumentValue(GetPresentation(diagServiceIO), execution);
	}

	internal void GetPresentation(McdDiagComPrimitive diagServiceIO, ServiceExecution execution)
	{
		AddArgumentValue(GetPresentation(diagServiceIO), execution);
	}

	internal void GetPresentation(List<McdResponseParameter> responseParameter, ServiceExecution execution)
	{
		if (base.Type != null)
		{
			AddArgumentValue(GetPresentation(responseParameter), execution);
		}
	}

	private void AddArgumentValue(object newvalue, ServiceExecution execution)
	{
		heldValue = newvalue;
		execution.AddOutputArgumentValue(argumentValues.Add((manipulatedValue != null) ? manipulatedValue : newvalue, execution.EndTime, fromLog: false, this));
	}

	internal new object ParseFromLog(string value)
	{
		if (base.Units.Length > 0)
		{
			int num = value.LastIndexOf(' ');
			if (num > -1)
			{
				value = value.Substring(0, num);
			}
		}
		return base.ParseFromLog(value);
	}

	internal void CreateStructuredOutputValues()
	{
		structuredOutputValues = new ServiceOutputValueCollection();
	}
}
