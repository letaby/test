using System;

namespace Softing.Dts;

public interface MCDDbOptionItem : MCDDbConfigurationItem, MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
	MCDDbItemValues DbItemValues { get; }

	ushort DecimalPlaces { get; }

	MCDValue PhysicalDefaultValue { get; }

	MCDAudience ReadAudienceState { get; }

	MCDAudience WriteAudienceState { get; }

	MCDDbAdditionalAudiences DbDisabledReadAdditionalAudiences { get; }

	MCDDbAdditionalAudiences DbEnabledReadAdditionalAudiences { get; }

	MCDDbAdditionalAudiences DbDisabledWriteAdditionalAudiences { get; }

	MCDDbAdditionalAudiences DbEnabledWriteAdditionalAudiences { get; }
}
