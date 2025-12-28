using System;
using System.Collections;

namespace Softing.Dts;

public interface DtsErrors : MCDErrors, MCDCollection, MCDObject, IDisposable, IEnumerable, DtsCollection, DtsObject
{
}
