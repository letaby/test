using System.Collections.Generic;
using System.Linq;
using Softing.Dts;

namespace McdAbstraction;

public class McdDBFlashSession
{
	private MCDDbFlashSession session;

	private string qualifier;

	private string name;

	private string description;

	private string flashJobName;

	private uint priority;

	private string key;

	private string[] layerReferences;

	private List<McdDBFlashDataBlock> dataBlocks;

	public string Qualifier => qualifier;

	public string Name => name;

	public string Description => description;

	public string FlashJobName => flashJobName;

	public long Priority => priority;

	public string FlashKey => key;

	public IEnumerable<string> LayerReferences
	{
		get
		{
			if (layerReferences == null)
			{
				layerReferences = ((DtsDbFlashSession)session).LayerReferences;
			}
			return layerReferences;
		}
	}

	public IEnumerable<McdDBFlashDataBlock> DBDataBlocks
	{
		get
		{
			if (dataBlocks == null)
			{
				dataBlocks = (from fs in session.DbDataBlocks.OfType<MCDDbFlashDataBlock>()
					select new McdDBFlashDataBlock(fs)).ToList();
			}
			return dataBlocks;
		}
	}

	internal McdDBFlashSession(MCDDbFlashSession session)
	{
		qualifier = session.ShortName;
		name = session.LongName;
		description = session.Description;
		flashJobName = ((DtsDbFlashSession)session).FlashJobName;
		priority = session.Priority;
		key = session.FlashKey;
		this.session = session;
	}
}
