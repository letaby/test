using DetroitDiesel.Common;

namespace DetroitDiesel.ServiceRoutineTabs.Dialogs.SCR_Replacement__EPA10_.panel;

internal static class Log
{
	public static void AddEvent(string eventText)
	{
		if (SapiManager.GlobalInstance != null && SapiManager.GlobalInstance.Sapi != null && !string.IsNullOrEmpty(eventText))
		{
			SapiManager.GlobalInstance.Sapi.LogFiles.LabelLog(eventText.Trim());
		}
	}
}
