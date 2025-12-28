using System;

namespace Softing.Dts;

public interface DtsSystemItem : MCDSystemItem, MCDConfigurationItem, MCDNamedObject, MCDObject, IDisposable, DtsConfigurationItem, DtsNamedObject, DtsObject
{
}
