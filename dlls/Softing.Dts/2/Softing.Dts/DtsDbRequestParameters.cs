using System;
using System.Collections;

namespace Softing.Dts;

public interface DtsDbRequestParameters : MCDDbRequestParameters, MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable, DtsNamedCollection, DtsCollection, DtsObject
{
}
