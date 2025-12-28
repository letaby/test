using System;

namespace Softing.Dts;

public interface MCDConfigurationItem : MCDNamedObject, MCDObject, IDisposable
{
	MCDDbConfigurationItem DbObject { get; }

	MCDValue ItemValue { get; }

	bool HasError { get; }

	MCDError Error { get; }
}
