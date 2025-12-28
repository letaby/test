using System;

namespace Softing.Dts;

public interface DtsDbFlashFilter : MCDDbFlashFilter, MCDDbObject, MCDNamedObject, MCDObject, IDisposable, DtsDbObject, DtsNamedObject, DtsObject
{
	ulong FilterEnd64 { get; }

	ulong FilterSize64 { get; }

	ulong FilterStart64 { get; }
}
