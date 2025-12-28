using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;
using DetroitDiesel.Common;
using DetroitDiesel.CrashHandling;
using DetroitDiesel.Extensions;
using DetroitDiesel.Interfaces;
using DetroitDiesel.Net;
using DetroitDiesel.Settings;
using DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties;
using DetroitDiesel.Windows.Forms.Themed;
using Microsoft.Win32;
using SapiLayer1;

namespace DetroitDiesel.Windows.Forms.Diagnostics.Container;

internal static class Program
{
	private static class NativeMethods
	{
		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		private delegate int ConfigureFunc();

		public enum SidConfigureResult
		{
			CouldNotExecute = -2,
			NoDrivers,
			Success,
			Canceled
		}

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
		private static extern IntPtr LoadLibrary(string path);

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool FreeLibrary(IntPtr hModule);

		[DllImport("kernel32", BestFitMapping = false, CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true, ThrowOnUnmappableChar = true)]
		private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

		public static SidConfigureResult ExecuteSidConfigure(string dllPath)
		{
			SidConfigureResult result = SidConfigureResult.CouldNotExecute;
			IntPtr intPtr = LoadLibrary(dllPath);
			if (intPtr != IntPtr.Zero)
			{
				IntPtr procAddress = GetProcAddress(intPtr, "Configure");
				if (procAddress != IntPtr.Zero)
				{
					ConfigureFunc configureFunc = (ConfigureFunc)Marshal.GetDelegateForFunctionPointer(procAddress, typeof(ConfigureFunc));
					result = (SidConfigureResult)configureFunc();
				}
				FreeLibrary(intPtr);
			}
			return result;
		}
	}

	private enum ExitCodes
	{
		ErrorSuccess = 0,
		ErrorDsInsuffAccessRights = 8344
	}

	private static SplashScreen splashScreen;

	[STAThread]
	private static void Main(string[] args)
	{
		CrashHandler.Initialize();
		Directory.SetCurrentDirectory(Path.GetDirectoryName(Application.ExecutablePath));
		bool flag = args.Length != 0 && args[0].Equals("removeLegacySidRegistryKey", StringComparison.OrdinalIgnoreCase);
		if (ApplicationForceUpgrade.ProductUpgradeRequired && ApplicationForceUpgrade.ForceUpgrade())
		{
			ShowErrorMessage(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_RequiresUpgradetoLatestRelease, ApplicationInformation.ProductName, ApplicationForceUpgrade.RequiredUpgradeVersion));
		}
		else if (RemoveLegacySidPassthruEntry(flag) && !flag)
		{
			RunApplication(args);
		}
	}

	private static void RunApplication(string[] args)
	{
		//IL_0272: Expected O, but got Unknown
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Expected O, but got Unknown
		CommandLineArguments commandLineArguments = new CommandLineArguments();
		if (!commandLineArguments.ProcessArguments(args))
		{
			ShowErrorMessage(commandLineArguments.Message);
			return;
		}
		Application.SetCompatibleTextRenderingDefault(defaultValue: false);
		if (!commandLineArguments.IsUninstall)
		{
			RunSidConfigure(checkIniFile: true);
		}
		Application.EnableVisualStyles();
		Application.DoEvents();
		AssemblyName name = Assembly.LoadFrom(Path.Combine(Path.GetDirectoryName(Assembly.GetAssembly(typeof(Program)).Location), "Common.dll")).GetName();
		AssemblyName name2 = Assembly.GetExecutingAssembly().GetName();
		if (name.Version != name2.Version)
		{
			ShowErrorMessage(string.Format(CultureInfo.CurrentCulture, Resources.Program_FormatInconsistentInstallation, ApplicationInformation.ProductName, name2.Name, name2.Version, name.Name, name.Version));
			return;
		}
		LanguageManager.ApplyApplicationLocale();
		TraceLogManager.LoadSettings((ISettings)(object)SettingsManager.GlobalInstance);
		TraceLogManager.Start();
		ServerDataManager globalInstance = ServerDataManager.GlobalInstance;
		globalInstance.Load();
		globalInstance.AddCrashHandlerInfo();
		if (!commandLineArguments.IsUninstall)
		{
			splashScreen = new SplashScreen();
			((Control)(object)splashScreen).Show();
			Application.DoEvents();
			ToolStripManager.RenderMode = ((Environment.OSVersion.Version.Major < 6) ? ToolStripManagerRenderMode.System : ToolStripManagerRenderMode.Professional);
			if (HasRequiredMinimumDotNet())
			{
				try
				{
					ExtensionLibrary.UpdateExtensions((IProgressBar)(object)splashScreen);
					splashScreen.OverallStatusText = Resources.StatusPreparingCommunications;
					SapiManager.GlobalInstance.ResetSapi();
					if (SapiManager.GlobalInstance.Sapi == null)
					{
						throw new InvalidOperationException();
					}
					SapiManager.GlobalInstance.ProgressBar = (IProgressBar)(object)splashScreen;
					splashScreen.OverallStatusText = Resources.StatusPreparingDiagnosticData;
					SapiManager.GlobalInstance.LoadSettings((ISettings)(object)SettingsManager.GlobalInstance);
					SapiManager.GlobalInstance.ProgressBar = null;
					bool flag = false;
					string logFileToLoad = string.Empty;
					if (ValidLicense())
					{
						if (!PrintHelper.Initialize())
						{
							ShowErrorMessage(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_RequiresInternetExplorer11OrLater, Application.ProductName));
						}
						else
						{
							flag = true;
							logFileToLoad = commandLineArguments.LogFilePath;
						}
					}
					if (flag)
					{
						MoveLegacyFiles();
						MainForm mainForm = new MainForm(splashScreen, logFileToLoad);
						using EventWaitHandle eventWaitHandle = new EventWaitHandle(initialState: false, EventResetMode.AutoReset);
						mainForm.Shown += OnMainFormShown;
						Application.Run(mainForm);
						mainForm.Shown -= OnMainFormShown;
						eventWaitHandle.Set();
					}
					else
					{
						Application.Exit();
					}
				}
				catch (CompilerVersionNotFoundException)
				{
					ShowErrorMessage(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_RequiresNETFramework4OrLaterPleaseUpgradeYourInstallationOfNETFrameworkAndThenRestart0, Application.ProductName));
				}
				catch (FileInUseException ex2)
				{
					FileInUseException ex3 = ex2;
					if (!ex3.Path.EndsWith("ade.exe", StringComparison.OrdinalIgnoreCase) || !(ex3.Version != Application.ProductVersion))
					{
						throw;
					}
					ShowErrorMessage(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_MismatchRunningADEVersion, ex3.Version, Application.ProductVersion));
				}
			}
			else
			{
				ShowErrorMessage(string.Format(CultureInfo.CurrentCulture, Resources.MessageFormat_RequiresNETFramework4OrLaterPleaseUpgradeYourInstallationOfNETFrameworkAndThenRestart0, Application.ProductName));
			}
		}
		else
		{
			UninstallCheckLicense();
		}
		TraceLogManager.SaveSettings((ISettings)(object)SettingsManager.GlobalInstance);
		TraceLogManager.Stop();
		TidyUpSingletons();
	}

	private static void UninstallCheckLicense()
	{
		if (!LicenseManager.GlobalInstance.Expired)
		{
			((IEnumerable<Ecu>)SapiManager.GlobalInstance.Sapi.Ecus).ToList().ForEach(delegate
			{
			});
			Application.Run(new UninstallDeauthorizeDialog());
		}
	}

	private static bool HasRequiredMinimumDotNet()
	{
		bool result = true;
		RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full");
		int num = 0;
		if (registryKey != null)
		{
			object value = registryKey.GetValue("Install");
			num = ((value != null) ? ((int)value) : 0);
		}
		if (num == 0)
		{
			result = false;
		}
		return result;
	}

	private static void MoveLegacyFiles()
	{
		string directoryRoot = Directory.GetDirectoryRoot(Application.ExecutablePath);
		string path = Path.Combine(Path.Combine(directoryRoot, Application.CompanyName), "Drumroll");
		string[] array = new string[6] { "Log Files", "Summary Files", "Technical Support", "User Data", "Application Data\\History", "Application Data\\Uploads" };
		string[] array2 = array;
		foreach (string path2 in array2)
		{
			MoveDirectory(Path.Combine(path, path2), Path.Combine(Directories.DrumrollData, path2));
		}
	}

	private static bool RemoveLegacySidPassthruEntry(bool silentInstance)
	{
		string text = "Software\\PassThruSupport\\Pi Technology\\";
		try
		{
			RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(text, RegistryRights.Delete);
			if (registryKey != null)
			{
				Registry.LocalMachine.DeleteSubKeyTree(text, throwOnMissingSubKey: false);
			}
		}
		catch (SecurityException)
		{
			if (!silentInstance)
			{
				WindowsPrincipal windowsPrincipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
				if (!windowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator))
				{
					MessageBox.Show(string.Format(CultureInfo.CurrentCulture, Resources.FormatLegacySidRegistryKeyDetected, ApplicationInformation.ProductName), ApplicationInformation.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1, (MessageBoxOptions)0);
					if (RunElevatedToRmoveLegacySidKey(Application.ExecutablePath))
					{
						return true;
					}
				}
				MessageBox.Show(string.Format(CultureInfo.CurrentCulture, Resources.FormatLegacySidRegistryKeyNotRemoved, ApplicationInformation.ProductName), ApplicationInformation.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1, (MessageBoxOptions)0);
			}
			else
			{
				Environment.ExitCode = 8344;
			}
			return false;
		}
		return true;
	}

	private static bool RunElevatedToRmoveLegacySidKey(string fileName)
	{
		ProcessStartInfo startInfo = new ProcessStartInfo
		{
			Verb = "runas",
			FileName = fileName,
			Arguments = "removeLegacySidRegistryKey"
		};
		try
		{
			Process process = Process.Start(startInfo);
			if (process != null && process.WaitForExit(5000))
			{
				if (process.ExitCode == 0)
				{
					return true;
				}
				Environment.ExitCode = process.ExitCode;
			}
		}
		catch (Win32Exception)
		{
		}
		return false;
	}

	private static void MoveDirectory(string source, string dest)
	{
		if (!Directory.Exists(source))
		{
			return;
		}
		try
		{
			string[] directories = Directory.GetDirectories(source);
			string[] files = Directory.GetFiles(source);
			if (directories.Length != 0 || files.Length != 0)
			{
				if (!Directory.Exists(dest))
				{
					Directory.CreateDirectory(dest);
				}
				string[] array = directories;
				foreach (string text in array)
				{
					string name = new DirectoryInfo(text).Name;
					string dest2 = Path.Combine(dest, name);
					MoveDirectory(text, dest2);
				}
				string[] array2 = files;
				foreach (string text2 in array2)
				{
					string fileName = Path.GetFileName(text2);
					string destFileName = Path.Combine(dest, fileName);
					try
					{
						splashScreen.OverallStatusText = string.Format(CultureInfo.CurrentCulture, Resources.MessageMoving, source);
						FileManagement.EnsureWritePossible(text2);
						File.Move(text2, destFileName);
					}
					catch (IOException)
					{
					}
					catch (ArgumentException)
					{
					}
					catch (UnauthorizedAccessException)
					{
					}
					catch (NotSupportedException)
					{
					}
				}
			}
			directories = Directory.GetDirectories(source);
			files = Directory.GetFiles(source);
			if (directories.Length == 0 && files.Length == 0)
			{
				Directory.Delete(source);
			}
		}
		catch (IOException)
		{
		}
		catch (UnauthorizedAccessException)
		{
		}
		catch (ArgumentException)
		{
		}
		catch (NotSupportedException)
		{
		}
	}

	private static void ShowErrorMessage(string message)
	{
		MessageBox.Show(message, ApplicationInformation.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1, CultureInfo.CurrentCulture.TextInfo.IsRightToLeft ? (MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading) : ((MessageBoxOptions)0));
	}

	private static void TidyUpSingletons()
	{
		NetworkSettings.GlobalInstance.Dispose();
		ThemeProvider.GlobalInstance.Dispose();
		LicenseManager.GlobalInstance.Dispose();
		SettingsManager.GlobalInstance.Dispose();
		SapiManager.GlobalInstance.Dispose();
	}

	private static void OnMainFormShown(object sender, EventArgs e)
	{
		if (((Control)(object)splashScreen).Visible)
		{
			((Control)(object)splashScreen).Hide();
		}
		((Component)(object)splashScreen).Dispose();
		splashScreen = null;
	}

	private static bool ValidLicense()
	{
		splashScreen.OverallStatusText = "Validating license...";
		bool expired = LicenseManager.GlobalInstance.Expired;
		TimeSpan timeSpanRemaining = LicenseManager.GlobalInstance.TimeSpanRemaining;
		bool flag = false;
		if (!expired && timeSpanRemaining >= new TimeSpan(30, 0, 0, 0) && ServerDataManager.GlobalInstance.CompatibilityTable != null && ServerDataManager.GlobalInstance.EdexCompatibilityTable != null)
		{
			return true;
		}
		return ServerRegistrationDialog.ShowRegistrationDialog();
	}

	public static string GetSidDll()
	{
		string name = "Software\\PassThruSupport\\Detroit Diesel\\Devices\\SID";
		string result = "";
		RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(name);
		if (registryKey != null)
		{
			result = registryKey.GetValue("FunctionLibrary") as string;
		}
		return result;
	}

	public static bool RunSidConfigure(bool checkIniFile)
	{
		string sidDll = GetSidDll();
		if (!string.IsNullOrEmpty(sidDll) && (!checkIniFile || !File.Exists(Directories.SidIniFile)))
		{
			NativeMethods.SidConfigureResult sidConfigureResult = NativeMethods.ExecuteSidConfigure(sidDll);
			if (sidConfigureResult == NativeMethods.SidConfigureResult.CouldNotExecute)
			{
				ShowErrorMessage(Resources.Message_SIDConfigureUnavailable);
			}
			return sidConfigureResult == NativeMethods.SidConfigureResult.Success;
		}
		return false;
	}
}
