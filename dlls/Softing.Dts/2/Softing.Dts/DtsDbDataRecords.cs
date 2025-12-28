using System;
using System.Collections;

namespace Softing.Dts;

public interface DtsDbDataRecords : MCDDbDataRecords, MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable, DtsNamedCollection, DtsCollection, DtsObject
{
}
