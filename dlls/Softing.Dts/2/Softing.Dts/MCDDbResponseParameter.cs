using System;

namespace Softing.Dts;

public interface MCDDbResponseParameter : MCDDbParameter, MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
	MCDValues NrcConstValues { get; }

	uint RequestBytePos { get; }
}
