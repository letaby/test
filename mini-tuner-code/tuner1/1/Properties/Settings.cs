// Decompiled with JetBrains decompiler
// Type: TunerSolution.Properties.Settings
// Assembly: TunerSolution, Version=1.0.0.142, Culture=neutral, PublicKeyToken=null
// MVID: 9D02C703-4AB8-4296-B056-FAFCB6EB03BA
// Assembly location: C:\Users\petra\Downloads\TunerSolution\TunerSolution.exe

using System.CodeDom.Compiler;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;

#nullable disable
namespace TunerSolution.Properties;

[CompilerGenerated]
[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "15.9.0.0")]
internal sealed class Settings : ApplicationSettingsBase
{
  private static Settings defaultInstance = (Settings) SettingsBase.Synchronized((SettingsBase) new Settings());

  public static Settings Default => Settings.defaultInstance;

  [UserScopedSetting]
  [DebuggerNonUserCode]
  [DefaultSettingValue("0")]
  public int selectedDriver
  {
    get => (int) this[nameof (selectedDriver)];
    set => this[nameof (selectedDriver)] = (object) value;
  }

  [UserScopedSetting]
  [DebuggerNonUserCode]
  [DefaultSettingValue("0")]
  public int selectedDevice
  {
    get => (int) this[nameof (selectedDevice)];
    set => this[nameof (selectedDevice)] = (object) value;
  }

  [UserScopedSetting]
  [DebuggerNonUserCode]
  [DefaultSettingValue("")]
  public string fileName
  {
    get => (string) this[nameof (fileName)];
    set => this[nameof (fileName)] = (object) value;
  }

  [UserScopedSetting]
  [DebuggerNonUserCode]
  [DefaultSettingValue("0")]
  public int selectedTruck
  {
    get => (int) this[nameof (selectedTruck)];
    set => this[nameof (selectedTruck)] = (object) value;
  }

  [UserScopedSetting]
  [DebuggerNonUserCode]
  [DefaultSettingValue("0")]
  public int selectedSpecific
  {
    get => (int) this[nameof (selectedSpecific)];
    set => this[nameof (selectedSpecific)] = (object) value;
  }

  [UserScopedSetting]
  [DebuggerNonUserCode]
  [DefaultSettingValue("")]
  public string parameterFileName
  {
    get => (string) this[nameof (parameterFileName)];
    set => this[nameof (parameterFileName)] = (object) value;
  }
}
