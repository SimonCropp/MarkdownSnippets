using BenchmarkDotNet.Running;

var switcher = BenchmarkSwitcher.FromTypes(
[
    typeof(FullRenderBenchmarks),
    typeof(WholeFileSnippetBenchmark),
    typeof(SnippetAsIncludeBenchmark),
    typeof(ValidateContentBenchmark),
    typeof(FilesToSnippetsLookupBenchmark),
    typeof(ClosurePerSnippetLineBenchmark),
    typeof(ReadSnippetsBenchmark),
    typeof(SnippetMarkdownHandlingBenchmark),
    typeof(MarkdownProcessorCtorBenchmark),
    typeof(StartEndTesterBenchmark),
    typeof(GetLanguageFromPathBenchmark),
    typeof(IndentedMarkdownApplyBenchmark)
]);
switcher.Run(args);
