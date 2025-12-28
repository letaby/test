// Decompiled with JetBrains decompiler
// Type: Softing.Dts.MCDDbDiagComPrimitive
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

public interface MCDDbDiagComPrimitive : MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
  MCDDbRequest DbRequest { get; }

  MCDDbResponses DbResponses { get; }

  MCDDbFunctionalClasses DbFunctionalClasses { get; }

  string Semantic { get; }

  MCDTransmissionMode TransmissionMode { get; }

  bool IsApiExecutable { get; }

  bool IsNoOperation { get; }

  MCDDbResponses GetDbResponsesByType(MCDResponseType type);

  MCDDbEcuStateTransitions GetDbEcuStateTransitionsByDbObject(MCDDbEcuStateChart chart);

  MCDDbEcuStateTransitions GetDbEcuStateTransitionsBySemantic(string semantic);

  MCDDbEcuStates GetDbPreConditionStatesByDbObject(MCDDbEcuStateChart chart);

  MCDDbEcuStates GetDbPreConditionStatesBySemantic(string semantic);
}
