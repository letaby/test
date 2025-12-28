using System;

namespace SapiLayer1;

[Flags]
public enum ReadFunctions
{
	None = 0,
	NonPermanent = 1,
	Permanent = 2,
	Snapshot = 4
}
