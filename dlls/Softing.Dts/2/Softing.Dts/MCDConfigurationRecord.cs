using System;

namespace Softing.Dts;

public interface MCDConfigurationRecord : MCDNamedObject, MCDObject, IDisposable
{
	MCDConfigurationIdItem ConfigurationIdItem { get; }

	MCDDbConfigurationRecord DbObject { get; }

	MCDError Error { get; }

	MCDErrors Errors { get; }

	MCDOptionItems OptionItems { get; }

	MCDReadDiagComPrimitives ReadDiagComPrimitives { get; }

	MCDSystemItems SystemItems { get; }

	MCDWriteDiagComPrimitives WriteDiagComPrimitives { get; }

	bool HasError { get; }

	MCDDataIdItem DataIdItem { get; }

	string ActiveFileName { get; }

	byte[] ConfigurationRecord { get; set; }

	string[] MatchingFileNames { get; }

	MCDDbDataRecord ConfigurationRecordByDbObject { set; }

	void LoadCodingData(string filename);

	void RemoveReadDiagComPrimitives(MCDReadDiagComPrimitives readDiagComs);

	void RemoveWriteDiagComPrimitives(MCDWriteDiagComPrimitives writeDiagComs);
}
