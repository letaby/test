using System;

namespace Softing.Dts;

public interface MCDDbFlashSessionClass : MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
	MCDDbFlashSessions DbFlashSessions { get; }
}
