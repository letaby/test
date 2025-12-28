using System;
using System.Collections;

namespace Softing.Dts;

public interface DtsDbFlashSessions : MCDDbFlashSessions, MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable, DtsNamedCollection, DtsCollection, DtsObject
{
}
