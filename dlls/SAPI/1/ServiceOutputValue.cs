// Decompiled with JetBrains decompiler
// Type: SapiLayer1.ServiceOutputValue
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using CaesarAbstraction;
using McdAbstraction;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
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

  internal ServiceOutputValue(Service service, ushort i)
    : base(i)
  {
    this.service = service;
    this.argumentValues = new ServiceArgumentValueCollection();
  }

  internal void Acquire(
    Channel channel,
    McdDBDiagComPrimitive diagService,
    McdDBResponseParameter response,
    IEnumerable<string> parentParameterNames,
    Dictionary<string, int> caesarEquivalentQualifiers)
  {
    Channel channel1 = channel;
    McdDBResponseParameter response1 = response;
    int num;
    if (this.service.ServiceTypes != ServiceTypes.DiagJob)
    {
      bool? isNoTransmission = this.service.IsNoTransmission;
      if (isNoTransmission.HasValue)
      {
        isNoTransmission = this.service.IsNoTransmission;
        num = isNoTransmission.Value ? 1 : 0;
      }
      else
        num = 0;
    }
    else
      num = 1;
    this.Acquire(channel1, response1, num != 0);
    this.parameterQualifier = response.Qualifier;
    if (this.service.ServiceTypes == ServiceTypes.Environment)
    {
      this.parameterName = response.Name;
      this.singleServiceName = response.Name;
      this.singleServiceQualifier = "ENV_" + McdCaesarEquivalence.MakeQualifier(this.parameterName);
    }
    else
    {
      string qualifier = this.service.Qualifier;
      string serviceName = this.service.Name;
      McdCaesarEquivalence.AdjustServiceQualifierName(diagService, response, ref qualifier, ref serviceName);
      string element = this.service.Channel.Ecu.CaesarEquivalentResponseParameterQualifierSource != Ecu.ResponseParameterQualifierSource.Qualifier ? response.Name : response.Qualifier;
      string[] array = parentParameterNames.Concat<string>(Enumerable.Repeat<string>(response.Name, 1)).ToArray<string>();
      this.singleServiceQualifier = McdCaesarEquivalence.MakeQualifier($"{qualifier}_{string.Join("_", parentParameterNames.Concat<string>(Enumerable.Repeat<string>(element, 1)))}", this.CanBeCaesarEquivalent ? caesarEquivalentQualifiers : (Dictionary<string, int>) null);
      this.singleServiceName = this.service.Channel.Ecu.Translate(Sapi.MakeTranslationIdentifier(this.singleServiceQualifier, "Name"), McdCaesarEquivalence.GetSingleServiceEquivalentName(serviceName, array));
      serviceName = this.Service.GetServiceName(this.singleServiceName, McdCaesarEquivalence.GetRetainedSuffix(serviceName) != null);
      this.parameterName = McdCaesarEquivalence.GetResponsePart(this.singleServiceName, serviceName, true);
    }
  }

  internal new void Acquire(Channel channel, CaesarDiagService diagService)
  {
    base.Acquire(channel, diagService);
    this.singleServiceName = this.service.Name;
    this.singleServiceQualifier = this.service.Qualifier;
    if (diagService.PresParamCount != 1U)
      return;
    if (diagService.ServiceType != 128 /*0x80*/)
    {
      string responsePart = McdCaesarEquivalence.GetResponsePart(this.service.Qualifier, this.service.ServiceQualifier, false);
      int length = responsePart.IndexOf("_physical", StringComparison.Ordinal);
      this.parameterQualifier = length != -1 ? responsePart.Substring(0, length) : responsePart;
      this.parameterName = McdCaesarEquivalence.GetResponsePart(this.service.Name, this.service.ServiceName, true);
    }
    else
    {
      this.parameterQualifier = this.service.Qualifier.Substring(4);
      this.parameterName = this.service.Name;
    }
  }

  internal void GetPresentation(CaesarDiagServiceIO diagServiceIO, ServiceExecution execution)
  {
    this.AddArgumentValue(this.GetPresentation(diagServiceIO), execution);
  }

  internal void GetPresentation(McdDiagComPrimitive diagServiceIO, ServiceExecution execution)
  {
    this.AddArgumentValue(this.GetPresentation(diagServiceIO), execution);
  }

  internal void GetPresentation(
    List<McdResponseParameter> responseParameter,
    ServiceExecution execution)
  {
    if (!(this.Type != (Type) null))
      return;
    this.AddArgumentValue(this.GetPresentation(responseParameter), execution);
  }

  private void AddArgumentValue(object newvalue, ServiceExecution execution)
  {
    this.heldValue = newvalue;
    execution.AddOutputArgumentValue(this.argumentValues.Add(this.manipulatedValue != null ? this.manipulatedValue : newvalue, execution.EndTime, false, (object) this));
  }

  internal new object ParseFromLog(string value)
  {
    if (this.Units.Length > 0)
    {
      int length = value.LastIndexOf(' ');
      if (length > -1)
        value = value.Substring(0, length);
    }
    return base.ParseFromLog(value);
  }

  public Service Service
  {
    get => this.service;
    internal set => this.service = value;
  }

  public object Value => this.argumentValues.Current?.Value;

  public override object ManipulatedValue
  {
    get => this.manipulatedValue;
    set
    {
      if (value == this.manipulatedValue)
        return;
      this.manipulatedValue = value;
      this.argumentValues.Add(this.manipulatedValue ?? this.heldValue, Sapi.Now, false, (object) this);
      this.Service.Channel.SetManipulatedState(this.service.Qualifier, value != null);
    }
  }

  public ServiceArgumentValueCollection ArgumentValues => this.argumentValues;

  public string ParameterQualifier => this.parameterQualifier;

  public string ParameterName => this.parameterName;

  internal void CreateStructuredOutputValues()
  {
    this.structuredOutputValues = new ServiceOutputValueCollection();
  }

  public ServiceOutputValueCollection StructuredOutputValues => this.structuredOutputValues;

  internal string SingleServiceQualifier => this.singleServiceQualifier;

  internal string SingleServiceName => this.singleServiceName;

  internal bool CanBeCaesarEquivalent
  {
    get => this.Type != (Type) null && !this.IsStructure && !this.IsArrayDefinition;
  }
}
