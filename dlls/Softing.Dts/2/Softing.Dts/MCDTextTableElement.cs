using System;

namespace Softing.Dts;

public interface MCDTextTableElement : MCDObject, IDisposable
{
	string LongNameID { get; }

	MCDInterval Interval { get; }

	string LongName { get; }
}
