// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.XCPCCPActivation.panel.VedocReadInputDataQualifiers
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.XCPCCPActivation.panel;

internal class VedocReadInputDataQualifiers
{
  public string ChallengeCodeQualifier;
  public string IdCodeQualifier;
  public string NumberOfCodesQualifier;
  public string TransponderCodeQualifier;

  private VedocReadInputDataQualifiers(
    string challengeCodeQualifier,
    string idCodeQualifier,
    string numberOfCodesQualifier,
    string transponderCodeQualifier)
  {
    this.ChallengeCodeQualifier = challengeCodeQualifier;
    this.IdCodeQualifier = idCodeQualifier;
    this.NumberOfCodesQualifier = numberOfCodesQualifier;
    this.TransponderCodeQualifier = transponderCodeQualifier;
  }

  public static VedocReadInputDataQualifiers Create(
    string challengeCodeQualifier,
    string idCodeQualifier,
    string numberOfCodesQualifier,
    string transponderCodeQualifier)
  {
    return new VedocReadInputDataQualifiers(challengeCodeQualifier, idCodeQualifier, numberOfCodesQualifier, transponderCodeQualifier);
  }
}
