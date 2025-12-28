using System;

namespace Softing.Dts;

public interface DtsSmartComPrimitive : DtsObject, MCDObject, IDisposable
{
	string LogicalLinkLongName { get; }

	string PhysicalInterfaceName { get; }

	string Description { get; }

	MCDObjectType ComPrimitiveType { get; }

	string VariantLongName { get; }

	MCDAccessKey AccessKey { get; }

	string LogicalLinkShortName { get; }
}
