using System;

namespace Softing.Dts;

public interface MCDDbProject : MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
	MCDAccessKeys AccessKeys { get; }

	MCDDbEcuBaseVariants DbEcuBaseVariants { get; }

	MCDDbFunctionalGroups DbFunctionalGroups { get; }

	MCDDbLocations DbProtocolLocations { get; }

	MCDDbVehicleInformations DbVehicleInformations { get; }

	MCDVersion Version { get; }

	MCDDbPhysicalVehicleLinkOrInterfaces DbPhysicalVehicleLinkOrInterfaces { get; }

	MCDDbEcuMems DbEcuMems { get; }

	MCDDbLocation DbMultipleEcuJobLocation { get; }

	MCDDbFunctionDictionaries DbFunctionDictionaries { get; }

	MCDDbObject GetDbElementByAccessKey(MCDAccessKey pAccessKey);

	void LoadNewECUMEM(string ecumemName, bool permanent);
}
