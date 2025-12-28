using System;

namespace Softing.Dts;

public interface DtsOptionItem : MCDOptionItem, MCDConfigurationItem, MCDNamedObject, MCDObject, IDisposable, DtsConfigurationItem, DtsNamedObject, DtsObject
{
	MCDValue ItemValueDts { get; set; }
}
