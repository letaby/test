using System;

namespace Softing.Dts;

public interface DtsDbAccessLevel : MCDDbAccessLevel, MCDDbObject, MCDNamedObject, MCDObject, IDisposable, DtsDbObject, DtsNamedObject, DtsObject
{
}
