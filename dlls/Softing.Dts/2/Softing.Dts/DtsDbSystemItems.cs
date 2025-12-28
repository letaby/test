using System;
using System.Collections;

namespace Softing.Dts;

public interface DtsDbSystemItems : MCDDbSystemItems, MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable, DtsNamedCollection, DtsCollection, DtsObject
{
}
