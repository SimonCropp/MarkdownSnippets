using BenchmarkDotNet.Running;

var switcher = BenchmarkSwitcher.FromTypes(
[
    typeof(FullRenderBenchmarks),
    typeof(WholeFileSnippetBenchmark),
    typeof(SnippetAsIncludeBenchmark),
    typeof(ValidateContentBenchmark),
    typeof(FilesToSnippetsLookupBenchmark)
]);
switcher.Run(args);
