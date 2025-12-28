using System;
using System.Runtime.InteropServices;

namespace Softing.Dts;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
internal struct ObjectInfo_Struct
{
	[MarshalAs(UnmanagedType.SysUInt)]
	internal IntPtr m_handle;

	[MarshalAs(UnmanagedType.I4)]
	internal MCDObjectType m_type;

	internal MCDObject ToObject()
	{
		if (m_handle != IntPtr.Zero)
		{
			return DTS_ObjectMapper.createObject(m_handle, m_type);
		}
		return null;
	}
}
