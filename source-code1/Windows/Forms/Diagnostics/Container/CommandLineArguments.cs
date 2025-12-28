// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.Windows.Forms.Diagnostics.Container.CommandLineArguments
// Assembly: Drumroll, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: C4A91DC6-1B26-469B-9D8E-0DD5580BB754
// Assembly location: C:\Users\petra\Downloads\Telegram Desktop\Drumroll.exe

using DetroitDiesel.Common;
using DetroitDiesel.Windows.Forms.Diagnostics.Container.Properties;
using System;
using System.Globalization;
using System.IO;

#nullable disable
namespace DetroitDiesel.Windows.Forms.Diagnostics.Container;

public class CommandLineArguments
{
  private string message;
  private string logFilePath;

  public string Message => this.message;

  public string LogFilePath => this.logFilePath;

  public bool IsUninstall { private set; get; }

  public bool ProcessArguments(string[] arguments)
  {
    bool flag = arguments.Length <= 1;
    if (flag)
    {
      foreach (string path in arguments)
      {
        if (path == "uninstall")
          this.IsUninstall = true;
        else if (File.Exists(path))
        {
          if (string.Compare(Path.GetExtension(path), ".DrumrollLog", CultureInfo.CurrentCulture, CompareOptions.IgnoreCase) == 0)
          {
            this.logFilePath = path;
          }
          else
          {
            this.message = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.CommandLineArgInvalidFileType, (object) ApplicationInformation.ApplicationName, (object) path);
            flag = false;
          }
        }
        else
        {
          this.message = "File not found.";
          flag = false;
        }
      }
    }
    else
      this.message = "Too many command line arguments.";
    return flag;
  }
}
