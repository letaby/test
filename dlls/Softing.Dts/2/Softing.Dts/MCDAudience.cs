using System;

namespace Softing.Dts;

public interface MCDAudience : MCDObject, IDisposable
{
	bool IsSupplier { get; }

	bool IsDevelopment { get; }

	bool IsManufacturing { get; }

	bool IsAfterSales { get; }

	bool IsAfterMarket { get; }
}
