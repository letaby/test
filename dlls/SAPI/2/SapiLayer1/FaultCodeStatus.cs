using System;

namespace SapiLayer1;

[Flags]
public enum FaultCodeStatus
{
	None = 0,
	Active = 1,
	Pending = 4,
	Stored = 8,
	TestFailedSinceLastClear = 0x20,
	Mil = 0x80,
	Permanent = 0x100,
	Immediate = 0x200
}
