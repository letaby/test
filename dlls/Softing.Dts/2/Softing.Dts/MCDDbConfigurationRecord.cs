using System;

namespace Softing.Dts;

public interface MCDDbConfigurationRecord : MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
	uint ByteLength { get; }

	MCDValue ConfigurationID { get; }

	MCDDbConfigurationIdItem DbConfigurationIdItem { get; }

	MCDDbDataIdItem DbDataIdItem { get; }

	MCDDbDataRecords DbDataRecords { get; }

	MCDDbDataRecord DbDefaultDataRecord { get; }

	MCDDbOptionItems DbOptionItems { get; }

	MCDDbSystemItems DbSystemItems { get; }

	MCDAudience AudienceState { get; }

	MCDDbAdditionalAudiences DbDisabledAdditionalAudiences { get; }

	MCDDbAdditionalAudiences DbEnabledAdditionalAudiences { get; }

	MCDDbSpecialDataGroups DbSDGs { get; }

	MCDDbDiagComPrimitives GetDbReadDiagComPrimitives(MCDDbLocation dbLocation);

	MCDDbDiagComPrimitives GetDbWriteDiagComPrimitives(MCDDbLocation dbLocation);
}
