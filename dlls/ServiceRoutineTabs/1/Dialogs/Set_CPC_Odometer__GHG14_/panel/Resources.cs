// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.Set_CPC_Odometer__GHG14_.panel.Resources
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

using System.ComponentModel;

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.Set_CPC_Odometer__GHG14_.panel;

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

  internal static string MessageFormat_AnErrorOccurredSettingTheOdometer0
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.MessageFormat_AnErrorOccurredSettingTheOdometer0");
    }
  }

  internal static string Message_UnsupportedSoftwareVersionTitle
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.Message_UnsupportedSoftwareVersionTitle");
    }
  }

  internal static string MessageFormat_UnsupportedSoftwareVersionNoRepair
  {
    get
    {
      return Resources.ResourceManager.GetString("StringTable.MessageFormat_UnsupportedSoftwareVersionNoRepair");
    }
  }
}
