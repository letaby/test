using System;

namespace Softing.Dts;

public interface DtsProject : MCDProject, MCDNamedObject, MCDObject, IDisposable, DtsNamedObject, DtsObject
{
	string[] TraceFilterNames { get; }

	int CustomerVersion { get; }

	DtsGlobalProtocolParameterSets GlobalProtocolParameterSets { get; }

	DtsFileLocations DatabaseFileList { get; }

	string ActiveSimFile { get; set; }

	string[] SimFiles { get; }

	uint Characteristic { get; }

	string[] AglFiles { get; }

	string ProjectUid { get; }

	DtsLogicalLinkMonitor CreateDtsLogicalLinkMonitor(string PilShortName);

	string[] ListAddonFiles(string strDirectory, string strBaseVariant, string strVariant, string strIdents, bool bReload);

	void UnlinkDatabaseFiles();

	void ReplaceProjectFlashFile(string strOldFile, string strNewFile);

	void UnlinkDatabaseFile(string strFile);

	MCDDbEcuMems LinkDatabaseFile(string strFile);

	MCDMonitoringLink CreateDtsMonitoringLink(string PilShortName);

	DtsDoIPMonitorLink CreateDoIPMonitorLink(string NetworkId);

	void ClearLinkCache();
}
