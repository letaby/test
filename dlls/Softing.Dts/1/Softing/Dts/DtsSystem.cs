// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsSystem
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

public interface DtsSystem : 
  MCDSystem,
  MCDNamedObject,
  MCDObject,
  IDisposable,
  DtsNamedObject,
  DtsObject
{
  void CloseByteTrace(string PhysicalInterfaceLink);

  void DisableApiTrace();

  void EnableApiTrace();

  DtsClassFactory ClassFactory { get; }

  string CurrentTracePath { get; }

  string InstallationPath { get; }

  uint MaxNoOfClients { get; }

  string ProjectPath { get; }

  string GetStringFromEnum(uint eConst);

  string GetStringFromErrorCode(MCDErrorCodes errorCode);

  void Initialize();

  bool IsApiTraceEnabled { get; }

  void OpenByteTrace(string PhysicalInterfaceLink, string FileName);

  uint ApiTraceLevel { set; }

  void StartByteTrace(string PhysicalInterfaceLink);

  void StopByteTrace(string PhysicalInterfaceLink);

  void Uninitialize();

  string TracePath { get; }

  MCDDbPhysicalVehicleLinkOrInterfaces DbPhysicalInterfaceLinks { get; }

  DtsMonitorLink CreateMonitorLinkByName(string PhysicalInterfaceLinkName);

  DtsMonitorLink CreateMonitorLink(
    MCDDbPhysicalVehicleLinkOrInterface PhysicalInterfaceLink);

  uint RegisterApp(uint appID, uint reqItem);

  uint GetRequiredItem(uint ulID, uint reqItem);

  uint ClientNo { get; }

  void EnableInterface(string shortName, bool bEnable);

  string FlashDataRoot { set; }

  string JavaLocation { set; }

  byte[] GetSeed(uint procedureId, DtsAppID appId);

  void SendKey(byte[] key);

  string ConfigurationPath { get; }

  void DumpRunningObjects(string outputFile, bool singleObjects);

  DtsSystemConfig Configuration { get; }

  MCDDbProject LoadViewerProject(string[] databaseFiles);

  void UnloadViewerProject(MCDDbProject project);

  string SessionProjectPath { set; }

  void ReloadConfiguration();

  void DumpMemoryUsage(string outputFile, int flags, bool append);

  void WriteTraceEntry(string message);

  string DebugTracePath { get; }

  void StartSnapshotModeTracing(string PhysicalInterfaceLink, uint timeInterval);

  void TakeSnapshotByteTrace(string PhysicalInterfaceLink);

  void GenerateSnapshotByteTrace(string PhysicalInterfaceLink, string outputPath);

  void StopSnapshotModeTracing(string PhysicalInterfaceLink);

  void WriteExternTraceEntry(string prefix, string message);

  MCDProject SelectDynamicProject(string name, string[] files);

  MCDValue GetOptionalProperty(string name);

  void StartInterfaceDetection();

  void CreateJVM();
}
