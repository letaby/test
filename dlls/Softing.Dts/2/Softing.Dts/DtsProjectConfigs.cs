using System;
using System.Collections;

namespace Softing.Dts;

public interface DtsProjectConfigs : DtsCollection, MCDCollection, MCDObject, IDisposable, IEnumerable, DtsObject
{
	DtsProjectConfig GetItemByIndex(uint index);

	DtsProjectConfig CreateItem(string name);

	void RemoveItem(DtsProjectConfig projectConfig);

	DtsProjectConfig GetItemByName(string name);

	bool ContainsValidProject(string dirPath, string projectPath);
}
