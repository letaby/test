using System;

namespace Softing.Dts;

internal interface MappedObject
{
	IntPtr Handle { get; set; }
}
