// Decompiled with JetBrains decompiler
// Type: SapiLayer1.CodingParameterGroupCollection
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

#nullable disable
namespace SapiLayer1;

public sealed class CodingParameterGroupCollection : LateLoadReadOnlyCollection<CodingParameterGroup>
{
  private Dictionary<Part, CodingChoice> cache = new Dictionary<Part, CodingChoice>();
  private Channel channel;

  internal CodingParameterGroupCollection(Channel channel) => this.channel = channel;

  internal void Add(CodingParameterGroup group) => this.Items.Add(group);

  protected override void AcquireList()
  {
    if (this.channel == null)
      return;
    foreach (CodingFile codingFile in (ReadOnlyCollection<CodingFile>) Sapi.GetSapi().CodingFiles)
    {
      foreach (CodingParameterGroup codingParameterGroup in (ReadOnlyCollection<CodingParameterGroup>) codingFile.CodingParameterGroups)
      {
        if (codingParameterGroup.VariantAllowed(this.channel.DiagnosisVariant))
        {
          CodingParameterGroup destination = this[codingParameterGroup.Qualifier];
          if (destination == null)
            this.Add(codingParameterGroup.Clone(this));
          else
            codingParameterGroup.CopyTo(destination);
        }
      }
    }
  }

  internal void GetAllCodingsForPart(string partString, List<CodingChoice> output)
  {
    if (!this.cache.Any<KeyValuePair<Part, CodingChoice>>())
    {
      foreach (CodingParameterGroup codingParameterGroup in (ReadOnlyCollection<CodingParameterGroup>) this)
      {
        foreach (CodingChoice choice in (ReadOnlyCollection<CodingChoice>) codingParameterGroup.Choices)
          this.cache[choice.Part] = choice;
        foreach (CodingParameter parameter in (ReadOnlyCollection<CodingParameter>) codingParameterGroup.Parameters)
        {
          foreach (CodingChoice choice in (ReadOnlyCollection<CodingChoice>) parameter.Choices)
            this.cache[choice.Part] = choice;
        }
      }
    }
    Part key = new Part(partString);
    if (key.Version != null)
    {
      CodingChoice codingChoice;
      if (!this.cache.TryGetValue(key, out codingChoice))
        return;
      output.Add(codingChoice);
    }
    else
    {
      foreach (KeyValuePair<Part, CodingChoice> keyValuePair in this.cache)
      {
        if (key.Equals((object) keyValuePair.Key))
          output.Add(keyValuePair.Value);
      }
    }
  }

  public CodingParameterGroup this[string qualifier]
  {
    get
    {
      return this.FirstOrDefault<CodingParameterGroup>((Func<CodingParameterGroup, bool>) (item => string.Equals(item.Qualifier, qualifier, StringComparison.Ordinal)));
    }
  }

  public Channel Channel => this.channel;

  public CodingChoice GetCodingForPart(string part)
  {
    List<CodingChoice> output = new List<CodingChoice>();
    this.GetAllCodingsForPart(part, output);
    CodingChoice codingForPart = (CodingChoice) null;
    foreach (CodingChoice codingChoice in output)
    {
      if (codingForPart == null)
        codingForPart = codingChoice;
      else if (codingChoice.Part.Version != null && codingForPart.Part.Version != null && (uint) codingChoice.Part.Version > (uint) codingForPart.Part.Version)
        codingForPart = codingChoice;
    }
    return codingForPart;
  }

  public void SetCodingForParts(IEnumerable<string> parts, Collection<string> unknownList)
  {
    CaesarException caesarException = (CaesarException) null;
    foreach (string part in parts)
    {
      CodingChoice codingForPart = this.GetCodingForPart(part);
      if (codingForPart != null)
      {
        try
        {
          codingForPart.SetAsValue();
        }
        catch (CaesarException ex)
        {
          caesarException = ex;
        }
      }
      else
        unknownList.Add(part);
    }
    if (caesarException != null)
      throw caesarException;
  }
}
