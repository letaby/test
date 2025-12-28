using System;

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

	string[] DbDynDefinedSpecTables { get; }

	MCDDbJobs DbJobs { get; }

	MCDDbUnitGroups UnitGroups { get; }

	string[] AuthorizationMethods { get; }

	MCDDbSubComponents DbSubComponents { get; }

	MCDDbConfigurationDatas DbConfigurationDatas { get; }

	MCDDbTables DbTables { get; }

	MCDDbFaultMemories DbFaultMemories { get; }

	MCDDbUnits DbUnits { get; }

	MCDDbMatchingPatterns DbVariantPatterns { get; }

	MCDDbAdditionalAudiences DbAdditionalAudiences { get; }

	MCDDbEnvDataDescs DbEnvDataDescs { get; }

	MCDDbEcuStateCharts DbEcuStateCharts { get; }

	MCDDbSpecialDataGroups DbSDGs { get; }

	MCDDbDiagTroubleCodes GetDbDTCs(ushort levelLowLimit, ushort levelUpLimit);

	MCDDbResponseParameter GetDbDynDefinedSpecTableByName(string shortName);

	string GetDbDynDefinedSpecTableByDefinitionMode(string definitionMode);

	MCDDbResponseParameters GetDbEnvDataByTroubleCode(uint troubleCode);

	MCDDbServices GetDbServicesBySemanticAttribute(string semantic);

	MCDValues GetSupportedDynIds(string definitionMode);

	MCDDbDiagComPrimitives GetDbDiagComPrimitivesByType(MCDObjectType type);

	MCDDbTables GetDbTablesBySemanticAttribute(string semantic);

	MCDDbDiagComPrimitives GetDbDiagComPrimitivesBySemanticAttribute(string sematic);

	MCDDbTable GetDbTableByDefinitionMode(string definitionMode);
}
