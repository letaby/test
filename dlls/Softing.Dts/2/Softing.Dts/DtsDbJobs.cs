using System;
using System.Collections;

namespace Softing.Dts;

public interface DtsDbJobs : MCDDbJobs, MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable, DtsNamedCollection, DtsCollection, DtsObject
{
}
