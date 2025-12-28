// Decompiled with JetBrains decompiler
// Type: McdAbstraction.McdRequestParameter
// Assembly: McdAbstraction, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 2CF84A4E-9C9E-4158-9C67-2CE39889DD31
// Assembly location: C:\Users\petra\Downloads\Архив (2)\McdAbstraction.dll

using Softing.Dts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

#nullable disable
namespace McdAbstraction;

public class McdRequestParameter : IDisposable
{
  private MCDRequestParameter requestParameter;
  private McdRequestParameter parent;
  private List<McdRequestParameter> parameters;
  private bool disposedValue = false;

  internal McdRequestParameter(MCDRequestParameter requestParameter, McdRequestParameter parent = null)
  {
    this.requestParameter = requestParameter;
    this.Qualifier = this.requestParameter.ShortName;
    this.Name = this.requestParameter.LongName;
    this.parent = parent;
  }

  public string Qualifier { get; private set; }

  public string Name { get; private set; }

  internal void SetValue(object newValue)
  {
    try
    {
      MCDValue mcdValue = this.requestParameter.CreateValue();
      switch (mcdValue.DataType)
      {
        case MCDDataType.eA_BYTEFIELD:
          if (newValue is byte[] numArray)
          {
            mcdValue.Bytefield = numArray;
            break;
          }
          mcdValue.ValueAsString = newValue.ToString();
          break;
        case MCDDataType.eA_FLOAT32:
          mcdValue.Float32 = (float) Convert.ChangeType(newValue, typeof (float), (IFormatProvider) CultureInfo.InvariantCulture);
          break;
        case MCDDataType.eA_FLOAT64:
          mcdValue.Float64 = (double) Convert.ChangeType(newValue, typeof (double), (IFormatProvider) CultureInfo.InvariantCulture);
          break;
        case MCDDataType.eA_INT16:
          mcdValue.Int16 = (short) Convert.ChangeType(newValue, typeof (short), (IFormatProvider) CultureInfo.InvariantCulture);
          break;
        case MCDDataType.eA_INT32:
          mcdValue.Int32 = (int) Convert.ChangeType(newValue, typeof (int), (IFormatProvider) CultureInfo.InvariantCulture);
          break;
        case MCDDataType.eA_INT64:
          mcdValue.Int64 = (long) Convert.ChangeType(newValue, typeof (long), (IFormatProvider) CultureInfo.InvariantCulture);
          break;
        case MCDDataType.eA_INT8:
          mcdValue.Int8 = (char) Convert.ChangeType(newValue, typeof (char), (IFormatProvider) CultureInfo.InvariantCulture);
          break;
        case MCDDataType.eA_UINT16:
          mcdValue.Uint16 = (ushort) Convert.ChangeType(newValue, typeof (ushort), (IFormatProvider) CultureInfo.InvariantCulture);
          break;
        case MCDDataType.eA_UINT32:
          mcdValue.Uint32 = (uint) Convert.ChangeType(newValue, typeof (uint), (IFormatProvider) CultureInfo.InvariantCulture);
          break;
        case MCDDataType.eA_UINT64:
          mcdValue.Uint64 = (ulong) Convert.ChangeType(newValue, typeof (ulong), (IFormatProvider) CultureInfo.InvariantCulture);
          break;
        case MCDDataType.eA_UINT8:
          mcdValue.Uint8 = (byte) Convert.ChangeType(newValue, typeof (byte), (IFormatProvider) CultureInfo.InvariantCulture);
          break;
        default:
          mcdValue.ValueAsString = newValue.ToString();
          break;
      }
      this.requestParameter.Value = mcdValue;
    }
    catch (MCDException ex)
    {
      throw new McdException(ex, "SetInput");
    }
  }

  internal McdValue Value
  {
    get
    {
      try
      {
        MCDValue mcdValue = (MCDValue) null;
        if (this.requestParameter.ValueRangeInfo == MCDRangeInfo.eVALUE_VALID)
        {
          try
          {
            mcdValue = this.requestParameter.Value;
          }
          catch (MCDException ex)
          {
          }
        }
        return new McdValue(mcdValue, this.requestParameter.CodedValue);
      }
      catch (MCDException ex)
      {
        throw new McdException(ex, "McdRequestParameter.Value");
      }
    }
  }

  public IEnumerable<McdRequestParameter> Parameters
  {
    get
    {
      if (this.parameters == null)
        this.parameters = this.requestParameter.Parameters.OfType<MCDRequestParameter>().Select<MCDRequestParameter, McdRequestParameter>((Func<MCDRequestParameter, McdRequestParameter>) (p => new McdRequestParameter(p, this))).ToList<McdRequestParameter>();
      return (IEnumerable<McdRequestParameter>) this.parameters;
    }
  }

  public McdRequestParameter Parent => this.parent;

  protected virtual void Dispose(bool disposing)
  {
    if (this.disposedValue)
      return;
    if (disposing)
    {
      if (this.parameters != null)
      {
        foreach (McdRequestParameter parameter in this.parameters)
          parameter.Dispose();
        this.parameters.Clear();
        this.parameters = (List<McdRequestParameter>) null;
      }
      if (this.requestParameter != null)
      {
        this.requestParameter.Dispose();
        this.requestParameter = (MCDRequestParameter) null;
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
