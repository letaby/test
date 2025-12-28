// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Procedures.Air_Mass_Adaptation.panel.Step
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Procedures.Air_Mass_Adaptation.panel;

internal enum Step
{
  None,
  StartAdaptation,
  SetMaxEngineSpeedLimit,
  ShutOffFans,
  CloseEGRValve,
  BeginCheckForOperatingConditions,
  WaitForOperatingConditions,
  DriveEngineSpeed,
  WaitEngineSpeed,
  HoldEngineSpeed,
  StopEngineSpeed,
  StartAAMA,
  RequestAAMAResults,
  ManualOperation,
  AdaptionComplete,
  OpenEGRValve,
  TurnOnFans,
  ResetMaxEngineSpeedLimit,
  CommitToPermanentMemory,
  ClearFaults,
}
