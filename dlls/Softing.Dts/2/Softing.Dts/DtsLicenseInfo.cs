using System;

namespace Softing.Dts;

public interface DtsLicenseInfo : DtsObject, MCDObject, IDisposable
{
	string HardwareName { get; }

	string[] Licenses { get; }

	string LendingTime { get; }

	string HardwareType { get; }

	string HardwareSerial { get; }

	string[] Products { get; }

	uint DatabaseEncryptionCount { get; }

	uint GetDatabaseEncryption(uint index);

	string GetDatabaseEncryptionName(uint index);
}
