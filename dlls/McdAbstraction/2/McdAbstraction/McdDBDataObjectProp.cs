using System;
using System.Collections.Generic;
using System.Linq;
using Softing.Dts;

namespace McdAbstraction;

public class McdDBDataObjectProp
{
	private DtsDbDataObjectProp dataObjectProp;

	private string qualifier;

	private string name;

	private Type codedParameterType;

	private bool? isHighLowByteOrder;

	private List<McdDBCompuScale> scaleEntries;

	public string Qualifier => qualifier;

	public string Name
	{
		get
		{
			if (name == null)
			{
				name = dataObjectProp.LongName;
			}
			return name;
		}
	}

	public Type CodedType
	{
		get
		{
			if (codedParameterType == null)
			{
				codedParameterType = McdRoot.MapDataType(dataObjectProp.CodedType);
			}
			return codedParameterType;
		}
	}

	public bool IsHighLowByteOrder
	{
		get
		{
			if (!isHighLowByteOrder.HasValue)
			{
				isHighLowByteOrder = dataObjectProp.IsHighlowByteOrder;
			}
			return isHighLowByteOrder.Value;
		}
	}

	public IEnumerable<McdDBCompuScale> ScaleEntries
	{
		get
		{
			if (scaleEntries == null)
			{
				scaleEntries = new List<McdDBCompuScale>();
				if (dataObjectProp.IsCompuMethodValid)
				{
					DtsDbCompuMethod compuMethod = dataObjectProp.CompuMethod;
					if (compuMethod != null && compuMethod.IsCompuInternalToPhysValid)
					{
						string categoryName = compuMethod.CategoryName;
						if (categoryName == "Linear" || categoryName == "ScaleLinear")
						{
							scaleEntries.AddRange(from cs in compuMethod.CompuInternalToPhys.DbCompuScales.OfType<DtsDbCompuScale>()
								select new McdDBCompuScale(cs));
						}
					}
				}
			}
			return scaleEntries;
		}
	}

	internal McdDBDataObjectProp(DtsDbDataObjectProp dataObjectProp)
	{
		this.dataObjectProp = dataObjectProp;
		qualifier = this.dataObjectProp.ShortName;
	}
}
