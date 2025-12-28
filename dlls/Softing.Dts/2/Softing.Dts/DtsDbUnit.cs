using System;

namespace Softing.Dts;

public interface DtsDbUnit : MCDDbUnit, MCDDbObject, MCDNamedObject, MCDObject, IDisposable, DtsDbObject, DtsNamedObject, DtsObject
{
	bool HasDbPhysicalDimension { get; }
}
