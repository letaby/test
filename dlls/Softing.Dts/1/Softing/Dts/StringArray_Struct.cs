// Decompiled with JetBrains decompiler
// Type: Softing.Dts.StringArray_Struct
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;
using System.Runtime.InteropServices;

#nullable disable
namespace Softing.Dts;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
internal struct StringArray_Struct
{
  [MarshalAs(UnmanagedType.SysUInt)]
  internal IntPtr m_data;
  [MarshalAs(UnmanagedType.U4)]
  internal uint m_length;

  internal string[] ToStringArray()
  {
    if (this.m_length <= 0U)
      return new string[0];
    string[] stringArray = new string[(int) this.m_length];
    IntPtr[] destination = new IntPtr[(int) this.m_length];
    Marshal.Copy(this.m_data, destination, 0, (int) this.m_length);
    for (uint index = 0; (long) index < (long) destination.Length; ++index)
      stringArray[(int) index] = Marshal.PtrToStringUni(destination[(int) index]);
    CSWrap.CSNIDTS_releaseStringArray(ref this);
    return stringArray;
  }

  internal StringArray_Struct(string[] stringArray)
  {
    this.m_data = IntPtr.Zero;
    this.m_length = 0U;
    if (stringArray == null || stringArray.Length == 0)
      return;
    this.m_length = (uint) stringArray.Length;
    this.m_data = CSWrap.CSNIDTS_cached_allocate((uint) (stringArray.Length * Marshal.SizeOf(typeof (IntPtr))));
    IntPtr[] source = new IntPtr[stringArray.Length];
    for (uint index = 0; index < this.m_length; ++index)
    {
      source[(int) index] = CSWrap.CSNIDTS_allocate_string((uint) stringArray[(int) index].Length);
      Marshal.Copy(stringArray[(int) index].ToCharArray(), 0, source[(int) index], stringArray[(int) index].Length);
    }
    Marshal.Copy(source, 0, this.m_data, source.Length);
  }

  internal void FreeMemory()
  {
    if (!(this.m_data != IntPtr.Zero))
      return;
    CSWrap.CSNIDTS_cached_free(this.m_data);
  }
}
