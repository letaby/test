using Softing.Dts;

namespace McdAbstraction;

public class McdDBLogicalLink
{
	private MCDDbLogicalLink logicalLinkInfo;

	private McdDBLocation protocolLocation;

	private McdDBLocation logicalLinkLocation;

	public string Qualifier { get; private set; }

	public string Name { get; private set; }

	public string ProtocolType { get; private set; }

	public string EcuQualifier { get; private set; }

	public McdDBLocation ProtocolLocation
	{
		get
		{
			if (protocolLocation == null)
			{
				protocolLocation = new McdDBLocation(((DtsDbLocation)logicalLinkInfo.DbLocation).ProtocolLocation);
			}
			return protocolLocation;
		}
	}

	public McdDBLocation LogicalLinkLocation
	{
		get
		{
			if (logicalLinkLocation == null)
			{
				logicalLinkLocation = new McdDBLocation(logicalLinkInfo.DbLocation);
			}
			return logicalLinkLocation;
		}
	}

	internal McdDBLogicalLink(MCDDbLogicalLink logicalLinkInfo)
	{
		this.logicalLinkInfo = logicalLinkInfo;
		Qualifier = logicalLinkInfo.ShortName;
		Name = logicalLinkInfo.LongName;
		ProtocolType = logicalLinkInfo.ProtocolType;
		if (this.logicalLinkInfo.DbLocation.Type == MCDLocationType.eECU_BASE_VARIANT)
		{
			EcuQualifier = this.logicalLinkInfo.DbLocation.DbECU.ShortName;
		}
	}
}
