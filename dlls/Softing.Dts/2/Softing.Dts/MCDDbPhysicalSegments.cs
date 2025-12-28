using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDDbPhysicalSegments : MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDDbPhysicalSegment> ToList();

	MCDDbPhysicalSegment[] ToArray();

	MCDDbPhysicalSegment GetItemByIndex(uint index);

	MCDDbPhysicalSegment GetItemByName(string name);
}
