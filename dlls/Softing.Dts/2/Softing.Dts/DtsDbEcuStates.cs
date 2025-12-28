using System;
using System.Collections;

namespace Softing.Dts;

public interface DtsDbEcuStates : MCDDbEcuStates, MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable, DtsNamedCollection, DtsCollection, DtsObject
{
}
