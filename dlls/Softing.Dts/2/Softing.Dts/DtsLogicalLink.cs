using System;

namespace Softing.Dts;

public interface DtsLogicalLink : MCDLogicalLink, MCDObject, IDisposable, DtsObject
{
	bool AutoSyncWithInternalState { get; set; }

	bool HasDetectedVariant { get; }

	MCDProtocolParameterSet ProtocolParameters { set; }

	DtsPhysicalInterfaceLink PhysicalInterfaceLink { get; }

	bool ChannelMonitoring { get; set; }

	MCDAccessKey CreationAccessKey { get; }

	MCDLogicalLinkState InternalState { get; }

	uint OpenCounter { get; }

	uint OnlineCounter { get; }

	uint StartCommCounter { get; }

	uint LockedCounter { get; }

	void LockLink();

	bool SupportsTimeStamp();

	void UnlockLink();

	MCDService CreateDVServiceByRelationType(string relationType);

	void GotoOffline();

	byte[] ExecuteIoCtl(uint uIoCtlCommandId, byte[] pInputData);

	void GotoOnlineWithTimeout(uint timeout);

	void OpenCached(bool useVariant);
}
