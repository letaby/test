using System.Globalization;
using System.IO;
using DetroitDiesel.Common;
using DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties;

namespace DetroitDiesel.Windows.Forms.Diagnostics.Container;

public class CommandLineArguments
{
	private string message;

	private string logFilePath;

	public string Message => message;

	public string LogFilePath => logFilePath;

	public bool IsUninstall { get; private set; }

	public bool ProcessArguments(string[] arguments)
	{
		bool flag = arguments.Length <= 1;
		if (flag)
		{
			foreach (string text in arguments)
			{
				if (text == "uninstall")
				{
					IsUninstall = true;
				}
				else if (File.Exists(text))
				{
					if (string.Compare(Path.GetExtension(text), ".DrumrollLog", CultureInfo.CurrentCulture, CompareOptions.IgnoreCase) == 0)
					{
						logFilePath = text;
						continue;
					}
					message = string.Format(CultureInfo.CurrentCulture, Resources.CommandLineArgInvalidFileType, ApplicationInformation.ApplicationName, text);
					flag = false;
				}
				else
				{
					message = "File not found.";
					flag = false;
				}
			}
		}
		else
		{
			message = "Too many command line arguments.";
		}
		return flag;
	}
}
