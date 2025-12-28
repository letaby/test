using System;

namespace Softing.Dts;

public interface MCDDbIdentDescription : MCDObject, IDisposable
{
	MCDDbDataPrimitive DbDataPrimitive { get; }

	MCDDbResponseParameter DbResponseParameter { get; }

	MCDDbDataPrimitive GetDbDataPrimitiveByLocation(MCDDbLocation pDtsDbLocation);
}
