public class PathsTests
{
    [Test]
    [Arguments("file.md", true)]
    [Arguments("file.mdx", true)]
    [Arguments("file.MD", true)]
    [Arguments("file.MDX", true)]
    [Arguments("file.Md", true)]
    [Arguments("file.txt", false)]
    [Arguments("file.mdxx", false)]
    public async Task IsMdFile(string value, bool expected) =>
        await Assert.That(value.IsMdFile()).IsEqualTo(expected);

    [Test]
    [Arguments("file.source.md", true)]
    [Arguments("file.source.mdx", true)]
    [Arguments("file.SOURCE.MD", true)]
    [Arguments("file.Source.Mdx", true)]
    [Arguments("file.md", false)]
    [Arguments("file.mdx", false)]
    public async Task IsSourceMdFile(string value, bool expected) =>
        await Assert.That(value.IsSourceMdFile()).IsEqualTo(expected);

    [Test]
    [Arguments("file.include.md", true)]
    [Arguments("file.INCLUDE.MD", true)]
    [Arguments("file.Include.Md", true)]
    [Arguments("file.include.mdx", false)]
    [Arguments("file.md", false)]
    public async Task IsIncludeMdFile(string value, bool expected) =>
        await Assert.That(value.IsIncludeMdFile()).IsEqualTo(expected);

    [Test]
    [Arguments("CLAUDE.md", true)]
    [Arguments("claude.md", true)]
    [Arguments("Claude.Md", true)]
    [Arguments("dir/CLAUDE.md", true)]
    [Arguments("C:/repo/CLAUDE.md", true)]
    [Arguments("claudez.md", false)]
    [Arguments("CLAUDE.source.md", true)]
    [Arguments("CLAUDE.local.md", true)]
    [Arguments("claude.include.md", true)]
    [Arguments("CLAUDE.txt", false)]
    [Arguments("AGENTS.md", true)]
    [Arguments("agents.source.md", true)]
    [Arguments("GEMINI.md", true)]
    [Arguments(".github/copilot-instructions.md", true)]
    [Arguments("csharp.instructions.md", true)]
    [Arguments("review.prompt.md", true)]
    [Arguments("plan.chatmode.md", true)]
    [Arguments("reviewer.agent.md", true)]
    [Arguments(".cursorrules", true)]
    [Arguments(".windsurfrules", true)]
    [Arguments("agentsz.md", false)]
    [Arguments("instructions.md", false)]
    [Arguments("file.md", false)]
    public async Task IsAgentFile(string value, bool expected) =>
        await Assert.That(value.IsAgentFile()).IsEqualTo(expected);
}
