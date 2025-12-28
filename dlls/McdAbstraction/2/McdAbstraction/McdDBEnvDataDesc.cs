using System.Collections.Generic;
using System.Linq;
using Softing.Dts;

namespace McdAbstraction;

public class McdDBEnvDataDesc
{
	private MCDDbEnvDataDesc env;

	public string Name { get; private set; }

	public string Qualifier { get; private set; }

	public IEnumerable<McdDBResponseParameter> CommonEnvironmentalDataSet => (from e in env.CommonDbEnvDatas.OfType<MCDDbResponseParameter>()
		select new McdDBResponseParameter(e)).ToList();

	internal McdDBEnvDataDesc(MCDDbEnvDataDesc env)
	{
		this.env = env;
		Name = this.env.LongName;
		Qualifier = this.env.ShortName;
	}

	public IEnumerable<McdDBResponseParameter> GetFaultSpecificEnvironmentalDataSet(long code)
	{
		return (from e in env.GetSpecificDbEnvDatasForDiagTroubleCode((uint)code).OfType<MCDDbResponseParameter>()
			select new McdDBResponseParameter(e)).ToList();
	}
}
