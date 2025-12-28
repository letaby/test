using System;

namespace Softing.Dts;

public interface MCDDbFlashSecurity : MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
	MCDValue SecurityMethod { get; }

	MCDValue Validity { get; }

	MCDValue FlashwareSignature { get; }

	MCDValue FlashwareChecksum { get; }
}
