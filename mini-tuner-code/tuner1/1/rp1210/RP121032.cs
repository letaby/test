// Decompiled with JetBrains decompiler
// Type: rp1210.RP121032
// Assembly: TunerSolution, Version=1.0.0.142, Culture=neutral, PublicKeyToken=null
// MVID: 9D02C703-4AB8-4296-B056-FAFCB6EB03BA
// Assembly location: C:\Users\petra\Downloads\TunerSolution\TunerSolution.exe

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

#nullable disable
namespace rp1210;

public class RP121032 : IDisposable
{
  public const string RP1210_INI = "rp121032.ini";
  public const byte BLOCKING_IO = 1;
  public const byte NON_BLOCKING_IO = 0;
  public const byte CONVERTED_MODE = 1;
  public const byte RAW_MODE = 0;
  public const uint MAX_J1708_MESSAGE_LENGTH = 508;
  public const uint MAX_J1939_MESSAGE_LENGTH = 1796;
  public const uint MAX_J1850_MESSAGE_LENGTH = 508;
  public const uint CAN_DATA_SIZE = 8;
  public const byte ECHO_OFF = 0;
  public const byte ECHO_ON = 1;
  public const byte RECEIVE_ON = 1;
  public const byte RECEIVE_OFF = 0;
  public const uint FILTER_PGN = 1;
  public const uint FILTER_PRIORITY = 2;
  public const uint FILTER_SOURCE = 4;
  public const uint FILTER_DESTINATION = 8;
  public const byte SILENT_J1939_CLAIM = 0;
  public const byte PASS_J1939_CLAIM_MESSAGES = 1;
  public const byte STANDARD_CAN = 0;
  public const byte EXTENDED_CAN = 1;
  private const string RP1210_CLIENT_CONNECT = "RP1210_ClientConnect";
  private const string RP1210_CLIENT_DISCONNECT = "RP1210_ClientDisconnect";
  private const string RP1210_GET_ERROR_MSG = "RP1210_GetErrorMsg";
  private const string RP1210_GET_HARDWARE_STATUS = "RP1210_GetHardwareStatus";
  private const string RP1210_READ_DETAILED_VERSION = "RP1210_ReadDetailedVersion";
  private const string RP1210_READ_MESSAGE = "RP1210_ReadMessage";
  private const string RP1210_READ_VERSION = "RP1210_ReadVersion";
  private const string RP1210_SEND_COMMAND = "RP1210_SendCommand";
  private const string RP1210_SEND_MESSAGE = "RP1210_SendMessage";
  private bool disposed;
  private short _connectedDevice = -1;
  private IntPtr pDLL;
  private IntPtr fpRP1210_ClientConnect;
  private IntPtr fpRP1210_ClientDisconnect;
  private IntPtr fpRP1210_SendMessage;
  private IntPtr fpRP1210_ReadMessage;
  private IntPtr fpRP1210_SendCommand;
  private IntPtr fpRP1210_ReadVersion;
  private IntPtr fpRP1210_ReadDetailedVersion;
  private IntPtr fpRP1210_GetHardwareStatus;
  private IntPtr fpRP1210_GetErrorMsg;
  private RP121032.RP1210ClientConnect pRP1210_ClientConnect;
  private RP121032.RP1210ClientDisconnect pRP1210_ClientDisconnect;
  private RP121032.RP1210SendMessage pRP1210_SendMessage;
  private RP121032.RP1210ReadMessage pRP1210_ReadMessage;
  private RP121032.RP1210SendCommand pRP1210_SendCommand;
  private RP121032.RP1210ReadVersion pRP1210_ReadVersion;
  private RP121032.RP1210ReadDetailedVersion pRP1210_ReadDetailedVersion;
  private RP121032.RP1210GetHardwareStatus pRP1210_GetHardwareStatus;
  private RP121032.RP1210GetErrorMsg pRP1210_GetErrorMsg;
  private RP1210BDriverInfo _DriverInfo;

  public short nClientID => this._connectedDevice;

  public RP1210BDriverInfo DriverInfo => this._DriverInfo;

  public short MaxBufferSize { get; set; }

  public RP121032(string NameOfRP1210DLL)
  {
    string dllToLoad = $"{Environment.GetEnvironmentVariable("SystemRoot")}\\System32\\{NameOfRP1210DLL}.dll";
    string strDeviceIniPath = $"{Environment.GetEnvironmentVariable("SystemRoot")}\\{NameOfRP1210DLL}.ini";
    this.pDLL = RP121032.Win32API.LoadLibrary(dllToLoad);
    if (!(this.pDLL == IntPtr.Zero))
    {
      this.fpRP1210_ClientConnect = RP121032.Win32API.GetProcAddress(this.pDLL, "RP1210_ClientConnect");
      this.fpRP1210_ClientDisconnect = RP121032.Win32API.GetProcAddress(this.pDLL, "RP1210_ClientDisconnect");
      this.fpRP1210_SendMessage = RP121032.Win32API.GetProcAddress(this.pDLL, "RP1210_SendMessage");
      this.fpRP1210_ReadMessage = RP121032.Win32API.GetProcAddress(this.pDLL, "RP1210_ReadMessage");
      this.fpRP1210_SendCommand = RP121032.Win32API.GetProcAddress(this.pDLL, "RP1210_SendCommand");
      this.fpRP1210_ReadVersion = RP121032.Win32API.GetProcAddress(this.pDLL, "RP1210_ReadVersion");
      this.fpRP1210_ReadDetailedVersion = RP121032.Win32API.GetProcAddress(this.pDLL, "RP1210_ReadDetailedVersion");
      this.fpRP1210_GetHardwareStatus = RP121032.Win32API.GetProcAddress(this.pDLL, "RP1210_GetHardwareStatus");
      this.fpRP1210_GetErrorMsg = RP121032.Win32API.GetProcAddress(this.pDLL, "RP1210_GetErrorMsg");
      this.pRP1210_ClientConnect = (RP121032.RP1210ClientConnect) Marshal.GetDelegateForFunctionPointer(this.fpRP1210_ClientConnect, typeof (RP121032.RP1210ClientConnect));
      this.pRP1210_ClientDisconnect = (RP121032.RP1210ClientDisconnect) Marshal.GetDelegateForFunctionPointer(this.fpRP1210_ClientDisconnect, typeof (RP121032.RP1210ClientDisconnect));
      this.pRP1210_SendMessage = (RP121032.RP1210SendMessage) Marshal.GetDelegateForFunctionPointer(this.fpRP1210_SendMessage, typeof (RP121032.RP1210SendMessage));
      this.pRP1210_ReadMessage = (RP121032.RP1210ReadMessage) Marshal.GetDelegateForFunctionPointer(this.fpRP1210_ReadMessage, typeof (RP121032.RP1210ReadMessage));
      this.pRP1210_SendCommand = (RP121032.RP1210SendCommand) Marshal.GetDelegateForFunctionPointer(this.fpRP1210_SendCommand, typeof (RP121032.RP1210SendCommand));
      this.pRP1210_ReadVersion = (RP121032.RP1210ReadVersion) Marshal.GetDelegateForFunctionPointer(this.fpRP1210_ReadVersion, typeof (RP121032.RP1210ReadVersion));
      this.pRP1210_ReadDetailedVersion = (RP121032.RP1210ReadDetailedVersion) Marshal.GetDelegateForFunctionPointer(this.fpRP1210_ReadDetailedVersion, typeof (RP121032.RP1210ReadDetailedVersion));
      this.pRP1210_GetHardwareStatus = (RP121032.RP1210GetHardwareStatus) Marshal.GetDelegateForFunctionPointer(this.fpRP1210_GetHardwareStatus, typeof (RP121032.RP1210GetHardwareStatus));
      this.pRP1210_GetErrorMsg = (RP121032.RP1210GetErrorMsg) Marshal.GetDelegateForFunctionPointer(this.fpRP1210_GetErrorMsg, typeof (RP121032.RP1210GetErrorMsg));
      this._DriverInfo = RP121032.LoadDeviceParameters(strDeviceIniPath);
    }
    this.MaxBufferSize = (short) byte.MaxValue;
  }

  public void RP1210_ClientConnect(
    short nDeviceId,
    StringBuilder fpchProtocol,
    int lSendBuffer,
    int lReceiveBuffer,
    short nIsAppPacketizingIncomingMsgs)
  {
    short err_code = this.pRP1210_ClientConnect(IntPtr.Zero, nDeviceId, fpchProtocol, lSendBuffer, lReceiveBuffer, nIsAppPacketizingIncomingMsgs);
    if (err_code >= (short) 0 && err_code <= (short) sbyte.MaxValue)
      this._connectedDevice = err_code;
    else if (err_code != (short) 275)
      throw new Exception($"ClientConnect Failed: {Convert.ToString(err_code)} {this.RP1210_GetErrorMsg((RP1210_Returns) err_code)}");
  }

  public RP1210_Returns RP1210_ClientDisconnect()
  {
    return (RP1210_Returns) this.pRP1210_ClientDisconnect(this._connectedDevice);
  }

  public RP1210_Returns RP1210_SendMessage(
    byte[] fpchClientMessage,
    short nMessageSize,
    short nNotifyStatusOnTx,
    short nBlockOnSend)
  {
    if (this._connectedDevice >= (short) 0)
    {
      RP1210_Returns err_code = (RP1210_Returns) this.pRP1210_SendMessage(this._connectedDevice, fpchClientMessage, nMessageSize, nNotifyStatusOnTx, nBlockOnSend);
      return (short) err_code <= (short) sbyte.MaxValue ? err_code : throw new Exception("SendMessage Failed: " + this.RP1210_GetErrorMsg(err_code));
    }
    throw new Exception("Device Not Connected");
  }

  public byte[] RP1210_ReadMessage(short nBlockOnSend)
  {
    byte[] numArray = new byte[(int) this.MaxBufferSize];
    RP1210_Returns length = (RP1210_Returns) this.pRP1210_ReadMessage(this._connectedDevice, numArray, this.MaxBufferSize, nBlockOnSend);
    byte[] destinationArray = (int) (ushort) length <= (int) this.MaxBufferSize ? new byte[(int) (short) length] : throw new Exception("ReadMessage Failed: " + this.RP1210_GetErrorMsg(length));
    Array.Copy((Array) numArray, (Array) destinationArray, (int) (short) length);
    return destinationArray;
  }

  public void RP1210_SendCommand(
    RP1210_Commands nCommandNumber,
    byte[] fpchClientCommand,
    short nMessageSize)
  {
    short err_code = this.pRP1210_SendCommand((short) nCommandNumber, this._connectedDevice, fpchClientCommand, nMessageSize);
    if (err_code != (short) 0)
      throw new Exception("SendCommand Failed: " + this.RP1210_GetErrorMsg((RP1210_Returns) err_code));
  }

  public string[] RP1210_ReadVersion()
  {
    string[] strArray = new string[2];
    StringBuilder fpchDLLMajorVersion = new StringBuilder();
    StringBuilder fpchDLLMinorVersion = new StringBuilder();
    StringBuilder fpchAPIMajorVersion = new StringBuilder();
    StringBuilder fpchAPIMinorVersion = new StringBuilder();
    this.pRP1210_ReadVersion(fpchDLLMajorVersion, fpchDLLMinorVersion, fpchAPIMajorVersion, fpchAPIMinorVersion);
    strArray[0] = $"{fpchDLLMajorVersion.ToString()}.{fpchDLLMinorVersion.ToString()}";
    strArray[1] = $"{fpchAPIMajorVersion.ToString()}.{fpchAPIMinorVersion.ToString()}";
    return strArray;
  }

  public string[] RP1210_ReadDetailedVersion()
  {
    string[] strArray = new string[3];
    StringBuilder fpchAPIVersionInfo = new StringBuilder();
    StringBuilder fpchDLLVersionInfo = new StringBuilder();
    StringBuilder fpchFWVersionInfo = new StringBuilder();
    RP1210_Returns err_code = (RP1210_Returns) this.pRP1210_ReadDetailedVersion(this._connectedDevice, fpchAPIVersionInfo, fpchDLLVersionInfo, fpchFWVersionInfo);
    if ((short) err_code > (short) sbyte.MaxValue)
      throw new Exception("ReadDetailedVersion Failed: " + this.RP1210_GetErrorMsg(err_code));
    strArray[0] = fpchDLLVersionInfo.ToString();
    strArray[1] = fpchAPIVersionInfo.ToString();
    strArray[2] = fpchFWVersionInfo.ToString();
    return strArray;
  }

  public RP1210_Returns RP1210_GetHardwareStatus(
    StringBuilder fpchClientInfo,
    short nInfoSize,
    short nBlockOnRequest)
  {
    return (RP1210_Returns) this.pRP1210_GetHardwareStatus(this._connectedDevice, fpchClientInfo, nInfoSize, nBlockOnRequest);
  }

  public string RP1210_GetErrorMsg(RP1210_Returns err_code)
  {
    StringBuilder fpchMessage = new StringBuilder();
    int num = (int) this.pRP1210_GetErrorMsg((short) err_code, fpchMessage);
    return fpchMessage.ToString();
  }

  public static List<string> ScanForDrivers()
  {
    string iniPath = Environment.GetEnvironmentVariable("SystemRoot") + "\\rp121032.ini";
    try
    {
      return new List<string>((IEnumerable<string>) new IniParser(iniPath).GetSetting("RP1210Support", "APIImplementations").Split(','));
    }
    catch
    {
      return (List<string>) null;
    }
  }

  public static RP1210BDriverInfo LoadDeviceParameters(string strDeviceIniPath)
  {
    RP1210BDriverInfo rp1210BdriverInfo = new RP1210BDriverInfo();
    IniParser iniParser = new IniParser(strDeviceIniPath);
    try
    {
      rp1210BdriverInfo.DriverVersion = iniParser.GetSetting("VendorInformation", "Version");
    }
    catch
    {
      rp1210BdriverInfo.DriverVersion = "Unknown";
    }
    try
    {
      rp1210BdriverInfo.VendorName = iniParser.GetSetting("VendorInformation", "Name");
    }
    catch
    {
      rp1210BdriverInfo.VendorName = "Unknown";
    }
    try
    {
      rp1210BdriverInfo.RP1210Version = iniParser.GetSetting("VendorInformation", "RP1210");
    }
    catch
    {
      rp1210BdriverInfo.RP1210Version = "B";
    }
    try
    {
      rp1210BdriverInfo.TimestampWeight = (int) Convert.ToInt16(iniParser.GetSetting("VendorInformation", "TimestampWeight"));
    }
    catch
    {
      rp1210BdriverInfo.TimestampWeight = 1000;
    }
    try
    {
      string setting = iniParser.GetSetting("VendorInformation", "CANFormatsSupported");
      rp1210BdriverInfo.CANFormatsSupported = new List<short>((IEnumerable<short>) Array.ConvertAll<string, short>(setting.Split(','), (Converter<string, short>) (x => Convert.ToInt16(x))));
    }
    catch
    {
      rp1210BdriverInfo.CANFormatsSupported = new List<short>((IEnumerable<short>) new short[1]
      {
        (short) 4
      });
    }
    try
    {
      string setting = iniParser.GetSetting("VendorInformation", "J1939FormatsSupported");
      rp1210BdriverInfo.J1939FormatsSupported = new List<short>((IEnumerable<short>) Array.ConvertAll<string, short>(setting.Split(','), (Converter<string, short>) (x => Convert.ToInt16(x))));
    }
    catch
    {
      rp1210BdriverInfo.J1939FormatsSupported = new List<short>((IEnumerable<short>) new short[1]
      {
        (short) 2
      });
    }
    try
    {
      string setting = iniParser.GetSetting("VendorInformation", "J1708FormatsSupported");
      rp1210BdriverInfo.J1708FormatsSupported = new List<short>((IEnumerable<short>) Array.ConvertAll<string, short>(setting.Split(','), (Converter<string, short>) (x => Convert.ToInt16(x))));
    }
    catch
    {
      rp1210BdriverInfo.J1708FormatsSupported = new List<short>((IEnumerable<short>) new short[1]
      {
        (short) 2
      });
    }
    try
    {
      string setting = iniParser.GetSetting("VendorInformation", "ISO15765FormatsSupported");
      rp1210BdriverInfo.CANFormatsSupported = new List<short>((IEnumerable<short>) Array.ConvertAll<string, short>(setting.Split(','), (Converter<string, short>) (x => Convert.ToInt16(x))));
    }
    catch
    {
      rp1210BdriverInfo.ISO15765FormatsSupported = new List<short>((IEnumerable<short>) new short[1]
      {
        (short) 2
      });
    }
    string setting1 = iniParser.GetSetting("VendorInformation", "Devices");
    char[] chArray1 = new char[1]{ ',' };
    foreach (string str in new List<string>((IEnumerable<string>) setting1.Split(chArray1)))
      rp1210BdriverInfo.RP1210Devices.Add(new DeviceInfo()
      {
        DeviceId = Convert.ToInt16(iniParser.GetSetting("DeviceInformation" + str, "DeviceId")),
        DeviceDescription = iniParser.GetSetting("DeviceInformation" + str, "DeviceDescription"),
        DeviceName = iniParser.GetSetting("DeviceInformation" + str, "DeviceName"),
        DeviceParams = iniParser.GetSetting("DeviceInformation" + str, "DeviceParams")
      });
    string setting2 = iniParser.GetSetting("VendorInformation", "Protocols");
    char[] chArray2 = new char[1]{ ',' };
    foreach (string str1 in new List<string>((IEnumerable<string>) setting2.Split(chArray2)))
    {
      ProtocolInfo protocolInfo = new ProtocolInfo();
      protocolInfo.ProtocolString = iniParser.GetSetting("ProtocolInformation" + str1, "ProtocolString");
      protocolInfo.ProtocolDescription = iniParser.GetSetting("ProtocolInformation" + str1, "ProtocolDescription");
      protocolInfo.ProtocolParams = iniParser.GetSetting("ProtocolInformation" + str1, "ProtocolParams");
      try
      {
        string setting3 = iniParser.GetSetting("ProtocolInformation" + str1, "ProtocolSpeed");
        protocolInfo.ProtocolSpeed = new List<string>((IEnumerable<string>) setting3.Split(','));
      }
      catch
      {
      }
      foreach (string str2 in new List<string>((IEnumerable<string>) iniParser.GetSetting("ProtocolInformation" + str1, "Devices").Split(',')))
      {
        string devTemp = str2;
        rp1210BdriverInfo.RP1210Devices.Find((Predicate<DeviceInfo>) (x => (int) x.DeviceId == (int) Convert.ToInt16(devTemp))).SupportedProtocols.Add(protocolInfo);
      }
    }
    return rp1210BdriverInfo;
  }

  public static J1939Message DecodeJ1939Message(byte[] message)
  {
    J1939Message j1939Message = new J1939Message()
    {
      TimeStamp = (uint) (((int) message[0] << 24) + ((int) message[1] << 16 /*0x10*/) + ((int) message[2] << 8)) + (uint) message[3],
      PGN = (ushort) ((uint) (((int) message[6] << 16 /*0x10*/) + ((int) message[5] << 8)) + (uint) message[4]),
      Priority = message[7],
      SourceAddress = (short) message[8],
      DestinationAddress = (short) message[9],
      dataLength = (ushort) (message.Length - 10)
    };
    j1939Message.Data = new byte[(int) j1939Message.dataLength];
    Array.Copy((Array) message, 10, (Array) j1939Message.Data, 0, (int) j1939Message.dataLength);
    return j1939Message;
  }

  public static byte[] EncodeJ1939Message(J1939Message MessageToEncode)
  {
    byte num1 = 0;
    byte[] numArray1 = new byte[(int) MessageToEncode.dataLength + 6];
    byte[] numArray2 = numArray1;
    int index1 = (int) num1;
    byte num2 = (byte) (index1 + 1);
    int num3 = (int) (byte) ((uint) MessageToEncode.PGN & (uint) byte.MaxValue);
    numArray2[index1] = (byte) num3;
    byte[] numArray3 = numArray1;
    int index2 = (int) num2;
    byte num4 = (byte) (index2 + 1);
    int num5 = (int) (byte) ((int) MessageToEncode.PGN >> 8 & (int) byte.MaxValue);
    numArray3[index2] = (byte) num5;
    byte[] numArray4 = numArray1;
    int index3 = (int) num4;
    byte num6 = (byte) (index3 + 1);
    int num7 = (int) (byte) ((int) MessageToEncode.PGN >> 16 /*0x10*/ & (int) byte.MaxValue);
    numArray4[index3] = (byte) num7;
    byte[] numArray5 = numArray1;
    int index4 = (int) num6;
    byte num8 = (byte) (index4 + 1);
    numArray5[index4] |= MessageToEncode.Priority;
    byte[] numArray6 = numArray1;
    int index5 = (int) num8;
    byte num9 = (byte) (index5 + 1);
    int sourceAddress = (int) (byte) MessageToEncode.SourceAddress;
    numArray6[index5] = (byte) sourceAddress;
    byte[] numArray7 = numArray1;
    int index6 = (int) num9;
    byte num10 = (byte) (index6 + 1);
    int destinationAddress = (int) (byte) MessageToEncode.DestinationAddress;
    numArray7[index6] = (byte) destinationAddress;
    foreach (byte num11 in MessageToEncode.Data)
      numArray1[(int) num10++] = num11;
    return numArray1;
  }

  public static J1587Message DecodeJ1587Message(byte[] message)
  {
    J1587Message j1587Message = new J1587Message()
    {
      TimeStamp = (uint) (((int) message[0] << 24) + ((int) message[1] << 16 /*0x10*/) + ((int) message[2] << 8)) + (uint) message[3],
      MID = message[4],
      PID = message[5],
      DataLength = (ushort) (message.Length - 6)
    };
    j1587Message.Data = new byte[(int) j1587Message.DataLength];
    Array.Copy((Array) message, 6, (Array) j1587Message.Data, 0, (int) j1587Message.DataLength);
    return j1587Message;
  }

  public static byte[] EncodeJ1587Message(J1587Message MessageToEncode)
  {
    byte num1 = 0;
    byte[] numArray1 = new byte[(int) MessageToEncode.DataLength + 3];
    byte[] numArray2 = numArray1;
    int index1 = (int) num1;
    byte num2 = (byte) (index1 + 1);
    int priority = (int) MessageToEncode.Priority;
    numArray2[index1] = (byte) priority;
    byte[] numArray3 = numArray1;
    int index2 = (int) num2;
    byte num3 = (byte) (index2 + 1);
    int mid = (int) MessageToEncode.MID;
    numArray3[index2] = (byte) mid;
    byte[] numArray4 = numArray1;
    int index3 = (int) num3;
    byte num4 = (byte) (index3 + 1);
    int pid = (int) MessageToEncode.PID;
    numArray4[index3] = (byte) pid;
    foreach (byte num5 in MessageToEncode.Data)
      numArray1[(int) num4++] = num5;
    return numArray1;
  }

  public void Dispose()
  {
    if (!this.disposed)
    {
      try
      {
        this.RP1210_SendCommand(RP1210_Commands.RP1210_Reset_Device, (byte[]) null, (short) 0);
        int num = (int) this.RP1210_ClientDisconnect();
      }
      catch
      {
      }
      RP121032.Win32API.FreeLibrary(this.pDLL);
    }
    this.disposed = true;
  }

  ~RP121032() => this.Dispose();

  [UnmanagedFunctionPointer(CallingConvention.Winapi)]
  private delegate short RP1210ClientConnect(
    IntPtr hwndClient,
    short nDeviceId,
    StringBuilder fpchProtocol,
    int lSendBuffer,
    int lReceiveBuffer,
    short nIsAppPacketizingIncomingMsgs);

  [UnmanagedFunctionPointer(CallingConvention.Winapi)]
  private delegate short RP1210ClientDisconnect(short nClientID);

  [UnmanagedFunctionPointer(CallingConvention.Winapi)]
  private delegate short RP1210SendMessage(
    short nClientID,
    byte[] fpchClientMessage,
    short nMessageSize,
    short nNotifyStatusOnTx,
    short nBlockOnSend);

  [UnmanagedFunctionPointer(CallingConvention.Winapi)]
  private delegate short RP1210ReadMessage(
    short nClientID,
    byte[] fpchAPIMessage,
    short nBufferSize,
    short nBlockOnSend);

  [UnmanagedFunctionPointer(CallingConvention.Winapi)]
  private delegate short RP1210SendCommand(
    short nCommandNumber,
    short nClientID,
    byte[] fpchClientCommand,
    short nMessageSize);

  [UnmanagedFunctionPointer(CallingConvention.Winapi)]
  private delegate void RP1210ReadVersion(
    StringBuilder fpchDLLMajorVersion,
    StringBuilder fpchDLLMinorVersion,
    StringBuilder fpchAPIMajorVersion,
    StringBuilder fpchAPIMinorVersion);

  [UnmanagedFunctionPointer(CallingConvention.Winapi)]
  private delegate short RP1210ReadDetailedVersion(
    short nClientID,
    StringBuilder fpchAPIVersionInfo,
    StringBuilder fpchDLLVersionInfo,
    StringBuilder fpchFWVersionInfo);

  [UnmanagedFunctionPointer(CallingConvention.Winapi)]
  private delegate short RP1210GetHardwareStatus(
    short nClientID,
    StringBuilder fpchClientInfo,
    short nInfoSize,
    short nBlockOnRequest);

  [UnmanagedFunctionPointer(CallingConvention.Winapi)]
  private delegate short RP1210GetErrorMsg(short err_code, StringBuilder fpchMessage);

  private class Win32API
  {
    [DllImport("kernel32.dll")]
    public static extern IntPtr LoadLibrary(string dllToLoad);

    [DllImport("kernel32.dll")]
    public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

    [DllImport("kernel32.dll")]
    public static extern bool FreeLibrary(IntPtr hModule);
  }
}
