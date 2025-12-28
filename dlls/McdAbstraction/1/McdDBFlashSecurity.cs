// Decompiled with JetBrains decompiler
// Type: McdAbstraction.McdDBFlashSecurity
// Assembly: McdAbstraction, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 2CF84A4E-9C9E-4158-9C67-2CE39889DD31
// Assembly location: C:\Users\petra\Downloads\Архив (2)\McdAbstraction.dll

using Softing.Dts;
using System.Collections.Generic;

#nullable disable
namespace McdAbstraction;

public class McdDBFlashSecurity
{
  private string securityMethod;
  private string validity;
  private IEnumerable<byte> flashwareSignature;
  private IEnumerable<byte> flashwareChecksum;

  internal McdDBFlashSecurity(MCDDbFlashSecurity security)
  {
    this.securityMethod = security.SecurityMethod.DataType == MCDDataType.eNO_TYPE ? (string) null : security.SecurityMethod.ValueAsString;
    this.validity = security.Validity.DataType == MCDDataType.eNO_TYPE ? (string) null : security.Validity.ValueAsString;
    this.flashwareSignature = security.FlashwareSignature.DataType == MCDDataType.eA_BYTEFIELD ? (IEnumerable<byte>) security.FlashwareSignature.Bytefield : (IEnumerable<byte>) (byte[]) null;
    this.flashwareChecksum = security.FlashwareChecksum.DataType == MCDDataType.eA_BYTEFIELD ? (IEnumerable<byte>) security.FlashwareChecksum.Bytefield : (IEnumerable<byte>) (byte[]) null;
  }

  public string SecurityMethod => this.securityMethod;

  public string Validity => this.validity;

  public IEnumerable<byte> FlashwareSignature => this.flashwareSignature;

  public IEnumerable<byte> FlashwareChecksum => this.flashwareChecksum;
}
