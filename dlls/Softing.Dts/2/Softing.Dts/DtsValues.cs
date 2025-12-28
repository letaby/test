using System;
using System.Collections;

namespace Softing.Dts;

public interface DtsValues : MCDValues, MCDCollection, MCDObject, IDisposable, IEnumerable, DtsCollection, DtsObject
{
}
