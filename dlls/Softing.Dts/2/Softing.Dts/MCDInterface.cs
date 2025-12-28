using System;

namespace Softing.Dts;

public interface MCDInterface : MCDNamedObject, MCDObject, IDisposable
{
	string PDUApiSoftwareName { get; }

	MCDVersion PDUApiSoftwareVersion { get; }

	string VendorName { get; }

	uint HardwareSerialNumber { get; }

	string[] IOControlNames { get; }

	MCDDbInterfaceCable CurrentDbInterfaceCable { get; }

	ulong CurrentTime { get; }

	MCDDbInterfaceCables DbInterfaceCables { get; }

	string FirmwareName { get; }

	MCDVersion FirmwareVersion { get; }

	string HardwareName { get; }

	MCDVersion HardwareVersion { get; }

	MCDInterfaceResources InterfaceResources { get; }

	MCDVersion MVCIVersionPart1StandardVersion { get; }

	MCDVersion MVCIVersionPart2StandardVersion { get; }

	MCDInterfaceStatus Status { get; }

	double BatteryVoltage { get; }

	MCDValue GetClampState(uint pinOnInterfaceConnector, string clampName);

	void Connect();

	void Disconnect();

	MCDValues ExecIOCtrl(string IOCtrlName, MCDValue inputData, uint inputDataItemType, uint outputDataSize);

	void DetectInterfaces(string optionString);

	void Reset();

	double GetProgrammingVoltage(uint pinOnInterfaceConnector);

	void SetProgrammingVoltage(uint pinOnInterfaceConnector, double voltage);
}
