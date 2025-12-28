using System;
using System.Collections;

namespace Softing.Dts;

public interface DtsDbTables : MCDDbTables, MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable, DtsNamedCollection, DtsCollection, DtsObject
{
}
