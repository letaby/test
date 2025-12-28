using System;
using System.Collections;

namespace Softing.Dts;

internal class DTS_ObjectMapper
{
	private static onEvent m_eventCallback = null;

	private static bool m_initDone = false;

	private static Hashtable m_objectTable = new Hashtable();

	private static DTS_ObjectMapper m_instance = new DTS_ObjectMapper();

	internal DTS_ObjectMapper Instance => m_instance;

	~DTS_ObjectMapper()
	{
		CSWrap.CSNIDTS_goodbyDts();
	}

	internal static MCDException createException(IntPtr objectHandle)
	{
		return CSWrap.CSNIDTS_getObjectType(objectHandle) switch
		{
			MCDObjectType.eMCDCOMMUNICATIONEXCEPTION => new DtsCommunicationExceptionImpl(objectHandle), 
			MCDObjectType.eMCDEXCEPTION => new DtsExceptionImpl(objectHandle), 
			MCDObjectType.eMCDDATABASEEXCEPTION => new DtsDatabaseExceptionImpl(objectHandle), 
			MCDObjectType.eMCDSYSTEMEXCEPTION => new DtsSystemExceptionImpl(objectHandle), 
			MCDObjectType.eMCDPARAMETERIZATIONEXCEPTION => new DtsParameterizationExceptionImpl(objectHandle), 
			MCDObjectType.eMCDPROGRAMVIOLATIONEXCEPTION => new DtsProgramViolationExceptionImpl(objectHandle), 
			MCDObjectType.eMCDSHAREEXCEPTION => new DtsShareExceptionImpl(objectHandle), 
			_ => null, 
		};
	}

	internal static IntPtr getHandle(MappedObject objectInstance)
	{
		return objectInstance.Handle;
	}

	internal static MCDSystem getSystem()
	{
		registerCallbacks();
		return createObject(CSWrap.CSNIDTS_getSystem(), MCDObjectType.eMCDSYSTEM) as MCDSystem;
	}

	internal static MCDObject createObject(IntPtr objectHandle, MCDObjectType objectType)
	{
		lock (m_objectTable)
		{
			if (m_objectTable.ContainsKey(objectHandle))
			{
				object obj = m_objectTable[objectHandle];
				if (obj != null)
				{
					object target = ((WeakReference)obj).Target;
					if (target != null)
					{
						CSWrap.CSNIDTS_releaseObject(objectHandle);
						return target as MCDObject;
					}
				}
			}
			switch (objectType)
			{
			case MCDObjectType.eMCDACCESSKEY:
				return new DtsAccessKeyImpl(objectHandle);
			case MCDObjectType.eMCDACCESSKEYS:
				return new DtsAccessKeysImpl(objectHandle);
			case MCDObjectType.eMCDAUDIENCE:
				return new DtsAudienceImpl(objectHandle);
			case MCDObjectType.eMCDCANFILTER:
				return new DtsCanFilterImpl(objectHandle);
			case MCDObjectType.eMCDCANFILTERENTRIES:
				return new DtsCanFilterEntriesImpl(objectHandle);
			case MCDObjectType.eMCDCANFILTERENTRY:
				return new DtsCanFilterEntryImpl(objectHandle);
			case MCDObjectType.eMCDCANFILTERS:
				return new DtsCanFiltersImpl(objectHandle);
			case MCDObjectType.eMCDCLASSFACTORY:
				return new DtsClassFactoryImpl(objectHandle);
			case MCDObjectType.eMCDCOLLECTION:
				return new DtsCollectionImpl(objectHandle);
			case MCDObjectType.eMCDCOMPRIMITIVEEVENT:
				return new DtsComPrimitiveEventImpl(objectHandle);
			case MCDObjectType.eMCDCOMMUNICATIONEXCEPTION:
				return new DtsCommunicationExceptionImpl(objectHandle);
			case MCDObjectType.eMCDCONFIGURATIONIDITEM:
				return new DtsConfigurationIdItemImpl(objectHandle);
			case MCDObjectType.eMCDCONFIGURATIONRECORD:
				return new DtsConfigurationRecordImpl(objectHandle);
			case MCDObjectType.eMCDCONFIGURATIONRECORDS:
				return new DtsConfigurationRecordsImpl(objectHandle);
			case MCDObjectType.eMCDCONSTRAINT:
				return new DtsConstraintImpl(objectHandle);
			case MCDObjectType.eMCDDATAIDITEM:
				return new DtsDataIdItemImpl(objectHandle);
			case MCDObjectType.eMCDDATABASEEXCEPTION:
				return new DtsDatabaseExceptionImpl(objectHandle);
			case MCDObjectType.eMCDDATATYPECOLLECTION:
				return new DtsDatatypeCollectionImpl(objectHandle);
			case MCDObjectType.eMCDDBACCESSLEVEL:
				return new DtsDbAccessLevelImpl(objectHandle);
			case MCDObjectType.eMCDDBADDITIONALAUDIENCE:
				return new DtsDbAdditionalAudienceImpl(objectHandle);
			case MCDObjectType.eMCDDBADDITIONALAUDIENCES:
				return new DtsDbAdditionalAudiencesImpl(objectHandle);
			case MCDObjectType.eMCDDBCODEINFORMATION:
				return new DtsDbCodeInformationImpl(objectHandle);
			case MCDObjectType.eMCDDBCODEINFORMATIONS:
				return new DtsDbCodeInformationsImpl(objectHandle);
			case MCDObjectType.eMCDDBCODINGDATA:
				return new DtsDbCodingDataImpl(objectHandle);
			case MCDObjectType.eMCDDBCOMPONENTCONNECTOR:
				return new DtsDbComponentConnectorImpl(objectHandle);
			case MCDObjectType.eMCDDBCOMPONENTCONNECTORS:
				return new DtsDbComponentConnectorsImpl(objectHandle);
			case MCDObjectType.eMCDDBCOMPUMETHOD:
				return new DtsDbCompuMethodImpl(objectHandle);
			case MCDObjectType.eMCDDBCOMPUSCALE:
				return new DtsDbCompuScaleImpl(objectHandle);
			case MCDObjectType.eMCDDBCOMPUSCALES:
				return new DtsDbCompuScalesImpl(objectHandle);
			case MCDObjectType.eMCDDBCOMPUTATION:
				return new DtsDbComputationImpl(objectHandle);
			case MCDObjectType.eMCDDBCONFIGURATIONDATA:
				return new DtsDbConfigurationDataImpl(objectHandle);
			case MCDObjectType.eMCDDBCONFIGURATIONDATAS:
				return new DtsDbConfigurationDatasImpl(objectHandle);
			case MCDObjectType.eMCDDBCONFIGURATIONIDITEM:
				return new DtsDbConfigurationIdItemImpl(objectHandle);
			case MCDObjectType.eMCDDBCONFIGURATIONRECORD:
				return new DtsDbConfigurationRecordImpl(objectHandle);
			case MCDObjectType.eMCDDBCONFIGURATIONRECORDS:
				return new DtsDbConfigurationRecordsImpl(objectHandle);
			case MCDObjectType.eMCDDBCONTROLPRIMITIVES:
				return new DtsDbControlPrimitivesImpl(objectHandle);
			case MCDObjectType.eMCDDBDATAIDITEM:
				return new DtsDbDataIdItemImpl(objectHandle);
			case MCDObjectType.eMCDDBDATAOBJECTPROP:
				return new DtsDbDataObjectPropImpl(objectHandle);
			case MCDObjectType.eMCDDBDATAPRIMITIVES:
				return new DtsDbDataPrimitivesImpl(objectHandle);
			case MCDObjectType.eMCDDBDATARECORD:
				return new DtsDbDataRecordImpl(objectHandle);
			case MCDObjectType.eMCDDBDATARECORDS:
				return new DtsDbDataRecordsImpl(objectHandle);
			case MCDObjectType.eMCDDBDELAY:
				return new DtsDbDelayImpl(objectHandle);
			case MCDObjectType.eMCDDBDIAGCOMPRIMITIVES:
				return new DtsDbDiagComPrimitivesImpl(objectHandle);
			case MCDObjectType.eMCDDBDIAGOBJECTCONNECTOR:
				return new DtsDbDiagObjectConnectorImpl(objectHandle);
			case MCDObjectType.eMCDDBDIAGSERVICES:
				return new DtsDbDiagServicesImpl(objectHandle);
			case MCDObjectType.eMCDDBDIAGTROUBLECODE:
				return new DtsDbDiagTroubleCodeImpl(objectHandle);
			case MCDObjectType.eMCDDBDIAGTROUBLECODECONNECTOR:
				return new DtsDbDiagTroubleCodeConnectorImpl(objectHandle);
			case MCDObjectType.eMCDDBDIAGTROUBLECODECONNECTORS:
				return new DtsDbDiagTroubleCodeConnectorsImpl(objectHandle);
			case MCDObjectType.eMCDDBDIAGTROUBLECODES:
				return new DtsDbDiagTroubleCodesImpl(objectHandle);
			case MCDObjectType.eMCDDBDIAGVARIABLE:
				return new DtsDbDiagVariableImpl(objectHandle);
			case MCDObjectType.eMCDDBDIAGVARIABLES:
				return new DtsDbDiagVariablesImpl(objectHandle);
			case MCDObjectType.eMCDDBDYNIDCLEARCOMPRIMITIVE:
				return new DtsDbDynIdClearComPrimitiveImpl(objectHandle);
			case MCDObjectType.eMCDDBDYNIDDEFINECOMPRIMITIVE:
				return new DtsDbDynIdDefineComPrimitiveImpl(objectHandle);
			case MCDObjectType.eMCDDBDYNIDREADCOMPRIMITIVE:
				return new DtsDbDynIdReadComPrimitiveImpl(objectHandle);
			case MCDObjectType.eMCDDBECUBASEVARIANT:
				return new DtsDbEcuBaseVariantImpl(objectHandle);
			case MCDObjectType.eMCDDBECUBASEVARIANTS:
				return new DtsDbEcuBaseVariantsImpl(objectHandle);
			case MCDObjectType.eMCDDBECUMEM:
				return new DtsDbEcuMemImpl(objectHandle);
			case MCDObjectType.eMCDDBECUMEMS:
				return new DtsDbEcuMemsImpl(objectHandle);
			case MCDObjectType.eMCDDBECUSTATE:
				return new DtsDbEcuStateImpl(objectHandle);
			case MCDObjectType.eMCDDBECUSTATECHART:
				return new DtsDbEcuStateChartImpl(objectHandle);
			case MCDObjectType.eMCDDBECUSTATECHARTS:
				return new DtsDbEcuStateChartsImpl(objectHandle);
			case MCDObjectType.eMCDDBECUSTATETRANSITION:
				return new DtsDbEcuStateTransitionImpl(objectHandle);
			case MCDObjectType.eMCDDBECUSTATETRANSITIONACTION:
				return new DtsDbEcuStateTransitionActionImpl(objectHandle);
			case MCDObjectType.eMCDDBECUSTATETRANSITIONACTIONS:
				return new DtsDbEcuStateTransitionActionsImpl(objectHandle);
			case MCDObjectType.eMCDDBECUSTATETRANSITIONS:
				return new DtsDbEcuStateTransitionsImpl(objectHandle);
			case MCDObjectType.eMCDDBECUSTATES:
				return new DtsDbEcuStatesImpl(objectHandle);
			case MCDObjectType.eMCDDBECUVARIANT:
				return new DtsDbEcuVariantImpl(objectHandle);
			case MCDObjectType.eMCDDBECUVARIANTS:
				return new DtsDbEcuVariantsImpl(objectHandle);
			case MCDObjectType.eMCDDBENVDATACONNECTOR:
				return new DtsDbEnvDataConnectorImpl(objectHandle);
			case MCDObjectType.eMCDDBENVDATACONNECTORS:
				return new DtsDbEnvDataConnectorsImpl(objectHandle);
			case MCDObjectType.eMCDDBENVDATADESC:
				return new DtsDbEnvDataDescImpl(objectHandle);
			case MCDObjectType.eMCDDBENVDATADESCS:
				return new DtsDbEnvDataDescsImpl(objectHandle);
			case MCDObjectType.eMCDDBEXTERNALACCESSMETHOD:
				return new DtsDbExternalAccessMethodImpl(objectHandle);
			case MCDObjectType.eMCDDBFAULTMEMORIES:
				return new DtsDbFaultMemoriesImpl(objectHandle);
			case MCDObjectType.eMCDDBFAULTMEMORY:
				return new DtsDbFaultMemoryImpl(objectHandle);
			case MCDObjectType.eMCDDBFLASHCHECKSUM:
				return new DtsDbFlashChecksumImpl(objectHandle);
			case MCDObjectType.eMCDDBFLASHCHECKSUMS:
				return new DtsDbFlashChecksumsImpl(objectHandle);
			case MCDObjectType.eMCDDBFLASHDATA:
				return new DtsDbFlashDataImpl(objectHandle);
			case MCDObjectType.eMCDDBFLASHDATABLOCK:
				return new DtsDbFlashDataBlockImpl(objectHandle);
			case MCDObjectType.eMCDDBFLASHDATABLOCKS:
				return new DtsDbFlashDataBlocksImpl(objectHandle);
			case MCDObjectType.eMCDDBFLASHFILTER:
				return new DtsDbFlashFilterImpl(objectHandle);
			case MCDObjectType.eMCDDBFLASHFILTERS:
				return new DtsDbFlashFiltersImpl(objectHandle);
			case MCDObjectType.eMCDDBFLASHIDENT:
				return new DtsDbFlashIdentImpl(objectHandle);
			case MCDObjectType.eMCDDBFLASHIDENTS:
				return new DtsDbFlashIdentsImpl(objectHandle);
			case MCDObjectType.eMCDDBFLASHJOB:
				return new DtsDbFlashJobImpl(objectHandle);
			case MCDObjectType.eMCDDBFLASHSECURITIES:
				return new DtsDbFlashSecuritiesImpl(objectHandle);
			case MCDObjectType.eMCDDBFLASHSECURITY:
				return new DtsDbFlashSecurityImpl(objectHandle);
			case MCDObjectType.eMCDDBFLASHSEGMENT:
				return new DtsDbFlashSegmentImpl(objectHandle);
			case MCDObjectType.eMCDDBFLASHSEGMENTS:
				return new DtsDbFlashSegmentsImpl(objectHandle);
			case MCDObjectType.eMCDDBFLASHSESSION:
				return new DtsDbFlashSessionImpl(objectHandle);
			case MCDObjectType.eMCDDBFLASHSESSIONCLASS:
				return new DtsDbFlashSessionClassImpl(objectHandle);
			case MCDObjectType.eMCDDBFLASHSESSIONCLASSES:
				return new DtsDbFlashSessionClassesImpl(objectHandle);
			case MCDObjectType.eMCDDBFLASHSESSIONS:
				return new DtsDbFlashSessionsImpl(objectHandle);
			case MCDObjectType.eMCDDBFUNCTIONDIAGCOMCONNECTOR:
				return new DtsDbFunctionDiagComConnectorImpl(objectHandle);
			case MCDObjectType.eMCDDBFUNCTIONDIAGCOMCONNECTORS:
				return new DtsDbFunctionDiagComConnectorsImpl(objectHandle);
			case MCDObjectType.eMCDDBFUNCTIONDICTIONARIES:
				return new DtsDbFunctionDictionariesImpl(objectHandle);
			case MCDObjectType.eMCDDBFUNCTIONDICTIONARY:
				return new DtsDbFunctionDictionaryImpl(objectHandle);
			case MCDObjectType.eMCDDBFUNCTIONINPARAMETER:
				return new DtsDbFunctionInParameterImpl(objectHandle);
			case MCDObjectType.eMCDDBFUNCTIONINPARAMETERS:
				return new DtsDbFunctionInParametersImpl(objectHandle);
			case MCDObjectType.eMCDDBFUNCTIONNODE:
				return new DtsDbFunctionNodeImpl(objectHandle);
			case MCDObjectType.eMCDDBFUNCTIONNODEGROUP:
				return new DtsDbFunctionNodeGroupImpl(objectHandle);
			case MCDObjectType.eMCDDBFUNCTIONNODEGROUPS:
				return new DtsDbFunctionNodeGroupsImpl(objectHandle);
			case MCDObjectType.eMCDDBFUNCTIONNODES:
				return new DtsDbFunctionNodesImpl(objectHandle);
			case MCDObjectType.eMCDDBFUNCTIONOUTPARAMETER:
				return new DtsDbFunctionOutParameterImpl(objectHandle);
			case MCDObjectType.eMCDDBFUNCTIONOUTPARAMETERS:
				return new DtsDbFunctionOutParametersImpl(objectHandle);
			case MCDObjectType.eMCDDBFUNCTIONALCLASS:
				return new DtsDbFunctionalClassImpl(objectHandle);
			case MCDObjectType.eMCDDBFUNCTIONALCLASSES:
				return new DtsDbFunctionalClassesImpl(objectHandle);
			case MCDObjectType.eMCDDBFUNCTIONALGROUP:
				return new DtsDbFunctionalGroupImpl(objectHandle);
			case MCDObjectType.eMCDDBFUNCTIONALGROUPS:
				return new DtsDbFunctionalGroupsImpl(objectHandle);
			case MCDObjectType.eMCDDBGOTOOFFLINE:
				return new DtsDbGotoOfflineImpl(objectHandle);
			case MCDObjectType.eMCDDBGOTOONLINE:
				return new DtsDbGotoOnlineImpl(objectHandle);
			case MCDObjectType.eMCDDBHEXSERVICE:
				return new DtsDbHexServiceImpl(objectHandle);
			case MCDObjectType.eMCDDBIDENTDESCRIPTION:
				return new DtsDbIdentDescriptionImpl(objectHandle);
			case MCDObjectType.eMCDDBINTERFACECABLE:
				return new DtsDbInterfaceCableImpl(objectHandle);
			case MCDObjectType.eMCDDBINTERFACECABLES:
				return new DtsDbInterfaceCablesImpl(objectHandle);
			case MCDObjectType.eMCDDBINTERFACECONNECTORPIN:
				return new DtsDbInterfaceConnectorPinImpl(objectHandle);
			case MCDObjectType.eMCDDBINTERFACECONNECTORPINS:
				return new DtsDbInterfaceConnectorPinsImpl(objectHandle);
			case MCDObjectType.eMCDDBITEMVALUE:
				return new DtsDbItemValueImpl(objectHandle);
			case MCDObjectType.eMCDDBITEMVALUES:
				return new DtsDbItemValuesImpl(objectHandle);
			case MCDObjectType.eMCDDBJOBS:
				return new DtsDbJobsImpl(objectHandle);
			case MCDObjectType.eMCDDBLIMIT:
				return new DtsDbLimitImpl(objectHandle);
			case MCDObjectType.eMCDDBLOCATION:
				return new DtsDbLocationImpl(objectHandle);
			case MCDObjectType.eMCDDBLOCATIONS:
				return new DtsDbLocationsImpl(objectHandle);
			case MCDObjectType.eMCDDBLOGICALLINK:
				return new DtsDbLogicalLinkImpl(objectHandle);
			case MCDObjectType.eMCDDBLOGICALLINKS:
				return new DtsDbLogicalLinksImpl(objectHandle);
			case MCDObjectType.eMCDDBMATCHINGPARAMETER:
				return new DtsDbMatchingParameterImpl(objectHandle);
			case MCDObjectType.eMCDDBMATCHINGPARAMETERS:
				return new DtsDbMatchingParametersImpl(objectHandle);
			case MCDObjectType.eMCDDBMATCHINGPATTERN:
				return new DtsDbMatchingPatternImpl(objectHandle);
			case MCDObjectType.eMCDDBMATCHINGPATTERNS:
				return new DtsDbMatchingPatternsImpl(objectHandle);
			case MCDObjectType.eMCDDBMULTIPLEECUJOB:
				return new DtsDbMultipleEcuJobImpl(objectHandle);
			case MCDObjectType.eMCDDBODXFILE:
				return new DtsDbODXFileImpl(objectHandle);
			case MCDObjectType.eMCDDBODXFILES:
				return new DtsDbODXFilesImpl(objectHandle);
			case MCDObjectType.eMCDDBOPTIONITEM:
				return new DtsDbOptionItemImpl(objectHandle);
			case MCDObjectType.eMCDDBOPTIONITEMS:
				return new DtsDbOptionItemsImpl(objectHandle);
			case MCDObjectType.eMCDDBPARAMETERS:
				return new DtsDbParametersImpl(objectHandle);
			case MCDObjectType.eMCDDBPHYSICALDIMENSION:
				return new DtsDbPhysicalDimensionImpl(objectHandle);
			case MCDObjectType.eMCDDBPHYSICALMEMORIES:
				return new DtsDbPhysicalMemoriesImpl(objectHandle);
			case MCDObjectType.eMCDDBPHYSICALMEMORY:
				return new DtsDbPhysicalMemoryImpl(objectHandle);
			case MCDObjectType.eMCDDBPHYSICALSEGMENT:
				return new DtsDbPhysicalSegmentImpl(objectHandle);
			case MCDObjectType.eMCDDBPHYSICALSEGMENTS:
				return new DtsDbPhysicalSegmentsImpl(objectHandle);
			case MCDObjectType.eMCDDBPHYSICALVEHICLELINK:
				return new DtsDbPhysicalVehicleLinkImpl(objectHandle);
			case MCDObjectType.eMCDDBPHYSICALVEHICLELINKORINTERFACE:
				return new DtsDbPhysicalVehicleLinkOrInterfaceImpl(objectHandle);
			case MCDObjectType.eMCDDBPHYSICALVEHICLELINKORINTERFACES:
				return new DtsDbPhysicalVehicleLinkOrInterfacesImpl(objectHandle);
			case MCDObjectType.eMCDDBPRECONDITIONDEFINITION:
				return new DtsDbPreconditionDefinitionImpl(objectHandle);
			case MCDObjectType.eMCDDBPRECONDITIONDEFINITIONS:
				return new DtsDbPreconditionDefinitionsImpl(objectHandle);
			case MCDObjectType.eMCDDBPROJECT:
				return new DtsDbProjectImpl(objectHandle);
			case MCDObjectType.eMCDDBPROJECTCONFIGURATION:
				return new DtsDbProjectConfigurationImpl(objectHandle);
			case MCDObjectType.eMCDDBPROJECTDESCRIPTION:
				return new DtsDbProjectDescriptionImpl(objectHandle);
			case MCDObjectType.eMCDDBPROJECTDESCRIPTIONS:
				return new DtsDbProjectDescriptionsImpl(objectHandle);
			case MCDObjectType.eMCDDBPROTOCOLPARAMETER:
				return new DtsDbProtocolParameterImpl(objectHandle);
			case MCDObjectType.eMCDDBPROTOCOLPARAMETERSET:
				return new DtsDbProtocolParameterSetImpl(objectHandle);
			case MCDObjectType.eMCDDBPROTOCOLPARAMETERS:
				return new DtsDbProtocolParametersImpl(objectHandle);
			case MCDObjectType.eMCDDBRAWSERVICE:
				return new DtsDbRawServiceImpl(objectHandle);
			case MCDObjectType.eMCDDBREQUEST:
				return new DtsDbRequestImpl(objectHandle);
			case MCDObjectType.eMCDDBREQUESTPARAMETER:
				return new DtsDbRequestParameterImpl(objectHandle);
			case MCDObjectType.eMCDDBREQUESTPARAMETERS:
				return new DtsDbRequestParametersImpl(objectHandle);
			case MCDObjectType.eMCDDBRESPONSE:
				return new DtsDbResponseImpl(objectHandle);
			case MCDObjectType.eMCDDBRESPONSEPARAMETER:
				return new DtsDbResponseParameterImpl(objectHandle);
			case MCDObjectType.eMCDDBRESPONSEPARAMETERS:
				return new DtsDbResponseParametersImpl(objectHandle);
			case MCDObjectType.eMCDDBRESPONSES:
				return new DtsDbResponsesImpl(objectHandle);
			case MCDObjectType.eMCDDBSERVICE:
				return new DtsDbServiceImpl(objectHandle);
			case MCDObjectType.eMCDDBSERVICES:
				return new DtsDbServicesImpl(objectHandle);
			case MCDObjectType.eMCDDBSINGLEECUJOB:
				return new DtsDbSingleEcuJobImpl(objectHandle);
			case MCDObjectType.eMCDDBSPECIALDATAELEMENT:
				return new DtsDbSpecialDataElementImpl(objectHandle);
			case MCDObjectType.eMCDDBSPECIALDATAGROUP:
				return new DtsDbSpecialDataGroupImpl(objectHandle);
			case MCDObjectType.eMCDDBSPECIALDATAGROUPCAPTION:
				return new DtsDbSpecialDataGroupCaptionImpl(objectHandle);
			case MCDObjectType.eMCDDBSPECIALDATAGROUPS:
				return new DtsDbSpecialDataGroupsImpl(objectHandle);
			case MCDObjectType.eMCDDBSTARTCOMMUNICATION:
				return new DtsDbStartCommunicationImpl(objectHandle);
			case MCDObjectType.eMCDDBSTOPCOMMUNICATION:
				return new DtsDbStopCommunicationImpl(objectHandle);
			case MCDObjectType.eMCDDBSUBCOMPONENT:
				return new DtsDbSubComponentImpl(objectHandle);
			case MCDObjectType.eMCDDBSUBCOMPONENTPARAMCONNECTOR:
				return new DtsDbSubComponentParamConnectorImpl(objectHandle);
			case MCDObjectType.eMCDDBSUBCOMPONENTPARAMCONNECTORS:
				return new DtsDbSubComponentParamConnectorsImpl(objectHandle);
			case MCDObjectType.eMCDDBSUBCOMPONENTS:
				return new DtsDbSubComponentsImpl(objectHandle);
			case MCDObjectType.eMCDDBSYSTEMITEM:
				return new DtsDbSystemItemImpl(objectHandle);
			case MCDObjectType.eMCDDBSYSTEMITEMS:
				return new DtsDbSystemItemsImpl(objectHandle);
			case MCDObjectType.eMCDDBTABLE:
				return new DtsDbTableImpl(objectHandle);
			case MCDObjectType.eMCDDBTABLEPARAMETER:
				return new DtsDbTableParameterImpl(objectHandle);
			case MCDObjectType.eMCDDBTABLEPARAMETERS:
				return new DtsDbTableParametersImpl(objectHandle);
			case MCDObjectType.eMCDDBTABLEROWCONNECTOR:
				return new DtsDbTableRowConnectorImpl(objectHandle);
			case MCDObjectType.eMCDDBTABLEROWCONNECTORS:
				return new DtsDbTableRowConnectorsImpl(objectHandle);
			case MCDObjectType.eMCDDBTABLES:
				return new DtsDbTablesImpl(objectHandle);
			case MCDObjectType.eMCDDBUNIT:
				return new DtsDbUnitImpl(objectHandle);
			case MCDObjectType.eMCDDBUNITGROUP:
				return new DtsDbUnitGroupImpl(objectHandle);
			case MCDObjectType.eMCDDBUNITGROUPS:
				return new DtsDbUnitGroupsImpl(objectHandle);
			case MCDObjectType.eMCDDBUNITS:
				return new DtsDbUnitsImpl(objectHandle);
			case MCDObjectType.eMCDDBVARIANTCODINGDOMAIN:
				return new DtsDbVariantCodingDomainImpl(objectHandle);
			case MCDObjectType.eMCDDBVARIANTCODINGDOMAINS:
				return new DtsDbVariantCodingDomainsImpl(objectHandle);
			case MCDObjectType.eMCDDBVARIANTCODINGFRAGMENT:
				return new DtsDbVariantCodingFragmentImpl(objectHandle);
			case MCDObjectType.eMCDDBVARIANTCODINGFRAGMENTS:
				return new DtsDbVariantCodingFragmentsImpl(objectHandle);
			case MCDObjectType.eMCDDBVARIANTCODINGSTRING:
				return new DtsDbVariantCodingStringImpl(objectHandle);
			case MCDObjectType.eMCDDBVARIANTCODINGSTRINGS:
				return new DtsDbVariantCodingStringsImpl(objectHandle);
			case MCDObjectType.eMCDDBVARIANTIDENTIFICATION:
				return new DtsDbVariantIdentificationImpl(objectHandle);
			case MCDObjectType.eMCDDBVARIANTIDENTIFICATIONANDSELECTION:
				return new DtsDbVariantIdentificationAndSelectionImpl(objectHandle);
			case MCDObjectType.eMCDDBVEHICLECONNECTOR:
				return new DtsDbVehicleConnectorImpl(objectHandle);
			case MCDObjectType.eMCDDBVEHICLECONNECTORPIN:
				return new DtsDbVehicleConnectorPinImpl(objectHandle);
			case MCDObjectType.eMCDDBVEHICLECONNECTORPINS:
				return new DtsDbVehicleConnectorPinsImpl(objectHandle);
			case MCDObjectType.eMCDDBVEHICLECONNECTORS:
				return new DtsDbVehicleConnectorsImpl(objectHandle);
			case MCDObjectType.eMCDDBVEHICLEINFORMATION:
				return new DtsDbVehicleInformationImpl(objectHandle);
			case MCDObjectType.eMCDDBVEHICLEINFORMATIONS:
				return new DtsDbVehicleInformationsImpl(objectHandle);
			case MCDObjectType.eMCDDELAY:
				return new DtsDelayImpl(objectHandle);
			case MCDObjectType.eMCDDIAGCOMPRIMITIVES:
				return new DtsDiagComPrimitivesImpl(objectHandle);
			case MCDObjectType.eMCDDOIPMONITORLINK:
				return new DtsDoIPMonitorLinkImpl(objectHandle);
			case MCDObjectType.eMCDDYNIDCLEARCOMPRIMITIVE:
				return new DtsDynIdClearComPrimitiveImpl(objectHandle);
			case MCDObjectType.eMCDDYNIDDEFINECOMPRIMITIVE:
				return new DtsDynIdDefineComPrimitiveImpl(objectHandle);
			case MCDObjectType.eMCDDYNIDREADCOMPRIMITIVE:
				return new DtsDynIdReadComPrimitiveImpl(objectHandle);
			case MCDObjectType.eMCDENUMVALUE:
				return new DtsEnumValueImpl(objectHandle);
			case MCDObjectType.eMCDERROR:
				return new DtsErrorImpl(objectHandle);
			case MCDObjectType.eMCDERRORS:
				return new DtsErrorsImpl(objectHandle);
			case MCDObjectType.eMCDEXCEPTION:
				return new DtsExceptionImpl(objectHandle);
			case MCDObjectType.eMCDFILELOCATION:
				return new DtsFileLocationImpl(objectHandle);
			case MCDObjectType.eMCDFILELOCATIONS:
				return new DtsFileLocationsImpl(objectHandle);
			case MCDObjectType.eMCDFLASHJOB:
				return new DtsFlashJobImpl(objectHandle);
			case MCDObjectType.eMCDFLASHSEGMENTITERATOR:
				return new DtsFlashSegmentIteratorImpl(objectHandle);
			case MCDObjectType.eMCDGLOBALPROTOCOLPARAMETER:
				return new DtsGlobalProtocolParameterImpl(objectHandle);
			case MCDObjectType.eMCDGLOBALPROTOCOLPARAMETERSET:
				return new DtsGlobalProtocolParameterSetImpl(objectHandle);
			case MCDObjectType.eMCDGLOBALPROTOCOLPARAMETERSETS:
				return new DtsGlobalProtocolParameterSetsImpl(objectHandle);
			case MCDObjectType.eMCDGLOBALPROTOCOLPARAMETERS:
				return new DtsGlobalProtocolParametersImpl(objectHandle);
			case MCDObjectType.eMCDGOTOOFFLINE:
				return new DtsGotoOfflineImpl(objectHandle);
			case MCDObjectType.eMCDGOTOONLINE:
				return new DtsGotoOnlineImpl(objectHandle);
			case MCDObjectType.eMCDHEXSERVICE:
				return new DtsHexServiceImpl(objectHandle);
			case MCDObjectType.eMCDIDENTIFIERINFO:
				return new DtsIdentifierInfoImpl(objectHandle);
			case MCDObjectType.eMCDIDENTIFIERINFOS:
				return new DtsIdentifierInfosImpl(objectHandle);
			case MCDObjectType.eMCDINTERFACE:
				return new DtsInterfaceImpl(objectHandle);
			case MCDObjectType.eMCDINTERFACECONFIG:
				return new DtsInterfaceConfigImpl(objectHandle);
			case MCDObjectType.eMCDINTERFACECONFIGS:
				return new DtsInterfaceConfigsImpl(objectHandle);
			case MCDObjectType.eMCDINTERFACEINFORMATION:
				return new DtsInterfaceInformationImpl(objectHandle);
			case MCDObjectType.eMCDINTERFACEINFORMATIONS:
				return new DtsInterfaceInformationsImpl(objectHandle);
			case MCDObjectType.eMCDINTERFACELINKCONFIG:
				return new DtsInterfaceLinkConfigImpl(objectHandle);
			case MCDObjectType.eMCDINTERFACELINKCONFIGS:
				return new DtsInterfaceLinkConfigsImpl(objectHandle);
			case MCDObjectType.eMCDINTERFACELINKINFORMATION:
				return new DtsInterfaceLinkInformationImpl(objectHandle);
			case MCDObjectType.eMCDINTERFACELINKINFORMATIONS:
				return new DtsInterfaceLinkInformationsImpl(objectHandle);
			case MCDObjectType.eMCDINTERFACERESOURCE:
				return new DtsInterfaceResourceImpl(objectHandle);
			case MCDObjectType.eMCDINTERFACERESOURCES:
				return new DtsInterfaceResourcesImpl(objectHandle);
			case MCDObjectType.eMCDINTERFACES:
				return new DtsInterfacesImpl(objectHandle);
			case MCDObjectType.eMCDINTERVAL:
				return new DtsIntervalImpl(objectHandle);
			case MCDObjectType.eMCDINTERVALS:
				return new DtsIntervalsImpl(objectHandle);
			case MCDObjectType.eMCDJAVACONFIG:
				return new DtsJavaConfigImpl(objectHandle);
			case MCDObjectType.eMCDLICENSEINFO:
				return new DtsLicenseInfoImpl(objectHandle);
			case MCDObjectType.eMCDLICENSEINFOS:
				return new DtsLicenseInfosImpl(objectHandle);
			case MCDObjectType.eMCDLINKMAPPING:
				return new DtsLinkMappingImpl(objectHandle);
			case MCDObjectType.eMCDLINKMAPPINGS:
				return new DtsLinkMappingsImpl(objectHandle);
			case MCDObjectType.eMCDLOGICALLINK:
				return new DtsLogicalLinkImpl(objectHandle);
			case MCDObjectType.eMCDLOGICALLINKCONFIG:
				return new DtsLogicalLinkConfigImpl(objectHandle);
			case MCDObjectType.eMCDLOGICALLINKCONFIGS:
				return new DtsLogicalLinkConfigsImpl(objectHandle);
			case MCDObjectType.eMCDLOGICALLINKEVENT:
				return new DtsLogicalLinkEventImpl(objectHandle);
			case MCDObjectType.eMCDLOGICALLINKFILTERCONFIG:
				return new DtsLogicalLinkFilterConfigImpl(objectHandle);
			case MCDObjectType.eMCDLOGICALLINKFILTERCONFIGS:
				return new DtsLogicalLinkFilterConfigsImpl(objectHandle);
			case MCDObjectType.eMCDLOGICALLINKMONITOR:
				return new DtsLogicalLinkMonitorImpl(objectHandle);
			case MCDObjectType.eMCDMESSAGEFILTER:
				return new DtsMessageFilterImpl(objectHandle);
			case MCDObjectType.eMCDMESSAGEFILTERVALUES:
				return new DtsMessageFilterValuesImpl(objectHandle);
			case MCDObjectType.eMCDMESSAGEFILTERS:
				return new DtsMessageFiltersImpl(objectHandle);
			case MCDObjectType.eMCDMONITORLINK:
				return new DtsMonitorLinkImpl(objectHandle);
			case MCDObjectType.eMCDMONITORINGLINK:
				return new DtsMonitoringLinkImpl(objectHandle);
			case MCDObjectType.eMCDMULTIPLEECUJOB:
				return new DtsMultipleEcuJobImpl(objectHandle);
			case MCDObjectType.eMCDOFFLINEVARIANTCODING:
				return new DtsOfflineVariantCodingImpl(objectHandle);
			case MCDObjectType.eMCDOPTIONITEM:
				return new DtsOptionItemImpl(objectHandle);
			case MCDObjectType.eMCDOPTIONITEMS:
				return new DtsOptionItemsImpl(objectHandle);
			case MCDObjectType.eMCDPARAMETERMETAINFO:
				return new DtsParameterMetaInfoImpl(objectHandle);
			case MCDObjectType.eMCDPARAMETERIZATIONEXCEPTION:
				return new DtsParameterizationExceptionImpl(objectHandle);
			case MCDObjectType.eMCDPDUAPIINFORMATION:
				return new DtsPduApiInformationImpl(objectHandle);
			case MCDObjectType.eMCDPDUAPIINFORMATIONS:
				return new DtsPduApiInformationsImpl(objectHandle);
			case MCDObjectType.eMCDPHYSICALINTERFACELINK:
				return new DtsPhysicalInterfaceLinkImpl(objectHandle);
			case MCDObjectType.eMCDPROGRAMVIOLATIONEXCEPTION:
				return new DtsProgramViolationExceptionImpl(objectHandle);
			case MCDObjectType.eMCDPROJECT:
				return new DtsProjectImpl(objectHandle);
			case MCDObjectType.eMCDPROJECTCONFIG:
				return new DtsProjectConfigImpl(objectHandle);
			case MCDObjectType.eMCDPROJECTCONFIGS:
				return new DtsProjectConfigsImpl(objectHandle);
			case MCDObjectType.eMCDPROTOCOLPARAMETER:
				return new DtsProtocolParameterImpl(objectHandle);
			case MCDObjectType.eMCDPROTOCOLPARAMETERSET:
				return new DtsProtocolParameterSetImpl(objectHandle);
			case MCDObjectType.eMCDRAWSERVICE:
				return new DtsRawServiceImpl(objectHandle);
			case MCDObjectType.eMCDREADDIAGCOMPRIMITIVES:
				return new DtsReadDiagComPrimitivesImpl(objectHandle);
			case MCDObjectType.eMCDREQUEST:
				return new DtsRequestImpl(objectHandle);
			case MCDObjectType.eMCDREQUESTPARAMETER:
				return new DtsRequestParameterImpl(objectHandle);
			case MCDObjectType.eMCDREQUESTPARAMETERS:
				return new DtsRequestParametersImpl(objectHandle);
			case MCDObjectType.eMCDRESPONSE:
				return new DtsResponseImpl(objectHandle);
			case MCDObjectType.eMCDRESPONSEPARAMETER:
				return new DtsResponseParameterImpl(objectHandle);
			case MCDObjectType.eMCDRESPONSEPARAMETERS:
				return new DtsResponseParametersImpl(objectHandle);
			case MCDObjectType.eMCDRESPONSES:
				return new DtsResponsesImpl(objectHandle);
			case MCDObjectType.eMCDRESULT:
				return new DtsResultImpl(objectHandle);
			case MCDObjectType.eMCDRESULTSTATE:
				return new DtsResultStateImpl(objectHandle);
			case MCDObjectType.eMCDRESULTS:
				return new DtsResultsImpl(objectHandle);
			case MCDObjectType.eMCDSCALECONSTRAINT:
				return new DtsScaleConstraintImpl(objectHandle);
			case MCDObjectType.eMCDSCALECONSTRAINTS:
				return new DtsScaleConstraintsImpl(objectHandle);
			case MCDObjectType.eMCDSERVICE:
				return new DtsServiceImpl(objectHandle);
			case MCDObjectType.eMCDSHAREEXCEPTION:
				return new DtsShareExceptionImpl(objectHandle);
			case MCDObjectType.eMCDSINGLEECUJOB:
				return new DtsSingleEcuJobImpl(objectHandle);
			case MCDObjectType.eMCDSMARTCOMPRIMITIVE:
				return new DtsSmartComPrimitiveImpl(objectHandle);
			case MCDObjectType.eMCDSTARTCOMMUNICATION:
				return new DtsStartCommunicationImpl(objectHandle);
			case MCDObjectType.eMCDSTOPCOMMUNICATION:
				return new DtsStopCommunicationImpl(objectHandle);
			case MCDObjectType.eMCDSYSTEM:
				return new DtsSystemImpl(objectHandle);
			case MCDObjectType.eMCDSYSTEMCONFIG:
				return new DtsSystemConfigImpl(objectHandle);
			case MCDObjectType.eMCDSYSTEMEVENT:
				return new DtsSystemEventImpl(objectHandle);
			case MCDObjectType.eMCDSYSTEMEXCEPTION:
				return new DtsSystemExceptionImpl(objectHandle);
			case MCDObjectType.eMCDSYSTEMITEM:
				return new DtsSystemItemImpl(objectHandle);
			case MCDObjectType.eMCDSYSTEMITEMS:
				return new DtsSystemItemsImpl(objectHandle);
			case MCDObjectType.eMCDSYSTEMPROPERTIES:
				return new DtsSystemPropertiesImpl(objectHandle);
			case MCDObjectType.eMCDSYSTEMPROPERTY:
				return new DtsSystemPropertyImpl(objectHandle);
			case MCDObjectType.eMCDTEXTTABLEELEMENT:
				return new DtsTextTableElementImpl(objectHandle);
			case MCDObjectType.eMCDTEXTTABLEELEMENTS:
				return new DtsTextTableElementsImpl(objectHandle);
			case MCDObjectType.eMCDTRACECONFIG:
				return new DtsTraceConfigImpl(objectHandle);
			case MCDObjectType.eMCDVALUE:
				return new DtsValueImpl(objectHandle);
			case MCDObjectType.eMCDVALUES:
				return new DtsValuesImpl(objectHandle);
			case MCDObjectType.eMCDVARIANTIDENTIFICATION:
				return new DtsVariantIdentificationImpl(objectHandle);
			case MCDObjectType.eMCDVARIANTIDENTIFICATIONANDSELECTION:
				return new DtsVariantIdentificationAndSelectionImpl(objectHandle);
			case MCDObjectType.eMCDVEHICLEINFORMATIONCONFIG:
				return new DtsVehicleInformationConfigImpl(objectHandle);
			case MCDObjectType.eMCDVEHICLEINFORMATIONCONFIGS:
				return new DtsVehicleInformationConfigsImpl(objectHandle);
			case MCDObjectType.eMCDVERSION:
				return new DtsVersionImpl(objectHandle);
			case MCDObjectType.eMCDWLANSIGNALDATA:
				return new DtsWLanSignalDataImpl(objectHandle);
			case MCDObjectType.eMCDWRITEDIAGCOMPRIMITIVES:
				return new DtsWriteDiagComPrimitivesImpl(objectHandle);
			}
		}
		return null;
	}

	internal static void unregisterObject(IntPtr handle)
	{
		lock (m_objectTable)
		{
			object obj = m_objectTable[handle];
			if (obj != null && ((WeakReference)obj).Target == null)
			{
				m_objectTable.Remove(handle);
				CSWrap.CSNIDTS_releaseObject(handle);
			}
		}
	}

	internal static void registerObject(IntPtr handle, object newObject)
	{
		lock (m_objectTable)
		{
			if (m_objectTable.ContainsKey(handle))
			{
				m_objectTable.Remove(handle);
				m_objectTable.Add(handle, new WeakReference(newObject));
			}
			else
			{
				m_objectTable.Add(handle, new WeakReference(newObject));
			}
		}
	}

	internal static void registerCallbacks()
	{
		if (!m_initDone)
		{
			m_eventCallback = (onEvent)Delegate.Combine(m_eventCallback, new onEvent(handleEvent));
			CSWrap.CSNIDTS_registerCallback(m_eventCallback);
			m_initDone = true;
		}
	}

	internal static bool handleEvent(ref ObjectInfo_Struct targetObject, ref ObjectInfo_Struct eventObject)
	{
		bool result = false;
		MCDObject mCDObject = createObject(targetObject.m_handle, targetObject.m_type);
		DtsEvent dtsEvent = (DtsEvent)createObject(eventObject.m_handle, eventObject.m_type);
		if (mCDObject != null)
		{
			switch (mCDObject.ObjectType)
			{
			case MCDObjectType.eMCDCONFIGURATIONRECORDS:
			{
				DtsConfigurationRecordsImpl dtsConfigurationRecordsImpl = (DtsConfigurationRecordsImpl)mCDObject;
				DtsEventId eventId = dtsEvent.EventId;
				if (eventId == DtsEventId.eCONFIGURATION_RECORD_LOADED)
				{
					result = dtsConfigurationRecordsImpl._onConfigurationRecordLoaded(dtsEvent.ConfigurationRecord);
				}
				break;
			}
			case MCDObjectType.eMCDDYNIDCLEARCOMPRIMITIVE:
			{
				DtsDynIdClearComPrimitiveImpl dtsDynIdClearComPrimitiveImpl = (DtsDynIdClearComPrimitiveImpl)mCDObject;
				switch (dtsEvent.EventId)
				{
				case DtsEventId.ePRIMITIVE_BUFFER_OVERFLOW:
					result = dtsDynIdClearComPrimitiveImpl._onPrimitiveBufferOverflow(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
					break;
				case DtsEventId.ePRIMITIVE_CANCELED_DURING_EXECUTION:
					result = dtsDynIdClearComPrimitiveImpl._onPrimitiveCanceledDuringExecution(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
					break;
				case DtsEventId.ePRIMITIVE_CANCELED_FROM_QUEUE:
					result = dtsDynIdClearComPrimitiveImpl._onPrimitiveCanceledFromQueue(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
					break;
				case DtsEventId.ePRIMITIVE_ERROR:
					result = dtsDynIdClearComPrimitiveImpl._onPrimitiveError(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.Error);
					break;
				case DtsEventId.ePRIMITIVE_HAS_INTERMEDIATE_RESULT:
					result = dtsDynIdClearComPrimitiveImpl._onPrimitiveHasIntermediateResult(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
					break;
				case DtsEventId.ePRIMITIVE_HAS_RESULT:
					result = dtsDynIdClearComPrimitiveImpl._onPrimitiveHasResult(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
					break;
				case DtsEventId.ePRIMITIVE_JOB_INFO:
					result = dtsDynIdClearComPrimitiveImpl._onPrimitiveJobInfo(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.JobInfo);
					break;
				case DtsEventId.ePRIMITIVE_PROGRESS_INFO:
					result = dtsDynIdClearComPrimitiveImpl._onPrimitiveProgressInfo(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.Progress);
					break;
				case DtsEventId.ePRIMITIVE_REPETION_STOPPED:
					result = dtsDynIdClearComPrimitiveImpl._onPrimitiveRepetitionStopped(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
					break;
				case DtsEventId.ePRIMITIVE_TERMINATED:
					result = dtsDynIdClearComPrimitiveImpl._onPrimitiveTerminated(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
					break;
				}
				break;
			}
			case MCDObjectType.eMCDDYNIDDEFINECOMPRIMITIVE:
			{
				DtsDynIdDefineComPrimitiveImpl dtsDynIdDefineComPrimitiveImpl = (DtsDynIdDefineComPrimitiveImpl)mCDObject;
				switch (dtsEvent.EventId)
				{
				case DtsEventId.ePRIMITIVE_BUFFER_OVERFLOW:
					result = dtsDynIdDefineComPrimitiveImpl._onPrimitiveBufferOverflow(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
					break;
				case DtsEventId.ePRIMITIVE_CANCELED_DURING_EXECUTION:
					result = dtsDynIdDefineComPrimitiveImpl._onPrimitiveCanceledDuringExecution(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
					break;
				case DtsEventId.ePRIMITIVE_CANCELED_FROM_QUEUE:
					result = dtsDynIdDefineComPrimitiveImpl._onPrimitiveCanceledFromQueue(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
					break;
				case DtsEventId.ePRIMITIVE_ERROR:
					result = dtsDynIdDefineComPrimitiveImpl._onPrimitiveError(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.Error);
					break;
				case DtsEventId.ePRIMITIVE_HAS_INTERMEDIATE_RESULT:
					result = dtsDynIdDefineComPrimitiveImpl._onPrimitiveHasIntermediateResult(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
					break;
				case DtsEventId.ePRIMITIVE_HAS_RESULT:
					result = dtsDynIdDefineComPrimitiveImpl._onPrimitiveHasResult(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
					break;
				case DtsEventId.ePRIMITIVE_JOB_INFO:
					result = dtsDynIdDefineComPrimitiveImpl._onPrimitiveJobInfo(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.JobInfo);
					break;
				case DtsEventId.ePRIMITIVE_PROGRESS_INFO:
					result = dtsDynIdDefineComPrimitiveImpl._onPrimitiveProgressInfo(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.Progress);
					break;
				case DtsEventId.ePRIMITIVE_REPETION_STOPPED:
					result = dtsDynIdDefineComPrimitiveImpl._onPrimitiveRepetitionStopped(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
					break;
				case DtsEventId.ePRIMITIVE_TERMINATED:
					result = dtsDynIdDefineComPrimitiveImpl._onPrimitiveTerminated(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
					break;
				}
				break;
			}
			case MCDObjectType.eMCDDYNIDREADCOMPRIMITIVE:
			{
				DtsDynIdReadComPrimitiveImpl dtsDynIdReadComPrimitiveImpl = (DtsDynIdReadComPrimitiveImpl)mCDObject;
				switch (dtsEvent.EventId)
				{
				case DtsEventId.ePRIMITIVE_BUFFER_OVERFLOW:
					result = dtsDynIdReadComPrimitiveImpl._onPrimitiveBufferOverflow(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
					break;
				case DtsEventId.ePRIMITIVE_CANCELED_DURING_EXECUTION:
					result = dtsDynIdReadComPrimitiveImpl._onPrimitiveCanceledDuringExecution(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
					break;
				case DtsEventId.ePRIMITIVE_CANCELED_FROM_QUEUE:
					result = dtsDynIdReadComPrimitiveImpl._onPrimitiveCanceledFromQueue(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
					break;
				case DtsEventId.ePRIMITIVE_ERROR:
					result = dtsDynIdReadComPrimitiveImpl._onPrimitiveError(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.Error);
					break;
				case DtsEventId.ePRIMITIVE_HAS_INTERMEDIATE_RESULT:
					result = dtsDynIdReadComPrimitiveImpl._onPrimitiveHasIntermediateResult(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
					break;
				case DtsEventId.ePRIMITIVE_HAS_RESULT:
					result = dtsDynIdReadComPrimitiveImpl._onPrimitiveHasResult(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
					break;
				case DtsEventId.ePRIMITIVE_JOB_INFO:
					result = dtsDynIdReadComPrimitiveImpl._onPrimitiveJobInfo(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.JobInfo);
					break;
				case DtsEventId.ePRIMITIVE_PROGRESS_INFO:
					result = dtsDynIdReadComPrimitiveImpl._onPrimitiveProgressInfo(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.Progress);
					break;
				case DtsEventId.ePRIMITIVE_REPETION_STOPPED:
					result = dtsDynIdReadComPrimitiveImpl._onPrimitiveRepetitionStopped(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
					break;
				case DtsEventId.ePRIMITIVE_TERMINATED:
					result = dtsDynIdReadComPrimitiveImpl._onPrimitiveTerminated(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
					break;
				}
				break;
			}
			case MCDObjectType.eMCDFLASHJOB:
			{
				DtsFlashJobImpl dtsFlashJobImpl = (DtsFlashJobImpl)mCDObject;
				switch (dtsEvent.EventId)
				{
				case DtsEventId.ePRIMITIVE_BUFFER_OVERFLOW:
					result = dtsFlashJobImpl._onPrimitiveBufferOverflow(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
					break;
				case DtsEventId.ePRIMITIVE_CANCELED_DURING_EXECUTION:
					result = dtsFlashJobImpl._onPrimitiveCanceledDuringExecution(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
					break;
				case DtsEventId.ePRIMITIVE_CANCELED_FROM_QUEUE:
					result = dtsFlashJobImpl._onPrimitiveCanceledFromQueue(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
					break;
				case DtsEventId.ePRIMITIVE_ERROR:
					result = dtsFlashJobImpl._onPrimitiveError(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.Error);
					break;
				case DtsEventId.ePRIMITIVE_HAS_INTERMEDIATE_RESULT:
					result = dtsFlashJobImpl._onPrimitiveHasIntermediateResult(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
					break;
				case DtsEventId.ePRIMITIVE_HAS_RESULT:
					result = dtsFlashJobImpl._onPrimitiveHasResult(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
					break;
				case DtsEventId.ePRIMITIVE_JOB_INFO:
					result = dtsFlashJobImpl._onPrimitiveJobInfo(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.JobInfo);
					break;
				case DtsEventId.ePRIMITIVE_PROGRESS_INFO:
					result = dtsFlashJobImpl._onPrimitiveProgressInfo(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.Progress);
					break;
				case DtsEventId.ePRIMITIVE_REPETION_STOPPED:
					result = dtsFlashJobImpl._onPrimitiveRepetitionStopped(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
					break;
				case DtsEventId.ePRIMITIVE_TERMINATED:
					result = dtsFlashJobImpl._onPrimitiveTerminated(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
					break;
				}
				break;
			}
			case MCDObjectType.eMCDHEXSERVICE:
			{
				DtsHexServiceImpl dtsHexServiceImpl = (DtsHexServiceImpl)mCDObject;
				switch (dtsEvent.EventId)
				{
				case DtsEventId.ePRIMITIVE_BUFFER_OVERFLOW:
					result = dtsHexServiceImpl._onPrimitiveBufferOverflow(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
					break;
				case DtsEventId.ePRIMITIVE_CANCELED_DURING_EXECUTION:
					result = dtsHexServiceImpl._onPrimitiveCanceledDuringExecution(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
					break;
				case DtsEventId.ePRIMITIVE_CANCELED_FROM_QUEUE:
					result = dtsHexServiceImpl._onPrimitiveCanceledFromQueue(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
					break;
				case DtsEventId.ePRIMITIVE_ERROR:
					result = dtsHexServiceImpl._onPrimitiveError(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.Error);
					break;
				case DtsEventId.ePRIMITIVE_HAS_INTERMEDIATE_RESULT:
					result = dtsHexServiceImpl._onPrimitiveHasIntermediateResult(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
					break;
				case DtsEventId.ePRIMITIVE_HAS_RESULT:
					result = dtsHexServiceImpl._onPrimitiveHasResult(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
					break;
				case DtsEventId.ePRIMITIVE_JOB_INFO:
					result = dtsHexServiceImpl._onPrimitiveJobInfo(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.JobInfo);
					break;
				case DtsEventId.ePRIMITIVE_PROGRESS_INFO:
					result = dtsHexServiceImpl._onPrimitiveProgressInfo(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.Progress);
					break;
				case DtsEventId.ePRIMITIVE_REPETION_STOPPED:
					result = dtsHexServiceImpl._onPrimitiveRepetitionStopped(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
					break;
				case DtsEventId.ePRIMITIVE_TERMINATED:
					result = dtsHexServiceImpl._onPrimitiveTerminated(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
					break;
				}
				break;
			}
			case MCDObjectType.eMCDMULTIPLEECUJOB:
			{
				DtsMultipleEcuJobImpl dtsMultipleEcuJobImpl = (DtsMultipleEcuJobImpl)mCDObject;
				switch (dtsEvent.EventId)
				{
				case DtsEventId.ePRIMITIVE_BUFFER_OVERFLOW:
					result = dtsMultipleEcuJobImpl._onPrimitiveBufferOverflow(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
					break;
				case DtsEventId.ePRIMITIVE_CANCELED_DURING_EXECUTION:
					result = dtsMultipleEcuJobImpl._onPrimitiveCanceledDuringExecution(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
					break;
				case DtsEventId.ePRIMITIVE_CANCELED_FROM_QUEUE:
					result = dtsMultipleEcuJobImpl._onPrimitiveCanceledFromQueue(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
					break;
				case DtsEventId.ePRIMITIVE_ERROR:
					result = dtsMultipleEcuJobImpl._onPrimitiveError(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.Error);
					break;
				case DtsEventId.ePRIMITIVE_HAS_INTERMEDIATE_RESULT:
					result = dtsMultipleEcuJobImpl._onPrimitiveHasIntermediateResult(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
					break;
				case DtsEventId.ePRIMITIVE_HAS_RESULT:
					result = dtsMultipleEcuJobImpl._onPrimitiveHasResult(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
					break;
				case DtsEventId.ePRIMITIVE_JOB_INFO:
					result = dtsMultipleEcuJobImpl._onPrimitiveJobInfo(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.JobInfo);
					break;
				case DtsEventId.ePRIMITIVE_PROGRESS_INFO:
					result = dtsMultipleEcuJobImpl._onPrimitiveProgressInfo(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.Progress);
					break;
				case DtsEventId.ePRIMITIVE_REPETION_STOPPED:
					result = dtsMultipleEcuJobImpl._onPrimitiveRepetitionStopped(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
					break;
				case DtsEventId.ePRIMITIVE_TERMINATED:
					result = dtsMultipleEcuJobImpl._onPrimitiveTerminated(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
					break;
				}
				break;
			}
			case MCDObjectType.eMCDPROTOCOLPARAMETERSET:
			{
				DtsProtocolParameterSetImpl dtsProtocolParameterSetImpl = (DtsProtocolParameterSetImpl)mCDObject;
				switch (dtsEvent.EventId)
				{
				case DtsEventId.ePRIMITIVE_BUFFER_OVERFLOW:
					result = dtsProtocolParameterSetImpl._onPrimitiveBufferOverflow(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
					break;
				case DtsEventId.ePRIMITIVE_CANCELED_DURING_EXECUTION:
					result = dtsProtocolParameterSetImpl._onPrimitiveCanceledDuringExecution(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
					break;
				case DtsEventId.ePRIMITIVE_CANCELED_FROM_QUEUE:
					result = dtsProtocolParameterSetImpl._onPrimitiveCanceledFromQueue(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
					break;
				case DtsEventId.ePRIMITIVE_ERROR:
					result = dtsProtocolParameterSetImpl._onPrimitiveError(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.Error);
					break;
				case DtsEventId.ePRIMITIVE_HAS_INTERMEDIATE_RESULT:
					result = dtsProtocolParameterSetImpl._onPrimitiveHasIntermediateResult(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
					break;
				case DtsEventId.ePRIMITIVE_HAS_RESULT:
					result = dtsProtocolParameterSetImpl._onPrimitiveHasResult(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
					break;
				case DtsEventId.ePRIMITIVE_JOB_INFO:
					result = dtsProtocolParameterSetImpl._onPrimitiveJobInfo(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.JobInfo);
					break;
				case DtsEventId.ePRIMITIVE_PROGRESS_INFO:
					result = dtsProtocolParameterSetImpl._onPrimitiveProgressInfo(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.Progress);
					break;
				case DtsEventId.ePRIMITIVE_REPETION_STOPPED:
					result = dtsProtocolParameterSetImpl._onPrimitiveRepetitionStopped(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
					break;
				case DtsEventId.ePRIMITIVE_TERMINATED:
					result = dtsProtocolParameterSetImpl._onPrimitiveTerminated(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
					break;
				}
				break;
			}
			case MCDObjectType.eMCDSERVICE:
			{
				DtsServiceImpl dtsServiceImpl = (DtsServiceImpl)mCDObject;
				switch (dtsEvent.EventId)
				{
				case DtsEventId.ePRIMITIVE_BUFFER_OVERFLOW:
					result = dtsServiceImpl._onPrimitiveBufferOverflow(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
					break;
				case DtsEventId.ePRIMITIVE_CANCELED_DURING_EXECUTION:
					result = dtsServiceImpl._onPrimitiveCanceledDuringExecution(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
					break;
				case DtsEventId.ePRIMITIVE_CANCELED_FROM_QUEUE:
					result = dtsServiceImpl._onPrimitiveCanceledFromQueue(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
					break;
				case DtsEventId.ePRIMITIVE_ERROR:
					result = dtsServiceImpl._onPrimitiveError(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.Error);
					break;
				case DtsEventId.ePRIMITIVE_HAS_INTERMEDIATE_RESULT:
					result = dtsServiceImpl._onPrimitiveHasIntermediateResult(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
					break;
				case DtsEventId.ePRIMITIVE_HAS_RESULT:
					result = dtsServiceImpl._onPrimitiveHasResult(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
					break;
				case DtsEventId.ePRIMITIVE_JOB_INFO:
					result = dtsServiceImpl._onPrimitiveJobInfo(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.JobInfo);
					break;
				case DtsEventId.ePRIMITIVE_PROGRESS_INFO:
					result = dtsServiceImpl._onPrimitiveProgressInfo(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.Progress);
					break;
				case DtsEventId.ePRIMITIVE_REPETION_STOPPED:
					result = dtsServiceImpl._onPrimitiveRepetitionStopped(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
					break;
				case DtsEventId.ePRIMITIVE_TERMINATED:
					result = dtsServiceImpl._onPrimitiveTerminated(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
					break;
				}
				break;
			}
			case MCDObjectType.eMCDSINGLEECUJOB:
			{
				DtsSingleEcuJobImpl dtsSingleEcuJobImpl = (DtsSingleEcuJobImpl)mCDObject;
				switch (dtsEvent.EventId)
				{
				case DtsEventId.ePRIMITIVE_BUFFER_OVERFLOW:
					result = dtsSingleEcuJobImpl._onPrimitiveBufferOverflow(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
					break;
				case DtsEventId.ePRIMITIVE_CANCELED_DURING_EXECUTION:
					result = dtsSingleEcuJobImpl._onPrimitiveCanceledDuringExecution(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
					break;
				case DtsEventId.ePRIMITIVE_CANCELED_FROM_QUEUE:
					result = dtsSingleEcuJobImpl._onPrimitiveCanceledFromQueue(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
					break;
				case DtsEventId.ePRIMITIVE_ERROR:
					result = dtsSingleEcuJobImpl._onPrimitiveError(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.Error);
					break;
				case DtsEventId.ePRIMITIVE_HAS_INTERMEDIATE_RESULT:
					result = dtsSingleEcuJobImpl._onPrimitiveHasIntermediateResult(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
					break;
				case DtsEventId.ePRIMITIVE_HAS_RESULT:
					result = dtsSingleEcuJobImpl._onPrimitiveHasResult(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
					break;
				case DtsEventId.ePRIMITIVE_JOB_INFO:
					result = dtsSingleEcuJobImpl._onPrimitiveJobInfo(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.JobInfo);
					break;
				case DtsEventId.ePRIMITIVE_PROGRESS_INFO:
					result = dtsSingleEcuJobImpl._onPrimitiveProgressInfo(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.Progress);
					break;
				case DtsEventId.ePRIMITIVE_REPETION_STOPPED:
					result = dtsSingleEcuJobImpl._onPrimitiveRepetitionStopped(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
					break;
				case DtsEventId.ePRIMITIVE_TERMINATED:
					result = dtsSingleEcuJobImpl._onPrimitiveTerminated(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
					break;
				}
				break;
			}
			case MCDObjectType.eMCDSTARTCOMMUNICATION:
			{
				DtsStartCommunicationImpl dtsStartCommunicationImpl = (DtsStartCommunicationImpl)mCDObject;
				switch (dtsEvent.EventId)
				{
				case DtsEventId.ePRIMITIVE_BUFFER_OVERFLOW:
					result = dtsStartCommunicationImpl._onPrimitiveBufferOverflow(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
					break;
				case DtsEventId.ePRIMITIVE_CANCELED_DURING_EXECUTION:
					result = dtsStartCommunicationImpl._onPrimitiveCanceledDuringExecution(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
					break;
				case DtsEventId.ePRIMITIVE_CANCELED_FROM_QUEUE:
					result = dtsStartCommunicationImpl._onPrimitiveCanceledFromQueue(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
					break;
				case DtsEventId.ePRIMITIVE_ERROR:
					result = dtsStartCommunicationImpl._onPrimitiveError(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.Error);
					break;
				case DtsEventId.ePRIMITIVE_HAS_INTERMEDIATE_RESULT:
					result = dtsStartCommunicationImpl._onPrimitiveHasIntermediateResult(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
					break;
				case DtsEventId.ePRIMITIVE_HAS_RESULT:
					result = dtsStartCommunicationImpl._onPrimitiveHasResult(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
					break;
				case DtsEventId.ePRIMITIVE_JOB_INFO:
					result = dtsStartCommunicationImpl._onPrimitiveJobInfo(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.JobInfo);
					break;
				case DtsEventId.ePRIMITIVE_PROGRESS_INFO:
					result = dtsStartCommunicationImpl._onPrimitiveProgressInfo(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.Progress);
					break;
				case DtsEventId.ePRIMITIVE_REPETION_STOPPED:
					result = dtsStartCommunicationImpl._onPrimitiveRepetitionStopped(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
					break;
				case DtsEventId.ePRIMITIVE_TERMINATED:
					result = dtsStartCommunicationImpl._onPrimitiveTerminated(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
					break;
				}
				break;
			}
			case MCDObjectType.eMCDSTOPCOMMUNICATION:
			{
				DtsStopCommunicationImpl dtsStopCommunicationImpl = (DtsStopCommunicationImpl)mCDObject;
				switch (dtsEvent.EventId)
				{
				case DtsEventId.ePRIMITIVE_BUFFER_OVERFLOW:
					result = dtsStopCommunicationImpl._onPrimitiveBufferOverflow(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
					break;
				case DtsEventId.ePRIMITIVE_CANCELED_DURING_EXECUTION:
					result = dtsStopCommunicationImpl._onPrimitiveCanceledDuringExecution(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
					break;
				case DtsEventId.ePRIMITIVE_CANCELED_FROM_QUEUE:
					result = dtsStopCommunicationImpl._onPrimitiveCanceledFromQueue(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
					break;
				case DtsEventId.ePRIMITIVE_ERROR:
					result = dtsStopCommunicationImpl._onPrimitiveError(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.Error);
					break;
				case DtsEventId.ePRIMITIVE_HAS_INTERMEDIATE_RESULT:
					result = dtsStopCommunicationImpl._onPrimitiveHasIntermediateResult(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
					break;
				case DtsEventId.ePRIMITIVE_HAS_RESULT:
					result = dtsStopCommunicationImpl._onPrimitiveHasResult(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
					break;
				case DtsEventId.ePRIMITIVE_JOB_INFO:
					result = dtsStopCommunicationImpl._onPrimitiveJobInfo(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.JobInfo);
					break;
				case DtsEventId.ePRIMITIVE_PROGRESS_INFO:
					result = dtsStopCommunicationImpl._onPrimitiveProgressInfo(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.Progress);
					break;
				case DtsEventId.ePRIMITIVE_REPETION_STOPPED:
					result = dtsStopCommunicationImpl._onPrimitiveRepetitionStopped(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
					break;
				case DtsEventId.ePRIMITIVE_TERMINATED:
					result = dtsStopCommunicationImpl._onPrimitiveTerminated(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
					break;
				}
				break;
			}
			case MCDObjectType.eMCDVARIANTIDENTIFICATION:
			{
				DtsVariantIdentificationImpl dtsVariantIdentificationImpl = (DtsVariantIdentificationImpl)mCDObject;
				switch (dtsEvent.EventId)
				{
				case DtsEventId.ePRIMITIVE_BUFFER_OVERFLOW:
					result = dtsVariantIdentificationImpl._onPrimitiveBufferOverflow(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
					break;
				case DtsEventId.ePRIMITIVE_CANCELED_DURING_EXECUTION:
					result = dtsVariantIdentificationImpl._onPrimitiveCanceledDuringExecution(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
					break;
				case DtsEventId.ePRIMITIVE_CANCELED_FROM_QUEUE:
					result = dtsVariantIdentificationImpl._onPrimitiveCanceledFromQueue(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
					break;
				case DtsEventId.ePRIMITIVE_ERROR:
					result = dtsVariantIdentificationImpl._onPrimitiveError(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.Error);
					break;
				case DtsEventId.ePRIMITIVE_HAS_INTERMEDIATE_RESULT:
					result = dtsVariantIdentificationImpl._onPrimitiveHasIntermediateResult(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
					break;
				case DtsEventId.ePRIMITIVE_HAS_RESULT:
					result = dtsVariantIdentificationImpl._onPrimitiveHasResult(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
					break;
				case DtsEventId.ePRIMITIVE_JOB_INFO:
					result = dtsVariantIdentificationImpl._onPrimitiveJobInfo(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.JobInfo);
					break;
				case DtsEventId.ePRIMITIVE_PROGRESS_INFO:
					result = dtsVariantIdentificationImpl._onPrimitiveProgressInfo(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.Progress);
					break;
				case DtsEventId.ePRIMITIVE_REPETION_STOPPED:
					result = dtsVariantIdentificationImpl._onPrimitiveRepetitionStopped(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
					break;
				case DtsEventId.ePRIMITIVE_TERMINATED:
					result = dtsVariantIdentificationImpl._onPrimitiveTerminated(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
					break;
				}
				break;
			}
			case MCDObjectType.eMCDVARIANTIDENTIFICATIONANDSELECTION:
			{
				DtsVariantIdentificationAndSelectionImpl dtsVariantIdentificationAndSelectionImpl = (DtsVariantIdentificationAndSelectionImpl)mCDObject;
				switch (dtsEvent.EventId)
				{
				case DtsEventId.ePRIMITIVE_BUFFER_OVERFLOW:
					result = dtsVariantIdentificationAndSelectionImpl._onPrimitiveBufferOverflow(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
					break;
				case DtsEventId.ePRIMITIVE_CANCELED_DURING_EXECUTION:
					result = dtsVariantIdentificationAndSelectionImpl._onPrimitiveCanceledDuringExecution(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
					break;
				case DtsEventId.ePRIMITIVE_CANCELED_FROM_QUEUE:
					result = dtsVariantIdentificationAndSelectionImpl._onPrimitiveCanceledFromQueue(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
					break;
				case DtsEventId.ePRIMITIVE_ERROR:
					result = dtsVariantIdentificationAndSelectionImpl._onPrimitiveError(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.Error);
					break;
				case DtsEventId.ePRIMITIVE_HAS_INTERMEDIATE_RESULT:
					result = dtsVariantIdentificationAndSelectionImpl._onPrimitiveHasIntermediateResult(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
					break;
				case DtsEventId.ePRIMITIVE_HAS_RESULT:
					result = dtsVariantIdentificationAndSelectionImpl._onPrimitiveHasResult(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
					break;
				case DtsEventId.ePRIMITIVE_JOB_INFO:
					result = dtsVariantIdentificationAndSelectionImpl._onPrimitiveJobInfo(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.JobInfo);
					break;
				case DtsEventId.ePRIMITIVE_PROGRESS_INFO:
					result = dtsVariantIdentificationAndSelectionImpl._onPrimitiveProgressInfo(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.Progress);
					break;
				case DtsEventId.ePRIMITIVE_REPETION_STOPPED:
					result = dtsVariantIdentificationAndSelectionImpl._onPrimitiveRepetitionStopped(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
					break;
				case DtsEventId.ePRIMITIVE_TERMINATED:
					result = dtsVariantIdentificationAndSelectionImpl._onPrimitiveTerminated(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
					break;
				}
				break;
			}
			case MCDObjectType.eMCDLOGICALLINK:
			{
				DtsLogicalLinkImpl dtsLogicalLinkImpl = (DtsLogicalLinkImpl)mCDObject;
				switch (dtsEvent.EventId)
				{
				case DtsEventId.eLINK_DEFINABLE_DYNID_CHANGED:
					result = dtsLogicalLinkImpl._onDefinableDynIdListChanged(dtsEvent.DynIdList, dtsEvent.LogicalLink);
					break;
				case DtsEventId.eLINK_FINISHED:
					result = dtsLogicalLinkImpl._onLinkActivityStateIdle(dtsEvent.LogicalLink, dtsEvent.LogicalLinkState);
					break;
				case DtsEventId.eLINK_RESUMED:
					result = dtsLogicalLinkImpl._onLinkActivityStateRunning(dtsEvent.LogicalLink, dtsEvent.LogicalLinkState);
					break;
				case DtsEventId.eLINK_SUSPENDED:
					result = dtsLogicalLinkImpl._onLinkActivityStateSuspended(dtsEvent.LogicalLink, dtsEvent.LogicalLinkState);
					break;
				case DtsEventId.eLINK_ERROR:
					result = dtsLogicalLinkImpl._onLinkError(dtsEvent.LogicalLink, dtsEvent.Error);
					break;
				case DtsEventId.eLINK_LOCKED:
					result = dtsLogicalLinkImpl._onLinkLocked(dtsEvent.LogicalLink);
					break;
				case DtsEventId.eLINK_ONE_VARIANT_IDENTIFIED:
					result = dtsLogicalLinkImpl._onLinkOneVariantIdentified(dtsEvent.LogicalLink, dtsEvent.LogicalLinkState);
					break;
				case DtsEventId.eLINK_ONE_VARIANT_SELECTED:
					result = dtsLogicalLinkImpl._onLinkOneVariantSelected(dtsEvent.LogicalLink, dtsEvent.LogicalLinkState);
					break;
				case DtsEventId.eLINK_QUEUE_CLEARED:
					result = dtsLogicalLinkImpl._onLinkQueueCleared(dtsEvent.LogicalLink, dtsEvent.LogicalLinkState);
					break;
				case DtsEventId.eLINK_COMMUNICATION:
					result = dtsLogicalLinkImpl._onLinkStateCommunication(dtsEvent.LogicalLink);
					break;
				case DtsEventId.eLINK_CLOSED:
					result = dtsLogicalLinkImpl._onLinkStateCreated(dtsEvent.LogicalLink);
					break;
				case DtsEventId.eLINK_OPEN:
					result = dtsLogicalLinkImpl._onLinkStateOffline(dtsEvent.LogicalLink);
					break;
				case DtsEventId.eLINK_ONLINE:
					result = dtsLogicalLinkImpl._onLinkStateOnline(dtsEvent.LogicalLink);
					break;
				case DtsEventId.eLINK_UNLOCKED:
					result = dtsLogicalLinkImpl._onLinkUnlocked(dtsEvent.LogicalLink);
					break;
				case DtsEventId.eLINK_VARIANT_IDENTIFIED:
					result = dtsLogicalLinkImpl._onLinkVariantIdentified(dtsEvent.LogicalLink, dtsEvent.LogicalLinkState);
					break;
				case DtsEventId.eLINK_VARIANT_SELECTED:
					result = dtsLogicalLinkImpl._onLinkVariantSelected(dtsEvent.LogicalLink, dtsEvent.LogicalLinkState);
					break;
				case DtsEventId.eLINK_VARIANT_SET:
					result = dtsLogicalLinkImpl._onLinkVariantSet(dtsEvent.LogicalLink, dtsEvent.LogicalLinkState);
					break;
				case DtsEventId.ePRIMITIVE_BUFFER_OVERFLOW:
					result = dtsLogicalLinkImpl._onPrimitiveBufferOverflow(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
					break;
				case DtsEventId.ePRIMITIVE_CANCELED_DURING_EXECUTION:
					result = dtsLogicalLinkImpl._onPrimitiveCanceledDuringExecution(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
					break;
				case DtsEventId.ePRIMITIVE_CANCELED_FROM_QUEUE:
					result = dtsLogicalLinkImpl._onPrimitiveCanceledFromQueue(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
					break;
				case DtsEventId.ePRIMITIVE_ERROR:
					result = dtsLogicalLinkImpl._onPrimitiveError(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.Error);
					break;
				case DtsEventId.ePRIMITIVE_HAS_INTERMEDIATE_RESULT:
					result = dtsLogicalLinkImpl._onPrimitiveHasIntermediateResult(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
					break;
				case DtsEventId.ePRIMITIVE_HAS_RESULT:
					result = dtsLogicalLinkImpl._onPrimitiveHasResult(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
					break;
				case DtsEventId.ePRIMITIVE_JOB_INFO:
					result = dtsLogicalLinkImpl._onPrimitiveJobInfo(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.JobInfo);
					break;
				case DtsEventId.ePRIMITIVE_PROGRESS_INFO:
					result = dtsLogicalLinkImpl._onPrimitiveProgressInfo(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.Progress);
					break;
				case DtsEventId.ePRIMITIVE_REPETION_STOPPED:
					result = dtsLogicalLinkImpl._onPrimitiveRepetitionStopped(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
					break;
				case DtsEventId.ePRIMITIVE_TERMINATED:
					result = dtsLogicalLinkImpl._onPrimitiveTerminated(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
					break;
				}
				break;
			}
			case MCDObjectType.eMCDSYSTEM:
			{
				DtsSystemImpl dtsSystemImpl = (DtsSystemImpl)mCDObject;
				switch (dtsEvent.EventId)
				{
				case DtsEventId.eLINK_DEFINABLE_DYNID_CHANGED:
					result = dtsSystemImpl._onDefinableDynIdListChanged(dtsEvent.DynIdList, dtsEvent.LogicalLink);
					break;
				case DtsEventId.eLINK_FINISHED:
					result = dtsSystemImpl._onLinkActivityStateIdle(dtsEvent.LogicalLink, dtsEvent.LogicalLinkState);
					break;
				case DtsEventId.eLINK_RESUMED:
					result = dtsSystemImpl._onLinkActivityStateRunning(dtsEvent.LogicalLink, dtsEvent.LogicalLinkState);
					break;
				case DtsEventId.eLINK_SUSPENDED:
					result = dtsSystemImpl._onLinkActivityStateSuspended(dtsEvent.LogicalLink, dtsEvent.LogicalLinkState);
					break;
				case DtsEventId.eLINK_ERROR:
					result = dtsSystemImpl._onLinkError(dtsEvent.LogicalLink, dtsEvent.Error);
					break;
				case DtsEventId.eLINK_LOCKED:
					result = dtsSystemImpl._onLinkLocked(dtsEvent.LogicalLink);
					break;
				case DtsEventId.eLINK_ONE_VARIANT_IDENTIFIED:
					result = dtsSystemImpl._onLinkOneVariantIdentified(dtsEvent.LogicalLink, dtsEvent.LogicalLinkState);
					break;
				case DtsEventId.eLINK_ONE_VARIANT_SELECTED:
					result = dtsSystemImpl._onLinkOneVariantSelected(dtsEvent.LogicalLink, dtsEvent.LogicalLinkState);
					break;
				case DtsEventId.eLINK_QUEUE_CLEARED:
					result = dtsSystemImpl._onLinkQueueCleared(dtsEvent.LogicalLink, dtsEvent.LogicalLinkState);
					break;
				case DtsEventId.eLINK_COMMUNICATION:
					result = dtsSystemImpl._onLinkStateCommunication(dtsEvent.LogicalLink);
					break;
				case DtsEventId.eLINK_CLOSED:
					result = dtsSystemImpl._onLinkStateCreated(dtsEvent.LogicalLink);
					break;
				case DtsEventId.eLINK_OPEN:
					result = dtsSystemImpl._onLinkStateOffline(dtsEvent.LogicalLink);
					break;
				case DtsEventId.eLINK_ONLINE:
					result = dtsSystemImpl._onLinkStateOnline(dtsEvent.LogicalLink);
					break;
				case DtsEventId.eLINK_UNLOCKED:
					result = dtsSystemImpl._onLinkUnlocked(dtsEvent.LogicalLink);
					break;
				case DtsEventId.eLINK_VARIANT_IDENTIFIED:
					result = dtsSystemImpl._onLinkVariantIdentified(dtsEvent.LogicalLink, dtsEvent.LogicalLinkState);
					break;
				case DtsEventId.eLINK_VARIANT_SELECTED:
					result = dtsSystemImpl._onLinkVariantSelected(dtsEvent.LogicalLink, dtsEvent.LogicalLinkState);
					break;
				case DtsEventId.eLINK_VARIANT_SET:
					result = dtsSystemImpl._onLinkVariantSet(dtsEvent.LogicalLink, dtsEvent.LogicalLinkState);
					break;
				case DtsEventId.ePRIMITIVE_BUFFER_OVERFLOW:
					result = dtsSystemImpl._onPrimitiveBufferOverflow(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
					break;
				case DtsEventId.ePRIMITIVE_CANCELED_DURING_EXECUTION:
					result = dtsSystemImpl._onPrimitiveCanceledDuringExecution(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
					break;
				case DtsEventId.ePRIMITIVE_CANCELED_FROM_QUEUE:
					result = dtsSystemImpl._onPrimitiveCanceledFromQueue(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
					break;
				case DtsEventId.ePRIMITIVE_ERROR:
					result = dtsSystemImpl._onPrimitiveError(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.Error);
					break;
				case DtsEventId.ePRIMITIVE_HAS_INTERMEDIATE_RESULT:
					result = dtsSystemImpl._onPrimitiveHasIntermediateResult(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
					break;
				case DtsEventId.ePRIMITIVE_HAS_RESULT:
					result = dtsSystemImpl._onPrimitiveHasResult(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
					break;
				case DtsEventId.ePRIMITIVE_JOB_INFO:
					result = dtsSystemImpl._onPrimitiveJobInfo(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.JobInfo);
					break;
				case DtsEventId.ePRIMITIVE_PROGRESS_INFO:
					result = dtsSystemImpl._onPrimitiveProgressInfo(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.Progress);
					break;
				case DtsEventId.ePRIMITIVE_REPETION_STOPPED:
					result = dtsSystemImpl._onPrimitiveRepetitionStopped(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
					break;
				case DtsEventId.ePRIMITIVE_TERMINATED:
					result = dtsSystemImpl._onPrimitiveTerminated(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
					break;
				case DtsEventId.eSYSTEM_CLAMP_STATE_CHANGED:
					result = dtsSystemImpl._onSystemClampStateChanged(dtsEvent.JobInfo, dtsEvent.ClampState);
					break;
				case DtsEventId.eSYSTEM_CONFIGURATION_CLOSED:
					result = dtsSystemImpl._onSystemConfigurationClosed();
					break;
				case DtsEventId.eSYSTEM_CONFIGURATION_OPENED:
					result = dtsSystemImpl._onSystemConfigurationOpened();
					break;
				case DtsEventId.eSYSTEM_ERROR:
					result = dtsSystemImpl._onSystemError(dtsEvent.Error);
					break;
				case DtsEventId.eSYSTEM_LOCKED:
					result = dtsSystemImpl._onSystemLocked();
					break;
				case DtsEventId.eSYSTEM_LOGICALLY_CONNECTED:
					result = dtsSystemImpl._onSystemLogicallyConnected();
					break;
				case DtsEventId.eSYSTEM_LOGICALLY_DISCONNECTED:
					result = dtsSystemImpl._onSystemLogicallyDisconnected();
					break;
				case DtsEventId.eSYSTEM_PROJECT_DESELECTED:
					result = dtsSystemImpl._onSystemProjectDeselected();
					break;
				case DtsEventId.eSYSTEM_PROJECT_SELECTED:
					result = dtsSystemImpl._onSystemProjectSelected();
					break;
				case DtsEventId.eSYSTEM_UNLOCKED:
					result = dtsSystemImpl._onSystemUnlocked();
					break;
				case DtsEventId.eSYSTEM_VEHICLEINFORMATION_DESELECTED:
					result = dtsSystemImpl._onSystemVehicleInfoDeselected();
					break;
				case DtsEventId.eSYSTEM_VEHICLEINFORMATION_SELECTED:
					result = dtsSystemImpl._onSystemVehicleInfoSelected();
					break;
				case DtsEventId.eCONFIGURATION_RECORD_LOADED:
					result = dtsSystemImpl._onConfigurationRecordLoaded(dtsEvent.ConfigurationRecord);
					break;
				case DtsEventId.eSTATIC_INTERFACE_ERROR:
					result = dtsSystemImpl._onStaticInterfaceError(dtsEvent.Error);
					break;
				case DtsEventId.eINTERFACE_STATUS_CHANGED:
					result = dtsSystemImpl._onInterfaceStatusChanged(dtsEvent.Interface, dtsEvent.InterfaceStatus);
					break;
				case DtsEventId.eMONITORING_FRAMES_READY:
					result = dtsSystemImpl._onMonitoringFramesReady(dtsEvent.MonitoringLink);
					break;
				case DtsEventId.eINTERFACE_ERROR:
					result = dtsSystemImpl._onInterfaceError(dtsEvent.Interface, dtsEvent.Error);
					break;
				case DtsEventId.eINTERFACES_MODIFIED:
					result = dtsSystemImpl._onInterfacesModified();
					break;
				case DtsEventId.eDETECTION_FINISHED:
					result = dtsSystemImpl._onDetectionFinished();
					break;
				case DtsEventId.eINTERFACE_DETECTED:
					result = dtsSystemImpl._onInterfaceDetected(dtsEvent.Interface);
					break;
				}
				break;
			}
			case MCDObjectType.eMCDDELAY:
			{
				DtsDelayImpl dtsDelayImpl = (DtsDelayImpl)mCDObject;
				switch (dtsEvent.EventId)
				{
				case DtsEventId.ePRIMITIVE_BUFFER_OVERFLOW:
					result = dtsDelayImpl._onPrimitiveBufferOverflow(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
					break;
				case DtsEventId.ePRIMITIVE_CANCELED_DURING_EXECUTION:
					result = dtsDelayImpl._onPrimitiveCanceledDuringExecution(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
					break;
				case DtsEventId.ePRIMITIVE_CANCELED_FROM_QUEUE:
					result = dtsDelayImpl._onPrimitiveCanceledFromQueue(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
					break;
				case DtsEventId.ePRIMITIVE_ERROR:
					result = dtsDelayImpl._onPrimitiveError(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.Error);
					break;
				case DtsEventId.ePRIMITIVE_HAS_INTERMEDIATE_RESULT:
					result = dtsDelayImpl._onPrimitiveHasIntermediateResult(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
					break;
				case DtsEventId.ePRIMITIVE_HAS_RESULT:
					result = dtsDelayImpl._onPrimitiveHasResult(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
					break;
				case DtsEventId.ePRIMITIVE_JOB_INFO:
					result = dtsDelayImpl._onPrimitiveJobInfo(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.JobInfo);
					break;
				case DtsEventId.ePRIMITIVE_PROGRESS_INFO:
					result = dtsDelayImpl._onPrimitiveProgressInfo(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.Progress);
					break;
				case DtsEventId.ePRIMITIVE_REPETION_STOPPED:
					result = dtsDelayImpl._onPrimitiveRepetitionStopped(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
					break;
				case DtsEventId.ePRIMITIVE_TERMINATED:
					result = dtsDelayImpl._onPrimitiveTerminated(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
					break;
				}
				break;
			}
			case MCDObjectType.eMCDGOTOOFFLINE:
			{
				DtsGotoOfflineImpl dtsGotoOfflineImpl = (DtsGotoOfflineImpl)mCDObject;
				switch (dtsEvent.EventId)
				{
				case DtsEventId.ePRIMITIVE_BUFFER_OVERFLOW:
					result = dtsGotoOfflineImpl._onPrimitiveBufferOverflow(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
					break;
				case DtsEventId.ePRIMITIVE_CANCELED_DURING_EXECUTION:
					result = dtsGotoOfflineImpl._onPrimitiveCanceledDuringExecution(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
					break;
				case DtsEventId.ePRIMITIVE_CANCELED_FROM_QUEUE:
					result = dtsGotoOfflineImpl._onPrimitiveCanceledFromQueue(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
					break;
				case DtsEventId.ePRIMITIVE_ERROR:
					result = dtsGotoOfflineImpl._onPrimitiveError(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.Error);
					break;
				case DtsEventId.ePRIMITIVE_HAS_INTERMEDIATE_RESULT:
					result = dtsGotoOfflineImpl._onPrimitiveHasIntermediateResult(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
					break;
				case DtsEventId.ePRIMITIVE_HAS_RESULT:
					result = dtsGotoOfflineImpl._onPrimitiveHasResult(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
					break;
				case DtsEventId.ePRIMITIVE_JOB_INFO:
					result = dtsGotoOfflineImpl._onPrimitiveJobInfo(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.JobInfo);
					break;
				case DtsEventId.ePRIMITIVE_PROGRESS_INFO:
					result = dtsGotoOfflineImpl._onPrimitiveProgressInfo(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.Progress);
					break;
				case DtsEventId.ePRIMITIVE_REPETION_STOPPED:
					result = dtsGotoOfflineImpl._onPrimitiveRepetitionStopped(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
					break;
				case DtsEventId.ePRIMITIVE_TERMINATED:
					result = dtsGotoOfflineImpl._onPrimitiveTerminated(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
					break;
				}
				break;
			}
			case MCDObjectType.eMCDGOTOONLINE:
			{
				DtsGotoOnlineImpl dtsGotoOnlineImpl = (DtsGotoOnlineImpl)mCDObject;
				switch (dtsEvent.EventId)
				{
				case DtsEventId.ePRIMITIVE_BUFFER_OVERFLOW:
					result = dtsGotoOnlineImpl._onPrimitiveBufferOverflow(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
					break;
				case DtsEventId.ePRIMITIVE_CANCELED_DURING_EXECUTION:
					result = dtsGotoOnlineImpl._onPrimitiveCanceledDuringExecution(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
					break;
				case DtsEventId.ePRIMITIVE_CANCELED_FROM_QUEUE:
					result = dtsGotoOnlineImpl._onPrimitiveCanceledFromQueue(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
					break;
				case DtsEventId.ePRIMITIVE_ERROR:
					result = dtsGotoOnlineImpl._onPrimitiveError(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.Error);
					break;
				case DtsEventId.ePRIMITIVE_HAS_INTERMEDIATE_RESULT:
					result = dtsGotoOnlineImpl._onPrimitiveHasIntermediateResult(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
					break;
				case DtsEventId.ePRIMITIVE_HAS_RESULT:
					result = dtsGotoOnlineImpl._onPrimitiveHasResult(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
					break;
				case DtsEventId.ePRIMITIVE_JOB_INFO:
					result = dtsGotoOnlineImpl._onPrimitiveJobInfo(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.JobInfo);
					break;
				case DtsEventId.ePRIMITIVE_PROGRESS_INFO:
					result = dtsGotoOnlineImpl._onPrimitiveProgressInfo(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.Progress);
					break;
				case DtsEventId.ePRIMITIVE_REPETION_STOPPED:
					result = dtsGotoOnlineImpl._onPrimitiveRepetitionStopped(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
					break;
				case DtsEventId.ePRIMITIVE_TERMINATED:
					result = dtsGotoOnlineImpl._onPrimitiveTerminated(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
					break;
				}
				break;
			}
			case MCDObjectType.eMCDRAWSERVICE:
			{
				DtsRawServiceImpl dtsRawServiceImpl = (DtsRawServiceImpl)mCDObject;
				switch (dtsEvent.EventId)
				{
				case DtsEventId.ePRIMITIVE_BUFFER_OVERFLOW:
					result = dtsRawServiceImpl._onPrimitiveBufferOverflow(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
					break;
				case DtsEventId.ePRIMITIVE_CANCELED_DURING_EXECUTION:
					result = dtsRawServiceImpl._onPrimitiveCanceledDuringExecution(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
					break;
				case DtsEventId.ePRIMITIVE_CANCELED_FROM_QUEUE:
					result = dtsRawServiceImpl._onPrimitiveCanceledFromQueue(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
					break;
				case DtsEventId.ePRIMITIVE_ERROR:
					result = dtsRawServiceImpl._onPrimitiveError(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.Error);
					break;
				case DtsEventId.ePRIMITIVE_HAS_INTERMEDIATE_RESULT:
					result = dtsRawServiceImpl._onPrimitiveHasIntermediateResult(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
					break;
				case DtsEventId.ePRIMITIVE_HAS_RESULT:
					result = dtsRawServiceImpl._onPrimitiveHasResult(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
					break;
				case DtsEventId.ePRIMITIVE_JOB_INFO:
					result = dtsRawServiceImpl._onPrimitiveJobInfo(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.JobInfo);
					break;
				case DtsEventId.ePRIMITIVE_PROGRESS_INFO:
					result = dtsRawServiceImpl._onPrimitiveProgressInfo(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.Progress);
					break;
				case DtsEventId.ePRIMITIVE_REPETION_STOPPED:
					result = dtsRawServiceImpl._onPrimitiveRepetitionStopped(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink);
					break;
				case DtsEventId.ePRIMITIVE_TERMINATED:
					result = dtsRawServiceImpl._onPrimitiveTerminated(dtsEvent.DiagComPrimitive, dtsEvent.LogicalLink, dtsEvent.ResultState);
					break;
				}
				break;
			}
			}
		}
		return result;
	}
}
