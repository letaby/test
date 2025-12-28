using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using DetroitDiesel.Settings;

namespace DetroitDiesel.Windows.Forms.Diagnostics.Container;

internal static class LanguageManager
{
	private const string NeutralCultureName = "en-US";

	private static readonly CultureInfo currentUICulture;

	public static string PresentationCulture
	{
		get
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Expected O, but got Unknown
			StringSetting value = SettingsManager.GlobalInstance.GetValue<StringSetting>("PresentationLanguage", "Client", new StringSetting());
			return value.Value;
		}
		set
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Expected O, but got Unknown
			if (value != PresentationCulture)
			{
				SettingsManager.GlobalInstance.SetValue<StringSetting>("PresentationLanguage", "Client", new StringSetting(value), false);
			}
		}
	}

	public static bool IsSupported(CultureInfo info)
	{
		if (info.Name == "en-US")
		{
			return true;
		}
		return Directory.Exists(Path.Combine(Application.StartupPath, info.Name));
	}

	public static bool IsSupported(string culture)
	{
		if (culture == "en-US")
		{
			return true;
		}
		try
		{
			return IsSupported(CultureInfo.GetCultureInfo(culture));
		}
		catch (ArgumentNullException)
		{
			throw;
		}
		catch (ArgumentException)
		{
		}
		return false;
	}

	public static IEnumerable<CultureInfo> EnumerateSupportedCultures()
	{
		List<CultureInfo> list = new List<CultureInfo>();
		list.Add(new CultureInfo("en-US"));
		DirectoryInfo directoryInfo = new DirectoryInfo(Application.StartupPath);
		DirectoryInfo[] directories = directoryInfo.GetDirectories("*", SearchOption.AllDirectories);
		DirectoryInfo[] array = directories;
		foreach (DirectoryInfo directoryInfo2 in array)
		{
			try
			{
				CultureInfo cultureInfo = CultureInfo.GetCultureInfo(directoryInfo2.Name);
				if (!list.Contains(cultureInfo) && IsSupported(cultureInfo) && !cultureInfo.CultureTypes.HasFlag(CultureTypes.UserCustomCulture))
				{
					list.Add(cultureInfo);
				}
			}
			catch (ArgumentException)
			{
			}
		}
		return list;
	}

	public static void ApplyApplicationLocale()
	{
		Thread.CurrentThread.CurrentUICulture = currentUICulture;
	}

	static LanguageManager()
	{
		string presentationCulture = PresentationCulture;
		if (string.IsNullOrEmpty(presentationCulture) || !IsSupported(presentationCulture))
		{
			currentUICulture = CultureInfo.CurrentUICulture;
		}
		else
		{
			currentUICulture = CultureInfo.GetCultureInfo(presentationCulture);
		}
	}
}
