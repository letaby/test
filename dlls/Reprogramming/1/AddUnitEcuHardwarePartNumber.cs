// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming.AddUnitEcuHardwarePartNumber
// Assembly: Reprogramming, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: 6E09671B-250E-411A-80FC-C490A3A17075
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Reprogramming.dll

using DetroitDiesel.Common;
using SapiLayer1;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

#nullable disable
namespace DetroitDiesel.Windows.Forms.Diagnostics.Panels.Reprogramming;

public class AddUnitEcuHardwarePartNumber
{
  private string ecuName;
  private Part ecuPartNumber;
  private string ecuPartNumberDisplayValue;
  private static Collection<string> ecuNamesAvailable;

  [Browsable(false)]
  public bool FormatPartNumber { get; set; }

  public string EcuName
  {
    get => this.ecuName;
    set
    {
      if (value.Equals(this.ecuName, StringComparison.OrdinalIgnoreCase))
        return;
      this.ecuName = value.ToUpperInvariant();
      this.EcuPartNumberDisplayValue = this.EcuPartNumberDisplayValue;
    }
  }

  public string EcuPartNumberDisplayValue
  {
    get => this.ecuPartNumberDisplayValue;
    set
    {
      if (!string.IsNullOrEmpty(value))
      {
        if (this.FormatPartNumber)
        {
          this.ecuPartNumber = new Part(value);
          this.ecuPartNumberDisplayValue = PartExtensions.ToHardwarePartNumberString(this.ecuPartNumber, this.ecuName, true);
        }
        else
        {
          this.ecuPartNumber = (Part) null;
          this.ecuPartNumberDisplayValue = value;
        }
      }
      else
      {
        this.ecuPartNumber = (Part) null;
        this.ecuPartNumberDisplayValue = string.Empty;
      }
    }
  }

  [Browsable(false)]
  public Part EcuPartNumber
  {
    get => this.ecuPartNumber;
    set => this.ecuPartNumber = value;
  }

  public static Collection<string> EcuNamesAvailable
  {
    get
    {
      if (AddUnitEcuHardwarePartNumber.ecuNamesAvailable == null)
      {
        AddUnitEcuHardwarePartNumber.ecuNamesAvailable = new Collection<string>();
        foreach (Ecu ecu in (ReadOnlyCollection<Ecu>) SapiManager.GlobalInstance.Sapi.Ecus)
        {
          if ((!ecu.IsRollCall || SapiManager.SupportsRollCallParameterization(ecu)) && !ecu.IsVirtual && (SapiExtensions.IsDataSourceDepot(ecu) || SapiExtensions.IsDataSourceEdex(ecu)))
            AddUnitEcuHardwarePartNumber.ecuNamesAvailable.Add(ecu.Name);
        }
      }
      return AddUnitEcuHardwarePartNumber.ecuNamesAvailable;
    }
  }

  public AddUnitEcuHardwarePartNumber()
  {
    this.ecuName = string.Empty;
    this.ecuPartNumber = (Part) null;
  }

  public AddUnitEcuHardwarePartNumber(string name, string partNumber, bool formatPartNumber)
  {
    this.FormatPartNumber = formatPartNumber;
    this.ecuName = name;
    this.EcuPartNumberDisplayValue = partNumber;
  }
}
