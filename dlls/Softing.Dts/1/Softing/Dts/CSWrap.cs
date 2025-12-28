// Decompiled with JetBrains decompiler
// Type: Softing.Dts.CSWrap
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;
using System.Runtime.InteropServices;

#nullable disable
namespace Softing.Dts;

internal class CSWrap
{
  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_createValue();

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_setEventListener(IntPtr targetObject, out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_releaseEventListener(
    IntPtr targetObject,
    uint listenerHandle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern void CSNIDTS_goodbyDts();

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern void CSNIDTS_registerCallback(onEvent callbackPointer);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern void CSNIDTS_releaseObject(IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern void CSNIDTS_writeToTrace(string message);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern void CSNIDTS_releaseStringArray(ref StringArray_Struct Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_allocate_string(uint size);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_cached_allocate(uint size);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_allocate(uint size);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern void CSNIDTS_releaseByteField(ref ByteField_Struct Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern MCDObjectType CSNIDTS_getObjectType(IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern void CSNIDTS_free(IntPtr memPointer);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern void CSNIDTS_cached_free(IntPtr memPointer);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_getSystem();

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsAccessKey_getLocationType(
    IntPtr Handle,
    out MCDLocationType returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsAccessKey_getString(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsAccessKey_getProtocol(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsAccessKey_getFunctionalGroup(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsAccessKey_getEcuBaseVariant(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsAccessKey_getEcuVariant(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsAccessKey_getMultipleEcuJob(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsAccessKeys_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsAudience_isSupplier(IntPtr Handle, out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsAudience_isDevelopment(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsAudience_isManufacturing(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsAudience_isAfterSales(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsAudience_isAfterMarket(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsConfigurationItem_getDbObject(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsConfigurationItem_getItemValue(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsConfigurationItem_hasError(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsConfigurationItem_getError(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsConfigurationRecord_getConfigurationIdItem(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsConfigurationRecord_getDbObject(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsConfigurationRecord_getError(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsConfigurationRecord_getErrors(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsConfigurationRecord_getOptionItems(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsConfigurationRecord_getReadDiagComPrimitives(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsConfigurationRecord_getSystemItems(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsConfigurationRecord_getWriteDiagComPrimitives(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsConfigurationRecord_hasError(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsConfigurationRecord_loadCodingData(
    IntPtr Handle,
    string _filename);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsConfigurationRecord_getDataIdItem(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsConfigurationRecord_getActiveFileName(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsConfigurationRecord_getConfigurationRecord(
    IntPtr Handle,
    out ByteField_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsConfigurationRecord_getMatchingFileNames(
    IntPtr Handle,
    out StringArray_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsConfigurationRecord_removeReadDiagComPrimitives(
    IntPtr Handle,
    IntPtr _readDiagComs);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsConfigurationRecord_removeWriteDiagComPrimitives(
    IntPtr Handle,
    IntPtr _writeDiagComs);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsConfigurationRecord_setConfigurationRecord(
    IntPtr Handle,
    ref ByteField_Struct _configRecordValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsConfigurationRecord_setConfigurationRecordByDbObject(
    IntPtr Handle,
    IntPtr _dbDataRecord);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsConfigurationRecords_setEventListener(
    IntPtr Handle,
    IntPtr _pDtsEventListener,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsConfigurationRecords_releaseEventListener(
    IntPtr Handle,
    uint _uEventLIstenerId);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsConfigurationRecords_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsConfigurationRecords_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsConfigurationRecords_remove(
    IntPtr Handle,
    IntPtr _ConfigurationRecord);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsConfigurationRecords_removeAll(IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsConfigurationRecords_removeByIndex(
    IntPtr Handle,
    uint _index);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsConfigurationRecords_removeByName(
    IntPtr Handle,
    string _name);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsConfigurationRecords_addByConfigurationIDAndDbConfigurationData(
    IntPtr Handle,
    IntPtr _ConfigurationID,
    IntPtr _configurationData,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsConfigurationRecords_addByDbObject(
    IntPtr Handle,
    IntPtr _DbConfigurationRecord,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsConfigurationRecords_addByNameAndDbConfigurationData(
    IntPtr Handle,
    string _dbConfigurationRecordName,
    IntPtr _dbConfigurationData,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsConstraint_getInterval(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsConstraint_getScaleConstraints(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsConstraint_isComputed(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDataPrimitive_getResultBufferSize(
    IntPtr Handle,
    out ushort returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDataPrimitive_setResultBufferSize(
    IntPtr Handle,
    ushort _bufferSize);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDataPrimitive_getRepetitionState(
    IntPtr Handle,
    out MCDRepetitionState returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDataPrimitive_getRepetitionTime(
    IntPtr Handle,
    out ushort returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDataPrimitive_setRepetitionTime(
    IntPtr Handle,
    ushort _repetitionTime);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDataPrimitive_startRepetition(IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDataPrimitive_stopRepetition(IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDataPrimitive_updateRepetitionParameters(IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDataPrimitive_executeAsync(IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDataPrimitive_fetchResults(
    IntPtr Handle,
    int _numReq,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDataPrimitive_getResultState(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDataPrimitive_startCyclicSend(
    IntPtr Handle,
    uint _cyclicTime,
    int _numSendCycles);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbAccessLevel_getAccessDiagComPrimitive(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbAccessLevel_getAccessLevelValue(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbAccessLevel_getExternalAccessMethod(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbAccessLevel_hasExternalAccessMethod(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbAdditionalAudiences_getItemByIndex(
    IntPtr Handle,
    uint _uIndex,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbAdditionalAudiences_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbBaseFunctionNode_getDbComponentConnectors(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbBaseFunctionNode_getDbFunctionInParams(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbBaseFunctionNode_getDbFunctionOutParams(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbBaseFunctionNode_getVersion(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbBaseFunctionNode_getAudienceState(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbBaseFunctionNode_getDbMultipleEcuJobs(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbCodeInformation_getCodeFile(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbCodeInformation_getEntryPoint(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbCodeInformation_getSyntax(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbCodeInformations_getItemByIndex(
    IntPtr Handle,
    uint _uIndex,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbCodeInformations_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbCodingData_getFileName(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbCodingData_isLateBound(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbComponentConnector_getDbEcuBaseVariant(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbComponentConnector_getDbEcuVariants(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbComponentConnector_getDbDiagObjectConnector(
    IntPtr Handle,
    IntPtr _locationContext,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbComponentConnector_getDbLocationsForDiagObjectConnector(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbComponentConnectors_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbConfigurationData_getDbConfigurationRecords(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbConfigurationData_getDbEcuBaseVariants(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbConfigurationData_getDbEcuVariants(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbConfigurationData_getVersion(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbConfigurationData_getDbAdditionalAudiences(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbConfigurationData_getDbSDGs(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbConfigurationDatas_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbConfigurationDatas_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbConfigurationItem_getBitLength(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbConfigurationItem_getBitPos(
    IntPtr Handle,
    out ushort returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbConfigurationItem_getBytePos(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbConfigurationItem_getDataType(
    IntPtr Handle,
    out MCDDataType returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbConfigurationItem_getDbUnit(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbConfigurationItem_getInternalConstraint(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbConfigurationItem_getInterval(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbConfigurationItem_getSemantic(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbConfigurationItem_getTextTableElements(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbConfigurationItem_isComplex(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbConfigurationItem_getDbSDGs(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbConfigurationRecord_getByteLength(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbConfigurationRecord_getConfigurationID(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbConfigurationRecord_getDbConfigurationIdItem(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbConfigurationRecord_getDbDataIdItem(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbConfigurationRecord_getDbDataRecords(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbConfigurationRecord_getDbDefaultDataRecord(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbConfigurationRecord_getDbOptionItems(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbConfigurationRecord_getDbReadDiagComPrimitives(
    IntPtr Handle,
    IntPtr _dbLocation,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbConfigurationRecord_getDbWriteDiagComPrimitives(
    IntPtr Handle,
    IntPtr _dbLocation,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbConfigurationRecord_getDbSystemItems(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbConfigurationRecord_getAudienceState(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbConfigurationRecord_getDbDisabledAdditionalAudiences(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbConfigurationRecord_getDbEnabledAdditionalAudiences(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbConfigurationRecord_getDbSDGs(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbConfigurationRecords_getItemByConfigurationID(
    IntPtr Handle,
    IntPtr _configurationID,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbConfigurationRecords_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbConfigurationRecords_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbControlPrimitives_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbControlPrimitives_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbControlPrimitives_getItemByType(
    IntPtr Handle,
    MCDObjectType _type,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDataPrimitive_getAccessLevel(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDataPrimitive_getRepetitionMode(
    IntPtr Handle,
    out MCDRepetitionMode returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDataPrimitive_getDbSDGs(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDataPrimitive_getRelatedDataPrimitives(
    IntPtr Handle,
    string _relationType,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDataPrimitive_getRepetitionTime(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDataPrimitive_getAudienceState(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDataPrimitive_getDbDisabledAdditionalAudiences(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDataPrimitive_getDbEnabledAdditionalAudiences(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDataPrimitives_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDataPrimitives_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDataRecord_getBinaryData(
    IntPtr Handle,
    out ByteField_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDataRecord_getDataFormat(
    IntPtr Handle,
    out MCDFlashDataFormat returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDataRecord_getDataID(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDataRecord_getDbCodingData(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDataRecord_getKey(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDataRecord_getRule(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDataRecord_getUserDefinedFormat(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDataRecord_getAudienceState(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDataRecord_getDbDisabledAdditionalAudiences(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDataRecord_getDbEnabledAdditionalAudiences(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDataRecord_getDbSDGs(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDataRecords_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDataRecords_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDataRecords_getItemByDataID(
    IntPtr Handle,
    IntPtr _dataID,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDataRecords_getItemByKey(
    IntPtr Handle,
    string _key,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDiagComPrimitive_getDbRequest(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDiagComPrimitive_getDbResponses(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDiagComPrimitive_getDbFunctionalClasses(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDiagComPrimitive_getSemantic(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDiagComPrimitive_getTransmissionMode(
    IntPtr Handle,
    out MCDTransmissionMode returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDiagComPrimitive_isApiExecutable(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDiagComPrimitive_isNoOperation(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDiagComPrimitive_getComPrimitiveType(
    IntPtr Handle,
    out DtsComPrimitiveType returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDiagComPrimitive_getID(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDiagComPrimitive_supportsPDUInformation(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDiagComPrimitive_getDbResponsesByType(
    IntPtr Handle,
    MCDResponseType _type,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDiagComPrimitive_getDbProtocolParameters(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDiagComPrimitive_getDbEcuStateTransitionsByDbObject(
    IntPtr Handle,
    IntPtr _chart,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDiagComPrimitive_getDbEcuStateTransitionsBySemantic(
    IntPtr Handle,
    string _semantic,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDiagComPrimitive_getDbPreConditionStatesByDbObject(
    IntPtr Handle,
    IntPtr _chart,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDiagComPrimitive_getDbPreConditionStatesBySemantic(
    IntPtr Handle,
    string _semantic,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDiagComPrimitive_getDID(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDiagComPrimitive_hasDID(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDiagComPrimitive_getInternalShortName(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDiagComPrimitives_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDiagComPrimitives_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDiagComPrimitives_getItemByType(
    IntPtr Handle,
    MCDObjectType _type,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDiagObjectConnector_getDbDiagTroubleCodeConnectors(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDiagObjectConnector_getDbEnvDataConnectors(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDiagObjectConnector_getDbFunctionDiagComConnectors(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDiagObjectConnector_getDbTableRowConnectors(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDiagService_getRuntimeMode(
    IntPtr Handle,
    out MCDRuntimeMode returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDiagServices_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDiagServices_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDiagTroubleCode_getDisplayTroubleCode(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDiagTroubleCode_getDTCText(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDiagTroubleCode_getLevel(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDiagTroubleCode_getTroubleCode(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDiagTroubleCode_getDbSDGs(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDiagTroubleCode_getDiagTroubleCodeTextID(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDiagTroubleCode_getUnicodeDTCText(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDiagTroubleCodeConnector_getDbDiagTroubleCode(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDiagTroubleCodeConnector_getDbFaultMemory(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDiagTroubleCodeConnectors_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDiagTroubleCodeConnectors_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDiagTroubleCodes_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDiagTroubleCodes_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDynIdClearComPrimitive_getDefinitionModes(
    IntPtr Handle,
    out StringArray_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDynIdDefineComPrimitive_getDefinitionModes(
    IntPtr Handle,
    out StringArray_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDynIdReadComPrimitive_getDefinitionModes(
    IntPtr Handle,
    out StringArray_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbEcu_getDbLocations(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbEcuBaseVariant_getDbEcuVariants(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbEcuBaseVariant_getDbBaseVariantPatterns(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbEcuBaseVariants_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbEcuBaseVariants_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbEcuMem_getBaseVariants(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbEcuMem_getFlashSessions(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbEcuMem_getVariants(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbEcuMem_getDbAdditionalAudiences(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbEcuMem_getDbSDGs(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbEcuMems_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbEcuMems_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbEcuState_getDbEcuStateTransitions(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbEcuState_getDbRestrictedDiagComPrimitives(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbEcuStateChart_getDbEcuStates(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbEcuStateChart_getDbEcuStateTransitions(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbEcuStateChart_getDbPreConditionStateDiagComPrimitives(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbEcuStateChart_getDbStartState(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbEcuStateChart_getDbStateTransitionDiagComPrimitives(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbEcuStateChart_getSemantic(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbEcuStateCharts_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbEcuStateCharts_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbEcuStateCharts_getItemBySemanticAttribute(
    IntPtr Handle,
    string _semantic,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbEcuStateCharts_getSemantics(
    IntPtr Handle,
    out StringArray_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbEcuStateTransition_getDbEcuStateTransitionActions(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbEcuStateTransition_getDbExternalAccessMethod(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbEcuStateTransition_getDbSourceState(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbEcuStateTransition_getDbTargetState(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbEcuStateTransition_hasDbExternalAccessMethod(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbEcuStateTransitionAction_getDbDiagComPrimitive(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbEcuStateTransitionAction_getDbRequestParameter(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbEcuStateTransitionAction_getValue(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbEcuStateTransitionActions_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbEcuStateTransitions_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbEcuStateTransitions_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbEcuStates_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbEcuStates_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbEcuVariant_getDbEcuBaseVariant(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbEcuVariants_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbEcuVariants_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbEnvDataConnector_getDbEnvData(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbEnvDataConnector_getDbEnvDataDesc(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbEnvDataConnectors_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbEnvDataConnectors_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbEnvDataDesc_getCommonDbEnvDatas(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbEnvDataDesc_getCompleteDbEnvDatasByDiagTroubleCode(
    IntPtr Handle,
    uint _troublecode,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbEnvDataDesc_getSpecificDbEnvDatasForDiagTroubleCode(
    IntPtr Handle,
    uint _troublecode,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbEnvDataDescs_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbEnvDataDescs_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbExternalAccessMethod_getMethod(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFaultMemories_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFaultMemories_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFaultMemory_getDbDiagTroubleCodeByTroubleCode(
    IntPtr Handle,
    uint _troublecode,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFaultMemory_getDbDiagTroubleCodes(
    IntPtr Handle,
    ushort _levelLowerLimit,
    ushort _levelUpperLimit,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashChecksum_getChecksumAlgorithm(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashChecksum_getChecksumResult(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashChecksum_getUncompressedSize(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashChecksum_getCompressedSize(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashChecksum_getFillByte(
    IntPtr Handle,
    out ByteField_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashChecksum_getSourceStartAddress(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashChecksum_getSourceEndAddress(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashChecksum_getUncompressedSize64(
    IntPtr Handle,
    out ulong returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashChecksum_getSourceStartAddress64(
    IntPtr Handle,
    out ulong returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashChecksum_getSourceEndAddress64(
    IntPtr Handle,
    out ulong returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashChecksum_getCompressedSize64(
    IntPtr Handle,
    out ulong returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashChecksums_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashChecksums_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashData_getDataFormat(
    IntPtr Handle,
    out MCDFlashDataFormat returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashData_getEncryptionCompressionMethod(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashData_getDataFileName(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashData_setTemporaryDataFileName(
    IntPtr Handle,
    string _temporaryDataFileName);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashData_resetTemporaryDataFileName(IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashData_isLateBound(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashData_getActiveFileName(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashData_getFileName(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashData_getTemporaryDataFileName(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashData_getMatchingFileNames(
    IntPtr Handle,
    out StringArray_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashData_setActiveFileName(
    IntPtr Handle,
    string _temporaryDataFileName);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashData_getDatabaseFileName(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashDataBlock_getDbFlashSegments(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashDataBlock_getDbFlashData(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashDataBlock_getDbOwnIdents(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashDataBlock_getDbFlashFilters(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashDataBlock_getAddressOffset(
    IntPtr Handle,
    out long returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashDataBlock_getDataBlockType(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashDataBlock_getDbSecurities(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashDataBlock_getDbSecuritiesAsSecurities(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashDataBlock_getDbSDGs(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashDataBlock_loadSegments(
    IntPtr Handle,
    string _filename);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashDataBlock_getAudienceState(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashDataBlock_getDbDisabledAdditionalAudiences(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashDataBlock_getDbEnabledAdditionalAudiences(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashDataBlocks_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashDataBlocks_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashFilter_getFilterStart(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashFilter_getFilterEnd(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashFilter_getFilterSize(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashFilter_getFilterEnd64(
    IntPtr Handle,
    out ulong returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashFilter_getFilterSize64(
    IntPtr Handle,
    out ulong returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashFilter_getFilterStart64(
    IntPtr Handle,
    out ulong returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashFilters_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashFilters_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashIdent_getIdentValues(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashIdent_getReadDbIdentDescription(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashIdents_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashIdents_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashSecurities_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashSecurities_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashSecurity_getSecurityMethod(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashSecurity_getValidity(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashSecurity_getFlashwareSignature(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashSecurity_getFlashwareChecksum(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashSegment_getUncompressedSize(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashSegment_getCompressedSize(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashSegment_getBinaryData(
    IntPtr Handle,
    out ByteField_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashSegment_getSourceStartAddress(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashSegment_getSourceEndAddress(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashSegment_getBinaryDataOffset(
    IntPtr Handle,
    uint _uOffset,
    uint _uLength,
    out ByteField_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashSegment_getFirstBinaryDataChunk(
    IntPtr Handle,
    uint _size,
    out ByteField_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashSegment_hasNextBinaryDataChunk(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashSegment_getNextBinaryDataChunk(
    IntPtr Handle,
    out ByteField_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashSegment_createFlashSegmentIterator(
    IntPtr Handle,
    uint _size,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashSegment_removeFlashSegmentIterator(
    IntPtr Handle,
    IntPtr _flashSegmentIterator);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashSegment_getSourceEndAddress64(
    IntPtr Handle,
    out ulong returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashSegment_getSourceStartAddress64(
    IntPtr Handle,
    out ulong returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashSegment_getUncompressedSize64(
    IntPtr Handle,
    out ulong returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashSegment_getCompressedSize64(
    IntPtr Handle,
    out ulong returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashSegment_getBinaryDataOffset64(
    IntPtr Handle,
    ulong _uOffset,
    ulong _uLength,
    out ByteField_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashSegments_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashSegments_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashSession_getDbFlashSessionsClasses(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashSession_getFlashKey(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashSession_getChecksums(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashSession_getDbExpectedIdents(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashSession_getDbSecurities(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashSession_getDbDataBlocks(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashSession_getDbFlashJob(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashSession_getPriority(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashSession_isDownload(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashSession_getDbSDGs(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashSession_getDbFlashJobByLocation(
    IntPtr Handle,
    IntPtr _pDtsDbLocation,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashSession_getFlashJobName(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashSession_getAllVariantReferences(
    IntPtr Handle,
    out StringArray_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashSession_getLayerReferences(
    IntPtr Handle,
    out StringArray_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashSessionClass_getDbFlashSessions(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashSessionClasses_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashSessionClasses_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashSessions_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashSessions_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashSessions_getDbFlashSessionByFlashKey(
    IntPtr Handle,
    string _flashkey,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFlashSessions_getFlashKeyPriority(
    IntPtr Handle,
    string _flashkey,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFunctionDiagComConnector_getDbDiagComPrimitive(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFunctionDiagComConnector_getDbLogicalLink(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFunctionDiagComConnectors_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFunctionDiagComConnectors_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFunctionDictionaries_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFunctionDictionaries_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFunctionDictionary_getDbFunctionNodeGroups(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFunctionDictionary_getDbFunctionNodes(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFunctionDictionary_getDbAdditionalAudiences(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFunctionDictionary_getDbSDGs(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFunctionInParameter_getDataType(
    IntPtr Handle,
    out MCDDataType returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFunctionInParameter_getDbFunctionDiagComConnector(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFunctionInParameter_getDbRequestParameter(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFunctionInParameter_getDbUnit(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFunctionInParameters_getDbLocationsForItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFunctionInParameters_getDbLocationsForItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFunctionInParameters_getItemByIndex(
    IntPtr Handle,
    uint _index,
    IntPtr _locationContext,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFunctionInParameters_getItemByName(
    IntPtr Handle,
    string _name,
    IntPtr _locationContext,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFunctionNode_getDbFunctionNodes(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFunctionNode_getDbSDGs(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFunctionNode_getDbEnabledAdditionalAudiences(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFunctionNode_getDbDisabledAdditionalAudiences(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFunctionNodeGroup_getDbFunctionNodeGroups(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFunctionNodeGroup_getDbFunctionNodes(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFunctionNodeGroup_getDbEnabledAdditionalAudiences(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFunctionNodeGroup_getDbDisabledAdditionalAudiences(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFunctionNodeGroups_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFunctionNodeGroups_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFunctionNodes_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFunctionNodes_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFunctionOutParameter_getDataType(
    IntPtr Handle,
    out MCDDataType returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFunctionOutParameter_getDbFunctionDiagComConnector(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFunctionOutParameter_getDbResponseParameter(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFunctionOutParameter_getDbUnit(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFunctionOutParameters_getDbLocationsForItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFunctionOutParameters_getDbLocationsForItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFunctionOutParameters_getItemByIndex(
    IntPtr Handle,
    uint _index,
    IntPtr _locationContext,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFunctionOutParameters_getItemByName(
    IntPtr Handle,
    string _name,
    IntPtr _locationContext,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFunctionalClass_getDbLocation(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFunctionalClass_getDbDataPrimitives(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFunctionalClasses_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFunctionalClasses_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFunctionalGroup_getGroupMembers(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFunctionalGroups_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbFunctionalGroups_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbIdentDescription_getDbDataPrimitive(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbIdentDescription_getDbResponseParameter(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbIdentDescription_getDbDataPrimitiveByLocation(
    IntPtr Handle,
    IntPtr _pDtsDbLocation,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbInterfaceCables_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbInterfaceCables_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbInterfaceCable_getDbInterfaceConnectorPins(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbInterfaceCable_getInterfaceConnectorType(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbInterfaceConnectorPin_getPinNumber(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbInterfaceConnectorPin_getPinNumberOnVCI(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbInterfaceConnectorPin_getPinType(
    IntPtr Handle,
    out MCDConnectorPinType returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbInterfaceConnectorPins_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbInterfaceConnectorPins_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbItemValue_getPhysicalConstantValue(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbItemValue_getDescription(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbItemValue_getDescriptionID(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbItemValue_getKey(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbItemValue_getMeaning(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbItemValue_getMeaningID(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbItemValue_getRule(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbItemValue_getAudienceState(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbItemValue_getDbDisabledAdditionalAudiences(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbItemValue_getDbEnabledAdditionalAudiences(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbItemValue_getDbSDGs(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbItemValues_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbItemValues_getItemByKey(
    IntPtr Handle,
    string _key,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbItemValues_getItemByPhysicalConstantValue(
    IntPtr Handle,
    IntPtr _physicalConstantValue,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbJob_getVersion(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbJob_getParentName(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbJob_isReducedResultEnabled(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbJob_getSourceFilePath(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbJob_getDbCodeInformations(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbJobs_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbJobs_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbMatchingParameter_getDbDiagComPrimitive(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbMatchingParameter_getDbResponseParameter(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbMatchingParameter_getExpectedValue(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbMatchingParameters_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbMatchingPattern_getDbMatchingParameters(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbMatchingPatterns_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbMultipleEcuJob_getDbLocations(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbOptionItem_getDbItemValues(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbOptionItem_getDecimalPlaces(
    IntPtr Handle,
    out ushort returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbOptionItem_getPhysicalDefaultValue(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbOptionItem_getReadAudienceState(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbOptionItem_getWriteAudienceState(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbOptionItem_getDbDisabledReadAdditionalAudiences(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbOptionItem_getDbEnabledReadAdditionalAudiences(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbOptionItem_getDbDisabledWriteAdditionalAudiences(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbOptionItem_getDbEnabledWriteAdditionalAudiences(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbOptionItems_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbOptionItems_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbParameter_getBytePos(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbParameter_getBitPos(
    IntPtr Handle,
    out byte returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbParameter_getByteLength(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbParameter_getDecimalPlaces(
    IntPtr Handle,
    out ushort returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbParameter_getDefaultValue(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbParameter_getMaxLength(
    IntPtr Handle,
    out ushort returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbParameter_getMinLength(
    IntPtr Handle,
    out ushort returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbParameter_getParameterType(
    IntPtr Handle,
    out MCDDataType returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbParameter_getRadix(
    IntPtr Handle,
    out ushort returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbParameter_getUnit(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbParameter_getDisplayLevel(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbParameter_getSemantic(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbParameter_getInterval(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbParameter_getDbParameters(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbParameter_getParameterMetaInformation(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbParameter_isConstant(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbParameter_getInternalConstraint(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbParameter_getUnitTextID(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbParameter_getBitLength(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbParameter_getDbTable(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbParameter_getDbTableKeyParam(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbParameter_getDbTableStructParams(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbParameter_getKeys(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbParameter_getStructureByKey(
    IntPtr Handle,
    IntPtr _key,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbParameter_getTextTableElements(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbParameter_getMCDParameterType(
    IntPtr Handle,
    out MCDParameterType returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbParameter_getDataType(
    IntPtr Handle,
    out MCDDataType returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbParameter_getInternalFromPhysicalValue(
    IntPtr Handle,
    IntPtr _pValue,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbParameter_getInternalValueFromPDUFragment(
    IntPtr Handle,
    IntPtr _pValue,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbParameter_getPDUFragmentFromInternalValue(
    IntPtr Handle,
    IntPtr _pValue,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbParameter_getPhysicalFromInternalValue(
    IntPtr Handle,
    IntPtr _pValue,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbParameter_getCodedDefaultValue(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbParameter_getValidInternalIntervals(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbParameter_getValidPhysicalIntervals(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbParameter_getPhysicalConstraint(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbParameter_getDbSDGs(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbParameter_getODXBytePos(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbParameter_getDbUnit(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbParameter_getLengthKey(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbParameter_isVariableLength(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbParameter_hasPhysicalConstraint(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbParameter_hasDbUnit(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbParameter_getCodedParameterType(
    IntPtr Handle,
    out MCDDataType returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbParameter_getDbDataObjectProp(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbParameter_getDtsMaxNumberOfItems(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbParameter_getTableRowShortName(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbParameter_getDbDTCs(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbParameter_hasDefaultValue(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbParameter_hasInternalConstraint(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbParameters_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbParameters_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbPhysicalMemories_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbPhysicalMemories_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbPhysicalMemory_getPhysicalSegments(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbPhysicalSegment_getBlockSize(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbPhysicalSegment_getEndAddress(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbPhysicalSegment_getFillByte(
    IntPtr Handle,
    out ByteField_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbPhysicalSegment_getStartAddress(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbPhysicalSegments_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbPhysicalSegments_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbPhysicalVehicleLink_getDbVehicleConnectorPins(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbPhysicalVehicleLink_getType(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbPreconditionDefinition_getDbDiagComPrimitive(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbPreconditionDefinition_getDbRequestParameter(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbPreconditionDefinition_getValue(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbPreconditionDefinitions_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbRequest_getDefaultPDU(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbRequest_getPDUMinLength(
    IntPtr Handle,
    out ushort returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbRequest_getPDUMaxLength(
    IntPtr Handle,
    out ushort returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbRequest_getDbRequestParameters(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbRequest_getDbSDGs(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbRequestParameter_getMaxNumberOfItems(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbRequestParameters_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbRequestParameters_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbResponse_getID(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbResponse_isNegativeResponse(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbResponse_getDbResponseParameters(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbResponse_getResponseType(
    IntPtr Handle,
    out MCDResponseType returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbResponse_getDbSDGs(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbResponseParameter_getNrcConstValues(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbResponseParameter_getRequestBytePos(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbResponseParameters_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbResponseParameters_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbResponses_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbResponses_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbService_getAddressingMode(
    IntPtr Handle,
    out MCDAddressingMode returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbService_hasSuppressPositiveResponseCapability(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbService_generateResult(
    IntPtr Handle,
    ref ByteField_Struct _pRequest,
    ref ByteField_Struct _pResponse,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbServices_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbServices_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbSpecialData_getSemanticInformation(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbSpecialDataElement_getContent(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbSpecialDataElement_getTextID(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbSpecialDataGroup_getCount(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbSpecialDataGroup_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbSpecialDataGroup_getCaption(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbSpecialDataGroup_hasCaption(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbSpecialDataGroups_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbSubComponent_getDbDiagTroubleCodeConnectors(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbSubComponent_getDbEnvDataConnectors(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbSubComponent_getDbSubComponentParamConnectors(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbSubComponent_getDbSubComponentPatterns(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbSubComponent_getDbTableRowConnectors(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbSubComponent_getSemantic(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbSubComponentParamConnector_getDbDiagComPrimitive(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbSubComponentParamConnector_getDbSubComponentInParams(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbSubComponentParamConnector_getDbSubComponentOutParams(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbSubComponentParamConnectors_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbSubComponentParamConnectors_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbSubComponents_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbSubComponents_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbSubComponents_getSemantics(
    IntPtr Handle,
    out StringArray_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbSubComponents_getItemsBySemanticAttribute(
    IntPtr Handle,
    string _semantic,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbSystemItem_getSystemParameterName(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbSystemItems_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbSystemItems_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbTable_getDbDiagComPrimitiveByConnectorSemantic(
    IntPtr Handle,
    string _semantic,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbTable_getDbDiagComPrimitives(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbTable_getKeys(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbTable_getSemantic(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbTable_getDbTableRows(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbTable_getDbSDGs(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbTableParameter_getKey(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbTableParameter_getAudienceState(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbTableParameter_getDbDisabledAdditionalAudiences(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbTableParameter_getDbEnabledAdditionalAudiences(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbTableParameter_isApiExecutable(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbTableParameters_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbTableParameters_getItemByKey(
    IntPtr Handle,
    IntPtr _key,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbTableParameters_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbTableRowConnector_getDbTable(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbTableRowConnector_getDbTableRow(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbTableRowConnectors_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbTableRowConnectors_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbTables_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbTables_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbUnitGroup_getCategory(
    IntPtr Handle,
    out MCDUnitGroupCategory returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbUnitGroup_getUnits(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbUnitGroups_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbUnitGroups_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbVehicleConnector_getDbVehicleConnectorPins(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbVehicleConnectorPin_getDbVehicleConnector(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbVehicleConnectorPin_getPinNumber(
    IntPtr Handle,
    out ushort returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbVehicleConnectorPins_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbVehicleConnectorPins_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbVehicleConnectors_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbVehicleConnectors_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDiagComPrimitive_setEventListener(
    IntPtr Handle,
    IntPtr _pDtsEventListener,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDiagComPrimitive_releaseEventListener(
    IntPtr Handle,
    uint _uEventLIstenerId);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDiagComPrimitive_cancel(IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDiagComPrimitive_executeSync(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDiagComPrimitive_getDbObject(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDiagComPrimitive_getErrors(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDiagComPrimitive_getRequest(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDiagComPrimitive_resetToDefaultValue(
    IntPtr Handle,
    string _parameterName);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDiagComPrimitive_resetToDefaultValues(IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDiagComPrimitive_executeSyncWithResultState(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDiagComPrimitive_executeSyncWithResults(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDiagComPrimitive_getParent(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDiagComPrimitive_getUniqueRuntimeID(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDiagComPrimitive_getState(
    IntPtr Handle,
    out MCDDiagComPrimitiveState returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDiagComPrimitive_executeAsync(IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDiagComPrimitive_fetchResults(
    IntPtr Handle,
    int _numReq,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDiagComPrimitive_getResultState(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDiagComPrimitive_getInternalShortName(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDiagComPrimitives_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDynIdClearComPrimitive_getDynId(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDynIdClearComPrimitive_setDynId(
    IntPtr Handle,
    IntPtr _id);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDynIdClearComPrimitive_getDefinitionMode(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDynIdClearComPrimitive_setDefinitionMode(
    IntPtr Handle,
    string _definitionMode);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDynIdDefineComPrimitive_setDynIdParams(
    IntPtr Handle,
    ref StringArray_Struct _paramnames);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDynIdDefineComPrimitive_getDynId(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDynIdDefineComPrimitive_setDynId(
    IntPtr Handle,
    IntPtr _id);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDynIdDefineComPrimitive_getDefinitionMode(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDynIdDefineComPrimitive_setDefinitionMode(
    IntPtr Handle,
    string _definitionMode);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDynIdReadComPrimitive_getDynId(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDynIdReadComPrimitive_setDynId(IntPtr Handle, IntPtr _id);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDynIdReadComPrimitive_setDefinitionMode(
    IntPtr Handle,
    string _definitionMode);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDynIdReadComPrimitive_getDefinitionMode(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsEnumValue_getEnumFromString(
    IntPtr Handle,
    string _enumString,
    out int returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsEnumValue_getStringFromEnum(
    IntPtr Handle,
    int _enumValue,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsErrors_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsFlashJob_setSessionByFlashKey(
    IntPtr Handle,
    string _flashKeyString);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsFlashJob_setSessionByName(
    IntPtr Handle,
    string _flashSessionName);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsFlashJob_setSession(IntPtr Handle, IntPtr _flashSession);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsFlashJob_getFlashSession(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsFlashSegmentIterator_getBinaryDataChunkSize(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsFlashSegmentIterator_getFirstBinaryDataChunk(
    IntPtr Handle,
    out ByteField_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsFlashSegmentIterator_getNextBinaryDataChunk(
    IntPtr Handle,
    out ByteField_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsFlashSegmentIterator_hasNextBinaryDataChunk(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsHexService_setServiceID(IntPtr Handle, uint _serviceID);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsHexService_getServiceID(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterface_getClampState(
    IntPtr Handle,
    uint _pinOnInterfaceConnector,
    string _clampName,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterface_getPDUApiSoftwareName(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterface_getPDUApiSoftwareVersion(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterface_getVendorName(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterface_setEthernetActivation(
    IntPtr Handle,
    bool _setEthernetActivation);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterface_getEthernetActivation(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterface_executeBroadcast(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterface_connect(IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterface_disconnect(IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterface_getHardwareSerialNumber(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterface_execIOCtrl(
    IntPtr Handle,
    string _IOCtrlName,
    IntPtr _inputData,
    uint _inputDataItemType,
    uint _outputDataSize,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterface_getIOControlNames(
    IntPtr Handle,
    out StringArray_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterface_detectInterfaces(
    IntPtr Handle,
    string _optionString);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterface_getCurrentDbInterfaceCable(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterface_getCurrentTime(
    IntPtr Handle,
    out ulong returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterface_getDbInterfaceCables(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterface_getFirmwareName(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterface_getFirmwareVersion(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterface_getHardwareName(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterface_getHardwareVersion(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterface_getInterfaceResources(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterface_getMVCIVersionPart1StandardVersion(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterface_getMVCIVersionPart2StandardVersion(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterface_getStatus(
    IntPtr Handle,
    out MCDInterfaceStatus returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterface_reset(IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterface_getProgrammingVoltage(
    IntPtr Handle,
    uint _pinOnInterfaceConnector,
    out double returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterface_getBatteryVoltage(
    IntPtr Handle,
    out double returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterface_setProgrammingVoltage(
    IntPtr Handle,
    uint _pinOnInterfaceConnector,
    double _voltage);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterface_setEthernetPinState(
    IntPtr Handle,
    bool _State,
    uint _Number);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterface_getEthernetPinState(
    IntPtr Handle,
    uint _Number,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterface_getVendorModuleName(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterface_setPhysicalLinkId(
    IntPtr Handle,
    string _keyLink,
    uint _id,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterface_getWLanSignalData(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterfaceResource_getDbPhysicalInterfaceLink(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterfaceResource_getInterface(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterfaceResource_getProtocolType(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterfaceResource_isAvailable(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterfaceResource_isInUse(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterfaceResources_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterfaceResources_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterfaces_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterfaces_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterval_getLowerLimit(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterval_getLowerLimitIntervalType(
    IntPtr Handle,
    out MCDLimitType returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterval_getUpperLimit(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterval_getUpperLimitIntervalType(
    IntPtr Handle,
    out MCDLimitType returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsIntervals_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsJob_createResult(
    IntPtr Handle,
    MCDResultType _resultType,
    MCDErrorCodes _code,
    string _codeDescription,
    ushort _vendorCode,
    string _vendorCodeDescription,
    MCDSeverity _severity,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsJob_releaseResult(IntPtr Handle, IntPtr _result);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsJob_getProgress(IntPtr Handle, out byte returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsJob_getJobInfo(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsJob_createResultCollection(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsJob_getRequestParameters(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsJob_getLogicalLink(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsJob_onJobFinished(IntPtr Handle, IntPtr _error);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsJob_getJobApi(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsJob_getJavaProgCodeInfo(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsMessageFilter_enableMessageFilter(
    IntPtr Handle,
    bool _enableMessageFilter);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsMessageFilter_getFilterCompareSize(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsMessageFilter_getFilterId(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsMessageFilter_getFilterMasks(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsMessageFilter_getFilterPatterns(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsMessageFilter_getFilterType(
    IntPtr Handle,
    out MCDMessageFilterType returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsMessageFilter_isMessageFilterEnabled(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsMessageFilter_setFilterCompareSize(
    IntPtr Handle,
    uint _filterCompareSize);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsMessageFilter_setFilterType(
    IntPtr Handle,
    MCDMessageFilterType _messageFilterType);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsMessageFilterValues_remove(
    IntPtr Handle,
    IntPtr _filterValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsMessageFilterValues_removeAll(IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsMessageFilterValues_removeByIndex(
    IntPtr Handle,
    uint _index);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsMessageFilterValues_add(
    IntPtr Handle,
    IntPtr _filterValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsMessageFilters_addByFilterType(
    IntPtr Handle,
    MCDMessageFilterType _messageFilterType,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsMessageFilters_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsMessageFilters_remove(
    IntPtr Handle,
    IntPtr _messageFilter);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsMessageFilters_removeAll(IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsMessageFilters_removeByIndex(IntPtr Handle, uint _index);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsMonitoringLink_fetchMonitoringFrames(
    IntPtr Handle,
    uint _numReq,
    out StringArray_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsMonitoringLink_getInterfaceResource(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsMonitoringLink_getMessageFilters(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsMonitoringLink_setNoOfSampleToFireEvent(
    IntPtr Handle,
    ushort _noOfSamples);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsMonitoringLink_setTimeToFireEvent(
    IntPtr Handle,
    ushort _milliseconds);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsMonitoringLink_start(IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsMonitoringLink_stop(IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsMonitoringLink_startFileTrace(IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsMonitoringLink_stopFileTrace(IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsMonitoringLink_openFileTrace(
    IntPtr Handle,
    string _FileName);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsMonitoringLink_closeFileTrace(IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsMonitoringLink_fetchDtsMonitorFrames(
    IntPtr Handle,
    uint _numReq,
    out ByteField_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsOptionItem_getDecimalPlaces(
    IntPtr Handle,
    out ushort returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsOptionItem_getMatchingDbItemValue(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsOptionItem_setItemValue(IntPtr Handle, IntPtr _value);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsOptionItem_setItemValueByDbObject(
    IntPtr Handle,
    IntPtr _dbItemValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsOptionItem_getItemValueDts(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsOptionItem_setItemValueDts(
    IntPtr Handle,
    IntPtr _pDtsValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsOptionItems_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsOptionItems_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProtocolParameterSet_getParameterLevel(
    IntPtr Handle,
    string _shortName,
    out DtsProtocolParameterLevel returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProtocolParameterSet_fetchValueFromInterface(
    IntPtr Handle,
    string _shortName);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProtocolParameterSet_fetchValuesFromInterface(
    IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsReadDiagComPrimitives_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsRequest_createValue(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsRequest_enterPDU(IntPtr Handle, IntPtr _pdu);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsRequest_getDbObject(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsRequest_getPDU(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsRequest_getRequestParameters(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsRequest_hasPDU(IntPtr Handle, out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsRequestParameter_getParameters(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsRequestParameter_createValue(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsRequestParameter_addParameters(
    IntPtr Handle,
    uint _count);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsRequestParameter_setValueUnchecked(
    IntPtr Handle,
    IntPtr _value);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsRequestParameter_getLengthKey(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsRequestParameter_isVariableLength(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsRequestParameter_setValuePDUConform(
    IntPtr Handle,
    IntPtr _value);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsRequestParameters_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsRequestParameters_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsResultState_getNoOfResults(
    IntPtr Handle,
    out ushort returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsResultState_getExecutionState(
    IntPtr Handle,
    out MCDExecutionState returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsResultState_getRepetitionState(
    IntPtr Handle,
    out MCDRepetitionState returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsResultState_getLogicalLinkState(
    IntPtr Handle,
    out MCDLogicalLinkState returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsResultState_getLogicalLinkLockState(
    IntPtr Handle,
    out MCDLockState returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsResultState_hasError(IntPtr Handle, out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsResultState_getError(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsScaleConstraint_getDescription(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsScaleConstraint_getDescriptionID(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsScaleConstraint_getInterval(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsScaleConstraint_getRangeInfo(
    IntPtr Handle,
    out MCDRangeInfo returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsScaleConstraint_getShortLabel(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsScaleConstraint_getShortLabelID(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsScaleConstraint_isComputed(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsScaleConstraints_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsService_getParameterValueRangeInfo(
    IntPtr Handle,
    string _shortName,
    out MCDRangeInfo returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsService_setParameterValueByRangeInfo(
    IntPtr Handle,
    string _shortName,
    MCDRangeInfo _rangeInfo);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsService_getRuntimeTransmissionMode(
    IntPtr Handle,
    out MCDTransmissionMode returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsService_setRuntimeTransmissionMode(
    IntPtr Handle,
    MCDTransmissionMode _mode);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsService_getDefaultResultBufferSize(
    IntPtr Handle,
    out ushort returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsService_isSuppressPositiveResponse(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsService_setSuppressPositiveResponse(
    IntPtr Handle,
    bool _suppress);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsService_hasSuppressPositiveResponseCapability(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsService_getResponse(
    IntPtr Handle,
    IntPtr _pDbResponse,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsStartCommunication_hasSuppressPositiveResponseCapability(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsStartCommunication_isSuppressPositiveResponse(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsStartCommunication_setSuppressPositiveResponse(
    IntPtr Handle,
    bool _suppress);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsStopCommunication_hasSuppressPositiveResponseCapability(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsStopCommunication_isSuppressPositiveResponse(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsStopCommunication_setSuppressPositiveResponse(
    IntPtr Handle,
    bool _suppress);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystemItems_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystemItems_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTextTableElement_getLongNameID(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTextTableElement_getInterval(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTextTableElement_getLongName(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTextTableElement_getDescription(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTextTableElements_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsWriteDiagComPrimitives_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsCollection_getCount(IntPtr Handle, out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsCollection_getObjectItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbLocation_getAccessKey(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbLocation_getDbECU(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbLocation_getProtocolLocation(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbLocation_getType(
    IntPtr Handle,
    out MCDLocationType returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbLocation_getDbServices(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbLocation_getDbFunctionalClasses(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbLocation_getDbFlashSessionClasses(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbLocation_getDbFlashSessions(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbLocation_hasDbVariantCodingDomains(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbLocation_getDbVariantCodingDomains(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbLocation_createOfflineVariantCoding(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbLocation_getDbPhysicalMemories(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbLocation_getDbControlPrimitives(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbLocation_getDbDataPrimitives(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbLocation_getDbDiagComPrimitives(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbLocation_getDbDiagServices(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbLocation_getDbDiagVariables(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbLocation_getDbDTCs(
    IntPtr Handle,
    ushort _levelLowLimit,
    ushort _levelUpLimit,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbLocation_getDbDynDefinedSpecTables(
    IntPtr Handle,
    out StringArray_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbLocation_getDbDynDefinedSpecTableByName(
    IntPtr Handle,
    string _shortName,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbLocation_getDbDynDefinedSpecTableByDefinitionMode(
    IntPtr Handle,
    string _definitionMode,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbLocation_getDbEnvDataByTroubleCode(
    IntPtr Handle,
    uint _troubleCode,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbLocation_getDbJobs(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbLocation_getDbServicesBySemanticAttribute(
    IntPtr Handle,
    string _semantic,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbLocation_getSupportedDynIds(
    IntPtr Handle,
    string _definitionMode,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbLocation_getUnitGroups(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbLocation_getAuthorizationMethods(
    IntPtr Handle,
    out StringArray_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbLocation_getDbDiagComPrimitivesByType(
    IntPtr Handle,
    MCDObjectType _type,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbLocation_isOnboard(IntPtr Handle, out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbLocation_getDbSubComponents(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbLocation_getDbConfigurationDatas(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbLocation_getDbTables(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbLocation_getDbTablesBySemanticAttribute(
    IntPtr Handle,
    string _semantic,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbLocation_getDbFaultMemories(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbLocation_getDbUnits(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbLocation_getVersion(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbLocation_getDataBaseType(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbLocation_getDbVariantPatterns(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbLocation_getDbAdditionalAudiences(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbLocation_getDbEnvDataDescs(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbLocation_getDbDiagComPrimitivesBySemanticAttribute(
    IntPtr Handle,
    string _sematic,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbLocation_getDbEcuStateCharts(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbLocation_getDbSDGs(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbLocation_getDbTableByDefinitionMode(
    IntPtr Handle,
    string _definitionMode,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbLocation_isLinLocation(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbLocation_isUdsLocation(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbLocation_getLogicalAddressValue(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbLocations_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbLocations_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbLogicalLink_getDbLocation(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbLogicalLink_getGatewayDbLogicalLink(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbLogicalLink_getDbPhysicalVehicleLinkOrInterface(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbLogicalLink_isGateway(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbLogicalLink_viaGateway(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbLogicalLink_isAccessedViaGateway(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbLogicalLink_getGatewayMode(
    IntPtr Handle,
    out MCDGatewayMode returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbLogicalLink_getDbLogicalLinksOfGateways(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbLogicalLink_getSourceIdentifier(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbLogicalLink_getTargetIdentifier(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbLogicalLink_getCANType(
    IntPtr Handle,
    out DtsPhysicalLinkOrInterfaceType returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbLogicalLink_getCommunicationParameters(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbLogicalLink_getDbPhysicalVehicleLink(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbLogicalLink_getProtocolType(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbLogicalLinks_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbLogicalLinks_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbObject_getLongNameID(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbObject_getDescriptionID(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbObject_getUniqueObjectIdentifier(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbObject_getDatabaseFile(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbPhysicalDimension_getCurrentExponent(
    IntPtr Handle,
    out int returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbPhysicalDimension_getLengthExponent(
    IntPtr Handle,
    out int returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbPhysicalDimension_getLuminousIntensity(
    IntPtr Handle,
    out int returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbPhysicalDimension_getMassExponent(
    IntPtr Handle,
    out int returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbPhysicalDimension_getMolarAmountExponent(
    IntPtr Handle,
    out int returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbPhysicalDimension_getTemperatureExponent(
    IntPtr Handle,
    out int returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbPhysicalDimension_getTimeExponent(
    IntPtr Handle,
    out int returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbPhysicalVehicleLinkOrInterface_getType(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbPhysicalVehicleLinkOrInterface_getDbVehicleConnectorPins(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbPhysicalVehicleLinkOrInterface_getPILType(
    IntPtr Handle,
    out DtsPhysicalLinkOrInterfaceType returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbPhysicalVehicleLinkOrInterface_getCommunicationParameters(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbPhysicalVehicleLinkOrInterface_getDbInterfaceConnectorPins(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbPhysicalVehicleLinkOrInterface_getPhysicalLinkId(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbPhysicalVehicleLinkOrInterface_getPhysicalLinkKey(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbPhysicalVehicleLinkOrInterfaces_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbPhysicalVehicleLinkOrInterfaces_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbProject_getAccessKeys(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbProject_getDbEcuBaseVariants(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbProject_getDbFunctionalGroups(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbProject_getDbProtocolLocations(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbProject_getDbVehicleInformations(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbProject_getVersion(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbProject_getDbElementByAccessKey(
    IntPtr Handle,
    IntPtr _pAccessKey,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbProject_getDbPhysicalVehicleLinkOrInterfaces(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbProject_getDbEcuMems(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbProject_loadNewECUMEM(
    IntPtr Handle,
    string _ecumemName,
    bool _permanent);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbProject_getDbMultipleEcuJobLocation(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbProject_getDbODXFiles(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbProject_getRevisionLabel(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbProject_getDbFunctionDictionaries(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbProject_getDbConfigurationDatas(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbProject_loadNewConfigurationDatasByFileName(
    IntPtr Handle,
    string _filename,
    bool _permanent,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbProject_getProjectType(
    IntPtr Handle,
    out DtsProjectType returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbProject_getVehicleModelRange(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbProject_getIdentifierInfos(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbProject_createIdentifierInfos(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbProject_getCanFilters(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbProjectConfiguration_getActiveDbProject(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbProjectConfiguration_close(IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbProjectConfiguration_load(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbProjectConfiguration_getAdditionalECUMEMNames(
    IntPtr Handle,
    out StringArray_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbProjectConfiguration_unloadConfigurationProject(
    IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbProjectDescriptions_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbProjectDescriptions_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbUnit_getDbPhysicalDimension(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbUnit_getDbUnitGroups(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbUnit_getDisplayName(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbUnit_getFactorSItoUnit(
    IntPtr Handle,
    out double returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbUnit_getOffsetSItoUnit(
    IntPtr Handle,
    out double returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbUnit_hasDbPhysicalDimension(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbUnits_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbUnits_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbVehicleInformation_getDbLogicalLinks(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbVehicleInformation_getDbPhysicalVehicleLinkOrInterfaces(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbVehicleInformation_getDbVehicleConnectors(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbVehicleInformations_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbVehicleInformations_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsError_getSeverity(
    IntPtr Handle,
    out MCDSeverity returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsError_getCode(
    IntPtr Handle,
    out MCDErrorCodes returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsError_getVendorCode(
    IntPtr Handle,
    out ushort returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsError_getVendorCodeDescription(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsError_getCodeDescription(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsError_getParent(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsException_getError(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsException_getSourceFile(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsException_getSourceLine(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLink_setEventListener(
    IntPtr Handle,
    IntPtr _pDtsEventListener,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLink_releaseEventListener(
    IntPtr Handle,
    uint _uEventLIstenerId);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLink_clearQueue(IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLink_close(IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLink_getDbObject(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLink_getIdentifiedVariantAccessKeys(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLink_getLockState(
    IntPtr Handle,
    out MCDLockState returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLink_getQueueErrorMode(
    IntPtr Handle,
    out MCDQueueErrorMode returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLink_getQueueFillingLevel(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLink_getAutoSyncWithInternalState(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLink_getQueueState(
    IntPtr Handle,
    out MCDActivityState returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLink_getSelectedVariantAccessKeys(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLink_getState(
    IntPtr Handle,
    out MCDLogicalLinkState returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLink_getType(
    IntPtr Handle,
    out MCDLogicalLinkType returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLink_hasDetectedVariant(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLink_lockLink(IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLink_open(IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLink_resume(IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLink_setProtocolParameters(
    IntPtr Handle,
    IntPtr _parameters);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLink_setQueueErrorMode(
    IntPtr Handle,
    MCDQueueErrorMode _mode);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLink_setAutoSyncWithInternalState(
    IntPtr Handle,
    bool _snyc);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLink_supportsTimeStamp(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLink_suspend(IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLink_unlockLink(IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLink_createDiagComPrimitiveByDbObject(
    IntPtr Handle,
    IntPtr _dbComPrimitive,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLink_createDiagComPrimitiveByName(
    IntPtr Handle,
    string _shortName,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLink_createDiagComPrimitiveBySemanticAttribute(
    IntPtr Handle,
    string _semanticAttribute,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLink_createDiagComPrimitiveByType(
    IntPtr Handle,
    MCDObjectType _diagComPrimitiveType,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLink_createDVServiceByRelationType(
    IntPtr Handle,
    string _relationType,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLink_disableReducedResults(IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLink_getDefinableDynIds(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLink_gotoOnline(IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLink_isUnsupportedComParametersAccepted(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLink_reset(IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLink_setUnitGroup(
    IntPtr Handle,
    string _shortName);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLink_unsupportedComParametersAccepted(
    IntPtr Handle,
    bool _accept);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLink_enableReducedResults(IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLink_removeDiagComPrimitive(
    IntPtr Handle,
    IntPtr _diagComPrimitive);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLink_gotoOffline(IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLink_getUnitGroup(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLink_createDynIdComPrimitiveByTypeAndDefinitionMode(
    IntPtr Handle,
    MCDObjectType _type,
    string _definitionMode,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLink_executeIoCtl(
    IntPtr Handle,
    uint _uIoCtlCommandId,
    ref ByteField_Struct _pInputData,
    out ByteField_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLink_getPhysicalInterfaceLink(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLink_getConfigurationRecords(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLink_getUniqueRuntimeID(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLink_execIOCtrl(
    IntPtr Handle,
    string _IOCtrlName,
    IntPtr _inputData,
    uint _inputDataItemType,
    uint _outputDataSize,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLink_getInterfaceResource(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLink_sendBreak(IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLink_getMatchedDbEcuVariantPattern(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLink_setChannelMonitoring(
    IntPtr Handle,
    bool _enable);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLink_getChannelMonitoring(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLink_getCreationAccessKey(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLink_getInternalState(
    IntPtr Handle,
    out MCDLogicalLinkState returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLink_gotoOnlineWithTimeout(
    IntPtr Handle,
    uint _timeout);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLink_openCached(IntPtr Handle, bool _useVariant);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLink_getQueueSize(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLink_setQueueSize(IntPtr Handle, uint _size);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLink_getOpenCounter(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLink_getOnlineCounter(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLink_getStartCommCounter(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLink_getLockedCounter(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsNamedCollection_getNames(
    IntPtr Handle,
    out StringArray_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsNamedObject_getDescription(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsNamedObject_getShortName(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsNamedObject_getLongName(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsNamedObject_getStringID(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsObject_getObjectType(
    IntPtr Handle,
    out MCDObjectType returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsObject_getObjectID(IntPtr Handle, out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsParameter_getDecimalPlaces(
    IntPtr Handle,
    out ushort returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsParameter_getValue(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsParameter_getUnit(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsParameter_getRadix(IntPtr Handle, out ushort returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsParameter_getValueRangeInfo(
    IntPtr Handle,
    out MCDRangeInfo returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsParameter_getType(
    IntPtr Handle,
    out MCDDataType returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsParameter_getDbObject(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsParameter_setValue(IntPtr Handle, IntPtr _pValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsParameter_getCodedType(
    IntPtr Handle,
    out MCDDataType returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsParameter_getCodedValue(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsParameter_setCodedValue(IntPtr Handle, IntPtr _pValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsParameter_getValueTextID(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsParameter_getLongNameID(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsParameter_getUnitTextID(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsParameter_getMCDParameterType(
    IntPtr Handle,
    out MCDParameterType returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsParameter_getDataType(
    IntPtr Handle,
    out MCDDataType returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsParameter_getPhysicalScaleConstraint(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsParameter_getInternalScaleConstraint(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsParameter_getBitPos(IntPtr Handle, out byte returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsParameter_getByteLength(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsParameter_getBytePos(IntPtr Handle, out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsParameter_createDtsValue(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsParameter_addDtsParameters(IntPtr Handle, uint _count);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsParameter_getDtsLengthKey(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsParameter_isDtsVariableLength(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsParameter_getDtsParameters(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsParameter_getSemantic(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsParameter_hasValue(IntPtr Handle, out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProject_selectDbVehicleInformationByName(
    IntPtr Handle,
    string _shortName,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProject_selectDbVehicleInformation(
    IntPtr Handle,
    IntPtr _vehicleInformation);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProject_getClampState(
    IntPtr Handle,
    string _clampName,
    out MCDClampState returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProject_deselectVehicleInformation(IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProject_getDbProject(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProject_getActiveDbVehicleInformation(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProject_createLogicalLink(
    IntPtr Handle,
    IntPtr _LogicalLink,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProject_createLogicalLinkByAccessKey(
    IntPtr Handle,
    string _AccessKeyString,
    string _PhysicalVehicleLinkString,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProject_createLogicalLinkByName(
    IntPtr Handle,
    string _LogicalLinkName,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProject_createLogicalLinkByVariant(
    IntPtr Handle,
    string _BaseVariantLogicalLinkName,
    string _VariantName,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProject_removeLogicalLink(
    IntPtr Handle,
    IntPtr _LogicalLink);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProject_getTraceFilterNames(
    IntPtr Handle,
    out StringArray_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProject_createDtsLogicalLinkMonitor(
    IntPtr Handle,
    string _PilShortName,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProject_listAddonFiles(
    IntPtr Handle,
    string _strDirectory,
    string _strBaseVariant,
    string _strVariant,
    string _strIdents,
    bool _bReload,
    out StringArray_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProject_unlinkDatabaseFiles(IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProject_replaceProjectFlashFile(
    IntPtr Handle,
    string _strOldFile,
    string _strNewFile);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProject_getCustomerVersion(
    IntPtr Handle,
    out int returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProject_createValue(
    IntPtr Handle,
    MCDDataType _dataType,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProject_createLogicalLinkByAccessKeyAndInterface(
    IntPtr Handle,
    string _accessKeyString,
    string _physicalVehicleLinkString,
    IntPtr _interface_,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProject_createLogicalLinkByAccessKeyAndInterfaceResource(
    IntPtr Handle,
    string _accessKeyString,
    string _physicalVehicleLinkString,
    IntPtr _interfaceResource,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProject_createLogicalLinkByInterface(
    IntPtr Handle,
    IntPtr _logicalLink,
    IntPtr _interface_,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProject_createLogicalLinkByInterfaceResource(
    IntPtr Handle,
    IntPtr _logicalLink,
    IntPtr _interfaceResource,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProject_createLogicalLinkByNameAndInterface(
    IntPtr Handle,
    string _logicalLinkName,
    IntPtr _interface_,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProject_createLogicalLinkByNameAndInterfaceResource(
    IntPtr Handle,
    string _logicalLinkName,
    IntPtr _interfaceResource,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProject_createLogicalLinkByVariantAndInterface(
    IntPtr Handle,
    string _shortNameDbLogicalLink,
    string _variantName,
    IntPtr _interface_,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProject_createLogicalLinkByVariantAndInterfaceResource(
    IntPtr Handle,
    string _shortNameDbLogicalLink,
    string _variantName,
    IntPtr _interfaceResource,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProject_createMonitoringLink(
    IntPtr Handle,
    IntPtr _ifResource,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProject_execIOCtrl(
    IntPtr Handle,
    string _IOCtrlName,
    IntPtr _inputData,
    uint _inputDataItemType,
    uint _outputDataSize,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProject_getIOControlNames(
    IntPtr Handle,
    out StringArray_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProject_removeMonitoringLink(
    IntPtr Handle,
    IntPtr _monLink);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProject_getGlobalProtocolParameterSets(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProject_unlinkDatabaseFile(
    IntPtr Handle,
    string _strFile);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProject_linkDatabaseFile(
    IntPtr Handle,
    string _strFile,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProject_getDatabaseFileList(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProject_createDtsMonitoringLink(
    IntPtr Handle,
    string _PilShortName,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProject_setActiveSimFile(IntPtr Handle, string _filePath);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProject_getActiveSimFile(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProject_getSimFiles(
    IntPtr Handle,
    out StringArray_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProject_createDoIPMonitorLink(
    IntPtr Handle,
    string _NetworkId,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProject_getCharacteristic(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProject_getAglFiles(
    IntPtr Handle,
    out StringArray_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProject_clearLinkCache(IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProject_getProjectUid(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsResponse_getState(
    IntPtr Handle,
    out MCDResponseState returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsResponse_hasError(IntPtr Handle, out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsResponse_getError(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsResponse_getAccessKeyOfLocation(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsResponse_getResponseMessage(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsResponse_getLocationAddress(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsResponse_getDbObject(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsResponse_getResponseParameters(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsResponse_setError(IntPtr Handle, IntPtr _pError);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsResponse_getResponseTime(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsResponse_getParent(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsResponse_getContainedResponseMessage(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsResponse_getEndTime(IntPtr Handle, out ulong returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsResponse_getStartTime(
    IntPtr Handle,
    out ulong returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsResponse_getCANIdentifier(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsResponse_enterPDU(IntPtr Handle, IntPtr _pdu);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsResponse_hasResponseMessage(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsResponseParameter_hasError(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsResponseParameter_getError(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsResponseParameter_getResponseParameters(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsResponseParameter_setError(IntPtr Handle, IntPtr _pError);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsResponseParameter_getParameters(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsResponseParameter_getParent(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsResponseParameter_getDbDTC(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsResponseParameter_isVariableLength(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsResponseParameter_getLengthKey(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsResponseParameters_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsResponseParameters_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsResponseParameters_addElement(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsResponseParameters_addElementWithContent(
    IntPtr Handle,
    IntPtr _pPattern,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsResponseParameters_addMuxBranch(
    IntPtr Handle,
    string _strBranch,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsResponseParameters_addMuxBranchByIndex(
    IntPtr Handle,
    byte _Index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsResponseParameters_addMuxBranchByIndexWithContent(
    IntPtr Handle,
    byte _Index,
    IntPtr _pContentList,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsResponseParameters_addMuxBranchWithContent(
    IntPtr Handle,
    string _strBranch,
    IntPtr _pContentList,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsResponseParameters_setParameterWithName(
    IntPtr Handle,
    string _name,
    IntPtr _value,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsResponseParameters_addMuxBranchByMuxValue(
    IntPtr Handle,
    IntPtr _muxValue,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsResponseParameters_addMuxBranchByMuxValueWithContent(
    IntPtr Handle,
    IntPtr _muxValue,
    IntPtr _pContentList,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsResponseParameters_getParent(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsResponseParameters_addEnvDataByDTC(
    IntPtr Handle,
    uint _dtc,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsResponses_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsResponses_add(
    IntPtr Handle,
    IntPtr _pDbLocation,
    bool _isPositive,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsResponses_removeByIndex(IntPtr Handle, uint _index);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsResponses_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsResponses_getParent(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsResult_getType(
    IntPtr Handle,
    out MCDResultType returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsResult_getRequestMessage(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsResult_hasError(IntPtr Handle, out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsResult_getError(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsResult_getRequestParameters(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsResult_getResponses(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsResult_getServiceShortName(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsResult_setError(IntPtr Handle, IntPtr _pError);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsResult_getResultState(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsResult_getRequestTime(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsResult_getRequestEndTime(
    IntPtr Handle,
    out ulong returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsResult_getCANIdentifier(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsResult_getServiceLongName(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsResult_hasRequestMessage(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsResults_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsResults_add(
    IntPtr Handle,
    IntPtr _pError,
    MCDResultType _type,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsResults_addWithContent(
    IntPtr Handle,
    IntPtr _pResult,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_setEventListener(
    IntPtr Handle,
    IntPtr _eventListener,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_releaseEventListener(
    IntPtr Handle,
    uint _dwEventListenerId);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_closeByteTrace(
    IntPtr Handle,
    string _PhysicalInterfaceLink);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_deselectProject(IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_disableApiTrace(IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_enableApiTrace(IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_getActiveProject(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_getASAMMCDVersion(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_getClassFactory(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_getCurrentTracePath(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_getDbProjectDescriptions(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_getInstallationPath(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_getLockState(
    IntPtr Handle,
    out MCDLockState returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_getMaxNoOfClients(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_getProjectPath(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_getServerType(
    IntPtr Handle,
    out MCDServerType returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_getState(
    IntPtr Handle,
    out MCDSystemState returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_getStringFromEnum(
    IntPtr Handle,
    uint _eConst,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_getStringFromErrorCode(
    IntPtr Handle,
    MCDErrorCodes _errorCode,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_getVersion(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_initialize(IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_isApiTraceEnabled(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_openByteTrace(
    IntPtr Handle,
    string _PhysicalInterfaceLink,
    string _FileName);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_prepareInterface(IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_selectProject(
    IntPtr Handle,
    IntPtr _project,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_selectProjectByName(
    IntPtr Handle,
    string _project,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_setApiTraceLevel(IntPtr Handle, uint _nLevel);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_startByteTrace(
    IntPtr Handle,
    string _PhysicalInterfaceLink);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_stopByteTrace(
    IntPtr Handle,
    string _PhysicalInterfaceLink);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_uninitialize(IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_unprepareInterface(IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_getTracePath(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_getDbProjectConfiguration(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_getDbPhysicalInterfaceLinks(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_createMonitorLinkByName(
    IntPtr Handle,
    string _PhysicalInterfaceLinkName,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_createMonitorLink(
    IntPtr Handle,
    IntPtr _PhysicalInterfaceLink,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_registerApp(
    IntPtr Handle,
    uint _appID,
    uint _reqItem,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_getRequiredItem(
    IntPtr Handle,
    uint _ulID,
    uint _reqItem,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_getClientNo(IntPtr Handle, out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_getInterfaceNumber(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_getSystemParameter(
    IntPtr Handle,
    string _value,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_isUnsupportedComParametersAccepted(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_lockServer(IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_unlockServer(IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_unsupportedComParametersAccepted(
    IntPtr Handle,
    bool _accept);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_enableInterface(
    IntPtr Handle,
    string _shortName,
    bool _bEnable);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_setFlashDataRoot(
    IntPtr Handle,
    string _pFlashDataRootPath);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_getProperty(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_setProperty(
    IntPtr Handle,
    string _name,
    IntPtr _value);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_setJavaLocation(IntPtr Handle, string _Location);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_getConnectedInterfaces(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_getCurrentInterfaces(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_getPropertyNames(
    IntPtr Handle,
    out StringArray_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_resetProperty(IntPtr Handle, string _name);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_getEnumValue(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_getSeed(
    IntPtr Handle,
    uint _procedureId,
    DtsAppID _appId,
    out ByteField_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_sendKey(IntPtr Handle, ref ByteField_Struct _key);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_detectInterfaces(
    IntPtr Handle,
    string _optionString);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_prepareVciAccessLayer(IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_unprepareVciAccessLayer(IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_getConfigurationPath(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_dumpRunningObjects(
    IntPtr Handle,
    string _outputFile,
    bool _singleObjects);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_getConfiguration(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_loadViewerProject(
    IntPtr Handle,
    ref StringArray_Struct _databaseFiles,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_unloadViewerProject(
    IntPtr Handle,
    IntPtr _project);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_setSessionProjectPath(IntPtr Handle, string _Path);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_reloadConfiguration(IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_dumpMemoryUsage(
    IntPtr Handle,
    string _outputFile,
    int _flags,
    bool _append);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_writeTraceEntry(IntPtr Handle, string _message);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_getDebugTracePath(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_startSnapshotModeTracing(
    IntPtr Handle,
    string _PhysicalInterfaceLink,
    uint _timeInterval);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_takeSnapshotByteTrace(
    IntPtr Handle,
    string _PhysicalInterfaceLink);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_generateSnapshotByteTrace(
    IntPtr Handle,
    string _PhysicalInterfaceLink,
    string _outputPath);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_stopSnapshotModeTracing(
    IntPtr Handle,
    string _PhysicalInterfaceLink);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_writeExternTraceEntry(
    IntPtr Handle,
    string _prefix,
    string _message);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_selectDynamicProject(
    IntPtr Handle,
    string _name,
    ref StringArray_Struct _files,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_getOptionalProperty(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_startInterfaceDetection(IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystem_createJVM(IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsValue_clear(IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsValue_getBitfield(
    IntPtr Handle,
    out BitField_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsValue_getBitfieldValue(
    IntPtr Handle,
    uint _index,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsValue_getBytefield(
    IntPtr Handle,
    out ByteField_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsValue_getBytefieldValue(
    IntPtr Handle,
    uint _Index,
    out byte returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsValue_getDataType(
    IntPtr Handle,
    out MCDDataType returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsValue_getFloat32(IntPtr Handle, out float returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsValue_getFloat64(IntPtr Handle, out double returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsValue_getInt16(IntPtr Handle, out short returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsValue_getInt32(IntPtr Handle, out int returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsValue_getInt64(IntPtr Handle, out long returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsValue_getInt8(IntPtr Handle, out char returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsValue_getLength(IntPtr Handle, out int returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsValue_getString(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsValue_getUint16(IntPtr Handle, out ushort returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsValue_getUint32(IntPtr Handle, out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsValue_getUint64(IntPtr Handle, out ulong returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsValue_getUint8(IntPtr Handle, out byte returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsValue_isEmpty(IntPtr Handle, out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsValue_setBitfield(
    IntPtr Handle,
    ref BitField_Struct _pBitField);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsValue_setBitfieldValue(
    IntPtr Handle,
    bool _value,
    uint _index);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsValue_setBytefield(
    IntPtr Handle,
    ref ByteField_Struct _pByteField);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsValue_setBytefieldValue(
    IntPtr Handle,
    byte _Value,
    uint _Index);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsValue_setDataType(IntPtr Handle, MCDDataType _NewType);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsValue_setFloat32(IntPtr Handle, float _Value);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsValue_setFloat64(IntPtr Handle, double _Value);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsValue_setInt16(IntPtr Handle, short _Value);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsValue_setInt32(IntPtr Handle, int _Value);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsValue_setInt64(IntPtr Handle, long _Value);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsValue_setInt8(IntPtr Handle, char _Value);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsValue_setUint16(IntPtr Handle, ushort _Value);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsValue_setUint32(IntPtr Handle, uint _Value);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsValue_setUint64(IntPtr Handle, ulong _Value);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsValue_setUint8(IntPtr Handle, byte _Value);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsValue_getValueAsString(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsValue_setValueAsString(IntPtr Handle, string _value);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsValue_isValid(IntPtr Handle, out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsValue_getAsciistring(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsValue_setAsciistring(IntPtr Handle, string _Value);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsValue_getUnicode2string(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsValue_setUnicode2string(IntPtr Handle, string _Value);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsValue_getBoolean(IntPtr Handle, out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsValue_setBoolean(IntPtr Handle, bool _Value);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsValue_copy(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsValues_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsVersion_getMajor(IntPtr Handle, out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsVersion_getMinor(IntPtr Handle, out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsVersion_getRevision(IntPtr Handle, out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsClassFactory_createValue(
    IntPtr Handle,
    MCDDataType _type,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsClassFactory_createAccessKey(
    IntPtr Handle,
    string _accessKey,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsClassFactory_createError(
    IntPtr Handle,
    MCDErrorCodes _code,
    string _text,
    ushort _vendorcode,
    string _information,
    MCDSeverity _severity,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsMonitorLink_open(IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsMonitorLink_close(IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsMonitorLink_openTraceFile(
    IntPtr Handle,
    string _TraceFileName,
    bool _bOverwriteIfFileExists);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsMonitorLink_startTraceFile(IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsMonitorLink_stopTraceFile(IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsMonitorLink_closeTraceFile(IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsMonitorLink_getTraceFileLimit(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsMonitorLink_setTraceFileLimit(
    IntPtr Handle,
    uint _uLimitInByte);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsMonitorLink_getLastItems(
    IntPtr Handle,
    uint _uNoOfItems,
    uint _uLastTotalNoOfItems,
    ref uint _puNoOfDeliveredItems,
    ref uint _puTotalNoOfItems,
    out ByteField_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsMonitorLink_getFilterForDisplayAndFileFlag(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsMonitorLink_setFilterForDisplayAndFileFlag(
    IntPtr Handle,
    bool _bUseFilter);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsMonitorLink_getFilterForDisplayFlag(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsMonitorLink_setFilterForDisplayFlag(
    IntPtr Handle,
    bool _bUseFilter);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsMonitorLink_getBusloadFlag(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsMonitorLink_setBusloadFlag(
    IntPtr Handle,
    bool _bBusLoadFlag);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsMonitorLink_getCurrentBusLoad(
    IntPtr Handle,
    out double returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsMonitorLink_setCanFilter(
    IntPtr Handle,
    string _FilterName);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDiagVariable_getValueType(
    IntPtr Handle,
    out MCDDiagVarType returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDiagVariable_isReadBeforeWrite(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDiagVariable_getDbRelationTypes(
    IntPtr Handle,
    out StringArray_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDiagVariables_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDiagVariables_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbODXFile_getFileName(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbODXFile_getFileVersion(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbODXFile_getODXVersion(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbODXFiles_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbProtocolParameters_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbProtocolParameters_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbVariantCodingDomain_getFragments(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbVariantCodingDomain_getDefaultStrings(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbVariantCodingDomain_getSegmentNumberOfDomainByIndex(
    IntPtr Handle,
    uint _index,
    out ushort returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbVariantCodingDomain_getSegmentNumberOfDomainByName(
    IntPtr Handle,
    string _name,
    out ushort returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbVariantCodingDomain_getExternalDefaultStrings(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbVariantCodingDomains_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbVariantCodingDomains_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbVariantCodingFragment_getParameterMetaInformation(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbVariantCodingFragment_hasBitPosition(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbVariantCodingFragment_getBitPosition(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbVariantCodingFragment_getBytePosition(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbVariantCodingFragment_getBitLength(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbVariantCodingFragment_getReadSecurityLevel(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbVariantCodingFragment_getWriteSecurityLevel(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbVariantCodingFragment_getTextTableSupplementKey(
    IntPtr Handle,
    uint _index,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbVariantCodingFragment_getFragmentMeanings(
    IntPtr Handle,
    bool _bIsInternal,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbVariantCodingFragments_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbVariantCodingFragments_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbVariantCodingString_getAccessLevel(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbVariantCodingString_getSupplementKey(
    IntPtr Handle,
    uint _index,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbVariantCodingString_getPartVersion(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbVariantCodingString_HasPartVersion(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbVariantCodingString_getPartNumber(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbVariantCodingString_getDefaultStringValue(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbVariantCodingStrings_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbVariantCodingStrings_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDelay_setTimeDelay(IntPtr Handle, uint _delayInMs);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDelay_getTimeDelay(IntPtr Handle, out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsEvent_getEventType(
    IntPtr Handle,
    out DtsEventType returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsEvent_getEventId(
    IntPtr Handle,
    out DtsEventId returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsEvent_getError(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsEvent_getDiagComPrimitive(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsEvent_getJobInfo(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsEvent_getProgress(IntPtr Handle, out byte returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsEvent_getResultState(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsEvent_getLockState(
    IntPtr Handle,
    out MCDLockState returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsEvent_getLogicalLink(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsEvent_getLogicalLinkState(
    IntPtr Handle,
    out MCDLogicalLinkState returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsEvent_getClamp(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsEvent_getClampState(
    IntPtr Handle,
    out MCDClampState returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsEvent_getMonitoringLink(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsEvent_getInterface(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsEvent_getInterfaceStatus(
    IntPtr Handle,
    out MCDInterfaceStatus returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsEvent_getConfigurationRecord(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsEvent_getDynIdList(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLinkMonitor_addAllLogicalLinkForMonitoring(
    IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLinkMonitor_removeAllLogicalLinkForMonitoring(
    IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLinkMonitor_addLogicalLinkForMonitoring(
    IntPtr Handle,
    string _NewLogicalLinkShortName);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLinkMonitor_removeLogicalLinkFromMonitoring(
    IntPtr Handle,
    string _RemoveLogicalLinkShortName);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLinkMonitor_setRingBufferSize(
    IntPtr Handle,
    uint _uSize);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLinkMonitor_start(IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLinkMonitor_stop(IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLinkMonitor_openFileTrace(
    IntPtr Handle,
    string _FileName,
    bool _bOverwrite);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLinkMonitor_startFileTrace(IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLinkMonitor_stopFileTrace(IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLinkMonitor_closeFileTrace(IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLinkMonitor_getLatestEvents(
    IntPtr Handle,
    uint _uMaxNoOfNewEvents,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLinkMonitor_setFilter(
    IntPtr Handle,
    IntPtr _filterConfig,
    bool _filterView,
    bool _filterTrace);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLinkMonitor_openFileTraceInFolder(
    IntPtr Handle,
    string _outputFolderPath,
    string _FileName);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLinkMonitor_startSnapshotModeTracing(
    IntPtr Handle,
    uint _timeInterval);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLinkMonitor_generateSnapshotTrace(
    IntPtr Handle,
    string _outputPath);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLinkMonitor_takeSnapshotTrace(IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLinkMonitor_stopSnapshotModeTracing(IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsOfflineVariantCoding_getCodingString(
    IntPtr Handle,
    string _domain,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsOfflineVariantCoding_setCodingString(
    IntPtr Handle,
    string _domain,
    IntPtr _codingString);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsOfflineVariantCoding_getInternalMeaning(
    IntPtr Handle,
    string _domain,
    string _fragment,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsOfflineVariantCoding_setInternalMeaning(
    IntPtr Handle,
    string _domain,
    string _fragment,
    IntPtr _meaning);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsOfflineVariantCoding_getExternalMeaning(
    IntPtr Handle,
    string _domain,
    string _fragment,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsOfflineVariantCoding_setExternalMeaning(
    IntPtr Handle,
    string _domain,
    string _fragment,
    IntPtr _meaning);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsParameterMetaInfo_getParameterType(
    IntPtr Handle,
    out MCDDataType returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsParameterMetaInfo_getDefaultValue(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsParameterMetaInfo_getMinValue(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsParameterMetaInfo_getMaxValue(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsParameterMetaInfo_getRadix(
    IntPtr Handle,
    out ushort returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsParameterMetaInfo_getDecimalPlaces(
    IntPtr Handle,
    out ushort returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsParameterMetaInfo_getUnit(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsParameterMetaInfo_getMinLength(
    IntPtr Handle,
    out ushort returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsParameterMetaInfo_getMaxLength(
    IntPtr Handle,
    out ushort returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsParameterMetaInfo_getTextTableElements(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsPhysicalInterfaceLink_getInterface(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsRawService_getRawData(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsRawService_enterRawData(IntPtr Handle, IntPtr _rawData);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSmartComPrimitive_getLogicalLinkLongName(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSmartComPrimitive_getPhysicalInterfaceName(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSmartComPrimitive_getDescription(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSmartComPrimitive_getComPrimitiveType(
    IntPtr Handle,
    out MCDObjectType returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSmartComPrimitive_getVariantLongName(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSmartComPrimitive_getAccessKey(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSmartComPrimitive_getLogicalLinkShortName(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsGlobalProtocolParameter_getActive(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsGlobalProtocolParameter_setActive(
    IntPtr Handle,
    bool _newValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsGlobalProtocolParameterSet_update(IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsGlobalProtocolParameterSet_getParameters(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsGlobalProtocolParameters_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsGlobalProtocolParameters_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsGlobalProtocolParameterSets_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsGlobalProtocolParameterSets_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDatatypeCollection_getCount(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDatatypeCollection_removeAll(IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDatatypeCollection_removeByIndex(
    IntPtr Handle,
    uint _index);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDataObjectProp_getCodedType(
    IntPtr Handle,
    out MCDDataType returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDataObjectProp_getCompuMethod(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDataObjectProp_getPhysicalType(
    IntPtr Handle,
    out MCDDataType returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDataObjectProp_isDiagCodedTypeValid(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDataObjectProp_isPhysicalTypeValid(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDataObjectProp_isCompuMethodValid(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDataObjectProp_getDiagCodedLengthType(
    IntPtr Handle,
    out DtsDiagCodedLengthType returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDataObjectProp_isDiagCodedLengthTypeValid(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbDataObjectProp_isHighlowByteOrder(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbComputation_isCompuDefaultValueValid(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbComputation_getCompuDefaultValue(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbComputation_getDbCompuScales(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbCompuMethod_getCategoryName(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbCompuMethod_isCompuInternalToPhysValid(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbCompuMethod_isCompuPhysToInternalValid(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbCompuMethod_getCompuInternalToPhys(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbCompuMethod_getCompuPhysToInternal(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbCompuScale_getShortLabel(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbCompuScale_isDescriptionValid(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbCompuScale_isShortLabelValid(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbCompuScale_getDescription(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbCompuScale_isCompuInverseValueValid(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbCompuScale_getCompuInverseValue(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbCompuScale_isCompuConstValueValid(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbCompuScale_getCompuConstValue(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbCompuScale_isLowerLimitValid(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbCompuScale_getLowerLimit(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbCompuScale_isUpperLimitValid(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbCompuScale_getUpperLimit(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbCompuScale_getCompuNumeratorCount(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbCompuScale_getCompuDenominatorCount(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbCompuScale_getCompuNumeratorAt(
    IntPtr Handle,
    uint _idx,
    out double returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbCompuScale_getCompuDenominatorAt(
    IntPtr Handle,
    uint _idx,
    out double returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbCompuScale_getCompuDenominatorsAsString(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbCompuScale_getCompuNumeratorsAsString(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbLimit_hasIntervalType(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbLimit_getValue(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbLimit_getType(
    IntPtr Handle,
    out MCDLimitType returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsDbCompuScales_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsIdentifierInfos_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsIdentifierInfos_loadDatabase(
    IntPtr Handle,
    string _fileName);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsIdentifierInfos_unloadAll(IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsIdentifierInfos_unloadDatabase(
    IntPtr Handle,
    string _fileName);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsIdentifierInfos_getItemByIdentifier(
    IntPtr Handle,
    uint _identifier,
    bool _extended,
    int _extendedAddress,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsIdentifierInfo_getEcuName(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsIdentifierInfo_getIdentifier(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsIdentifierInfo_getMessageName(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsIdentifierInfo_isExtendedIdentifier(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsIdentifierInfo_getIdentifierType(
    IntPtr Handle,
    out DtsIdentifierType returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsIdentifierInfo_getExtendedAddress(
    IntPtr Handle,
    out int returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsCanFilters_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsCanFilters_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsCanFilters_createItem(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsCanFilters_removeItem(IntPtr Handle, IntPtr _item);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsCanFilter_getFilterEntries(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsCanFilter_getType(
    IntPtr Handle,
    out DtsFilterType returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsCanFilter_setDescription(IntPtr Handle, string _name);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsCanFilter_setShortName(IntPtr Handle, string _name);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsCanFilter_getShortName(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsCanFilter_getDescription(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsCanFilter_save(IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsCanFilter_isChanged(IntPtr Handle, out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsCanFilter_reloadFromFile(IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsCanFilter_getFile(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsCanFilterEntry_getIdentifier(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsCanFilterEntry_getExtended(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsCanFilterEntry_setExtended(IntPtr Handle, bool _extended);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsCanFilterEntry_setIdentifier(
    IntPtr Handle,
    uint _identifier);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsCanFilterEntries_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsCanFilterEntries_createItem(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsCanFilterEntries_removeItem(IntPtr Handle, IntPtr _entry);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsWLanSignalData_getType(
    IntPtr Handle,
    out DtsWLanType returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsWLanSignalData_getChannel(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsWLanSignalData_getChannelFreq(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsWLanSignalData_getChannelWidth(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsWLanSignalData_getTxPower(
    IntPtr Handle,
    out float returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsWLanSignalData_getLinkSpeed(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsWLanSignalData_getRSSI(
    IntPtr Handle,
    out int returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsWLanSignalData_getSNR(IntPtr Handle, out int returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsWLanSignalData_getNoise(
    IntPtr Handle,
    out int returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsWLanSignalData_getSigQuality(
    IntPtr Handle,
    out int returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsWLanSignalData_getSSID(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsWLanSignalData_getValidityFlag(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystemConfig_getProjectPath(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystemConfig_setProjectPath(
    IntPtr Handle,
    string _projectPath);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystemConfig_getDatabaseCachesPath(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystemConfig_setTracePath(
    IntPtr Handle,
    string _tracePath);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystemConfig_updateLicenseInfo(IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystemConfig_getSystemProperties(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystemConfig_setFinasReportPath(
    IntPtr Handle,
    string _reportPath);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystemConfig_getFinasReportPath(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystemConfig_setRootDescriptionFile(
    IntPtr Handle,
    string _rootFilePath);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystemConfig_getOdx201EditorPath(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystemConfig_getLicensedProducts(
    IntPtr Handle,
    out StringArray_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystemConfig_enableWriteAccess(IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystemConfig_releaseWriteAccess(IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystemConfig_save(IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystemConfig_getProjectConfigs(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystemConfig_getTraceConfig(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystemConfig_getJavaConfig(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystemConfig_getSupportedInterfaces(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystemConfig_getInterfaceConfigs(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystemConfig_getUserInterfaceLanguage(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystemConfig_setUserInterfaceLanguage(
    IntPtr Handle,
    uint _language);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystemConfig_setLicenseFile(
    IntPtr Handle,
    string _licenseFile);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystemConfig_getLicenseInfos(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystemConfig_hasChanges(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystemConfig_getRootDescriptionFile(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystemConfig_setOdx201EditorPath(
    IntPtr Handle,
    string _dts7VenicePath);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystemConfig_getTracePath(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystemConfig_setDatabaseCachesPath(
    IntPtr Handle,
    string _databaseCachesPath);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystemConfig_getDefaultConfigPath(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystemConfig_getSupportedPduApis(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTraceConfig_getClientCallTrace(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTraceConfig_setClientCallTrace(
    IntPtr Handle,
    bool _value);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTraceConfig_getClientCallTraceThreadContext(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTraceConfig_setClientCallTraceFunctionCalls(
    IntPtr Handle,
    bool _enabled);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTraceConfig_getClientCallTraceFunctionCalls(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTraceConfig_setClientCallTraceFunctionReturns(
    IntPtr Handle,
    bool _enabled);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTraceConfig_getClientCallTraceFunctionParameters(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTraceConfig_setClientCallTraceFunctionParameters(
    IntPtr Handle,
    bool _enabled);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTraceConfig_getClientCallTraceExceptions(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTraceConfig_setClientCallTraceExceptions(
    IntPtr Handle,
    bool _enabled);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTraceConfig_getClientCallTraceExtendedObjectInfo(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTraceConfig_setClientCallTraceExtendedObjectInfo(
    IntPtr Handle,
    bool _enabled);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTraceConfig_getClientCallTraceObjectLifetime(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTraceConfig_setClientCallTraceObjectLifetime(
    IntPtr Handle,
    bool _enabled);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTraceConfig_getClientCallTraceFunctionTimings(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTraceConfig_setClientCallTraceSuppressTimestamps(
    IntPtr Handle,
    bool _enabled);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTraceConfig_getClientCallTraceSuppressPointerAddress(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTraceConfig_setClientCallTraceSuppressPointerAddress(
    IntPtr Handle,
    bool _enabled);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTraceConfig_getTraceMaxFileSize(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTraceConfig_setTraceMaxFileSize(
    IntPtr Handle,
    uint _maxFileSize);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTraceConfig_setUseSubDirectory(
    IntPtr Handle,
    bool _enabled);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTraceConfig_getUseSubDirectory(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTraceConfig_getPduApiCallTrace(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTraceConfig_setPduApiCallTrace(
    IntPtr Handle,
    bool _enabled);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTraceConfig_getPduApiCallTraceLogFunctionParameters(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTraceConfig_setPduApiCallTraceLogFunctionParameters(
    IntPtr Handle,
    bool _enabled);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTraceConfig_getPduApiCallTraceLogVersionInfo(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTraceConfig_setPduApiCallTraceLogVersionInfo(
    IntPtr Handle,
    bool _enabled);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTraceConfig_getPduApiCallTraceLogComParameterCalls(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTraceConfig_setPduApiCallTraceLogComParameterCalls(
    IntPtr Handle,
    bool _enabled);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTraceConfig_getPduApiCallTraceLogDetails(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTraceConfig_setPduApiCallTraceLogEvents(
    IntPtr Handle,
    bool _enabled);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTraceConfig_setDebugTrace(IntPtr Handle, bool _enable);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTraceConfig_getDebugTrace(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTraceConfig_setLimitNumberOfTraceFiles(
    IntPtr Handle,
    bool _enable);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTraceConfig_getLimitNumberOfTraceFiles(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTraceConfig_getLimitNumberOfTraceSessions(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTraceConfig_setLimitNumberOfTraceSessions(
    IntPtr Handle,
    bool _enable);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTraceConfig_getMaxNumberOfTraceFiles(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTraceConfig_setMaxNumberOfTraceFiles(
    IntPtr Handle,
    uint _maxFileNumber);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTraceConfig_getMaxNumberOfTraceSessions(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTraceConfig_setMaxNumberOfTraceSessions(
    IntPtr Handle,
    uint _maxTraceSessions);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTraceConfig_getUseTracePathForWritingSimFile(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTraceConfig_setUseTracePathForWritingSimFile(
    IntPtr Handle,
    bool _enable);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTraceConfig_getAppendSimFileTrace(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTraceConfig_setAppendSimFileTrace(
    IntPtr Handle,
    bool _enable);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTraceConfig_setClientCallTraceFunctionTimings(
    IntPtr Handle,
    bool _enabled);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTraceConfig_getClientCallTraceSuppressTimestamps(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTraceConfig_getWriteMicroseconds(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTraceConfig_setWriteMicroseconds(
    IntPtr Handle,
    bool _enabled);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTraceConfig_getUseSystemTracePath(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTraceConfig_setUseSystemTracePath(
    IntPtr Handle,
    bool _enable);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTraceConfig_getPduApiCallTraceMaxPduSize(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTraceConfig_setPduApiCallTraceMaxPduSize(
    IntPtr Handle,
    uint _MaxPduSize);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTraceConfig_setClientCallTraceThreadContext(
    IntPtr Handle,
    bool _enabled);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTraceConfig_getClientCallTraceFunctionReturns(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTraceConfig_setClientCallTraceSuppressThreadChanges(
    IntPtr Handle,
    bool _enabled);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTraceConfig_getClientCallTraceSuppressThreadChanges(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTraceConfig_getPduApiCallTraceMergeIntoApiTrace(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTraceConfig_setPduApiCallTraceMergeIntoApiTrace(
    IntPtr Handle,
    bool _value);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTraceConfig_setDebugTraceToApiTrace(
    IntPtr Handle,
    bool _enable);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTraceConfig_getDebugTraceToDebugOut(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTraceConfig_getDebugTraceToApiTrace(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTraceConfig_setDebugTraceToDebugOut(
    IntPtr Handle,
    bool _enable);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTraceConfig_getDebugTraceLevel(
    IntPtr Handle,
    out DtsDebugTraceLevel returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTraceConfig_setDebugTraceLevel(
    IntPtr Handle,
    DtsDebugTraceLevel _level);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTraceConfig_getJobLogging(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTraceConfig_setJobLogging(IntPtr Handle, bool _enable);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTraceConfig_getRemoteLogging(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTraceConfig_setRemoteLogging(IntPtr Handle, bool _value);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTraceConfig_setRemoteLoggingAddress(
    IntPtr Handle,
    string _address);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTraceConfig_getRemoteLoggingAddress(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTraceConfig_getPduApiCallTraceLogEvents(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsTraceConfig_setPduApiCallTraceLogDetails(
    IntPtr Handle,
    bool _enabled);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProjectConfig_getVehicleInformations(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProjectConfig_getModularDatabaseFiles(
    IntPtr Handle,
    bool _bForceReload,
    out StringArray_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProjectConfig_addModularDatabaseFile(
    IntPtr Handle,
    string _file,
    bool _allowMove);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProjectConfig_getDatabase(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProjectConfig_setProjectType(
    IntPtr Handle,
    DtsProjectType _type);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProjectConfig_getCxfDatabaseFiles(
    IntPtr Handle,
    bool _bForceReload,
    out StringArray_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProjectConfig_addCxfDatabaseFile(
    IntPtr Handle,
    string _file,
    bool _allowMove);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProjectConfig_getVehicleModelRange(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProjectConfig_setVehicleModelRange(
    IntPtr Handle,
    string _range);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProjectConfig_setDatabaseFile(
    IntPtr Handle,
    string _databaseFile,
    bool _allowMove,
    bool _overwriteExisting);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProjectConfig_removeCxfDatabaseFiles(
    IntPtr Handle,
    ref StringArray_Struct _files);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProjectConfig_removeVariantCodingConfigFiles(
    IntPtr Handle,
    ref StringArray_Struct _files);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProjectConfig_addCANFilterFile(
    IntPtr Handle,
    string _file,
    bool _allowMove);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProjectConfig_getExternalFlashFiles(
    IntPtr Handle,
    out StringArray_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProjectConfig_removeCANFilterFiles(
    IntPtr Handle,
    ref StringArray_Struct _files);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProjectConfig_getUseOptimizedDatabase(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProjectConfig_setUseOptimizedDatabase(
    IntPtr Handle,
    bool _useCompressed);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProjectConfig_setOptimizedDatabaseFile(
    IntPtr Handle,
    string _databaseFile,
    bool _allowMove,
    bool _overwriteExisting);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProjectConfig_getOptimizedDatabase(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProjectConfig_getProjectType(
    IntPtr Handle,
    out DtsProjectType returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProjectConfig_isDatabaseODX201Legacy(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProjectConfig_getSynchronizeVitWithDatabase(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProjectConfig_setSynchronizeVitWithDatabase(
    IntPtr Handle,
    bool _value);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProjectConfig_getCreateDefaultVit(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProjectConfig_setCreateDefaultVit(
    IntPtr Handle,
    bool _value);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProjectConfig_addExternalFlashFile(
    IntPtr Handle,
    string _file,
    bool _allowMove);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProjectConfig_getCANFilterFiles(
    IntPtr Handle,
    bool _bForceReload,
    out StringArray_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProjectConfig_removeExternalFlashFiles(
    IntPtr Handle,
    ref StringArray_Struct _files);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProjectConfig_updateVehicleInformationTables(
    IntPtr Handle);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProjectConfig_addOTXProject(
    IntPtr Handle,
    string _otxProject,
    bool _allowMove);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProjectConfig_getOTXProjects(
    IntPtr Handle,
    bool _bForceReload,
    out StringArray_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProjectConfig_removeOTXProjects(
    IntPtr Handle,
    ref StringArray_Struct _otxProjects);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProjectConfig_setDCDIFirmware(
    IntPtr Handle,
    string _firmware);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProjectConfig_getDCDIFirmware(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProjectConfig_generateOptimizedDatabase(
    IntPtr Handle,
    uint _encryption);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProjectConfig_getPreferredDbEncryption(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProjectConfig_setPreferredDbEncryption(
    IntPtr Handle,
    uint _encryptionMethod);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProjectConfig_checkDatabaseConsistency(
    IntPtr Handle,
    bool _bRunCheck,
    out int returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProjectConfig_getVariantCodingConfigFiles(
    IntPtr Handle,
    bool _bForceReload,
    out StringArray_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProjectConfig_addVariantCodingConfigFile(
    IntPtr Handle,
    string _file,
    bool _allowMove);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProjectConfig_removeModularDatabaseFiles(
    IntPtr Handle,
    ref StringArray_Struct _files);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProjectConfig_isOptimizedDatabaseODX201Legacy(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProjectConfig_checkOptimizedDatabaseConsistency(
    IntPtr Handle,
    out int returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProjectConfig_getJavaJob201Legacy(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProjectConfig_setJavaJob201Legacy(
    IntPtr Handle,
    bool _useJavaJob201Legacy);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProjectConfig_addAdditionalDatabaseFiles(
    IntPtr Handle,
    string _file,
    bool _allowMove);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProjectConfig_getAdditionalDatabaseFiles(
    IntPtr Handle,
    bool _bForceReload,
    out StringArray_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProjectConfig_removeAdditionalDatabaseFiles(
    IntPtr Handle,
    ref StringArray_Struct _files);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProjectConfig_addLogicalLinkFilterFile(
    IntPtr Handle,
    string _file,
    bool _allowMove);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProjectConfig_getLogicalLinkFilterFiles(
    IntPtr Handle,
    bool _bForceReload,
    out StringArray_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProjectConfig_removeLogicalLinkFilterFiles(
    IntPtr Handle,
    ref StringArray_Struct _files);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProjectConfig_getLogicalLinkFilters(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProjectConfig_getPreferredOptionSet(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProjectConfig_setPreferredOptionSet(
    IntPtr Handle,
    string _optionSet);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProjectConfig_isReferencing(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProjectConfig_getWriteSimFile(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProjectConfig_setWriteSimFile(
    IntPtr Handle,
    bool _useSimFileTrace);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProjectConfig_getSimFileAppend(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProjectConfig_setSimFileAppend(
    IntPtr Handle,
    bool _useSimFileTraceAppend);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProjectConfig_addSimFile(
    IntPtr Handle,
    string _file,
    bool _allowMove);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProjectConfig_getSimFiles(
    IntPtr Handle,
    bool _bForceReload,
    out StringArray_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProjectConfig_removeSimFiles(
    IntPtr Handle,
    ref StringArray_Struct _files);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProjectConfig_getActiveSimFile(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProjectConfig_setActiveSimFile(
    IntPtr Handle,
    string _simFileName);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProjectConfig_getCharacteristic(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProjectConfig_setCharacteristic(
    IntPtr Handle,
    uint _value);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProjectConfigs_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProjectConfigs_createItem(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProjectConfigs_removeItem(
    IntPtr Handle,
    IntPtr _projectConfig);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProjectConfigs_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsProjectConfigs_containsValidProject(
    IntPtr Handle,
    string _dirPath,
    string _projectPath,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLinkConfig_getAccessKey(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLinkConfig_getPhysicalVehicleLink(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLinkConfig_getManual(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLinkConfig_setManual(IntPtr Handle, bool _manual);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLinkConfig_getIsGateway(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLinkConfig_getGateway(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLinkConfig_setIsGateway(
    IntPtr Handle,
    bool _isGateway);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLinkConfig_setGateway(
    IntPtr Handle,
    string _gateway);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLinkConfig_setPhysicalVehicleLink(
    IntPtr Handle,
    string _physicalVehicleLink);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLinkConfigs_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsJavaConfig_getJvmLocations(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsJavaConfig_getCompilerLocations(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsJavaConfig_setCurrentJvmLocation(
    IntPtr Handle,
    IntPtr _jvmLocation);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsJavaConfig_setCurrentCompilerLocation(
    IntPtr Handle,
    IntPtr _compilerLocation);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsJavaConfig_getCurrentJvmLocation(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsJavaConfig_getCurrentCompilerLocation(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsJavaConfig_getJobDebugging(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsJavaConfig_setJobDebugging(IntPtr Handle, bool _enabled);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsJavaConfig_getCompilerOptions(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsJavaConfig_setCompilerOptions(
    IntPtr Handle,
    string _compilerOptions);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsVehicleInformationConfigs_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsVehicleInformationConfigs_removeItem(
    IntPtr Handle,
    IntPtr _vehicleInfoConfig);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsVehicleInformationConfig_getLinkMappings(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsVehicleInformationConfig_getLogicalLinkConfigs(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLinkMapping_getPhysicalVehicleLink(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLinkMapping_setPhysicalInterfaceLink(
    IntPtr Handle,
    string _interfaceLink);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLinkMapping_getPhysicalInterfaceLink(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLinkMapping_setPhysicalVehicleLink(
    IntPtr Handle,
    string _vehicleLink);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLinkMappings_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLinkMappings_removeItem(IntPtr Handle, IntPtr _item);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLinkMappings_createItem(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsNamedObjectConfig_setShortName(
    IntPtr Handle,
    string _name);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsNamedObjectConfig_getShortName(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsNamedObjectConfig_getLongName(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsNamedObjectConfig_setLongName(
    IntPtr Handle,
    string _name);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsNamedObjectConfig_setDescription(
    IntPtr Handle,
    string _name);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsNamedObjectConfig_getDescription(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLicenseInfos_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLicenseInfo_getHardwareName(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLicenseInfo_getLicenses(
    IntPtr Handle,
    out StringArray_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLicenseInfo_getLendingTime(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLicenseInfo_getHardwareType(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLicenseInfo_getHardwareSerial(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLicenseInfo_getProducts(
    IntPtr Handle,
    out StringArray_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLicenseInfo_getDatabaseEncryptionCount(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLicenseInfo_getDatabaseEncryption(
    IntPtr Handle,
    uint _index,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLicenseInfo_getDatabaseEncryptionName(
    IntPtr Handle,
    uint _index,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystemProperty_setValue(IntPtr Handle, IntPtr _value);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystemProperty_getValue(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystemProperty_getName(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystemProperty_setName(IntPtr Handle, string _name);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystemProperty_setStringValue(
    IntPtr Handle,
    string _stringValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystemProperties_createItem(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsSystemProperties_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsFileLocation_getFilePath(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsFileLocation_getShortName(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsFileLocation_getVersion(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsFileLocation_setFilePath(IntPtr Handle, string _filePath);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsFileLocation_setShortName(
    IntPtr Handle,
    string _shortName);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsFileLocation_setVersion(IntPtr Handle, string _version);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsFileLocations_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterfaceInformation_getBusSystemInterfaceType(
    IntPtr Handle,
    out DtsBusSystemInterfaceType returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterfaceInformation_getSupportsIpAddress(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterfaceInformation_getDbInterfaceCables(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterfaceInformation_getPDUAPIVersion(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterfaceInformation_getInterfaceLinks(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterfaceInformation_getVendorModuleNames(
    IntPtr Handle,
    out StringArray_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterfaceInformation_getDefaultCable(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterfaceInformations_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterfaceConfig_getModuleType(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterfaceConfig_getPDUAPIVersion(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterfaceConfig_setModuleType(
    IntPtr Handle,
    string _moduleType);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterfaceConfig_setPDUAPIVersion(
    IntPtr Handle,
    string _pduApiVersion);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterfaceConfig_getEnabled(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterfaceConfig_setEnabled(IntPtr Handle, bool _enabled);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterfaceConfig_getBusSystemInterfaceType(
    IntPtr Handle,
    out DtsBusSystemInterfaceType returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterfaceConfig_getInterfaceInformation(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterfaceConfig_setBusSystemInterfaceType(
    IntPtr Handle,
    DtsBusSystemInterfaceType _busSystemInterfaceType);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterfaceConfig_setIpAddress(
    IntPtr Handle,
    string _ipAddress);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterfaceConfig_getSerialNumber(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterfaceConfig_getIpAddress(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterfaceConfig_setSerialNumber(
    IntPtr Handle,
    string _serial);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterfaceConfig_setCable(
    IntPtr Handle,
    string _cableName);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterfaceConfig_getCable(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterfaceConfig_getInterfaceLinkInformations(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterfaceConfig_getInterfaceLinkConfigs(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterfaceConfig_setVendorModuleName(
    IntPtr Handle,
    string _vendorModuleName);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterfaceConfig_getVendorModuleName(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterfaceConfig_getUseForLicensing(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterfaceConfig_setUseForLicensing(
    IntPtr Handle,
    bool _use);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterfaceConfig_getConnectedStatus(
    IntPtr Handle,
    bool _doDetection,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterfaceConfig_getDetectedSerialNumber(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterfaceConfigs_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterfaceConfigs_createInterface(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterfaceConfigs_remove(
    IntPtr Handle,
    IntPtr _interfaceConfig);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterfaceLinkConfig_getLinkType(
    IntPtr Handle,
    out DtsPhysicalLinkOrInterfaceType returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterfaceLinkConfig_setLinkType(
    IntPtr Handle,
    DtsPhysicalLinkOrInterfaceType _type);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterfaceLinkConfig_setPduApiLinkType(
    IntPtr Handle,
    DtsPduApiLinkType _type);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterfaceLinkConfig_getPduApiLinkType(
    IntPtr Handle,
    out DtsPduApiLinkType returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterfaceLinkConfig_getGlobalIndex(
    IntPtr Handle,
    out int returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterfaceLinkConfig_setGlobalIndex(
    IntPtr Handle,
    int _index);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterfaceLinkConfig_getLocalIndex(
    IntPtr Handle,
    out int returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterfaceLinkConfig_setLocalIndex(
    IntPtr Handle,
    int _index);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterfaceLinkConfig_assign(
    IntPtr Handle,
    IntPtr _linkInformation);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterfaceLinkConfig_getPinType(
    IntPtr Handle,
    uint _index,
    out MCDConnectorPinType returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterfaceLinkConfig_getVehiclePin(
    IntPtr Handle,
    uint _index,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterfaceLinkConfig_getPinCount(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterfaceLinkConfig_getString(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterfaceLinkConfigs_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterfaceLinkConfigs_createInterfaceLink(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterfaceLinkConfigs_remove(IntPtr Handle, IntPtr _link);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterfaceLinkInformations_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterfaceLinkInformation_getConnectorPins(
    IntPtr Handle,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterfaceLinkInformation_getLinkType(
    IntPtr Handle,
    out DtsPhysicalLinkOrInterfaceType returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterfaceLinkInformation_getPduApiLinkType(
    IntPtr Handle,
    out DtsPduApiLinkType returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterfaceLinkInformation_getLocalIndex(
    IntPtr Handle,
    out uint returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsInterfaceLinkInformation_getString(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLinkFilterConfigs_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLinkFilterConfigs_createItem(
    IntPtr Handle,
    string _filterName,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLinkFilterConfigs_removeItem(
    IntPtr Handle,
    IntPtr _filterConfig);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLinkFilterConfigs_getItemByName(
    IntPtr Handle,
    string _name,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLinkFilterConfig_addAccessKey(
    IntPtr Handle,
    string _accessKey);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLinkFilterConfig_removeAccessKey(
    IntPtr Handle,
    string _accessKey);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLinkFilterConfig_getFilteringFlag(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLinkFilterConfig_setFilteringFlag(
    IntPtr Handle,
    bool _stop);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsLogicalLinkFilterConfig_getAccessKeys(
    IntPtr Handle,
    out StringArray_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsPduApiInformations_getItemByIndex(
    IntPtr Handle,
    uint _index,
    out ObjectInfo_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsPduApiInformation_getPDUAPIVersion(
    IntPtr Handle,
    out String_Struct returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsPduApiInformation_getEnabled(
    IntPtr Handle,
    out bool returnValue);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsPduApiInformation_setEnabled(
    IntPtr Handle,
    bool _enabled);

  [DllImport("CSWrap.dll", CharSet = CharSet.Unicode)]
  internal static extern IntPtr CSNIDTS_DtsPduApiInformation_getLibraryFile(
    IntPtr Handle,
    out String_Struct returnValue);
}
