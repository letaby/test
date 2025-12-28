using System;
using System.Collections;

namespace Softing.Dts;

public interface DtsDbPhysicalSegments : MCDDbPhysicalSegments, MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable, DtsNamedCollection, DtsCollection, DtsObject
{
}
