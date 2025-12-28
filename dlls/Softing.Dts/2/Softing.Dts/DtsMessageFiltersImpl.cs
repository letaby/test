using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

internal class DtsMessageFiltersImpl : MappedObject, DtsMessageFilters, MCDMessageFilters, MCDCollection, MCDObject, IDisposable, IEnumerable, DtsCollection, DtsObject
{
	private class DtsMessageFiltersEnumerator : IEnumerator
	{
		private DtsMessageFiltersImpl m_classList;

		private int m_index;

		public object Current => m_classList.GetItemByIndex((uint)m_index);

		public DtsMessageFiltersEnumerator(DtsMessageFiltersImpl classList)
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

	public DtsMessageFiltersImpl(IntPtr handle)
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

	~DtsMessageFiltersImpl()
	{
		Dispose(disposing: false);
	}

	public MCDMessageFilter AddByFilterType(MCDMessageFilterType messageFilterType)
	{
		GC.KeepAlive(this);
		ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsMessageFilters_addByFilterType(Handle, messageFilterType, out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsMessageFilter;
	}

	public MCDMessageFilter GetItemByIndex(uint index)
	{
		GC.KeepAlive(this);
		ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsMessageFilters_getItemByIndex(Handle, index, out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsMessageFilter;
	}

	public void Remove(MCDMessageFilter messageFilter)
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsMessageFilters_remove(Handle, DTS_ObjectMapper.getHandle(messageFilter as MappedObject));
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void RemoveAll()
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsMessageFilters_removeAll(Handle);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
	}

	public void RemoveByIndex(uint index)
	{
		GC.KeepAlive(this);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsMessageFilters_removeByIndex(Handle, index);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
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

	public List<MCDMessageFilter> ToList()
	{
		List<MCDMessageFilter> list = new List<MCDMessageFilter>();
		for (uint num = 0u; num < Count; num++)
		{
			list.Add(GetItemByIndex(num));
		}
		return list;
	}

	public MCDMessageFilter[] ToArray()
	{
		MCDMessageFilter[] array = new MCDMessageFilter[Count];
		for (uint num = 0u; num < Count; num++)
		{
			array[num] = GetItemByIndex(num);
		}
		return array;
	}

	public IEnumerator GetEnumerator()
	{
		return new DtsMessageFiltersEnumerator(this);
	}
}
