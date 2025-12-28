using System;

namespace Softing.Dts;

public interface MCDDbFunctionNode : MCDDbBaseFunctionNode, MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
	MCDDbFunctionNodes DbFunctionNodes { get; }

	MCDDbSpecialDataGroups DbSDGs { get; }

	MCDDbAdditionalAudiences DbEnabledAdditionalAudiences { get; }

	MCDDbAdditionalAudiences DbDisabledAdditionalAudiences { get; }
}
