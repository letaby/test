// Decompiled with JetBrains decompiler
// Type: Softing.Dts.MCDDbLocation
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

public interface MCDDbLocation : MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
  MCDAccessKey AccessKey { get; }

  MCDDbEcu DbECU { get; }

  MCDLocationType Type { get; }

  MCDDbServices DbServices { get; }

  MCDDbFunctionalClasses DbFunctionalClasses { get; }

  MCDDbFlashSessionClasses DbFlashSessionClasses { get; }

  MCDDbFlashSessions DbFlashSessions { get; }

  MCDDbPhysicalMemories DbPhysicalMemories { get; }

  MCDDbControlPrimitives DbControlPrimitives { get; }

  MCDDbDataPrimitives DbDataPrimitives { get; }

  MCDDbDiagComPrimitives DbDiagComPrimitives { get; }

  MCDDbDiagServices DbDiagServices { get; }

  MCDDbDiagTroubleCodes GetDbDTCs(ushort levelLowLimit, ushort levelUpLimit);

  string[] DbDynDefinedSpecTables { get; }

  MCDDbResponseParameter GetDbDynDefinedSpecTableByName(string shortName);

  string GetDbDynDefinedSpecTableByDefinitionMode(string definitionMode);

  MCDDbResponseParameters GetDbEnvDataByTroubleCode(uint troubleCode);

  MCDDbJobs DbJobs { get; }

  MCDDbServices GetDbServicesBySemanticAttribute(string semantic);

  MCDValues GetSupportedDynIds(string definitionMode);

  MCDDbUnitGroups UnitGroups { get; }

  string[] AuthorizationMethods { get; }

  MCDDbDiagComPrimitives GetDbDiagComPrimitivesByType(MCDObjectType type);

  MCDDbSubComponents DbSubComponents { get; }

  MCDDbConfigurationDatas DbConfigurationDatas { get; }

  MCDDbTables DbTables { get; }

  MCDDbTables GetDbTablesBySemanticAttribute(string semantic);

  MCDDbFaultMemories DbFaultMemories { get; }

  MCDDbUnits DbUnits { get; }

  MCDDbMatchingPatterns DbVariantPatterns { get; }

  MCDDbAdditionalAudiences DbAdditionalAudiences { get; }

  MCDDbEnvDataDescs DbEnvDataDescs { get; }

  MCDDbDiagComPrimitives GetDbDiagComPrimitivesBySemanticAttribute(string sematic);

  MCDDbEcuStateCharts DbEcuStateCharts { get; }

  MCDDbSpecialDataGroups DbSDGs { get; }

  MCDDbTable GetDbTableByDefinitionMode(string definitionMode);
}
