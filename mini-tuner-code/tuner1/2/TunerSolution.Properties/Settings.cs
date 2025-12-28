using System.CodeDom.Compiler;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace TunerSolution.Properties;

[CompilerGenerated]
[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "15.9.0.0")]
internal sealed class Settings : ApplicationSettingsBase
{
	private static Settings defaultInstance = (Settings)SettingsBase.Synchronized(new Settings());

	public static Settings Default => defaultInstance;

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("0")]
	public int selectedDriver
	{
		get
		{
			return (int)this["selectedDriver"];
		}
		set
		{
			this["selectedDriver"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("0")]
	public int selectedDevice
	{
		get
		{
			return (int)this["selectedDevice"];
		}
		set
		{
			this["selectedDevice"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("")]
	public string fileName
	{
		get
		{
			return (string)this["fileName"];
		}
		set
		{
			this["fileName"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("0")]
	public int selectedTruck
	{
		get
		{
			return (int)this["selectedTruck"];
		}
		set
		{
			this["selectedTruck"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("0")]
	public int selectedSpecific
	{
		get
		{
			return (int)this["selectedSpecific"];
		}
		set
		{
			this["selectedSpecific"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("")]
	public string parameterFileName
	{
		get
		{
			return (string)this["parameterFileName"];
		}
		set
		{
			this["parameterFileName"] = value;
		}
	}
}
