using System;

namespace Softing.Dts;

public interface MCDDbCodingData : MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
	string FileName { get; }

	bool IsLateBound { get; }
}
