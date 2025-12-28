// Decompiled with JetBrains decompiler
// Type: SapiLayer1.FlashSecurity
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using CaesarAbstraction;
using McdAbstraction;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#nullable disable
namespace SapiLayer1;

public sealed class FlashSecurity
{
  private FlashDataBlock flashDataBlock;
  private string securityMethod;
  private string ecuKey;
  private string validity;
  private Dump fwSignature;
  private Dump checksum;

  internal FlashSecurity(FlashDataBlock fdb) => this.flashDataBlock = fdb;

  internal void Acquire(CaesarDICffSecur flashSecurityBlock)
  {
    IEnumerable<byte> securityMethod = (IEnumerable<byte>) flashSecurityBlock.GetSecurityMethod();
    IEnumerable<byte> firmwareSignature = (IEnumerable<byte>) flashSecurityBlock.GetFirmwareSignature();
    IEnumerable<byte> checksum = (IEnumerable<byte>) flashSecurityBlock.GetChecksum();
    IEnumerable<byte> ecuKey = (IEnumerable<byte>) flashSecurityBlock.GetEcuKey();
    this.checksum = checksum == null ? (Dump) null : new Dump(checksum);
    this.fwSignature = firmwareSignature == null ? (Dump) null : new Dump(firmwareSignature);
    this.securityMethod = securityMethod == null ? (string) null : Encoding.UTF8.GetString(securityMethod.ToArray<byte>());
    this.ecuKey = ecuKey == null ? (string) null : Encoding.UTF8.GetString(ecuKey.ToArray<byte>());
  }

  internal void Acquire(McdDBFlashSecurity flashSecurityBLock)
  {
    IEnumerable<byte> flashwareSignature = flashSecurityBLock.FlashwareSignature;
    IEnumerable<byte> flashwareChecksum = flashSecurityBLock.FlashwareChecksum;
    this.checksum = flashwareChecksum == null ? (Dump) null : new Dump(flashwareChecksum);
    this.fwSignature = flashwareSignature == null ? (Dump) null : new Dump(flashwareSignature);
    this.securityMethod = flashSecurityBLock.SecurityMethod;
    this.validity = flashSecurityBLock.Validity;
  }

  public FlashDataBlock FlashDataBlock => this.flashDataBlock;

  public string SecurityMethod => this.securityMethod;

  public Dump FirmwareSignature => this.fwSignature;

  public Dump Checksum => this.checksum;

  public string EcuKey => this.ecuKey;

  public string Validity => this.validity;
}
