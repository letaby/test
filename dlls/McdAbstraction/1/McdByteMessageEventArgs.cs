// Decompiled with JetBrains decompiler
// Type: McdAbstraction.McdByteMessageEventArgs
// Assembly: McdAbstraction, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 2CF84A4E-9C9E-4158-9C67-2CE39889DD31
// Assembly location: C:\Users\petra\Downloads\Архив (2)\McdAbstraction.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace McdAbstraction;

public class McdByteMessageEventArgs : EventArgs
{
  public McdByteMessageEventArgs(IEnumerable<byte> byteMessage, bool isSend)
  {
    this.Message = byteMessage;
    this.IsSend = isSend;
  }

  public IEnumerable<byte> Message { private set; get; }

  public bool IsSend { private set; get; }
}
