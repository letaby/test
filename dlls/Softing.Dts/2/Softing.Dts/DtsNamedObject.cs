using System;

namespace Softing.Dts;

public interface DtsNamedObject : MCDNamedObject, MCDObject, IDisposable, DtsObject
{
	uint StringID { get; }
}
