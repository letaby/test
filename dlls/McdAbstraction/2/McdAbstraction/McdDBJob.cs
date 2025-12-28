using System.Collections.Generic;
using Softing.Dts;

namespace McdAbstraction;

public class McdDBJob : McdDBDiagComPrimitive
{
	private MCDDbJob job;

	private Dictionary<string, string> specialData;

	public Dictionary<string, string> SpecialData
	{
		get
		{
			if (specialData == null)
			{
				specialData = McdDBDiagComPrimitive.GetSpecialData(job.DbSDGs);
			}
			return specialData;
		}
	}

	public bool IsFlashJob => job.ObjectType == MCDObjectType.eMCDDBFLASHJOB;

	internal McdDBJob(MCDDbJob job)
		: base(job)
	{
		this.job = job;
	}
}
