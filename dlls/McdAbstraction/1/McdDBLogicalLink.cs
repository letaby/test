// Decompiled with JetBrains decompiler
// Type: McdAbstraction.McdDBLogicalLink
// Assembly: McdAbstraction, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 2CF84A4E-9C9E-4158-9C67-2CE39889DD31
// Assembly location: C:\Users\petra\Downloads\Архив (2)\McdAbstraction.dll

using Softing.Dts;

#nullable disable
namespace McdAbstraction;

public class McdDBLogicalLink
{
  private MCDDbLogicalLink logicalLinkInfo;
  private McdDBLocation protocolLocation;
  private McdDBLocation logicalLinkLocation;

  internal McdDBLogicalLink(MCDDbLogicalLink logicalLinkInfo)
  {
    this.logicalLinkInfo = logicalLinkInfo;
    this.Qualifier = logicalLinkInfo.ShortName;
    this.Name = logicalLinkInfo.LongName;
    this.ProtocolType = logicalLinkInfo.ProtocolType;
    if (this.logicalLinkInfo.DbLocation.Type != MCDLocationType.eECU_BASE_VARIANT)
      return;
    this.EcuQualifier = this.logicalLinkInfo.DbLocation.DbECU.ShortName;
  }

  public string Qualifier { get; private set; }

  public string Name { get; private set; }

  public string ProtocolType { get; private set; }

  public string EcuQualifier { get; private set; }

  public McdDBLocation ProtocolLocation
  {
    get
    {
      if (this.protocolLocation == null)
        this.protocolLocation = new McdDBLocation(((DtsDbLocation) this.logicalLinkInfo.DbLocation).ProtocolLocation);
      return this.protocolLocation;
    }
  }

  public McdDBLocation LogicalLinkLocation
  {
    get
    {
      if (this.logicalLinkLocation == null)
        this.logicalLinkLocation = new McdDBLocation(this.logicalLinkInfo.DbLocation);
      return this.logicalLinkLocation;
    }
  }
}
