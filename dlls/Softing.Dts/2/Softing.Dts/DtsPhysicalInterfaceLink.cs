using System;

namespace Softing.Dts;

public interface DtsPhysicalInterfaceLink : DtsObject, MCDObject, IDisposable
{
	MCDInterface Interface { get; }
}
