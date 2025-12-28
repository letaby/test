using System;

namespace Softing.Dts;

public interface MCDDbEcuVariant : MCDDbEcu, MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
	MCDDbEcuBaseVariant DbEcuBaseVariant { get; }
}
