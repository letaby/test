// Decompiled with JetBrains decompiler
// Type: DetroitDiesel.DataHub.XtrFile
// Assembly: DataHub, Version=8.19.5842.0, Culture=neutral, PublicKeyToken=1d4aea3187b835fe
// MVID: 89346980-C6E7-48B1-88DD-CF29796E810E
// Assembly location: C:\Users\petra\Downloads\Архив (2)\DataHub.dll

using DetroitDiesel.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

#nullable disable
namespace DetroitDiesel.DataHub;

public class XtrFile
{
  private const byte DeviceMid = 128 /*0x80*/;
  private const byte MessageType = 3;
  private const byte RequestType = 4;
  private readonly string deviceIdentifier;
  private readonly string softwareVersion;
  private List<DataPage> dataPages;

  public XtrFile(string deviceIdentifier, string softwareVersion)
  {
    this.dataPages = new List<DataPage>();
    this.deviceIdentifier = deviceIdentifier;
    this.softwareVersion = softwareVersion;
  }

  private static int GetTimestamp()
  {
    return (int) (DateTime.UtcNow - new DateTime(1985, 1, 1, 0, 0, 0, 0)).TotalSeconds;
  }

  public static short Crc16(byte[] data, int offset, int count)
  {
    short num = -1;
    for (int index1 = offset; index1 < offset + count; ++index1)
    {
      for (int index2 = 0; index2 < 8; ++index2)
      {
        if ((((int) data[index1] << index2 + 8 ^ (int) num) & 32768 /*0x8000*/) != 0)
          num = (short) ((int) num << 1 ^ 4129);
        else
          num <<= 1;
      }
    }
    return num;
  }

  internal void AddDataPage(DataPage dataPage)
  {
    if (dataPage == null)
      return;
    this.dataPages.Add(dataPage);
  }

  private void Encode(BinaryWriter writer)
  {
    writer.Write(Encoding.ASCII.GetBytes("DDEC1"));
    writer.Write((byte) 0);
    writer.Write((short) 1);
    long position1 = writer.BaseStream.Position;
    writer.Write(0);
    byte[] bytes1 = Encoding.ASCII.GetBytes(this.deviceIdentifier);
    writer.Write((byte) (bytes1.Length + 1));
    writer.Write((byte) 128 /*0x80*/);
    writer.Write(bytes1);
    byte[] bytes2 = Encoding.ASCII.GetBytes(this.softwareVersion ?? string.Empty);
    writer.Write((byte) bytes2.Length);
    writer.Write(bytes2);
    writer.Write(XtrFile.GetTimestamp());
    writer.Write((byte) 3);
    using (MemoryStream output = new MemoryStream())
    {
      using (BinaryWriter writer1 = new BinaryWriter((Stream) output))
      {
        writer1.Write((byte) this.dataPages.Count);
        writer1.Write((byte) 4);
        foreach (DataPage dataPage in this.dataPages)
          dataPage.Encode(writer1);
        byte[] array = output.ToArray();
        writer.Write(array);
        short num = XtrFile.Crc16(array, 0, array.Length);
        writer.Write(num);
      }
    }
    long position2 = writer.BaseStream.Position;
    writer.BaseStream.Seek(position1, SeekOrigin.Begin);
    writer.Write((uint) ((ulong) (position2 - position1) - 4UL));
    writer.BaseStream.Seek(position2, SeekOrigin.Begin);
  }

  public void Save(string fileName, bool encrypt)
  {
    if (encrypt)
    {
      using (MemoryStream output = new MemoryStream())
      {
        using (BinaryWriter writer = new BinaryWriter((Stream) output))
          this.Encode(writer);
        FileEncryptionProvider.WriteEncryptedFile(output.ToArray(), fileName, (EncryptionType) 1);
      }
    }
    else
    {
      using (FileStream output = new FileStream(fileName, FileMode.Create, FileAccess.Write))
      {
        using (BinaryWriter writer = new BinaryWriter((Stream) output))
          this.Encode(writer);
      }
    }
  }
}
