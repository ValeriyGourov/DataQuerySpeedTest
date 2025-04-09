namespace Tester.Core.Modules;

public interface IModule
{
	ValueTask<long?> ExecuteGetAsync(CancellationToken cancellationToken);
	ValueTask<long?> ExecuteGetAllAsync(CancellationToken cancellationToken);
	ValueTask<long?> ExecuteCreateAsync(CancellationToken cancellationToken);
}
