using System;
using System.Collections;

namespace Softing.Dts;

public interface DtsConfigurationRecords : MCDConfigurationRecords, MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable, DtsNamedCollection, DtsCollection, DtsObject
{
}
