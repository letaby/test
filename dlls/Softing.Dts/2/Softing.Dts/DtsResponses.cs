using System;
using System.Collections;

namespace Softing.Dts;

public interface DtsResponses : MCDResponses, MCDCollection, MCDObject, IDisposable, IEnumerable, DtsCollection, DtsObject
{
}
