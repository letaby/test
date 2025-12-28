// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DtsDbParameter
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;

#nullable disable
namespace Softing.Dts;

public interface DtsDbParameter : 
  MCDDbParameter,
  MCDDbObject,
  MCDNamedObject,
  MCDObject,
  IDisposable,
  DtsDbObject,
  DtsNamedObject,
  DtsObject
{
  DtsParameterMetaInfo ParameterMetaInformation { get; }

  string UnitTextID { get; }

  MCDValue GetInternalFromPhysicalValue(MCDValue pValue);

  MCDValue GetInternalValueFromPDUFragment(MCDValue pValue);

  MCDValue GetPDUFragmentFromInternalValue(MCDValue pValue);

  MCDValue GetPhysicalFromInternalValue(MCDValue pValue);

  bool HasPhysicalConstraint { get; }

  bool HasDbUnit { get; }

  MCDDataType CodedParameterType { get; }

  DtsDbDataObjectProp DbDataObjectProp { get; }

  uint DtsMaxNumberOfItems { get; }

  string TableRowShortName { get; }

  MCDDbDiagTroubleCodes DbDTCs { get; }

  bool HasDefaultValue { get; }

  bool HasInternalConstraint { get; }
}
