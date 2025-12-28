using System;

namespace Softing.Dts;

public interface DtsDbFlashData : MCDDbFlashData, MCDDbObject, MCDNamedObject, MCDObject, IDisposable, DtsDbObject, DtsNamedObject, DtsObject
{
	string FileName { get; }

	string TemporaryDataFileName { get; set; }

	string DatabaseFileName { get; }

	void ResetTemporaryDataFileName();
}
