using System;
using System.Collections;

namespace Softing.Dts;

public interface DtsDbComponentConnectors : MCDDbComponentConnectors, MCDCollection, MCDObject, IDisposable, IEnumerable, DtsCollection, DtsObject
{
}
