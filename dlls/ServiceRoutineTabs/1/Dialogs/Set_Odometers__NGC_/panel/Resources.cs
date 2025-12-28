// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Set_Odometers__NGC_.panel.Resources
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using System.ComponentModel;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Set_Odometers__NGC_.panel;

public class Resources
{
  private static ComponentResourceManager ResourceManager
  {
    get => new ComponentResourceManager(typeof (UserPanel));
  }

  internal static string Message_WritingOdometerValue
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_WritingOdometerValue");
  }

  internal static string Message_TheOdometerCannotBeSetBecauseTheProposedValueIsEffectivelyEqualToCurrentOdometer
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_TheOdometerCannotBeSetBecauseTheProposedValueIsEffectivelyEqualToCurrentOdometer");
    }
  }

  internal static string Message_ErrorTheOdometerCannotBeSetBecauseTheProposedValueIsLessThanTheCurrentOdometer
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_ErrorTheOdometerCannotBeSetBecauseTheProposedValueIsLessThanTheCurrentOdometer");
    }
  }

  internal static string MessageFormat_UnsupportedSoftwareVersion
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.MessageFormat_UnsupportedSoftwareVersion");
    }
  }

  internal static string Message_TheOdometerIsReadyToBeSetBecauseTheProposedValueIsGreaterThanTheCurrentOdometer
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_TheOdometerIsReadyToBeSetBecauseTheProposedValueIsGreaterThanTheCurrentOdometer");
    }
  }

  internal static string Message_TheOdometerValueIsUnknownAndCannotBeSet
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_TheOdometerValueIsUnknownAndCannotBeSet");
    }
  }

  internal static string MessageFormat_UnsupportedSoftwareVersionNoRepair
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.MessageFormat_UnsupportedSoftwareVersionNoRepair");
    }
  }

  internal static string Message_TheOdometerHasBeenSetTheValueShownMayBeSlightlyDifferentThanTheOneEnteredDueToRoundingIssues
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_TheOdometerHasBeenSetTheValueShownMayBeSlightlyDifferentThanTheOneEnteredDueToRoundingIssues");
    }
  }

  internal static string Message_DDECReportsLifeToDateDataWillBeResetClickOkToContinueOrClickCancelToAbortChangesAndExit
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_DDECReportsLifeToDateDataWillBeResetClickOkToContinueOrClickCancelToAbortChangesAndExit");
    }
  }

  internal static string Message_Done
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_Done");
  }

  internal static string Message_CPCServiceNA
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_CPCServiceNA");
  }

  internal static string Message_InstrumentClusterOdometer
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_InstrumentClusterOdometer");
  }

  internal static string MessageFormat_AnErrorOccurredSettingTheOdometer0
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.MessageFormat_AnErrorOccurredSettingTheOdometer0");
    }
  }

  internal static string Message_Error
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_Error");
  }

  internal static string Message_UnsupportedSoftwareVersionTitle
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_UnsupportedSoftwareVersionTitle");
    }
  }

  internal static string Message_SettingCPC
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_SettingCPC");
  }

  internal static string Message_SettingInstrumentCluster
  {
    get => Resources.ResourceManager.GetString("StringTable.Message_SettingInstrumentCluster");
  }
}
