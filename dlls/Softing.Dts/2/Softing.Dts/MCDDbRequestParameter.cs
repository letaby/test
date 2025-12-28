using System;

namespace Softing.Dts;

public interface MCDDbRequestParameter : MCDDbParameter, MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
	uint MaxNumberOfItems { get; }
}
