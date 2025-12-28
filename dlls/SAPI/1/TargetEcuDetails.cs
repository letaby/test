// Decompiled with JetBrains decompiler
// Type: SapiLayer1.TargetEcuDetails
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

#nullable disable
namespace SapiLayer1;

public sealed class TargetEcuDetails
{
  internal TargetEcuDetails(
    string ecu,
    string diagnosisVariant,
    string assumedDiagnosisVariant,
    int assumedUnknownCount)
  {
    this.Ecu = ecu;
    this.DiagnosisVariant = diagnosisVariant;
    this.AssumedDiagnosisVariant = assumedDiagnosisVariant;
    this.AssumedUnknownCount = assumedUnknownCount;
  }

  public string Ecu { get; private set; }

  public string DiagnosisVariant { get; private set; }

  public string AssumedDiagnosisVariant { get; private set; }

  public int AssumedUnknownCount { get; private set; }
}
