namespace DetroitDiesel.ServiceRoutineTabs.Procedures.Air_Mass_Adaptation.panel;

internal class ContinueMessageDialog
{
	private readonly string message;

	private readonly string labelYes;

	private readonly string labelNo;

	public string Message => message;

	public string LabelYes => labelYes;

	public string LabelNo => labelNo;

	public ContinueMessageDialog(string message, string labelYes, string labelNo)
	{
		this.message = message;
		this.labelYes = labelYes;
		this.labelNo = labelNo;
	}
}
