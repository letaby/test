using System;

namespace Softing.Dts;

public interface MCDDbSpecialData : MCDObject, IDisposable
{
	string SemanticInformation { get; }
}
