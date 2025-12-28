using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

internal class DtsIdentifierInfosImpl : MappedObject, DtsIdentifierInfos, DtsCollection, MCDCollection, MCDObject, IDisposable, IEnumerable, DtsObject
{
	private class DtsIdentifierInfosEnumerator : IEnumerator
	{
		private DtsIdentifierInfosImpl m_classList;

		private int m_index;

		public object Current => m_classList.GetItemByIndex((uint)m_index);

		public DtsIdentifierInfosEnumerator(DtsIdentifierInfosImpl classList)
		{
			m_classList = classList;
			m_index = -1;
		}

		public void Reset()
		{
			m_index = -1;
		}

		public bool MoveNext()
		{
			m_index++;
			if (m_index >= m_classList.Count)
			{
				return false;
			}
			return true;
		}
	}

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

	public uint Count
	{
		get
		{
			GC.KeepAlive(this);
			uint returnValue;
			IntPtr intPtr = CSWrap.CSNIDTS_DtsCollection_getCount(Handle, out returnValue);
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

	public DtsIdentifierInfosImpl(IntPtr handle)
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

	~DtsIdentifierInfosImpl()
	{
		Dispose(disposing: false);
	}

	public DtsIdentifierInfo GetItemByIndex(uint index)
	{
		GC.KeepAlive(this);
		ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsIdentifierInfos_getItemByIndex(Handle, index, out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsIdentifierInfo;
	}

	public void LoadDatabase(string fileName)
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsIdentifierInfos_loadDatabase(Handle, fileName);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void UnloadAll()
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsIdentifierInfos_unloadAll(Handle);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void UnloadDatabase(string fileName)
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsIdentifierInfos_unloadDatabase(Handle, fileName);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public DtsIdentifierInfo GetItemByIdentifier(uint identifier, bool extended, int extendedAddress)
	{
		GC.KeepAlive(this);
		ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsIdentifierInfos_getItemByIdentifier(Handle, identifier, extended, extendedAddress, out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsIdentifierInfo;
	}

	public MCDObject GetObjectItemByIndex(uint index)
	{
		GC.KeepAlive(this);
		ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsCollection_getObjectItemByIndex(Handle, index, out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsObject;
	}

	public List<DtsIdentifierInfo> ToList()
	{
		List<DtsIdentifierInfo> list = new List<DtsIdentifierInfo>();
		for (uint num = 0u; num < Count; num++)
		{
			list.Add(GetItemByIndex(num));
		}
		return list;
	}

	public DtsIdentifierInfo[] ToArray()
	{
		DtsIdentifierInfo[] array = new DtsIdentifierInfo[Count];
		for (uint num = 0u; num < Count; num++)
		{
			array[num] = GetItemByIndex(num);
		}
		return array;
	}

	public IEnumerator GetEnumerator()
	{
		return new DtsIdentifierInfosEnumerator(this);
	}
}
