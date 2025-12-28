// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.Windows.Forms.Diagnostics.Container.FleetTimeZone
// Assembly: Drumroll, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: C4A91DC6-1B26-469B-9D8E-0DD5580BB754
// Assembly location: C:\Users\petra\Downloads\Telegram Desktop\Drumroll.exe

using System;

#nullable disable
namespace DetroitDiesel.Windows.Forms.Diagnostics.Container;

public sealed class FleetTimeZone : IEquatable<FleetTimeZone>
{
  private readonly string name;
  private readonly TimeSpan offset;

  public string Name => this.name;

  public TimeSpan Offset => this.offset;

  public override int GetHashCode() => this.name.GetHashCode();

  public override string ToString() => this.name;

  internal FleetTimeZone(string name, int offsetInHours)
  {
    this.name = name;
    this.offset = new TimeSpan(offsetInHours, 0, 0);
  }

  public bool Equals(FleetTimeZone other)
  {
    return this.offset == other.offset && string.Equals(this.name, other.name, StringComparison.OrdinalIgnoreCase);
  }
}
