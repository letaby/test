using System;

namespace Softing.Dts;

public interface MCDDbCodeInformation : MCDObject, IDisposable
{
	string CodeFile { get; }

	string EntryPoint { get; }

	string Syntax { get; }
}
