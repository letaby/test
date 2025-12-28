using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

internal class DtsDbProjectDescriptionsImpl : MappedObject, DtsDbProjectDescriptions, MCDDbProjectDescriptions, MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable, DtsNamedCollection, DtsCollection, DtsObject
{
	private class DtsDbProjectDescriptionsEnumerator : IEnumerator
	{
		private DtsDbProjectDescriptionsImpl m_classList;

		private int m_index;

		public object Current => m_classList.GetItemByIndex((uint)m_index);

		public DtsDbProjectDescriptionsEnumerator(DtsDbProjectDescriptionsImpl classList)
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

	public string[] Names
	{
		get
		{
			GC.KeepAlive(this);
			StringArray_Struct returnValue = default(StringArray_Struct);
			IntPtr intPtr = CSWrap.CSNIDTS_DtsNamedCollection_getNames(Handle, out returnValue);
			if (intPtr != IntPtr.Zero)
			{
				throw DTS_ObjectMapper.createException(intPtr);
			}
			return returnValue.ToStringArray();
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

	public DtsDbProjectDescriptionsImpl(IntPtr handle)
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

	~DtsDbProjectDescriptionsImpl()
	{
		Dispose(disposing: false);
	}

	public MCDDbProjectDescription GetItemByIndex(uint index)
	{
		GC.KeepAlive(this);
		ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsDbProjectDescriptions_getItemByIndex(Handle, index, out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbProjectDescription;
	}

	public MCDDbProjectDescription GetItemByName(string name)
	{
		GC.KeepAlive(this);
		ObjectInfo_Struct returnValue = default(ObjectInfo_Struct);
		IntPtr intPtr = CSWrap.CSNIDTS_DtsDbProjectDescriptions_getItemByName(Handle, name, out returnValue);
		if (intPtr != IntPtr.Zero)
		{
			throw DTS_ObjectMapper.createException(intPtr);
		}
		return DTS_ObjectMapper.createObject(returnValue.m_handle, returnValue.m_type) as DtsDbProjectDescription;
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

	public List<MCDDbProjectDescription> ToList()
	{
		List<MCDDbProjectDescription> list = new List<MCDDbProjectDescription>();
		for (uint num = 0u; num < Count; num++)
		{
			list.Add(GetItemByIndex(num));
		}
		return list;
	}

	public MCDDbProjectDescription[] ToArray()
	{
		MCDDbProjectDescription[] array = new MCDDbProjectDescription[Count];
		for (uint num = 0u; num < Count; num++)
		{
			array[num] = GetItemByIndex(num);
		}
		return array;
	}

	public IEnumerator GetEnumerator()
	{
		return new DtsDbProjectDescriptionsEnumerator(this);
	}
}
