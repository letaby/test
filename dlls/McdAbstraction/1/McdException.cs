// Decompiled with JetBrains decompiler
// Type: McdAbstraction.McdException
// Assembly: McdAbstraction, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 2CF84A4E-9C9E-4158-9C67-2CE39889DD31
// Assembly location: C:\Users\petra\Downloads\Архив (2)\McdAbstraction.dll

using Softing.Dts;
using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Security.Permissions;

#nullable disable
namespace McdAbstraction;

[Serializable]
public class McdException : Exception
{
  internal McdException(MCDException ex, string function)
    : this(ex.Error, function)
  {
  }

  internal McdException(MCDError error, string function)
    : base(error.VendorCodeDescription)
  {
    this.Error = error.CodeDescription;
    try
    {
      this.ErrorCodeString = McdRoot.Dts.EnumValue.GetStringFromEnum((int) error.Code);
    }
    catch (MCDException ex)
    {
      this.ErrorCodeString = error.Code.ToString();
    }
    this.ErrorCodeNumber = (int) error.Code;
    this.VendorError = error.VendorCodeDescription;
    this.VendorErrorCodeString = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "0x{0:X}", (object) error.VendorCode);
    this.VendorErrorCodeNumber = (int) error.VendorCode;
    this.Function = function;
  }

  public McdException()
  {
  }

  public McdException(string message)
    : base(message)
  {
  }

  public McdException(string message, Exception exception)
    : base(message, exception)
  {
  }

  protected McdException(SerializationInfo info, StreamingContext context)
    : base(info, context)
  {
    if (info == null)
      return;
    this.Error = info.GetString(nameof (Error));
    this.ErrorCodeString = info.GetString(nameof (ErrorCodeString));
    this.ErrorCodeNumber = info.GetInt32(nameof (ErrorCodeNumber));
    this.VendorError = info.GetString(nameof (VendorError));
    this.VendorErrorCodeString = info.GetString(nameof (VendorErrorCodeString));
    this.VendorErrorCodeNumber = info.GetInt32(nameof (VendorErrorCodeNumber));
    this.Function = info.GetString(nameof (Function));
  }

  [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
  public override void GetObjectData(SerializationInfo info, StreamingContext context)
  {
    if (info != null)
    {
      info.AddValue("Error", (object) this.Error);
      info.AddValue("ErrorCodeString", (object) this.ErrorCodeString);
      info.AddValue("ErrorCodeNumber", this.ErrorCodeNumber);
      info.AddValue("VendorError", (object) this.VendorError);
      info.AddValue("VendorErrorCodeString", (object) this.VendorErrorCodeString);
      info.AddValue("VendorErrorCodeNumber", this.VendorErrorCodeNumber);
      info.AddValue("Function", (object) this.Function);
    }
    base.GetObjectData(info, context);
  }

  public string Error { get; private set; }

  public string ErrorCodeString { get; private set; }

  public int ErrorCodeNumber { get; private set; }

  public string VendorError { get; private set; }

  public string VendorErrorCodeString { get; private set; }

  public int VendorErrorCodeNumber { get; private set; }

  public string Function { get; private set; }
}
