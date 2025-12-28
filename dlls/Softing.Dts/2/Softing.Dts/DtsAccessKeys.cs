using System;
using System.Collections;

namespace Softing.Dts;

public interface DtsAccessKeys : MCDAccessKeys, MCDCollection, MCDObject, IDisposable, IEnumerable, DtsCollection, DtsObject
{
}
