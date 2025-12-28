// Decompiled with JetBrains decompiler
// Type: McdAbstraction.McdDBService
// Assembly: McdAbstraction, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 2CF84A4E-9C9E-4158-9C67-2CE39889DD31
// Assembly location: C:\Users\petra\Downloads\Архив (2)\McdAbstraction.dll

using Softing.Dts;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace McdAbstraction;

public class McdDBService : McdDBDiagComPrimitive
{
  private MCDDbService service;
  private IEnumerable<byte> requestMessage;
  private Dictionary<string, string> specialData;

  internal McdDBService(MCDDbService service)
    : base((MCDDbDiagComPrimitive) service)
  {
    this.service = service;
  }

  public override IEnumerable<byte> RequestMessage
  {
    get
    {
      if (this.requestMessage == null && this.DefaultPdu != null)
      {
        long length = this.AllRequestParameters.Max<McdDBRequestParameter>((Func<McdDBRequestParameter, long>) (rp => rp.BytePos + rp.ByteLength));
        if ((long) this.DefaultPdu.Count<byte>() < length)
        {
          byte[] numArray = new byte[length];
          this.DefaultPdu.ToArray<byte>().CopyTo((Array) numArray, 0);
          this.requestMessage = (IEnumerable<byte>) numArray;
        }
        else
          this.requestMessage = this.DefaultPdu;
      }
      return this.requestMessage;
    }
  }

  public Dictionary<string, string> SpecialData
  {
    get
    {
      if (this.specialData == null)
        this.specialData = McdDBDiagComPrimitive.GetSpecialData(this.service.DbSDGs);
      return this.specialData;
    }
  }
}
