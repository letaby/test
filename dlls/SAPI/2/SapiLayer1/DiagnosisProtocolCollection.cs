using System;
using System.Linq;
using CaesarAbstraction;
using McdAbstraction;

namespace SapiLayer1;

public sealed class DiagnosisProtocolCollection : LateLoadReadOnlyCollection<DiagnosisProtocol>
{
	public DiagnosisProtocol this[string name] => this.FirstOrDefault((DiagnosisProtocol item) => string.Equals(item.Name, name, StringComparison.Ordinal));

	internal DiagnosisProtocolCollection()
	{
	}

	protected override void AcquireList()
	{
		base.Items.Clear();
		foreach (string protocol in CaesarRoot.Protocols)
		{
			base.Items.Add(new DiagnosisProtocol(protocol));
		}
		foreach (string dBProtocolLocationName in McdRoot.DBProtocolLocationNames)
		{
			base.Items.Add(new DiagnosisProtocol(dBProtocolLocationName, isMcd: true));
		}
	}

	internal void ClearList()
	{
		base.Items.Clear();
		ResetList();
	}
}
