using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DetroitDiesel.Security.Cryptography;

namespace DetroitDiesel.DataHub;

public class XtrFile
{
	private const byte DeviceMid = 128;

	private const byte MessageType = 3;

	private const byte RequestType = 4;

	private readonly string deviceIdentifier;

	private readonly string softwareVersion;

	private List<DataPage> dataPages;

	public XtrFile(string deviceIdentifier, string softwareVersion)
	{
		dataPages = new List<DataPage>();
		this.deviceIdentifier = deviceIdentifier;
		this.softwareVersion = softwareVersion;
	}

	private static int GetTimestamp()
	{
		return (int)(DateTime.UtcNow - new DateTime(1985, 1, 1, 0, 0, 0, 0)).TotalSeconds;
	}

	public static short Crc16(byte[] data, int offset, int count)
	{
		short num = -1;
		for (int i = offset; i < offset + count; i++)
		{
			for (int j = 0; j < 8; j++)
			{
				num = (((((data[i] << j + 8) ^ num) & 0x8000) == 0) ? ((short)(num << 1)) : ((short)((num << 1) ^ 0x1021)));
			}
		}
		return num;
	}

	internal void AddDataPage(DataPage dataPage)
	{
		if (dataPage != null)
		{
			dataPages.Add(dataPage);
		}
	}

	private void Encode(BinaryWriter writer)
	{
		writer.Write(Encoding.ASCII.GetBytes("DDEC1"));
		writer.Write((byte)0);
		writer.Write((short)1);
		long position = writer.BaseStream.Position;
		writer.Write(0);
		byte[] bytes = Encoding.ASCII.GetBytes(deviceIdentifier);
		writer.Write((byte)(bytes.Length + 1));
		writer.Write((byte)128);
		writer.Write(bytes);
		byte[] bytes2 = Encoding.ASCII.GetBytes(softwareVersion ?? string.Empty);
		writer.Write((byte)bytes2.Length);
		writer.Write(bytes2);
		writer.Write(GetTimestamp());
		writer.Write((byte)3);
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
			binaryWriter.Write((byte)dataPages.Count);
			binaryWriter.Write((byte)4);
			foreach (DataPage dataPage in dataPages)
			{
				dataPage.Encode(binaryWriter);
			}
			byte[] array = memoryStream.ToArray();
			writer.Write(array);
			short value = Crc16(array, 0, array.Length);
			writer.Write(value);
		}
		long position2 = writer.BaseStream.Position;
		writer.BaseStream.Seek(position, SeekOrigin.Begin);
		writer.Write((uint)(position2 - position - 4));
		writer.BaseStream.Seek(position2, SeekOrigin.Begin);
	}

	public void Save(string fileName, bool encrypt)
	{
		if (encrypt)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter writer = new BinaryWriter(memoryStream))
				{
					Encode(writer);
				}
				FileEncryptionProvider.WriteEncryptedFile(memoryStream.ToArray(), fileName, (EncryptionType)1);
				return;
			}
		}
		using FileStream output = new FileStream(fileName, FileMode.Create, FileAccess.Write);
		using BinaryWriter writer2 = new BinaryWriter(output);
		Encode(writer2);
	}
}
