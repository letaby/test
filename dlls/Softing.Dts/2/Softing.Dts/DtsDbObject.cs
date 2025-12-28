using System;

namespace Softing.Dts;

public interface DtsDbObject : MCDDbObject, MCDNamedObject, MCDObject, IDisposable, DtsNamedObject, DtsObject
{
	string DatabaseFile { get; }
}
