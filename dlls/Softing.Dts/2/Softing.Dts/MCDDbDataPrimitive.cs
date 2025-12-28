using System;

namespace Softing.Dts;

public interface MCDDbDataPrimitive : MCDDbDiagComPrimitive, MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
	MCDDbAccessLevel AccessLevel { get; }

	MCDRepetitionMode RepetitionMode { get; }

	MCDDbSpecialDataGroups DbSDGs { get; }

	MCDAudience AudienceState { get; }

	MCDDbAdditionalAudiences DbDisabledAdditionalAudiences { get; }

	MCDDbAdditionalAudiences DbEnabledAdditionalAudiences { get; }

	MCDDbDataPrimitives GetRelatedDataPrimitives(string relationType);
}
