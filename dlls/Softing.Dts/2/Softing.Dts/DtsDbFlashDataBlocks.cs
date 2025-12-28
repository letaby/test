using System;
using System.Collections;

namespace Softing.Dts;

public interface DtsDbFlashDataBlocks : MCDDbFlashDataBlocks, MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable, DtsNamedCollection, DtsCollection, DtsObject
{
}
