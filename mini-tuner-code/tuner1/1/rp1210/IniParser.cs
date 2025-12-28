// Decompiled with JetBrains decompiler
// Type: rp1210.IniParser
// Assembly: TunerSolution, Version=1.0.0.142, Culture=neutral, PublicKeyToken=null
// MVID: 9D02C703-4AB8-4296-B056-FAFCB6EB03BA
// Assembly location: C:\Users\petra\Downloads\TunerSolution\TunerSolution.exe

using System;
using System.Collections;
using System.IO;

#nullable disable
namespace rp1210;

public class IniParser
{
  private Hashtable keyPairs = new Hashtable();
  private string iniFilePath;

  public IniParser(string iniPath)
  {
    TextReader textReader = (TextReader) null;
    string str1 = (string) null;
    this.iniFilePath = iniPath;
    if (!File.Exists(iniPath))
      throw new FileNotFoundException("Unable to locate " + iniPath);
    try
    {
      textReader = (TextReader) new StreamReader(iniPath);
      for (string str2 = textReader.ReadLine(); str2 != null; str2 = textReader.ReadLine())
      {
        string upper = str2.Trim().ToUpper();
        if (upper != "")
        {
          if (upper.StartsWith("[") && upper.EndsWith("]"))
            str1 = upper.Substring(1, upper.Length - 2);
          else if (!upper.StartsWith("#") & !upper.StartsWith(";"))
          {
            string[] strArray = upper.Split(new char[1]
            {
              '='
            }, 2);
            string str3 = (string) null;
            if (str1 == null)
              str1 = "ROOT";
            IniParser.SectionPair key;
            key.Section = str1;
            key.Key = strArray[0];
            if (strArray.Length > 1)
              str3 = strArray[1];
            this.keyPairs.Add((object) key, (object) str3);
          }
        }
      }
    }
    catch (Exception ex)
    {
      throw ex;
    }
    finally
    {
      textReader?.Close();
    }
  }

  public string GetSetting(string sectionName, string settingName)
  {
    IniParser.SectionPair key;
    key.Section = sectionName.ToUpper();
    key.Key = settingName.ToUpper();
    return (string) this.keyPairs[(object) key];
  }

  public string[] EnumSection(string sectionName)
  {
    ArrayList arrayList = new ArrayList();
    foreach (IniParser.SectionPair key in (IEnumerable) this.keyPairs.Keys)
    {
      if (key.Section == sectionName.ToUpper())
        arrayList.Add((object) key.Key);
    }
    return (string[]) arrayList.ToArray(typeof (string));
  }

  public void AddSetting(string sectionName, string settingName, string settingValue)
  {
    IniParser.SectionPair key;
    key.Section = sectionName.ToUpper();
    key.Key = settingName.ToUpper();
    if (this.keyPairs.ContainsKey((object) key))
      this.keyPairs.Remove((object) key);
    this.keyPairs.Add((object) key, (object) settingValue);
  }

  public void AddSetting(string sectionName, string settingName)
  {
    this.AddSetting(sectionName, settingName, (string) null);
  }

  public void DeleteSetting(string sectionName, string settingName)
  {
    IniParser.SectionPair key;
    key.Section = sectionName.ToUpper();
    key.Key = settingName.ToUpper();
    if (!this.keyPairs.ContainsKey((object) key))
      return;
    this.keyPairs.Remove((object) key);
  }

  public void SaveSettings(string newFilePath)
  {
    ArrayList arrayList = new ArrayList();
    string str1 = "";
    foreach (IniParser.SectionPair key in (IEnumerable) this.keyPairs.Keys)
    {
      if (!arrayList.Contains((object) key.Section))
        arrayList.Add((object) key.Section);
    }
    foreach (string str2 in arrayList)
    {
      str1 = $"{str1}[{str2}]\r\n";
      foreach (IniParser.SectionPair key in (IEnumerable) this.keyPairs.Keys)
      {
        if (key.Section == str2)
        {
          string str3 = (string) this.keyPairs[(object) key];
          if (str3 != null)
            str3 = "=" + str3;
          str1 = $"{str1}{key.Key}{str3}\r\n";
        }
      }
      str1 += "\r\n";
    }
    try
    {
      StreamWriter streamWriter = new StreamWriter(newFilePath);
      streamWriter.Write(str1);
      streamWriter.Close();
    }
    catch (Exception ex)
    {
      throw ex;
    }
  }

  public void SaveSettings() => this.SaveSettings(this.iniFilePath);

  private struct SectionPair
  {
    public string Section;
    public string Key;
  }
}
