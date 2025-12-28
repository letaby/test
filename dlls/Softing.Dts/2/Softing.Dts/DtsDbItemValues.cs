using System;
using System.Collections;

namespace Softing.Dts;

public interface DtsDbItemValues : MCDDbItemValues, MCDCollection, MCDObject, IDisposable, IEnumerable, DtsCollection, DtsObject
{
}
