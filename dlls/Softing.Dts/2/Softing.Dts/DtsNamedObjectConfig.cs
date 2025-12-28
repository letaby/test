using System;

namespace Softing.Dts;

public interface DtsNamedObjectConfig : DtsObject, MCDObject, IDisposable
{
	string ShortName { get; set; }

	string LongName { get; set; }

	string Description { get; set; }
}
