using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDResponseParameters : MCDNamedCollection, MCDCollection, MCDObject, IDisposable, IEnumerable
{
	MCDObject Parent { get; }

	List<MCDResponseParameter> ToList();

	MCDResponseParameter[] ToArray();

	MCDResponseParameter GetItemByIndex(uint index);

	MCDResponseParameter GetItemByName(string name);

	MCDResponseParameter AddElement();

	MCDResponseParameter AddElementWithContent(MCDResponseParameter pPattern);

	MCDResponseParameter AddMuxBranch(string strBranch);

	MCDResponseParameter AddMuxBranchByIndex(byte Index);

	MCDResponseParameter AddMuxBranchByIndexWithContent(byte Index, MCDResponseParameters pContentList);

	MCDResponseParameter AddMuxBranchWithContent(string strBranch, MCDResponseParameters pContentList);

	MCDResponseParameter SetParameterWithName(string name, MCDValue value);

	MCDResponseParameter AddMuxBranchByMuxValue(MCDValue muxValue);

	MCDResponseParameter AddMuxBranchByMuxValueWithContent(MCDValue muxValue, MCDResponseParameters pContentList);

	MCDResponseParameter AddEnvDataByDTC(uint dtc);
}
