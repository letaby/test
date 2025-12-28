using System;
using System.Collections.Generic;
using System.IO;
using CaesarAbstraction;
using McdAbstraction;

namespace SapiLayer1;

public sealed class FlashArea
{
	private Channel channel;

	private string qualifier;

	private string name;

	private string description;

	private FlashMeaningCollection flashMeanings;

	private FlashMeaning marked;

	public Channel Channel => channel;

	public string Qualifier => qualifier;

	public string Name => name;

	public string Description => description;

	public FlashMeaningCollection FlashMeanings => flashMeanings;

	public FlashMeaning Marked
	{
		get
		{
			return marked;
		}
		set
		{
			marked = value;
		}
	}

	internal FlashArea(Channel ch, string qualifier)
	{
		name = string.Empty;
		this.qualifier = qualifier;
		description = string.Empty;
		channel = ch;
		flashMeanings = new FlashMeaningCollection();
	}

	internal void Acquire(McdDBFlashSession flashSession, IEnumerable<string> allowedEcus, uint areaIndex)
	{
		name = flashSession.Name;
		description = flashSession.Description;
		uint num = 0u;
		foreach (McdDBFlashDataBlock dBDataBlock in flashSession.DBDataBlocks)
		{
			if (File.Exists(dBDataBlock.DatabaseFileName))
			{
				FlashMeaning flashMeaning = new FlashMeaning(this, areaIndex, num++, null);
				flashMeaning.Acquire(flashSession, dBDataBlock, allowedEcus);
				flashMeanings.Add(flashMeaning);
			}
		}
	}

	internal void Acquire(CaesarDIFlashArea flashArea, uint areaIndex)
	{
		//IL_006c: Expected O, but got Unknown
		name = flashArea.Name;
		description = flashArea.Description;
		uint flashTableCount = flashArea.FlashTableCount;
		for (ushort num = 0; num < flashTableCount; num++)
		{
			try
			{
				CaesarDIFlashTable flashTable = flashArea.GetFlashTable(num);
				try
				{
					FlashMeaning flashMeaning = new FlashMeaning(this, areaIndex, num, flashArea.FileNameAndPath);
					flashMeaning.Acquire(flashTable);
					flashMeanings.Add(flashMeaning);
				}
				finally
				{
					((IDisposable)flashTable)?.Dispose();
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
