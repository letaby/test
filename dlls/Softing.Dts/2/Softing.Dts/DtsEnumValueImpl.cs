using System;

namespace Softing.Dts;

internal class DtsEnumValueImpl : MappedObject, DtsEnumValue, MCDEnumValue, MCDObject, IDisposable, DtsObject
{
	protected IntPtr m_dtsHandle = IntPtr.Zero;

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

	public MCDObjectType ObjectType
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

	public uint ObjectID
	{
		get
		{
			GC.KeepAlive(this);
			uint returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsObject_getObjectID(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue;
		}
	}

	public DtsEnumValueImpl(IntPtr handle)
	{
		Handle = handle;
		DTS_ObjectMapper.registerObject(Handle, this);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (Handle != IntPtr.Zero)
		{
			DTS_ObjectMapper.unregisterObject(Handle);
			Handle = IntPtr.Zero;
		}
	}

	~DtsEnumValueImpl()
	{
		Dispose(disposing: false);
	}

	public int GetEnumFromString(string enumString)
	{
		GC.KeepAlive(this);
		int returnValue;
		IntPtr intPtr = CSWrap.CSNIDTS_DtsEnumValue_getEnumFromString(Handle, enumString, out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return returnValue;
	}

	public string GetStringFromEnum(int enumValue)
	{
		GC.KeepAlive(this);
		String_Struct returnValue = default(String_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsEnumValue_getStringFromEnum(Handle, enumValue, out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return returnValue.makeString();
	}
}
