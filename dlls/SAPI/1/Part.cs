// Decompiled with JetBrains decompiler
// Type: SapiLayer1.Part
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using System;
using System.Globalization;
using System.Linq;

#nullable disable
namespace SapiLayer1;

public sealed class Part
{
  public const int PartNumberLength = 11;
  private readonly string number;
  private readonly object version;
  private bool freightlinerHardwareHasAPrefix;
  private bool isFreightlinerHardware;

  public Part(string number)
  {
    this.DetermineFormatting(number);
    this.number = Part.Strip(number);
    if (this.number.Length <= 11)
      return;
    string str = this.number.Substring(11);
    try
    {
      this.version = (object) Convert.ToUInt32(str, (IFormatProvider) CultureInfo.InvariantCulture);
    }
    catch (FormatException ex)
    {
      Sapi.GetSapi().RaiseDebugInfoEvent((object) number, "Error parsing the part version - not a supported format of part version read");
    }
    this.number = this.number.Substring(0, 11);
  }

  public Part(string number, int version)
  {
    this.DetermineFormatting(number);
    this.number = Part.Strip(number);
    this.version = (object) Convert.ToUInt32((object) version, (IFormatProvider) CultureInfo.InvariantCulture);
  }

  public Part(string number, object version)
  {
    this.DetermineFormatting(number);
    this.number = Part.Strip(number);
    try
    {
      this.version = (object) Convert.ToUInt32(version, (IFormatProvider) CultureInfo.InvariantCulture);
    }
    catch (FormatException ex)
    {
      Sapi.GetSapi().RaiseDebugInfoEvent((object) number, "Error parsing the part version - not a supported format of part version read");
    }
  }

  private void DetermineFormatting(string number)
  {
    if (number.Count<char>((Func<char, bool>) (c => c == '-')) != 2)
      return;
    this.isFreightlinerHardware = true;
    if (!number.StartsWith("A", StringComparison.OrdinalIgnoreCase))
      return;
    this.freightlinerHardwareHasAPrefix = true;
  }

  public string Number => this.number;

  public object Version => this.version;

  public bool FreightlinerHardwareHasAPrefix => this.freightlinerHardwareHasAPrefix;

  public bool IsFreightlinerHardware => this.isFreightlinerHardware;

  public override string ToString()
  {
    return this.version != null ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}-{1,2:000}", (object) this.number, (object) (uint) this.version) : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}", (object) this.number);
  }

  public override int GetHashCode() => this.ToString().GetHashCode();

  public override bool Equals(object obj)
  {
    if (obj == null)
      return false;
    Part part = (Part) obj;
    return part.Version != null ? string.Equals(this.ToString(), obj.ToString()) : string.Equals(this.Number, part.Number);
  }

  private static string Strip(string input)
  {
    return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "A{0}", (object) input.Replace(" ", string.Empty).Replace("A", string.Empty).Replace("_", string.Empty).Replace("-", string.Empty).Replace("ZGS", string.Empty));
  }
}
