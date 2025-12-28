using System;

namespace Softing.Dts;

public interface MCDDbFunctionalClass : MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
	MCDDbLocation DbLocation { get; }

	MCDDbDataPrimitives DbDataPrimitives { get; }
}
