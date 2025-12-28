using System;

namespace Softing.Dts;

public interface DtsCanFilterEntry : DtsObject, MCDObject, IDisposable
{
	uint Identifier { get; set; }

	bool Extended { get; set; }
}
