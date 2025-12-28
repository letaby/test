using System;

namespace Softing.Dts;

public interface MCDDbFlashDataBlock : MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
	MCDDbFlashSegments DbFlashSegments { get; }

	MCDDbFlashData DbFlashData { get; }

	MCDDbFlashIdents DbOwnIdents { get; }

	MCDDbFlashFilters DbFlashFilters { get; }

	long AddressOffset { get; }

	string DataBlockType { get; }

	MCDDbFlashSecurities DbSecurities { get; }

	MCDDbSpecialDataGroups DbSDGs { get; }

	MCDAudience AudienceState { get; }

	MCDDbAdditionalAudiences DbDisabledAdditionalAudiences { get; }

	MCDDbAdditionalAudiences DbEnabledAdditionalAudiences { get; }
}
