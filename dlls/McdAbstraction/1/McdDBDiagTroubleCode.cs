// Decompiled with JetBrains decompiler
// Type: McdAbstraction.McdDBDiagTroubleCode
// Assembly: McdAbstraction, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 2CF84A4E-9C9E-4158-9C67-2CE39889DD31
// Assembly location: C:\Users\petra\Downloads\Архив (2)\McdAbstraction.dll

using Softing.Dts;

#nullable disable
namespace McdAbstraction;

public class McdDBDiagTroubleCode
{
  private MCDDbDiagTroubleCode code;

  internal McdDBDiagTroubleCode(MCDDbDiagTroubleCode code)
  {
    this.code = code;
    this.DisplayTroubleCode = this.code.DisplayTroubleCode;
    this.Text = this.code.DTCText;
    this.TroubleCode = (long) this.code.TroubleCode;
  }

  public string DisplayTroubleCode { get; private set; }

  public string Text { get; private set; }

  public long TroubleCode { get; private set; }
}
