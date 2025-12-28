// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.DataHub.ExtractionProgressEventArgs
// Assembly: DataHub, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: 89346980-C6E7-48B1-88DD-CF29796E810E
// Assembly location: C:\Users\petra\Downloads\Архив (2)\DataHub.dll

using SapiLayer1;
using System;

#nullable disable
namespace DetroitDiesel.DataHub;

public class ExtractionProgressEventArgs : EventArgs
{
  public double Percent { get; private set; }

  public Channel Channel { get; private set; }

  public ExtractionProgressEventArgs(Channel channel, double percent)
  {
    this.Channel = channel;
    this.Percent = percent;
  }
}
