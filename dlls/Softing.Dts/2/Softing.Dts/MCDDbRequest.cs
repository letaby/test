using System;

namespace Softing.Dts;

public interface MCDDbRequest : MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
	MCDValue DefaultPDU { get; }

	ushort PDUMinLength { get; }

	ushort PDUMaxLength { get; }

	MCDDbRequestParameters DbRequestParameters { get; }

	MCDDbSpecialDataGroups DbSDGs { get; }
}
