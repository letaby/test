using Softing.Dts;

namespace McdAbstraction;

public class McdDBFlashDataBlock
{
	private MCDDbFlashDataBlock dataBlock;

	private string qualifier;

	private string name;

	private string fileName;

	private string dataFileName;

	private string databaseFileName;

	private string dataBlockType;

	private uint numberSecurities;

	private uint numberSegments;

	private string description;

	public string DataFileName => dataFileName;

	public string FileName => fileName;

	public string DatabaseFileName => databaseFileName;

	public string Qualifier => qualifier;

	public string Name => name;

	public string DataBlockType => dataBlockType;

	public long NumberOfSecurities => numberSecurities;

	public long NumberOfSegments => numberSegments;

	public string Description => description;

	internal McdDBFlashDataBlock(MCDDbFlashDataBlock dataBlock)
	{
		name = dataBlock.LongName;
		qualifier = dataBlock.ShortName;
		fileName = dataBlock.DbFlashData.ActiveFileName;
		dataFileName = dataBlock.DbFlashData.DataFileName;
		databaseFileName = ((DtsDbFlashData)dataBlock.DbFlashData).DatabaseFileName;
		numberSecurities = dataBlock.DbSecurities.Count;
		numberSegments = dataBlock.DbFlashSegments.Count;
		dataBlockType = dataBlock.DataBlockType;
		description = dataBlock.Description;
		this.dataBlock = dataBlock;
	}

	public McdDBFlashSecurity GetFlashSecurity(int index)
	{
		return new McdDBFlashSecurity(dataBlock.DbSecurities.GetItemByIndex((uint)index));
	}

	public McdDBFlashSegment GetFlashSegment(int index)
	{
		return new McdDBFlashSegment(dataBlock.DbFlashSegments.GetItemByIndex((uint)index));
	}
}
