using AutoFixture;

using Mediator;

using Server.Data;

namespace Server.Operations;

#pragma warning disable CA1515
public readonly record struct GetQuery(int Id) : IQuery<Order>;
#pragma warning restore CA1515

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
