using System;

namespace Softing.Dts;

public interface MCDDbEcuMem : MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
	MCDDbEcuBaseVariants BaseVariants { get; }

	MCDDbFlashSessions FlashSessions { get; }

	MCDDbEcuVariants Variants { get; }

	MCDDbAdditionalAudiences DbAdditionalAudiences { get; }

	MCDDbSpecialDataGroups DbSDGs { get; }
}
