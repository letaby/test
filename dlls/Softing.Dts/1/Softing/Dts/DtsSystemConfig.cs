// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsSystemConfig
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

public interface DtsSystemConfig : DtsObject, MCDObject, IDisposable
{
  string ProjectPath { get; set; }

  string DatabaseCachesPath { get; set; }

  void UpdateLicenseInfo();

  DtsSystemProperties SystemProperties { get; }

  string FinasReportPath { get; set; }

  string Odx201EditorPath { get; set; }

  string[] LicensedProducts { get; }

  void EnableWriteAccess();

  void ReleaseWriteAccess();

  void Save();

  DtsProjectConfigs ProjectConfigs { get; }

  DtsTraceConfig TraceConfig { get; }

  DtsJavaConfig JavaConfig { get; }

  DtsInterfaceInformations SupportedInterfaces { get; }

  DtsInterfaceConfigs InterfaceConfigs { get; }

  uint UserInterfaceLanguage { get; set; }

  string LicenseFile { set; }

  DtsLicenseInfos LicenseInfos { get; }

  bool HasChanges { get; }

  string RootDescriptionFile { get; set; }

  string TracePath { get; set; }

  string DefaultConfigPath { get; }

  DtsPduApiInformations SupportedPduApis { get; }
}
