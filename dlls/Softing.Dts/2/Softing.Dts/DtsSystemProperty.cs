using System;

namespace Softing.Dts;

public interface DtsSystemProperty : DtsObject, MCDObject, IDisposable
{
	MCDValue Value { get; set; }

	string Name { get; set; }

	string StringValue { set; }
}
