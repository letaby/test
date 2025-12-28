using System;

namespace Softing.Dts;

public interface DtsInterfaceConfig : DtsNamedObjectConfig, DtsObject, MCDObject, IDisposable
{
	string ModuleType { get; set; }

	string PDUAPIVersion { get; set; }

	bool Enabled { get; set; }

	DtsBusSystemInterfaceType BusSystemInterfaceType { get; set; }

	DtsInterfaceInformation InterfaceInformation { get; }

	string SerialNumber { get; set; }

	string IpAddress { get; set; }

	string Cable { get; set; }

	DtsInterfaceLinkInformations InterfaceLinkInformations { get; }

	DtsInterfaceLinkConfigs InterfaceLinkConfigs { get; }

	string VendorModuleName { get; set; }

	bool UseForLicensing { get; set; }

	string DetectedSerialNumber { get; }

	bool GetConnectedStatus(bool doDetection);
}
