using System;

namespace Softing.Dts;

internal class DtsAudienceImpl : MappedObject, DtsAudience, MCDAudience, MCDObject, IDisposable, DtsObject
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

	public bool IsSupplier
	{
		get
		{
			GC.KeepAlive(this);
			bool returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsAudience_isSupplier(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue;
		}
	}

	public bool IsDevelopment
	{
		get
		{
			GC.KeepAlive(this);
			bool returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsAudience_isDevelopment(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue;
		}
	}

	public bool IsManufacturing
	{
		get
		{
			GC.KeepAlive(this);
			bool returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsAudience_isManufacturing(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue;
		}
	}

	public bool IsAfterSales
	{
		get
		{
			GC.KeepAlive(this);
			bool returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsAudience_isAfterSales(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue;
		}
	}

	public bool IsAfterMarket
	{
		get
		{
			GC.KeepAlive(this);
			bool returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsAudience_isAfterMarket(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue;
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

	public DtsAudienceImpl(IntPtr handle)
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

	~DtsAudienceImpl()
	{
		Dispose(disposing: false);
	}
}
