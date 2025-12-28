using System;
using System.Runtime.InteropServices;

namespace Softing.Dts;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
internal struct ByteField_Struct
{
	[MarshalAs(UnmanagedType.SysUInt)]
	internal IntPtr m_data;

	[MarshalAs(UnmanagedType.SysUInt)]
	internal IntPtr m_byteField;

	[MarshalAs(UnmanagedType.U4)]
	internal uint m_length;

	internal byte[] ToByteArray()
	{
		if (m_length != 0)
		{
			byte[] array = new byte[m_length];
			Marshal.Copy(m_data, array, 0, (int)m_length);
			CSWrap.CSNIDTS_releaseByteField(ref this);
			return array;
		}
		return null;
	}

	internal ByteField_Struct(byte[] byteArray)
	{
		m_data = IntPtr.Zero;
		m_byteField = IntPtr.Zero;
		m_length = 0u;
		if (byteArray != null && byteArray.Length != 0)
		{
			m_length = (uint)byteArray.Length;
			m_data = CSWrap.CSNIDTS_allocate((uint)byteArray.Length);
			Marshal.Copy(byteArray, 0, m_data, byteArray.Length);
		}
	}

	internal void FreeMemory()
	{
		if (m_data != IntPtr.Zero)
		{
			CSWrap.CSNIDTS_free(m_data);
		}
	}
}
