// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.DataHub.ChangeDataPagePasswordRequestEventArgs
// Assembly: DataHub, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: 89346980-C6E7-48B1-88DD-CF29796E810E
// Assembly location: C:\Users\petra\Downloads\Архив (2)\DataHub.dll

using System;

#nullable disable
namespace DetroitDiesel.DataHub;

public class ChangeDataPagePasswordRequestEventArgs : EventArgs
{
  public ChangePasswordResult Result { get; private set; }

  internal ChangeDataPagePasswordRequestEventArgs(ChangePasswordResult result)
  {
    this.Result = result;
  }
}
