// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsDbProject
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

public interface DtsDbProject : 
  MCDDbProject,
  MCDDbObject,
  MCDNamedObject,
  MCDObject,
  IDisposable,
  DtsDbObject,
  DtsNamedObject,
  DtsObject
{
  DtsDbODXFiles DbODXFiles { get; }

  string RevisionLabel { get; }

  MCDDbConfigurationDatas DbConfigurationDatas { get; }

  MCDDbConfigurationDatas LoadNewConfigurationDatasByFileName(string filename, bool permanent);

  DtsProjectType ProjectType { get; }

  string VehicleModelRange { get; }

  DtsIdentifierInfos IdentifierInfos { get; }

  DtsIdentifierInfos CreateIdentifierInfos();

  DtsCanFilters CanFilters { get; }
}
