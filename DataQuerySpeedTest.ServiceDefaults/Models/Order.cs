using MessagePack;

namespace DataQuerySpeedTest.ServiceDefaults.Models;

[MessagePackObject]
public sealed class Order
{
	[Key(0)]
	public required int Id { get; init; }

	[Key(1)]
	public required string ProductName { get; init; }

	[Key(2)]
	public required decimal Price { get; init; }

	[Key(3)]
	public required decimal Quantity { get; init; }

	[Key(4)]
	public required decimal Amount { get; init; }
}
