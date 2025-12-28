using System.Collections.Generic;

namespace McdAbstraction;

public interface IMcdDataItem
{
	string Qualifier { get; }

	string Name { get; }

	IMcdDataItem Parent { get; }

	bool IsEnvironmentalData { get; }

	IEnumerable<IMcdDataItem> Parameters { get; }
}
