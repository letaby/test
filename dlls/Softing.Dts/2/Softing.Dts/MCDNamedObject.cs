using System;

namespace Softing.Dts;

public interface MCDNamedObject : MCDObject, IDisposable
{
	string Description { get; }

	string ShortName { get; }

	string LongName { get; }
}
