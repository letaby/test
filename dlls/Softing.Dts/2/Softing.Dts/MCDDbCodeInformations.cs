using System;
using System.Collections;
using System.Collections.Generic;

namespace Softing.Dts;

public interface MCDDbCodeInformations : MCDCollection, MCDObject, IDisposable, IEnumerable
{
	List<MCDDbCodeInformation> ToList();

	MCDDbCodeInformation[] ToArray();

	MCDDbCodeInformation GetItemByIndex(uint uIndex);

	MCDDbCodeInformation GetItemByName(string name);
}
