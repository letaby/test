// Decompiled with JetBrains decompiler
// Type: Softing.Dts.ByteField_Struct
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;
using System.Runtime.InteropServices;

#nullable disable
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
    if (this.m_length <= 0U)
      return (byte[]) null;
    byte[] destination = new byte[(int) this.m_length];
    Marshal.Copy(this.m_data, destination, 0, (int) this.m_length);
    CSWrap.CSNIDTS_releaseByteField(ref this);
    return destination;
  }

  internal ByteField_Struct(byte[] byteArray)
  {
    this.m_data = IntPtr.Zero;
    this.m_byteField = IntPtr.Zero;
    this.m_length = 0U;
    if (byteArray == null || byteArray.Length == 0)
      return;
    this.m_length = (uint) byteArray.Length;
    this.m_data = CSWrap.CSNIDTS_allocate((uint) byteArray.Length);
    Marshal.Copy(byteArray, 0, this.m_data, byteArray.Length);
  }

  internal void FreeMemory()
  {
    if (!(this.m_data != IntPtr.Zero))
      return;
    CSWrap.CSNIDTS_free(this.m_data);
  }
}
