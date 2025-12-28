using System;
using System.Collections;

namespace Softing.Dts;

public interface DtsRequestParameters : MCDRequestParameters, MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable, DtsNamedCollection, DtsCollection, DtsObject
{
}
