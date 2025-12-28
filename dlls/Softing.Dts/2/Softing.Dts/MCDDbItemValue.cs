using System;

namespace Softing.Dts;

public interface MCDDbItemValue : MCDObject, IDisposable
{
	MCDValue PhysicalConstantValue { get; }

	string Description { get; }

	string DescriptionID { get; }

	string Key { get; }

	string Meaning { get; }

	string MeaningID { get; }

	string Rule { get; }

	MCDAudience AudienceState { get; }

	MCDDbAdditionalAudiences DbDisabledAdditionalAudiences { get; }

	MCDDbAdditionalAudiences DbEnabledAdditionalAudiences { get; }

	MCDDbSpecialDataGroups DbSDGs { get; }
}
