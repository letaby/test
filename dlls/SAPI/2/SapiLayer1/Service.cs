using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using CaesarAbstraction;
using McdAbstraction;

namespace SapiLayer1;

public sealed class Service : IComparable, IDiogenesDataItem
{
	internal enum ExecuteType
	{
		UserInvoke,
		UserInvokeFromList,
		SystemInvoke,
		EcuInfoInvoke,
		EcuInfoUserInvoke,
		CombinedServiceInvoke
	}

	internal const int defaultResetSleepTime = 10;

	internal const int defaultCacheTime = 100;

	internal const int defaultStoredDataCacheTime = 2000;

	internal const string LabelInputOutputSeparator = ")={";

	private const int MaxCaesarQualifierLength = 64;

	private string serviceQualifier;

	private Dump requestMessageMask;

	private ServiceOutputValueCollection structuredOutputValues;

	private ServiceTypes type;

	private Channel channel;

	private Dump requestMessage;

	private Dump responseMessage;

	private StringCollection arguments;

	private Dictionary<string, string> specialData;

	private string qualifier;

	private string mcdQualifier;

	private string caesarQualifier;

	private string name;

	private string description;

	private string groupName;

	private string groupQualifier;

	private bool visible;

	private ServiceInputValueCollection inputValues;

	private ServiceOutputValueCollection outputValues;

	private ushort accessLevel;

	private ushort cacheTime;

	private string affected;

	private string preService;

	private byte[] negativeResponsesToIgnore;

	private int? systemRequestLimit;

	private string systemRequestLimitResetOn;

	private int executeCount;

	private List<string> enabledAdditionalAudiences;

	private ServiceExecutionCollection executions;

	public int? MessageNumber { get; private set; }

	public string PreService => preService;

	public Channel Channel => channel;

	public string Qualifier => qualifier;

	public string DiagnosisQualifier
	{
		get
		{
			if (!Channel.Ecu.IsMcd)
			{
				return caesarQualifier;
			}
			return mcdQualifier;
		}
	}

	public string Name
	{
		get
		{
			if (channel.IsRollCall)
			{
				return channel.Ecu.Translate(Sapi.MakeTranslationIdentifier(qualifier, "SPN"), name);
			}
			return channel.Ecu.Translate(Sapi.MakeTranslationIdentifier(qualifier, "Name"), name);
		}
	}

	internal string ServiceQualifier
	{
		get
		{
			if (serviceQualifier == null)
			{
				string serviceName = GetServiceName(name, requestMessage == null || requestMessage.Data[0] == 39 || requestMessage.Data[0] == 49);
				string partialQualifier = new string(McdCaesarEquivalence.MakeQualifier(serviceName).Take(64).ToArray());
				if (McdCaesarEquivalence.TryGetIndexLengthIgnoreUnderscores(qualifier, partialQualifier, out var position, out var length))
				{
					serviceQualifier = qualifier.Substring(0, position + length);
				}
				else
				{
					string partialQualifier2 = McdCaesarEquivalence.MakeQualifier(McdCaesarEquivalence.GetResponsePart(name, serviceName, isName: true));
					if (McdCaesarEquivalence.TryGetIndexLengthIgnoreUnderscores(qualifier, partialQualifier2, out var position2, out var length2) && position2 + length2 == qualifier.Length)
					{
						serviceQualifier = qualifier.Substring(0, position2).TrimEnd('_');
					}
					else
					{
						serviceQualifier = qualifier;
						Sapi.GetSapi().RaiseDebugInfoEvent(this, "Could not determine service qualifier using either method. " + channel.Ecu.Name + "." + channel.DiagnosisVariant.Name + "." + qualifier + "; Name is " + name);
					}
				}
			}
			return serviceQualifier;
		}
	}

	internal string ServiceName => GetServiceName(Name, requestMessage == null || requestMessage.Data[0] == 39 || requestMessage.Data[0] == 49);

	public Service CombinedService { get; internal set; }

	public string ShortName => channel.Ecu.ShortName(Name);

	public string Description => channel.Ecu.Translate(Sapi.MakeTranslationIdentifier(qualifier, "Description"), description);

	public string GroupName => groupName;

	public string GroupQualifier => groupQualifier;

	public string Units
	{
		get
		{
			if (outputValues.Count > 0)
			{
				return outputValues[0].Units;
			}
			return string.Empty;
		}
	}

	public object Precision
	{
		get
		{
			if (outputValues.Count > 0)
			{
				return outputValues[0].Precision;
			}
			return null;
		}
	}

	public ChoiceCollection Choices
	{
		get
		{
			if (outputValues.Count > 0)
			{
				return outputValues[0].Choices;
			}
			return null;
		}
	}

	public Dump RequestMessage
	{
		get
		{
			if (channel.McdChannelHandle != null)
			{
				McdDiagComPrimitive service = channel.McdChannelHandle.GetService(mcdQualifier);
				for (int i = 0; i < inputValues.Count; i++)
				{
					if (channel.IsChannelErrorSet)
					{
						break;
					}
					ServiceInputValue serviceInputValue = inputValues[i];
					if (serviceInputValue.Type != null && !serviceInputValue.IsReserved)
					{
						serviceInputValue.SetPreparation(service);
					}
				}
				return new Dump(service.RequestMessage);
			}
			if (channel.ChannelHandle != null)
			{
				CaesarDiagService val = channel.EcuHandle.OpenDiagServiceHandle(caesarQualifier);
				try
				{
					for (int j = 0; j < inputValues.Count; j++)
					{
						if (channel.IsChannelErrorSet)
						{
							break;
						}
						inputValues[j].SetPreparation(val);
					}
					return new Dump(val.RequestMessage);
				}
				finally
				{
					((IDisposable)val)?.Dispose();
				}
			}
			return requestMessage;
		}
	}

	public Dump BaseRequestMessage => requestMessage;

	internal string McdQualifier => mcdQualifier;

	public Dump RequestMessageMask
	{
		get
		{
			if (requestMessageMask == null && requestMessage != null)
			{
				byte[] array = Enumerable.Repeat(byte.MaxValue, requestMessage.Data.Count).ToArray();
				if (InputValues != null)
				{
					foreach (ServiceInputValue inputValue in InputValues)
					{
						int destinationIndex = Convert.ToInt32(inputValue.BytePosition, CultureInfo.InvariantCulture);
						int num = Convert.ToInt32(inputValue.ByteLength, CultureInfo.InvariantCulture);
						Array.Copy(Enumerable.Repeat((byte)0, num).ToArray(), 0, array, destinationIndex, num);
					}
				}
				int num2 = array.Length;
				while (num2 > 0 && array[num2 - 1] == 0)
				{
					num2--;
				}
				requestMessageMask = new Dump(array.Take(num2));
			}
			return requestMessageMask;
		}
	}

	public Dump ResponseMessage => responseMessage;

	public ServiceInputValueCollection InputValues => inputValues;

	public ServiceOutputValueCollection OutputValues => outputValues;

	public ServiceOutputValueCollection StructuredOutputValues => structuredOutputValues;

	public bool Visible => visible;

	public ServiceTypes ServiceTypes => type;

	public int Access => accessLevel;

	public int CacheTimeout
	{
		get
		{
			return cacheTime;
		}
		set
		{
			cacheTime = (ushort)value;
		}
	}

	public StringCollection Arguments => arguments;

	public Dictionary<string, string> SpecialData => specialData;

	public bool IsControlPrimitive { get; private set; }

	internal bool? IsNoTransmission { get; private set; }

	[CLSCompliant(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("CacheTime is deprecated due to non-CLS compliance, please use CacheTimeout instead.")]
	public ushort CacheTime
	{
		get
		{
			return cacheTime;
		}
		set
		{
			cacheTime = value;
		}
	}

	[CLSCompliant(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("AccessLevel is deprecated due to non-CLS compliance, please use Access instead.")]
	public ushort AccessLevel => accessLevel;

	public IEnumerable<string> EnabledAdditionalAudiences => enabledAdditionalAudiences;

	public ServiceExecutionCollection Executions => executions;

	public event ServiceCompleteEventHandler ServiceCompleteEvent;

	internal Service(Channel c, ServiceTypes st, string qualifier)
	{
		name = string.Empty;
		this.qualifier = qualifier;
		description = string.Empty;
		groupName = string.Empty;
		groupQualifier = string.Empty;
		channel = c;
		cacheTime = (ushort)((st == ServiceTypes.StoredData) ? 2000u : 100u);
		type = st;
		inputValues = new ServiceInputValueCollection(this);
		outputValues = new ServiceOutputValueCollection();
		executions = new ServiceExecutionCollection();
		negativeResponsesToIgnore = c.DiagnosisVariant.GetEcuInfoIgnoreNegativeResponses(this.qualifier);
	}

	internal void AcquireFromRollCall()
	{
		OutputValues.Add(new ServiceOutputValue(this, 0));
	}

	internal void AcquireFromRollCall(IDictionary<string, string> content)
	{
		name = content.GetNamedPropertyValue("Name", string.Empty);
		groupName = (groupQualifier = ServiceTypes.ToString());
		visible = true;
		MessageNumber = content.GetNamedPropertyValue("MessageNumber", -1);
		requestMessage = new Dump(content.GetNamedPropertyValue("RequestMessage", string.Empty));
		if (channel.SourceAddress.HasValue && requestMessage.Data.Count > 6)
		{
			byte[] array = requestMessage.Data.ToArray();
			array[5] = channel.SourceAddress.Value;
			requestMessage = new Dump(array);
		}
		cacheTime = (ushort)content.GetNamedPropertyValue("CacheTime", 100);
		description = content.GetNamedPropertyValue("Description", string.Empty);
	}

	internal void Acquire(string name, McdDBDiagComPrimitive diagService, IEnumerable<McdDBResponseParameter> responseParameters, Dictionary<string, string> specialData = null)
	{
		groupQualifier = ServiceTypes.ToString();
		groupName = ServiceTypes.ToString();
		this.name = name;
		requestMessage = ((diagService != null && diagService.RequestMessage != null) ? new Dump(diagService.RequestMessage) : null);
		if (diagService != null)
		{
			if (diagService is McdDBControlPrimitive)
			{
				IsNoTransmission = diagService.IsNoTransmission;
				IsControlPrimitive = true;
				groupQualifier = "ComPrimitive";
				groupName = "Control Primitives";
			}
			else
			{
				IList<string> functionalClassQualifiers = diagService.FunctionalClassQualifiers;
				if (functionalClassQualifiers != null && functionalClassQualifiers.Count > 0)
				{
					groupQualifier = functionalClassQualifiers[0];
					groupName = diagService.GetFunctionalClassName(0);
				}
			}
			ushort num = 0;
			ushort num2 = 0;
			foreach (McdDBRequestParameter allRequestParameter in diagService.AllRequestParameters)
			{
				if (!allRequestParameter.IsConst && !allRequestParameter.IsStructure && allRequestParameter.DataType != null)
				{
					ServiceInputValue serviceInputValue = new ServiceInputValue(this, num, num2++);
					serviceInputValue.AcquirePreparation(allRequestParameter);
					inputValues.Add(serviceInputValue);
				}
				num++;
			}
		}
		if (responseParameters == null)
		{
			responseParameters = diagService.ResponseParameters;
		}
		ushort index = 0;
		Dictionary<string, int> caesarEquivalentQualifiers = new Dictionary<string, int>();
		structuredOutputValues = new ServiceOutputValueCollection();
		AcquireServiceOutputValueCollection(structuredOutputValues, responseParameters, new List<string>());
		if (diagService != null)
		{
			mcdQualifier = diagService.Qualifier;
			enabledAdditionalAudiences = diagService.EnabledAdditionalAudiences?.ToList();
		}
		visible = true;
		this.specialData = specialData;
		AcquireCommonEcuInfo();
		void AcquireServiceOutputValueCollection(ServiceOutputValueCollection outputValues, IEnumerable<McdDBResponseParameter> responseParameterSet, List<string> parentParameterNames)
		{
			bool flag = responseParameterSet.AllSiblingsAreStructures();
			foreach (McdDBResponseParameter item in responseParameterSet)
			{
				if (!item.IsConst && !item.IsMatchingRequestParameter && !item.IsReserved)
				{
					ServiceOutputValue serviceOutputValue = new ServiceOutputValue(this, index++);
					serviceOutputValue.Acquire(channel, diagService, item, parentParameterNames, caesarEquivalentQualifiers);
					if (item.Parameters.Any())
					{
						List<string> list = parentParameterNames.ToList();
						if (!item.IsStructure || flag)
						{
							list.Add(item.Name);
						}
						serviceOutputValue.CreateStructuredOutputValues();
						AcquireServiceOutputValueCollection(serviceOutputValue.StructuredOutputValues, item.Parameters, list.ToList());
					}
					outputValues.Add(serviceOutputValue);
					if (serviceOutputValue.CanBeCaesarEquivalent)
					{
						this.outputValues.Add(serviceOutputValue);
					}
				}
				else
				{
					index++;
				}
			}
		}
	}

	internal void Acquire(List<Service> sourceServices)
	{
		Service service = sourceServices.First();
		name = service.ServiceName;
		description = service.description;
		requestMessage = service.requestMessage;
		accessLevel = service.accessLevel;
		visible = service.visible;
		caesarQualifier = service.caesarQualifier;
		groupQualifier = service.groupQualifier;
		groupName = service.groupName;
		service.inputValues.ToList().ForEach(delegate(ServiceInputValue iv)
		{
			inputValues.Add(iv);
		});
		foreach (Service sourceService in sourceServices)
		{
			sourceService.CombinedService = this;
			outputValues.Add(sourceService.outputValues[0]);
		}
		AcquireCommonEcuInfo();
	}

	internal void Acquire(Service structuredService, ServiceOutputValue outputValue)
	{
		if (type == ServiceTypes.Environment)
		{
			name = outputValue.ParameterName;
		}
		else
		{
			name = ((outputValue != null) ? outputValue.SingleServiceName : McdCaesarEquivalence.GetSingleServiceEquivalentName(structuredService.name, null));
		}
		description = structuredService.description;
		requestMessage = structuredService.requestMessage;
		mcdQualifier = structuredService.mcdQualifier;
		caesarQualifier = qualifier;
		accessLevel = structuredService.accessLevel;
		visible = structuredService.visible;
		type = structuredService.ServiceTypes;
		enabledAdditionalAudiences = structuredService.enabledAdditionalAudiences;
		groupQualifier = structuredService.groupQualifier;
		groupName = structuredService.groupName;
		structuredService.inputValues.ToList().ForEach(delegate(ServiceInputValue iv)
		{
			inputValues.Add(iv);
		});
		if (outputValue != null)
		{
			CombinedService = structuredService;
			outputValue.Service = this;
			outputValues.Add(outputValue);
		}
		else if (type == ServiceTypes.DiagJob)
		{
			outputValues = structuredService.outputValues;
			structuredOutputValues = structuredService.structuredOutputValues;
		}
		AcquireCommonEcuInfo();
	}

	internal void Acquire(CaesarDiagService diagService)
	{
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Invalid comparison between Unknown and I4
		groupName = (groupQualifier = ServiceTypes.ToString());
		int num = diagService.Name.IndexOf("_physical", StringComparison.Ordinal);
		name = ((num != -1) ? diagService.Name.Substring(0, num) : diagService.Name);
		description = diagService.Description;
		caesarQualifier = diagService.Qualifier;
		IList<byte> list = diagService.RequestMessage;
		if (list != null)
		{
			requestMessage = new Dump(list);
		}
		bool flag = false;
		ushort num2 = 0;
		for (ushort num3 = 0; num3 < diagService.PrepParamCount; num3++)
		{
			if ((int)diagService.GetPrepType((uint)num3) != 23)
			{
				ServiceInputValue serviceInputValue = new ServiceInputValue(this, num3, num2++);
				serviceInputValue.AcquirePreparation(diagService);
				inputValues.Add(serviceInputValue);
				if (serviceInputValue.Type == null)
				{
					flag = true;
				}
			}
		}
		for (ushort num4 = 0; num4 < diagService.PresParamCount; num4++)
		{
			ServiceOutputValue serviceOutputValue = new ServiceOutputValue(this, num4);
			serviceOutputValue.Acquire(channel, diagService);
			outputValues.Add(serviceOutputValue);
		}
		accessLevel = diagService.AccessLevel;
		uint argValueCount = diagService.ArgValueCount;
		if (argValueCount != 0)
		{
			arguments = new StringCollection();
			for (uint num5 = 0u; num5 < argValueCount; num5++)
			{
				arguments.Add(diagService.GetArgValue(num5));
			}
		}
		AcquireCommonEcuInfo();
		if (flag)
		{
			visible = false;
		}
	}

	private void AcquireCommonEcuInfo()
	{
		Sapi sapi = Sapi.GetSapi();
		switch (type)
		{
		case ServiceTypes.Data:
		case ServiceTypes.Environment:
		case ServiceTypes.Static:
		case ServiceTypes.Global:
		case ServiceTypes.StoredData:
		case ServiceTypes.ReadVarCode:
			if (inputValues.Count > 0)
			{
				int? ecuInfoWriteAccessLevel2 = Channel.DiagnosisVariant.GetEcuInfoWriteAccessLevel(Qualifier);
				if (ecuInfoWriteAccessLevel2.HasValue)
				{
					accessLevel = (ushort)ecuInfoWriteAccessLevel2.Value;
				}
				visible = sapi.WriteAccess >= accessLevel;
			}
			else
			{
				int? ecuInfoReadAccessLevel = Channel.DiagnosisVariant.GetEcuInfoReadAccessLevel(Qualifier);
				if (ecuInfoReadAccessLevel.HasValue)
				{
					accessLevel = (ushort)ecuInfoReadAccessLevel.Value;
				}
				visible = sapi.ReadAccess >= accessLevel;
			}
			break;
		case ServiceTypes.Actuator:
		case ServiceTypes.Adjustment:
		case ServiceTypes.Download:
		case ServiceTypes.Function:
		case ServiceTypes.DiagJob:
		case ServiceTypes.Security:
		case ServiceTypes.Session:
		case ServiceTypes.Routine:
		case ServiceTypes.IOControl:
		case ServiceTypes.WriteVarCode:
		{
			int? ecuInfoWriteAccessLevel = Channel.DiagnosisVariant.GetEcuInfoWriteAccessLevel(Qualifier);
			if (ecuInfoWriteAccessLevel.HasValue)
			{
				accessLevel = (ushort)ecuInfoWriteAccessLevel.Value;
			}
			visible = sapi.WriteAccess >= accessLevel;
			break;
		}
		}
		if (channel.Ecu.PreService.ContainsKey(qualifier))
		{
			SetPreService(channel.Ecu.PreService[qualifier]);
		}
		if (channel.Ecu.AffectServices.ContainsKey(qualifier))
		{
			SetAffected(channel.Ecu.AffectServices[qualifier]);
		}
		systemRequestLimit = channel.DiagnosisVariant.GetEcuInfoAttribute<int?>("SystemRequestLimit", qualifier);
		systemRequestLimitResetOn = Channel.DiagnosisVariant.GetEcuInfoAttribute<string>("SystemRequestLimitResetOn", qualifier);
	}

	internal CaesarException InternalExecute(ExecuteType invokeType)
	{
		return InternalExecute(invokeType, new ServiceExecution(this));
	}

	internal CaesarException InternalExecute(ExecuteType invokeType, ServiceExecution currentExecution)
	{
		CaesarException ce = null;
		if (invokeType == ExecuteType.SystemInvoke && systemRequestLimit.HasValue && executeCount >= systemRequestLimit.Value)
		{
			return null;
		}
		if ((channel.ChannelOptions & ChannelOptions.ExecutePreService) != ChannelOptions.None && preService != null)
		{
			channel.Services.InternalExecute(preService, userInvoke: false);
		}
		if (outputValues.Any() && outputValues.All((ServiceOutputValue ov) => ov.Service.CombinedService == this))
		{
			foreach (Service item in outputValues.Select((ServiceOutputValue ov) => ov.Service).ToList())
			{
				ce = item.InternalExecute(ExecuteType.CombinedServiceInvoke, currentExecution);
				responseMessage = item.ResponseMessage;
			}
		}
		else
		{
			currentExecution.StartTime = Sapi.Now;
			if (!channel.IsRollCall)
			{
				if (channel.McdChannelHandle != null)
				{
					responseMessage = null;
					McdDiagComPrimitive mcdDiagComPrimitive = null;
					try
					{
						mcdDiagComPrimitive = channel.McdChannelHandle.GetService(mcdQualifier);
						for (int num = 0; num < inputValues.Count; num++)
						{
							if (channel.IsChannelErrorSet)
							{
								break;
							}
							ServiceInputValue serviceInputValue = inputValues[num];
							if (serviceInputValue.Type != null && !serviceInputValue.IsReserved)
							{
								serviceInputValue.SetPreparation(mcdDiagComPrimitive, currentExecution);
							}
						}
						ushort num2 = cacheTime;
						if (Channel.CommunicationsState == CommunicationsState.ReadEcuInfo && invokeType == ExecuteType.EcuInfoInvoke)
						{
							num2 = ushort.MaxValue;
						}
						mcdDiagComPrimitive.Execute(num2);
						currentExecution.EndTime = Sapi.Now;
						executeCount++;
						if (!mcdDiagComPrimitive.IsNegativeResponse)
						{
							foreach (KeyValuePair<ServiceOutputValue, List<McdResponseParameter>> item2 in (from pr in mcdDiagComPrimitive.AllPositiveResponseParameters
								group pr by pr.QualifierPath into g
								select Tuple.Create(OutputValues.FirstOrDefault((ServiceOutputValue ov) => ov.McdParameterQualifierPath == g.Key), g.ToList()) into t
								where t.Item1 != null
								select t).ToDictionary((Tuple<ServiceOutputValue, List<McdResponseParameter>> k) => k.Item1, (Tuple<ServiceOutputValue, List<McdResponseParameter>> v) => v.Item2))
							{
								item2.Key.GetPresentation(item2.Value, currentExecution);
							}
						}
						else
						{
							foreach (ServiceOutputValue outputValue in outputValues)
							{
								outputValue.GetPresentation(mcdDiagComPrimitive, currentExecution);
							}
						}
						IEnumerable<byte> enumerable = mcdDiagComPrimitive.ResponseMessage;
						if (enumerable != null)
						{
							responseMessage = new Dump(enumerable);
						}
						if (mcdDiagComPrimitive.IsNegativeResponse)
						{
							ce = new CaesarException(mcdDiagComPrimitive);
							currentExecution.NegativeResponseCode = ce.NegativeResponseCode;
						}
					}
					catch (McdException mcdError)
					{
						ce = new CaesarException(mcdError);
						currentExecution.Error = ce.Message;
					}
				}
				else
				{
					CaesarDiagServiceIO val = channel.ChannelHandle.CreateDiagServiceIO(caesarQualifier);
					try
					{
						responseMessage = null;
						if (!channel.IsChannelErrorSet)
						{
							for (int num3 = 0; num3 < inputValues.Count; num3++)
							{
								if (channel.IsChannelErrorSet)
								{
									break;
								}
								inputValues[num3].SetPreparation(val, currentExecution);
							}
							if (!channel.IsChannelErrorSet)
							{
								ushort num4 = cacheTime;
								if (Channel.CommunicationsState == CommunicationsState.ReadEcuInfo && invokeType == ExecuteType.EcuInfoInvoke)
								{
									num4 = ushort.MaxValue;
								}
								try
								{
									val.Do(num4);
									currentExecution.EndTime = Sapi.Now;
									executeCount++;
									for (int num5 = 0; num5 < outputValues.Count; num5++)
									{
										if (channel.IsChannelErrorSet)
										{
											break;
										}
										outputValues[num5].GetPresentation(val, currentExecution);
									}
								}
								catch (AccessViolationException)
								{
									ce = new CaesarException(SapiError.AccessViolationDuringServiceExecution);
								}
							}
						}
						if (!channel.IsChannelErrorSet)
						{
							IList<byte> list = val.ResponseMessage;
							if (list != null)
							{
								responseMessage = new Dump(list);
							}
						}
						if (channel.IsChannelErrorSet)
						{
							ce = new CaesarException(channel.ChannelHandle);
							currentExecution.Error = ce.Message;
						}
						else if (val.IsNegativeResponse)
						{
							ce = new CaesarException(val);
							currentExecution.NegativeResponseCode = ce.NegativeResponseCode;
						}
					}
					finally
					{
						((IDisposable)val)?.Dispose();
					}
				}
			}
			else
			{
				try
				{
					channel.Ecu.RollCallManager.DoByteMessage(channel, RequestMessage.Data.ToArray(), null);
					Thread.Sleep(cacheTime);
				}
				catch (CaesarException ex2)
				{
					ce = ex2;
				}
			}
			if (OutputValues.Any((ServiceOutputValue ov) => ov.ManipulatedValue != null) || channel.Services.ManipulateToPositiveResponse)
			{
				ce = null;
			}
			if (ce != null && negativeResponsesToIgnore != null && negativeResponsesToIgnore.Any((byte nrc) => ce.NegativeResponseCode == nrc))
			{
				Sapi.GetSapi().RaiseDebugInfoEvent(this, string.Format(CultureInfo.InvariantCulture, "{0}: Ignoring negative response ${1:X}", qualifier, ce.NegativeResponseCode));
				ce = null;
			}
		}
		if ((channel.ChannelOptions & ChannelOptions.ProcessAffected) != ChannelOptions.None && ce == null)
		{
			ProcessAffected();
		}
		if (invokeType == ExecuteType.UserInvoke || invokeType == ExecuteType.UserInvokeFromList)
		{
			RaiseServiceCompleteEvent(ce, invokeType, currentExecution);
		}
		if (channel.Ecu.IsUds && ce == null && requestMessage != null && requestMessage.Data[0] == 17 && responseMessage != null)
		{
			byte b = byte.MaxValue;
			if (responseMessage.Data.Count > 2)
			{
				b = responseMessage.Data[2];
			}
			else if (responseMessage.Data.Count == 2)
			{
				b = 0;
			}
			if (channel.Services.ResetTime != null)
			{
				b = Convert.ToByte(channel.Services.ResetTime, CultureInfo.InvariantCulture);
			}
			if (b == byte.MaxValue)
			{
				b = 10;
				Sapi.GetSapi().RaiseDebugInfoEvent(this, string.Format(CultureInfo.InvariantCulture, "Reset command {0} for {1}: no sleep time available, using default {2}s", name, channel.Ecu.Name, 10));
			}
			Thread.Sleep(b * 1000);
			ce = channel.Reset();
		}
		return ce;
	}

	internal void CheckInputs()
	{
		for (int i = 0; i < inputValues.Count; i++)
		{
			ServiceInputValue serviceInputValue = inputValues[i];
			if (serviceInputValue.Value == null)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Input argument '{0}' was not set", serviceInputValue.Name));
			}
		}
	}

	private void RaiseServiceCompleteEvent(CaesarException e, ExecuteType invokeType, ServiceExecution currentExecution)
	{
		if (invokeType == ExecuteType.UserInvoke || invokeType == ExecuteType.UserInvokeFromList)
		{
			FireAndForget.Invoke(this.ServiceCompleteEvent, this, new ResultEventArgs(e));
			if (invokeType == ExecuteType.UserInvoke)
			{
				channel.Services.RaiseServiceCompleteEvent(this, e);
			}
			LabelLog(currentExecution);
			executions.Add(currentExecution, fromLog: false);
			if (invokeType == ExecuteType.UserInvoke)
			{
				channel.SyncDone(e);
			}
		}
	}

	internal void AddStringsForTranslation(Dictionary<string, string> table)
	{
		table[Sapi.MakeTranslationIdentifier(qualifier, "Name")] = name;
		if (!string.IsNullOrEmpty(description))
		{
			table[Sapi.MakeTranslationIdentifier(qualifier, "Description")] = description;
		}
		if (InputValues != null)
		{
			foreach (ServiceInputValue inputValue in InputValues)
			{
				inputValue.AddStringsForTranslation(table);
			}
		}
		if (OutputValues == null)
		{
			return;
		}
		foreach (ServiceOutputValue outputValue in OutputValues)
		{
			outputValue.AddStringsForTranslation(table);
		}
	}

	internal void SetPreService(string item)
	{
		preService = channel.Services.GetDereferencedServiceList(item) ?? item;
	}

	internal void SetAffected(string item)
	{
		affected = item;
	}

	private static void LabelLog(ServiceExecution currentExecution)
	{
		Sapi sapi = Sapi.GetSapi();
		if (sapi.LogFiles.Logging)
		{
			sapi.LogFiles.LabelLog(currentExecution.CreateLabel());
		}
	}

	internal static bool IsServiceLabel(Label label, out string serviceName)
	{
		serviceName = null;
		if (label.Ecu != null && label.Name.Contains(")={"))
		{
			serviceName = label.Name.Substring(0, label.Name.IndexOf('('));
		}
		return serviceName != null;
	}

	internal void ParseFromLog(string label, DateTime startTime, DateTime endTime)
	{
		executions.Add(ServiceExecution.ParseFromLog(label, startTime, endTime, this), fromLog: true);
	}

	internal void SynchronousCheckFailure(object sender, CaesarException exception)
	{
		FireAndForget.Invoke(this.ServiceCompleteEvent, this, new ResultEventArgs(exception));
		channel.Services.RaiseServiceCompleteEvent(this, exception);
	}

	public void Execute(bool synchronous)
	{
		CheckInputs();
		channel.QueueAction(new ServiceExecution(this), synchronous, SynchronousCheckFailure);
	}

	internal string GetServiceName(string name, bool checkPrefixes)
	{
		string[] array = name.Split(new string[1] { Channel.Ecu.NameSplit }, StringSplitOptions.None);
		if (channel.Ecu.DiagnosisSource == DiagnosisSource.CaesarDatabase && (ServiceTypes == ServiceTypes.ReadVarCode || ServiceTypes == ServiceTypes.WriteVarCode) && array.Length > 2)
		{
			return string.Join(Channel.Ecu.NameSplit, array.Take(array.Length - 1));
		}
		if (array.Length > 1 && checkPrefixes)
		{
			string[] array2 = new string[5] { "Start", "Stop", "Request Results", "Send", "Request" };
			foreach (string text in array2)
			{
				if (array[1].Equals(text, StringComparison.OrdinalIgnoreCase) || array[1].StartsWith(text + " ", StringComparison.OrdinalIgnoreCase))
				{
					return array[0] + Channel.Ecu.NameSplit + text;
				}
			}
		}
		return array[0];
	}

	public int CompareTo(object obj)
	{
		Service service = obj as Service;
		if (service.ServiceTypes != ServiceTypes)
		{
			return ServiceTypes.CompareTo(service.ServiceTypes);
		}
		return string.Compare(Name, service.Name, StringComparison.Ordinal);
	}

	public override bool Equals(object obj)
	{
		return base.Equals(obj);
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	public static bool operator ==(Service object1, Service object2)
	{
		return object.Equals(object1, object2);
	}

	public static bool operator !=(Service object1, Service object2)
	{
		return !object.Equals(object1, object2);
	}

	public static bool operator <(Service r1, Service r2)
	{
		if (r1 == null)
		{
			throw new ArgumentNullException("r1");
		}
		return r1.CompareTo(r2) < 0;
	}

	public static bool operator >(Service r1, Service r2)
	{
		if (r1 == null)
		{
			throw new ArgumentNullException("r1");
		}
		return r1.CompareTo(r2) > 0;
	}

	private void ProcessAffected()
	{
		if (affected != null)
		{
			if (string.Equals(affected, "Parameters", StringComparison.OrdinalIgnoreCase))
			{
				channel.Parameters.ResetEcuReadFlags();
			}
			else if (string.Equals(affected, "EcuInfo", StringComparison.OrdinalIgnoreCase))
			{
				channel.EcuInfos.InternalRead(explicitread: false);
			}
			else
			{
				string[] array = affected.Split(";".ToCharArray());
				for (int i = 0; i < array.Length; i++)
				{
					EcuInfo ecuInfo = channel.EcuInfos[array[i]];
					if (ecuInfo != null)
					{
						ecuInfo.InternalRead(explicitread: false);
					}
					else
					{
						Sapi.GetSapi().RaiseDebugInfoEvent(this, string.Format(CultureInfo.InvariantCulture, "Could not find affected item {0} for service {1}", array[i], Qualifier));
					}
				}
			}
		}
		foreach (Service item in channel.Services.Where((Service s) => s.systemRequestLimitResetOn != null && s.systemRequestLimitResetOn.Split(",;".ToCharArray()).Contains(qualifier)))
		{
			item.executeCount = 0;
		}
	}

	internal static void LoadFromLog(XElement element, LogFileFormatTagCollection format, Channel channel, List<string> missingQualifierList, object missingInfoLock)
	{
		string value = element.Attribute(format[TagName.Qualifier]).Value;
		Service service = new Service[2]
		{
			channel.Services[value],
			channel.StructuredServices?[value]
		}.Where((Service s) => s != null).Distinct().FirstOrDefault();
		if (service != null)
		{
			DateTime dateTime = DateTime.MinValue;
			if (service.executions.Count > 0)
			{
				dateTime = service.executions.Last().EndTime;
			}
			{
				foreach (XElement item in element.Elements(format[TagName.Execution]))
				{
					ServiceExecution serviceExecution = ServiceExecution.FromXElement(item, format, service, missingQualifierList, missingInfoLock);
					if (serviceExecution.StartTime > dateTime)
					{
						service.executions.Add(serviceExecution, fromLog: true);
					}
				}
				return;
			}
		}
		if (!channel.Ecu.IgnoreQualifier(value))
		{
			lock (missingInfoLock)
			{
				missingQualifierList.Add(string.Format(CultureInfo.InvariantCulture, "{0}.{1}", channel.Ecu.Name, value));
			}
		}
	}

	internal void WriteXmlTo(DateTime startTime, DateTime endTime, XmlWriter writer)
	{
		LogFileFormatTagCollection currentFormat = LogFile.CurrentFormat;
		writer.WriteStartElement(currentFormat[TagName.Service].LocalName);
		writer.WriteAttributeString(currentFormat[TagName.Qualifier].LocalName, Qualifier);
		foreach (ServiceExecution execution in executions)
		{
			if (execution.StartTime >= startTime && execution.EndTime <= endTime)
			{
				execution.WriteXmlTo(writer);
			}
		}
		writer.WriteEndElement();
	}
}
