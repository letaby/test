using System;
using System.Collections;

namespace Softing.Dts;

public interface MCDCollection : MCDObject, IDisposable, IEnumerable
{
	uint Count { get; }
}
