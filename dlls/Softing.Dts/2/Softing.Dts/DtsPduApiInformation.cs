using System;

namespace Softing.Dts;

public interface DtsPduApiInformation : DtsNamedObject, MCDNamedObject, MCDObject, IDisposable, DtsObject
{
	string PDUAPIVersion { get; }

	bool Enabled { get; set; }

	string LibraryFile { get; }
}
