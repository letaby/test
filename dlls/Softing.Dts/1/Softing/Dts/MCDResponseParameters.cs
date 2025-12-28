// Decompiled with JetBrains decompiler
// Type: Softing.Dts.MCDResponseParameters
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;
using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace Softing.Dts;

public interface MCDResponseParameters : 
  MCDNamedCollection,
  MCDCollection,
  MCDObject,
  IDisposable,
  IEnumerable
{
  List<MCDResponseParameter> ToList();

  MCDResponseParameter[] ToArray();

  MCDResponseParameter GetItemByIndex(uint index);

  MCDResponseParameter GetItemByName(string name);

  MCDResponseParameter AddElement();

  MCDResponseParameter AddElementWithContent(MCDResponseParameter pPattern);

  MCDResponseParameter AddMuxBranch(string strBranch);

  MCDResponseParameter AddMuxBranchByIndex(byte Index);

  MCDResponseParameter AddMuxBranchByIndexWithContent(
    byte Index,
    MCDResponseParameters pContentList);

  MCDResponseParameter AddMuxBranchWithContent(string strBranch, MCDResponseParameters pContentList);

  MCDResponseParameter SetParameterWithName(string name, MCDValue value);

  MCDResponseParameter AddMuxBranchByMuxValue(MCDValue muxValue);

  MCDResponseParameter AddMuxBranchByMuxValueWithContent(
    MCDValue muxValue,
    MCDResponseParameters pContentList);

  MCDObject Parent { get; }

  MCDResponseParameter AddEnvDataByDTC(uint dtc);
}
