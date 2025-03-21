using DataQuerySpeedTest.ServiceDefaults.Models;

using Mediator;

namespace Server.Operations;

#pragma warning disable CA1812
internal sealed class CreateCommandHandler : ICommandHandler<CreateCommand>
{
	public ValueTask<Unit> Handle(
		CreateCommand command,
		CancellationToken cancellationToken)
		=> ValueTask.FromResult(Unit.Value);
}
#pragma warning restore CA1812
