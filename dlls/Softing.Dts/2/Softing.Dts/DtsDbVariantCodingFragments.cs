using System;
using System.Collections;

namespace Softing.Dts;

public interface DtsDbVariantCodingFragments : DtsNamedCollection, MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable, DtsCollection, DtsObject
{
	DtsDbVariantCodingFragment GetItemByIndex(uint index);

	DtsDbVariantCodingFragment GetItemByName(string name);
}
