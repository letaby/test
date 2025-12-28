using System;

namespace Softing.Dts;

public interface DtsDbDiagComPrimitive : MCDDbDiagComPrimitive, MCDDbObject, MCDNamedObject, MCDObject, IDisposable, DtsDbObject, DtsNamedObject, DtsObject
{
	DtsComPrimitiveType ComPrimitiveType { get; }

	MCDValue ID { get; }

	DtsDbProtocolParameters DbProtocolParameters { get; }

	MCDValue DID { get; }

	bool HasDID { get; }

	string InternalShortName { get; }

	bool SupportsPDUInformation();
}
