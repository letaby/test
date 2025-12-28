// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.EGR_DP_Sensor_Recalibration__MY13_.panel.Resources
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using System.ComponentModel;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.EGR_DP_Sensor_Recalibration__MY13_.panel;

public class Resources
{
  private static ComponentResourceManager ResourceManager
  {
    get => new ComponentResourceManager(typeof (UserPanel));
  }

  internal static string Message_EngineIsNotRunning
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_EngineIsNotRunning");
  }

  internal static string MessageFormat_AnErrorOccurredDuringRecalibration
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.MessageFormat_AnErrorOccurredDuringRecalibration");
    }
  }

  internal static string Message_MCM21TIsBusy
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_MCM21TIsBusy");
  }

  internal static string Message_EngineSpeedCannotBeDetected
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_EngineSpeedCannotBeDetected");
  }

  internal static string MessageFormat_RecalibrationExecuted0
  {
    get => Resources.ResourceManager.GetString("StringTable.MessageFormat_RecalibrationExecuted0");
  }

  internal static string Message_PerformEGRDPSensorRecalibration
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_PerformEGRDPSensorRecalibration");
    }
  }

  internal static string Message_EngineIsRunning
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_EngineIsRunning");
  }

  internal static string Message_MCM21TIsNotConnected
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_MCM21TIsNotConnected");
  }

  internal static string Message_UnableToAcquireTheServiceEGRDPSensorCannotBeRecalibrated
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_UnableToAcquireTheServiceEGRDPSensorCannotBeRecalibrated");
    }
  }

  internal static string Message_MCM21TIsConnected
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_MCM21TIsConnected");
  }
}
