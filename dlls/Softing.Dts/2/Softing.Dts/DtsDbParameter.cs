using System;

namespace Softing.Dts;

public interface DtsDbParameter : MCDDbParameter, MCDDbObject, MCDNamedObject, MCDObject, IDisposable, DtsDbObject, DtsNamedObject, DtsObject
{
	DtsParameterMetaInfo ParameterMetaInformation { get; }

	string UnitTextID { get; }

	bool HasPhysicalConstraint { get; }

	bool HasDbUnit { get; }

	MCDDataType CodedParameterType { get; }

	DtsDbDataObjectProp DbDataObjectProp { get; }

	uint DtsMaxNumberOfItems { get; }

	string TableRowShortName { get; }

	MCDDbDiagTroubleCodes DbDTCs { get; }

	bool HasDefaultValue { get; }

	bool HasInternalConstraint { get; }

	MCDValue GetInternalFromPhysicalValue(MCDValue pValue);

	MCDValue GetInternalValueFromPDUFragment(MCDValue pValue);

	MCDValue GetPDUFragmentFromInternalValue(MCDValue pValue);

	MCDValue GetPhysicalFromInternalValue(MCDValue pValue);
}
