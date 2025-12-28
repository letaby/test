using System;

namespace Softing.Dts;

public interface MCDDbExternalAccessMethod : MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
	string Method { get; }
}
