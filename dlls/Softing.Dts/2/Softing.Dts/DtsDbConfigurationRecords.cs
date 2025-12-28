using System;
using System.Collections;

namespace Softing.Dts;

public interface DtsDbConfigurationRecords : MCDDbConfigurationRecords, MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable, DtsNamedCollection, DtsCollection, DtsObject
{
}
