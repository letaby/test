using System;
using System.Collections;

namespace Softing.Dts;

public interface DtsDbDiagServices : MCDDbDiagServices, MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable, DtsNamedCollection, DtsCollection, DtsObject
{
}
