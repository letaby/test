// Decompiled with JetBrains decompiler
// Type: McdAbstraction.IMcdDataItem
// Assembly: McdAbstraction, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 2CF84A4E-9C9E-4158-9C67-2CE39889DD31
// Assembly location: C:\Users\petra\Downloads\Архив (2)\McdAbstraction.dll

using System.Collections.Generic;

#nullable disable
namespace McdAbstraction;

public interface IMcdDataItem
{
  string Qualifier { get; }

  string Name { get; }

  IMcdDataItem Parent { get; }

  bool IsEnvironmentalData { get; }

  IEnumerable<IMcdDataItem> Parameters { get; }
}
