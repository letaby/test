// Decompiled with JetBrains decompiler
// Type: SapiLayer1.FaultCodeStatusExtensions
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace SapiLayer1;

internal static class FaultCodeStatusExtensions
{
  private static FaultCodeStatusExtensions.TriState GetTriState(
    bool setCondition,
    bool previouslySetCondition)
  {
    if (setCondition)
      return FaultCodeStatusExtensions.TriState.Set;
    return !previouslySetCondition ? FaultCodeStatusExtensions.TriState.NotSet : FaultCodeStatusExtensions.TriState.PreviouslySet;
  }

  private static string ToDisplayString(
    this FaultCodeStatusExtensions.TriState status,
    bool confirmed,
    Ecu ecu)
  {
    switch (status)
    {
      case FaultCodeStatusExtensions.TriState.NotSet:
        return !confirmed ? FaultCodeStatusExtensions.Translate("not active", ecu) : FaultCodeStatusExtensions.Translate("not confirmed", ecu);
      case FaultCodeStatusExtensions.TriState.Set:
        return !confirmed ? FaultCodeStatusExtensions.Translate("active", ecu) : FaultCodeStatusExtensions.Translate(nameof (confirmed), ecu);
      case FaultCodeStatusExtensions.TriState.PreviouslySet:
        return !confirmed ? FaultCodeStatusExtensions.Translate("previously active", ecu) : FaultCodeStatusExtensions.Translate("previously confirmed", ecu);
      default:
        throw new ArgumentException("Unknown status value.", nameof (status));
    }
  }

  private static string Translate(string original, Ecu ecu)
  {
    return ecu.Translate(Sapi.MakeTranslationIdentifier(original.CreateQualifierFromName(), "FaultCodeStatus"), original);
  }

  public static string ToStatusString(this FaultCodeStatus status, bool obd, Ecu ecu)
  {
    if (status == FaultCodeStatus.None)
      return FaultCodeStatusExtensions.Translate("no fault", ecu);
    if (!obd)
      return FaultCodeStatusExtensions.GetTriState((status & FaultCodeStatus.Active) != 0, (status & FaultCodeStatus.TestFailedSinceLastClear) != 0).ToDisplayString(false, ecu);
    List<string> values = new List<string>();
    if ((status & FaultCodeStatus.Pending) != FaultCodeStatus.None)
      values.Add(FaultCodeStatusExtensions.Translate("pending", ecu));
    FaultCodeStatusExtensions.TriState triState1 = FaultCodeStatusExtensions.GetTriState((status & FaultCodeStatus.Mil) != 0, (status & FaultCodeStatus.Stored) != 0);
    if (triState1 != FaultCodeStatusExtensions.TriState.NotSet)
      values.Add(triState1.ToDisplayString(true, ecu));
    FaultCodeStatusExtensions.TriState triState2 = FaultCodeStatusExtensions.GetTriState((status & FaultCodeStatus.Active) != 0, (status & FaultCodeStatus.TestFailedSinceLastClear) != 0);
    if (triState2 != FaultCodeStatusExtensions.TriState.NotSet)
      values.Add(triState2.ToDisplayString(false, ecu));
    if ((status & FaultCodeStatus.Permanent) != FaultCodeStatus.None)
      values.Add(FaultCodeStatusExtensions.Translate("permanent", ecu));
    if ((status & FaultCodeStatus.Immediate) != FaultCodeStatus.None)
      values.Add(FaultCodeStatusExtensions.Translate("immediate", ecu));
    return string.Join(", ", (IEnumerable<string>) values);
  }

  private enum TriState
  {
    NotSet,
    Set,
    PreviouslySet,
  }
}
