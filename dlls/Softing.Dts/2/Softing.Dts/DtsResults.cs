using System;
using System.Collections;

namespace Softing.Dts;

public interface DtsResults : MCDResults, MCDCollection, MCDObject, IDisposable, IEnumerable, DtsCollection, DtsObject
{
}
