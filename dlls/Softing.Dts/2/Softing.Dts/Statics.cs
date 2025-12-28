using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Softing.Dts;

public class Statics
{
	[DllImport("kernel32.dll", SetLastError = true)]
	private static extern bool SetDllDirectory(string lpPathName);

	public static MCDSystem getSystem()
	{
		string directoryName = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().EscapedCodeBase).LocalPath);
		directoryName = Path.Combine(directoryName, "COS", Environment.Is64BitProcess ? "x64" : "x86", "bin");
		if (File.Exists(Path.Combine(directoryName, "cswrap.dll")))
		{
			SetDllDirectory(directoryName);
		}
		DTS_ObjectMapper.registerCallbacks();
		return DTS_ObjectMapper.createObject(CSWrap.CSNIDTS_getSystem(), MCDObjectType.eMCDSYSTEM) as MCDSystem;
	}

	public static MCDValue createValue()
	{
		return DTS_ObjectMapper.createObject(CSWrap.CSNIDTS_createValue(), MCDObjectType.eMCDVALUE) as MCDValue;
	}
}
