using System;

namespace Softing.Dts;

public interface DtsParameter : MCDParameter, MCDNamedObject, MCDObject, IDisposable, DtsNamedObject, DtsObject
{
	MCDDataType CodedType { get; }

	string ValueTextID { get; }

	string LongNameID { get; }

	string UnitTextID { get; }

	MCDParameter DtsLengthKey { get; }

	bool IsDtsVariableLength { get; }

	MCDNamedCollection DtsParameters { get; }

	string Semantic { get; }

	bool HasValue { get; }

	MCDValue CreateDtsValue();

	void AddDtsParameters(uint count);
}
