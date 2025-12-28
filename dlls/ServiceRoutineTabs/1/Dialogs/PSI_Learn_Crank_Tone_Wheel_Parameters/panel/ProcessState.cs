// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.ServiceRoutineTabs.Dialogs.PSI_Learn_Crank_Tone_Wheel_Parameters.panel.ProcessState
// Assembly: ServiceRoutineTabs, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: D5CFA739-5617-418F-8542-A6885771D80A
// Assembly location: C:\Users\petra\Downloads\Архив (2)\ServiceRoutineTabs.dll

#nullable disable
namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.PSI_Learn_Crank_Tone_Wheel_Parameters.panel;

public enum ProcessState
{
  Start,
  NotRunning,
  RequestSeed,
  SendKey,
  SetShortTermAdjust,
  Running,
  Stopping,
  ReturnControl,
  ReturnControlFailed,
  WaitingOnShutdown,
  Complete,
}
