using System;

namespace Softing.Dts;

public interface MCDDbSpecialDataElement : MCDDbSpecialData, MCDObject, IDisposable
{
	string Content { get; }

	string TextID { get; }
}
