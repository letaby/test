// Decompiled with JetBrains decompiler
// Type: Softing.Dts.MCDDbProject
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

public interface MCDDbProject : MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
  MCDAccessKeys AccessKeys { get; }

  MCDDbEcuBaseVariants DbEcuBaseVariants { get; }

  MCDDbFunctionalGroups DbFunctionalGroups { get; }

  MCDDbLocations DbProtocolLocations { get; }

  MCDDbVehicleInformations DbVehicleInformations { get; }

  MCDVersion Version { get; }

  MCDDbObject GetDbElementByAccessKey(MCDAccessKey pAccessKey);

  MCDDbPhysicalVehicleLinkOrInterfaces DbPhysicalVehicleLinkOrInterfaces { get; }

  MCDDbEcuMems DbEcuMems { get; }

  void LoadNewECUMEM(string ecumemName, bool permanent);

  MCDDbLocation DbMultipleEcuJobLocation { get; }

  MCDDbFunctionDictionaries DbFunctionDictionaries { get; }
}
