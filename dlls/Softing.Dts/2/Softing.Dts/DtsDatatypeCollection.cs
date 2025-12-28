using System;

namespace Softing.Dts;

public interface DtsDatatypeCollection : DtsObject, MCDObject, IDisposable
{
	uint Count { get; }

	void RemoveAll();

	void RemoveByIndex(uint index);
}
