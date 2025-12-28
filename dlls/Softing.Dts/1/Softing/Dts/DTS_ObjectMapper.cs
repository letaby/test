// Decompiled with JetBrains decompiler
// Type: Softing.Dts.DTS_ObjectMapper
// Assembly: Softing.Dts, Version=9.5.57994.11, Culture=neutral, PublicKeyToken=27906fee02086bf7
// MVID: EEF17F7E-0985-487A-B8F1-041534183DBD
// Assembly location: C:\Users\petra\Downloads\Архив (2)\Softing.Dts.dll

using System;
using System.Collections;

#nullable disable
namespace Softing.Dts;

internal class DTS_ObjectMapper
{
  private static onEvent m_eventCallback = (onEvent) null;
  private static bool m_initDone = false;
  private static Hashtable m_objectTable = new Hashtable();
  private static DTS_ObjectMapper m_instance = new DTS_ObjectMapper();

  internal DTS_ObjectMapper Instance => DTS_ObjectMapper.m_instance;

  ~DTS_ObjectMapper() => CSWrap.CSNIDTS_goodbyDts();

  internal static MCDException createException(IntPtr objectHandle)
  {
    switch (CSWrap.CSNIDTS_getObjectType(objectHandle))
    {
      case MCDObjectType.eMCDCOMMUNICATIONEXCEPTION:
        return (MCDException) new DtsCommunicationExceptionImpl(objectHandle);
      case MCDObjectType.eMCDDATABASEEXCEPTION:
        return (MCDException) new DtsDatabaseExceptionImpl(objectHandle);
      case MCDObjectType.eMCDEXCEPTION:
        return (MCDException) new DtsExceptionImpl(objectHandle);
      case MCDObjectType.eMCDPARAMETERIZATIONEXCEPTION:
        return (MCDException) new DtsParameterizationExceptionImpl(objectHandle);
      case MCDObjectType.eMCDPROGRAMVIOLATIONEXCEPTION:
        return (MCDException) new DtsProgramViolationExceptionImpl(objectHandle);
      case MCDObjectType.eMCDSHAREEXCEPTION:
        return (MCDException) new DtsShareExceptionImpl(objectHandle);
      case MCDObjectType.eMCDSYSTEMEXCEPTION:
        return (MCDException) new DtsSystemExceptionImpl(objectHandle);
      default:
        return (MCDException) null;
    }
  }

  internal static IntPtr getHandle(MappedObject objectInstance) => objectInstance.Handle;

  internal static MCDSystem getSystem()
  {
    DTS_ObjectMapper.registerCallbacks();
    return DTS_ObjectMapper.createObject(CSWrap.CSNIDTS_getSystem(), MCDObjectType.eMCDSYSTEM) as MCDSystem;
  }

  internal static MCDObject createObject(IntPtr objectHandle, MCDObjectType objectType)
  {
    lock (DTS_ObjectMapper.m_objectTable)
    {
      if (DTS_ObjectMapper.m_objectTable.ContainsKey((object) objectHandle))
      {
        object obj = DTS_ObjectMapper.m_objectTable[(object) objectHandle];
        if (obj != null)
        {
          object target = ((WeakReference) obj).Target;
          if (target != null)
          {
            CSWrap.CSNIDTS_releaseObject(objectHandle);
            return target as MCDObject;
          }
        }
      }
      switch (objectType)
      {
        case MCDObjectType.eMCDCOLLECTION:
          return (MCDObject) new DtsCollectionImpl(objectHandle);
        case MCDObjectType.eMCDCOMMUNICATIONEXCEPTION:
          return (MCDObject) new DtsCommunicationExceptionImpl(objectHandle);
        case MCDObjectType.eMCDDATABASEEXCEPTION:
          return (MCDObject) new DtsDatabaseExceptionImpl(objectHandle);
        case MCDObjectType.eMCDDBLOCATION:
          return (MCDObject) new DtsDbLocationImpl(objectHandle);
        case MCDObjectType.eMCDDBLOCATIONS:
          return (MCDObject) new DtsDbLocationsImpl(objectHandle);
        case MCDObjectType.eMCDDBLOGICALLINK:
          return (MCDObject) new DtsDbLogicalLinkImpl(objectHandle);
        case MCDObjectType.eMCDDBLOGICALLINKS:
          return (MCDObject) new DtsDbLogicalLinksImpl(objectHandle);
        case MCDObjectType.eMCDDBPHYSICALVEHICLELINKORINTERFACE:
          return (MCDObject) new DtsDbPhysicalVehicleLinkOrInterfaceImpl(objectHandle);
        case MCDObjectType.eMCDDBPHYSICALVEHICLELINKORINTERFACES:
          return (MCDObject) new DtsDbPhysicalVehicleLinkOrInterfacesImpl(objectHandle);
        case MCDObjectType.eMCDDBPROJECT:
          return (MCDObject) new DtsDbProjectImpl(objectHandle);
        case MCDObjectType.eMCDDBPROJECTDESCRIPTION:
          return (MCDObject) new DtsDbProjectDescriptionImpl(objectHandle);
        case MCDObjectType.eMCDDBPROJECTDESCRIPTIONS:
          return (MCDObject) new DtsDbProjectDescriptionsImpl(objectHandle);
        case MCDObjectType.eMCDDBVEHICLEINFORMATION:
          return (MCDObject) new DtsDbVehicleInformationImpl(objectHandle);
        case MCDObjectType.eMCDDBVEHICLEINFORMATIONS:
          return (MCDObject) new DtsDbVehicleInformationsImpl(objectHandle);
        case MCDObjectType.eMCDERROR:
          return (MCDObject) new DtsErrorImpl(objectHandle);
        case MCDObjectType.eMCDEXCEPTION:
          return (MCDObject) new DtsExceptionImpl(objectHandle);
        case MCDObjectType.eMCDLOGICALLINK:
          return (MCDObject) new DtsLogicalLinkImpl(objectHandle);
        case MCDObjectType.eMCDPARAMETERIZATIONEXCEPTION:
          return (MCDObject) new DtsParameterizationExceptionImpl(objectHandle);
        case MCDObjectType.eMCDPROGRAMVIOLATIONEXCEPTION:
          return (MCDObject) new DtsProgramViolationExceptionImpl(objectHandle);
        case MCDObjectType.eMCDPROJECT:
          return (MCDObject) new DtsProjectImpl(objectHandle);
        case MCDObjectType.eMCDRESPONSE:
          return (MCDObject) new DtsResponseImpl(objectHandle);
        case MCDObjectType.eMCDRESPONSEPARAMETER:
          return (MCDObject) new DtsResponseParameterImpl(objectHandle);
        case MCDObjectType.eMCDRESPONSEPARAMETERS:
          return (MCDObject) new DtsResponseParametersImpl(objectHandle);
        case MCDObjectType.eMCDRESPONSES:
          return (MCDObject) new DtsResponsesImpl(objectHandle);
        case MCDObjectType.eMCDRESULT:
          return (MCDObject) new DtsResultImpl(objectHandle);
        case MCDObjectType.eMCDRESULTS:
          return (MCDObject) new DtsResultsImpl(objectHandle);
        case MCDObjectType.eMCDSHAREEXCEPTION:
          return (MCDObject) new DtsShareExceptionImpl(objectHandle);
        case MCDObjectType.eMCDSYSTEM:
          return (MCDObject) new DtsSystemImpl(objectHandle);
        case MCDObjectType.eMCDSYSTEMEXCEPTION:
          return (MCDObject) new DtsSystemExceptionImpl(objectHandle);
        case MCDObjectType.eMCDVALUE:
          return (MCDObject) new DtsValueImpl(objectHandle);
        case MCDObjectType.eMCDVALUES:
          return (MCDObject) new DtsValuesImpl(objectHandle);
        case MCDObjectType.eMCDVERSION:
          return (MCDObject) new DtsVersionImpl(objectHandle);
        case MCDObjectType.eMCDDBPROJECTCONFIGURATION:
          return (MCDObject) new DtsDbProjectConfigurationImpl(objectHandle);
        case MCDObjectType.eMCDDBUNITS:
          return (MCDObject) new DtsDbUnitsImpl(objectHandle);
        case MCDObjectType.eMCDDBPARAMETERS:
          return (MCDObject) new DtsDbParametersImpl(objectHandle);
        case MCDObjectType.eMCDDBODXFILE:
          return (MCDObject) new DtsDbODXFileImpl(objectHandle);
        case MCDObjectType.eMCDDBDIAGCOMPRIMITIVES:
          return (MCDObject) new DtsDbDiagComPrimitivesImpl(objectHandle);
        case MCDObjectType.eMCDDBODXFILES:
          return (MCDObject) new DtsDbODXFilesImpl(objectHandle);
        case MCDObjectType.eMCDDBECUBASEVARIANT:
          return (MCDObject) new DtsDbEcuBaseVariantImpl(objectHandle);
        case MCDObjectType.eMCDDBECUBASEVARIANTS:
          return (MCDObject) new DtsDbEcuBaseVariantsImpl(objectHandle);
        case MCDObjectType.eMCDDBECUVARIANT:
          return (MCDObject) new DtsDbEcuVariantImpl(objectHandle);
        case MCDObjectType.eMCDDBECUVARIANTS:
          return (MCDObject) new DtsDbEcuVariantsImpl(objectHandle);
        case MCDObjectType.eMCDDBFUNCTIONALCLASS:
          return (MCDObject) new DtsDbFunctionalClassImpl(objectHandle);
        case MCDObjectType.eMCDDBFUNCTIONALCLASSES:
          return (MCDObject) new DtsDbFunctionalClassesImpl(objectHandle);
        case MCDObjectType.eMCDDBFUNCTIONALGROUP:
          return (MCDObject) new DtsDbFunctionalGroupImpl(objectHandle);
        case MCDObjectType.eMCDDBFUNCTIONALGROUPS:
          return (MCDObject) new DtsDbFunctionalGroupsImpl(objectHandle);
        case MCDObjectType.eMCDDBENVDATADESC:
          return (MCDObject) new DtsDbEnvDataDescImpl(objectHandle);
        case MCDObjectType.eMCDDBENVDATADESCS:
          return (MCDObject) new DtsDbEnvDataDescsImpl(objectHandle);
        case MCDObjectType.eMCDDBHEXSERVICE:
          return (MCDObject) new DtsDbHexServiceImpl(objectHandle);
        case MCDObjectType.eMCDDBMULTIPLEECUJOB:
          return (MCDObject) new DtsDbMultipleEcuJobImpl(objectHandle);
        case MCDObjectType.eMCDDBPROTOCOLPARAMETER:
          return (MCDObject) new DtsDbProtocolParameterImpl(objectHandle);
        case MCDObjectType.eMCDDBPROTOCOLPARAMETERSET:
          return (MCDObject) new DtsDbProtocolParameterSetImpl(objectHandle);
        case MCDObjectType.eMCDDBREQUESTPARAMETER:
          return (MCDObject) new DtsDbRequestParameterImpl(objectHandle);
        case MCDObjectType.eMCDDBRESPONSE:
          return (MCDObject) new DtsDbResponseImpl(objectHandle);
        case MCDObjectType.eMCDDBRESPONSEPARAMETER:
          return (MCDObject) new DtsDbResponseParameterImpl(objectHandle);
        case MCDObjectType.eMCDDBRESPONSEPARAMETERS:
          return (MCDObject) new DtsDbResponseParametersImpl(objectHandle);
        case MCDObjectType.eMCDDBRESPONSES:
          return (MCDObject) new DtsDbResponsesImpl(objectHandle);
        case MCDObjectType.eMCDDBSERVICE:
          return (MCDObject) new DtsDbServiceImpl(objectHandle);
        case MCDObjectType.eMCDDBSERVICES:
          return (MCDObject) new DtsDbServicesImpl(objectHandle);
        case MCDObjectType.eMCDDBSINGLEECUJOB:
          return (MCDObject) new DtsDbSingleEcuJobImpl(objectHandle);
        case MCDObjectType.eMCDDBSTARTCOMMUNICATION:
          return (MCDObject) new DtsDbStartCommunicationImpl(objectHandle);
        case MCDObjectType.eMCDDBSTOPCOMMUNICATION:
          return (MCDObject) new DtsDbStopCommunicationImpl(objectHandle);
        case MCDObjectType.eMCDDBVARIANTIDENTIFICATION:
          return (MCDObject) new DtsDbVariantIdentificationImpl(objectHandle);
        case MCDObjectType.eMCDDBVARIANTIDENTIFICATIONANDSELECTION:
          return (MCDObject) new DtsDbVariantIdentificationAndSelectionImpl(objectHandle);
        case MCDObjectType.eMCDDBVEHICLECONNECTOR:
          return (MCDObject) new DtsDbVehicleConnectorImpl(objectHandle);
        case MCDObjectType.eMCDDBVEHICLECONNECTORPIN:
          return (MCDObject) new DtsDbVehicleConnectorPinImpl(objectHandle);
        case MCDObjectType.eMCDDBVEHICLECONNECTORPINS:
          return (MCDObject) new DtsDbVehicleConnectorPinsImpl(objectHandle);
        case MCDObjectType.eMCDHEXSERVICE:
          return (MCDObject) new DtsHexServiceImpl(objectHandle);
        case MCDObjectType.eMCDMULTIPLEECUJOB:
          return (MCDObject) new DtsMultipleEcuJobImpl(objectHandle);
        case MCDObjectType.eMCDPROTOCOLPARAMETERSET:
          return (MCDObject) new DtsProtocolParameterSetImpl(objectHandle);
        case MCDObjectType.eMCDREQUESTPARAMETER:
          return (MCDObject) new DtsRequestParameterImpl(objectHandle);
        case MCDObjectType.eMCDREQUESTPARAMETERS:
          return (MCDObject) new DtsRequestParametersImpl(objectHandle);
        case MCDObjectType.eMCDRESULTSTATE:
          return (MCDObject) new DtsResultStateImpl(objectHandle);
        case MCDObjectType.eMCDFLASHJOB:
          return (MCDObject) new DtsFlashJobImpl(objectHandle);
        case MCDObjectType.eMCDSERVICE:
          return (MCDObject) new DtsServiceImpl(objectHandle);
        case MCDObjectType.eMCDSINGLEECUJOB:
          return (MCDObject) new DtsSingleEcuJobImpl(objectHandle);
        case MCDObjectType.eMCDSTARTCOMMUNICATION:
          return (MCDObject) new DtsStartCommunicationImpl(objectHandle);
        case MCDObjectType.eMCDSTOPCOMMUNICATION:
          return (MCDObject) new DtsStopCommunicationImpl(objectHandle);
        case MCDObjectType.eMCDTEXTTABLEELEMENT:
          return (MCDObject) new DtsTextTableElementImpl(objectHandle);
        case MCDObjectType.eMCDTEXTTABLEELEMENTS:
          return (MCDObject) new DtsTextTableElementsImpl(objectHandle);
        case MCDObjectType.eMCDVARIANTIDENTIFICATION:
          return (MCDObject) new DtsVariantIdentificationImpl(objectHandle);
        case MCDObjectType.eMCDVARIANTIDENTIFICATIONANDSELECTION:
          return (MCDObject) new DtsVariantIdentificationAndSelectionImpl(objectHandle);
        case MCDObjectType.eMCDDBFLASHCHECKSUM:
          return (MCDObject) new DtsDbFlashChecksumImpl(objectHandle);
        case MCDObjectType.eMCDDBFLASHCHECKSUMS:
          return (MCDObject) new DtsDbFlashChecksumsImpl(objectHandle);
        case MCDObjectType.eMCDDBFLASHDATA:
          return (MCDObject) new DtsDbFlashDataImpl(objectHandle);
        case MCDObjectType.eMCDDBFLASHDATABLOCK:
          return (MCDObject) new DtsDbFlashDataBlockImpl(objectHandle);
        case MCDObjectType.eMCDDBFLASHDATABLOCKS:
          return (MCDObject) new DtsDbFlashDataBlocksImpl(objectHandle);
        case MCDObjectType.eMCDDBFLASHFILTER:
          return (MCDObject) new DtsDbFlashFilterImpl(objectHandle);
        case MCDObjectType.eMCDDBFLASHFILTERS:
          return (MCDObject) new DtsDbFlashFiltersImpl(objectHandle);
        case MCDObjectType.eMCDDBFLASHIDENT:
          return (MCDObject) new DtsDbFlashIdentImpl(objectHandle);
        case MCDObjectType.eMCDDBFLASHIDENTS:
          return (MCDObject) new DtsDbFlashIdentsImpl(objectHandle);
        case MCDObjectType.eMCDDBFLASHJOB:
          return (MCDObject) new DtsDbFlashJobImpl(objectHandle);
        case MCDObjectType.eMCDDBFLASHSECURITIES:
          return (MCDObject) new DtsDbFlashSecuritiesImpl(objectHandle);
        case MCDObjectType.eMCDDBFLASHSECURITY:
          return (MCDObject) new DtsDbFlashSecurityImpl(objectHandle);
        case MCDObjectType.eMCDDBFLASHSEGMENT:
          return (MCDObject) new DtsDbFlashSegmentImpl(objectHandle);
        case MCDObjectType.eMCDDBFLASHSEGMENTS:
          return (MCDObject) new DtsDbFlashSegmentsImpl(objectHandle);
        case MCDObjectType.eMCDDBFLASHSESSION:
          return (MCDObject) new DtsDbFlashSessionImpl(objectHandle);
        case MCDObjectType.eMCDDBFLASHSESSIONCLASS:
          return (MCDObject) new DtsDbFlashSessionClassImpl(objectHandle);
        case MCDObjectType.eMCDDBFLASHSESSIONCLASSES:
          return (MCDObject) new DtsDbFlashSessionClassesImpl(objectHandle);
        case MCDObjectType.eMCDDBFLASHSESSIONS:
          return (MCDObject) new DtsDbFlashSessionsImpl(objectHandle);
        case MCDObjectType.eMCDDBVEHICLECONNECTORS:
          return (MCDObject) new DtsDbVehicleConnectorsImpl(objectHandle);
        case MCDObjectType.eMCDACCESSKEY:
          return (MCDObject) new DtsAccessKeyImpl(objectHandle);
        case MCDObjectType.eMCDACCESSKEYS:
          return (MCDObject) new DtsAccessKeysImpl(objectHandle);
        case MCDObjectType.eMCDDBDIAGTROUBLECODE:
          return (MCDObject) new DtsDbDiagTroubleCodeImpl(objectHandle);
        case MCDObjectType.eMCDDBDIAGTROUBLECODES:
          return (MCDObject) new DtsDbDiagTroubleCodesImpl(objectHandle);
        case MCDObjectType.eMCDERRORS:
          return (MCDObject) new DtsErrorsImpl(objectHandle);
        case MCDObjectType.eMCDDBUNIT:
          return (MCDObject) new DtsDbUnitImpl(objectHandle);
        case MCDObjectType.eMCDDBDIAGVARIABLE:
          return (MCDObject) new DtsDbDiagVariableImpl(objectHandle);
        case MCDObjectType.eMCDDBDIAGVARIABLES:
          return (MCDObject) new DtsDbDiagVariablesImpl(objectHandle);
        case MCDObjectType.eMCDDBREQUEST:
          return (MCDObject) new DtsDbRequestImpl(objectHandle);
        case MCDObjectType.eMCDREQUEST:
          return (MCDObject) new DtsRequestImpl(objectHandle);
        case MCDObjectType.eMCDDBECUMEM:
          return (MCDObject) new DtsDbEcuMemImpl(objectHandle);
        case MCDObjectType.eMCDDBECUMEMS:
          return (MCDObject) new DtsDbEcuMemsImpl(objectHandle);
        case MCDObjectType.eMCDDBACCESSLEVEL:
          return (MCDObject) new DtsDbAccessLevelImpl(objectHandle);
        case MCDObjectType.eMCDDBREQUESTPARAMETERS:
          return (MCDObject) new DtsDbRequestParametersImpl(objectHandle);
        case MCDObjectType.eMCDDBDYNIDDEFINECOMPRIMITIVE:
          return (MCDObject) new DtsDbDynIdDefineComPrimitiveImpl(objectHandle);
        case MCDObjectType.eMCDDBDYNIDREADCOMPRIMITIVE:
          return (MCDObject) new DtsDbDynIdReadComPrimitiveImpl(objectHandle);
        case MCDObjectType.eMCDDYNIDDEFINECOMPRIMITIVE:
          return (MCDObject) new DtsDynIdDefineComPrimitiveImpl(objectHandle);
        case MCDObjectType.eMCDDYNIDREADCOMPRIMITIVE:
          return (MCDObject) new DtsDynIdReadComPrimitiveImpl(objectHandle);
        case MCDObjectType.eMCDDBDIAGSERVICES:
          return (MCDObject) new DtsDbDiagServicesImpl(objectHandle);
        case MCDObjectType.eMCDDBCONTROLPRIMITIVES:
          return (MCDObject) new DtsDbControlPrimitivesImpl(objectHandle);
        case MCDObjectType.eMCDDIAGCOMPRIMITIVES:
          return (MCDObject) new DtsDiagComPrimitivesImpl(objectHandle);
        case MCDObjectType.eMCDINTERVAL:
          return (MCDObject) new DtsIntervalImpl(objectHandle);
        case MCDObjectType.eMCDDBIDENTDESCRIPTION:
          return (MCDObject) new DtsDbIdentDescriptionImpl(objectHandle);
        case MCDObjectType.eMCDDBPHYSICALMEMORY:
          return (MCDObject) new DtsDbPhysicalMemoryImpl(objectHandle);
        case MCDObjectType.eMCDDBPHYSICALMEMORIES:
          return (MCDObject) new DtsDbPhysicalMemoriesImpl(objectHandle);
        case MCDObjectType.eMCDDBPHYSICALSEGMENT:
          return (MCDObject) new DtsDbPhysicalSegmentImpl(objectHandle);
        case MCDObjectType.eMCDDBPHYSICALSEGMENTS:
          return (MCDObject) new DtsDbPhysicalSegmentsImpl(objectHandle);
        case MCDObjectType.eMCDDBJOBS:
          return (MCDObject) new DtsDbJobsImpl(objectHandle);
        case MCDObjectType.eMCDDBUNITGROUPS:
          return (MCDObject) new DtsDbUnitGroupsImpl(objectHandle);
        case MCDObjectType.eMCDDBUNITGROUP:
          return (MCDObject) new DtsDbUnitGroupImpl(objectHandle);
        case MCDObjectType.eMCDDBDATAPRIMITIVES:
          return (MCDObject) new DtsDbDataPrimitivesImpl(objectHandle);
        case MCDObjectType.eMCDDBDYNIDCLEARCOMPRIMITIVE:
          return (MCDObject) new DtsDbDynIdClearComPrimitiveImpl(objectHandle);
        case MCDObjectType.eMCDDYNIDCLEARCOMPRIMITIVE:
          return (MCDObject) new DtsDynIdClearComPrimitiveImpl(objectHandle);
        case MCDObjectType.eMCDDBSPECIALDATAGROUPS:
          return (MCDObject) new DtsDbSpecialDataGroupsImpl(objectHandle);
        case MCDObjectType.eMCDDBSPECIALDATAGROUP:
          return (MCDObject) new DtsDbSpecialDataGroupImpl(objectHandle);
        case MCDObjectType.eMCDDBSPECIALDATAELEMENT:
          return (MCDObject) new DtsDbSpecialDataElementImpl(objectHandle);
        case MCDObjectType.eMCDDBSPECIALDATAGROUPCAPTION:
          return (MCDObject) new DtsDbSpecialDataGroupCaptionImpl(objectHandle);
        case MCDObjectType.eMCDDBTABLES:
          return (MCDObject) new DtsDbTablesImpl(objectHandle);
        case MCDObjectType.eMCDDBTABLE:
          return (MCDObject) new DtsDbTableImpl(objectHandle);
        case MCDObjectType.eMCDDBTABLEROWCONNECTOR:
          return (MCDObject) new DtsDbTableRowConnectorImpl(objectHandle);
        case MCDObjectType.eMCDDBTABLEROWCONNECTORS:
          return (MCDObject) new DtsDbTableRowConnectorsImpl(objectHandle);
        case MCDObjectType.eMCDDBTABLEPARAMETERS:
          return (MCDObject) new DtsDbTableParametersImpl(objectHandle);
        case MCDObjectType.eMCDDBFAULTMEMORY:
          return (MCDObject) new DtsDbFaultMemoryImpl(objectHandle);
        case MCDObjectType.eMCDDBTABLEPARAMETER:
          return (MCDObject) new DtsDbTableParameterImpl(objectHandle);
        case MCDObjectType.eMCDDBFAULTMEMORIES:
          return (MCDObject) new DtsDbFaultMemoriesImpl(objectHandle);
        case MCDObjectType.eMCDDBPROTOCOLPARAMETERS:
          return (MCDObject) new DtsDbProtocolParametersImpl(objectHandle);
        case MCDObjectType.eMCDPROTOCOLPARAMETER:
          return (MCDObject) new DtsProtocolParameterImpl(objectHandle);
        case MCDObjectType.eMCDPHYSICALINTERFACELINK:
          return (MCDObject) new DtsPhysicalInterfaceLinkImpl(objectHandle);
        case MCDObjectType.eMCDCLASSFACTORY:
          return (MCDObject) new DtsClassFactoryImpl(objectHandle);
        case MCDObjectType.eMCDMONITORLINK:
          return (MCDObject) new DtsMonitorLinkImpl(objectHandle);
        case MCDObjectType.eMCDLOGICALLINKMONITOR:
          return (MCDObject) new DtsLogicalLinkMonitorImpl(objectHandle);
        case MCDObjectType.eMCDCOMPRIMITIVEEVENT:
          return (MCDObject) new DtsComPrimitiveEventImpl(objectHandle);
        case MCDObjectType.eMCDLOGICALLINKEVENT:
          return (MCDObject) new DtsLogicalLinkEventImpl(objectHandle);
        case MCDObjectType.eMCDSYSTEMEVENT:
          return (MCDObject) new DtsSystemEventImpl(objectHandle);
        case MCDObjectType.eMCDDBVARIANTCODINGDOMAIN:
          return (MCDObject) new DtsDbVariantCodingDomainImpl(objectHandle);
        case MCDObjectType.eMCDDBVARIANTCODINGDOMAINS:
          return (MCDObject) new DtsDbVariantCodingDomainsImpl(objectHandle);
        case MCDObjectType.eMCDDBVARIANTCODINGFRAGMENT:
          return (MCDObject) new DtsDbVariantCodingFragmentImpl(objectHandle);
        case MCDObjectType.eMCDDBVARIANTCODINGFRAGMENTS:
          return (MCDObject) new DtsDbVariantCodingFragmentsImpl(objectHandle);
        case MCDObjectType.eMCDOFFLINEVARIANTCODING:
          return (MCDObject) new DtsOfflineVariantCodingImpl(objectHandle);
        case MCDObjectType.eMCDPARAMETERMETAINFO:
          return (MCDObject) new DtsParameterMetaInfoImpl(objectHandle);
        case MCDObjectType.eMCDSMARTCOMPRIMITIVE:
          return (MCDObject) new DtsSmartComPrimitiveImpl(objectHandle);
        case MCDObjectType.eMCDRAWSERVICE:
          return (MCDObject) new DtsRawServiceImpl(objectHandle);
        case MCDObjectType.eMCDDBRAWSERVICE:
          return (MCDObject) new DtsDbRawServiceImpl(objectHandle);
        case MCDObjectType.eMCDDELAY:
          return (MCDObject) new DtsDelayImpl(objectHandle);
        case MCDObjectType.eMCDDBDELAY:
          return (MCDObject) new DtsDbDelayImpl(objectHandle);
        case MCDObjectType.eMCDGOTOOFFLINE:
          return (MCDObject) new DtsGotoOfflineImpl(objectHandle);
        case MCDObjectType.eMCDDBGOTOOFFLINE:
          return (MCDObject) new DtsDbGotoOfflineImpl(objectHandle);
        case MCDObjectType.eMCDGOTOONLINE:
          return (MCDObject) new DtsGotoOnlineImpl(objectHandle);
        case MCDObjectType.eMCDDBGOTOONLINE:
          return (MCDObject) new DtsDbGotoOnlineImpl(objectHandle);
        case MCDObjectType.eMCDDBMATCHINGPATTERNS:
          return (MCDObject) new DtsDbMatchingPatternsImpl(objectHandle);
        case MCDObjectType.eMCDDBMATCHINGPATTERN:
          return (MCDObject) new DtsDbMatchingPatternImpl(objectHandle);
        case MCDObjectType.eMCDDBMATCHINGPARAMETERS:
          return (MCDObject) new DtsDbMatchingParametersImpl(objectHandle);
        case MCDObjectType.eMCDDBMATCHINGPARAMETER:
          return (MCDObject) new DtsDbMatchingParameterImpl(objectHandle);
        case MCDObjectType.eMCDCONSTRAINT:
          return (MCDObject) new DtsConstraintImpl(objectHandle);
        case MCDObjectType.eMCDSCALECONSTRAINT:
          return (MCDObject) new DtsScaleConstraintImpl(objectHandle);
        case MCDObjectType.eMCDSCALECONSTRAINTS:
          return (MCDObject) new DtsScaleConstraintsImpl(objectHandle);
        case MCDObjectType.eMCDDBCONFIGURATIONRECORD:
          return (MCDObject) new DtsDbConfigurationRecordImpl(objectHandle);
        case MCDObjectType.eMCDDBDATARECORDS:
          return (MCDObject) new DtsDbDataRecordsImpl(objectHandle);
        case MCDObjectType.eMCDDBDATARECORD:
          return (MCDObject) new DtsDbDataRecordImpl(objectHandle);
        case MCDObjectType.eMCDDBCODINGDATA:
          return (MCDObject) new DtsDbCodingDataImpl(objectHandle);
        case MCDObjectType.eMCDDBSYSTEMITEM:
          return (MCDObject) new DtsDbSystemItemImpl(objectHandle);
        case MCDObjectType.eMCDDBCONFIGURATIONIDITEM:
          return (MCDObject) new DtsDbConfigurationIdItemImpl(objectHandle);
        case MCDObjectType.eMCDDBDATAIDITEM:
          return (MCDObject) new DtsDbDataIdItemImpl(objectHandle);
        case MCDObjectType.eMCDDBOPTIONITEM:
          return (MCDObject) new DtsDbOptionItemImpl(objectHandle);
        case MCDObjectType.eMCDDBSYSTEMITEMS:
          return (MCDObject) new DtsDbSystemItemsImpl(objectHandle);
        case MCDObjectType.eMCDDBOPTIONITEMS:
          return (MCDObject) new DtsDbOptionItemsImpl(objectHandle);
        case MCDObjectType.eMCDDBITEMVALUE:
          return (MCDObject) new DtsDbItemValueImpl(objectHandle);
        case MCDObjectType.eMCDDBITEMVALUES:
          return (MCDObject) new DtsDbItemValuesImpl(objectHandle);
        case MCDObjectType.eMCDCONFIGURATIONRECORDS:
          return (MCDObject) new DtsConfigurationRecordsImpl(objectHandle);
        case MCDObjectType.eMCDCONFIGURATIONRECORD:
          return (MCDObject) new DtsConfigurationRecordImpl(objectHandle);
        case MCDObjectType.eMCDOPTIONITEM:
          return (MCDObject) new DtsOptionItemImpl(objectHandle);
        case MCDObjectType.eMCDCONFIGURATIONIDITEM:
          return (MCDObject) new DtsConfigurationIdItemImpl(objectHandle);
        case MCDObjectType.eMCDDATAIDITEM:
          return (MCDObject) new DtsDataIdItemImpl(objectHandle);
        case MCDObjectType.eMCDSYSTEMITEM:
          return (MCDObject) new DtsSystemItemImpl(objectHandle);
        case MCDObjectType.eMCDWRITEDIAGCOMPRIMITIVES:
          return (MCDObject) new DtsWriteDiagComPrimitivesImpl(objectHandle);
        case MCDObjectType.eMCDREADDIAGCOMPRIMITIVES:
          return (MCDObject) new DtsReadDiagComPrimitivesImpl(objectHandle);
        case MCDObjectType.eMCDSYSTEMITEMS:
          return (MCDObject) new DtsSystemItemsImpl(objectHandle);
        case MCDObjectType.eMCDDBPHYSICALDIMENSION:
          return (MCDObject) new DtsDbPhysicalDimensionImpl(objectHandle);
        case MCDObjectType.eMCDAUDIENCE:
          return (MCDObject) new DtsAudienceImpl(objectHandle);
        case MCDObjectType.eMCDDBADDITIONALAUDIENCE:
          return (MCDObject) new DtsDbAdditionalAudienceImpl(objectHandle);
        case MCDObjectType.eMCDDBADDITIONALAUDIENCES:
          return (MCDObject) new DtsDbAdditionalAudiencesImpl(objectHandle);
        case MCDObjectType.eMCDINTERVALS:
          return (MCDObject) new DtsIntervalsImpl(objectHandle);
        case MCDObjectType.eMCDDBFUNCTIONDICTIONARIES:
          return (MCDObject) new DtsDbFunctionDictionariesImpl(objectHandle);
        case MCDObjectType.eMCDDBFUNCTIONDICTIONARY:
          return (MCDObject) new DtsDbFunctionDictionaryImpl(objectHandle);
        case MCDObjectType.eMCDDBFUNCTIONINPARAMETERS:
          return (MCDObject) new DtsDbFunctionInParametersImpl(objectHandle);
        case MCDObjectType.eMCDDBFUNCTIONINPARAMETER:
          return (MCDObject) new DtsDbFunctionInParameterImpl(objectHandle);
        case MCDObjectType.eMCDDBFUNCTIONOUTPARAMETERS:
          return (MCDObject) new DtsDbFunctionOutParametersImpl(objectHandle);
        case MCDObjectType.eMCDDBFUNCTIONOUTPARAMETER:
          return (MCDObject) new DtsDbFunctionOutParameterImpl(objectHandle);
        case MCDObjectType.eMCDDBDIAGOBJECTCONNECTOR:
          return (MCDObject) new DtsDbDiagObjectConnectorImpl(objectHandle);
        case MCDObjectType.eMCDDBDIAGTROUBLECODECONNECTORS:
          return (MCDObject) new DtsDbDiagTroubleCodeConnectorsImpl(objectHandle);
        case MCDObjectType.eMCDDBDIAGTROUBLECODECONNECTOR:
          return (MCDObject) new DtsDbDiagTroubleCodeConnectorImpl(objectHandle);
        case MCDObjectType.eMCDDBENVDATACONNECTORS:
          return (MCDObject) new DtsDbEnvDataConnectorsImpl(objectHandle);
        case MCDObjectType.eMCDDBENVDATACONNECTOR:
          return (MCDObject) new DtsDbEnvDataConnectorImpl(objectHandle);
        case MCDObjectType.eMCDDBFUNCTIONDIAGCOMCONNECTORS:
          return (MCDObject) new DtsDbFunctionDiagComConnectorsImpl(objectHandle);
        case MCDObjectType.eMCDDBFUNCTIONDIAGCOMCONNECTOR:
          return (MCDObject) new DtsDbFunctionDiagComConnectorImpl(objectHandle);
        case MCDObjectType.eMCDDBFUNCTIONNODE:
          return (MCDObject) new DtsDbFunctionNodeImpl(objectHandle);
        case MCDObjectType.eMCDDBFUNCTIONNODEGROUP:
          return (MCDObject) new DtsDbFunctionNodeGroupImpl(objectHandle);
        case MCDObjectType.eMCDDBCOMPONENTCONNECTORS:
          return (MCDObject) new DtsDbComponentConnectorsImpl(objectHandle);
        case MCDObjectType.eMCDDBCOMPONENTCONNECTOR:
          return (MCDObject) new DtsDbComponentConnectorImpl(objectHandle);
        case MCDObjectType.eMCDDBSUBCOMPONENTS:
          return (MCDObject) new DtsDbSubComponentsImpl(objectHandle);
        case MCDObjectType.eMCDDBSUBCOMPONENTPARAMCONNECTOR:
          return (MCDObject) new DtsDbSubComponentParamConnectorImpl(objectHandle);
        case MCDObjectType.eMCDDBFUNCTIONNODES:
          return (MCDObject) new DtsDbFunctionNodesImpl(objectHandle);
        case MCDObjectType.eMCDDBFUNCTIONNODEGROUPS:
          return (MCDObject) new DtsDbFunctionNodeGroupsImpl(objectHandle);
        case MCDObjectType.eMCDDBSUBCOMPONENTPARAMCONNECTORS:
          return (MCDObject) new DtsDbSubComponentParamConnectorsImpl(objectHandle);
        case MCDObjectType.eMCDMONITORINGLINK:
          return (MCDObject) new DtsMonitoringLinkImpl(objectHandle);
        case MCDObjectType.eMCDINTERFACERESOURCE:
          return (MCDObject) new DtsInterfaceResourceImpl(objectHandle);
        case MCDObjectType.eMCDDBSUBCOMPONENT:
          return (MCDObject) new DtsDbSubComponentImpl(objectHandle);
        case MCDObjectType.eMCDINTERFACERESOURCES:
          return (MCDObject) new DtsInterfaceResourcesImpl(objectHandle);
        case MCDObjectType.eMCDINTERFACE:
          return (MCDObject) new DtsInterfaceImpl(objectHandle);
        case MCDObjectType.eMCDINTERFACES:
          return (MCDObject) new DtsInterfacesImpl(objectHandle);
        case MCDObjectType.eMCDDBINTERFACECABLE:
          return (MCDObject) new DtsDbInterfaceCableImpl(objectHandle);
        case MCDObjectType.eMCDDBINTERFACECABLES:
          return (MCDObject) new DtsDbInterfaceCablesImpl(objectHandle);
        case MCDObjectType.eMCDDBINTERFACECONNECTORPIN:
          return (MCDObject) new DtsDbInterfaceConnectorPinImpl(objectHandle);
        case MCDObjectType.eMCDDBINTERFACECONNECTORPINS:
          return (MCDObject) new DtsDbInterfaceConnectorPinsImpl(objectHandle);
        case MCDObjectType.eMCDDBPHYSICALVEHICLELINK:
          return (MCDObject) new DtsDbPhysicalVehicleLinkImpl(objectHandle);
        case MCDObjectType.eMCDMESSAGEFILTER:
          return (MCDObject) new DtsMessageFilterImpl(objectHandle);
        case MCDObjectType.eMCDMESSAGEFILTERS:
          return (MCDObject) new DtsMessageFiltersImpl(objectHandle);
        case MCDObjectType.eMCDMESSAGEFILTERVALUES:
          return (MCDObject) new DtsMessageFilterValuesImpl(objectHandle);
        case MCDObjectType.eMCDDBCONFIGURATIONDATA:
          return (MCDObject) new DtsDbConfigurationDataImpl(objectHandle);
        case MCDObjectType.eMCDDBCONFIGURATIONDATAS:
          return (MCDObject) new DtsDbConfigurationDatasImpl(objectHandle);
        case MCDObjectType.eMCDDBCONFIGURATIONRECORDS:
          return (MCDObject) new DtsDbConfigurationRecordsImpl(objectHandle);
        case MCDObjectType.eMCDDBECUSTATE:
          return (MCDObject) new DtsDbEcuStateImpl(objectHandle);
        case MCDObjectType.eMCDDBEXTERNALACCESSMETHOD:
          return (MCDObject) new DtsDbExternalAccessMethodImpl(objectHandle);
        case MCDObjectType.eMCDDBCODEINFORMATION:
          return (MCDObject) new DtsDbCodeInformationImpl(objectHandle);
        case MCDObjectType.eMCDDBPRECONDITIONDEFINITIONS:
          return (MCDObject) new DtsDbPreconditionDefinitionsImpl(objectHandle);
        case MCDObjectType.eMCDDBECUSTATETRANSITIONS:
          return (MCDObject) new DtsDbEcuStateTransitionsImpl(objectHandle);
        case MCDObjectType.eMCDDBECUSTATETRANSITIONACTIONS:
          return (MCDObject) new DtsDbEcuStateTransitionActionsImpl(objectHandle);
        case MCDObjectType.eMCDOPTIONITEMS:
          return (MCDObject) new DtsOptionItemsImpl(objectHandle);
        case MCDObjectType.eMCDDBCODEINFORMATIONS:
          return (MCDObject) new DtsDbCodeInformationsImpl(objectHandle);
        case MCDObjectType.eMCDDBECUSTATETRANSITIONACTION:
          return (MCDObject) new DtsDbEcuStateTransitionActionImpl(objectHandle);
        case MCDObjectType.eMCDDBPRECONDITIONDEFINITION:
          return (MCDObject) new DtsDbPreconditionDefinitionImpl(objectHandle);
        case MCDObjectType.eMCDDBECUSTATETRANSITION:
          return (MCDObject) new DtsDbEcuStateTransitionImpl(objectHandle);
        case MCDObjectType.eMCDDBECUSTATECHARTS:
          return (MCDObject) new DtsDbEcuStateChartsImpl(objectHandle);
        case MCDObjectType.eMCDDBECUSTATECHART:
          return (MCDObject) new DtsDbEcuStateChartImpl(objectHandle);
        case MCDObjectType.eMCDDBECUSTATES:
          return (MCDObject) new DtsDbEcuStatesImpl(objectHandle);
        case MCDObjectType.eMCDFLASHSEGMENTITERATOR:
          return (MCDObject) new DtsFlashSegmentIteratorImpl(objectHandle);
        case MCDObjectType.eMCDDBVARIANTCODINGSTRING:
          return (MCDObject) new DtsDbVariantCodingStringImpl(objectHandle);
        case MCDObjectType.eMCDDBVARIANTCODINGSTRINGS:
          return (MCDObject) new DtsDbVariantCodingStringsImpl(objectHandle);
        case MCDObjectType.eMCDENUMVALUE:
          return (MCDObject) new DtsEnumValueImpl(objectHandle);
        case MCDObjectType.eMCDDATATYPECOLLECTION:
          return (MCDObject) new DtsDatatypeCollectionImpl(objectHandle);
        case MCDObjectType.eMCDGLOBALPROTOCOLPARAMETER:
          return (MCDObject) new DtsGlobalProtocolParameterImpl(objectHandle);
        case MCDObjectType.eMCDGLOBALPROTOCOLPARAMETERS:
          return (MCDObject) new DtsGlobalProtocolParametersImpl(objectHandle);
        case MCDObjectType.eMCDGLOBALPROTOCOLPARAMETERSET:
          return (MCDObject) new DtsGlobalProtocolParameterSetImpl(objectHandle);
        case MCDObjectType.eMCDGLOBALPROTOCOLPARAMETERSETS:
          return (MCDObject) new DtsGlobalProtocolParameterSetsImpl(objectHandle);
        case MCDObjectType.eMCDVEHICLEINFORMATIONCONFIGS:
          return (MCDObject) new DtsVehicleInformationConfigsImpl(objectHandle);
        case MCDObjectType.eMCDVEHICLEINFORMATIONCONFIG:
          return (MCDObject) new DtsVehicleInformationConfigImpl(objectHandle);
        case MCDObjectType.eMCDSYSTEMPROPERTY:
          return (MCDObject) new DtsSystemPropertyImpl(objectHandle);
        case MCDObjectType.eMCDSYSTEMPROPERTIES:
          return (MCDObject) new DtsSystemPropertiesImpl(objectHandle);
        case MCDObjectType.eMCDSYSTEMCONFIG:
          return (MCDObject) new DtsSystemConfigImpl(objectHandle);
        case MCDObjectType.eMCDPROJECTCONFIGS:
          return (MCDObject) new DtsProjectConfigsImpl(objectHandle);
        case MCDObjectType.eMCDPROJECTCONFIG:
          return (MCDObject) new DtsProjectConfigImpl(objectHandle);
        case MCDObjectType.eMCDLOGICALLINKCONFIGS:
          return (MCDObject) new DtsLogicalLinkConfigsImpl(objectHandle);
        case MCDObjectType.eMCDLOGICALLINKCONFIG:
          return (MCDObject) new DtsLogicalLinkConfigImpl(objectHandle);
        case MCDObjectType.eMCDLINKMAPPINGS:
          return (MCDObject) new DtsLinkMappingsImpl(objectHandle);
        case MCDObjectType.eMCDLINKMAPPING:
          return (MCDObject) new DtsLinkMappingImpl(objectHandle);
        case MCDObjectType.eMCDLICENSEINFOS:
          return (MCDObject) new DtsLicenseInfosImpl(objectHandle);
        case MCDObjectType.eMCDLICENSEINFO:
          return (MCDObject) new DtsLicenseInfoImpl(objectHandle);
        case MCDObjectType.eMCDINTERFACECONFIG:
          return (MCDObject) new DtsInterfaceConfigImpl(objectHandle);
        case MCDObjectType.eMCDINTERFACELINKCONFIG:
          return (MCDObject) new DtsInterfaceLinkConfigImpl(objectHandle);
        case MCDObjectType.eMCDINTERFACELINKCONFIGS:
          return (MCDObject) new DtsInterfaceLinkConfigsImpl(objectHandle);
        case MCDObjectType.eMCDINTERFACECONFIGS:
          return (MCDObject) new DtsInterfaceConfigsImpl(objectHandle);
        case MCDObjectType.eMCDTRACECONFIG:
          return (MCDObject) new DtsTraceConfigImpl(objectHandle);
        case MCDObjectType.eMCDJAVACONFIG:
          return (MCDObject) new DtsJavaConfigImpl(objectHandle);
        case MCDObjectType.eMCDFILELOCATION:
          return (MCDObject) new DtsFileLocationImpl(objectHandle);
        case MCDObjectType.eMCDFILELOCATIONS:
          return (MCDObject) new DtsFileLocationsImpl(objectHandle);
        case MCDObjectType.eMCDINTERFACEINFORMATION:
          return (MCDObject) new DtsInterfaceInformationImpl(objectHandle);
        case MCDObjectType.eMCDINTERFACEINFORMATIONS:
          return (MCDObject) new DtsInterfaceInformationsImpl(objectHandle);
        case MCDObjectType.eMCDINTERFACELINKINFORMATION:
          return (MCDObject) new DtsInterfaceLinkInformationImpl(objectHandle);
        case MCDObjectType.eMCDINTERFACELINKINFORMATIONS:
          return (MCDObject) new DtsInterfaceLinkInformationsImpl(objectHandle);
        case MCDObjectType.eMCDDBDATAOBJECTPROP:
          return (MCDObject) new DtsDbDataObjectPropImpl(objectHandle);
        case MCDObjectType.eMCDDBCOMPUMETHOD:
          return (MCDObject) new DtsDbCompuMethodImpl(objectHandle);
        case MCDObjectType.eMCDDBCOMPUTATION:
          return (MCDObject) new DtsDbComputationImpl(objectHandle);
        case MCDObjectType.eMCDDBCOMPUSCALES:
          return (MCDObject) new DtsDbCompuScalesImpl(objectHandle);
        case MCDObjectType.eMCDDBCOMPUSCALE:
          return (MCDObject) new DtsDbCompuScaleImpl(objectHandle);
        case MCDObjectType.eMCDDBLIMIT:
          return (MCDObject) new DtsDbLimitImpl(objectHandle);
        case MCDObjectType.eMCDIDENTIFIERINFO:
          return (MCDObject) new DtsIdentifierInfoImpl(objectHandle);
        case MCDObjectType.eMCDIDENTIFIERINFOS:
          return (MCDObject) new DtsIdentifierInfosImpl(objectHandle);
        case MCDObjectType.eMCDLOGICALLINKFILTERCONFIG:
          return (MCDObject) new DtsLogicalLinkFilterConfigImpl(objectHandle);
        case MCDObjectType.eMCDLOGICALLINKFILTERCONFIGS:
          return (MCDObject) new DtsLogicalLinkFilterConfigsImpl(objectHandle);
        case MCDObjectType.eMCDCANFILTERS:
          return (MCDObject) new DtsCanFiltersImpl(objectHandle);
        case MCDObjectType.eMCDCANFILTER:
          return (MCDObject) new DtsCanFilterImpl(objectHandle);
        case MCDObjectType.eMCDCANFILTERENTRY:
          return (MCDObject) new DtsCanFilterEntryImpl(objectHandle);
        case MCDObjectType.eMCDCANFILTERENTRIES:
          return (MCDObject) new DtsCanFilterEntriesImpl(objectHandle);
        case MCDObjectType.eMCDDOIPMONITORLINK:
          return (MCDObject) new DtsDoIPMonitorLinkImpl(objectHandle);
        case MCDObjectType.eMCDPDUAPIINFORMATION:
          return (MCDObject) new DtsPduApiInformationImpl(objectHandle);
        case MCDObjectType.eMCDPDUAPIINFORMATIONS:
          return (MCDObject) new DtsPduApiInformationsImpl(objectHandle);
        case MCDObjectType.eMCDWLANSIGNALDATA:
          return (MCDObject) new DtsWLanSignalDataImpl(objectHandle);
      }
    }
    return (MCDObject) null;
  }

  internal static void unregisterObject(IntPtr handle)
  {
    lock (DTS_ObjectMapper.m_objectTable)
    {
      object obj = DTS_ObjectMapper.m_objectTable[(object) handle];
      if (obj == null || ((WeakReference) obj).Target != null)
        return;
      DTS_ObjectMapper.m_objectTable.Remove((object) handle);
      CSWrap.CSNIDTS_releaseObject(handle);
    }
  }

  internal static void registerObject(IntPtr handle, object newObject)
  {
    lock (DTS_ObjectMapper.m_objectTable)
    {
      if (DTS_ObjectMapper.m_objectTable.ContainsKey((object) handle))
      {
        DTS_ObjectMapper.m_objectTable.Remove((object) handle);
        DTS_ObjectMapper.m_objectTable.Add((object) handle, (object) new WeakReference(newObject));
      }
      else
        DTS_ObjectMapper.m_objectTable.Add((object) handle, (object) new WeakReference(newObject));
    }
  }

  internal static void registerCallbacks()
  {
    if (DTS_ObjectMapper.m_initDone)
      return;
    DTS_ObjectMapper.m_eventCallback += new onEvent(DTS_ObjectMapper.handleEvent);
    CSWrap.CSNIDTS_registerCallback(DTS_ObjectMapper.m_eventCallback);
    DTS_ObjectMapper.m_initDone = true;
  }

  internal static bool handleEvent(
    ref ObjectInfo_Struct targetObject,
    ref ObjectInfo_Struct eventObject)
  {
    bool flag = false;
    MCDObject mcdObject = DTS_ObjectMapper.createObject(targetObject.m_handle, targetObject.m_type);
    DtsEvent dtsEvent = (DtsEvent) DTS_ObjectMapper.createObject(eventObject.m_handle, eventObject.m_type);
    if (mcdObject != null)
    {
      switch (mcdObject.ObjectType)
      {
        case MCDObjectType.eMCDLOGICALLINK:
          DtsLogicalLinkImpl dtsLogicalLinkImpl = (DtsLogicalLinkImpl) mcdObject;
          switch (dtsEvent.EventId)
          {
            case DtsEventId.eLINK_ERROR:
              flag = dtsLogicalLinkImpl._onLinkError(dtsEvent.LogicalLink, dtsEvent.Error);
              break;
            case DtsEventId.eLINK_ONE_VARIANT_IDENTIFIED:
              flag = dtsLogicalLinkImpl._onLinkOneVariantIdentified(dtsEvent.LogicalLink, dtsEvent.LogicalLinkState);
              break;
            case DtsEventId.eLINK_ONE_VARIANT_SELECTED:
              flag = dtsLogicalLinkImpl._onLinkOneVariantSelected(dtsEvent.LogicalLink, dtsEvent.LogicalLinkState);
              break;
            case DtsEventId.eLINK_QUEUE_CLEARED:
              flag = dtsLogicalLinkImpl._onLinkQueueCleared(dtsEvent.LogicalLink, dtsEvent.LogicalLinkState);
              break;
            case DtsEventId.eLINK_RESUMED:
              flag = dtsLogicalLinkImpl._onLinkActivityStateRunning(dtsEvent.LogicalLink, dtsEvent.LogicalLinkState);
              break;
            case DtsEventId.eLINK_SUSPENDED:
              flag = dtsLogicalLinkImpl._onLinkActivityStateSuspended(dtsEvent.LogicalLink, dtsEvent.LogicalLinkState);
              break;
            case DtsEventId.eLINK_FINISHED:
              flag = dtsLogicalLinkImpl._onLinkActivityStateIdle(dtsEvent.LogicalLink, dtsEvent.LogicalLinkState);
              break;
            case DtsEventId.eLINK_VARIANT_IDENTIFIED:
              flag = dtsLogicalLinkImpl._onLinkVariantIdentified(dtsEvent.LogicalLink, dtsEvent.LogicalLinkState);
              break;
            case DtsEventId.eLINK_VARIANT_SELECTED:
              flag = dtsLogicalLinkImpl._onLinkVariantSelected(dtsEvent.LogicalLink, dtsEvent.LogicalLinkState);
              break;
            case DtsEventId.eLINK_VARIANT_SET:
              flag = dtsLogicalLinkImpl._onLinkVariantSet(dtsEvent.LogicalLink, dtsEvent.LogicalLinkState);
              break;
            case DtsEventId.eLINK_OPEN:
              flag = dtsLogicalLinkImpl._onLinkStateOffline(dtsEvent.LogicalLink);
              break;
            case DtsEventId.eLINK_CLOSED:
              flag = dtsLogicalLinkImpl._onLinkStateCreated(dtsEvent.LogicalLink);
              break;
            case DtsEventId.eLINK_LOCKED:
              flag = dtsLogicalLinkImpl._onLinkLocked(dtsEvent.LogicalLink);
              break;
            case DtsEventId.eLINK_UNLOCKED:
              flag = dtsLogicalLinkImpl._onLinkUnlocked(dtsEvent.LogicalLink);
              break;
            case DtsEventId.eLINK_ONLINE:
              flag = dtsLogicalLinkImpl._onLinkStateOnline(dtsEvent.LogicalLink);
              break;
            case DtsEventId.eLINK_COMMUNICATION:
              flag = dtsLogicalLinkImpl._onLinkStateCommunication(dtsEvent.LogicalLink);
              break;
            case DtsEventId.eLINK_DEFINABLE_DYNID_CHANGED:
              flag = dtsLogicalLinkImpl._onDefinableDynIdListChanged(dtsEvent.DynIdList, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_BUFFER_OVERFLOW:
              flag = dtsLogicalLinkImpl._onPrimitiveBufferOverflow(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_CANCELED_DURING_EXECUTION:
              flag = dtsLogicalLinkImpl._onPrimitiveCanceledDuringExecution(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_CANCELED_FROM_QUEUE:
              flag = dtsLogicalLinkImpl._onPrimitiveCanceledFromQueue(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_ERROR:
              flag = dtsLogicalLinkImpl._onPrimitiveError(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.Error);
              break;
            case DtsEventId.ePRIMITIVE_HAS_INTERMEDIATE_RESULT:
              flag = dtsLogicalLinkImpl._onPrimitiveHasIntermediateResult(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
              break;
            case DtsEventId.ePRIMITIVE_HAS_RESULT:
              flag = dtsLogicalLinkImpl._onPrimitiveHasResult(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
              break;
            case DtsEventId.ePRIMITIVE_PROGRESS_INFO:
              flag = dtsLogicalLinkImpl._onPrimitiveProgressInfo(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.Progress);
              break;
            case DtsEventId.ePRIMITIVE_JOB_INFO:
              flag = dtsLogicalLinkImpl._onPrimitiveJobInfo(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.JobInfo);
              break;
            case DtsEventId.ePRIMITIVE_REPETION_STOPPED:
              flag = dtsLogicalLinkImpl._onPrimitiveRepetitionStopped(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_TERMINATED:
              flag = dtsLogicalLinkImpl._onPrimitiveTerminated(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
              break;
          }
          break;
        case MCDObjectType.eMCDSYSTEM:
          DtsSystemImpl dtsSystemImpl = (DtsSystemImpl) mcdObject;
          switch (dtsEvent.EventId)
          {
            case DtsEventId.eLINK_ERROR:
              flag = dtsSystemImpl._onLinkError(dtsEvent.LogicalLink, dtsEvent.Error);
              break;
            case DtsEventId.eLINK_ONE_VARIANT_IDENTIFIED:
              flag = dtsSystemImpl._onLinkOneVariantIdentified(dtsEvent.LogicalLink, dtsEvent.LogicalLinkState);
              break;
            case DtsEventId.eLINK_ONE_VARIANT_SELECTED:
              flag = dtsSystemImpl._onLinkOneVariantSelected(dtsEvent.LogicalLink, dtsEvent.LogicalLinkState);
              break;
            case DtsEventId.eLINK_QUEUE_CLEARED:
              flag = dtsSystemImpl._onLinkQueueCleared(dtsEvent.LogicalLink, dtsEvent.LogicalLinkState);
              break;
            case DtsEventId.eLINK_RESUMED:
              flag = dtsSystemImpl._onLinkActivityStateRunning(dtsEvent.LogicalLink, dtsEvent.LogicalLinkState);
              break;
            case DtsEventId.eLINK_SUSPENDED:
              flag = dtsSystemImpl._onLinkActivityStateSuspended(dtsEvent.LogicalLink, dtsEvent.LogicalLinkState);
              break;
            case DtsEventId.eLINK_FINISHED:
              flag = dtsSystemImpl._onLinkActivityStateIdle(dtsEvent.LogicalLink, dtsEvent.LogicalLinkState);
              break;
            case DtsEventId.eLINK_VARIANT_IDENTIFIED:
              flag = dtsSystemImpl._onLinkVariantIdentified(dtsEvent.LogicalLink, dtsEvent.LogicalLinkState);
              break;
            case DtsEventId.eLINK_VARIANT_SELECTED:
              flag = dtsSystemImpl._onLinkVariantSelected(dtsEvent.LogicalLink, dtsEvent.LogicalLinkState);
              break;
            case DtsEventId.eLINK_VARIANT_SET:
              flag = dtsSystemImpl._onLinkVariantSet(dtsEvent.LogicalLink, dtsEvent.LogicalLinkState);
              break;
            case DtsEventId.eLINK_OPEN:
              flag = dtsSystemImpl._onLinkStateOffline(dtsEvent.LogicalLink);
              break;
            case DtsEventId.eLINK_CLOSED:
              flag = dtsSystemImpl._onLinkStateCreated(dtsEvent.LogicalLink);
              break;
            case DtsEventId.eLINK_LOCKED:
              flag = dtsSystemImpl._onLinkLocked(dtsEvent.LogicalLink);
              break;
            case DtsEventId.eLINK_UNLOCKED:
              flag = dtsSystemImpl._onLinkUnlocked(dtsEvent.LogicalLink);
              break;
            case DtsEventId.eLINK_ONLINE:
              flag = dtsSystemImpl._onLinkStateOnline(dtsEvent.LogicalLink);
              break;
            case DtsEventId.eLINK_COMMUNICATION:
              flag = dtsSystemImpl._onLinkStateCommunication(dtsEvent.LogicalLink);
              break;
            case DtsEventId.eLINK_DEFINABLE_DYNID_CHANGED:
              flag = dtsSystemImpl._onDefinableDynIdListChanged(dtsEvent.DynIdList, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_BUFFER_OVERFLOW:
              flag = dtsSystemImpl._onPrimitiveBufferOverflow(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_CANCELED_DURING_EXECUTION:
              flag = dtsSystemImpl._onPrimitiveCanceledDuringExecution(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_CANCELED_FROM_QUEUE:
              flag = dtsSystemImpl._onPrimitiveCanceledFromQueue(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_ERROR:
              flag = dtsSystemImpl._onPrimitiveError(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.Error);
              break;
            case DtsEventId.ePRIMITIVE_HAS_INTERMEDIATE_RESULT:
              flag = dtsSystemImpl._onPrimitiveHasIntermediateResult(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
              break;
            case DtsEventId.ePRIMITIVE_HAS_RESULT:
              flag = dtsSystemImpl._onPrimitiveHasResult(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
              break;
            case DtsEventId.ePRIMITIVE_PROGRESS_INFO:
              flag = dtsSystemImpl._onPrimitiveProgressInfo(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.Progress);
              break;
            case DtsEventId.ePRIMITIVE_JOB_INFO:
              flag = dtsSystemImpl._onPrimitiveJobInfo(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.JobInfo);
              break;
            case DtsEventId.ePRIMITIVE_REPETION_STOPPED:
              flag = dtsSystemImpl._onPrimitiveRepetitionStopped(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_TERMINATED:
              flag = dtsSystemImpl._onPrimitiveTerminated(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
              break;
            case DtsEventId.eSYSTEM_CLAMP_STATE_CHANGED:
              flag = dtsSystemImpl._onSystemClampStateChanged(dtsEvent.JobInfo, dtsEvent.ClampState);
              break;
            case DtsEventId.eSYSTEM_ERROR:
              flag = dtsSystemImpl._onSystemError(dtsEvent.Error);
              break;
            case DtsEventId.eSYSTEM_LOGICALLY_CONNECTED:
              flag = dtsSystemImpl._onSystemLogicallyConnected();
              break;
            case DtsEventId.eSYSTEM_LOGICALLY_DISCONNECTED:
              flag = dtsSystemImpl._onSystemLogicallyDisconnected();
              break;
            case DtsEventId.eSYSTEM_PROJECT_SELECTED:
              flag = dtsSystemImpl._onSystemProjectSelected();
              break;
            case DtsEventId.eSYSTEM_PROJECT_DESELECTED:
              flag = dtsSystemImpl._onSystemProjectDeselected();
              break;
            case DtsEventId.eSYSTEM_VEHICLEINFORMATION_SELECTED:
              flag = dtsSystemImpl._onSystemVehicleInfoSelected();
              break;
            case DtsEventId.eSYSTEM_VEHICLEINFORMATION_DESELECTED:
              flag = dtsSystemImpl._onSystemVehicleInfoDeselected();
              break;
            case DtsEventId.eSYSTEM_CONFIGURATION_OPENED:
              flag = dtsSystemImpl._onSystemConfigurationOpened();
              break;
            case DtsEventId.eSYSTEM_CONFIGURATION_CLOSED:
              flag = dtsSystemImpl._onSystemConfigurationClosed();
              break;
            case DtsEventId.eSYSTEM_LOCKED:
              flag = dtsSystemImpl._onSystemLocked();
              break;
            case DtsEventId.eSYSTEM_UNLOCKED:
              flag = dtsSystemImpl._onSystemUnlocked();
              break;
            case DtsEventId.eCONFIGURATION_RECORD_LOADED:
              flag = dtsSystemImpl._onConfigurationRecordLoaded(dtsEvent.ConfigurationRecord);
              break;
            case DtsEventId.eSTATIC_INTERFACE_ERROR:
              flag = dtsSystemImpl._onStaticInterfaceError(dtsEvent.Error);
              break;
            case DtsEventId.eINTERFACE_ERROR:
              flag = dtsSystemImpl._onInterfaceError(dtsEvent.Interface, dtsEvent.Error);
              break;
            case DtsEventId.eINTERFACE_STATUS_CHANGED:
              flag = dtsSystemImpl._onInterfaceStatusChanged(dtsEvent.Interface, dtsEvent.InterfaceStatus);
              break;
            case DtsEventId.eINTERFACES_MODIFIED:
              flag = dtsSystemImpl._onInterfacesModified();
              break;
            case DtsEventId.eINTERFACE_DETECTED:
              flag = dtsSystemImpl._onInterfaceDetected(dtsEvent.Interface);
              break;
            case DtsEventId.eDETECTION_FINISHED:
              flag = dtsSystemImpl._onDetectionFinished();
              break;
            case DtsEventId.eMONITORING_FRAMES_READY:
              flag = dtsSystemImpl._onMonitoringFramesReady(dtsEvent.MonitoringLink);
              break;
          }
          break;
        case MCDObjectType.eMCDHEXSERVICE:
          DtsHexServiceImpl dtsHexServiceImpl = (DtsHexServiceImpl) mcdObject;
          switch (dtsEvent.EventId)
          {
            case DtsEventId.ePRIMITIVE_BUFFER_OVERFLOW:
              flag = dtsHexServiceImpl._onPrimitiveBufferOverflow(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_CANCELED_DURING_EXECUTION:
              flag = dtsHexServiceImpl._onPrimitiveCanceledDuringExecution(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_CANCELED_FROM_QUEUE:
              flag = dtsHexServiceImpl._onPrimitiveCanceledFromQueue(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_ERROR:
              flag = dtsHexServiceImpl._onPrimitiveError(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.Error);
              break;
            case DtsEventId.ePRIMITIVE_HAS_INTERMEDIATE_RESULT:
              flag = dtsHexServiceImpl._onPrimitiveHasIntermediateResult(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
              break;
            case DtsEventId.ePRIMITIVE_HAS_RESULT:
              flag = dtsHexServiceImpl._onPrimitiveHasResult(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
              break;
            case DtsEventId.ePRIMITIVE_PROGRESS_INFO:
              flag = dtsHexServiceImpl._onPrimitiveProgressInfo(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.Progress);
              break;
            case DtsEventId.ePRIMITIVE_JOB_INFO:
              flag = dtsHexServiceImpl._onPrimitiveJobInfo(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.JobInfo);
              break;
            case DtsEventId.ePRIMITIVE_REPETION_STOPPED:
              flag = dtsHexServiceImpl._onPrimitiveRepetitionStopped(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_TERMINATED:
              flag = dtsHexServiceImpl._onPrimitiveTerminated(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
              break;
          }
          break;
        case MCDObjectType.eMCDMULTIPLEECUJOB:
          DtsMultipleEcuJobImpl multipleEcuJobImpl = (DtsMultipleEcuJobImpl) mcdObject;
          switch (dtsEvent.EventId)
          {
            case DtsEventId.ePRIMITIVE_BUFFER_OVERFLOW:
              flag = multipleEcuJobImpl._onPrimitiveBufferOverflow(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_CANCELED_DURING_EXECUTION:
              flag = multipleEcuJobImpl._onPrimitiveCanceledDuringExecution(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_CANCELED_FROM_QUEUE:
              flag = multipleEcuJobImpl._onPrimitiveCanceledFromQueue(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_ERROR:
              flag = multipleEcuJobImpl._onPrimitiveError(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.Error);
              break;
            case DtsEventId.ePRIMITIVE_HAS_INTERMEDIATE_RESULT:
              flag = multipleEcuJobImpl._onPrimitiveHasIntermediateResult(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
              break;
            case DtsEventId.ePRIMITIVE_HAS_RESULT:
              flag = multipleEcuJobImpl._onPrimitiveHasResult(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
              break;
            case DtsEventId.ePRIMITIVE_PROGRESS_INFO:
              flag = multipleEcuJobImpl._onPrimitiveProgressInfo(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.Progress);
              break;
            case DtsEventId.ePRIMITIVE_JOB_INFO:
              flag = multipleEcuJobImpl._onPrimitiveJobInfo(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.JobInfo);
              break;
            case DtsEventId.ePRIMITIVE_REPETION_STOPPED:
              flag = multipleEcuJobImpl._onPrimitiveRepetitionStopped(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_TERMINATED:
              flag = multipleEcuJobImpl._onPrimitiveTerminated(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
              break;
          }
          break;
        case MCDObjectType.eMCDPROTOCOLPARAMETERSET:
          DtsProtocolParameterSetImpl parameterSetImpl = (DtsProtocolParameterSetImpl) mcdObject;
          switch (dtsEvent.EventId)
          {
            case DtsEventId.ePRIMITIVE_BUFFER_OVERFLOW:
              flag = parameterSetImpl._onPrimitiveBufferOverflow(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_CANCELED_DURING_EXECUTION:
              flag = parameterSetImpl._onPrimitiveCanceledDuringExecution(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_CANCELED_FROM_QUEUE:
              flag = parameterSetImpl._onPrimitiveCanceledFromQueue(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_ERROR:
              flag = parameterSetImpl._onPrimitiveError(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.Error);
              break;
            case DtsEventId.ePRIMITIVE_HAS_INTERMEDIATE_RESULT:
              flag = parameterSetImpl._onPrimitiveHasIntermediateResult(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
              break;
            case DtsEventId.ePRIMITIVE_HAS_RESULT:
              flag = parameterSetImpl._onPrimitiveHasResult(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
              break;
            case DtsEventId.ePRIMITIVE_PROGRESS_INFO:
              flag = parameterSetImpl._onPrimitiveProgressInfo(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.Progress);
              break;
            case DtsEventId.ePRIMITIVE_JOB_INFO:
              flag = parameterSetImpl._onPrimitiveJobInfo(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.JobInfo);
              break;
            case DtsEventId.ePRIMITIVE_REPETION_STOPPED:
              flag = parameterSetImpl._onPrimitiveRepetitionStopped(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_TERMINATED:
              flag = parameterSetImpl._onPrimitiveTerminated(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
              break;
          }
          break;
        case MCDObjectType.eMCDFLASHJOB:
          DtsFlashJobImpl dtsFlashJobImpl = (DtsFlashJobImpl) mcdObject;
          switch (dtsEvent.EventId)
          {
            case DtsEventId.ePRIMITIVE_BUFFER_OVERFLOW:
              flag = dtsFlashJobImpl._onPrimitiveBufferOverflow(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_CANCELED_DURING_EXECUTION:
              flag = dtsFlashJobImpl._onPrimitiveCanceledDuringExecution(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_CANCELED_FROM_QUEUE:
              flag = dtsFlashJobImpl._onPrimitiveCanceledFromQueue(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_ERROR:
              flag = dtsFlashJobImpl._onPrimitiveError(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.Error);
              break;
            case DtsEventId.ePRIMITIVE_HAS_INTERMEDIATE_RESULT:
              flag = dtsFlashJobImpl._onPrimitiveHasIntermediateResult(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
              break;
            case DtsEventId.ePRIMITIVE_HAS_RESULT:
              flag = dtsFlashJobImpl._onPrimitiveHasResult(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
              break;
            case DtsEventId.ePRIMITIVE_PROGRESS_INFO:
              flag = dtsFlashJobImpl._onPrimitiveProgressInfo(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.Progress);
              break;
            case DtsEventId.ePRIMITIVE_JOB_INFO:
              flag = dtsFlashJobImpl._onPrimitiveJobInfo(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.JobInfo);
              break;
            case DtsEventId.ePRIMITIVE_REPETION_STOPPED:
              flag = dtsFlashJobImpl._onPrimitiveRepetitionStopped(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_TERMINATED:
              flag = dtsFlashJobImpl._onPrimitiveTerminated(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
              break;
          }
          break;
        case MCDObjectType.eMCDSERVICE:
          DtsServiceImpl dtsServiceImpl = (DtsServiceImpl) mcdObject;
          switch (dtsEvent.EventId)
          {
            case DtsEventId.ePRIMITIVE_BUFFER_OVERFLOW:
              flag = dtsServiceImpl._onPrimitiveBufferOverflow(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_CANCELED_DURING_EXECUTION:
              flag = dtsServiceImpl._onPrimitiveCanceledDuringExecution(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_CANCELED_FROM_QUEUE:
              flag = dtsServiceImpl._onPrimitiveCanceledFromQueue(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_ERROR:
              flag = dtsServiceImpl._onPrimitiveError(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.Error);
              break;
            case DtsEventId.ePRIMITIVE_HAS_INTERMEDIATE_RESULT:
              flag = dtsServiceImpl._onPrimitiveHasIntermediateResult(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
              break;
            case DtsEventId.ePRIMITIVE_HAS_RESULT:
              flag = dtsServiceImpl._onPrimitiveHasResult(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
              break;
            case DtsEventId.ePRIMITIVE_PROGRESS_INFO:
              flag = dtsServiceImpl._onPrimitiveProgressInfo(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.Progress);
              break;
            case DtsEventId.ePRIMITIVE_JOB_INFO:
              flag = dtsServiceImpl._onPrimitiveJobInfo(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.JobInfo);
              break;
            case DtsEventId.ePRIMITIVE_REPETION_STOPPED:
              flag = dtsServiceImpl._onPrimitiveRepetitionStopped(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_TERMINATED:
              flag = dtsServiceImpl._onPrimitiveTerminated(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
              break;
          }
          break;
        case MCDObjectType.eMCDSINGLEECUJOB:
          DtsSingleEcuJobImpl singleEcuJobImpl = (DtsSingleEcuJobImpl) mcdObject;
          switch (dtsEvent.EventId)
          {
            case DtsEventId.ePRIMITIVE_BUFFER_OVERFLOW:
              flag = singleEcuJobImpl._onPrimitiveBufferOverflow(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_CANCELED_DURING_EXECUTION:
              flag = singleEcuJobImpl._onPrimitiveCanceledDuringExecution(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_CANCELED_FROM_QUEUE:
              flag = singleEcuJobImpl._onPrimitiveCanceledFromQueue(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_ERROR:
              flag = singleEcuJobImpl._onPrimitiveError(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.Error);
              break;
            case DtsEventId.ePRIMITIVE_HAS_INTERMEDIATE_RESULT:
              flag = singleEcuJobImpl._onPrimitiveHasIntermediateResult(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
              break;
            case DtsEventId.ePRIMITIVE_HAS_RESULT:
              flag = singleEcuJobImpl._onPrimitiveHasResult(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
              break;
            case DtsEventId.ePRIMITIVE_PROGRESS_INFO:
              flag = singleEcuJobImpl._onPrimitiveProgressInfo(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.Progress);
              break;
            case DtsEventId.ePRIMITIVE_JOB_INFO:
              flag = singleEcuJobImpl._onPrimitiveJobInfo(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.JobInfo);
              break;
            case DtsEventId.ePRIMITIVE_REPETION_STOPPED:
              flag = singleEcuJobImpl._onPrimitiveRepetitionStopped(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_TERMINATED:
              flag = singleEcuJobImpl._onPrimitiveTerminated(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
              break;
          }
          break;
        case MCDObjectType.eMCDSTARTCOMMUNICATION:
          DtsStartCommunicationImpl communicationImpl1 = (DtsStartCommunicationImpl) mcdObject;
          switch (dtsEvent.EventId)
          {
            case DtsEventId.ePRIMITIVE_BUFFER_OVERFLOW:
              flag = communicationImpl1._onPrimitiveBufferOverflow(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_CANCELED_DURING_EXECUTION:
              flag = communicationImpl1._onPrimitiveCanceledDuringExecution(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_CANCELED_FROM_QUEUE:
              flag = communicationImpl1._onPrimitiveCanceledFromQueue(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_ERROR:
              flag = communicationImpl1._onPrimitiveError(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.Error);
              break;
            case DtsEventId.ePRIMITIVE_HAS_INTERMEDIATE_RESULT:
              flag = communicationImpl1._onPrimitiveHasIntermediateResult(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
              break;
            case DtsEventId.ePRIMITIVE_HAS_RESULT:
              flag = communicationImpl1._onPrimitiveHasResult(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
              break;
            case DtsEventId.ePRIMITIVE_PROGRESS_INFO:
              flag = communicationImpl1._onPrimitiveProgressInfo(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.Progress);
              break;
            case DtsEventId.ePRIMITIVE_JOB_INFO:
              flag = communicationImpl1._onPrimitiveJobInfo(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.JobInfo);
              break;
            case DtsEventId.ePRIMITIVE_REPETION_STOPPED:
              flag = communicationImpl1._onPrimitiveRepetitionStopped(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_TERMINATED:
              flag = communicationImpl1._onPrimitiveTerminated(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
              break;
          }
          break;
        case MCDObjectType.eMCDSTOPCOMMUNICATION:
          DtsStopCommunicationImpl communicationImpl2 = (DtsStopCommunicationImpl) mcdObject;
          switch (dtsEvent.EventId)
          {
            case DtsEventId.ePRIMITIVE_BUFFER_OVERFLOW:
              flag = communicationImpl2._onPrimitiveBufferOverflow(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_CANCELED_DURING_EXECUTION:
              flag = communicationImpl2._onPrimitiveCanceledDuringExecution(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_CANCELED_FROM_QUEUE:
              flag = communicationImpl2._onPrimitiveCanceledFromQueue(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_ERROR:
              flag = communicationImpl2._onPrimitiveError(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.Error);
              break;
            case DtsEventId.ePRIMITIVE_HAS_INTERMEDIATE_RESULT:
              flag = communicationImpl2._onPrimitiveHasIntermediateResult(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
              break;
            case DtsEventId.ePRIMITIVE_HAS_RESULT:
              flag = communicationImpl2._onPrimitiveHasResult(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
              break;
            case DtsEventId.ePRIMITIVE_PROGRESS_INFO:
              flag = communicationImpl2._onPrimitiveProgressInfo(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.Progress);
              break;
            case DtsEventId.ePRIMITIVE_JOB_INFO:
              flag = communicationImpl2._onPrimitiveJobInfo(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.JobInfo);
              break;
            case DtsEventId.ePRIMITIVE_REPETION_STOPPED:
              flag = communicationImpl2._onPrimitiveRepetitionStopped(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_TERMINATED:
              flag = communicationImpl2._onPrimitiveTerminated(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
              break;
          }
          break;
        case MCDObjectType.eMCDVARIANTIDENTIFICATION:
          DtsVariantIdentificationImpl identificationImpl = (DtsVariantIdentificationImpl) mcdObject;
          switch (dtsEvent.EventId)
          {
            case DtsEventId.ePRIMITIVE_BUFFER_OVERFLOW:
              flag = identificationImpl._onPrimitiveBufferOverflow(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_CANCELED_DURING_EXECUTION:
              flag = identificationImpl._onPrimitiveCanceledDuringExecution(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_CANCELED_FROM_QUEUE:
              flag = identificationImpl._onPrimitiveCanceledFromQueue(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_ERROR:
              flag = identificationImpl._onPrimitiveError(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.Error);
              break;
            case DtsEventId.ePRIMITIVE_HAS_INTERMEDIATE_RESULT:
              flag = identificationImpl._onPrimitiveHasIntermediateResult(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
              break;
            case DtsEventId.ePRIMITIVE_HAS_RESULT:
              flag = identificationImpl._onPrimitiveHasResult(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
              break;
            case DtsEventId.ePRIMITIVE_PROGRESS_INFO:
              flag = identificationImpl._onPrimitiveProgressInfo(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.Progress);
              break;
            case DtsEventId.ePRIMITIVE_JOB_INFO:
              flag = identificationImpl._onPrimitiveJobInfo(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.JobInfo);
              break;
            case DtsEventId.ePRIMITIVE_REPETION_STOPPED:
              flag = identificationImpl._onPrimitiveRepetitionStopped(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_TERMINATED:
              flag = identificationImpl._onPrimitiveTerminated(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
              break;
          }
          break;
        case MCDObjectType.eMCDVARIANTIDENTIFICATIONANDSELECTION:
          DtsVariantIdentificationAndSelectionImpl andSelectionImpl = (DtsVariantIdentificationAndSelectionImpl) mcdObject;
          switch (dtsEvent.EventId)
          {
            case DtsEventId.ePRIMITIVE_BUFFER_OVERFLOW:
              flag = andSelectionImpl._onPrimitiveBufferOverflow(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_CANCELED_DURING_EXECUTION:
              flag = andSelectionImpl._onPrimitiveCanceledDuringExecution(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_CANCELED_FROM_QUEUE:
              flag = andSelectionImpl._onPrimitiveCanceledFromQueue(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_ERROR:
              flag = andSelectionImpl._onPrimitiveError(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.Error);
              break;
            case DtsEventId.ePRIMITIVE_HAS_INTERMEDIATE_RESULT:
              flag = andSelectionImpl._onPrimitiveHasIntermediateResult(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
              break;
            case DtsEventId.ePRIMITIVE_HAS_RESULT:
              flag = andSelectionImpl._onPrimitiveHasResult(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
              break;
            case DtsEventId.ePRIMITIVE_PROGRESS_INFO:
              flag = andSelectionImpl._onPrimitiveProgressInfo(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.Progress);
              break;
            case DtsEventId.ePRIMITIVE_JOB_INFO:
              flag = andSelectionImpl._onPrimitiveJobInfo(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.JobInfo);
              break;
            case DtsEventId.ePRIMITIVE_REPETION_STOPPED:
              flag = andSelectionImpl._onPrimitiveRepetitionStopped(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_TERMINATED:
              flag = andSelectionImpl._onPrimitiveTerminated(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
              break;
          }
          break;
        case MCDObjectType.eMCDDYNIDDEFINECOMPRIMITIVE:
          DtsDynIdDefineComPrimitiveImpl comPrimitiveImpl1 = (DtsDynIdDefineComPrimitiveImpl) mcdObject;
          switch (dtsEvent.EventId)
          {
            case DtsEventId.ePRIMITIVE_BUFFER_OVERFLOW:
              flag = comPrimitiveImpl1._onPrimitiveBufferOverflow(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_CANCELED_DURING_EXECUTION:
              flag = comPrimitiveImpl1._onPrimitiveCanceledDuringExecution(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_CANCELED_FROM_QUEUE:
              flag = comPrimitiveImpl1._onPrimitiveCanceledFromQueue(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_ERROR:
              flag = comPrimitiveImpl1._onPrimitiveError(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.Error);
              break;
            case DtsEventId.ePRIMITIVE_HAS_INTERMEDIATE_RESULT:
              flag = comPrimitiveImpl1._onPrimitiveHasIntermediateResult(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
              break;
            case DtsEventId.ePRIMITIVE_HAS_RESULT:
              flag = comPrimitiveImpl1._onPrimitiveHasResult(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
              break;
            case DtsEventId.ePRIMITIVE_PROGRESS_INFO:
              flag = comPrimitiveImpl1._onPrimitiveProgressInfo(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.Progress);
              break;
            case DtsEventId.ePRIMITIVE_JOB_INFO:
              flag = comPrimitiveImpl1._onPrimitiveJobInfo(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.JobInfo);
              break;
            case DtsEventId.ePRIMITIVE_REPETION_STOPPED:
              flag = comPrimitiveImpl1._onPrimitiveRepetitionStopped(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_TERMINATED:
              flag = comPrimitiveImpl1._onPrimitiveTerminated(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
              break;
          }
          break;
        case MCDObjectType.eMCDDYNIDREADCOMPRIMITIVE:
          DtsDynIdReadComPrimitiveImpl comPrimitiveImpl2 = (DtsDynIdReadComPrimitiveImpl) mcdObject;
          switch (dtsEvent.EventId)
          {
            case DtsEventId.ePRIMITIVE_BUFFER_OVERFLOW:
              flag = comPrimitiveImpl2._onPrimitiveBufferOverflow(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_CANCELED_DURING_EXECUTION:
              flag = comPrimitiveImpl2._onPrimitiveCanceledDuringExecution(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_CANCELED_FROM_QUEUE:
              flag = comPrimitiveImpl2._onPrimitiveCanceledFromQueue(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_ERROR:
              flag = comPrimitiveImpl2._onPrimitiveError(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.Error);
              break;
            case DtsEventId.ePRIMITIVE_HAS_INTERMEDIATE_RESULT:
              flag = comPrimitiveImpl2._onPrimitiveHasIntermediateResult(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
              break;
            case DtsEventId.ePRIMITIVE_HAS_RESULT:
              flag = comPrimitiveImpl2._onPrimitiveHasResult(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
              break;
            case DtsEventId.ePRIMITIVE_PROGRESS_INFO:
              flag = comPrimitiveImpl2._onPrimitiveProgressInfo(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.Progress);
              break;
            case DtsEventId.ePRIMITIVE_JOB_INFO:
              flag = comPrimitiveImpl2._onPrimitiveJobInfo(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.JobInfo);
              break;
            case DtsEventId.ePRIMITIVE_REPETION_STOPPED:
              flag = comPrimitiveImpl2._onPrimitiveRepetitionStopped(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_TERMINATED:
              flag = comPrimitiveImpl2._onPrimitiveTerminated(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
              break;
          }
          break;
        case MCDObjectType.eMCDDYNIDCLEARCOMPRIMITIVE:
          DtsDynIdClearComPrimitiveImpl comPrimitiveImpl3 = (DtsDynIdClearComPrimitiveImpl) mcdObject;
          switch (dtsEvent.EventId)
          {
            case DtsEventId.ePRIMITIVE_BUFFER_OVERFLOW:
              flag = comPrimitiveImpl3._onPrimitiveBufferOverflow(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_CANCELED_DURING_EXECUTION:
              flag = comPrimitiveImpl3._onPrimitiveCanceledDuringExecution(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_CANCELED_FROM_QUEUE:
              flag = comPrimitiveImpl3._onPrimitiveCanceledFromQueue(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_ERROR:
              flag = comPrimitiveImpl3._onPrimitiveError(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.Error);
              break;
            case DtsEventId.ePRIMITIVE_HAS_INTERMEDIATE_RESULT:
              flag = comPrimitiveImpl3._onPrimitiveHasIntermediateResult(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
              break;
            case DtsEventId.ePRIMITIVE_HAS_RESULT:
              flag = comPrimitiveImpl3._onPrimitiveHasResult(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
              break;
            case DtsEventId.ePRIMITIVE_PROGRESS_INFO:
              flag = comPrimitiveImpl3._onPrimitiveProgressInfo(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.Progress);
              break;
            case DtsEventId.ePRIMITIVE_JOB_INFO:
              flag = comPrimitiveImpl3._onPrimitiveJobInfo(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.JobInfo);
              break;
            case DtsEventId.ePRIMITIVE_REPETION_STOPPED:
              flag = comPrimitiveImpl3._onPrimitiveRepetitionStopped(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_TERMINATED:
              flag = comPrimitiveImpl3._onPrimitiveTerminated(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
              break;
          }
          break;
        case MCDObjectType.eMCDRAWSERVICE:
          DtsRawServiceImpl dtsRawServiceImpl = (DtsRawServiceImpl) mcdObject;
          switch (dtsEvent.EventId)
          {
            case DtsEventId.ePRIMITIVE_BUFFER_OVERFLOW:
              flag = dtsRawServiceImpl._onPrimitiveBufferOverflow(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_CANCELED_DURING_EXECUTION:
              flag = dtsRawServiceImpl._onPrimitiveCanceledDuringExecution(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_CANCELED_FROM_QUEUE:
              flag = dtsRawServiceImpl._onPrimitiveCanceledFromQueue(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_ERROR:
              flag = dtsRawServiceImpl._onPrimitiveError(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.Error);
              break;
            case DtsEventId.ePRIMITIVE_HAS_INTERMEDIATE_RESULT:
              flag = dtsRawServiceImpl._onPrimitiveHasIntermediateResult(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
              break;
            case DtsEventId.ePRIMITIVE_HAS_RESULT:
              flag = dtsRawServiceImpl._onPrimitiveHasResult(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
              break;
            case DtsEventId.ePRIMITIVE_PROGRESS_INFO:
              flag = dtsRawServiceImpl._onPrimitiveProgressInfo(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.Progress);
              break;
            case DtsEventId.ePRIMITIVE_JOB_INFO:
              flag = dtsRawServiceImpl._onPrimitiveJobInfo(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.JobInfo);
              break;
            case DtsEventId.ePRIMITIVE_REPETION_STOPPED:
              flag = dtsRawServiceImpl._onPrimitiveRepetitionStopped(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_TERMINATED:
              flag = dtsRawServiceImpl._onPrimitiveTerminated(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
              break;
          }
          break;
        case MCDObjectType.eMCDDELAY:
          DtsDelayImpl dtsDelayImpl = (DtsDelayImpl) mcdObject;
          switch (dtsEvent.EventId)
          {
            case DtsEventId.ePRIMITIVE_BUFFER_OVERFLOW:
              flag = dtsDelayImpl._onPrimitiveBufferOverflow(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_CANCELED_DURING_EXECUTION:
              flag = dtsDelayImpl._onPrimitiveCanceledDuringExecution(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_CANCELED_FROM_QUEUE:
              flag = dtsDelayImpl._onPrimitiveCanceledFromQueue(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_ERROR:
              flag = dtsDelayImpl._onPrimitiveError(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.Error);
              break;
            case DtsEventId.ePRIMITIVE_HAS_INTERMEDIATE_RESULT:
              flag = dtsDelayImpl._onPrimitiveHasIntermediateResult(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
              break;
            case DtsEventId.ePRIMITIVE_HAS_RESULT:
              flag = dtsDelayImpl._onPrimitiveHasResult(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
              break;
            case DtsEventId.ePRIMITIVE_PROGRESS_INFO:
              flag = dtsDelayImpl._onPrimitiveProgressInfo(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.Progress);
              break;
            case DtsEventId.ePRIMITIVE_JOB_INFO:
              flag = dtsDelayImpl._onPrimitiveJobInfo(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.JobInfo);
              break;
            case DtsEventId.ePRIMITIVE_REPETION_STOPPED:
              flag = dtsDelayImpl._onPrimitiveRepetitionStopped(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_TERMINATED:
              flag = dtsDelayImpl._onPrimitiveTerminated(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
              break;
          }
          break;
        case MCDObjectType.eMCDGOTOOFFLINE:
          DtsGotoOfflineImpl dtsGotoOfflineImpl = (DtsGotoOfflineImpl) mcdObject;
          switch (dtsEvent.EventId)
          {
            case DtsEventId.ePRIMITIVE_BUFFER_OVERFLOW:
              flag = dtsGotoOfflineImpl._onPrimitiveBufferOverflow(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_CANCELED_DURING_EXECUTION:
              flag = dtsGotoOfflineImpl._onPrimitiveCanceledDuringExecution(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_CANCELED_FROM_QUEUE:
              flag = dtsGotoOfflineImpl._onPrimitiveCanceledFromQueue(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_ERROR:
              flag = dtsGotoOfflineImpl._onPrimitiveError(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.Error);
              break;
            case DtsEventId.ePRIMITIVE_HAS_INTERMEDIATE_RESULT:
              flag = dtsGotoOfflineImpl._onPrimitiveHasIntermediateResult(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
              break;
            case DtsEventId.ePRIMITIVE_HAS_RESULT:
              flag = dtsGotoOfflineImpl._onPrimitiveHasResult(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
              break;
            case DtsEventId.ePRIMITIVE_PROGRESS_INFO:
              flag = dtsGotoOfflineImpl._onPrimitiveProgressInfo(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.Progress);
              break;
            case DtsEventId.ePRIMITIVE_JOB_INFO:
              flag = dtsGotoOfflineImpl._onPrimitiveJobInfo(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.JobInfo);
              break;
            case DtsEventId.ePRIMITIVE_REPETION_STOPPED:
              flag = dtsGotoOfflineImpl._onPrimitiveRepetitionStopped(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_TERMINATED:
              flag = dtsGotoOfflineImpl._onPrimitiveTerminated(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
              break;
          }
          break;
        case MCDObjectType.eMCDGOTOONLINE:
          DtsGotoOnlineImpl dtsGotoOnlineImpl = (DtsGotoOnlineImpl) mcdObject;
          switch (dtsEvent.EventId)
          {
            case DtsEventId.ePRIMITIVE_BUFFER_OVERFLOW:
              flag = dtsGotoOnlineImpl._onPrimitiveBufferOverflow(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_CANCELED_DURING_EXECUTION:
              flag = dtsGotoOnlineImpl._onPrimitiveCanceledDuringExecution(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_CANCELED_FROM_QUEUE:
              flag = dtsGotoOnlineImpl._onPrimitiveCanceledFromQueue(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_ERROR:
              flag = dtsGotoOnlineImpl._onPrimitiveError(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.Error);
              break;
            case DtsEventId.ePRIMITIVE_HAS_INTERMEDIATE_RESULT:
              flag = dtsGotoOnlineImpl._onPrimitiveHasIntermediateResult(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
              break;
            case DtsEventId.ePRIMITIVE_HAS_RESULT:
              flag = dtsGotoOnlineImpl._onPrimitiveHasResult(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
              break;
            case DtsEventId.ePRIMITIVE_PROGRESS_INFO:
              flag = dtsGotoOnlineImpl._onPrimitiveProgressInfo(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.Progress);
              break;
            case DtsEventId.ePRIMITIVE_JOB_INFO:
              flag = dtsGotoOnlineImpl._onPrimitiveJobInfo(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.JobInfo);
              break;
            case DtsEventId.ePRIMITIVE_REPETION_STOPPED:
              flag = dtsGotoOnlineImpl._onPrimitiveRepetitionStopped(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
              break;
            case DtsEventId.ePRIMITIVE_TERMINATED:
              flag = dtsGotoOnlineImpl._onPrimitiveTerminated(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
              break;
          }
          break;
        case MCDObjectType.eMCDCONFIGURATIONRECORDS:
          DtsConfigurationRecordsImpl configurationRecordsImpl = (DtsConfigurationRecordsImpl) mcdObject;
          if (dtsEvent.EventId == DtsEventId.eCONFIGURATION_RECORD_LOADED)
          {
            flag = configurationRecordsImpl._onConfigurationRecordLoaded(dtsEvent.ConfigurationRecord);
            break;
          }
          break;
      }
    }
    return flag;
  }
}
