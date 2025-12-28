using System;

namespace Softing.Dts;

public interface DtsGlobalProtocolParameterSet : DtsNamedObject, MCDNamedObject, MCDObject, IDisposable, DtsObject
{
	DtsGlobalProtocolParameters Parameters { get; }

	void Update();
}
