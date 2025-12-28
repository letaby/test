using Softing.Dts;

namespace McdAbstraction;

public class McdDBDiagTroubleCode
{
	private MCDDbDiagTroubleCode code;

	public string DisplayTroubleCode { get; private set; }

	public string Text { get; private set; }

	public long TroubleCode { get; private set; }

	internal McdDBDiagTroubleCode(MCDDbDiagTroubleCode code)
	{
		this.code = code;
		DisplayTroubleCode = this.code.DisplayTroubleCode;
		Text = this.code.DTCText;
		TroubleCode = this.code.TroubleCode;
	}
}
