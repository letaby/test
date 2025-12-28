// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.Windows.Forms.Diagnostics.Container.LanguageManager
// Assembly: Drumroll, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: C4A91DC6-1B26-469B-9D8E-0DD5580BB754
// Assembly location: C:\Users\petra\Downloads\Telegram Desktop\Drumroll.exe

using DetroitDiesel.Settings;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;

#nullable disable
namespace DetroitDiesel.Windows.Forms.Diagnostics.Container;

internal static class LanguageManager
{
  private const string NeutralCultureName = "en-US";
  private static readonly CultureInfo currentUICulture;

  public static bool IsSupported(CultureInfo info)
  {
    return info.Name == "en-US" || Directory.Exists(Path.Combine(Application.StartupPath, info.Name));
  }

  public static bool IsSupported(string culture)
  {
    if (culture == "en-US")
      return true;
    try
    {
      return LanguageManager.IsSupported(CultureInfo.GetCultureInfo(culture));
    }
    catch (ArgumentNullException ex)
    {
      throw;
    }
    catch (ArgumentException ex)
    {
    }
    return false;
  }

  public static IEnumerable<CultureInfo> EnumerateSupportedCultures()
  {
    List<CultureInfo> cultureInfoList = new List<CultureInfo>();
    cultureInfoList.Add(new CultureInfo("en-US"));
    foreach (DirectoryInfo directory in new DirectoryInfo(Application.StartupPath).GetDirectories("*", SearchOption.AllDirectories))
    {
      try
      {
        CultureInfo cultureInfo = CultureInfo.GetCultureInfo(directory.Name);
        if (!cultureInfoList.Contains(cultureInfo))
        {
          if (LanguageManager.IsSupported(cultureInfo))
          {
            if (!cultureInfo.CultureTypes.HasFlag((Enum) CultureTypes.UserCustomCulture))
              cultureInfoList.Add(cultureInfo);
          }
        }
      }
      catch (ArgumentException ex)
      {
      }
    }
    return (IEnumerable<CultureInfo>) cultureInfoList;
  }

  public static void ApplyApplicationLocale()
  {
    Thread.CurrentThread.CurrentUICulture = LanguageManager.currentUICulture;
  }

  static LanguageManager()
  {
    string presentationCulture = LanguageManager.PresentationCulture;
    if (string.IsNullOrEmpty(presentationCulture) || !LanguageManager.IsSupported(presentationCulture))
      LanguageManager.currentUICulture = CultureInfo.CurrentUICulture;
    else
      LanguageManager.currentUICulture = CultureInfo.GetCultureInfo(presentationCulture);
  }

  public static string PresentationCulture
  {
    get
    {
      return SettingsManager.GlobalInstance.GetValue<StringSetting>("PresentationLanguage", "Client", new StringSetting()).Value;
    }
    set
    {
      if (!(value != LanguageManager.PresentationCulture))
        return;
      SettingsManager.GlobalInstance.SetValue<StringSetting>("PresentationLanguage", "Client", new StringSetting(value), false);
    }
  }
}
