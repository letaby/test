using System;

namespace Softing.Dts;

public interface DtsInterface : MCDInterface, MCDNamedObject, MCDObject, IDisposable, DtsNamedObject, DtsObject
{
	bool EthernetActivation { get; set; }

	string VendorModuleName { get; }

	DtsWLanSignalData WLanSignalData { get; }

	bool ExecuteBroadcast();

	void SetEthernetPinState(bool State, uint Number);

	uint GetEthernetPinState(uint Number);

	bool SetPhysicalLinkId(string keyLink, uint id);
}
