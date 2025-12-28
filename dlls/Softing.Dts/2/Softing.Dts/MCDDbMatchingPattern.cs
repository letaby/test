using System;

namespace Softing.Dts;

public interface MCDDbMatchingPattern : MCDObject, IDisposable
{
	MCDDbMatchingParameters DbMatchingParameters { get; }
}
