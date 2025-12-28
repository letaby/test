using System;
using System.Collections.Generic;
using System.Linq;
using CaesarAbstraction;
using McdAbstraction;

namespace SapiLayer1;

public sealed class FlashMeaning
{
	private FlashArea flashArea;

	private string description;

	private string name;

	private string flashKey;

	private uint priority;

	private uint index;

	private uint areaIndex;

	private string fileName;

	private FlashDataBlockCollection flashDataBlocks;

	private IEnumerable<Ecu> allowedEcus;

	private string lateBoundFileName;

	private string flashJobName;

	public FlashArea FlashArea => flashArea;

	public string Name => name;

	public string Description => description;

	public string FlashKey => flashKey;

	public int Priority => (int)priority;

	public int Index => (int)index;

	public int AreaIndex => (int)areaIndex;

	public string FileName => fileName;

	public string FlashJobName => flashJobName;

	public IEnumerable<Ecu> AllowedEcus => allowedEcus;

	public FlashDataBlockCollection FlashDataBlocks => flashDataBlocks;

	public string LateBoundContentFileName => lateBoundFileName;

	internal FlashMeaning(FlashArea fa, uint areaIndex, uint index, string fileName)
	{
		name = string.Empty;
		flashKey = string.Empty;
		flashArea = fa;
		this.index = index;
		this.areaIndex = areaIndex;
		description = string.Empty;
		this.fileName = fileName;
		flashDataBlocks = new FlashDataBlockCollection();
	}

	internal void Acquire(McdDBFlashSession flashSession, McdDBFlashDataBlock dataBlock, IEnumerable<string> allowedEcus)
	{
		priority = (uint)flashSession.Priority;
		flashKey = flashSession.FlashKey;
		this.allowedEcus = allowedEcus.Select((string e) => Sapi.GetSapi().Ecus.FirstOrDefault((Ecu ecu) => ecu.IsMcd && ecu.Name == e));
		name = flashSession.Name;
		fileName = dataBlock.DatabaseFileName;
		flashJobName = flashSession.FlashJobName;
		FlashDataBlock flashDataBlock = new FlashDataBlock(this, index, 0u);
		flashDataBlock.Acquire(dataBlock);
		flashDataBlocks.Add(flashDataBlock);
	}

	internal void Acquire(CaesarDIFlashTable flashTable)
	{
		//IL_00f8: Expected O, but got Unknown
		name = flashTable.Name;
		description = flashTable.Description;
		flashKey = flashTable.FlashKey;
		priority = flashTable.Priority;
		flashJobName = flashTable.FlashService;
		List<Ecu> list = new List<Ecu>();
		for (uint num = 0u; num < flashTable.AllowedEcuCount; num++)
		{
			string allowedEcuByIndex = flashTable.GetAllowedEcuByIndex(num);
			Ecu ecu = Sapi.GetSapi().Ecus[allowedEcuByIndex];
			if (ecu != null)
			{
				list.Add(ecu);
			}
		}
		allowedEcus = list.AsReadOnly();
		uint flashDataBlockCount = flashTable.FlashDataBlockCount;
		lateBoundFileName = flashTable.GetDataBlockFileName((ushort)0);
		for (ushort num2 = 0; num2 < flashDataBlockCount; num2++)
		{
			try
			{
				CaesarDIFlashDataBlock flashDataBlock = flashTable.GetFlashDataBlock(num2);
				try
				{
					FlashDataBlock flashDataBlock2 = new FlashDataBlock(this, index, num2);
					flashDataBlock2.Acquire(flashDataBlock, lateBoundFileName != null);
					flashDataBlocks.Add(flashDataBlock2);
				}
				finally
				{
					((IDisposable)flashDataBlock)?.Dispose();
				}
			}
			catch (CaesarErrorException ex)
			{
				CaesarException e = new CaesarException(ex, null, null);
				Sapi.GetSapi().RaiseExceptionEvent(this, e);
			}
		}
	}
}
