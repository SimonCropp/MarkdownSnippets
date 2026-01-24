// Integration tests for MSBuild task
//
// These tests verify that the MarkdownSnippets.MsBuild NuGet package works correctly
// when consumed by a project using both .NET Core (dotnet build) and .NET Framework (msbuild.exe).
//
// How it works:
// 1. Creates a temporary directory with a minimal test project
// 2. Configures nuget.config to use the local nugets folder (C:\Code\MarkdownSnippets\nugets)
//    with a local packages cache to avoid global NuGet cache issues
// 3. Creates a .csproj referencing MarkdownSnippets.MsBuild
// 4. Creates a C# file with a code snippet and a markdown file referencing it
// 5. Runs the build (dotnet or msbuild.exe) which triggers the MarkdownSnippets task
// 6. Verifies the markdown was processed correctly
//
// The .NET Framework test (msbuild.exe) is particularly important because:
// - MSBuild loads the netstandard2.0 version of the task DLL
// - All dependencies (including System.Collections.Immutable with FrozenSet) are shaded
//   via PackageShader to avoid version conflicts with MSBuild's own dependencies
// - Static field data (FieldRVA entries) must be correctly patched when shading
//
// These tests only run in RELEASE configuration because they depend on the
// MarkdownSnippets.MsBuild.nupkg being built in the nugets folder.
#if RELEASE
public class MsBuildIntegrationTests
{
    static string GetNugetsDir()
    {
        var solutionDir = ProjectFiles.SolutionDirectory.Path.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        // SolutionDirectory is C:\Code\MarkdownSnippets\src, nugets is at C:\Code\MarkdownSnippets\nugets
        var repoDir = Directory.GetParent(solutionDir)?.FullName
                      ?? throw new InvalidOperationException($"Could not get parent of {solutionDir}");
        return Path.Combine(repoDir, "nugets");
    }

    [Fact]
    public async Task DotnetBuild_UsesNetCoreTask()
    {
        using var tempDir = new TempDirectory();
        await SetupTestProject(tempDir);

        var result = await RunProcess("dotnet", $"build \"{tempDir}\" -c Release -nodeReuse:false", tempDir);

        Assert.True(result.ExitCode == 0, $"dotnet build failed:\n{result.Output}\n{result.Error}");

        // Verify the markdown was processed (generated header indicates task ran)
        var outputMd = Path.Combine(tempDir, "docs", "readme.md");
        Assert.True(File.Exists(outputMd), $"Output markdown should exist at {outputMd}");
        var content = await File.ReadAllTextAsync(outputMd);
        Assert.Contains("GENERATED FILE", content);
    }

    [Fact]
    public async Task MsBuild_UsesNetFrameworkTask()
    {
        var msbuildPath = FindMsBuild();
        if (msbuildPath == null)
        {
            // Skip if msbuild.exe not found
            return;
        }

        using var tempDir = new TempDirectory();
        await SetupTestProject(tempDir);

        var result = await RunProcess(msbuildPath, $"\"{tempDir}\" /p:Configuration=Release /restore /nodeReuse:false -verbosity:minimal", tempDir);

        Assert.True(result.ExitCode == 0, $"msbuild failed:\n{result.Output}\n{result.Error}");

        // Verify the markdown was processed (generated header indicates task ran)
        var outputMd = Path.Combine(tempDir, "docs", "readme.md");
        Assert.True(File.Exists(outputMd), $"Output markdown should exist at {outputMd}");
        var content = await File.ReadAllTextAsync(outputMd);
        Assert.Contains("GENERATED FILE", content);
    }

    static async Task SetupTestProject(TempDirectory tempDir)
    {
        // Find the latest nuget version
        var nugetVersion = GetLatestNugetVersion();

        // Create nuget.config pointing to local nugets folder
        // Use a local packages folder to avoid global cache issues when testing local package changes
        var localPackagesFolder = Path.Combine(tempDir, "packages");
        var nugetConfig = $"""
                           <?xml version="1.0" encoding="utf-8"?>
                           <configuration>
                             <config>
                               <add key="globalPackagesFolder" value="{localPackagesFolder}" />
                             </config>
                             <packageSources>
                               <clear />
                               <add key="local" value="{GetNugetsDir()}" />
                               <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
                             </packageSources>
                           </configuration>
                           """;
        await File.WriteAllTextAsync(Path.Combine(tempDir, "nuget.config"), nugetConfig);

        // Create a minimal csproj
        var csproj = $"""
                      <Project Sdk="Microsoft.NET.Sdk">
                        <PropertyGroup>
                          <TargetFramework>net8.0</TargetFramework>
                          <OutputType>Library</OutputType>
                        </PropertyGroup>
                        <ItemGroup>
                          <PackageReference Include="MarkdownSnippets.MsBuild" Version="{nugetVersion}" PrivateAssets="all" />
                        </ItemGroup>
                      </Project>
                      """;
        await File.WriteAllTextAsync(Path.Combine(tempDir, "TestProject.csproj"), csproj);

        // Create a simple C# file with a snippet
        var csFile = """
                     using System;

                     public class Sample
                     {
                         public void Method()
                         {
                             // begin-snippet: MySnippet
                             var message = "Hello from snippet";
                             Console.WriteLine(message);
                             // end-snippet
                         }
                     }
                     """;
        await File.WriteAllTextAsync(Path.Combine(tempDir, "Sample.cs"), csFile);

        // Initialize git repo (required by MarkdownSnippets)
        await RunProcess("git", "init", tempDir);
        await RunProcess("git", "config user.email \"test@test.com\"", tempDir);
        await RunProcess("git", "config user.name \"Test\"", tempDir);

        // Create docs directory and source.md
        var docsDir = Path.Combine(tempDir, "docs");
        Directory.CreateDirectory(docsDir);

        var sourceMd = """
                       # Test Document

                       <!-- snippet: MySnippet -->
                       <!-- endSnippet -->
                       """;
        await File.WriteAllTextAsync(Path.Combine(docsDir, "readme.source.md"), sourceMd);
    }

    static string GetLatestNugetVersion()
    {
        var packages = Directory.GetFiles(GetNugetsDir(), "MarkdownSnippets.MsBuild.*.nupkg")
            .Select(Path.GetFileNameWithoutExtension)
            .Where(_ => _ != null)
            .Select(_ => _!.Replace("MarkdownSnippets.MsBuild.", ""))
            .OrderByDescending(_ => _)
            .FirstOrDefault();

        return packages ?? throw new InvalidOperationException("No MarkdownSnippets.MsBuild nuget found. Run Release build first.");
    }

    static string? FindMsBuild()
    {
        // Try to find msbuild.exe via vswhere
        var programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
        var vswherePath = Path.Combine(programFiles, "Microsoft Visual Studio", "Installer", "vswhere.exe");

        if (!File.Exists(vswherePath))
        {
            return null;
        }

        var process = Process.Start(
            new ProcessStartInfo
            {
                FileName = vswherePath,
                Arguments = @"-latest -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            })!;

        process.WaitForExit();
        var msbuildPath = process.StandardOutput.ReadToEnd().Trim().Split('\n').FirstOrDefault()?.Trim();

        if (string.IsNullOrEmpty(msbuildPath) || !File.Exists(msbuildPath))
        {
            return null;
        }

        return msbuildPath;
    }

    static async Task<ProcessResult> RunProcess(string fileName, string arguments, string workingDirectory)
    {
        var psi = new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = arguments,
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(psi)!;
        var output = await process.StandardOutput.ReadToEndAsync();
        var error = await process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();

        return new(process.ExitCode, output, error);
    }

    record ProcessResult(int ExitCode, string Output, string Error);
}
#endif
