using System;

namespace Softing.Dts;

public interface DtsLogicalLinkFilterConfig : DtsNamedObjectConfig, DtsObject, MCDObject, IDisposable
{
	bool FilteringFlag { get; set; }

	string[] AccessKeys { get; }

	void AddAccessKey(string accessKey);

	void RemoveAccessKey(string accessKey);
}
