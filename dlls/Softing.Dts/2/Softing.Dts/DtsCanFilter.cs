using System;

namespace Softing.Dts;

public interface DtsCanFilter : DtsObject, MCDObject, IDisposable
{
	DtsFilterType Type { get; }

	string ShortName { get; set; }

	string Description { get; set; }

	bool IsChanged { get; }

	string File { get; }

	DtsCanFilterEntries GetFilterEntries(uint index);

	void Save();

	void ReloadFromFile();
}
