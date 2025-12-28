using System;

namespace SapiLayer1;

[Flags]
internal enum CodingStringState
{
	None = 0,
	NeedsUpdate = 1,
	AssignedByClient = 2,
	Incomplete = 4
}
