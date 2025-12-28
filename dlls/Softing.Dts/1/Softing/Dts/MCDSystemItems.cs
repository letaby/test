// Decompiled with JetBrains decompiler
// Type: Softing.Dts.MCDSystemItems
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;
using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace Softing.Dts;

public interface MCDSystemItems : 
  MCDNamedCollection,
  MCDCollection,
  MCDObject,
  IDisposable,
  IEnumerable
{
  List<MCDSystemItem> ToList();

  MCDSystemItem[] ToArray();

  MCDSystemItem GetItemByIndex(uint index);

  MCDSystemItem GetItemByName(string name);
}
