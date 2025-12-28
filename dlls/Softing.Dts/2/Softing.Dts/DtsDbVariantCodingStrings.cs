using System;
using System.Collections;

namespace Softing.Dts;

public interface DtsDbVariantCodingStrings : DtsNamedCollection, MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable, DtsCollection, DtsObject
{
	DtsDbVariantCodingString GetItemByIndex(uint index);

	DtsDbVariantCodingString GetItemByName(string name);
}
