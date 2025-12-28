using System;

namespace Softing.Dts;

public interface DtsGlobalProtocolParameter : DtsRequestParameter, MCDRequestParameter, MCDParameter, MCDNamedObject, MCDObject, IDisposable, DtsParameter, DtsNamedObject, DtsObject
{
	bool Active { get; set; }
}
