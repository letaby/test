using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDDbFlashSessions : MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDDbFlashSession> ToList();

	MCDDbFlashSession[] ToArray();

	MCDDbFlashSession GetItemByIndex(uint index);

	MCDDbFlashSession GetItemByName(string name);

	MCDDbFlashSession GetDbFlashSessionByFlashKey(string flashkey);

	uint GetFlashKeyPriority(string flashkey);
}
