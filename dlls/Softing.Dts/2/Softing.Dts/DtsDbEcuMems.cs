using System;
using System.Collections;

namespace Softing.Dts;

public interface DtsDbEcuMems : MCDDbEcuMems, MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable, DtsNamedCollection, DtsCollection, DtsObject
{
}
