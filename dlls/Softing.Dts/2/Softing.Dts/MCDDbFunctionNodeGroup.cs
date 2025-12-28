using System;

namespace Softing.Dts;

public interface MCDDbFunctionNodeGroup : MCDDbBaseFunctionNode, MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
	MCDDbFunctionNodeGroups DbFunctionNodeGroups { get; }

	MCDDbFunctionNodes DbFunctionNodes { get; }

	MCDDbAdditionalAudiences DbEnabledAdditionalAudiences { get; }

	MCDDbAdditionalAudiences DbDisabledAdditionalAudiences { get; }
}
