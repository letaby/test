// Decompiled with JetBrains decompiler
// Type: SapiLayer1.VarcodeMcd
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using CaesarAbstraction;
using McdAbstraction;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

#nullable disable
namespace SapiLayer1;

internal class VarcodeMcd : Varcode
{
  private Channel channel;
  private McdLogicalLink mcdVarcoding;
  private bool mcdVarcodingOwner;
  private List<McdDiagComPrimitive> pendingPrimitives = new List<McdDiagComPrimitive>();

  internal VarcodeMcd(Channel channel, string interfaceQualifier, McdInterface mcdInterface)
  {
    this.channel = channel;
    this.mcdVarcoding = McdRoot.CreateOfflineLogicalLink(interfaceQualifier, this.channel.DiagnosisVariant.Name, mcdInterface);
    this.mcdVarcodingOwner = true;
  }

  internal VarcodeMcd(Channel channel, McdLogicalLink mcdLogicalLink)
  {
    this.channel = channel;
    this.mcdVarcoding = mcdLogicalLink;
    this.mcdVarcodingOwner = false;
  }

  private static bool IsSplittedParameterGroup(McdDiagComPrimitive diagComPrimitive)
  {
    return VarcodeMcd.IsSplittedParameterGroup(diagComPrimitive.SpecialData);
  }

  internal static bool IsSplittedParameterGroup(Dictionary<string, string> sdgs)
  {
    string str;
    return sdgs != null && sdgs.TryGetValue(".Splitted_Parameter_Group", out str) && str.Equals("yes", StringComparison.OrdinalIgnoreCase);
  }

  private static CaesarException GetSplittedParameterGroupResult(McdDiagComPrimitive job)
  {
    switch (Convert.ToUInt32(job.AllPositiveResponseParameters.FirstOrDefault<McdResponseParameter>((Func<McdResponseParameter, bool>) (r => r.Qualifier == "Status")).Value.CodedValue, (IFormatProvider) CultureInfo.InvariantCulture))
    {
      case 0:
        return (CaesarException) null;
      case 1:
        return new CaesarException(job.AllPositiveResponseParameters.FirstOrDefault<McdResponseParameter>((Func<McdResponseParameter, bool>) (r => r.Qualifier == "ErrorMessage")).Value.Value as string);
      case 2:
        return new CaesarException(job.AllPositiveResponseParameters.FirstOrDefault<McdResponseParameter>((Func<McdResponseParameter, bool>) (r => r.Qualifier == "NRC")).Value);
      default:
        return (CaesarException) null;
    }
  }

  internal override void DoCoding()
  {
    this.Exception = (CaesarException) null;
    foreach (McdDiagComPrimitive pendingPrimitive in this.pendingPrimitives)
    {
      try
      {
        if (VarcodeMcd.IsSplittedParameterGroup(pendingPrimitive))
        {
          McdDiagComPrimitive service = this.mcdVarcoding.GetService("DiagJob_" + pendingPrimitive.Qualifier);
          if (service != null)
          {
            if (service.SetInput("Data", (object) pendingPrimitive.RequestMessage.Skip<byte>(pendingPrimitive.PduActualDataStartPos).ToArray<byte>()))
            {
              service.Execute(0);
              CaesarException parameterGroupResult = VarcodeMcd.GetSplittedParameterGroupResult(service);
              if (parameterGroupResult != null)
                this.Exception = parameterGroupResult;
            }
          }
          else
            this.Exception = new CaesarException(SapiError.SplitGroupDiagjobNotFound);
        }
        else
        {
          pendingPrimitive.Execute(0);
          if (pendingPrimitive.IsNegativeResponse)
            this.Exception = new CaesarException(pendingPrimitive);
        }
      }
      catch (McdException ex)
      {
        this.Exception = new CaesarException(ex);
      }
    }
    this.pendingPrimitives.Clear();
  }

  internal override bool AllowSetDefaultString(string groupQualifier) => true;

  internal override void EnableReadCodingStringFromEcu(bool enableReadCodingStringFromEcu)
  {
  }

  internal override byte[] GetCurrentCodingString(string groupQualifier)
  {
    this.Exception = (CaesarException) null;
    Parameter itemFirstInGroup = this.channel.Parameters.GetItemFirstInGroup(groupQualifier);
    McdDiagComPrimitive service1 = this.mcdVarcoding.GetService(itemFirstInGroup.WriteService.McdQualifier);
    if (this.mcdVarcoding.State == this.mcdVarcoding.TargetState)
    {
      try
      {
        McdDiagComPrimitive service2 = this.mcdVarcoding.GetService(itemFirstInGroup.ReadService.McdQualifier);
        if (VarcodeMcd.IsSplittedParameterGroup(service2))
        {
          McdDiagComPrimitive service3 = this.mcdVarcoding.GetService("DiagJob_" + service2.Qualifier);
          if (service3 != null)
          {
            service3.Execute(0);
            CaesarException parameterGroupResult = VarcodeMcd.GetSplittedParameterGroupResult(service3);
            if (parameterGroupResult != null)
            {
              this.Exception = parameterGroupResult;
            }
            else
            {
              byte[] second = service3.AllPositiveResponseParameters.FirstOrDefault<McdResponseParameter>((Func<McdResponseParameter, bool>) (p => p.Qualifier == "Data")).Value.Value as byte[];
              service1.RequestMessage = ((IEnumerable<byte>) itemFirstInGroup.WritePrefix).Concat<byte>((IEnumerable<byte>) second);
            }
          }
          else
            this.Exception = new CaesarException(SapiError.SplitGroupDiagjobNotFound);
        }
        else
        {
          service2.Execute(0);
          if (service2.IsNegativeResponse)
          {
            this.Exception = new CaesarException(service2);
          }
          else
          {
            byte[] array = service2.ResponseMessage.Skip<byte>(itemFirstInGroup.WritePrefix.Length).ToArray<byte>();
            service1.RequestMessage = ((IEnumerable<byte>) itemFirstInGroup.WritePrefix).Concat<byte>((IEnumerable<byte>) array);
          }
        }
      }
      catch (McdException ex)
      {
        this.Exception = new CaesarException(ex);
      }
    }
    return this.Exception != null ? new byte[0] : service1.RequestMessage.Skip<byte>(itemFirstInGroup.WritePrefix.Length).ToArray<byte>();
  }

  internal override void SetCurrentCodingString(string groupQualifier, byte[] content)
  {
    this.Exception = (CaesarException) null;
    Parameter itemFirstInGroup = this.channel.Parameters.GetItemFirstInGroup(groupQualifier);
    McdDiagComPrimitive service = this.mcdVarcoding.GetService(itemFirstInGroup.WriteService.McdQualifier);
    service.RequestMessage = ((IEnumerable<byte>) itemFirstInGroup.WritePrefix).Concat<byte>((IEnumerable<byte>) content);
    this.AddPendingPrimitive(service);
  }

  internal override void SetDefaultStringByPartNumber(string partNumber)
  {
    this.Exception = (CaesarException) null;
    try
    {
      McdConfigurationRecord configRecord = this.mcdVarcoding.SetDefaultStringByPartNumber(partNumber);
      if (configRecord == null)
      {
        this.Exception = new CaesarException(SapiError.DefaultStringKeyNotFound, partNumber);
      }
      else
      {
        McdDiagComPrimitive diagComPrimitive = this.mcdVarcoding.UpdateDiagCodingString(configRecord);
        if (diagComPrimitive != null)
          this.AddPendingPrimitive(diagComPrimitive);
        else
          this.Exception = new CaesarException(SapiError.DiagComPrimitiveReferenceFromCodingFileNotFound, configRecord.DBRecord.Qualifier);
      }
    }
    catch (McdException ex)
    {
      this.Exception = new CaesarException(ex);
    }
  }

  internal override void SetDefaultStringByPartNumberAndPartVersion(
    string partNumber,
    uint partVersion)
  {
    this.SetDefaultStringByPartNumber(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_{1,2:000}", (object) partNumber, (object) partVersion));
  }

  internal override void SetFragmentMeaningByPartNumber(string partNumber)
  {
    this.Exception = (CaesarException) null;
    try
    {
      McdConfigurationRecord configRecord = this.mcdVarcoding.SetFragmentMeaningByPartNumber(partNumber);
      if (configRecord == null)
      {
        this.Exception = new CaesarException(SapiError.FragmentKeyNotFound, partNumber);
      }
      else
      {
        McdDiagComPrimitive diagComPrimitive = this.mcdVarcoding.UpdateDiagCodingString(configRecord);
        if (diagComPrimitive != null)
          this.AddPendingPrimitive(diagComPrimitive);
        else
          this.Exception = new CaesarException(SapiError.DiagComPrimitiveReferenceFromCodingFileNotFound, configRecord.DBRecord.Qualifier);
      }
    }
    catch (McdException ex)
    {
      this.Exception = new CaesarException(ex);
    }
  }

  internal override void SetFragmentMeaningByPartNumberAndPartVersion(
    string partNumber,
    uint partVersion)
  {
    this.SetFragmentMeaningByPartNumber(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_{1,2:000}", (object) partNumber, (object) partVersion));
  }

  internal override void SetFragmentValue(Parameter parameter, object value)
  {
    this.Exception = (CaesarException) null;
    ParamType paramType = parameter.ParamType;
    if (paramType != 15)
    {
      if (paramType == 18)
        value = (object) ((Choice) value).OriginalName;
    }
    else
      value = (object) ((Dump) value).Data.ToArray<byte>();
    McdDiagComPrimitive service = this.mcdVarcoding.GetService(parameter.WriteService.McdQualifier);
    try
    {
      if (!service.SetInput(parameter.McdQualifier, value))
        this.Exception = new CaesarException(SapiError.FragmentNotFound);
    }
    catch (McdException ex)
    {
      this.Exception = new CaesarException(ex);
    }
    catch (FormatException ex)
    {
      this.Exception = new CaesarException("Exception setting fragment value", (System.Exception) ex);
    }
    if (this.Exception != null)
      return;
    this.AddPendingPrimitive(service);
  }

  internal void AddPendingPrimitive(McdDiagComPrimitive diagComPrimitive)
  {
    if (this.mcdVarcoding.State != this.mcdVarcoding.TargetState || this.pendingPrimitives.Contains(diagComPrimitive))
      return;
    this.pendingPrimitives.Add(diagComPrimitive);
  }

  internal override object GetFragmentValue(Parameter parameter)
  {
    this.Exception = (CaesarException) null;
    McdDiagComPrimitive service = this.mcdVarcoding.GetService(parameter.WriteService.McdQualifier);
    try
    {
      McdValue input = service.GetInput(parameter.McdQualifier);
      if (input != null)
      {
        if (input.IsValueValid)
          return input.GetValue(parameter.Type, parameter.Choices);
        if (parameter.Choices == null || parameter.Choices.Count == 0)
        {
          ScaleEntry scaleEntry = parameter.Scales == null || !parameter.Scales.Any<ScaleEntry>() ? parameter.FactorOffsetScale : parameter.Scales.First<ScaleEntry>();
          if (scaleEntry != null)
            return Convert.ChangeType((object) scaleEntry.ToPhysicalValue(Convert.ToDecimal(input.CodedValue, (IFormatProvider) CultureInfo.InvariantCulture)), parameter.Type, (IFormatProvider) CultureInfo.InvariantCulture);
          this.Exception = new CaesarException(SapiError.NoMatchingIntervalWasFound);
        }
        else
          this.Exception = new CaesarException(SapiError.NoMatchingChoiceValueInVarcodingString);
      }
    }
    catch (McdException ex)
    {
      this.Exception = new CaesarException(ex);
    }
    return (object) null;
  }

  internal bool IsOwner(McdLogicalLink link) => this.mcdVarcodingOwner && this.mcdVarcoding == link;

  protected override void Dispose(bool disposing)
  {
    if (this.disposedValue)
      return;
    if (disposing && this.mcdVarcoding != null)
    {
      if (this.mcdVarcodingOwner)
        this.mcdVarcoding.Dispose();
      this.mcdVarcoding = (McdLogicalLink) null;
    }
    this.disposedValue = true;
  }
}
