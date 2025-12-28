using System;

namespace Softing.Dts;

public interface MCDProtocolParameterSet : MCDControlPrimitive, MCDDiagComPrimitive, MCDObject, IDisposable
{
	void FetchValueFromInterface(string shortName);

	void FetchValuesFromInterface();
}
