using Mediator;

using MessagePack;

namespace DataQuerySpeedTest.ServiceDefaults.Models;

[MessagePackObject]
public readonly record struct GetQuery(
	[property: Key(0)] int Id)
	: IQuery<Order>;
