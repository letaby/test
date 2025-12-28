// Decompiled with JetBrains decompiler
// Type: SapiLayer1.Service
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using CaesarAbstraction;
using McdAbstraction;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Xml;
using System.Xml.Linq;

#nullable disable
namespace SapiLayer1;

public sealed class Service : IComparable, IDiogenesDataItem
{
  internal const int defaultResetSleepTime = 10;
  internal const int defaultCacheTime = 100;
  internal const int defaultStoredDataCacheTime = 2000;
  internal const string LabelInputOutputSeparator = ")={";
  private const int MaxCaesarQualifierLength = 64 /*0x40*/;
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

  internal Service(Channel c, ServiceTypes st, string qualifier)
  {
    this.name = string.Empty;
    this.qualifier = qualifier;
    this.description = string.Empty;
    this.groupName = string.Empty;
    this.groupQualifier = string.Empty;
    this.channel = c;
    this.cacheTime = st == ServiceTypes.StoredData ? (ushort) 2000 : (ushort) 100;
    this.type = st;
    this.inputValues = new ServiceInputValueCollection(this);
    this.outputValues = new ServiceOutputValueCollection();
    this.executions = new ServiceExecutionCollection();
    this.negativeResponsesToIgnore = c.DiagnosisVariant.GetEcuInfoIgnoreNegativeResponses(this.qualifier);
  }

  internal void AcquireFromRollCall()
  {
    this.OutputValues.Add(new ServiceOutputValue(this, (ushort) 0));
  }

  internal void AcquireFromRollCall(IDictionary<string, string> content)
  {
    this.name = content.GetNamedPropertyValue<string>("Name", string.Empty);
    this.groupName = this.groupQualifier = this.ServiceTypes.ToString();
    this.visible = true;
    this.MessageNumber = new int?(content.GetNamedPropertyValue<int>("MessageNumber", -1));
    this.requestMessage = new Dump(content.GetNamedPropertyValue<string>("RequestMessage", string.Empty));
    if (this.channel.SourceAddress.HasValue && this.requestMessage.Data.Count > 6)
    {
      byte[] array = this.requestMessage.Data.ToArray<byte>();
      array[5] = this.channel.SourceAddress.Value;
      this.requestMessage = new Dump((IEnumerable<byte>) array);
    }
    this.cacheTime = (ushort) content.GetNamedPropertyValue<int>("CacheTime", 100);
    this.description = content.GetNamedPropertyValue<string>("Description", string.Empty);
  }

  public int? MessageNumber { private set; get; }

  internal void Acquire(
    string name,
    McdDBDiagComPrimitive diagService,
    IEnumerable<McdDBResponseParameter> responseParameters,
    Dictionary<string, string> specialData = null)
  {
    this.groupQualifier = this.ServiceTypes.ToString();
    this.groupName = this.ServiceTypes.ToString();
    this.name = name;
    this.requestMessage = diagService == null || diagService.RequestMessage == null ? (Dump) null : new Dump(diagService.RequestMessage);
    if (diagService != null)
    {
      if (diagService is McdDBControlPrimitive)
      {
        this.IsNoTransmission = new bool?(diagService.IsNoTransmission);
        this.IsControlPrimitive = true;
        this.groupQualifier = "ComPrimitive";
        this.groupName = "Control Primitives";
      }
      else
      {
        IList<string> functionalClassQualifiers = diagService.FunctionalClassQualifiers;
        if (functionalClassQualifiers != null && functionalClassQualifiers.Count > 0)
        {
          this.groupQualifier = functionalClassQualifiers[0];
          this.groupName = diagService.GetFunctionalClassName(0);
        }
      }
      ushort indexDI = 0;
      ushort num = 0;
      foreach (McdDBRequestParameter requestParameter in diagService.AllRequestParameters)
      {
        if (!requestParameter.IsConst && !requestParameter.IsStructure && requestParameter.DataType != (Type) null)
        {
          ServiceInputValue serviceInputValue = new ServiceInputValue(this, indexDI, num++);
          serviceInputValue.AcquirePreparation(requestParameter);
          this.inputValues.Add(serviceInputValue);
        }
        ++indexDI;
      }
    }
    if (responseParameters == null)
      responseParameters = diagService.ResponseParameters;
    ushort index = 0;
    Dictionary<string, int> caesarEquivalentQualifiers = new Dictionary<string, int>();
    this.structuredOutputValues = new ServiceOutputValueCollection();
    AcquireServiceOutputValueCollection(this.structuredOutputValues, responseParameters, new List<string>());
    if (diagService != null)
    {
      this.mcdQualifier = diagService.Qualifier;
      IEnumerable<string> additionalAudiences = diagService.EnabledAdditionalAudiences;
      this.enabledAdditionalAudiences = additionalAudiences != null ? additionalAudiences.ToList<string>() : (List<string>) null;
    }
    this.visible = true;
    this.specialData = specialData;
    this.AcquireCommonEcuInfo();

    void AcquireServiceOutputValueCollection(
      ServiceOutputValueCollection outputValues,
      IEnumerable<McdDBResponseParameter> responseParameterSet,
      List<string> parentParameterNames)
    {
      bool flag = responseParameterSet.AllSiblingsAreStructures();
      foreach (McdDBResponseParameter responseParameter in responseParameterSet)
      {
        if (!responseParameter.IsConst && !responseParameter.IsMatchingRequestParameter && !responseParameter.IsReserved)
        {
          ServiceOutputValue serviceOutputValue = new ServiceOutputValue(this, index++);
          serviceOutputValue.Acquire(this.channel, diagService, responseParameter, (IEnumerable<string>) parentParameterNames, caesarEquivalentQualifiers);
          if (responseParameter.Parameters.Any<McdDBResponseParameter>())
          {
            List<string> list = parentParameterNames.ToList<string>();
            if (!responseParameter.IsStructure | flag)
              list.Add(responseParameter.Name);
            serviceOutputValue.CreateStructuredOutputValues();
            AcquireServiceOutputValueCollection(serviceOutputValue.StructuredOutputValues, responseParameter.Parameters, list.ToList<string>());
          }
          outputValues.Add(serviceOutputValue);
          if (serviceOutputValue.CanBeCaesarEquivalent)
            this.outputValues.Add(serviceOutputValue);
        }
        else
          index++;
      }
    }
  }

  internal void Acquire(List<Service> sourceServices)
  {
    Service service = sourceServices.First<Service>();
    this.name = service.ServiceName;
    this.description = service.description;
    this.requestMessage = service.requestMessage;
    this.accessLevel = service.accessLevel;
    this.visible = service.visible;
    this.caesarQualifier = service.caesarQualifier;
    this.groupQualifier = service.groupQualifier;
    this.groupName = service.groupName;
    service.inputValues.ToList<ServiceInputValue>().ForEach((Action<ServiceInputValue>) (iv => this.inputValues.Add(iv)));
    foreach (Service sourceService in sourceServices)
    {
      sourceService.CombinedService = this;
      this.outputValues.Add(sourceService.outputValues[0]);
    }
    this.AcquireCommonEcuInfo();
  }

  internal void Acquire(Service structuredService, ServiceOutputValue outputValue)
  {
    this.name = this.type != ServiceTypes.Environment ? (outputValue != null ? outputValue.SingleServiceName : McdCaesarEquivalence.GetSingleServiceEquivalentName(structuredService.name, (string[]) null)) : outputValue.ParameterName;
    this.description = structuredService.description;
    this.requestMessage = structuredService.requestMessage;
    this.mcdQualifier = structuredService.mcdQualifier;
    this.caesarQualifier = this.qualifier;
    this.accessLevel = structuredService.accessLevel;
    this.visible = structuredService.visible;
    this.type = structuredService.ServiceTypes;
    this.enabledAdditionalAudiences = structuredService.enabledAdditionalAudiences;
    this.groupQualifier = structuredService.groupQualifier;
    this.groupName = structuredService.groupName;
    structuredService.inputValues.ToList<ServiceInputValue>().ForEach((Action<ServiceInputValue>) (iv => this.inputValues.Add(iv)));
    if (outputValue != null)
    {
      this.CombinedService = structuredService;
      outputValue.Service = this;
      this.outputValues.Add(outputValue);
    }
    else if (this.type == ServiceTypes.DiagJob)
    {
      this.outputValues = structuredService.outputValues;
      this.structuredOutputValues = structuredService.structuredOutputValues;
    }
    this.AcquireCommonEcuInfo();
  }

  internal void Acquire(CaesarDiagService diagService)
  {
    this.groupName = this.groupQualifier = this.ServiceTypes.ToString();
    int length = diagService.Name.IndexOf("_physical", StringComparison.Ordinal);
    this.name = length != -1 ? diagService.Name.Substring(0, length) : diagService.Name;
    this.description = diagService.Description;
    this.caesarQualifier = diagService.Qualifier;
    IList<byte> requestMessage = diagService.RequestMessage;
    if (requestMessage != null)
      this.requestMessage = new Dump((IEnumerable<byte>) requestMessage);
    bool flag = false;
    ushort num = 0;
    for (ushort indexDI = 0; (uint) indexDI < diagService.PrepParamCount; ++indexDI)
    {
      if (diagService.GetPrepType((uint) indexDI) != 23)
      {
        ServiceInputValue serviceInputValue = new ServiceInputValue(this, indexDI, num++);
        serviceInputValue.AcquirePreparation(diagService);
        this.inputValues.Add(serviceInputValue);
        if (serviceInputValue.Type == (Type) null)
          flag = true;
      }
    }
    for (ushort i = 0; (uint) i < diagService.PresParamCount; ++i)
    {
      ServiceOutputValue serviceOutputValue = new ServiceOutputValue(this, i);
      serviceOutputValue.Acquire(this.channel, diagService);
      this.outputValues.Add(serviceOutputValue);
    }
    this.accessLevel = diagService.AccessLevel;
    uint argValueCount = diagService.ArgValueCount;
    if (argValueCount > 0U)
    {
      this.arguments = new StringCollection();
      for (uint index = 0; index < argValueCount; ++index)
        this.arguments.Add(diagService.GetArgValue(index));
    }
    this.AcquireCommonEcuInfo();
    if (!flag)
      return;
    this.visible = false;
  }

  private void AcquireCommonEcuInfo()
  {
    Sapi sapi = Sapi.GetSapi();
    switch (this.type)
    {
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
        int? writeAccessLevel1 = this.Channel.DiagnosisVariant.GetEcuInfoWriteAccessLevel(this.Qualifier);
        if (writeAccessLevel1.HasValue)
          this.accessLevel = (ushort) writeAccessLevel1.Value;
        this.visible = sapi.WriteAccess >= (int) this.accessLevel;
        break;
      case ServiceTypes.Data:
      case ServiceTypes.Environment:
      case ServiceTypes.Static:
      case ServiceTypes.Global:
      case ServiceTypes.StoredData:
      case ServiceTypes.ReadVarCode:
        if (this.inputValues.Count > 0)
        {
          int? writeAccessLevel2 = this.Channel.DiagnosisVariant.GetEcuInfoWriteAccessLevel(this.Qualifier);
          if (writeAccessLevel2.HasValue)
            this.accessLevel = (ushort) writeAccessLevel2.Value;
          this.visible = sapi.WriteAccess >= (int) this.accessLevel;
          break;
        }
        int? infoReadAccessLevel = this.Channel.DiagnosisVariant.GetEcuInfoReadAccessLevel(this.Qualifier);
        if (infoReadAccessLevel.HasValue)
          this.accessLevel = (ushort) infoReadAccessLevel.Value;
        this.visible = sapi.ReadAccess >= (int) this.accessLevel;
        break;
    }
    if (this.channel.Ecu.PreService.ContainsKey(this.qualifier))
      this.SetPreService(this.channel.Ecu.PreService[this.qualifier]);
    if (this.channel.Ecu.AffectServices.ContainsKey(this.qualifier))
      this.SetAffected(this.channel.Ecu.AffectServices[this.qualifier]);
    this.systemRequestLimit = this.channel.DiagnosisVariant.GetEcuInfoAttribute<int?>("SystemRequestLimit", this.qualifier);
    this.systemRequestLimitResetOn = this.Channel.DiagnosisVariant.GetEcuInfoAttribute<string>("SystemRequestLimitResetOn", this.qualifier);
  }

  internal CaesarException InternalExecute(Service.ExecuteType invokeType)
  {
    return this.InternalExecute(invokeType, new ServiceExecution(this));
  }

  internal CaesarException InternalExecute(
    Service.ExecuteType invokeType,
    ServiceExecution currentExecution)
  {
    CaesarException ce = (CaesarException) null;
    if (invokeType == Service.ExecuteType.SystemInvoke && this.systemRequestLimit.HasValue && this.executeCount >= this.systemRequestLimit.Value)
      return (CaesarException) null;
    if ((this.channel.ChannelOptions & ChannelOptions.ExecutePreService) != ChannelOptions.None && this.preService != null)
      this.channel.Services.InternalExecute(this.preService, false);
    if (this.outputValues.Any<ServiceOutputValue>() && this.outputValues.All<ServiceOutputValue>((Func<ServiceOutputValue, bool>) (ov => ov.Service.CombinedService == this)))
    {
      foreach (Service service in this.outputValues.Select<ServiceOutputValue, Service>((Func<ServiceOutputValue, Service>) (ov => ov.Service)).ToList<Service>())
      {
        ce = service.InternalExecute(Service.ExecuteType.CombinedServiceInvoke, currentExecution);
        this.responseMessage = service.ResponseMessage;
      }
    }
    else
    {
      currentExecution.StartTime = Sapi.Now;
      if (!this.channel.IsRollCall)
      {
        if (this.channel.McdChannelHandle != null)
        {
          this.responseMessage = (Dump) null;
          try
          {
            McdDiagComPrimitive service = this.channel.McdChannelHandle.GetService(this.mcdQualifier);
            for (int index = 0; index < this.inputValues.Count && !this.channel.IsChannelErrorSet; ++index)
            {
              ServiceInputValue inputValue = this.inputValues[index];
              if (inputValue.Type != (Type) null && !inputValue.IsReserved)
                inputValue.SetPreparation(service, currentExecution);
            }
            ushort cacheTime = this.cacheTime;
            if (this.Channel.CommunicationsState == CommunicationsState.ReadEcuInfo && invokeType == Service.ExecuteType.EcuInfoInvoke)
              cacheTime = ushort.MaxValue;
            service.Execute((int) cacheTime);
            currentExecution.EndTime = Sapi.Now;
            ++this.executeCount;
            if (!service.IsNegativeResponse)
            {
              foreach (KeyValuePair<ServiceOutputValue, List<McdResponseParameter>> keyValuePair in service.AllPositiveResponseParameters.GroupBy<McdResponseParameter, string>((Func<McdResponseParameter, string>) (pr => pr.QualifierPath)).Select<IGrouping<string, McdResponseParameter>, Tuple<ServiceOutputValue, List<McdResponseParameter>>>((Func<IGrouping<string, McdResponseParameter>, Tuple<ServiceOutputValue, List<McdResponseParameter>>>) (g => Tuple.Create<ServiceOutputValue, List<McdResponseParameter>>(this.OutputValues.FirstOrDefault<ServiceOutputValue>((Func<ServiceOutputValue, bool>) (ov => ov.McdParameterQualifierPath == g.Key)), g.ToList<McdResponseParameter>()))).Where<Tuple<ServiceOutputValue, List<McdResponseParameter>>>((Func<Tuple<ServiceOutputValue, List<McdResponseParameter>>, bool>) (t => t.Item1 != null)).ToDictionary<Tuple<ServiceOutputValue, List<McdResponseParameter>>, ServiceOutputValue, List<McdResponseParameter>>((Func<Tuple<ServiceOutputValue, List<McdResponseParameter>>, ServiceOutputValue>) (k => k.Item1), (Func<Tuple<ServiceOutputValue, List<McdResponseParameter>>, List<McdResponseParameter>>) (v => v.Item2)))
                keyValuePair.Key.GetPresentation(keyValuePair.Value, currentExecution);
            }
            else
            {
              foreach (ServiceOutputValue outputValue in (ReadOnlyCollection<ServiceOutputValue>) this.outputValues)
                outputValue.GetPresentation(service, currentExecution);
            }
            IEnumerable<byte> responseMessage = service.ResponseMessage;
            if (responseMessage != null)
              this.responseMessage = new Dump(responseMessage);
            if (service.IsNegativeResponse)
            {
              ce = new CaesarException(service);
              currentExecution.NegativeResponseCode = new int?(ce.NegativeResponseCode);
            }
          }
          catch (McdException ex)
          {
            ce = new CaesarException(ex);
            currentExecution.Error = ce.Message;
          }
        }
        else
        {
          using (CaesarDiagServiceIO diagServiceIo = this.channel.ChannelHandle.CreateDiagServiceIO(this.caesarQualifier))
          {
            this.responseMessage = (Dump) null;
            if (!this.channel.IsChannelErrorSet)
            {
              for (int index = 0; index < this.inputValues.Count && !this.channel.IsChannelErrorSet; ++index)
                this.inputValues[index].SetPreparation(diagServiceIo, currentExecution);
              if (!this.channel.IsChannelErrorSet)
              {
                ushort num = this.cacheTime;
                if (this.Channel.CommunicationsState == CommunicationsState.ReadEcuInfo && invokeType == Service.ExecuteType.EcuInfoInvoke)
                  num = ushort.MaxValue;
                try
                {
                  diagServiceIo.Do(num);
                  currentExecution.EndTime = Sapi.Now;
                  ++this.executeCount;
                  for (int index = 0; index < this.outputValues.Count; ++index)
                  {
                    if (!this.channel.IsChannelErrorSet)
                      this.outputValues[index].GetPresentation(diagServiceIo, currentExecution);
                    else
                      break;
                  }
                }
                catch (AccessViolationException ex)
                {
                  ce = new CaesarException(SapiError.AccessViolationDuringServiceExecution);
                }
              }
            }
            if (!this.channel.IsChannelErrorSet)
            {
              IList<byte> responseMessage = diagServiceIo.ResponseMessage;
              if (responseMessage != null)
                this.responseMessage = new Dump((IEnumerable<byte>) responseMessage);
            }
            if (this.channel.IsChannelErrorSet)
            {
              ce = new CaesarException(this.channel.ChannelHandle);
              currentExecution.Error = ce.Message;
            }
            else if (diagServiceIo.IsNegativeResponse)
            {
              ce = new CaesarException(diagServiceIo);
              currentExecution.NegativeResponseCode = new int?(ce.NegativeResponseCode);
            }
          }
        }
      }
      else
      {
        try
        {
          this.channel.Ecu.RollCallManager.DoByteMessage(this.channel, this.RequestMessage.Data.ToArray<byte>(), (byte[]) null);
          Thread.Sleep((int) this.cacheTime);
        }
        catch (CaesarException ex)
        {
          ce = ex;
        }
      }
      if (this.OutputValues.Any<ServiceOutputValue>((Func<ServiceOutputValue, bool>) (ov => ov.ManipulatedValue != null)) || this.channel.Services.ManipulateToPositiveResponse)
        ce = (CaesarException) null;
      if (ce != null && this.negativeResponsesToIgnore != null && ((IEnumerable<byte>) this.negativeResponsesToIgnore).Any<byte>((Func<byte, bool>) (nrc => ce.NegativeResponseCode == (int) nrc)))
      {
        Sapi.GetSapi().RaiseDebugInfoEvent((object) this, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}: Ignoring negative response ${1:X}", (object) this.qualifier, (object) ce.NegativeResponseCode));
        ce = (CaesarException) null;
      }
    }
    if ((this.channel.ChannelOptions & ChannelOptions.ProcessAffected) != ChannelOptions.None && ce == null)
      this.ProcessAffected();
    if (invokeType == Service.ExecuteType.UserInvoke || invokeType == Service.ExecuteType.UserInvokeFromList)
      this.RaiseServiceCompleteEvent(ce, invokeType, currentExecution);
    if (this.channel.Ecu.IsUds && ce == null && this.requestMessage != null && this.requestMessage.Data[0] == (byte) 17 && this.responseMessage != null)
    {
      byte num = byte.MaxValue;
      if (this.responseMessage.Data.Count > 2)
        num = this.responseMessage.Data[2];
      else if (this.responseMessage.Data.Count == 2)
        num = (byte) 0;
      if (this.channel.Services.ResetTime != null)
        num = Convert.ToByte(this.channel.Services.ResetTime, (IFormatProvider) CultureInfo.InvariantCulture);
      if (num == byte.MaxValue)
      {
        num = (byte) 10;
        Sapi.GetSapi().RaiseDebugInfoEvent((object) this, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Reset command {0} for {1}: no sleep time available, using default {2}s", (object) this.name, (object) this.channel.Ecu.Name, (object) 10));
      }
      Thread.Sleep((int) num * 1000);
      ce = this.channel.Reset();
    }
    return ce;
  }

  internal void CheckInputs()
  {
    for (int index = 0; index < this.inputValues.Count; ++index)
    {
      ServiceInputValue inputValue = this.inputValues[index];
      if (inputValue.Value == null)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Input argument '{0}' was not set", (object) inputValue.Name));
    }
  }

  private void RaiseServiceCompleteEvent(
    CaesarException e,
    Service.ExecuteType invokeType,
    ServiceExecution currentExecution)
  {
    if (invokeType != Service.ExecuteType.UserInvoke && invokeType != Service.ExecuteType.UserInvokeFromList)
      return;
    FireAndForget.Invoke((MulticastDelegate) this.ServiceCompleteEvent, (object) this, (EventArgs) new ResultEventArgs((Exception) e));
    if (invokeType == Service.ExecuteType.UserInvoke)
      this.channel.Services.RaiseServiceCompleteEvent(this, (Exception) e);
    Service.LabelLog(currentExecution);
    this.executions.Add(currentExecution, false);
    if (invokeType != Service.ExecuteType.UserInvoke)
      return;
    this.channel.SyncDone((Exception) e);
  }

  internal void AddStringsForTranslation(Dictionary<string, string> table)
  {
    table[Sapi.MakeTranslationIdentifier(this.qualifier, "Name")] = this.name;
    if (!string.IsNullOrEmpty(this.description))
      table[Sapi.MakeTranslationIdentifier(this.qualifier, "Description")] = this.description;
    if (this.InputValues != null)
    {
      foreach (ServiceInputValue inputValue in (ReadOnlyCollection<ServiceInputValue>) this.InputValues)
        inputValue.AddStringsForTranslation(table);
    }
    if (this.OutputValues == null)
      return;
    foreach (Presentation outputValue in (ReadOnlyCollection<ServiceOutputValue>) this.OutputValues)
      outputValue.AddStringsForTranslation(table);
  }

  internal void SetPreService(string item)
  {
    this.preService = this.channel.Services.GetDereferencedServiceList(item) ?? item;
  }

  public string PreService => this.preService;

  internal void SetAffected(string item) => this.affected = item;

  private static void LabelLog(ServiceExecution currentExecution)
  {
    Sapi sapi = Sapi.GetSapi();
    if (!sapi.LogFiles.Logging)
      return;
    sapi.LogFiles.LabelLog(currentExecution.CreateLabel());
  }

  internal static bool IsServiceLabel(Label label, out string serviceName)
  {
    serviceName = (string) null;
    if (label.Ecu != null && label.Name.Contains(")={"))
      serviceName = label.Name.Substring(0, label.Name.IndexOf('('));
    return serviceName != null;
  }

  internal void ParseFromLog(string label, DateTime startTime, DateTime endTime)
  {
    this.executions.Add(ServiceExecution.ParseFromLog(label, startTime, endTime, this), true);
  }

  internal void SynchronousCheckFailure(object sender, CaesarException exception)
  {
    FireAndForget.Invoke((MulticastDelegate) this.ServiceCompleteEvent, (object) this, (EventArgs) new ResultEventArgs((Exception) exception));
    this.channel.Services.RaiseServiceCompleteEvent(this, (Exception) exception);
  }

  public void Execute(bool synchronous)
  {
    this.CheckInputs();
    this.channel.QueueAction((object) new ServiceExecution(this), synchronous, new SynchronousCheckFailureHandler(this.SynchronousCheckFailure));
  }

  public Channel Channel => this.channel;

  public string Qualifier => this.qualifier;

  public string DiagnosisQualifier
  {
    get => !this.Channel.Ecu.IsMcd ? this.caesarQualifier : this.mcdQualifier;
  }

  public string Name
  {
    get
    {
      return this.channel.IsRollCall ? this.channel.Ecu.Translate(Sapi.MakeTranslationIdentifier(this.qualifier, "SPN"), this.name) : this.channel.Ecu.Translate(Sapi.MakeTranslationIdentifier(this.qualifier, nameof (Name)), this.name);
    }
  }

  internal string GetServiceName(string name, bool checkPrefixes)
  {
    string[] source = name.Split(new string[1]
    {
      this.Channel.Ecu.NameSplit
    }, StringSplitOptions.None);
    if (this.channel.Ecu.DiagnosisSource == DiagnosisSource.CaesarDatabase && (this.ServiceTypes == ServiceTypes.ReadVarCode || this.ServiceTypes == ServiceTypes.WriteVarCode) && source.Length > 2)
      return string.Join(this.Channel.Ecu.NameSplit, ((IEnumerable<string>) source).Take<string>(source.Length - 1));
    if (source.Length > 1 & checkPrefixes)
    {
      string[] strArray = new string[5]
      {
        "Start",
        "Stop",
        "Request Results",
        "Send",
        "Request"
      };
      foreach (string str in strArray)
      {
        if (source[1].Equals(str, StringComparison.OrdinalIgnoreCase) || source[1].StartsWith(str + " ", StringComparison.OrdinalIgnoreCase))
          return source[0] + this.Channel.Ecu.NameSplit + str;
      }
    }
    return source[0];
  }

  internal string ServiceQualifier
  {
    get
    {
      if (this.serviceQualifier == null)
      {
        string serviceName = this.GetServiceName(this.name, this.requestMessage == null || this.requestMessage.Data[0] == (byte) 39 || this.requestMessage.Data[0] == (byte) 49);
        int position1;
        int length1;
        if (McdCaesarEquivalence.TryGetIndexLengthIgnoreUnderscores(this.qualifier, new string(McdCaesarEquivalence.MakeQualifier(serviceName).Take<char>(64 /*0x40*/).ToArray<char>()), out position1, out length1))
        {
          this.serviceQualifier = this.qualifier.Substring(0, position1 + length1);
        }
        else
        {
          int position2;
          int length2;
          if (McdCaesarEquivalence.TryGetIndexLengthIgnoreUnderscores(this.qualifier, McdCaesarEquivalence.MakeQualifier(McdCaesarEquivalence.GetResponsePart(this.name, serviceName, true)), out position2, out length2) && position2 + length2 == this.qualifier.Length)
          {
            this.serviceQualifier = this.qualifier.Substring(0, position2).TrimEnd('_');
          }
          else
          {
            this.serviceQualifier = this.qualifier;
            Sapi.GetSapi().RaiseDebugInfoEvent((object) this, $"Could not determine service qualifier using either method. {this.channel.Ecu.Name}.{this.channel.DiagnosisVariant.Name}.{this.qualifier}; Name is {this.name}");
          }
        }
      }
      return this.serviceQualifier;
    }
  }

  internal string ServiceName
  {
    get
    {
      return this.GetServiceName(this.Name, this.requestMessage == null || this.requestMessage.Data[0] == (byte) 39 || this.requestMessage.Data[0] == (byte) 49);
    }
  }

  public Service CombinedService { internal set; get; }

  public string ShortName => this.channel.Ecu.ShortName(this.Name);

  public string Description
  {
    get
    {
      return this.channel.Ecu.Translate(Sapi.MakeTranslationIdentifier(this.qualifier, nameof (Description)), this.description);
    }
  }

  public string GroupName => this.groupName;

  public string GroupQualifier => this.groupQualifier;

  public string Units => this.outputValues.Count > 0 ? this.outputValues[0].Units : string.Empty;

  public object Precision
  {
    get => this.outputValues.Count > 0 ? this.outputValues[0].Precision : (object) null;
  }

  public ChoiceCollection Choices
  {
    get => this.outputValues.Count > 0 ? this.outputValues[0].Choices : (ChoiceCollection) null;
  }

  public Dump RequestMessage
  {
    get
    {
      if (this.channel.McdChannelHandle != null)
      {
        McdDiagComPrimitive service = this.channel.McdChannelHandle.GetService(this.mcdQualifier);
        for (int index = 0; index < this.inputValues.Count && !this.channel.IsChannelErrorSet; ++index)
        {
          ServiceInputValue inputValue = this.inputValues[index];
          if (inputValue.Type != (Type) null && !inputValue.IsReserved)
            inputValue.SetPreparation(service);
        }
        return new Dump(service.RequestMessage);
      }
      if (this.channel.ChannelHandle == null)
        return this.requestMessage;
      using (CaesarDiagService diagService = this.channel.EcuHandle.OpenDiagServiceHandle(this.caesarQualifier))
      {
        for (int index = 0; index < this.inputValues.Count && !this.channel.IsChannelErrorSet; ++index)
          this.inputValues[index].SetPreparation(diagService);
        return new Dump((IEnumerable<byte>) diagService.RequestMessage);
      }
    }
  }

  public Dump BaseRequestMessage => this.requestMessage;

  internal string McdQualifier => this.mcdQualifier;

  public Dump RequestMessageMask
  {
    get
    {
      if (this.requestMessageMask == null && this.requestMessage != null)
      {
        byte[] array = Enumerable.Repeat<byte>(byte.MaxValue, this.requestMessage.Data.Count).ToArray<byte>();
        if (this.InputValues != null)
        {
          foreach (ServiceInputValue inputValue in (ReadOnlyCollection<ServiceInputValue>) this.InputValues)
          {
            int int32_1 = Convert.ToInt32((object) inputValue.BytePosition, (IFormatProvider) CultureInfo.InvariantCulture);
            int int32_2 = Convert.ToInt32((object) inputValue.ByteLength, (IFormatProvider) CultureInfo.InvariantCulture);
            Array.Copy((Array) Enumerable.Repeat<byte>((byte) 0, int32_2).ToArray<byte>(), 0, (Array) array, int32_1, int32_2);
          }
        }
        int length = array.Length;
        while (length > 0 && array[length - 1] == (byte) 0)
          --length;
        this.requestMessageMask = new Dump(((IEnumerable<byte>) array).Take<byte>(length));
      }
      return this.requestMessageMask;
    }
  }

  public Dump ResponseMessage => this.responseMessage;

  public ServiceInputValueCollection InputValues => this.inputValues;

  public ServiceOutputValueCollection OutputValues => this.outputValues;

  public ServiceOutputValueCollection StructuredOutputValues => this.structuredOutputValues;

  public bool Visible => this.visible;

  public ServiceTypes ServiceTypes => this.type;

  public int Access => (int) this.accessLevel;

  public int CacheTimeout
  {
    get => (int) this.cacheTime;
    set => this.cacheTime = (ushort) value;
  }

  public StringCollection Arguments => this.arguments;

  public Dictionary<string, string> SpecialData => this.specialData;

  public bool IsControlPrimitive { private set; get; }

  internal bool? IsNoTransmission { private set; get; }

  public event ServiceCompleteEventHandler ServiceCompleteEvent;

  public int CompareTo(object obj)
  {
    Service service = obj as Service;
    return service.ServiceTypes != this.ServiceTypes ? this.ServiceTypes.CompareTo((object) service.ServiceTypes) : string.Compare(this.Name, service.Name, StringComparison.Ordinal);
  }

  public override bool Equals(object obj) => base.Equals(obj);

  public override int GetHashCode() => base.GetHashCode();

  public static bool operator ==(Service object1, Service object2)
  {
    return object.Equals((object) object1, (object) object2);
  }

  public static bool operator !=(Service object1, Service object2)
  {
    return !object.Equals((object) object1, (object) object2);
  }

  public static bool operator <(Service r1, Service r2)
  {
    if (r1 == (Service) null)
      throw new ArgumentNullException(nameof (r1));
    return r1.CompareTo((object) r2) < 0;
  }

  public static bool operator >(Service r1, Service r2)
  {
    if (r1 == (Service) null)
      throw new ArgumentNullException(nameof (r1));
    return r1.CompareTo((object) r2) > 0;
  }

  private void ProcessAffected()
  {
    if (this.affected != null)
    {
      if (string.Equals(this.affected, "Parameters", StringComparison.OrdinalIgnoreCase))
        this.channel.Parameters.ResetEcuReadFlags();
      else if (string.Equals(this.affected, "EcuInfo", StringComparison.OrdinalIgnoreCase))
      {
        this.channel.EcuInfos.InternalRead(false);
      }
      else
      {
        string[] strArray = this.affected.Split(";".ToCharArray());
        for (int index = 0; index < strArray.Length; ++index)
        {
          EcuInfo ecuInfo = this.channel.EcuInfos[strArray[index]];
          if (ecuInfo != null)
            ecuInfo.InternalRead(false);
          else
            Sapi.GetSapi().RaiseDebugInfoEvent((object) this, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Could not find affected item {0} for service {1}", (object) strArray[index], (object) this.Qualifier));
        }
      }
    }
    foreach (Service service in this.channel.Services.Where<Service>((Func<Service, bool>) (s => s.systemRequestLimitResetOn != null && ((IEnumerable<string>) s.systemRequestLimitResetOn.Split(",;".ToCharArray())).Contains<string>(this.qualifier))))
      service.executeCount = 0;
  }

  [CLSCompliant(false)]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Obsolete("CacheTime is deprecated due to non-CLS compliance, please use CacheTimeout instead.")]
  public ushort CacheTime
  {
    get => this.cacheTime;
    set => this.cacheTime = value;
  }

  [CLSCompliant(false)]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Obsolete("AccessLevel is deprecated due to non-CLS compliance, please use Access instead.")]
  public ushort AccessLevel => this.accessLevel;

  public IEnumerable<string> EnabledAdditionalAudiences
  {
    get => (IEnumerable<string>) this.enabledAdditionalAudiences;
  }

  public ServiceExecutionCollection Executions => this.executions;

  internal static void LoadFromLog(
    XElement element,
    LogFileFormatTagCollection format,
    Channel channel,
    List<string> missingQualifierList,
    object missingInfoLock)
  {
    string qualifier = element.Attribute(format[TagName.Qualifier]).Value;
    Service service = ((IEnumerable<Service>) new Service[2]
    {
      channel.Services[qualifier],
      channel.StructuredServices?[qualifier]
    }).Where<Service>((Func<Service, bool>) (s => s != (Service) null)).Distinct<Service>().FirstOrDefault<Service>();
    if (service != (Service) null)
    {
      DateTime dateTime = DateTime.MinValue;
      if (service.executions.Count > 0)
        dateTime = service.executions.Last<ServiceExecution>().EndTime;
      foreach (XElement element1 in element.Elements(format[TagName.Execution]))
      {
        ServiceExecution serviceExecution = ServiceExecution.FromXElement(element1, format, service, missingQualifierList, missingInfoLock);
        if (serviceExecution.StartTime > dateTime)
          service.executions.Add(serviceExecution, true);
      }
    }
    else
    {
      if (channel.Ecu.IgnoreQualifier(qualifier))
        return;
      lock (missingInfoLock)
        missingQualifierList.Add(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}", (object) channel.Ecu.Name, (object) qualifier));
    }
  }

  internal void WriteXmlTo(DateTime startTime, DateTime endTime, XmlWriter writer)
  {
    LogFileFormatTagCollection currentFormat = LogFile.CurrentFormat;
    writer.WriteStartElement(currentFormat[TagName.Service].LocalName);
    writer.WriteAttributeString(currentFormat[TagName.Qualifier].LocalName, this.Qualifier);
    foreach (ServiceExecution execution in this.executions)
    {
      if (execution.StartTime >= startTime && execution.EndTime <= endTime)
        execution.WriteXmlTo(writer);
    }
    writer.WriteEndElement();
  }

  internal enum ExecuteType
  {
    UserInvoke,
    UserInvokeFromList,
    SystemInvoke,
    EcuInfoInvoke,
    EcuInfoUserInvoke,
    CombinedServiceInvoke,
  }
}
