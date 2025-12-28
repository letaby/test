using System;
using System.Collections;

namespace Softing.Dts;

public interface DtsDbTableRowConnectors : MCDDbTableRowConnectors, MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable, DtsNamedCollection, DtsCollection, DtsObject
{
}
