using MessagePack;

namespace DataQuerySpeedTest.ServiceDefaults.Models;

[MessagePackObject]
public readonly record struct Order(
	[property: Key(0)] int Id,
	[property: Key(1)] string ProductName,
	[property: Key(2)] decimal Price,
	[property: Key(3)] decimal Quantity,
	[property: Key(4)] decimal Amount);
