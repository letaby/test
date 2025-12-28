using System.Collections.Generic;
using DetroitDiesel.Net;

namespace DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming;

public interface IProvideProgrammingData
{
	IEnumerable<ProgrammingData> ProgrammingData { get; set; }

	UnitInformation SelectedUnit { get; set; }

	bool RequiresCompatibilityChecks { get; }
}
