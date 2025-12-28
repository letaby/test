using System;

namespace Softing.Dts;

public interface MCDDbPhysicalSegment : MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
	uint BlockSize { get; }

	uint EndAddress { get; }

	byte[] FillByte { get; }

	uint StartAddress { get; }
}
