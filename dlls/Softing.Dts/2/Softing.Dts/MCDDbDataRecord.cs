using System;

namespace Softing.Dts;

public interface MCDDbDataRecord : MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
	byte[] BinaryData { get; }

	MCDFlashDataFormat DataFormat { get; }

	MCDValue DataID { get; }

	MCDDbCodingData DbCodingData { get; }

	string Key { get; }

	string Rule { get; }

	string UserDefinedFormat { get; }

	MCDAudience AudienceState { get; }

	MCDDbAdditionalAudiences DbDisabledAdditionalAudiences { get; }

	MCDDbAdditionalAudiences DbEnabledAdditionalAudiences { get; }

	MCDDbSpecialDataGroups DbSDGs { get; }
}
