// Decompiled with JetBrains decompiler
// Type: Softing.Dts.MCDDiagComPrimitive
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

public interface MCDDiagComPrimitive : MCDObject, IDisposable
{
  event OnPrimitiveBufferOverflow PrimitiveBufferOverflow;

  event OnPrimitiveCanceledDuringExecution PrimitiveCanceledDuringExecution;

  event OnPrimitiveCanceledFromQueue PrimitiveCanceledFromQueue;

  event OnPrimitiveError PrimitiveError;

  event OnPrimitiveHasIntermediateResult PrimitiveHasIntermediateResult;

  event OnPrimitiveHasResult PrimitiveHasResult;

  event OnPrimitiveJobInfo PrimitiveJobInfo;

  event OnPrimitiveProgressInfo PrimitiveProgressInfo;

  event OnPrimitiveRepetitionStopped PrimitiveRepetitionStopped;

  event OnPrimitiveTerminated PrimitiveTerminated;

  void Cancel();

  MCDResult ExecuteSync();

  MCDDbDiagComPrimitive DbObject { get; }

  MCDErrors Errors { get; }

  MCDRequest Request { get; }

  void ResetToDefaultValue(string parameterName);

  void ResetToDefaultValues();

  MCDObject Parent { get; }

  uint UniqueRuntimeID { get; }

  MCDDiagComPrimitiveState State { get; }
}
