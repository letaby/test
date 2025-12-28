using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using CaesarAbstraction;
using McdAbstraction;

namespace SapiLayer1;

public sealed class ServiceCollection : LateLoadReadOnlyCollection<Service>
{
	private Dictionary<string, string> serviceListVariables = new Dictionary<string, string>();

	private bool manipulateToPositiveResponse;

	private Dictionary<string, Service> cache;

	private Channel channel;

	private object resetTime;

	private const int PauseTime = 1000;

	private static string PauseQualifier = "Pause";

	private ServiceTypes serviceTypes;

	private FaultCode faultCode;

	private bool reconstructCombined;

	private bool isStructuredSet;

	internal object ResetTime => resetTime;

	public bool ManipulateToPositiveResponse
	{
		get
		{
			return manipulateToPositiveResponse;
		}
		set
		{
			if (value != manipulateToPositiveResponse)
			{
				manipulateToPositiveResponse = value;
				channel.SetManipulatedState(GetType().Name, value);
			}
		}
	}

	public Service this[string qualifier]
	{
		get
		{
			if (qualifier != null)
			{
				qualifier = qualifier.RemoveArguments();
				if (cache == null)
				{
					cache = ToDictionary((Service e) => e.Qualifier, StringComparer.OrdinalIgnoreCase);
				}
				Service value = null;
				if (!cache.TryGetValue(qualifier, out value) && channel.Ecu.AlternateQualifiers.TryGetValue(qualifier, out var value2))
				{
					cache.TryGetValue(value2, out value);
				}
				return value;
			}
			return null;
		}
	}

	public event ServiceCompleteEventHandler ServiceCompleteEvent;

	internal ServiceCollection(Channel c, ServiceTypes serviceTypes, bool reconstructCombined = false)
	{
		this.serviceTypes = serviceTypes;
		channel = c;
		this.reconstructCombined = reconstructCombined;
		if (channel.Ecu.Properties.ContainsKey("ResetTime"))
		{
			if (!byte.TryParse(channel.Ecu.Properties["ResetTime"], out var result))
			{
				Sapi.GetSapi().RaiseDebugInfoEvent(this, string.Format(CultureInfo.InvariantCulture, "Error parsing reset time property for {0}", channel.Ecu.Name));
			}
			else
			{
				resetTime = result;
			}
		}
	}

	internal ServiceCollection(FaultCode faultCode)
	{
		serviceTypes = ServiceTypes.Environment;
		this.faultCode = faultCode;
		channel = faultCode.Channel;
	}

	internal ServiceCollection(Channel channel)
	{
		this.channel = channel;
		isStructuredSet = true;
	}

	protected override void AcquireList()
	{
		if (isStructuredSet)
		{
			AcquireListStructured();
		}
		else
		{
			AcquireListCaesarEquivalent();
		}
	}

	private void AcquireListStructured()
	{
		if (channel.McdEcuHandle != null)
		{
			foreach (McdCaesarEquivalence item in from eq in McdCaesarEquivalence.FromDBLocation(channel.McdEcuHandle)
				where eq.ServiceTypes != ServiceTypes.None || eq.Service is McdDBControlPrimitive
				select eq)
			{
				Service service = new Service(channel, item.ServiceTypes, item.Qualifier);
				service.Acquire(item.Name, item.Service, null);
				base.Items.Add(service);
			}
			return;
		}
		if (channel.EcuHandle == null)
		{
			return;
		}
		ServiceTypes serviceTypes = ServiceTypes.Actuator | ServiceTypes.Adjustment | ServiceTypes.Data | ServiceTypes.Download | ServiceTypes.DiagJob | ServiceTypes.Function | ServiceTypes.Global | ServiceTypes.IOControl | ServiceTypes.ReadVarCode | ServiceTypes.Routine | ServiceTypes.Security | ServiceTypes.Session | ServiceTypes.Static | ServiceTypes.StoredData | ServiceTypes.WriteVarCode;
		IEnumerable<Service> singleServices = from q in channel.EcuHandle.GetServices((ServiceTypes)serviceTypes).OfType<string>()
			where channel.Ecu.IsVirtual || !q.EndsWith("_functional", StringComparison.Ordinal)
			select AcquireService(q) into service2
			where service2 != null
			select service2;
		foreach (Service item2 in CreateCombinedSet(singleServices))
		{
			base.Items.Add(item2);
		}
	}

	private void AcquireListCaesarEquivalent()
	{
		if (channel.IsRollCall)
		{
			foreach (Service item in channel.Ecu.RollCallManager.CreateServices(channel, serviceTypes))
			{
				base.Items.Add(item);
			}
		}
		else if (serviceTypes != ServiceTypes.None)
		{
			if (serviceTypes != ServiceTypes.Environment)
			{
				foreach (IGrouping<ServiceTypes, Tuple<Service, ServiceOutputValue>> item2 in from s in channel.CaesarEquivalentServices
					orderby s.Item1.Qualifier
					group s by s.Item1.ServiceTypes)
				{
					ServiceTypes key = item2.Key;
					if ((key & serviceTypes) == 0 && (serviceTypes != ServiceTypes.StoredData || key != ServiceTypes.Data))
					{
						continue;
					}
					foreach (Tuple<Service, ServiceOutputValue> item3 in item2)
					{
						string qualifier = ((item3.Item2 != null) ? item3.Item2.SingleServiceQualifier : item3.Item1.Qualifier);
						if (serviceTypes != ServiceTypes.Environment && (channel.Ecu.IgnoreQualifier(qualifier) || channel.Ecu.MakeInstrumentQualifier(qualifier)))
						{
							continue;
						}
						bool flag = (key & serviceTypes) != 0;
						if (serviceTypes == ServiceTypes.StoredData && key == ServiceTypes.Data && channel.Ecu.MakeStoredQualifier(qualifier) && !channel.Ecu.IgnoreQualifier(qualifier))
						{
							flag = true;
						}
						if (flag)
						{
							if (item3.Item2 != null)
							{
								Service service = new Service(channel, item3.Item1.ServiceTypes, qualifier);
								service.Acquire(item3.Item1, item3.Item2);
								base.Items.Add(service);
							}
							else
							{
								base.Items.Add(item3.Item1);
							}
						}
					}
				}
			}
			else if (channel.McdEcuHandle != null)
			{
				foreach (McdDBEnvDataDesc dBEnvironmentDataDescription in channel.McdEcuHandle.DBEnvironmentDataDescriptions)
				{
					IEnumerable<McdDBResponseParameter> enumerable = ((faultCode != null) ? dBEnvironmentDataDescription.GetFaultSpecificEnvironmentalDataSet(faultCode.LongNumber) : dBEnvironmentDataDescription.CommonEnvironmentalDataSet);
					if (enumerable.Any())
					{
						Service service2 = new Service(channel, ServiceTypes.Environment, dBEnvironmentDataDescription.Qualifier);
						service2.Acquire(dBEnvironmentDataDescription.Name, null, enumerable);
						base.Items.Add(service2);
					}
				}
			}
			else if (channel.EcuHandle != null)
			{
				foreach (string item4 in channel.EcuHandle.GetServices((ServiceTypes)128).OfType<string>())
				{
					Service service3 = AcquireService(item4);
					if (service3 != null)
					{
						base.Items.Add(service3);
					}
				}
			}
		}
		if (!reconstructCombined)
		{
			return;
		}
		foreach (IGrouping<Service, Service> servicesByPrimitive in from s in base.Items.Where((Service s) => s.ServiceTypes != ServiceTypes.DiagJob && s.OutputValues.Count == 1).ToList()
			group s by s.CombinedService)
		{
			if (!base.Items.Any((Service s) => s.Qualifier == servicesByPrimitive.Key.Qualifier))
			{
				base.Items.Insert(base.Items.IndexOf(servicesByPrimitive.First()), servicesByPrimitive.Key);
			}
		}
	}

	private IEnumerable<Service> CreateCombinedSet(IEnumerable<Service> singleServices)
	{
		Dictionary<Tuple<string, string>, List<Service>> dictionary = new Dictionary<Tuple<string, string>, List<Service>>();
		foreach (Service singleService in singleServices)
		{
			Tuple<string, string> key = Tuple.Create(singleService.ServiceQualifier, singleService.BaseRequestMessage?.ToString());
			if (dictionary.TryGetValue(key, out var value))
			{
				value.Add(singleService);
			}
			else
			{
				dictionary.Add(key, Enumerable.Repeat(singleService, 1).ToList());
			}
		}
		foreach (KeyValuePair<Tuple<string, string>, List<Service>> servicesByPrimitive in dictionary)
		{
			if (servicesByPrimitive.Key.Item2 == null || servicesByPrimitive.Value.First().OutputValues.Count == 0)
			{
				yield return servicesByPrimitive.Value.First();
			}
			else
			{
				yield return AcquireService(servicesByPrimitive.Key.Item1, servicesByPrimitive.Value);
			}
		}
	}

	private Service AcquireService(string qualifier)
	{
		//IL_0040: Expected O, but got Unknown
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected I4, but got Unknown
		try
		{
			CaesarDiagService val = channel.EcuHandle.OpenDiagServiceHandle(qualifier);
			try
			{
				if (val != null)
				{
					Service service = new Service(channel, (ServiceTypes)val.ServiceType, qualifier);
					service.Acquire(val);
					return service;
				}
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
		catch (CaesarErrorException ex)
		{
			CaesarErrorException caesarError = ex;
			Sapi.GetSapi().RaiseExceptionEvent(qualifier, new CaesarException(caesarError));
		}
		return null;
	}

	private Service AcquireService(string serviceQualifier, List<Service> sourceServices)
	{
		Service service = new Service(channel, sourceServices.First().ServiceTypes, serviceQualifier);
		service.Acquire(sourceServices);
		return service;
	}

	internal void RaiseServiceCompleteEvent(Service s, Exception e)
	{
		FireAndForget.Invoke(this.ServiceCompleteEvent, s, new ResultEventArgs(e));
	}

	internal void RaiseServiceCompleteEvent(string serviceList, Exception e)
	{
		FireAndForget.Invoke(this.ServiceCompleteEvent, serviceList, new ResultEventArgs(e));
		channel.SyncDone(e);
	}

	public string GetDereferencedServiceList(string shortPropertyName)
	{
		string text = string.Format(CultureInfo.InvariantCulture, "{0}_{1}", shortPropertyName, channel.DiagnosisVariant.Name);
		int length = shortPropertyName.Length;
		while (text.Length >= length)
		{
			if (channel.Ecu.Properties.ContainsKey(text))
			{
				return channel.Ecu.Properties[text];
			}
			text = text.Remove(text.Length - 1);
		}
		return null;
	}

	internal void InternalDereferencedExecute(string name)
	{
		string dereferencedServiceList = GetDereferencedServiceList(name);
		if (dereferencedServiceList != null)
		{
			InternalExecute(dereferencedServiceList, userInvoke: false);
		}
	}

	public void SetListVariable(string name, string value)
	{
		serviceListVariables[name] = value;
	}

	internal void InternalExecute(string serviceList, bool userInvoke)
	{
		string[] array = serviceList.Split(";".ToCharArray());
		Exception ex = null;
		for (int i = 0; i < array.Length; i++)
		{
			if (ex != null)
			{
				break;
			}
			if (!channel.ChannelRunning)
			{
				ex = new CaesarException(SapiError.CommunicationsCeasedDuringSyncOperation);
				break;
			}
			Service service = this[array[i]];
			if (service != null)
			{
				ex = service.InputValues.InternalParseValues(array[i], serviceListVariables);
				if (ex == null)
				{
					ex = service.InternalExecute(userInvoke ? Service.ExecuteType.UserInvokeFromList : Service.ExecuteType.SystemInvoke);
					continue;
				}
				Sapi.GetSapi().RaiseDebugInfoEvent(this, string.Format(CultureInfo.InvariantCulture, "Exception '{0}' while parsing inputs for service {1} during processing of service list", ex.Message, array[i]));
				ex = null;
			}
			else if (string.Equals(array[i], PauseQualifier, StringComparison.OrdinalIgnoreCase))
			{
				Thread.Sleep(1000);
			}
			else
			{
				Sapi.GetSapi().RaiseDebugInfoEvent(this, string.Format(CultureInfo.InvariantCulture, "Could not find service {0} during processing of service list", array[i]));
			}
		}
		if (userInvoke)
		{
			RaiseServiceCompleteEvent(serviceList, ex);
		}
		else if (ex != null)
		{
			Sapi.GetSapi().RaiseExceptionEvent(serviceList, ex);
		}
	}

	internal void SynchronousCheckFailure(object sender, CaesarException exception)
	{
		FireAndForget.Invoke(this.ServiceCompleteEvent, sender, new ResultEventArgs(exception));
	}

	public int Execute(string serviceList, bool synchronous)
	{
		int num = 0;
		if (serviceList != null)
		{
			string[] array = serviceList.Split(";".ToCharArray());
			for (int i = 0; i < array.Length; i++)
			{
				if (this[array[i]] != null)
				{
					num++;
				}
				else if (string.Equals(array[i], PauseQualifier, StringComparison.OrdinalIgnoreCase))
				{
					num++;
				}
			}
			if (num > 0)
			{
				channel.QueueAction(serviceList, synchronous, SynchronousCheckFailure);
			}
		}
		return num;
	}
}
