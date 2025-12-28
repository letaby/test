// Decompiled with JetBrains decompiler
// Type: SapiLayer1.SapiError
// Assembly: SAPI, Version=1.29.837.0, Culture=neutral, PublicKeyToken=2f6b11bb555a207b
// MVID: 18016536-4FB3-411C-A763-231E25A150EB
// Assembly location: C:\Users\petra\Downloads\Архив (2)\SAPI.dll

using System.ComponentModel;

#nullable disable
namespace SapiLayer1;

public enum SapiError
{
  None = 0,
  [Description("06035:Presentation: BytePos was greater than the message length")] BytePosGreaterThanMessageLength = 6035, // 0x00001793
  [Description("06055:No security access for reading that fragment")] NoSecurityAccessForReadingThatFragment = 6055, // 0x000017A7
  [Description("06056:Access denied, authorization level too low")] AccessDeniedAuthorizationLevelTooLow = 6056, // 0x000017A8
  [Description("06058:No matching interval was found")] NoMatchingIntervalWasFound = 6058, // 0x000017AA
  [Description("06059:Preparation value was out of limits")] PreparationValueOutOfLimits = 6059, // 0x000017AB
  [Description("06119:VarCoding: no matching partnumber and partversion found")] NoMatchingPartNumberAndPartVersionFound = 6119, // 0x000017E7
  [Description("06126:VarCoding: no matching choice value in varcoding string found")] NoMatchingChoiceValueInVarcodingString = 6126, // 0x000017EE
  [Description("6600: Default string read data didn't match write data")] DefaultStringReadWriteMismatch = 6600, // 0x000019C8
  [Description("6601: Could not read associated coding string")] DefaultStringNotAccessible = 6601, // 0x000019C9
  [Description("6602: Fragment read data didn't match write data")] FragmentReadWriteMismatch = 6602, // 0x000019CA
  [Description("6603: Could not read associated fragment")] FragmentNotAccessible = 6603, // 0x000019CB
  [Description("6604: Cannot enter flashing mode")] CannotEnterFlashingMode = 6604, // 0x000019CC
  [Description("6605: Could not get comparameter specification")] ComParameterSpecUnavailable = 6605, // 0x000019CD
  [Description("6606: Unknown presentation type")] UnknownPresentationType = 6606, // 0x000019CE
  [Description("6607: Communications ceased during synchronous operation")] CommunicationsCeasedDuringSyncOperation = 6607, // 0x000019CF
  [Description("6608: Write service not available")] WriteServiceNotAvailable = 6608, // 0x000019D0
  [Description("6609: Read service not available")] ReadServiceNotAvailable = 6609, // 0x000019D1
  [Description("6610: Connection resource not available")] ConnectionResourceNotAvailable = 6610, // 0x000019D2
  [Description("6611: Disconnected, found better variant match from other ECU")] FoundBetterVariantMatch = 6611, // 0x000019D3
  [Description("6612: A channel is already connected to that identifier")] ChannelAlreadyConnectedToIdentifier = 6612, // 0x000019D4
  [Description("6613: A parameter specified in a CPF file did not exist")] ParameterSpecifiedDidNotExist = 6613, // 0x000019D5
  [Description("6614: Connection resource not available - other baud rate in use by connected channel")] ConnectionResourceNotAvailableOtherBaudRateInUseByConnectedChannel = 6614, // 0x000019D6
  [Description("6615: Connection resource not available - other baud rate in use by connecting channel")] ConnectionResourceNotAvailableOtherBaudRateInUseByConnectingChannel = 6615, // 0x000019D7
  [Description("6616: Functional ECU configuration error (missing CBF or undefined programming sequence)")] FunctionalEcuConfigurationError = 6616, // 0x000019D8
  [Description("6616: Functional ECU resource not available")] FunctionalEcuResourceNotAvailable = 6616, // 0x000019D8
  [Description("6617: Communications ceased during varcoding")] CommunicationsCeasedDuringVarcoding = 6617, // 0x000019D9
  [Description("6618: SAE Device invalid")] DeviceInvalid = 6618, // 0x000019DA
  [Description("6619: Not supported for SAE device")] NotSupportedForDevice = 6619, // 0x000019DB
  [Description("6620: Cannot send message to SAE device")] CannotSendMessageToDevice = 6620, // 0x000019DC
  [Description("6621: Timeout receiving message from SAE device")] TimeoutReceivingMessageFromDevice = 6621, // 0x000019DD
  [Description("6622: Negative response message from SAE device")] NegativeResponseMessageFromDevice = 6622, // 0x000019DE
  [Description("6623: Busy response message from SAE device")] BusyResponseMessageFromDevice = 6623, // 0x000019DF
  [Description("6624: Diagnostic information has no plausible match")] NoPlausibleVariantMatch = 6624, // 0x000019E0
  [Description("6625: Found better variant match from other ECU but connection resource is restricted")] FoundBetterVariantMatchButResourceRestricted = 6625, // 0x000019E1
  [Description("6626: Found better variant match from other ECU but connection resource is unavailable")] FoundBetterVariantMatchButResourceUnavailable = 6626, // 0x000019E2
  [Description("6627: VCP parameter error")] ExternalVcpParameterError = 6627, // 0x000019E3
  [Description("6628: VCP execution error")] ExternalVcpExecutionError = 6628, // 0x000019E4
  [Description("6629: VCP not found")] ExternalVcpNotFound = 6629, // 0x000019E5
  [Description("6630: VCP did not create an output .VER file")] ExternalVcpVerFileNotCreated = 6630, // 0x000019E6
  [Description("6631: VCP component error")] ExternalVcpComponentError = 6631, // 0x000019E7
  [Description("6632: Diagnostic information is not available for variant match")] NoDiagnosticVersionForVariantMatch = 6632, // 0x000019E8
  [Description("6633: Unable to connect to PassThru device")] CannotConnectToPassThruDevice = 6633, // 0x000019E9
  [Description("6634: Failed to start message filter for PassThru device")] CannotStartMessageFilterForPassThruDevice = 6634, // 0x000019EA
  [Description("6635: SID services are not available")] SidNotAvailable = 6635, // 0x000019EB
  [Description("6636: Failed to start CAESAR monitoring")] FailedToStartCaesarMonitoring = 6636, // 0x000019EC
  [Description("6637: Unable to read CAN monitoring statistics")] UnableToRetrieveCanMonitoringStatistics = 6637, // 0x000019ED
  [Description("6638: Hardware not responding")] HardwareNotResponding = 6638, // 0x000019EE
  [Description("6639: Passthru device error")] PassThruDeviceError = 6639, // 0x000019EF
  [Description("6640: An access violation occurred during a service or diagjob execution")] AccessViolationDuringServiceExecution = 6640, // 0x000019F0
  [Description("6641: The ECU failed to remain in the intended diagnostic session")] EcuFailedToRemainInDiagnosticSession = 6641, // 0x000019F1
  [Description("6642: Fragment qualifier specified in MCD-3D was not found")] FragmentNotFound = 6642, // 0x000019F2
  [Description("6643: Fragment value specified in ODX-E was not valid")] FragmentValueNotValid = 6643, // 0x000019F3
  [Description("6644: Domain pecified in ODX-E was not valid")] DomainNotFound = 6644, // 0x000019F4
  [Description("6645: Default string specified by part number was not found")] DefaultStringKeyNotFound = 6645, // 0x000019F5
  [Description("6646: Fragment specified by part number was not found")] FragmentKeyNotFound = 6646, // 0x000019F6
  [Description("6647: Splitted parameter group diagjob not found")] SplitGroupDiagjobNotFound = 6647, // 0x000019F7
  [Description("6648: offline variant coding handle is not available")] OfflineVarcodingNotAvailable = 6648, // 0x000019F8
  [Description("6649: unable to locate SMR-D DiagComPrimitive from SMR-E reference")] DiagComPrimitiveReferenceFromCodingFileNotFound = 6649, // 0x000019F9
  [Description("6650: the pending connection did not reach the target state")] PendingConnectionTargetStateNotReached = 6650, // 0x000019FA
  [Description("6651: the pending connection was aborted")] PendingConnectionAborted = 6651, // 0x000019FB
  [Description("6652: CAESAR configuration specified PDU-API but there are no loadable PDU-API devices in that configuration")] NoLoadableCaesarPduApi = 6652, // 0x000019FC
  [Description("6653: translator box was disconnected")] TranslatorDisconnected = 6653, // 0x000019FD
  [Description("6654: no traffic was seen")] NoTraffic = 6654, // 0x000019FE
  [Description("Negative response")] NegativeResponse = 6701, // 0x00001A2D
  [Description("52912: No matching DiagCert found for authentication")] NoMatchingDiagnosticCertificateForAuthentication = 52912, // 0x0000CEB0
}
