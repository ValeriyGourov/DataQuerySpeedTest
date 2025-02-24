using Mediator;

namespace Server.Operations;

#pragma warning disable CA1515
public readonly record struct CreateCommand(
	string ProductName,
	decimal Quantity)
	: ICommand;
#pragma warning restore CA1515

#pragma warning disable CA1812
internal sealed class CreateCommandHandler : ICommandHandler<CreateCommand>
{
	public async ValueTask<Unit> Handle(
		CreateCommand command,
		CancellationToken cancellationToken)
	{
		await Task
			.Delay(200, cancellationToken)
			.ConfigureAwait(false);

		return Unit.Value;
	}
}
#pragma warning restore CA1812
