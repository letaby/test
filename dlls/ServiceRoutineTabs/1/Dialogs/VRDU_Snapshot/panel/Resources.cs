// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.VRDU_Snapshot.panel.Resources
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using System.ComponentModel;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.VRDU_Snapshot.panel;

public class Resources
{
  private static ComponentResourceManager ResourceManager
  {
    get => new ComponentResourceManager(typeof (UserPanel));
  }

  internal static string Message_ExtractingDiagnosticLinkQualifiers
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_ExtractingDiagnosticLinkQualifiers");
    }
  }

  internal static string Message_StartingVRDUExtraction
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_StartingVRDUExtraction");
  }

  internal static string Message_VRDUNotReady
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_VRDUNotReady");
  }

  internal static string Message_ProcessingComplete
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_ProcessingComplete");
  }

  internal static string MessageFormat_WritingExcelCompatableXmlDataFile0
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.MessageFormat_WritingExcelCompatableXmlDataFile0");
    }
  }

  internal static string Message_CouldNotRefreshEcuInfo
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_CouldNotRefreshEcuInfo");
  }

  internal static string Message_CouldNotUnlockVRDU
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_CouldNotUnlockVRDU");
  }

  internal static string Message_RefreshingData
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_RefreshingData");
  }

  internal static string Message_HoldOnThisWillTakeAbout60Seconds
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_HoldOnThisWillTakeAbout60Seconds");
    }
  }

  internal static string MessageFormat_WritingDataFile
  {
    get => Resources.ResourceManager.GetString("StringTable.MessageFormat_WritingDataFile");
  }

  internal static string Message_ExtractingVRDUQualifiers
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_ExtractingVRDUQualifiers");
  }

  internal static string Message_Working
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_Working");
  }

  internal static string Message_DataRefreshed
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_DataRefreshed");
  }

  internal static string Message_UnlockingVRDU
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_UnlockingVRDU");
  }

  internal static string Message_VRDUUnlocked
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_VRDUUnlocked");
  }

  internal static string Message_ExtractingABAData
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_ExtractingABAData");
  }
}
