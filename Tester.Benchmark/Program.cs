using System.Diagnostics;

using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

using Tester.Benchmark;

IConfig? config = Debugger.IsAttached
	? new DebugInProcessConfig()
	: null;

_ = BenchmarkRunner.Run<Benchmarks>(config);
