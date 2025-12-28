// Decompiled with JetBrains decompiler
// Type: Softing.Dts.MCDDbFunctionOutParameters
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;
using System.Collections;

#nullable disable
namespace Softing.Dts;

public interface MCDDbFunctionOutParameters : 
  MCDNamedCollection,
  MCDCollection,
  MCDObject,
  IDisposable,
  IEnumerable
{
  MCDDbLocations GetDbLocationsForItemByIndex(uint index);

  MCDDbLocations GetDbLocationsForItemByName(string name);

  MCDDbFunctionOutParameter GetItemByIndex(uint index, MCDDbLocation locationContext);

  MCDDbFunctionOutParameter GetItemByName(string name, MCDDbLocation locationContext);
}
