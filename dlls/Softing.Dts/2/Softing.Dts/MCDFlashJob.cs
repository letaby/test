using System;

namespace Softing.Dts;

public interface MCDFlashJob : MCDJob, MCDDataPrimitive, MCDDiagComPrimitive, MCDObject, IDisposable
{
	string SessionByFlashKey { set; }

	string SessionByName { set; }

	MCDDbFlashSession Session { set; }
}
