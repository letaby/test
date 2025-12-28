// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.DataHub.ExtractionCompleteEventArgs
// Assembly: DataHub, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: 89346980-C6E7-48B1-88DD-CF29796E810E
// Assembly location: C:\Users\petra\Downloads\Архив (2)\DataHub.dll

using SapiLayer1;
using System;

#nullable disable
namespace DetroitDiesel.DataHub;

public class ExtractionCompleteEventArgs : EventArgs
{
  public XtrFile XtrFile { get; private set; }

  public Channel Channel { get; private set; }

  public bool Succeeded { get; private set; }

  public ExtractionCompleteEventArgs(bool succeeded, Channel channel, XtrFile xtrFile)
  {
    this.Succeeded = succeeded;
    this.Channel = channel;
    this.XtrFile = xtrFile;
  }
}
