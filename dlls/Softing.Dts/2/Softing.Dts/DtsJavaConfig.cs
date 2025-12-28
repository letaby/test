using System;

namespace Softing.Dts;

public interface DtsJavaConfig : DtsObject, MCDObject, IDisposable
{
	DtsFileLocations JvmLocations { get; }

	DtsFileLocations CompilerLocations { get; }

	DtsFileLocation CurrentJvmLocation { get; set; }

	DtsFileLocation CurrentCompilerLocation { get; set; }

	bool JobDebugging { get; set; }

	string CompilerOptions { get; set; }
}
