using System;

namespace Softing.Dts;

public interface DtsDbProjectConfiguration : MCDDbProjectConfiguration, MCDObject, IDisposable, DtsObject
{
	void UnloadConfigurationProject();
}
