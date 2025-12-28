using System;
using System.Collections;

namespace Softing.Dts;

public interface DtsDbTableParameters : MCDDbTableParameters, MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable, DtsNamedCollection, DtsCollection, DtsObject
{
}
