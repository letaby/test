using System.Collections.Generic;
using System.Linq;
using Softing.Dts;

namespace McdAbstraction;

public class McdDBEcuBaseVariant
{
	private MCDDbEcuBaseVariant baseVariant;

	private Dictionary<string, McdDBEcuVariant> variants;

	private Dictionary<string, McdDBLocation> dblocations = new Dictionary<string, McdDBLocation>();

	public string Description { get; private set; }

	public string DatabaseFile { get; private set; }

	public string Preamble => McdRoot.GetPreamble(DatabaseFile);

	public IEnumerable<string> DBEcuVariantNames => variants.Keys.ToList();

	public IEnumerable<string> DBLocationNames => baseVariant.DbLocations.Names;

	internal McdDBEcuBaseVariant(MCDDbEcuBaseVariant baseVariant)
	{
		this.baseVariant = baseVariant;
		Description = this.baseVariant.Description;
		DatabaseFile = ((DtsDbEcuBaseVariant)this.baseVariant).DatabaseFile;
		variants = this.baseVariant.DbEcuVariants.Names.Select((string n) => new KeyValuePair<string, McdDBEcuVariant>(n, null)).ToDictionary((KeyValuePair<string, McdDBEcuVariant> k) => k.Key, (KeyValuePair<string, McdDBEcuVariant> v) => v.Value);
	}

	public McdDBEcuVariant GetDBEcuVariant(string name)
	{
		if (!variants.TryGetValue(name, out var value) || value == null)
		{
			MCDDbEcuVariant itemByName = baseVariant.DbEcuVariants.GetItemByName(name);
			if (itemByName != null)
			{
				value = (variants[name] = new McdDBEcuVariant(itemByName));
			}
		}
		return value;
	}

	public McdDBLocation GetDBLocationForProtocol(string protocol)
	{
		if (!dblocations.TryGetValue(protocol, out var value) || value == null)
		{
			MCDDbLocation mCDDbLocation = baseVariant.DbLocations.OfType<MCDDbLocation>().FirstOrDefault((MCDDbLocation l) => l.AccessKey.Protocol == protocol);
			if (mCDDbLocation != null)
			{
				value = (dblocations[protocol] = new McdDBLocation(mCDDbLocation));
			}
		}
		return value;
	}
}
