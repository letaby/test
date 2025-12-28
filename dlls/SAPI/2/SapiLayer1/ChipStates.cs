using System;

namespace SapiLayer1;

[Flags]
public enum ChipStates
{
	None = 0,
	BusOff = 1,
	ErrorPassive = 2,
	ErrorWarning = 4,
	ErrorActive = 8
}
