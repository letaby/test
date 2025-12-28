using System;

namespace Softing.Dts;

public interface MCDDbFlashFilter : MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
	uint FilterStart { get; }

	uint FilterEnd { get; }

	uint FilterSize { get; }
}
