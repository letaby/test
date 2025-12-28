using System;

namespace Softing.Dts;

public interface MCDProject : MCDNamedObject, MCDObject, IDisposable
{
	MCDDbProject DbProject { get; }

	MCDDbVehicleInformation ActiveDbVehicleInformation { get; }

	string[] IOControlNames { get; }

	MCDDbVehicleInformation SelectDbVehicleInformationByName(string shortName);

	void SelectDbVehicleInformation(MCDDbVehicleInformation vehicleInformation);

	MCDClampState GetClampState(string clampName);

	void DeselectVehicleInformation();

	MCDLogicalLink CreateLogicalLink(MCDDbLogicalLink LogicalLink);

	MCDLogicalLink CreateLogicalLinkByAccessKey(string AccessKeyString, string PhysicalVehicleLinkString);

	MCDLogicalLink CreateLogicalLinkByName(string LogicalLinkName);

	MCDLogicalLink CreateLogicalLinkByVariant(string BaseVariantLogicalLinkName, string VariantName);

	void RemoveLogicalLink(MCDLogicalLink LogicalLink);

	MCDValue CreateValue(MCDDataType dataType);

	MCDLogicalLink CreateLogicalLinkByAccessKeyAndInterface(string accessKeyString, string physicalVehicleLinkString, MCDInterface interface_);

	MCDLogicalLink CreateLogicalLinkByAccessKeyAndInterfaceResource(string accessKeyString, string physicalVehicleLinkString, MCDInterfaceResource interfaceResource);

	MCDLogicalLink CreateLogicalLinkByInterface(MCDDbLogicalLink logicalLink, MCDInterface interface_);

	MCDLogicalLink CreateLogicalLinkByInterfaceResource(MCDDbLogicalLink logicalLink, MCDInterfaceResource interfaceResource);

	MCDLogicalLink CreateLogicalLinkByNameAndInterface(string logicalLinkName, MCDInterface interface_);

	MCDLogicalLink CreateLogicalLinkByNameAndInterfaceResource(string logicalLinkName, MCDInterfaceResource interfaceResource);

	MCDLogicalLink CreateLogicalLinkByVariantAndInterface(string shortNameDbLogicalLink, string variantName, MCDInterface interface_);

	MCDLogicalLink CreateLogicalLinkByVariantAndInterfaceResource(string shortNameDbLogicalLink, string variantName, MCDInterfaceResource interfaceResource);

	MCDMonitoringLink CreateMonitoringLink(MCDInterfaceResource ifResource);

	MCDValues ExecIOCtrl(string IOCtrlName, MCDValue inputData, uint inputDataItemType, uint outputDataSize);

	void RemoveMonitoringLink(MCDMonitoringLink monLink);
}
