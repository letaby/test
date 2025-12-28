// Decompiled with JetBrains decompiler
// Type: SapiLayer1.ByteMessage
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using CaesarAbstraction;
using McdAbstraction;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace SapiLayer1;

public sealed class ByteMessage
{
  private Channel channel;
  private Dump request;
  private Dump response;
  private Dump requiredResponse;

  internal ByteMessage(Channel channel, Dump request)
  {
    this.channel = channel;
    this.request = request;
  }

  internal ByteMessage(Channel channel, Dump request, Dump requiredResponse)
  {
    this.channel = channel;
    this.request = request;
    this.requiredResponse = requiredResponse;
  }

  internal void InternalDoMessage() => this.InternalDoMessage(false);

  internal void InternalDoMessage(bool internalRequest)
  {
    CaesarException e = (CaesarException) null;
    if (this.channel.ChannelHandle != null)
    {
      try
      {
        this.response = new Dump((IEnumerable<byte>) this.channel.ChannelHandle.CCDoMessage(this.request.Data.ToArray<byte>()));
      }
      catch (CaesarErrorException ex)
      {
        byte? negativeResponseCode = new byte?();
        e = new CaesarException(ex, negativeResponseCode);
      }
    }
    else if (this.channel.McdChannelHandle != null)
    {
      try
      {
        McdDiagComPrimitive hexService = this.channel.McdChannelHandle.GetHexService();
        hexService.RequestMessage = (IEnumerable<byte>) this.request.Data.ToArray<byte>();
        hexService.Execute(0);
        this.response = new Dump(hexService.ResponseMessage);
      }
      catch (McdException ex)
      {
        e = new CaesarException(ex);
      }
    }
    else if (this.channel.Ecu.RollCallManager != null)
    {
      try
      {
        this.response = new Dump((IEnumerable<byte>) this.channel.Ecu.RollCallManager.DoByteMessage(this.channel, this.request.Data.ToArray<byte>(), this.requiredResponse != null ? this.requiredResponse.Data.ToArray<byte>() : (byte[]) null));
      }
      catch (CaesarException ex)
      {
        e = ex;
      }
    }
    if (internalRequest)
      return;
    this.channel.RaiseByteMessageComplete(this, (Exception) e);
  }

  public Channel Channel => this.channel;

  public Dump Request => this.request;

  public Dump Response => this.response;

  public Dump RequiredResponse => this.requiredResponse;
}
