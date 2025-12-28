using System;

namespace Softing.Dts;

public interface DtsTextTableElement : MCDTextTableElement, MCDObject, IDisposable, DtsObject
{
	string Description { get; }
}
