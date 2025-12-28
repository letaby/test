using System;
using System.Collections;

namespace Softing.Dts;

public interface DtsDbResponseParameters : MCDDbResponseParameters, MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable, DtsNamedCollection, DtsCollection, DtsObject
{
}
