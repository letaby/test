using System;

namespace Softing.Dts;

public interface MCDOptionItem : MCDConfigurationItem, MCDNamedObject, MCDObject, IDisposable
{
	ushort DecimalPlaces { get; }

	MCDDbItemValue MatchingDbItemValue { get; }

	MCDDbItemValue ItemValueByDbObject { set; }
}
