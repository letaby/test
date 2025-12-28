using System;

namespace Softing.Dts;

public interface DtsSystemConfig : DtsObject, MCDObject, IDisposable
{
	string ProjectPath { get; set; }

	string DatabaseCachesPath { get; set; }

	DtsSystemProperties SystemProperties { get; }

	string FinasReportPath { get; set; }

	string Odx201EditorPath { get; set; }

	string[] LicensedProducts { get; }

	DtsProjectConfigs ProjectConfigs { get; }

	DtsTraceConfig TraceConfig { get; }

	DtsJavaConfig JavaConfig { get; }

	DtsInterfaceInformations SupportedInterfaces { get; }

	DtsInterfaceConfigs InterfaceConfigs { get; }

	uint UserInterfaceLanguage { get; set; }

	string LicenseFile { set; }

	DtsLicenseInfos LicenseInfos { get; }

	bool HasChanges { get; }

	string RootDescriptionFile { get; set; }

	string TracePath { get; set; }

	string DefaultConfigPath { get; }

	DtsPduApiInformations SupportedPduApis { get; }

	void UpdateLicenseInfo();

	void EnableWriteAccess();

	void ReleaseWriteAccess();

	void Save();
}
