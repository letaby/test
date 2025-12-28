using System;
using System.Text;

namespace Softing.Dts;

internal class DtsParameterizationExceptionImpl : DtsParameterizationException
{
	protected IntPtr m_dtsHandle = IntPtr.Zero;

	public override string Message => ToString();

	public IntPtr Handle
	{
		get
		{
			return m_dtsHandle;
		}
		set
		{
			m_dtsHandle = value;
		}
	}

	public override MCDError Error
	{
		get
		{
			GC.KeepAlive(this);
			ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsException_getError(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsError;
		}
	}

	public override string SourceFile
	{
		get
		{
			GC.KeepAlive(this);
			String_Struct returnValue = default(String_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsException_getSourceFile(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue.makeString();
		}
	}

	public override uint SourceLine
	{
		get
		{
			GC.KeepAlive(this);
			uint returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsException_getSourceLine(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue;
		}
	}

	public override MCDObjectType ObjectType
	{
		get
		{
			GC.KeepAlive(this);
			MCDObjectType returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsObject_getObjectType(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue;
		}
	}

	public DtsParameterizationExceptionImpl(IntPtr handle)
	{
		Handle = handle;
		DTS_ObjectMapper.registerObject(Handle, this);
	}

	protected override void Dispose(bool disposing)
	{
		if (Handle != IntPtr.Zero)
		{
			Handle = IntPtr.Zero;
		}
	}

	~DtsParameterizationExceptionImpl()
	{
		Dispose(disposing: false);
	}

	public override string ToString()
	{
		MCDError error = Error;
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine(error.CodeDescription.ToString() + " (code = " + (int)error.Code + ")");
		stringBuilder.Append("Severity = ");
		stringBuilder.AppendLine(error.Severity.ToString());
		stringBuilder.Append("VendorCodeDescription = ");
		stringBuilder.AppendLine(error.VendorCodeDescription + " (code = " + (int)error.VendorCode + ")");
		stringBuilder.AppendLine("");
		stringBuilder.Append("SourceFile = ");
		stringBuilder.AppendLine(SourceFile + "(" + SourceLine + ")");
		return stringBuilder.ToString();
	}
}
