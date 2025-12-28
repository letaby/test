// Decompiled with JetBrains decompiler
// Type: SapiLayer1.Dump
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

#nullable disable
namespace SapiLayer1;

public sealed class Dump
{
  private IList<byte> data;

  public Dump(string source)
  {
    if (source == null)
      return;
    if (source.Length % 2 == 1)
      source = "0" + source;
    int num = source.Length / 2;
    List<byte> byteList = new List<byte>();
    for (int index = 0; index < num; ++index)
    {
      string str = source.Substring(index * 2, 2);
      byteList.Add(Convert.ToByte(str, 16 /*0x10*/));
    }
    this.data = (IList<byte>) byteList.AsReadOnly();
  }

  public Dump(IEnumerable<byte> data)
  {
    if (data == null)
      return;
    this.data = (IList<byte>) new List<byte>(data).AsReadOnly();
  }

  public override string ToString() => this.data != null ? this.data.ToHexString() : string.Empty;

  public override int GetHashCode() => this.ToString().GetHashCode();

  public override bool Equals(object obj)
  {
    return obj != null && string.Equals(this.ToString(), obj.ToString());
  }

  public IList<byte> Data => this.data;

  [EditorBrowsable(EditorBrowsableState.Never)]
  [Obsolete("The GetData method is deprecated, please use the Data property instead.")]
  public byte[] GetData() => this.data.ToArray<byte>();

  public static bool MaskedMatch(byte[] content, Dump pattern, Dump mask)
  {
    int num = mask != null ? mask.data.Count : pattern.data.Count;
    if (content.Length < num || num == 0)
      return false;
    for (int index = 0; index < num; ++index)
    {
      if (mask != null)
      {
        if (((int) mask.data[index] & (int) content[index]) != (int) pattern.data[index])
          return false;
      }
      else if ((int) content[index] != (int) pattern.data[index])
        return false;
    }
    return true;
  }
}
