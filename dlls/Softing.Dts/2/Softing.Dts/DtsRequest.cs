using System;

namespace Softing.Dts;

public interface DtsRequest : MCDRequest, MCDObject, IDisposable, DtsObject
{
	bool HasPDU { get; }
}
