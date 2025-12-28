using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Softing.Dts;

namespace McdAbstraction;

[Serializable]
public class McdException : Exception
{
	public string Error { get; private set; }

	public string ErrorCodeString { get; private set; }

	public int ErrorCodeNumber { get; private set; }

	public string VendorError { get; private set; }

	public string VendorErrorCodeString { get; private set; }

	public int VendorErrorCodeNumber { get; private set; }

	public string Function { get; private set; }

	internal McdException(MCDException ex, string function)
		: this(ex.Error, function)
	{
	}

	internal McdException(MCDError error, string function)
		: base(error.VendorCodeDescription)
	{
		Error = error.CodeDescription;
		try
		{
			ErrorCodeString = McdRoot.Dts.EnumValue.GetStringFromEnum((int)error.Code);
		}
		catch (MCDException)
		{
			ErrorCodeString = error.Code.ToString();
		}
		ErrorCodeNumber = (int)error.Code;
		VendorError = error.VendorCodeDescription;
		VendorErrorCodeString = string.Format(CultureInfo.InvariantCulture, "0x{0:X}", error.VendorCode);
		VendorErrorCodeNumber = error.VendorCode;
		Function = function;
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
		if (info != null)
		{
			Error = info.GetString("Error");
			ErrorCodeString = info.GetString("ErrorCodeString");
			ErrorCodeNumber = info.GetInt32("ErrorCodeNumber");
			VendorError = info.GetString("VendorError");
			VendorErrorCodeString = info.GetString("VendorErrorCodeString");
			VendorErrorCodeNumber = info.GetInt32("VendorErrorCodeNumber");
			Function = info.GetString("Function");
		}
	}

	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		if (info != null)
		{
			info.AddValue("Error", Error);
			info.AddValue("ErrorCodeString", ErrorCodeString);
			info.AddValue("ErrorCodeNumber", ErrorCodeNumber);
			info.AddValue("VendorError", VendorError);
			info.AddValue("VendorErrorCodeString", VendorErrorCodeString);
			info.AddValue("VendorErrorCodeNumber", VendorErrorCodeNumber);
			info.AddValue("Function", Function);
		}
		base.GetObjectData(info, context);
	}
}
