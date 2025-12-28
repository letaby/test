// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.ATD_Maximum_Sensor_Value_Reset.panel.Resources
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using System.ComponentModel;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.ATD_Maximum_Sensor_Value_Reset.panel;

public class Resources
{
  private static ComponentResourceManager ResourceManager
  {
    get => new ComponentResourceManager(typeof (UserPanel));
  }

  internal static string MessageFormat_AnErrorOccurredDuringTheReset0
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.MessageFormat_AnErrorOccurredDuringTheReset0");
    }
  }

  internal static string MessageFormat_ResetExecuted0
  {
    get => Resources.ResourceManager.GetString("StringTable.MessageFormat_ResetExecuted0");
  }

  internal static string Message_Executing
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_Executing");
  }

  internal static string Message_PerformSensorValueReset
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_PerformSensorValueReset");
  }

  internal static string Message_ResetExecutedSuccessfully
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_ResetExecutedSuccessfully");
  }

  internal static string Message_PleaseConnectTheMCM
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_PleaseConnectTheMCM");
  }

  internal static string Message_MCMIsBusy
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_MCMIsBusy");
  }

  internal static string Message_UnableToAcquireTheResetServiceATDMaximumSensorValuesCannotBeReset
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_UnableToAcquireTheResetServiceATDMaximumSensorValuesCannotBeReset");
    }
  }

  internal static string Message_MCMIsConnected
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_MCMIsConnected");
  }
}
