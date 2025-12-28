using System;
using System.Collections;

namespace Softing.Dts;

public interface DtsNamedCollection : MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable, DtsCollection, DtsObject
{
}
