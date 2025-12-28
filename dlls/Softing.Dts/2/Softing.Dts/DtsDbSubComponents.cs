using System;
using System.Collections;

namespace Softing.Dts;

public interface DtsDbSubComponents : MCDDbSubComponents, MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable, DtsNamedCollection, DtsCollection, DtsObject
{
	string[] Semantics { get; }
}
