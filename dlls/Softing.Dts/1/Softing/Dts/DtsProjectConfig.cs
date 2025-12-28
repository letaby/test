// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsProjectConfig
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

public interface DtsProjectConfig : DtsNamedObjectConfig, DtsObject, MCDObject, IDisposable
{
  DtsVehicleInformationConfigs VehicleInformations { get; }

  string[] GetModularDatabaseFiles(bool bForceReload);

  void AddModularDatabaseFile(string file, bool allowMove);

  string Database { get; }

  string[] GetCxfDatabaseFiles(bool bForceReload);

  void AddCxfDatabaseFile(string file, bool allowMove);

  string VehicleModelRange { get; set; }

  void SetDatabaseFile(string databaseFile, bool allowMove, bool overwriteExisting);

  void RemoveCxfDatabaseFiles(string[] files);

  void RemoveVariantCodingConfigFiles(string[] files);

  void AddCANFilterFile(string file, bool allowMove);

  string[] ExternalFlashFiles { get; }

  void RemoveCANFilterFiles(string[] files);

  bool UseOptimizedDatabase { get; set; }

  void SetOptimizedDatabaseFile(string databaseFile, bool allowMove, bool overwriteExisting);

  string OptimizedDatabase { get; }

  DtsProjectType ProjectType { get; set; }

  bool IsDatabaseODX201Legacy { get; }

  bool SynchronizeVitWithDatabase { get; set; }

  bool CreateDefaultVit { get; set; }

  void AddExternalFlashFile(string file, bool allowMove);

  string[] GetCANFilterFiles(bool bForceReload);

  void RemoveExternalFlashFiles(string[] files);

  void UpdateVehicleInformationTables();

  void AddOTXProject(string otxProject, bool allowMove);

  string[] GetOTXProjects(bool bForceReload);

  void RemoveOTXProjects(string[] otxProjects);

  string DCDIFirmware { get; set; }

  void GenerateOptimizedDatabase(uint encryption);

  uint PreferredDbEncryption { get; set; }

  int CheckDatabaseConsistency(bool bRunCheck);

  string[] GetVariantCodingConfigFiles(bool bForceReload);

  void AddVariantCodingConfigFile(string file, bool allowMove);

  void RemoveModularDatabaseFiles(string[] files);

  bool IsOptimizedDatabaseODX201Legacy { get; }

  int CheckOptimizedDatabaseConsistency();

  bool JavaJob201Legacy { get; set; }

  void AddAdditionalDatabaseFiles(string file, bool allowMove);

  string[] GetAdditionalDatabaseFiles(bool bForceReload);

  void RemoveAdditionalDatabaseFiles(string[] files);

  void AddLogicalLinkFilterFile(string file, bool allowMove);

  string[] GetLogicalLinkFilterFiles(bool bForceReload);

  void RemoveLogicalLinkFilterFiles(string[] files);

  DtsLogicalLinkFilterConfigs LogicalLinkFilters { get; }

  string PreferredOptionSet { get; set; }

  bool IsReferencing { get; }

  bool WriteSimFile { get; set; }

  bool SimFileAppend { get; set; }

  void AddSimFile(string file, bool allowMove);

  string[] GetSimFiles(bool bForceReload);

  void RemoveSimFiles(string[] files);

  string ActiveSimFile { get; set; }

  uint Characteristic { get; set; }
}
