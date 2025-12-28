using System;

namespace Softing.Dts;

public interface DtsSystem : MCDSystem, MCDNamedObject, MCDObject, IDisposable, DtsNamedObject, DtsObject
{
	DtsClassFactory ClassFactory { get; }

	string CurrentTracePath { get; }

	string InstallationPath { get; }

	uint MaxNoOfClients { get; }

	string ProjectPath { get; }

	bool IsApiTraceEnabled { get; }

	uint ApiTraceLevel { set; }

	string TracePath { get; }

	MCDDbPhysicalVehicleLinkOrInterfaces DbPhysicalInterfaceLinks { get; }

	uint ClientNo { get; }

	string FlashDataRoot { set; }

	string JavaLocation { set; }

	string ConfigurationPath { get; }

	DtsSystemConfig Configuration { get; }

	string SessionProjectPath { set; }

	string DebugTracePath { get; }

	void CloseByteTrace(string PhysicalInterfaceLink);

	void DisableApiTrace();

	void EnableApiTrace();

	string GetStringFromEnum(uint eConst);

	string GetStringFromErrorCode(MCDErrorCodes errorCode);

	void Initialize();

	void OpenByteTrace(string PhysicalInterfaceLink, string FileName);

	void StartByteTrace(string PhysicalInterfaceLink);

	void StopByteTrace(string PhysicalInterfaceLink);

	void Uninitialize();

	DtsMonitorLink CreateMonitorLinkByName(string PhysicalInterfaceLinkName);

	DtsMonitorLink CreateMonitorLink(MCDDbPhysicalVehicleLinkOrInterface PhysicalInterfaceLink);

	uint RegisterApp(uint appID, uint reqItem);

	uint GetRequiredItem(uint ulID, uint reqItem);

	void EnableInterface(string shortName, bool bEnable);

	byte[] GetSeed(uint procedureId, DtsAppID appId);

	void SendKey(byte[] key);

	void DumpRunningObjects(string outputFile, bool singleObjects);

	MCDDbProject LoadViewerProject(string[] databaseFiles);

	void UnloadViewerProject(MCDDbProject project);

	void ReloadConfiguration();

	void DumpMemoryUsage(string outputFile, int flags, bool append);

	void WriteTraceEntry(string message);

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
