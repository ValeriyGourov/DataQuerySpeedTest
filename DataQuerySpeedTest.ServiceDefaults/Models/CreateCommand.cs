using Mediator;

using MessagePack;

namespace DataQuerySpeedTest.ServiceDefaults.Models;

[MessagePackObject]
public readonly record struct CreateCommand(
	[property: Key(0)] string ProductName,
	[property: Key(1)] decimal Quantity)
	: ICommand;
