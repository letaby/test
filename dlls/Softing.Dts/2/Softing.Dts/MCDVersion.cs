using System;

namespace Softing.Dts;

public interface MCDVersion : MCDObject, IDisposable
{
	uint Major { get; }

	uint Minor { get; }

	uint Revision { get; }
}
