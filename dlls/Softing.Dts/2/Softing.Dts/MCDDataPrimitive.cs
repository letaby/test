using System;

namespace Softing.Dts;

public interface MCDDataPrimitive : MCDDiagComPrimitive, MCDObject, IDisposable
{
	ushort ResultBufferSize { get; set; }

	MCDRepetitionState RepetitionState { get; }

	ushort RepetitionTime { get; set; }

	MCDResultState ResultState { get; }

	void StartRepetition();

	void StopRepetition();

	void UpdateRepetitionParameters();

	void ExecuteAsync();

	MCDResults FetchResults(int numReq);
}
