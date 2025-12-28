// Decompiled with JetBrains decompiler
// Type: Softing.Dts.MCDMessageFilters
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;
using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace Softing.Dts;

public interface MCDMessageFilters : MCDCollection, MCDObject, IDisposable, IEnumerable
{
  List<MCDMessageFilter> ToList();

  MCDMessageFilter[] ToArray();

  MCDMessageFilter AddByFilterType(MCDMessageFilterType messageFilterType);

  MCDMessageFilter GetItemByIndex(uint index);

  void Remove(MCDMessageFilter messageFilter);

  void RemoveAll();

  void RemoveByIndex(uint index);
}
