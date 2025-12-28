// Decompiled with JetBrains decompiler
// Type: McdAbstraction.McdDBMatchingParameter
// Assembly: McdAbstraction, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 2CF84A4E-9C9E-4158-9C67-2CE39889DD31
// Assembly location: C:\Users\petra\Downloads\Архив (2)\McdAbstraction.dll

using Softing.Dts;

#nullable disable
namespace McdAbstraction;

public class McdDBMatchingParameter
{
  private MCDDbMatchingParameter matchingParameter;
  private string primitiveQualifier;
  private string responseQualifier;
  private McdValue expectedValue;

  internal McdDBMatchingParameter(MCDDbMatchingParameter parameter)
  {
    this.matchingParameter = parameter;
    this.primitiveQualifier = this.matchingParameter.DbDiagComPrimitive.ShortName;
    this.responseQualifier = this.matchingParameter.DbResponseParameter.ShortName;
    this.expectedValue = new McdValue(this.matchingParameter.ExpectedValue);
  }

  public string Primitive => this.primitiveQualifier;

  public string ResponseParameter => this.responseQualifier;

  public McdValue ExpectedValue => this.expectedValue;

  public bool IsMatch(McdLogicalLink link)
  {
    McdValue identificationResult = link.GetVariantIdentificationResult(this.primitiveQualifier, this.responseQualifier);
    return identificationResult != null && identificationResult.Value != null && this.expectedValue != null && this.expectedValue.Value != null && object.Equals(identificationResult.Value, this.expectedValue.Value);
  }
}
