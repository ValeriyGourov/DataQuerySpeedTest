using Mediator;

using MessagePack;

namespace DataQuerySpeedTest.ServiceDefaults.Models;

[MessagePackObject]
public readonly record struct GetAllQuery(
	[property: Key(0)] ushort PageSize = 20)
	: IQuery<IEnumerable<Order>>;
