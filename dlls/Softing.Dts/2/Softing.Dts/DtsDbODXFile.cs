using System;

namespace Softing.Dts;

public interface DtsDbODXFile : DtsDbObject, MCDDbObject, MCDNamedObject, MCDObject, IDisposable, DtsNamedObject, DtsObject
{
	string FileName { get; }

	string FileVersion { get; }

	MCDVersion ODXVersion { get; }
}
