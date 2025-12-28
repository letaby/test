// Decompiled with JetBrains decompiler
// Type: SapiLayer1.ServiceCollection
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using CaesarAbstraction;
using McdAbstraction;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

#nullable disable
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

  internal ServiceCollection(Channel c, ServiceTypes serviceTypes, bool reconstructCombined = false)
  {
    this.serviceTypes = serviceTypes;
    this.channel = c;
    this.reconstructCombined = reconstructCombined;
    if (!this.channel.Ecu.Properties.ContainsKey(nameof (ResetTime)))
      return;
    byte result;
    if (!byte.TryParse(this.channel.Ecu.Properties[nameof (ResetTime)], out result))
      Sapi.GetSapi().RaiseDebugInfoEvent((object) this, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Error parsing reset time property for {0}", (object) this.channel.Ecu.Name));
    else
      this.resetTime = (object) result;
  }

  internal ServiceCollection(FaultCode faultCode)
  {
    this.serviceTypes = ServiceTypes.Environment;
    this.faultCode = faultCode;
    this.channel = faultCode.Channel;
  }

  internal ServiceCollection(Channel channel)
  {
    this.channel = channel;
    this.isStructuredSet = true;
  }

  protected override void AcquireList()
  {
    if (this.isStructuredSet)
      this.AcquireListStructured();
    else
      this.AcquireListCaesarEquivalent();
  }

  private void AcquireListStructured()
  {
    if (this.channel.McdEcuHandle != null)
    {
      foreach (McdCaesarEquivalence caesarEquivalence in McdCaesarEquivalence.FromDBLocation(this.channel.McdEcuHandle).Where<McdCaesarEquivalence>((Func<McdCaesarEquivalence, bool>) (eq => eq.ServiceTypes != ServiceTypes.None || eq.Service is McdDBControlPrimitive)))
      {
        Service service = new Service(this.channel, caesarEquivalence.ServiceTypes, caesarEquivalence.Qualifier);
        service.Acquire(caesarEquivalence.Name, caesarEquivalence.Service, (IEnumerable<McdDBResponseParameter>) null);
        this.Items.Add(service);
      }
    }
    else
    {
      if (this.channel.EcuHandle == null)
        return;
      foreach (Service combined in this.CreateCombinedSet(this.channel.EcuHandle.GetServices((ServiceTypes) 117245523).OfType<string>().Where<string>((Func<string, bool>) (q => this.channel.Ecu.IsVirtual || !q.EndsWith("_functional", StringComparison.Ordinal))).Select<string, Service>((Func<string, Service>) (q => this.AcquireService(q))).Where<Service>((Func<Service, bool>) (service => service != (Service) null))))
        this.Items.Add(combined);
    }
  }

  private void AcquireListCaesarEquivalent()
  {
    if (this.channel.IsRollCall)
    {
      foreach (Service service in this.channel.Ecu.RollCallManager.CreateServices(this.channel, this.serviceTypes))
        this.Items.Add(service);
    }
    else if (this.serviceTypes != ServiceTypes.None)
    {
      if (this.serviceTypes != ServiceTypes.Environment)
      {
        foreach (IGrouping<ServiceTypes, Tuple<Service, ServiceOutputValue>> grouping in this.channel.CaesarEquivalentServices.OrderBy<Tuple<Service, ServiceOutputValue>, string>((Func<Tuple<Service, ServiceOutputValue>, string>) (s => s.Item1.Qualifier)).GroupBy<Tuple<Service, ServiceOutputValue>, ServiceTypes>((Func<Tuple<Service, ServiceOutputValue>, ServiceTypes>) (s => s.Item1.ServiceTypes)))
        {
          ServiceTypes key = grouping.Key;
          if ((key & this.serviceTypes) != ServiceTypes.None || this.serviceTypes == ServiceTypes.StoredData && key == ServiceTypes.Data)
          {
            foreach (Tuple<Service, ServiceOutputValue> tuple in (IEnumerable<Tuple<Service, ServiceOutputValue>>) grouping)
            {
              string qualifier = tuple.Item2 != null ? tuple.Item2.SingleServiceQualifier : tuple.Item1.Qualifier;
              if (this.serviceTypes == ServiceTypes.Environment || !this.channel.Ecu.IgnoreQualifier(qualifier) && !this.channel.Ecu.MakeInstrumentQualifier(qualifier))
              {
                bool flag = (key & this.serviceTypes) != 0;
                if (this.serviceTypes == ServiceTypes.StoredData && key == ServiceTypes.Data && this.channel.Ecu.MakeStoredQualifier(qualifier) && !this.channel.Ecu.IgnoreQualifier(qualifier))
                  flag = true;
                if (flag)
                {
                  if (tuple.Item2 != null)
                  {
                    Service service = new Service(this.channel, tuple.Item1.ServiceTypes, qualifier);
                    service.Acquire(tuple.Item1, tuple.Item2);
                    this.Items.Add(service);
                  }
                  else
                    this.Items.Add(tuple.Item1);
                }
              }
            }
          }
        }
      }
      else if (this.channel.McdEcuHandle != null)
      {
        foreach (McdDBEnvDataDesc environmentDataDescription in this.channel.McdEcuHandle.DBEnvironmentDataDescriptions)
        {
          IEnumerable<McdDBResponseParameter> responseParameters = this.faultCode != null ? environmentDataDescription.GetFaultSpecificEnvironmentalDataSet((long) this.faultCode.LongNumber) : environmentDataDescription.CommonEnvironmentalDataSet;
          if (responseParameters.Any<McdDBResponseParameter>())
          {
            Service service = new Service(this.channel, ServiceTypes.Environment, environmentDataDescription.Qualifier);
            service.Acquire(environmentDataDescription.Name, (McdDBDiagComPrimitive) null, responseParameters);
            this.Items.Add(service);
          }
        }
      }
      else if (this.channel.EcuHandle != null)
      {
        foreach (string qualifier in this.channel.EcuHandle.GetServices((ServiceTypes) 128 /*0x80*/).OfType<string>())
        {
          Service service = this.AcquireService(qualifier);
          if (service != (Service) null)
            this.Items.Add(service);
        }
      }
    }
    if (!this.reconstructCombined)
      return;
    foreach (IGrouping<Service, Service> grouping in this.Items.Where<Service>((Func<Service, bool>) (s => s.ServiceTypes != ServiceTypes.DiagJob && s.OutputValues.Count == 1)).ToList<Service>().GroupBy<Service, Service>((Func<Service, Service>) (s => s.CombinedService)))
    {
      IGrouping<Service, Service> servicesByPrimitive = grouping;
      if (!this.Items.Any<Service>((Func<Service, bool>) (s => s.Qualifier == servicesByPrimitive.Key.Qualifier)))
        this.Items.Insert(this.Items.IndexOf(servicesByPrimitive.First<Service>()), servicesByPrimitive.Key);
    }
  }

  private IEnumerable<Service> CreateCombinedSet(IEnumerable<Service> singleServices)
  {
    Dictionary<Tuple<string, string>, List<Service>> dictionary = new Dictionary<Tuple<string, string>, List<Service>>();
    foreach (Service singleService in singleServices)
    {
      Tuple<string, string> key = Tuple.Create<string, string>(singleService.ServiceQualifier, singleService.BaseRequestMessage?.ToString());
      List<Service> serviceList;
      if (dictionary.TryGetValue(key, out serviceList))
        serviceList.Add(singleService);
      else
        dictionary.Add(key, Enumerable.Repeat<Service>(singleService, 1).ToList<Service>());
    }
    foreach (KeyValuePair<Tuple<string, string>, List<Service>> keyValuePair in dictionary)
    {
      KeyValuePair<Tuple<string, string>, List<Service>> servicesByPrimitive = keyValuePair;
      if (servicesByPrimitive.Key.Item2 == null || servicesByPrimitive.Value.First<Service>().OutputValues.Count == 0)
        yield return servicesByPrimitive.Value.First<Service>();
      else
        yield return this.AcquireService(servicesByPrimitive.Key.Item1, servicesByPrimitive.Value);
      servicesByPrimitive = new KeyValuePair<Tuple<string, string>, List<Service>>();
    }
  }

  private Service AcquireService(string qualifier)
  {
    try
    {
      using (CaesarDiagService diagService = this.channel.EcuHandle.OpenDiagServiceHandle(qualifier))
      {
        if (diagService != null)
        {
          Service service = new Service(this.channel, (ServiceTypes) diagService.ServiceType, qualifier);
          service.Acquire(diagService);
          return service;
        }
      }
    }
    catch (CaesarErrorException ex)
    {
      Sapi.GetSapi().RaiseExceptionEvent((object) qualifier, (Exception) new CaesarException(ex));
    }
    return (Service) null;
  }

  private Service AcquireService(string serviceQualifier, List<Service> sourceServices)
  {
    Service service = new Service(this.channel, sourceServices.First<Service>().ServiceTypes, serviceQualifier);
    service.Acquire(sourceServices);
    return service;
  }

  internal void RaiseServiceCompleteEvent(Service s, Exception e)
  {
    FireAndForget.Invoke((MulticastDelegate) this.ServiceCompleteEvent, (object) s, (EventArgs) new ResultEventArgs(e));
  }

  internal void RaiseServiceCompleteEvent(string serviceList, Exception e)
  {
    FireAndForget.Invoke((MulticastDelegate) this.ServiceCompleteEvent, (object) serviceList, (EventArgs) new ResultEventArgs(e));
    this.channel.SyncDone(e);
  }

  public string GetDereferencedServiceList(string shortPropertyName)
  {
    string key = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_{1}", (object) shortPropertyName, (object) this.channel.DiagnosisVariant.Name);
    for (int length = shortPropertyName.Length; key.Length >= length; key = key.Remove(key.Length - 1))
    {
      if (this.channel.Ecu.Properties.ContainsKey(key))
        return this.channel.Ecu.Properties[key];
    }
    return (string) null;
  }

  internal void InternalDereferencedExecute(string name)
  {
    string dereferencedServiceList = this.GetDereferencedServiceList(name);
    if (dereferencedServiceList == null)
      return;
    this.InternalExecute(dereferencedServiceList, false);
  }

  public void SetListVariable(string name, string value) => this.serviceListVariables[name] = value;

  internal void InternalExecute(string serviceList, bool userInvoke)
  {
    string[] strArray = serviceList.Split(";".ToCharArray());
    Exception e = (Exception) null;
    for (int index = 0; index < strArray.Length && e == null; ++index)
    {
      if (!this.channel.ChannelRunning)
      {
        e = (Exception) new CaesarException(SapiError.CommunicationsCeasedDuringSyncOperation);
        break;
      }
      Service service = this[strArray[index]];
      if (service != (Service) null)
      {
        Exception values = service.InputValues.InternalParseValues(strArray[index], this.serviceListVariables);
        if (values == null)
        {
          e = (Exception) service.InternalExecute(userInvoke ? Service.ExecuteType.UserInvokeFromList : Service.ExecuteType.SystemInvoke);
        }
        else
        {
          Sapi.GetSapi().RaiseDebugInfoEvent((object) this, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Exception '{0}' while parsing inputs for service {1} during processing of service list", (object) values.Message, (object) strArray[index]));
          e = (Exception) null;
        }
      }
      else if (string.Equals(strArray[index], ServiceCollection.PauseQualifier, StringComparison.OrdinalIgnoreCase))
        Thread.Sleep(1000);
      else
        Sapi.GetSapi().RaiseDebugInfoEvent((object) this, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Could not find service {0} during processing of service list", (object) strArray[index]));
    }
    if (userInvoke)
    {
      this.RaiseServiceCompleteEvent(serviceList, e);
    }
    else
    {
      if (e == null)
        return;
      Sapi.GetSapi().RaiseExceptionEvent((object) serviceList, e);
    }
  }

  internal void SynchronousCheckFailure(object sender, CaesarException exception)
  {
    FireAndForget.Invoke((MulticastDelegate) this.ServiceCompleteEvent, sender, (EventArgs) new ResultEventArgs((Exception) exception));
  }

  internal object ResetTime => this.resetTime;

  public int Execute(string serviceList, bool synchronous)
  {
    int num = 0;
    if (serviceList != null)
    {
      string[] strArray = serviceList.Split(";".ToCharArray());
      for (int index = 0; index < strArray.Length; ++index)
      {
        if (this[strArray[index]] != (Service) null)
          ++num;
        else if (string.Equals(strArray[index], ServiceCollection.PauseQualifier, StringComparison.OrdinalIgnoreCase))
          ++num;
      }
      if (num > 0)
        this.channel.QueueAction((object) serviceList, synchronous, new SynchronousCheckFailureHandler(this.SynchronousCheckFailure));
    }
    return num;
  }

  public bool ManipulateToPositiveResponse
  {
    get => this.manipulateToPositiveResponse;
    set
    {
      if (value == this.manipulateToPositiveResponse)
        return;
      this.manipulateToPositiveResponse = value;
      this.channel.SetManipulatedState(this.GetType().Name, value);
    }
  }

  public Service this[string qualifier]
  {
    get
    {
      if (qualifier == null)
        return (Service) null;
      qualifier = qualifier.RemoveArguments();
      if (this.cache == null)
        this.cache = this.ToDictionary((Func<Service, string>) (e => e.Qualifier), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      Service service = (Service) null;
      string key;
      if (!this.cache.TryGetValue(qualifier, out service) && this.channel.Ecu.AlternateQualifiers.TryGetValue(qualifier, out key))
        this.cache.TryGetValue(key, out service);
      return service;
    }
  }

  public event ServiceCompleteEventHandler ServiceCompleteEvent;
}
