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

	public async ValueTask<Order> Handle(
		GetQuery query,
		CancellationToken cancellationToken)
	{
		await Task
			.Delay(100, cancellationToken)
			.ConfigureAwait(false);

		return _fixture
			.Build<Order>()
			.With(order => order.Id, query.Id)
			.Create();
	}
}
#pragma warning restore CA1812
