using System;
using System.Collections;

namespace Softing.Dts;

public interface DtsSystemProperties : DtsCollection, MCDCollection, MCDObject, IDisposable, IEnumerable, DtsObject
{
	DtsSystemProperty CreateItem();

	DtsSystemProperty GetItemByIndex(uint index);
}
