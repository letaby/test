using System;

namespace Softing.Dts;

public interface DtsProjectConfig : DtsNamedObjectConfig, DtsObject, MCDObject, IDisposable
{
	DtsVehicleInformationConfigs VehicleInformations { get; }

	string Database { get; }

	string VehicleModelRange { get; set; }

	string[] ExternalFlashFiles { get; }

	bool UseOptimizedDatabase { get; set; }

	string OptimizedDatabase { get; }

	DtsProjectType ProjectType { get; set; }

	bool IsDatabaseODX201Legacy { get; }

	bool SynchronizeVitWithDatabase { get; set; }

	bool CreateDefaultVit { get; set; }

	string DCDIFirmware { get; set; }

	uint PreferredDbEncryption { get; set; }

	bool IsOptimizedDatabaseODX201Legacy { get; }

	bool JavaJob201Legacy { get; set; }

	DtsLogicalLinkFilterConfigs LogicalLinkFilters { get; }

	string PreferredOptionSet { get; set; }

	bool IsReferencing { get; }

	bool WriteSimFile { get; set; }

	bool SimFileAppend { get; set; }

	string ActiveSimFile { get; set; }

	uint Characteristic { get; set; }

	string[] GetModularDatabaseFiles(bool bForceReload);

	void AddModularDatabaseFile(string file, bool allowMove);

	string[] GetCxfDatabaseFiles(bool bForceReload);

	void AddCxfDatabaseFile(string file, bool allowMove);

	void SetDatabaseFile(string databaseFile, bool allowMove, bool overwriteExisting);

	void RemoveCxfDatabaseFiles(string[] files);

	void RemoveVariantCodingConfigFiles(string[] files);

	void AddCANFilterFile(string file, bool allowMove);

	void RemoveCANFilterFiles(string[] files);

	void SetOptimizedDatabaseFile(string databaseFile, bool allowMove, bool overwriteExisting);

	void AddExternalFlashFile(string file, bool allowMove);

	string[] GetCANFilterFiles(bool bForceReload);

	void RemoveExternalFlashFiles(string[] files);

	void UpdateVehicleInformationTables();

	void AddOTXProject(string otxProject, bool allowMove);

	string[] GetOTXProjects(bool bForceReload);

	void RemoveOTXProjects(string[] otxProjects);

	void GenerateOptimizedDatabase(uint encryption);

	int CheckDatabaseConsistency(bool bRunCheck);

	string[] GetVariantCodingConfigFiles(bool bForceReload);

	void AddVariantCodingConfigFile(string file, bool allowMove);

	void RemoveModularDatabaseFiles(string[] files);

	int CheckOptimizedDatabaseConsistency();

	void AddAdditionalDatabaseFiles(string file, bool allowMove);

	string[] GetAdditionalDatabaseFiles(bool bForceReload);

	void RemoveAdditionalDatabaseFiles(string[] files);

	void AddLogicalLinkFilterFile(string file, bool allowMove);

	string[] GetLogicalLinkFilterFiles(bool bForceReload);

	void RemoveLogicalLinkFilterFiles(string[] files);

	void AddSimFile(string file, bool allowMove);

	string[] GetSimFiles(bool bForceReload);

	void RemoveSimFiles(string[] files);
}
