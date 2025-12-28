using System.Collections.Generic;
using System.Linq;
using Softing.Dts;

namespace McdAbstraction;

public class McdDBEcuMem
{
	private MCDDbEcuMem ecumem;

	private string[] ecus;

	private IEnumerable<McdDBFlashSession> flashSessions;

	public string Qualifier { get; private set; }

	public string Name { get; private set; }

	public string Description { get; private set; }

	public IEnumerable<string> Ecus
	{
		get
		{
			if (ecus == null)
			{
				ecus = (from e in ecumem.BaseVariants.OfType<MCDDbEcuBaseVariant>()
					select e.ShortName).ToArray();
				if (!ecus.Any())
				{
					ecus = DBFlashSessions.SelectMany((McdDBFlashSession session) => session.LayerReferences.Select((string s) => s.Split(".".ToCharArray()).Last())).ToArray();
				}
			}
			return ecus;
		}
	}

	public IEnumerable<McdDBFlashSession> DBFlashSessions
	{
		get
		{
			if (flashSessions == null)
			{
				flashSessions = (from fs in ecumem.FlashSessions.OfType<MCDDbFlashSession>()
					select new McdDBFlashSession(fs)).ToList();
			}
			return flashSessions;
		}
	}

	internal McdDBEcuMem(MCDDbEcuMem ecumem)
	{
		this.ecumem = ecumem;
		Qualifier = ecumem.ShortName;
		Name = ecumem.LongName;
		Description = ecumem.Description;
	}
}
