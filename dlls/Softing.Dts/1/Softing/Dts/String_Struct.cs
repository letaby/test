// Decompiled with JetBrains decompiler
// Type: Softing.Dts.String_Struct
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;
using System.Runtime.InteropServices;

#nullable disable
namespace Softing.Dts;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
internal struct String_Struct
{
  [MarshalAs(UnmanagedType.SysUInt)]
  internal IntPtr m_data;

  internal string makeString()
  {
    if (!(this.m_data != IntPtr.Zero))
      return (string) null;
    string stringUni = Marshal.PtrToStringUni(this.m_data);
    CSWrap.CSNIDTS_cached_free(this.m_data);
    return stringUni;
  }
}
