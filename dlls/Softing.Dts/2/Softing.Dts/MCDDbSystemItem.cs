using System;

namespace Softing.Dts;

public interface MCDDbSystemItem : MCDDbConfigurationItem, MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
	string SystemParameterName { get; }
}
