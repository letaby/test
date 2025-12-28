using System;
using System.Collections;

namespace Softing.Dts;

public interface DtsDbParameters : MCDDbParameters, MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable, DtsNamedCollection, DtsCollection, DtsObject
{
}
