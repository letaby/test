using System;

namespace Softing.Dts;

public interface MCDValue : MCDObject, IDisposable
{
	bool[] Bitfield { get; set; }

	byte[] Bytefield { get; set; }

	MCDDataType DataType { get; set; }

	float Float32 { get; set; }

	double Float64 { get; set; }

	short Int16 { get; set; }

	int Int32 { get; set; }

	long Int64 { get; set; }

	char Int8 { get; set; }

	int Length { get; }

	ushort Uint16 { get; set; }

	uint Uint32 { get; set; }

	ulong Uint64 { get; set; }

	byte Uint8 { get; set; }

	string ValueAsString { get; set; }

	string Asciistring { get; set; }

	string Unicode2string { get; set; }

	bool Boolean { get; set; }

	void Clear();

	MCDValue Copy();
}
