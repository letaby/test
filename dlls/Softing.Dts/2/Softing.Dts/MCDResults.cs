using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDResults : MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDResult> ToList();

	MCDResult[] ToArray();

	MCDResult GetItemByIndex(uint index);

	MCDResult Add(MCDError pError, MCDResultType type);

	MCDResult AddWithContent(MCDResult pResult);
}
