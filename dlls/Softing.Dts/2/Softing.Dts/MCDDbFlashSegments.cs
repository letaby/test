using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDDbFlashSegments : MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDDbFlashSegment> ToList();

	MCDDbFlashSegment[] ToArray();

	MCDDbFlashSegment GetItemByIndex(uint index);

	MCDDbFlashSegment GetItemByName(string name);
}
