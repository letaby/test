using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDAccessKeys : MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDAccessKey> ToList();

	MCDAccessKey[] ToArray();

	MCDAccessKey GetItemByIndex(uint index);
}
