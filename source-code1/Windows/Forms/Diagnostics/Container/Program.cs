// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.Windows.Forms.Diagnostics.Container.Program
// Assembly: Drumroll, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: C4A91DC6-1B26-469B-9D8E-0DD5580BB754
// Assembly location: C:\Users\petra\Downloads\Telegram Desktop\Drumroll.exe

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

#nullable disable
namespace DetroitDiesel.Windows.Forms.Diagnostics.Container;

internal static class Program
{
  private static SplashScreen splashScreen;

  [STAThread]
  private static void Main(string[] args)
  {
    CrashHandler.Initialize();
    Directory.SetCurrentDirectory(Path.GetDirectoryName(Application.ExecutablePath));
    bool silentInstance = args.Length != 0 && args[0].Equals("removeLegacySidRegistryKey", StringComparison.OrdinalIgnoreCase);
    if (ApplicationForceUpgrade.ProductUpgradeRequired && ApplicationForceUpgrade.ForceUpgrade())
    {
      Program.ShowErrorMessage(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_RequiresUpgradetoLatestRelease, (object) ApplicationInformation.ProductName, (object) ApplicationForceUpgrade.RequiredUpgradeVersion));
    }
    else
    {
      if (!Program.RemoveLegacySidPassthruEntry(silentInstance) || silentInstance)
        return;
      Program.RunApplication(args);
    }
  }

  private static void RunApplication(string[] args)
  {
    CommandLineArguments commandLineArguments = new CommandLineArguments();
    if (!commandLineArguments.ProcessArguments(args))
    {
      Program.ShowErrorMessage(commandLineArguments.Message);
    }
    else
    {
      Application.SetCompatibleTextRenderingDefault(false);
      if (!commandLineArguments.IsUninstall)
        Program.RunSidConfigure(true);
      Application.EnableVisualStyles();
      Application.DoEvents();
      AssemblyName name1 = Assembly.LoadFrom(Path.Combine(Path.GetDirectoryName(Assembly.GetAssembly(typeof (Program)).Location), "Common.dll")).GetName();
      AssemblyName name2 = Assembly.GetExecutingAssembly().GetName();
      if (name1.Version != name2.Version)
      {
        Program.ShowErrorMessage(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Program_FormatInconsistentInstallation, (object) ApplicationInformation.ProductName, (object) name2.Name, (object) name2.Version, (object) name1.Name, (object) name1.Version));
      }
      else
      {
        LanguageManager.ApplyApplicationLocale();
        TraceLogManager.LoadSettings((ISettings) SettingsManager.GlobalInstance);
        TraceLogManager.Start();
        ServerDataManager globalInstance = ServerDataManager.GlobalInstance;
        globalInstance.Load();
        globalInstance.AddCrashHandlerInfo();
        if (!commandLineArguments.IsUninstall)
        {
          Program.splashScreen = new SplashScreen();
          ((Control) Program.splashScreen).Show();
          Application.DoEvents();
          ToolStripManager.RenderMode = Environment.OSVersion.Version.Major >= 6 ? ToolStripManagerRenderMode.Professional : ToolStripManagerRenderMode.System;
          if (Program.HasRequiredMinimumDotNet())
          {
            try
            {
              ExtensionLibrary.UpdateExtensions((IProgressBar) Program.splashScreen);
              Program.splashScreen.OverallStatusText = Resources.StatusPreparingCommunications;
              SapiManager.GlobalInstance.ResetSapi();
              if (SapiManager.GlobalInstance.Sapi == null)
                throw new InvalidOperationException();
              SapiManager.GlobalInstance.ProgressBar = (IProgressBar) Program.splashScreen;
              Program.splashScreen.OverallStatusText = Resources.StatusPreparingDiagnosticData;
              SapiManager.GlobalInstance.LoadSettings((ISettings) SettingsManager.GlobalInstance);
              SapiManager.GlobalInstance.ProgressBar = (IProgressBar) null;
              bool flag = false;
              string logFileToLoad = string.Empty;
              if (Program.ValidLicense())
              {
                if (!PrintHelper.Initialize())
                {
                  Program.ShowErrorMessage(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_RequiresInternetExplorer11OrLater, (object) Application.ProductName));
                }
                else
                {
                  flag = true;
                  logFileToLoad = commandLineArguments.LogFilePath;
                }
              }
              if (flag)
              {
                Program.MoveLegacyFiles();
                MainForm mainForm = new MainForm(Program.splashScreen, logFileToLoad);
                using (EventWaitHandle eventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset))
                {
                  mainForm.Shown += new EventHandler(Program.OnMainFormShown);
                  Application.Run((Form) mainForm);
                  mainForm.Shown -= new EventHandler(Program.OnMainFormShown);
                  eventWaitHandle.Set();
                }
              }
              else
                Application.Exit();
            }
            catch (CompilerVersionNotFoundException ex)
            {
              Program.ShowErrorMessage(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_RequiresNETFramework4OrLaterPleaseUpgradeYourInstallationOfNETFrameworkAndThenRestart0, (object) Application.ProductName));
            }
            catch (FileInUseException ex)
            {
              if (ex.Path.EndsWith("ade.exe", StringComparison.OrdinalIgnoreCase) && ex.Version != Application.ProductVersion)
                Program.ShowErrorMessage(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_MismatchRunningADEVersion, (object) ex.Version, (object) Application.ProductVersion));
              else
                throw;
            }
          }
          else
            Program.ShowErrorMessage(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageFormat_RequiresNETFramework4OrLaterPleaseUpgradeYourInstallationOfNETFrameworkAndThenRestart0, (object) Application.ProductName));
        }
        else
          Program.UninstallCheckLicense();
        TraceLogManager.SaveSettings((ISettings) SettingsManager.GlobalInstance);
        TraceLogManager.Stop();
        Program.TidyUpSingletons();
      }
    }
  }

  private static void UninstallCheckLicense()
  {
    if (LicenseManager.GlobalInstance.Expired)
      return;
    ((IEnumerable<Ecu>) SapiManager.GlobalInstance.Sapi.Ecus).ToList<Ecu>().ForEach((Action<Ecu>) (ecu => { }));
    Application.Run((Form) new UninstallDeauthorizeDialog());
  }

  private static bool HasRequiredMinimumDotNet()
  {
    bool flag = true;
    RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full");
    int num = 0;
    if (registryKey != null)
    {
      object obj = registryKey.GetValue("Install");
      num = obj != null ? (int) obj : 0;
    }
    if (num == 0)
      flag = false;
    return flag;
  }

  private static void MoveLegacyFiles()
  {
    string path1 = Path.Combine(Path.Combine(Directory.GetDirectoryRoot(Application.ExecutablePath), Application.CompanyName), "Drumroll");
    string[] strArray = new string[6]
    {
      "Log Files",
      "Summary Files",
      "Technical Support",
      "User Data",
      "Application Data\\History",
      "Application Data\\Uploads"
    };
    foreach (string path2 in strArray)
      Program.MoveDirectory(Path.Combine(path1, path2), Path.Combine(Directories.DrumrollData, path2));
  }

  private static bool RemoveLegacySidPassthruEntry(bool silentInstance)
  {
    string str = "Software\\PassThruSupport\\Pi Technology\\";
    try
    {
      if (Registry.LocalMachine.OpenSubKey(str, RegistryRights.Delete) != null)
        Registry.LocalMachine.DeleteSubKeyTree(str, false);
    }
    catch (SecurityException ex)
    {
      if (!silentInstance)
      {
        if (!new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator))
        {
          int num = (int) MessageBox.Show(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.FormatLegacySidRegistryKeyDetected, (object) ApplicationInformation.ProductName), ApplicationInformation.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1, (MessageBoxOptions) 0);
          if (Program.RunElevatedToRmoveLegacySidKey(Application.ExecutablePath))
            return true;
        }
        int num1 = (int) MessageBox.Show(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.FormatLegacySidRegistryKeyNotRemoved, (object) ApplicationInformation.ProductName), ApplicationInformation.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1, (MessageBoxOptions) 0);
      }
      else
        Environment.ExitCode = 8344;
      return false;
    }
    return true;
  }

  private static bool RunElevatedToRmoveLegacySidKey(string fileName)
  {
    ProcessStartInfo startInfo = new ProcessStartInfo()
    {
      Verb = "runas",
      FileName = fileName,
      Arguments = "removeLegacySidRegistryKey"
    };
    try
    {
      Process process = Process.Start(startInfo);
      if (process != null)
      {
        if (process.WaitForExit(5000))
        {
          if (process.ExitCode == 0)
            return true;
          Environment.ExitCode = process.ExitCode;
        }
      }
    }
    catch (Win32Exception ex)
    {
    }
    return false;
  }

  private static void MoveDirectory(string source, string dest)
  {
    if (!Directory.Exists(source))
      return;
    try
    {
      string[] directories1 = Directory.GetDirectories(source);
      string[] files1 = Directory.GetFiles(source);
      if (directories1.Length != 0 || files1.Length != 0)
      {
        if (!Directory.Exists(dest))
          Directory.CreateDirectory(dest);
        foreach (string str in directories1)
        {
          string name = new DirectoryInfo(str).Name;
          string dest1 = Path.Combine(dest, name);
          Program.MoveDirectory(str, dest1);
        }
        foreach (string str in files1)
        {
          string fileName = Path.GetFileName(str);
          string destFileName = Path.Combine(dest, fileName);
          try
          {
            Program.splashScreen.OverallStatusText = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MessageMoving, (object) source);
            FileManagement.EnsureWritePossible(str);
            File.Move(str, destFileName);
          }
          catch (IOException ex)
          {
          }
          catch (ArgumentException ex)
          {
          }
          catch (UnauthorizedAccessException ex)
          {
          }
          catch (NotSupportedException ex)
          {
          }
        }
      }
      string[] directories2 = Directory.GetDirectories(source);
      string[] files2 = Directory.GetFiles(source);
      if (directories2.Length != 0 || files2.Length != 0)
        return;
      Directory.Delete(source);
    }
    catch (IOException ex)
    {
    }
    catch (UnauthorizedAccessException ex)
    {
    }
    catch (ArgumentException ex)
    {
    }
    catch (NotSupportedException ex)
    {
    }
  }

  private static void ShowErrorMessage(string message)
  {
    int num = (int) MessageBox.Show(message, ApplicationInformation.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1, CultureInfo.CurrentCulture.TextInfo.IsRightToLeft ? MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading : (MessageBoxOptions) 0);
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
    if (((Control) Program.splashScreen).Visible)
      ((Control) Program.splashScreen).Hide();
    ((Component) Program.splashScreen).Dispose();
    Program.splashScreen = (SplashScreen) null;
  }

  private static bool ValidLicense()
  {
    Program.splashScreen.OverallStatusText = "Validating license...";
    bool expired = LicenseManager.GlobalInstance.Expired;
    TimeSpan timeSpanRemaining = LicenseManager.GlobalInstance.TimeSpanRemaining;
    return !expired && timeSpanRemaining >= new TimeSpan(30, 0, 0, 0) && ServerDataManager.GlobalInstance.CompatibilityTable != null && ServerDataManager.GlobalInstance.EdexCompatibilityTable != null || ServerRegistrationDialog.ShowRegistrationDialog();
  }

  public static string GetSidDll()
  {
    string name = "Software\\PassThruSupport\\Detroit Diesel\\Devices\\SID";
    string sidDll = "";
    RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(name);
    if (registryKey != null)
      sidDll = registryKey.GetValue("FunctionLibrary") as string;
    return sidDll;
  }

  public static bool RunSidConfigure(bool checkIniFile)
  {
    string sidDll = Program.GetSidDll();
    if (string.IsNullOrEmpty(sidDll) || checkIniFile && File.Exists(Directories.SidIniFile))
      return false;
    Program.NativeMethods.SidConfigureResult sidConfigureResult = Program.NativeMethods.ExecuteSidConfigure(sidDll);
    if (sidConfigureResult == Program.NativeMethods.SidConfigureResult.CouldNotExecute)
      Program.ShowErrorMessage(Resources.Message_SIDConfigureUnavailable);
    return sidConfigureResult == Program.NativeMethods.SidConfigureResult.Success;
  }

  private static class NativeMethods
  {
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
    private static extern IntPtr LoadLibrary(string path);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool FreeLibrary(IntPtr hModule);

    [DllImport("kernel32", CharSet = CharSet.Ansi, SetLastError = true, ThrowOnUnmappableChar = true, BestFitMapping = false)]
    private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

    public static Program.NativeMethods.SidConfigureResult ExecuteSidConfigure(string dllPath)
    {
      Program.NativeMethods.SidConfigureResult sidConfigureResult = Program.NativeMethods.SidConfigureResult.CouldNotExecute;
      IntPtr hModule = Program.NativeMethods.LoadLibrary(dllPath);
      if (hModule != IntPtr.Zero)
      {
        IntPtr procAddress = Program.NativeMethods.GetProcAddress(hModule, "Configure");
        if (procAddress != IntPtr.Zero)
          sidConfigureResult = (Program.NativeMethods.SidConfigureResult) ((Program.NativeMethods.ConfigureFunc) Marshal.GetDelegateForFunctionPointer(procAddress, typeof (Program.NativeMethods.ConfigureFunc)))();
        Program.NativeMethods.FreeLibrary(hModule);
      }
      return sidConfigureResult;
    }

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate int ConfigureFunc();

    public enum SidConfigureResult
    {
      CouldNotExecute = -2, // 0xFFFFFFFE
      NoDrivers = -1, // 0xFFFFFFFF
      Success = 0,
      Canceled = 1,
    }
  }

  private enum ExitCodes
  {
    ErrorSuccess = 0,
    ErrorDsInsuffAccessRights = 8344, // 0x00002098
  }
}
