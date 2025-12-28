using System;

namespace Softing.Dts;

public interface MCDObject : IDisposable
{
	MCDObjectType ObjectType { get; }
}
