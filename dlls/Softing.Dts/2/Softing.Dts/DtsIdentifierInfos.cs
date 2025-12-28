using System;
using System.Collections;

namespace Softing.Dts;

public interface DtsIdentifierInfos : DtsCollection, MCDCollection, MCDObject, IDisposable, IEnumerable, DtsObject
{
	DtsIdentifierInfo GetItemByIndex(uint index);

	void LoadDatabase(string fileName);

	void UnloadAll();

	void UnloadDatabase(string fileName);

	DtsIdentifierInfo GetItemByIdentifier(uint identifier, bool extended, int extendedAddress);
}
