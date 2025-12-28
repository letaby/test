using System;

namespace Softing.Dts;

internal class DtsFlashSegmentIteratorImpl : MappedObject, DtsFlashSegmentIterator, MCDFlashSegmentIterator, MCDObject, IDisposable, DtsObject
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

	public uint BinaryDataChunkSize
	{
		get
		{
			GC.KeepAlive(this);
			uint returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsFlashSegmentIterator_getBinaryDataChunkSize(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue;
		}
	}

	public byte[] FirstBinaryDataChunk
	{
		get
		{
			GC.KeepAlive(this);
			ByteField_Struct returnValue = default(ByteField_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsFlashSegmentIterator_getFirstBinaryDataChunk(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue.ToByteArray();
		}
	}

	public byte[] NextBinaryDataChunk
	{
		get
		{
			GC.KeepAlive(this);
			ByteField_Struct returnValue = default(ByteField_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsFlashSegmentIterator_getNextBinaryDataChunk(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue.ToByteArray();
		}
	}

	public bool HasNextBinaryDataChunk
	{
		get
		{
			GC.KeepAlive(this);
			bool returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsFlashSegmentIterator_hasNextBinaryDataChunk(Handle, out returnValue);
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

	public DtsFlashSegmentIteratorImpl(IntPtr handle)
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

	~DtsFlashSegmentIteratorImpl()
	{
		Dispose(disposing: false);
	}
}
