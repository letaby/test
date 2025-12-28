using System;
using System.Collections;

namespace Softing.Dts;

public interface DtsDbVariantCodingDomains : DtsNamedCollection, MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable, DtsCollection, DtsObject
{
	DtsDbVariantCodingDomain GetItemByIndex(uint index);

	DtsDbVariantCodingDomain GetItemByName(string name);
}
