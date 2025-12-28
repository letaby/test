using System;

namespace Softing.Dts;

public interface MCDDbConfigurationData : MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
	MCDDbConfigurationRecords DbConfigurationRecords { get; }

	MCDDbEcuBaseVariants DbEcuBaseVariants { get; }

	MCDDbEcuVariants DbEcuVariants { get; }

	MCDVersion Version { get; }

	MCDDbAdditionalAudiences DbAdditionalAudiences { get; }

	MCDDbSpecialDataGroups DbSDGs { get; }
}
