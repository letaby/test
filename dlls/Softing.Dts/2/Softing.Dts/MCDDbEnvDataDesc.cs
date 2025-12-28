using System;

namespace Softing.Dts;

public interface MCDDbEnvDataDesc : MCDDbObject, MCDNamedObject, MCDObject, IDisposable
{
	MCDDbResponseParameters CommonDbEnvDatas { get; }

	MCDDbResponseParameters GetCompleteDbEnvDatasByDiagTroubleCode(uint troublecode);

	MCDDbResponseParameters GetSpecificDbEnvDatasForDiagTroubleCode(uint troublecode);
}
