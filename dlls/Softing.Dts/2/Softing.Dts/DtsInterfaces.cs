using System;
using System.Collections;

namespace Softing.Dts;

public interface DtsInterfaces : MCDInterfaces, MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable, DtsNamedCollection, DtsCollection, DtsObject
{
}
