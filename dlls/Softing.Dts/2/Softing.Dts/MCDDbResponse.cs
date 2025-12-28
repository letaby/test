using System;

namespace Softing.Dts;

public interface MCDDbResponse : MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
	MCDDbResponseParameters DbResponseParameters { get; }

	MCDResponseType ResponseType { get; }

	MCDDbSpecialDataGroups DbSDGs { get; }
}
