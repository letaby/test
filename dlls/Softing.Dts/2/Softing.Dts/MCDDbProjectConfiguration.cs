using System;

namespace Softing.Dts;

public interface MCDDbProjectConfiguration : MCDObject, IDisposable
{
	MCDDbProject ActiveDbProject { get; }

	string[] AdditionalECUMEMNames { get; }

	void Close();

	MCDDbProject Load(string name);
}
