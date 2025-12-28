using System;
using System.Collections;

namespace Softing.Dts;

public interface DtsDbUnits : MCDDbUnits, MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable, DtsNamedCollection, DtsCollection, DtsObject
{
}
