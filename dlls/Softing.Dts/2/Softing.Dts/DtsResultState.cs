using System;

namespace Softing.Dts;

public interface DtsResultState : MCDResultState, MCDObject, IDisposable, DtsObject
{
	bool HasError { get; }

	MCDError Error { get; }
}
