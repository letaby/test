using System.Windows.Forms;
using DetroitDiesel.Common;
using SapiLayer1;

namespace DetroitDiesel.ServiceRoutineTabs.Tests.Cylinder_Cutout_Test__Manual___EPA10_.panel;

internal static class WarningManager
{
	private static bool warningRequired = true;

	private static string warningFormat = Resources.Message_CAUTIONToAvoidPersonalInjuryPlaceTransmissionInPARKOrNEUTRALAndApplyParkingBrake + "\r\n" + Resources.MessageFormat_ContinueWith0;

	private static string message;

	private static string Message
	{
		get
		{
			if (string.IsNullOrEmpty(message))
			{
				SetJobName(string.Empty);
			}
			return message;
		}
	}

	public static void SetJobName(string jobName)
	{
		if (string.IsNullOrEmpty(jobName))
		{
			message = string.Format(warningFormat, "test");
		}
		else
		{
			message = string.Format(warningFormat, jobName);
		}
	}

	public static void Reset()
	{
		warningRequired = true;
	}

	public static bool RequestContinue()
	{
		bool result = false;
		if (!warningRequired || DialogResult.Yes == MessageBox.Show(Message, ApplicationInformation.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2))
		{
			result = true;
			warningRequired = false;
			if (Sapi.GetSapi() != null && Sapi.GetSapi().LogFiles.Logging)
			{
				SapiExtensions.LabelLogWithPrefix(Sapi.GetSapi().LogFiles, "Cylinder Cutout (Manual)", Resources.Message_UserAcknowledgedCaution);
			}
		}
		return result;
	}
}
