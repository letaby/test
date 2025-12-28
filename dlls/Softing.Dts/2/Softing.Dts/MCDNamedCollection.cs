using System;
using System.Collections;

namespace Softing.Dts;

public interface MCDNamedCollection : MCDCollection, MCDObject, IDisposable, IEnumerable
{
	string[] Names { get; }
}
