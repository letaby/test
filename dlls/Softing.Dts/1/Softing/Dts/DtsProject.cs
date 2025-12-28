// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsProject
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

public interface DtsProject : 
  MCDProject,
  MCDNamedObject,
  MCDObject,
  IDisposable,
  DtsNamedObject,
  DtsObject
{
  string[] TraceFilterNames { get; }

  DtsLogicalLinkMonitor CreateDtsLogicalLinkMonitor(string PilShortName);

  string[] ListAddonFiles(
    string strDirectory,
    string strBaseVariant,
    string strVariant,
    string strIdents,
    bool bReload);

  void UnlinkDatabaseFiles();

  void ReplaceProjectFlashFile(string strOldFile, string strNewFile);

  int CustomerVersion { get; }

  DtsGlobalProtocolParameterSets GlobalProtocolParameterSets { get; }

  void UnlinkDatabaseFile(string strFile);

  MCDDbEcuMems LinkDatabaseFile(string strFile);

  DtsFileLocations DatabaseFileList { get; }

  MCDMonitoringLink CreateDtsMonitoringLink(string PilShortName);

  string ActiveSimFile { get; set; }

  string[] SimFiles { get; }

  DtsDoIPMonitorLink CreateDoIPMonitorLink(string NetworkId);

  uint Characteristic { get; }

  string[] AglFiles { get; }

  void ClearLinkCache();

  string ProjectUid { get; }
}
