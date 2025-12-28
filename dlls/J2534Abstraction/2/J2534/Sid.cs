using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using _003CCppImplementationDetails_003E;

namespace J2534;

public sealed class Sid
{
	internal static Dictionary<uint, PassthruCallback> PassthruCallback = new Dictionary<uint, PassthruCallback>();

	public static J2534Error Open()
	{
		return (J2534Error)global::_003CModule_003E.PassThruOpen();
	}

	public static J2534Error Close()
	{
		return (J2534Error)global::_003CModule_003E.PassThruClose();
	}

	public static J2534Error Disconnect(uint channelId)
	{
		return (J2534Error)global::_003CModule_003E.PassThruDisconnect(channelId);
	}

	public unsafe static J2534Error Connect(ProtocolId id, uint options, ref uint channelId)
	{
		uint num = 0u;
		int result = global::_003CModule_003E.PassThruConnect((uint)id, options, &num);
		channelId = num;
		return (J2534Error)result;
	}

	public unsafe static J2534Error StartMsgFilter(uint channelId, FilterType type, PassThruMsg mask, PassThruMsg pattern, PassThruMsg flow, ref uint filterId)
	{
		//IL_0003: Expected I, but got I8
		//IL_0006: Expected I, but got I8
		//IL_0009: Expected I, but got I8
		PASSTHRU_MSG* ptr = null;
		PASSTHRU_MSG* ptr2 = null;
		PASSTHRU_MSG* ptr3 = null;
		uint num = 0u;
		if (mask != null)
		{
			ptr = mask.Convert();
		}
		if (pattern != null)
		{
			ptr2 = pattern.Convert();
		}
		if (flow != null)
		{
			ptr3 = flow.Convert();
		}
		int result = global::_003CModule_003E.PassThruStartMsgFilter(channelId, (uint)type, ptr, ptr2, ptr3, &num);
		if (ptr != null)
		{
			global::_003CModule_003E.delete(ptr);
		}
		if (ptr2 != null)
		{
			global::_003CModule_003E.delete(ptr2);
		}
		if (ptr3 != null)
		{
			global::_003CModule_003E.delete(ptr3);
		}
		filterId = num;
		return (J2534Error)result;
	}

	public static J2534Error StopMsgFilter(uint channelId, uint filterId)
	{
		return (J2534Error)global::_003CModule_003E.PassThruStopMsgFilter(channelId, filterId);
	}

	public unsafe static string GetLastError()
	{
		System.Runtime.CompilerServices.Unsafe.SkipInit(out _0024ArrayType_0024_0024_0024BY0FA_0040D _0024ArrayType_0024_0024_0024BY0FA_0040D);
		if (global::_003CModule_003E.PassThruGetLastError((sbyte*)(&_0024ArrayType_0024_0024_0024BY0FA_0040D)) == 0)
		{
			return new string((sbyte*)(&_0024ArrayType_0024_0024_0024BY0FA_0040D));
		}
		return null;
	}

	public unsafe static J2534Error WriteMsgs(uint channelId, IList<PassThruMsg> messages, uint timeout)
	{
		//IL_0062: Expected I, but got I8
		ulong num = (ulong)messages.Count;
		PASSTHRU_MSG* ptr = (PASSTHRU_MSG*)global::_003CModule_003E.new_005B_005D((num > 4442857435864535L) ? ulong.MaxValue : (num * 4152));
		int num2 = 0;
		if (0 < messages.Count)
		{
			do
			{
				PASSTHRU_MSG* ptr2 = messages[num2].Convert();
				if (global::_003CModule_003E._003FA0xb03e8ad9_002Ememcpy_s((void*)((long)num2 * 4152L + (nint)ptr), 4152uL, ptr2, 4152uL) == 0)
				{
					global::_003CModule_003E.delete(ptr2);
					num2++;
					continue;
				}
				throw new InvalidOperationException("Unable to copy write message");
			}
			while (num2 < messages.Count);
		}
		uint count = (uint)messages.Count;
		int result = global::_003CModule_003E.PassThruWriteMsgs(channelId, ptr, &count, timeout);
		global::_003CModule_003E.delete(ptr);
		return (J2534Error)result;
	}

	public unsafe static J2534Error ReadMsgs(uint channelId, IList<PassThruMsg> messages, uint messageCount, uint timeout)
	{
		//IL_0033: Expected I, but got I8
		PASSTHRU_MSG* ptr = (PASSTHRU_MSG*)global::_003CModule_003E.new_005B_005D((ulong)messageCount * 4152uL);
		uint num = messageCount;
		uint result = (uint)global::_003CModule_003E.PassThruReadMsgs(channelId, ptr, &num, timeout);
		uint num2 = 0u;
		if (0 < num)
		{
			do
			{
				messages.Add(new PassThruMsg((PASSTHRU_MSG*)((long)num2 * 4152L + (nint)ptr)));
				num2++;
			}
			while (num2 < num);
		}
		global::_003CModule_003E.delete(ptr);
		return (J2534Error)result;
	}

	public unsafe static J2534Error SetPassthruCallback(uint channelId, PassthruCallback function)
	{
		//IL_0020: Expected I, but got I8
		//IL_0024: Expected I8, but got I
		//IL_003a: Expected I, but got I8
		PassthruCallback[channelId] = function;
		delegate* unmanaged[Cdecl, Cdecl]<uint, uint, void*, void*, void> delegate_002A = (delegate* unmanaged[Cdecl, Cdecl]<uint, uint, void*, void*, void>)((!(function != null)) ? null : global::_003CModule_003E.__unep_0040_003FNativeCallbackFunction_0040_003FA0xb03e8ad9_0040_0040_0024_0024FYAXKKPEAX0_0040Z);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out CALLBACK_CONFIG cALLBACK_CONFIG);
		*(long*)(&cALLBACK_CONFIG) = (nint)delegate_002A;
		System.Runtime.CompilerServices.Unsafe.As<CALLBACK_CONFIG, long>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref cALLBACK_CONFIG, 8)) = 1L;
		return (J2534Error)global::_003CModule_003E.PassThruIoctl(channelId, 69323u, &cALLBACK_CONFIG, null);
	}

	public unsafe static J2534Error SetAllFiltersToPass(uint channelId, byte state)
	{
		//IL_000f: Expected I, but got I8
		return (J2534Error)global::_003CModule_003E.PassThruIoctl(channelId, 65711u, &state, null);
	}

	public unsafe static J2534Error SetBaudRate(uint channelId, uint baudRate)
	{
		//IL_0023: Expected I, but got I8
		System.Runtime.CompilerServices.Unsafe.SkipInit(out SCONFIG sCONFIG);
		*(int*)(&sCONFIG) = 1;
		System.Runtime.CompilerServices.Unsafe.As<SCONFIG, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref sCONFIG, 4)) = baudRate;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out SCONFIG_LIST sCONFIG_LIST);
		*(int*)(&sCONFIG_LIST) = 1;
		System.Runtime.CompilerServices.Unsafe.WriteUnaligned(ref System.Runtime.CompilerServices.Unsafe.As<SCONFIG_LIST, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref sCONFIG_LIST, 4)), (long)(nint)(&sCONFIG));
		return (J2534Error)global::_003CModule_003E.PassThruIoctl(channelId, 2u, &sCONFIG_LIST, null);
	}

	public unsafe static string GetDeviceName(uint channelId)
	{
		//IL_000c: Expected I8, but got I
		//IL_001b: Expected I, but got I8
		//IL_002e: Expected I, but got I8
		//IL_0026: Expected I, but got I8
		object result = null;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out RP1210_DEVICE_INFO rP1210_DEVICE_INFO);
		*(long*)(&rP1210_DEVICE_INFO) = (nint)global::_003CModule_003E.new_005B_005D(80uL);
		if (global::_003CModule_003E.PassThruIoctl(channelId, 69036u, null, &rP1210_DEVICE_INFO) == 0)
		{
			result = new string((sbyte*)(*(ulong*)(&rP1210_DEVICE_INFO)));
		}
		global::_003CModule_003E.delete((void*)(*(ulong*)(&rP1210_DEVICE_INFO)));
		return (string)result;
	}

	public unsafe static void GetDeviceVersionInfo(uint channelId, ref string libraryName, ref string libraryVersion, ref string driverVersion, ref string firmwareVersion, ref string supportedProtocols)
	{
		//IL_000b: Expected I8, but got I
		//IL_0018: Expected I8, but got I
		//IL_0026: Expected I8, but got I
		//IL_0034: Expected I8, but got I
		//IL_0042: Expected I8, but got I
		//IL_0051: Expected I, but got I8
		//IL_0099: Expected I, but got I8
		//IL_00a3: Expected I, but got I8
		//IL_00ae: Expected I, but got I8
		//IL_00b9: Expected I, but got I8
		//IL_00c4: Expected I, but got I8
		//IL_005c: Expected I, but got I8
		//IL_0068: Expected I, but got I8
		//IL_0074: Expected I, but got I8
		//IL_0082: Expected I, but got I8
		//IL_0090: Expected I, but got I8
		System.Runtime.CompilerServices.Unsafe.SkipInit(out RP1210_DEVICE_VERSION_INFO rP1210_DEVICE_VERSION_INFO);
		*(long*)(&rP1210_DEVICE_VERSION_INFO) = (nint)global::_003CModule_003E.new_005B_005D(80uL);
		System.Runtime.CompilerServices.Unsafe.As<RP1210_DEVICE_VERSION_INFO, long>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref rP1210_DEVICE_VERSION_INFO, 8)) = (nint)global::_003CModule_003E.new_005B_005D(80uL);
		System.Runtime.CompilerServices.Unsafe.As<RP1210_DEVICE_VERSION_INFO, long>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref rP1210_DEVICE_VERSION_INFO, 16)) = (nint)global::_003CModule_003E.new_005B_005D(80uL);
		System.Runtime.CompilerServices.Unsafe.As<RP1210_DEVICE_VERSION_INFO, long>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref rP1210_DEVICE_VERSION_INFO, 24)) = (nint)global::_003CModule_003E.new_005B_005D(80uL);
		System.Runtime.CompilerServices.Unsafe.As<RP1210_DEVICE_VERSION_INFO, long>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref rP1210_DEVICE_VERSION_INFO, 32)) = (nint)global::_003CModule_003E.new_005B_005D(80uL);
		if (global::_003CModule_003E.PassThruIoctl(channelId, 69035u, null, &rP1210_DEVICE_VERSION_INFO) == 0)
		{
			libraryName = new string((sbyte*)(*(ulong*)(&rP1210_DEVICE_VERSION_INFO)));
			libraryVersion = new string((sbyte*)System.Runtime.CompilerServices.Unsafe.As<RP1210_DEVICE_VERSION_INFO, ulong>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref rP1210_DEVICE_VERSION_INFO, 8)));
			driverVersion = new string((sbyte*)System.Runtime.CompilerServices.Unsafe.As<RP1210_DEVICE_VERSION_INFO, ulong>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref rP1210_DEVICE_VERSION_INFO, 8)));
			firmwareVersion = new string((sbyte*)System.Runtime.CompilerServices.Unsafe.As<RP1210_DEVICE_VERSION_INFO, ulong>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref rP1210_DEVICE_VERSION_INFO, 24)));
			supportedProtocols = new string((sbyte*)System.Runtime.CompilerServices.Unsafe.As<RP1210_DEVICE_VERSION_INFO, ulong>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref rP1210_DEVICE_VERSION_INFO, 32)));
		}
		global::_003CModule_003E.delete((void*)(*(ulong*)(&rP1210_DEVICE_VERSION_INFO)));
		global::_003CModule_003E.delete((void*)System.Runtime.CompilerServices.Unsafe.As<RP1210_DEVICE_VERSION_INFO, ulong>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref rP1210_DEVICE_VERSION_INFO, 8)));
		global::_003CModule_003E.delete((void*)System.Runtime.CompilerServices.Unsafe.As<RP1210_DEVICE_VERSION_INFO, ulong>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref rP1210_DEVICE_VERSION_INFO, 16)));
		global::_003CModule_003E.delete((void*)System.Runtime.CompilerServices.Unsafe.As<RP1210_DEVICE_VERSION_INFO, ulong>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref rP1210_DEVICE_VERSION_INFO, 24)));
		global::_003CModule_003E.delete((void*)System.Runtime.CompilerServices.Unsafe.As<RP1210_DEVICE_VERSION_INFO, ulong>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref rP1210_DEVICE_VERSION_INFO, 32)));
	}

	public unsafe static J2534Error GetBaudRate(uint channelId, ref uint baudRate)
	{
		//IL_0023: Expected I, but got I8
		System.Runtime.CompilerServices.Unsafe.SkipInit(out SCONFIG sCONFIG);
		*(int*)(&sCONFIG) = 1;
		System.Runtime.CompilerServices.Unsafe.As<SCONFIG, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref sCONFIG, 4)) = 0;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out SCONFIG_LIST sCONFIG_LIST);
		*(int*)(&sCONFIG_LIST) = 1;
		System.Runtime.CompilerServices.Unsafe.WriteUnaligned(ref System.Runtime.CompilerServices.Unsafe.As<SCONFIG_LIST, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref sCONFIG_LIST, 4)), (long)(nint)(&sCONFIG));
		J2534Error j2534Error = (J2534Error)global::_003CModule_003E.PassThruIoctl(channelId, 1u, &sCONFIG_LIST, null);
		if (j2534Error == J2534Error.NoError)
		{
			baudRate = System.Runtime.CompilerServices.Unsafe.As<SCONFIG, uint>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref sCONFIG, 4));
		}
		return j2534Error;
	}

	public unsafe static J2534Error IsAutoBaudRateCapable(uint channelId, ref bool isCapable)
	{
		//IL_0027: Expected I, but got I8
		System.Runtime.CompilerServices.Unsafe.SkipInit(out SCONFIG sCONFIG);
		*(int*)(&sCONFIG) = 65537;
		System.Runtime.CompilerServices.Unsafe.As<SCONFIG, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref sCONFIG, 4)) = 0;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out SCONFIG_LIST sCONFIG_LIST);
		*(int*)(&sCONFIG_LIST) = 1;
		System.Runtime.CompilerServices.Unsafe.WriteUnaligned(ref System.Runtime.CompilerServices.Unsafe.As<SCONFIG_LIST, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref sCONFIG_LIST, 4)), (long)(nint)(&sCONFIG));
		J2534Error j2534Error = (J2534Error)global::_003CModule_003E.PassThruIoctl(channelId, 1u, &sCONFIG_LIST, null);
		if (j2534Error == J2534Error.NoError)
		{
			int num = ((System.Runtime.CompilerServices.Unsafe.As<SCONFIG, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref sCONFIG, 4)) != 0) ? 1 : 0);
			isCapable = (byte)num != 0;
		}
		return j2534Error;
	}

	public unsafe static J2534Error GetErrorCode(uint channelId, ref uint errorCode)
	{
		//IL_000f: Expected I, but got I8
		System.Runtime.CompilerServices.Unsafe.SkipInit(out uint num);
		int result = global::_003CModule_003E.PassThruIoctl(channelId, 65772u, null, &num);
		errorCode = num;
		return (J2534Error)result;
	}

	public unsafe static J2534Error SetUseConnectionMutex(uint channelId, byte state)
	{
		//IL_0027: Expected I, but got I8
		System.Runtime.CompilerServices.Unsafe.SkipInit(out SCONFIG sCONFIG);
		*(int*)(&sCONFIG) = 65539;
		System.Runtime.CompilerServices.Unsafe.As<SCONFIG, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref sCONFIG, 4)) = state;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out SCONFIG_LIST sCONFIG_LIST);
		*(int*)(&sCONFIG_LIST) = 1;
		System.Runtime.CompilerServices.Unsafe.WriteUnaligned(ref System.Runtime.CompilerServices.Unsafe.As<SCONFIG_LIST, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref sCONFIG_LIST, 4)), (long)(nint)(&sCONFIG));
		return (J2534Error)global::_003CModule_003E.PassThruIoctl(channelId, 2u, &sCONFIG_LIST, null);
	}

	public unsafe static J2534Error SetProvideCustomMessageTypes(uint channelId, byte state)
	{
		//IL_0027: Expected I, but got I8
		System.Runtime.CompilerServices.Unsafe.SkipInit(out SCONFIG sCONFIG);
		*(int*)(&sCONFIG) = 65540;
		System.Runtime.CompilerServices.Unsafe.As<SCONFIG, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref sCONFIG, 4)) = state;
		System.Runtime.CompilerServices.Unsafe.SkipInit(out SCONFIG_LIST sCONFIG_LIST);
		*(int*)(&sCONFIG_LIST) = 1;
		System.Runtime.CompilerServices.Unsafe.WriteUnaligned(ref System.Runtime.CompilerServices.Unsafe.As<SCONFIG_LIST, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref sCONFIG_LIST, 4)), (long)(nint)(&sCONFIG));
		return (J2534Error)global::_003CModule_003E.PassThruIoctl(channelId, 2u, &sCONFIG_LIST, null);
	}

	public unsafe static J2534Error GetHardwareStatus(uint channelId, ref byte hardwareStatus, ref byte protocolStatusJ1939, ref byte protocolStatusJ1708, ref byte protocolStatusCan, ref byte protocolStatusIso15765)
	{
		//IL_000b: Expected I4, but got I8
		//IL_001a: Expected I, but got I8
		System.Runtime.CompilerServices.Unsafe.SkipInit(out RP1210_HARDWARE_STATUS rP1210_HARDWARE_STATUS);
		// IL initblk instruction
		System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ref rP1210_HARDWARE_STATUS, 0, 10);
		J2534Error j2534Error = (J2534Error)global::_003CModule_003E.PassThruIoctl(channelId, 69034u, null, &rP1210_HARDWARE_STATUS);
		if (j2534Error == J2534Error.NoError)
		{
			hardwareStatus = *(byte*)(&rP1210_HARDWARE_STATUS);
			protocolStatusJ1939 = System.Runtime.CompilerServices.Unsafe.As<RP1210_HARDWARE_STATUS, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref rP1210_HARDWARE_STATUS, 2));
			protocolStatusJ1708 = System.Runtime.CompilerServices.Unsafe.As<RP1210_HARDWARE_STATUS, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref rP1210_HARDWARE_STATUS, 4));
			protocolStatusCan = System.Runtime.CompilerServices.Unsafe.As<RP1210_HARDWARE_STATUS, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref rP1210_HARDWARE_STATUS, 6));
			protocolStatusIso15765 = System.Runtime.CompilerServices.Unsafe.As<RP1210_HARDWARE_STATUS, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref rP1210_HARDWARE_STATUS, 8));
		}
		return j2534Error;
	}

	public unsafe static J2534Error GetChannelInfo(PassThruMsg msg, ref uint channelId, ref RP1210ProtocolId rp1210ProtocolId, ref ushort physicalChannel, ref string rp1210ProtocolString)
	{
		//IL_0003: Expected I, but got I8
		//IL_001a: Expected I8, but got I
		//IL_0057: Expected I, but got I8
		//IL_004c: Expected I, but got I8
		PASSTHRU_MSG* ptr = null;
		if (msg != null)
		{
			ptr = msg.Convert();
		}
		System.Runtime.CompilerServices.Unsafe.SkipInit(out RP1210_CHANNEL_INFO rP1210_CHANNEL_INFO);
		System.Runtime.CompilerServices.Unsafe.As<RP1210_CHANNEL_INFO, long>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref rP1210_CHANNEL_INFO, 8)) = (nint)global::_003CModule_003E.new_005B_005D(80uL);
		J2534Error j2534Error = (J2534Error)global::_003CModule_003E.PassThruIoctl(channelId, 69033u, ptr, &rP1210_CHANNEL_INFO);
		if (j2534Error == J2534Error.NoError)
		{
			channelId = *(uint*)(&rP1210_CHANNEL_INFO);
			rp1210ProtocolId = (RP1210ProtocolId)System.Runtime.CompilerServices.Unsafe.As<RP1210_CHANNEL_INFO, ushort>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref rP1210_CHANNEL_INFO, 4));
			physicalChannel = System.Runtime.CompilerServices.Unsafe.As<RP1210_CHANNEL_INFO, ushort>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref rP1210_CHANNEL_INFO, 6));
			rp1210ProtocolString = new string((sbyte*)System.Runtime.CompilerServices.Unsafe.As<RP1210_CHANNEL_INFO, ulong>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref rP1210_CHANNEL_INFO, 8)));
		}
		global::_003CModule_003E.delete((void*)System.Runtime.CompilerServices.Unsafe.As<RP1210_CHANNEL_INFO, ulong>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref rP1210_CHANNEL_INFO, 8)));
		if (ptr != null)
		{
			global::_003CModule_003E.delete(ptr);
		}
		return j2534Error;
	}

	public unsafe static J2534Error SetAllowAutoBaudRate([MarshalAs(UnmanagedType.U1)] bool allowAutoBaudRate)
	{
		//IL_0027: Expected I, but got I8
		System.Runtime.CompilerServices.Unsafe.SkipInit(out SCONFIG sCONFIG);
		*(int*)(&sCONFIG) = 65537;
		System.Runtime.CompilerServices.Unsafe.As<SCONFIG, int>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref sCONFIG, 4)) = (allowAutoBaudRate ? 1 : 0);
		System.Runtime.CompilerServices.Unsafe.SkipInit(out SCONFIG_LIST sCONFIG_LIST);
		*(int*)(&sCONFIG_LIST) = 1;
		System.Runtime.CompilerServices.Unsafe.WriteUnaligned(ref System.Runtime.CompilerServices.Unsafe.As<SCONFIG_LIST, byte>(ref System.Runtime.CompilerServices.Unsafe.AddByteOffset(ref sCONFIG_LIST, 4)), (long)(nint)(&sCONFIG));
		return (J2534Error)global::_003CModule_003E.PassThruIoctl(0u, 2u, &sCONFIG_LIST, null);
	}

	public unsafe static J2534Error GetEthernetGuid(uint channelId, ref string guid)
	{
		//IL_0017: Expected I, but got I8
		sbyte* ptr = (sbyte*)global::_003CModule_003E.new_005B_005D(80uL);
		J2534Error j2534Error = (J2534Error)global::_003CModule_003E.PassThruIoctl(channelId, 69032u, null, ptr);
		if (j2534Error == J2534Error.NoError)
		{
			guid = new string(ptr);
		}
		global::_003CModule_003E.delete(ptr);
		return j2534Error;
	}

	private Sid()
	{
	}
}
