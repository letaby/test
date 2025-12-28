// Decompiled with JetBrains decompiler
// Type: SapiLayer1.Session
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using System;

#nullable disable
namespace SapiLayer1;

public sealed class Session : IComparable
{
  private DateTime startTime;
  private DateTime endTime;
  private Channel channel;
  private string descriptionVersion;
  private ConnectionResource resource;
  private bool? isFixedVariant;
  private ChannelOptions channelOptions;
  private string variantName;

  internal Session(
    Channel channel,
    DateTime start,
    DateTime end,
    string version,
    ConnectionResource resource,
    string variantName,
    bool? isFixedVariant,
    ChannelOptions channelOptions)
  {
    this.startTime = start;
    this.endTime = end;
    this.channel = channel;
    this.descriptionVersion = version;
    this.resource = resource;
    this.variantName = variantName;
    this.isFixedVariant = isFixedVariant;
    this.channelOptions = channelOptions;
  }

  internal void UpdateEndTime(DateTime endTime) => this.endTime = endTime;

  public DateTime StartTime => this.startTime;

  public DateTime EndTime => this.endTime;

  public string DescriptionVersion => this.descriptionVersion;

  public ConnectionResource Resource => this.resource;

  public Channel Channel => this.channel;

  public bool? IsFixedVariant => this.isFixedVariant;

  public ChannelOptions ChannelOptions => this.channelOptions;

  public string VariantName => this.variantName;

  public int CompareTo(object obj) => this.startTime.CompareTo(((Session) obj).StartTime);

  public override bool Equals(object obj) => base.Equals(obj);

  public override int GetHashCode() => base.GetHashCode();

  public static bool operator ==(Session object1, Session object2)
  {
    return object.Equals((object) object1, (object) object2);
  }

  public static bool operator !=(Session object1, Session object2)
  {
    return !object.Equals((object) object1, (object) object2);
  }

  public static bool operator <(Session object1, Session object2)
  {
    if (object1 == (Session) null)
      throw new ArgumentNullException(nameof (object1));
    return object1.CompareTo((object) object2) < 0;
  }

  public static bool operator >(Session object1, Session object2)
  {
    if (object1 == (Session) null)
      throw new ArgumentNullException(nameof (object1));
    return object1.CompareTo((object) object2) > 0;
  }
}
