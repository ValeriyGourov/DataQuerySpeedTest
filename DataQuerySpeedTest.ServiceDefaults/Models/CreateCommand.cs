using Mediator;

using MessagePack;

namespace DataQuerySpeedTest.ServiceDefaults.Models;

[MessagePackObject]
public sealed class CreateCommand : ICommand
{
	[property: Key(0)]
	public required string ProductName { get; init; }

	[property: Key(1)]
	public required decimal Quantity { get; init; }
}
