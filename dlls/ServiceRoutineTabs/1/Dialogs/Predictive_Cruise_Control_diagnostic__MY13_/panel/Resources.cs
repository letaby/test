// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Predictive_Cruise_Control_diagnostic__MY13_.panel.Resources
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using System.ComponentModel;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Predictive_Cruise_Control_diagnostic__MY13_.panel;

public class Resources
{
  private static ComponentResourceManager ResourceManager
  {
    get => new ComponentResourceManager(typeof (UserPanel));
  }

  internal static string Message_ThePredictiveCruiseControlConfigurationCanOnlyBeValidatedWhenTheCPCIsOnlineAndInAnApplicationMode
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_ThePredictiveCruiseControlConfigurationCanOnlyBeValidatedWhenTheCPCIsOnlineAndInAnApplicationMode");
    }
  }

  internal static string Message_ThePredictiveCruiseControlDeviceIsDetectedOnTheDataLinkAndTheCPCAppearsToBeConfiguredToUseIt
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_ThePredictiveCruiseControlDeviceIsDetectedOnTheDataLinkAndTheCPCAppearsToBeConfiguredToUseIt");
    }
  }

  internal static string Message_TheCPCIsNotConfiguredForPredictiveCruiseControlOperationAndTheDeviceIsNotDetectedOnTheDataLink
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_TheCPCIsNotConfiguredForPredictiveCruiseControlOperationAndTheDeviceIsNotDetectedOnTheDataLink");
    }
  }

  internal static string Message_ThePredictiveCruiseControlConfigurationCanOnlyBeValidatedOnceParametersHaveBeenReadFromTheCPCPleaseWait
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_ThePredictiveCruiseControlConfigurationCanOnlyBeValidatedOnceParametersHaveBeenReadFromTheCPCPleaseWait");
    }
  }

  internal static string Message_ThePredictiveCruiseControlConfigurationCanOnlyBeValidatedWhenTheDiagnosticToolIsConnectedToAJ1939Vehicle
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_ThePredictiveCruiseControlConfigurationCanOnlyBeValidatedWhenTheDiagnosticToolIsConnectedToAJ1939Vehicle");
    }
  }

  internal static string Message_TheCPCIsConfiguredForPredictiveCruiseControlOperationButTheDeviceIsNotDetectedOnTheDataLinkCheckTheVehicleWiring
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_TheCPCIsConfiguredForPredictiveCruiseControlOperationButTheDeviceIsNotDetectedOnTheDataLinkCheckTheVehicleWiring");
    }
  }

  internal static string Message_NoDataLink
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_NoDataLink");
  }

  internal static string Message_ThePredictiveCruiseControlDeviceIsDetectedOnTheDataLinkButTheCPCIsNotConfiguredToUseItUseTheDropDownToChangeTheCPCConfigurationThenPressTheSendButtonToCommit
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_ThePredictiveCruiseControlDeviceIsDetectedOnTheDataLinkButTheCPCIsNotConfiguredToUseItUseTheDropDownToChangeTheCPCConfigurationThenPressTheSendButtonToCommit");
    }
  }

  internal static string Message_ThePredictiveCruiseControlConfigurationIsBeingWrittenToTheCPCPleaseWait
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_ThePredictiveCruiseControlConfigurationIsBeingWrittenToTheCPCPleaseWait");
    }
  }

  internal static string Message_ThePredictiveCruiseControlConfigurationCanOnlyBeValidatedWhenAutomaticConnectionToStandardSAEJ1939DevicesIsEnabledInToolsOptionsConnections
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_ThePredictiveCruiseControlConfigurationCanOnlyBeValidatedWhenAutomaticConnectionToStandardSAEJ1939DevicesIsEnabledInToolsOptionsConnections");
    }
  }

  internal static string Message_UnableToDetect
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_UnableToDetect");
  }

  internal static string Message_NotDetected
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_NotDetected");
  }

  internal static string Message_Detected0
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_Detected0");
  }
}
