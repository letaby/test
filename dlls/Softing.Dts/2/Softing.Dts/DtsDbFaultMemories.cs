using System;
using System.Collections;

namespace Softing.Dts;

public interface DtsDbFaultMemories : MCDDbFaultMemories, MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable, DtsNamedCollection, DtsCollection, DtsObject
{
}
