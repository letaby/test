using System;

namespace Softing.Dts;

public interface MCDDbObject : MCDNamedObject, MCDObject, IDisposable
{
	string LongNameID { get; }

	string DescriptionID { get; }

	string UniqueObjectIdentifier { get; }
}
