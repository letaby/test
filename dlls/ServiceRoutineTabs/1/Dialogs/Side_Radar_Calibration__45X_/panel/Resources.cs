// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Side_Radar_Calibration__45X_.panel.Resources
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using System.ComponentModel;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Side_Radar_Calibration__45X_.panel;

public class Resources
{
  private static ComponentResourceManager ResourceManager
  {
    get => new ComponentResourceManager(typeof (UserPanel));
  }

  internal static string Message_ResettingFaultCodes
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_ResettingFaultCodes");
  }

  internal static string Message_MessageText
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_MessageText");
  }

  internal static string Message_AuthenticationStateChanged
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_AuthenticationStateChanged");
  }

  internal static string Message_UnableToChangeAuthhenticatedState
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_UnableToChangeAuthhenticatedState");
    }
  }

  internal static string Message_DynamicCalibrationSDAStopped
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_DynamicCalibrationSDAStopped");
  }

  internal static string Message_DynamicCalibrationSDAStarted
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_DynamicCalibrationSDAStarted");
  }

  internal static string Message_UnableToStopDynamicCalibrationSDA
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_UnableToStopDynamicCalibrationSDA");
    }
  }

  internal static string MessageFormat_StatusMessage
  {
    get => Resources.ResourceManager.GetString("StringTable.MessageFormat_StatusMessage");
  }
}
