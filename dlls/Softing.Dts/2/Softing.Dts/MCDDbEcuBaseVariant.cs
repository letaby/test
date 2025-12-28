using System;

namespace Softing.Dts;

public interface MCDDbEcuBaseVariant : MCDDbEcu, MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
	MCDDbEcuVariants DbEcuVariants { get; }
}
