using System.Collections.Generic;
using System.Linq;
using Softing.Dts;

namespace McdAbstraction;

public class McdDBEcuVariant
{
	private readonly MCDDbEcuVariant variant;

	private Dictionary<string, McdDBLocation> dblocations = new Dictionary<string, McdDBLocation>();

	public string Name { get; private set; }

	public string Description { get; private set; }

	public IEnumerable<string> DBLocationNames => variant.DbLocations.Names;

	internal McdDBEcuVariant(MCDDbEcuVariant variant)
	{
		this.variant = variant;
		Name = this.variant.LongName;
		Description = this.variant.Description;
	}

	public McdDBLocation GetDBLocationForProtocol(string protocol)
	{
		if (!dblocations.TryGetValue(protocol, out var value) || value == null)
		{
			MCDDbLocation mCDDbLocation = variant.DbLocations.OfType<MCDDbLocation>().FirstOrDefault((MCDDbLocation l) => l.AccessKey.Protocol == protocol);
			if (mCDDbLocation != null)
			{
				value = (dblocations[protocol] = new McdDBLocation(mCDDbLocation));
			}
		}
		return value;
	}
}
