using System;

namespace Softing.Dts;

public interface DtsValue : MCDValue, MCDObject, IDisposable, DtsObject
{
	string String { get; }

	bool IsEmpty { get; }

	bool IsValid { get; }

	bool GetBitfieldValue(uint index);

	byte GetBytefieldValue(uint Index);

	void SetBitfieldValue(bool value, uint index);

	void SetBytefieldValue(byte Value, uint Index);
}
