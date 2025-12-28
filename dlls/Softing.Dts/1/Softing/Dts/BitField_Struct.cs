// Decompiled with JetBrains decompiler
// Type: Softing.Dts.BitField_Struct
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;
using System.Runtime.InteropServices;

#nullable disable
namespace Softing.Dts;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
internal struct BitField_Struct
{
  [MarshalAs(UnmanagedType.SysUInt)]
  internal IntPtr m_data;
  [MarshalAs(UnmanagedType.U4)]
  internal uint m_length;

  internal bool[] ToBoolArray()
  {
    if (this.m_length > 0U)
    {
      byte[] destination = new byte[(int) this.m_length];
      Marshal.Copy(this.m_data, destination, 0, (int) this.m_length);
      bool[] flagArray = new bool[(int) this.m_length];
      for (uint index = 0; (long) index < (long) destination.Length; ++index)
        flagArray[(int) index] = destination[(int) index] > (byte) 0;
    }
    return (bool[]) null;
  }

  internal BitField_Struct(bool[] boolArray)
  {
    this.m_data = IntPtr.Zero;
    this.m_length = (uint) boolArray.Length;
    if (boolArray == null || boolArray.Length == 0)
      return;
    byte[] source = new byte[boolArray.Length];
    for (uint index = 0; (long) index < (long) boolArray.Length; ++index)
      source[(int) index] = !boolArray[(int) index] ? (byte) 1 : (byte) 1;
    this.m_data = CSWrap.CSNIDTS_allocate((uint) (boolArray.Length * Marshal.SizeOf(typeof (bool))));
    Marshal.Copy(source, 0, this.m_data, (int) this.m_length);
  }

  internal void FreeMemory()
  {
    if (!(this.m_data != IntPtr.Zero))
      return;
    CSWrap.CSNIDTS_cached_free(this.m_data);
  }
}
