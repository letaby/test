// Decompiled with JetBrains decompiler
// Type: Softing.Dts.Statics
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

#nullable disable
namespace Softing.Dts;

public class Statics
{
  [DllImport("kernel32.dll", SetLastError = true)]
  private static extern bool SetDllDirectory(string lpPathName);

  public static MCDSystem getSystem()
  {
    string str = Path.Combine(Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().EscapedCodeBase).LocalPath), "COS", Environment.Is64BitProcess ? "x64" : "x86", "bin");
    if (File.Exists(Path.Combine(str, "cswrap.dll")))
      Statics.SetDllDirectory(str);
    DTS_ObjectMapper.registerCallbacks();
    return DTS_ObjectMapper.createObject(CSWrap.CSNIDTS_getSystem(), MCDObjectType.eMCDSYSTEM) as MCDSystem;
  }

  public static MCDValue createValue()
  {
    return DTS_ObjectMapper.createObject(CSWrap.CSNIDTS_createValue(), MCDObjectType.eMCDVALUE) as MCDValue;
  }
}
