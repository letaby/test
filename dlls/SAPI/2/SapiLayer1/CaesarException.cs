using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.Serialization;
using System.Security.Permissions;
using CaesarAbstraction;
using McdAbstraction;

namespace SapiLayer1;

[Serializable]
public sealed class CaesarException : Exception
{
	private long errNo;

	private uint negativeResponseCode;

	private string message;

	public override string Message => message;

	public long ErrorNumber => errNo;

	public int NegativeResponseCode => (int)negativeResponseCode;

	[CLSCompliant(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("ErrNo is deprecated due to non-CLS compliance, please use ErrorNumber instead.")]
	public ulong ErrNo => (ulong)errNo;

	public static string GetErrorText(SapiError error)
	{
		Type typeFromHandle = typeof(SapiError);
		return ((DescriptionAttribute)typeFromHandle.GetField(Enum.GetName(typeFromHandle, error)).GetCustomAttributes(typeof(DescriptionAttribute), inherit: false)[0]).Description;
	}

	internal CaesarException(SapiError error)
	{
		errNo = (long)error;
		message = GetErrorText(error);
	}

	internal CaesarException(SapiError error, string additionalInformation)
	{
		errNo = (long)error;
		message = GetErrorText(error) + " " + additionalInformation;
	}

	internal CaesarException(McdException mcdError)
	{
		Initialise(mcdError);
	}

	internal CaesarException(CaesarErrorException caesarError, byte? negativeResponseCode = null, DiagnosisProtocol protocol = null)
	{
		Initialise(caesarError);
		if (negativeResponseCode.HasValue)
		{
			this.negativeResponseCode = negativeResponseCode.Value;
			message = string.Format(CultureInfo.InvariantCulture, "{0} - {1} (NRC${2:X})", message, (protocol != null) ? protocol.GetNegativeResponseCodeDescription(negativeResponseCode.Value) : string.Empty, this.negativeResponseCode);
		}
	}

	internal CaesarException(CaesarChannel channel)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected O, but got Unknown
		Initialise(new CaesarErrorException(channel));
		Channel item = Sapi.GetSapi().Channels.GetItem(channel);
		if (item != null && !channel.ChannelRunning)
		{
			item.DisconnectionException = this;
		}
	}

	internal CaesarException(CaesarDiagServiceIO diagServiceIO)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected O, but got Unknown
		Initialise(new CaesarErrorException(diagServiceIO));
		if (negativeResponseCode != 0)
		{
			Presentation presentation = new Presentation(0);
			message = string.Format(CultureInfo.InvariantCulture, "{0}(NRC${1:X})", presentation.GetPresentation(diagServiceIO).ToString(), negativeResponseCode);
		}
	}

	internal CaesarException(McdDiagComPrimitive diagServiceIO)
		: this(diagServiceIO.NegativeResponseParameter.Value)
	{
	}

	internal CaesarException(McdValue negativeResponseInfo)
	{
		errNo = 6701L;
		negativeResponseCode = Convert.ToUInt32(negativeResponseInfo.CodedValue, CultureInfo.InvariantCulture);
		if (negativeResponseCode != 0)
		{
			message = string.Format(CultureInfo.InvariantCulture, "{0}(NRC${1:X})", negativeResponseInfo.Value, negativeResponseCode);
		}
	}

	private void Initialise(CaesarErrorException caesarError)
	{
		errNo = caesarError.ErrNo;
		message = ((Exception)(object)caesarError).Message;
		negativeResponseCode = caesarError.NegativeResponseCode;
	}

	private void Initialise(McdException mcdError)
	{
		errNo = mcdError.ErrorCodeNumber;
		message = mcdError.ErrorCodeString + ": " + mcdError.Error + " (" + mcdError.VendorErrorCodeString + ": " + mcdError.VendorError + ")";
		negativeResponseCode = 0u;
	}

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
		if (info != null)
		{
			message = info.GetString("CaesarMessage");
			errNo = info.GetInt64("CaesarErrNo");
		}
	}

	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		if (info != null)
		{
			info.AddValue("CaesarMessage", message);
			info.AddValue("CaesarErrNo", errNo);
		}
		base.GetObjectData(info, context);
	}
}
