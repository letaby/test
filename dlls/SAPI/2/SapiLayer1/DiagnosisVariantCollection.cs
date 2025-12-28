using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SapiLayer1;

public sealed class DiagnosisVariantCollection : ReadOnlyCollection<DiagnosisVariant>
{
	public DiagnosisVariant this[string name] => this.FirstOrDefault((DiagnosisVariant item) => string.Equals(item.Name, name, StringComparison.Ordinal));

	public DiagnosisVariant Base => base[0];

	internal DiagnosisVariantCollection(Ecu e)
		: base((IList<DiagnosisVariant>)new List<DiagnosisVariant>())
	{
		if (!e.IsRollCall && !e.IsByteMessaging && !e.IsMcd)
		{
			DiagnosisVariant diagnosisVariant = new DiagnosisVariant(e, "_base_", string.Empty, isBase: true);
			Add(diagnosisVariant);
		}
	}

	internal void Add(DiagnosisVariant diagnosisVariant)
	{
		base.Items.Add(diagnosisVariant);
	}
}
