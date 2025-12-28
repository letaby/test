using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDDbFlashChecksums : MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDDbFlashChecksum> ToList();

	MCDDbFlashChecksum[] ToArray();

	MCDDbFlashChecksum GetItemByIndex(uint index);

	MCDDbFlashChecksum GetItemByName(string name);
}
