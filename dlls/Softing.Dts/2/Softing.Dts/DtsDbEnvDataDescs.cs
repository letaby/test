using System;
using System.Collections;

namespace Softing.Dts;

public interface DtsDbEnvDataDescs : MCDDbEnvDataDescs, MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable, DtsNamedCollection, DtsCollection, DtsObject
{
}
