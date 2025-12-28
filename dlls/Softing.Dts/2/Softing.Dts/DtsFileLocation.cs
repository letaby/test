using System;

namespace Softing.Dts;

public interface DtsFileLocation : DtsObject, MCDObject, IDisposable
{
	string FilePath { get; set; }

	string ShortName { get; set; }

	string Version { get; set; }
}
