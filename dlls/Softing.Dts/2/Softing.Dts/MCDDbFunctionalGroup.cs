using System;

namespace Softing.Dts;

public interface MCDDbFunctionalGroup : MCDDbEcu, MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
	MCDDbEcuBaseVariants GroupMembers { get; }
}
