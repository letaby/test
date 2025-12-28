using System;
using System.Collections;

namespace Softing.Dts;

public interface DtsDbEnvDataConnectors : MCDDbEnvDataConnectors, MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable, DtsNamedCollection, DtsCollection, DtsObject
{
}
