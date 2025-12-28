using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace J2534;

public class PassThruMsg
{
	private ProtocolId m_ProtocolId;

	private uint m_RxStatus;

	private uint m_TxFlags;

	private uint m_Timestamp;

	private uint m_DataSize;

	private uint m_ExtraDataIndex;

	private byte[] m_Data;

	public bool IsBaudRate
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return (byte)((m_RxStatus >> 26) & 1) != 0;
		}
	}

	public bool IsErrorFrame
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return (byte)((m_RxStatus >> 25) & 1) != 0;
		}
	}

	public bool IsChipState
	{
		[return: MarshalAs(UnmanagedType.U1)]
		get
		{
			return (byte)((m_RxStatus >> 24) & 1) != 0;
		}
	}

	public long ExtraDataIndex => m_ExtraDataIndex;

	public long DataSize => m_DataSize;

	public long Timestamp => m_Timestamp;

	public long TXFlags => m_TxFlags;

	public long RXStatus => m_RxStatus;

	public ProtocolId ProtcolId => m_ProtocolId;

	internal unsafe PassThruMsg(PASSTHRU_MSG* source)
	{
		m_ProtocolId = System.Runtime.CompilerServices.Unsafe.ReadUnaligned<ProtocolId>(source);
		m_RxStatus = System.Runtime.CompilerServices.Unsafe.ReadUnaligned<uint>((void*)((long)(nint)source + 4L));
		m_TxFlags = System.Runtime.CompilerServices.Unsafe.ReadUnaligned<uint>((void*)((long)(nint)source + 8L));
		m_Timestamp = System.Runtime.CompilerServices.Unsafe.ReadUnaligned<uint>((void*)((long)(nint)source + 12L));
		uint num = (m_DataSize = System.Runtime.CompilerServices.Unsafe.ReadUnaligned<uint>((void*)((long)(nint)source + 16L)));
		m_ExtraDataIndex = System.Runtime.CompilerServices.Unsafe.ReadUnaligned<uint>((void*)((long)(nint)source + 20L));
		byte[] array = (m_Data = new byte[num]);
		uint num2 = 0u;
		if (0 < num)
		{
			uint num3 = num;
			do
			{
				array[num2] = *(byte*)((nint)source + num2 + 24);
				num2++;
			}
			while (num2 < num3);
		}
	}

	public PassThruMsg(ProtocolId protocolId, uint rxStatus, uint transmitOptions, uint timestamp, uint extraDataIndex, byte[] data)
	{
		if (data == null)
		{
			throw new ArgumentNullException("data");
		}
		m_ProtocolId = protocolId;
		m_RxStatus = rxStatus;
		m_TxFlags = transmitOptions;
		m_Timestamp = timestamp;
		m_DataSize = (uint)data.Length;
		m_ExtraDataIndex = extraDataIndex;
		m_Data = data;
	}

	public PassThruMsg(ProtocolId protocolId, byte[] data)
	{
		if (data == null)
		{
			throw new ArgumentNullException("data");
		}
		m_ProtocolId = protocolId;
		m_RxStatus = 0u;
		m_TxFlags = 0u;
		m_Timestamp = 0u;
		m_ExtraDataIndex = 0u;
		m_DataSize = (uint)data.Length;
		m_Data = data;
	}

	internal unsafe PASSTHRU_MSG* Convert()
	{
		//IL_0027: Expected I, but got I8
		//IL_001f: Expected I4, but got I8
		//IL_008f: Expected I, but got I8
		//IL_009d: Expected I, but got I8
		PASSTHRU_MSG* ptr = (PASSTHRU_MSG*)global::_003CModule_003E.@new(4152uL);
		PASSTHRU_MSG* ptr2;
		if (ptr != null)
		{
			// IL initblk instruction
			System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(ptr, 0, 4152);
			ptr2 = ptr;
		}
		else
		{
			ptr2 = null;
		}
		System.Runtime.CompilerServices.Unsafe.WriteUnaligned(ptr2, m_ProtocolId);
		System.Runtime.CompilerServices.Unsafe.WriteUnaligned((void*)((long)(nint)ptr2 + 4L), m_RxStatus);
		System.Runtime.CompilerServices.Unsafe.WriteUnaligned((void*)((long)(nint)ptr2 + 8L), m_TxFlags);
		System.Runtime.CompilerServices.Unsafe.WriteUnaligned((void*)((long)(nint)ptr2 + 16L), m_DataSize);
		System.Runtime.CompilerServices.Unsafe.WriteUnaligned((void*)((long)(nint)ptr2 + 12L), m_Timestamp);
		System.Runtime.CompilerServices.Unsafe.WriteUnaligned((void*)((long)(nint)ptr2 + 20L), m_ExtraDataIndex);
		int num = 0;
		byte[] data = m_Data;
		if (0 < (nint)data.LongLength)
		{
			PASSTHRU_MSG* ptr3 = (PASSTHRU_MSG*)((ulong)(nint)ptr2 + 24uL);
			do
			{
				*(byte*)ptr3 = data[num];
				num++;
				ptr3 = (PASSTHRU_MSG*)((ulong)(nint)ptr3 + 1uL);
				data = m_Data;
			}
			while (num < (nint)data.LongLength);
		}
		return ptr2;
	}

	public byte[] GetData()
	{
		return m_Data;
	}
}
