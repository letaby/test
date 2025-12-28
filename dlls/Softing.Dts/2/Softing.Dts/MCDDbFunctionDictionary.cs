using System;

namespace Softing.Dts;

public interface MCDDbFunctionDictionary : MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
	MCDDbFunctionNodeGroups DbFunctionNodeGroups { get; }

	MCDDbFunctionNodes DbFunctionNodes { get; }

	MCDDbAdditionalAudiences DbAdditionalAudiences { get; }

	MCDDbSpecialDataGroups DbSDGs { get; }
}
