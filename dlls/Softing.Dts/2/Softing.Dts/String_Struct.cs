using System;
using System.Runtime.InteropServices;

namespace Softing.Dts;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
internal struct String_Struct
{
	[MarshalAs(UnmanagedType.SysUInt)]
	internal IntPtr m_data;

	internal string makeString()
	{
		if (m_data != IntPtr.Zero)
		{
			string result = Marshal.PtrToStringUni(m_data);
			CSWrap.CSNIDTS_cached_free(m_data);
			return result;
		}
		return null;
	}
}
