using System;

namespace Softing.Dts;

public interface MCDDbTableParameter : MCDDbParameter, MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
	MCDValue Key { get; }

	MCDAudience AudienceState { get; }

	MCDDbAdditionalAudiences DbDisabledAdditionalAudiences { get; }

	MCDDbAdditionalAudiences DbEnabledAdditionalAudiences { get; }

	bool IsApiExecutable { get; }
}
