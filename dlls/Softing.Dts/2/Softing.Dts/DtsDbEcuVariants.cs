using System;
using System.Collections;

namespace Softing.Dts;

public interface DtsDbEcuVariants : MCDDbEcuVariants, MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable, DtsNamedCollection, DtsCollection, DtsObject
{
}
