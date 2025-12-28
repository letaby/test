// Decompiled with JetBrains decompiler
// Type: McdAbstraction.McdValue
// Assembly: McdAbstraction, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 2CF84A4E-9C9E-4158-9C67-2CE39889DD31
// Assembly location: C:\Users\petra\Downloads\Архив (2)\McdAbstraction.dll

using Softing.Dts;
using System;
using System.Globalization;

#nullable disable
namespace McdAbstraction;

public class McdValue
{
  internal McdValue(MCDValue mcdValue, MCDValue mcdCodedValue = null)
  {
    this.IsValueValid = mcdValue != null;
    if (mcdValue != null)
      this.Value = McdValue.GetValue(mcdValue);
    else if (mcdCodedValue != null)
      this.Value = (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Invalid value (${0:X})", McdValue.GetValue(mcdCodedValue));
    if (mcdCodedValue == null)
      return;
    this.CodedValue = McdValue.GetValue(mcdCodedValue);
  }

  internal McdValue(MCDScaleConstraint constraint, MCDValue mcdCodedValue)
  {
    string shortLabel = constraint.ShortLabel;
    this.IsValueValid = !string.IsNullOrEmpty(shortLabel);
    if (!string.IsNullOrEmpty(shortLabel))
      this.Value = (object) shortLabel;
    else if (mcdCodedValue != null)
      this.Value = (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Invalid value (${0:X})", McdValue.GetValue(mcdCodedValue));
    if (mcdCodedValue == null)
      return;
    this.CodedValue = McdValue.GetValue(mcdCodedValue);
  }

  private static object GetValue(MCDValue mcdValue)
  {
    object obj;
    switch (mcdValue.DataType)
    {
      case MCDDataType.eA_ASCIISTRING:
        obj = (object) mcdValue.Asciistring;
        break;
      case MCDDataType.eA_BYTEFIELD:
        obj = (object) mcdValue.Bytefield;
        break;
      case MCDDataType.eA_FLOAT32:
        obj = (object) mcdValue.Float32;
        break;
      case MCDDataType.eA_FLOAT64:
        obj = (object) mcdValue.Float64;
        break;
      case MCDDataType.eA_INT16:
        obj = (object) mcdValue.Int16;
        break;
      case MCDDataType.eA_INT32:
        obj = (object) mcdValue.Int32;
        break;
      case MCDDataType.eA_INT64:
        obj = (object) mcdValue.Int64;
        break;
      case MCDDataType.eA_INT8:
        obj = (object) mcdValue.Int8;
        break;
      case MCDDataType.eA_UINT16:
        obj = (object) mcdValue.Uint16;
        break;
      case MCDDataType.eA_UINT32:
        obj = (object) mcdValue.Uint32;
        break;
      case MCDDataType.eA_UINT64:
        obj = (object) mcdValue.Uint64;
        break;
      case MCDDataType.eA_UINT8:
        obj = (object) mcdValue.Uint8;
        break;
      case MCDDataType.eA_UNICODE2STRING:
        obj = (object) mcdValue.Unicode2string;
        break;
      default:
        obj = (object) mcdValue.ValueAsString;
        break;
    }
    return obj;
  }

  public object Value { get; private set; }

  public object CodedValue { get; private set; }

  public bool IsValueValid { get; private set; }
}
