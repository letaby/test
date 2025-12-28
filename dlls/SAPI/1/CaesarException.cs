// Decompiled with JetBrains decompiler
// Type: SapiLayer1.CaesarException
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using CaesarAbstraction;
using McdAbstraction;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.Serialization;
using System.Security.Permissions;

#nullable disable
namespace SapiLayer1;

[Serializable]
public sealed class CaesarException : Exception
{
  private long errNo;
  private uint negativeResponseCode;
  private string message;

  public static string GetErrorText(SapiError error)
  {
    Type enumType = typeof (SapiError);
    return ((DescriptionAttribute) enumType.GetField(Enum.GetName(enumType, (object) error)).GetCustomAttributes(typeof (DescriptionAttribute), false)[0]).Description;
  }

  internal CaesarException(SapiError error)
  {
    this.errNo = (long) error;
    this.message = CaesarException.GetErrorText(error);
  }

  internal CaesarException(SapiError error, string additionalInformation)
  {
    this.errNo = (long) error;
    this.message = $"{CaesarException.GetErrorText(error)} {additionalInformation}";
  }

  internal CaesarException(McdException mcdError) => this.Initialise(mcdError);

  internal CaesarException(
    CaesarErrorException caesarError,
    byte? negativeResponseCode = null,
    DiagnosisProtocol protocol = null)
  {
    this.Initialise(caesarError);
    if (!negativeResponseCode.HasValue)
      return;
    this.negativeResponseCode = (uint) negativeResponseCode.Value;
    this.message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} - {1} (NRC${2:X})", (object) this.message, protocol != null ? (object) protocol.GetNegativeResponseCodeDescription(negativeResponseCode.Value) : (object) string.Empty, (object) this.negativeResponseCode);
  }

  internal CaesarException(CaesarChannel channel)
  {
    this.Initialise(new CaesarErrorException(channel));
    Channel channel1 = Sapi.GetSapi().Channels.GetItem(channel);
    if (channel1 == null || channel.ChannelRunning)
      return;
    channel1.DisconnectionException = this;
  }

  internal CaesarException(CaesarDiagServiceIO diagServiceIO)
  {
    this.Initialise(new CaesarErrorException(diagServiceIO));
    if (this.negativeResponseCode <= 0U)
      return;
    this.message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}(NRC${1:X})", (object) new Presentation((ushort) 0).GetPresentation(diagServiceIO).ToString(), (object) this.negativeResponseCode);
  }

  internal CaesarException(McdDiagComPrimitive diagServiceIO)
    : this(diagServiceIO.NegativeResponseParameter.Value)
  {
  }

  internal CaesarException(McdValue negativeResponseInfo)
  {
    this.errNo = 6701L;
    this.negativeResponseCode = Convert.ToUInt32(negativeResponseInfo.CodedValue, (IFormatProvider) CultureInfo.InvariantCulture);
    if (this.negativeResponseCode <= 0U)
      return;
    this.message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}(NRC${1:X})", negativeResponseInfo.Value, (object) this.negativeResponseCode);
  }

  private void Initialise(CaesarErrorException caesarError)
  {
    this.errNo = caesarError.ErrNo;
    this.message = ((Exception) caesarError).Message;
    this.negativeResponseCode = caesarError.NegativeResponseCode;
  }

  private void Initialise(McdException mcdError)
  {
    this.errNo = (long) mcdError.ErrorCodeNumber;
    this.message = $"{mcdError.ErrorCodeString}: {mcdError.Error} ({mcdError.VendorErrorCodeString}: {mcdError.VendorError})";
    this.negativeResponseCode = 0U;
  }

  public override string Message => this.message;

  public long ErrorNumber => this.errNo;

  public int NegativeResponseCode => (int) this.negativeResponseCode;

  public CaesarException()
  {
  }

  public CaesarException(string message)
    : base(message)
  {
    this.message = message;
  }

  public CaesarException(string message, Exception innerexception)
    : base(message, innerexception)
  {
    this.message = message;
  }

  private CaesarException(SerializationInfo info, StreamingContext context)
    : base(info, context)
  {
    if (info == null)
      return;
    this.message = info.GetString("CaesarMessage");
    this.errNo = info.GetInt64("CaesarErrNo");
  }

  [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
  public override void GetObjectData(SerializationInfo info, StreamingContext context)
  {
    if (info != null)
    {
      info.AddValue("CaesarMessage", (object) this.message);
      info.AddValue("CaesarErrNo", this.errNo);
    }
    base.GetObjectData(info, context);
  }

  [CLSCompliant(false)]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Obsolete("ErrNo is deprecated due to non-CLS compliance, please use ErrorNumber instead.")]
  public ulong ErrNo => (ulong) this.errNo;
}
