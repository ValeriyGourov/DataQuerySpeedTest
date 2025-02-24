using AutoFixture;

using Mediator;

using Server.Data;

namespace Server.Operations;

#pragma warning disable CA1515
public readonly record struct GetAllQuery(ushort PageSize = 20) : IQuery<IEnumerable<Order>>;
#pragma warning restore CA1515

#pragma warning disable CA1812
internal sealed class GetAllQueryHandler : IQueryHandler<GetAllQuery, IEnumerable<Order>>
{
	private static readonly Fixture _fixture = new();

	public async ValueTask<IEnumerable<Order>> Handle(
		GetAllQuery query,
		CancellationToken cancellationToken)
	{
		await Task
			.Delay(150, cancellationToken)
			.ConfigureAwait(false);

		return _fixture.CreateMany<Order>(query.PageSize);
	}
}
#pragma warning restore CA1812
