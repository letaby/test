using System;

namespace Softing.Dts;

public interface MCDDbMatchingParameter : MCDObject, IDisposable
{
	MCDDbDiagComPrimitive DbDiagComPrimitive { get; }

	MCDDbResponseParameter DbResponseParameter { get; }

	MCDValue ExpectedValue { get; }
}
