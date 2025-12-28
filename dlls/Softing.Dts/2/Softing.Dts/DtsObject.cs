using System;

namespace Softing.Dts;

public interface DtsObject : MCDObject, IDisposable
{
	uint ObjectID { get; }
}
