using System;

namespace Softing.Dts;

public interface MCDDbFlashIdent : MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
	MCDValues IdentValues { get; }

	MCDDbIdentDescription ReadDbIdentDescription { get; }
}
