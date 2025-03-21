using AutoFixture;

using DataQuerySpeedTest.ServiceDefaults.Models;

using Mediator;

namespace Server.Operations;

#pragma warning disable CA1812
internal sealed class GetQueryHandler : IQueryHandler<GetQuery, Order>
{
	private static readonly Fixture _fixture = new();

	public ValueTask<Order> Handle(
		GetQuery query,
		CancellationToken cancellationToken)
	{
		Order order = _fixture
			.Build<Order>()
			.With(order => order.Id, query.Id)
			.Create();
		return ValueTask.FromResult(order);
	}
}
#pragma warning restore CA1812
