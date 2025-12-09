using System.Diagnostics;

using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

using Tester.Benchmark;

IConfig config = Debugger.IsAttached
	? new DebugInProcessConfig()
	: DefaultConfig.Instance;

config = config.WithSummaryStyle(config.SummaryStyle.WithMaxParameterColumnWidth(35));

_ = BenchmarkRunner.Run<Benchmarks>(config);
