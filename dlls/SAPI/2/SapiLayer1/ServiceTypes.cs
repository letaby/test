using System;

namespace SapiLayer1;

[Flags]
public enum ServiceTypes
{
	None = 0,
	Actuator = 1,
	Adjustment = 2,
	Data = 0x10,
	Download = 0x40,
	DiagJob = 0x40000,
	Environment = 0x80,
	Function = 0x200,
	Global = 0x10000,
	IOControl = 0x800000,
	ReadVarCode = 0x4000000,
	Routine = 0x400000,
	Security = 0x80000,
	Session = 0x100000,
	Static = 0x400,
	StoredData = 0x200000,
	WriteVarCode = 0x2000000
}
