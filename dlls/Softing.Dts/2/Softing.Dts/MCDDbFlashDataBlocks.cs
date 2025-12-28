using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDDbFlashDataBlocks : MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDDbFlashDataBlock> ToList();

	MCDDbFlashDataBlock[] ToArray();

	MCDDbFlashDataBlock GetItemByIndex(uint index);

	MCDDbFlashDataBlock GetItemByName(string name);
}
