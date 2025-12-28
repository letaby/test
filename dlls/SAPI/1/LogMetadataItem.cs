// Decompiled with JetBrains decompiler
// Type: SapiLayer1.LogMetadataItem
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using System;

#nullable disable
namespace SapiLayer1;

public struct LogMetadataItem
{
  public const char FaultCodeSplitCharacter = '/';

  public LogMetadataItem(LogMetadataType type, string ecu, string content, string time)
    : this()
  {
    this.Type = type;
    this.Ecu = ecu;
    this.Content = content;
    this.Time = Sapi.TimeFromString(time);
  }

  public LogMetadataType Type { private set; get; }

  public string Ecu { private set; get; }

  public string Content { private set; get; }

  public DateTime Time { private set; get; }

  public static bool operator ==(LogMetadataItem left, LogMetadataItem right)
  {
    return left.Equals((object) right);
  }

  public static bool operator !=(LogMetadataItem left, LogMetadataItem right)
  {
    return !left.Equals((object) right);
  }

  public override bool Equals(object obj) => base.Equals(obj);

  public override int GetHashCode() => base.GetHashCode();
}
