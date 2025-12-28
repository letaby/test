// Decompiled with JetBrains decompiler
// Type: McdAbstraction.McdResponseParameter
// Assembly: McdAbstraction, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 2CF84A4E-9C9E-4158-9C67-2CE39889DD31
// Assembly location: C:\Users\petra\Downloads\Архив (2)\McdAbstraction.dll

using Softing.Dts;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace McdAbstraction;

public class McdResponseParameter : IDisposable, IMcdDataItem
{
  private MCDResponseParameter responseParameter;
  private McdResponseParameter parent;
  private List<McdResponseParameter> parameters;
  private bool disposedValue = false;

  internal McdResponseParameter(MCDResponseParameter responseParameter, McdResponseParameter parent = null)
  {
    this.responseParameter = responseParameter;
    this.Qualifier = this.responseParameter.ShortName;
    this.Name = this.responseParameter.LongName;
    this.parent = parent;
    this.QualifierPath = this.parent != null ? $"{this.parent.QualifierPath}_{this.Qualifier}" : this.Qualifier;
  }

  public string QualifierPath { get; private set; }

  public string Qualifier { get; private set; }

  public string Name { get; private set; }

  public bool IsDiagnosticTroubleCode => this.responseParameter.DataType == MCDDataType.eDTC;

  public bool IsEnvironmentalData => this.responseParameter.DataType == MCDDataType.eENVDATA;

  public McdDBDiagTroubleCode DBDiagTroubleCode
  {
    get
    {
      try
      {
        return this.IsDiagnosticTroubleCode ? new McdDBDiagTroubleCode(this.responseParameter.DbDTC) : (McdDBDiagTroubleCode) null;
      }
      catch (DtsSystemException ex)
      {
        return (McdDBDiagTroubleCode) null;
      }
    }
  }

  public McdValue Value
  {
    get
    {
      MCDValue codedValue = this.responseParameter.DataType == MCDDataType.eA_BYTEFIELD || this.responseParameter.DataType == MCDDataType.eA_ASCIISTRING || this.responseParameter.DataType == MCDDataType.eA_UNICODE2STRING ? (MCDValue) null : this.responseParameter.CodedValue;
      try
      {
        switch (this.responseParameter.ValueRangeInfo)
        {
          case MCDRangeInfo.eVALUE_VALID:
            return new McdValue(this.responseParameter.Value, codedValue);
          case MCDRangeInfo.eVALUE_NOT_DEFINED:
            return new McdValue(codedValue);
          case MCDRangeInfo.eVALUE_NOT_AVAILABLE:
          case MCDRangeInfo.eVALUE_NOT_VALID:
            try
            {
              return new McdValue(this.responseParameter.InternalScaleConstraint, codedValue);
            }
            catch (DtsParameterizationException ex)
            {
              return new McdValue((MCDValue) null, codedValue);
            }
            catch (DtsDatabaseException ex)
            {
              return new McdValue((MCDValue) null, codedValue);
            }
          default:
            return new McdValue((MCDValue) null, codedValue);
        }
      }
      catch (DtsDatabaseException ex)
      {
        if (ex.Error.VendorCode == (ushort) 57661)
          return (McdValue) null;
        throw;
      }
    }
  }

  public bool IsValueValid => this.responseParameter.ValueRangeInfo == MCDRangeInfo.eVALUE_VALID;

  public IEnumerable<McdResponseParameter> Parameters
  {
    get
    {
      if (this.parameters == null)
        this.parameters = this.responseParameter.Parameters.OfType<MCDResponseParameter>().Select<MCDResponseParameter, McdResponseParameter>((Func<MCDResponseParameter, McdResponseParameter>) (p => new McdResponseParameter(p, this))).ToList<McdResponseParameter>();
      return (IEnumerable<McdResponseParameter>) this.parameters;
    }
  }

  public IEnumerable<McdResponseParameter> AllParameters
  {
    get
    {
      return McdRoot.FlattenStructures<McdResponseParameter>(this.Parameters, (Func<McdResponseParameter, IEnumerable<McdResponseParameter>>) (p => p.Parameters));
    }
  }

  public McdResponseParameter Parent => this.parent;

  IEnumerable<IMcdDataItem> IMcdDataItem.Parameters => (IEnumerable<IMcdDataItem>) this.Parameters;

  IMcdDataItem IMcdDataItem.Parent => (IMcdDataItem) this.Parent;

  protected virtual void Dispose(bool disposing)
  {
    if (this.disposedValue)
      return;
    if (disposing)
    {
      if (this.parameters != null)
      {
        foreach (McdResponseParameter parameter in this.parameters)
          parameter.Dispose();
        this.parameters.Clear();
        this.parameters = (List<McdResponseParameter>) null;
      }
      if (this.responseParameter != null)
      {
        this.responseParameter.Dispose();
        this.responseParameter = (MCDResponseParameter) null;
      }
    }
    this.disposedValue = true;
  }

  public void Dispose()
  {
    this.Dispose(true);
    GC.SuppressFinalize((object) this);
  }
}
